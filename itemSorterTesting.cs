

public IMyTextPanel display;
public IMyTerminalBlock block;
public IMyTerminalBlock toBlock;
public IMyInventory toInventory;
public MyItemType itemType;

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
        block = GridTerminalSystem.GetBlockWithName("Large Cargo Container hoved") as IMyTerminalBlock;
        toBlock = GridTerminalSystem.GetBlockWithName("tilTest") as IMyTerminalBlock;
        toInventory = toBlock.GetInventory(0);
    }
    catch (Exception e) {
        return;
    }
    
    // IMyTextPanel.WriteText(string text, bool append);
    display.WriteText("Hello sorting\n", false);
    display.ShowPublicTextOnScreen();

    // Get all blocks in grid
    // List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    // GridTerminalSystem.GetBlocks(blocks);
    // // Loop all blocks
    // foreach (IMyTerminalBlock block in blocks) {
    //     // Loop all inventories in block


    // }
    itemType = MyItemType.MakeComponent("SolarCell");
    display.WriteText("Funka dette?"+itemType.ToString()+"\n", true);

    for (int i = 0; i < block.InventoryCount; i++) {
        IMyInventory inventory = block.GetInventory(i);
        // display.WriteText("i:"+i+", itemcount:"+inventory.ItemCount+"\n", true);

        // Loop all items in inventory:
        // for (int j = 0; j < inventory.ItemCount; j++) {
        //     MyInventoryItem? item = inventory.GetItemAt(j);
        //     display.WriteText("item: "+item.ToString()+", j = "+j+"\n", true);
        // }
        MyInventoryItem item = (MyInventoryItem) inventory.FindItem(itemType);
        inventory.TransferItemTo(toInventory, item, 1);
        display.WriteText("amount:"+item.Amount+"\n");
    }

}
