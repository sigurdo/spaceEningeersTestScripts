public Program()
{
    
    // The constructor, called only once every session and
    // always before any other method is called. Use it to
    // initialize your script. 
    //     
    // The constructor is optional and can be removed if not
    // needed.
    // 
    // It's recommended to set RuntimeInfo.UpdateFrequency 
    // here, which will allow your script to run itself without a 
    // timer block.
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
    IMyAirtightSlideDoor DoorInne = GridTerminalSystem.GetBlockWithName("DoorInne") as IMyAirtightSlideDoor;
    IMyAirtightSlideDoor DoorUte = GridTerminalSystem.GetBlockWithName("DoorUte") as IMyAirtightSlideDoor;
    Ingame.IMyTimerBlock DoorTimer = GridTerminalSystem.GetBlockWithName("DoorTimer") as Ingame.IMyTimerBlock;

    if (DoorInne.Open) {
        DoorInne.CloseDoor();
        DoorTimer
        DoorUte.OpenDoor();
    }
    
    DoorInne.ToggleDoor();
}
