

public const int counterMAX = 2;
public int counter = counterMAX;
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
	Runtime.UpdateFrequency = UpdateFrequency.Update100;
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
    switch (directionState) {
        case (DirectionState.None):
            if (DoorInne.Open) {
                directionState = DirectionState.Out;
            }
            else if (DoorUte.Open) {
                directionState = DirectionState.In;
            }
            counter = counterMAX;
            break;
        case (DirectionState.Out):
            counter--;
            if (counter == 0){
                DoorInne.CloseDoor();
            } else if (counter < 0){
                DoorUte.OpenDoor();
            } else if (counter < -counterMAX) {
                DoorUte.CloseDoor();
                directionState = DirectionState.None;
            }
            break;
        case (DirectionState.In):
            counter--;
            if (counter == 0){
                DoorUte.CloseDoor();
            } else if (counter < 0){
                DoorInne.OpenDoor();
            } else if (counter < -counterMAX) {
                DoorInne.CloseDoor();
                directionState = DirectionState.None;
            }
            break;
    }
}
