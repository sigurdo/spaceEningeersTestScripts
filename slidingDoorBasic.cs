

public int counter = 0;
public IMyAirtightSlideDoor Door;

public enum State {
    None,
    Counting
}
public State state = State.None;

public Program()
{
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
    try {
        Door = GridTerminalSystem.GetBlockWithName("Door") as IMyAirtightSlideDoor;
    }
    catch (Exception e) {
        return;
    }
    // IMyTextPanel display = GridTerminalSystem.GetBlockWithName("LCD Panel 2") as IMyTextPanel;
    // display.WriteText(directionState.ToString()+counter.ToString(), false);
    // display.ShowPublicTextOnScreen();
    counter++;
    switch (state) {
        case (State.None):
            if (Door.Open) {
                counter = 0;
                state = State.Counting;
            }
            break;
        case (State.Counting):
            switch (counter) {
                case 24:
                    Door.CloseDoor();
                    break;
                case 36:
                    state = State.None;
                    break;
            }
            break;
    }
}
