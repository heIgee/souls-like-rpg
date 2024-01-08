using System;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize;
    //public bool Stackable { get; private set; }

    public InventoryItem(ItemData newItemData)
    {
        data = newItemData;
        AddStack();
    }

    //private void Awake()
    //{
    //    CheckForStackable();
    //}

    //private void CheckForStackable()
    //{
    //    if (data is not EquipmentData newEquipmentData)
    //    {
    //        Stackable = true;
    //        return;
    //    }

    //    Stackable = newEquipmentData.equipmentType switch
    //    {
    //        EquipmentType.Weapon or EquipmentType.Armor => false,
    //        EquipmentType.Flask or EquipmentType.Amulet => true,
    //        _ => true,
    //    };
    //}

    public void AddStack() => stackSize++;

    public void AddStacks(int stackAmount) => stackSize += stackAmount;

    public void RemoveStack() => stackSize--;

    public void RemoveStacks(int stackAmount) => stackSize -= stackAmount;
}
