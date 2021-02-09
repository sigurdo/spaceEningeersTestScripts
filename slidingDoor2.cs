/*
 * Airlock groups are found by the pattern [Groupname] [airlockMarkerString] [interiorSuffix/exteriorSuffix]
 * for example:
 *   Cool Spacespation Airlock interior
 *   Cool Spacespation Airlock exterior
 * will result in the group Cool Spacespation with 2 doors
 *
 * every group needs atleast one interior and exterior door
 */

private const string airlockMarkerString = "airlock";
private const string interiorSuffix = "interior";
private const string exteriorSuffix = "exterior";

// the refreshrate of the Airlockgroups
// default 10sec
private const int setupRefreshInSec = 10;

// how long a door stays open
// default 5sec
private const int doorOpenTimeInSec = 5;

/*
 * Please do not change anything past this point
 */

private string internalAirlockMarkerString;
private string internalInteriorSuffix;
private string internalExteriorSuffix;
private bool isSetup = false;
private bool isSettingUp = false;
private long setupTimeStamp;
private Dictionary<string, AirlockGroup> airlocks = new Dictionary<string, AirlockGroup>();

private TimeSpan setupRefresh = new TimeSpan(0, 0, 10);
private TimeSpan doorOpenTimeSpan = new TimeSpan(0, 0, 5);
private TimeSpan TimeSinceUpdate => new TimeSpan(DateTime.Now.Ticks - setupTimeStamp);

public Program()
{
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
	internalAirlockMarkerString = string.IsNullOrWhiteSpace(airlockMarkerString) ? "airlock" : airlockMarkerString.ToLower();
	internalInteriorSuffix = string.IsNullOrWhiteSpace(interiorSuffix) ? "interior" : interiorSuffix.ToLower();
	internalExteriorSuffix = string.IsNullOrWhiteSpace(exteriorSuffix) ? "exterior" : exteriorSuffix.ToLower();
	if (setupRefreshInSec > 0) setupRefresh = new TimeSpan(0, 0, setupRefreshInSec);
	if (doorOpenTimeInSec > 0) doorOpenTimeSpan = new TimeSpan(0, 0, doorOpenTimeInSec);
}

public void Main(string argument, UpdateType updateSource)
{
	if (isSettingUp)
		return;

	if (!this.isSetup || TimeSinceUpdate > this.setupRefresh)
	{
		isSettingUp = true;
		Setup();
		setupTimeStamp = DateTime.Now.Ticks;
		isSetup = true;
		isSettingUp = false;
		return;
	}

	UpdateDoors();
}

private void UpdateDoors()
{
	Echo($"time since update: {TimeSinceUpdate:g}");

	foreach (var airlock in airlocks)
	{
		var header = $"==={airlock.Key}===";
		Echo(header);
		Echo(airlock.Value.ToString());
		airlock.Value.Update();

		Echo(new string('=', header.Length));
	}
}

private void Setup()
{
	var allAirlockDoors = new List<IMyDoor>();
	GridTerminalSystem.GetBlocksOfType(
		allAirlockDoors,
		block =>
			block.CubeGrid == Me.CubeGrid // only own grid
			&& block is IMyDoor // only doors
			&& block.CustomName.ToLower().Contains(internalAirlockMarkerString)); // only with airlock in name

	if (allAirlockDoors.Count == 0)
	{
		Echo("no Airlocks");
		return;
	}

	var oldDoors = new Dictionary<long, TimableDoor>();
	foreach (var airlockGroup in airlocks.Values)
		foreach (var door in airlockGroup.AllDoors)
			oldDoors.Add(door.Id, door);

	airlocks.Clear();

	foreach (var door in allAirlockDoors)
	{
		var timableDoor = GetDoor(oldDoors, door);
		if (door.CustomName.ToLower().Contains(internalInteriorSuffix))
			AddAsInterior(timableDoor);
		else if (door.CustomName.ToLower().Contains(internalExteriorSuffix))
			AddAsExterior(timableDoor);
	}
}

private TimableDoor GetDoor(Dictionary<long, TimableDoor> oldDoors, IMyDoor door)
{
	TimableDoor timableDoor;
	if (oldDoors.ContainsKey(door.EntityId))
		timableDoor = oldDoors[door.EntityId];
	else
		timableDoor = new TimableDoor(door, doorOpenTimeSpan);
	return timableDoor;
}

