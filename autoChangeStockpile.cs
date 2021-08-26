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
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
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
    // The main entry point of the script, invoked every time
    // one of the programmable block's Run actions are invoked,
    // or the script updates itself. The updateSource argument
    // describes where the update came from.
    // 
    // The method itself is required, but the arguments above
    // can be removed if not needed.
    IMyCockpit controlSeat = GridTerminalSystem.GetBlockWithName("Control Seat 2") as IMyCockpit;
    IMyTextSurface display = controlSeat.GetSurface(0);
    IMyShipConnector connector = GridTerminalSystem.GetBlockWithName("Connector 2") as IMyShipConnector;
    IMyGasTank hydrogentank = GridTerminalSystem.GetBlockWithName("Hydrogen Tank 2") as IMyGasTank;
    if (connector.Status == MyShipConnectorStatus.Connected) {
        hydrogentank.Stockpile = true;
        display.WriteText("Stockpile on", false);
    }
    else {
        hydrogentank.Stockpile = false;
        display.WriteText("Stockpile off", false);
    }
    // display.ShowPublicTextOnScreen();
}
