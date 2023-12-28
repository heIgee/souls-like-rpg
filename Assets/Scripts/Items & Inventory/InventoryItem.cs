using System;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize;

    public InventoryItem(ItemData newItemData)
    {
        data = newItemData;
        AddStack();
    }

    public void AddStack() => stackSize++;

    public void AddStacks(int stackAmount) => stackSize += stackAmount;

    public void RemoveStack() => stackSize--;

    public void RemoveStacks(int stackAmount) => stackSize -= stackAmount;
}
