

public int counter = 0;
public IMyAirtightSlideDoor DoorInne;
public IMyAirtightSlideDoor DoorUte;

public enum DirectionState {
    None,
    In,
    Out
}
public DirectionState directionState = DirectionState.None;

public Program()
{
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
    DoorInne = GridTerminalSystem.GetBlockWithName("DoorInne") as IMyAirtightSlideDoor;
    DoorUte = GridTerminalSystem.GetBlockWithName("DoorUte") as IMyAirtightSlideDoor;
}

public void Save()
{
    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means. 
    // 
    // This method is optional and can be removed if not
    // needed.
}


public void Main(string argument, UpdateType updateSource)
{
    IMyTextPanel display = GridTerminalSystem.GetBlockWithName("LCD Panel 2") as IMyTextPanel;
    display.WriteText(directionState.ToString()+counter.ToString(), false);
    display.ShowPublicTextOnScreen();
    counter++;
    switch (directionState) {
        case (DirectionState.None):
            if (DoorInne.Status == DoorStatus.Open) {
                counter = 0;
                directionState = DirectionState.Out;
            }
            else if (DoorUte.Status == DoorStatus.Open) {
                counter = 0;
                directionState = DirectionState.In;
            }
            break;
        case (DirectionState.Out):
            switch (counter) {
                case 12:
                    DoorInne.CloseDoor();
                    break;
                case 18:
                    DoorUte.OpenDoor();
                    break;
                case 30:
                    DoorUte.CloseDoor();
                    break;
                case 42:
                    directionState = DirectionState.None;
                    break;
            }
            break;
        case (DirectionState.In):
            switch (counter) {
                case 12:
                    DoorUte.CloseDoor();
                    break;
                case 18:
                    DoorInne.OpenDoor();
                    break;
                case 30:
                    DoorInne.CloseDoor();
                    break;
                case 42:
                    directionState = DirectionState.None;
                    break;
            }
            break;
    }
}
