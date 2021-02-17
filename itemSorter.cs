

public int counter = 0;
public IMyTextPanel display;

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
        display = GridTerminalSystem.GetBlockWithName("LCD Panel 2") as IMyTextPanel;
    }
    catch (Exception e) {
        return;
    }
    display.ShowPublicTextOnScreen();
    // IMyTextPanel.WriteText(string text, bool append);
    display.WriteText("", false);
    counter++;
    if (counter == 100) {
        counter = 0;

        // Get all blocks in grid
        List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
        GridTerminalSystem.GetBlocks(blocks);

        // Loop all blocks
        foreach (IMyTerminalBlock block in blocks) {
            // Loop all inventories in block
            for (int i = 0; i < block.InvetoryCount; i++) {
                IMyInventory inventory = block.GetInventory(i);

                // Loop all items in inventory:
                for (int j = 0; j < inventory.ItemCount; j++) {
                    MyInventoryItem item = inventory.GetItemAt(j);
                    display.WriteText("item: "+item.ToString()+", j = "+j+"\n", true);
                }
            }
        }
    }
}

enum SortingStrategy {
    OneTarget,
    EqualBetweenAll,
    EqualBetweenAllLargeCargoContainers,
    EqualBetweenAllCargoContainers,
    EqualBetweenList
}

class ItemTypeManagementConfig {
    MyItemType itemType;

    int minimumTotalAmount;
    int maximumTotalAmount;

    SortingStrategy sortingStrategy;
}