private void AddAsInterior(TimableDoor door) => GetAirlockGroup(door.Name).AddAsInterior(door);

private void AddAsExterior(TimableDoor door) => GetAirlockGroup(door.Name).AddAsExterior(door);

private AirlockGroup GetAirlockGroup(string doorName)
{
	var name = doorName.ToLower().Replace(" ", string.Empty);
	var groupName = name.Substring(0, name.IndexOf(internalAirlockMarkerString));
	if (!airlocks.ContainsKey(groupName))
	{
		airlocks.Add(groupName, new AirlockGroup());
	}

	return airlocks[groupName];
}

public class TimableDoor
{
	private IMyDoor door;
	private bool oldIsOpen;
	private long opendTimestamp;
	private TimeSpan doorOpenTimeSpan = new TimeSpan(0, 0, 5);

	public TimableDoor(IMyDoor door, TimeSpan doorOpenTimeSpan)
	{
		this.door = door;
		this.doorOpenTimeSpan = doorOpenTimeSpan;
		oldIsOpen = IsCurrentlyOpen;
		if (IsCurrentlyOpen)
			opendTimestamp = DateTime.Now.Ticks;
	}

	public long Id => door.EntityId;
	public string Name => door.CustomName;
	public bool IsCurrentlyOpen => door.Status != DoorStatus.Closed;

	public void Update()
	{
		if (!door.IsWorking) return;

		if (!IsCurrentlyOpen)
		{
			oldIsOpen = false;
			return;
		}

		if (oldIsOpen)
		{
			// was open for longer than x sec
			if (doorOpenTimeSpan < new TimeSpan(DateTime.Now.Ticks - opendTimestamp))
				Close();
		}
		else
		{
			oldIsOpen = true;
			opendTimestamp = DateTime.Now.Ticks;
		}
	}

	public void Unlock()
	{
		if (!door.IsWorking) door.Enabled = true;
	}

	public void Close()
	{
		if (!door.IsWorking)
			door.Enabled = true;

		door.CloseDoor();
	}

	public void CloseOrLock()
	{
		if (!IsCurrentlyOpen)
		{
			door.Enabled = false;
			return;
		}

		Close();
	}
}

public class AirlockGroup
{
	private List<TimableDoor> interiorList = new List<TimableDoor>();
	private List<TimableDoor> exteriorList = new List<TimableDoor>();

	public List<TimableDoor> AllDoors
	{
		get
		{
			var list = new List<TimableDoor>();
			list.AddRange(interiorList);
			list.AddRange(exteriorList);
			return list;
		}
	}

	public void AddAsInterior(TimableDoor door) => interiorList.Add(door);

	public void AddAsExterior(TimableDoor door) => exteriorList.Add(door);

	public void Update()
	{
		if (interiorList.Count == 0 || exteriorList.Count == 0)
			return;

		var interiorOpen = false;
		var exteriorOpen = false;

		foreach (var door in interiorList)
		{
			door.Update();
			if (!door.IsCurrentlyOpen) continue;

			interiorOpen = true;
		}

		foreach (var door in exteriorList)
		{
			door.Update();
			if (!door.IsCurrentlyOpen) continue;

			exteriorOpen = true;
		}

		if (interiorOpen && exteriorOpen)
		{
			CloseAllDoors();
			return;
		}

		if (interiorOpen)
			CloseOrLock(exteriorList);
		else
			Unlock(exteriorList);

		if (exteriorOpen)
			CloseOrLock(interiorList);
		else
			Unlock(interiorList);
	}

	private void Unlock(List<TimableDoor> doorList) => doorList.ForEach(door => door.Unlock());

	private void CloseOrLock(List<TimableDoor> doorList) => doorList.ForEach(door => door.CloseOrLock());

	private void CloseAllDoors()
	{
		interiorList.ForEach(door => door.Close());
		exteriorList.ForEach(door => door.Close());
	}

	public override string ToString() => $"interior: {interiorList.Count}{Environment.NewLine}exterior: {exteriorList.Count}";
}