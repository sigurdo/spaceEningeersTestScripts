

public const int counterMAX = 2;
public int counter = counterMAX;
public IMyAirtightSlideDoor DoorInne;
public IMyAirtightSlideDoor DoorUte;
public Door DoorOBJInne;
public Door DoorOBJUte;

public Program()
{
	Runtime.UpdateFrequency = UpdateFrequency.Update10;
    DoorInne = GridTerminalSystem.GetBlockWithName("DoorInne") as IMyAirtightSlideDoor;
    DoorUte = GridTerminalSystem.GetBlockWithName("DoorUte") as IMyAirtightSlideDoor;
    DoorOBJInne = new Door(DoorInne);
    DoorOBJUte = new Door(DoorUte);

    DoorOBJInne.SettAnnenDoor(DoorOBJUte);
    DoorOBJUte.SettAnnenDoor(DoorOBJInne);
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
    DoorOBJInne.Sjekk();
    DoorOBJUte.Sjekk();
}

public class Door {
    private IMyAirtightSlideDoor megDoor;
    private Door annenDoor;
    private int counter = 10;
    private bool blirAApnet = false;

    public Door(IMyAirtightSlideDoor megDoor){
		this.megDoor = megDoor;
	}

    public void SettAnnenDoor(Door a){
        annenDoor = a;
    }

    public void Sjekk(){
        if (blirAApnet){
            counter--;
            if (counter == 10){
                megDoor.OpenDoor();
            }
            else if (counter < 0){
                megDoor.CloseDoor();
                counter = 10; 
                blirAApnet = false;
            }
        }
		else if (megDoor.Open && !blirAApnet){
            if (counter < 0){
                //Åpne den andre
                megDoor.CloseDoor();
                annenDoor.AApne();
                counter = 10; 
            } else {
                counter--;
            }
        }
	}
    public void AApne(){
        blirAApnet = true;
        counter = 20;
    }
}
