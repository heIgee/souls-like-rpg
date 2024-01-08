using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player drop")]
    [SerializeField] private float equipmentLoseChance;
    [SerializeField] private float inventoryLoseChance;
    [SerializeField] private float stashLoseChance;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;

        // ToList() is essential to create copy
        // or else we'd get modified collection exception in foreach
        List<InventoryItem> currentEquipment = inventory.equipment.ToList();
        List<InventoryItem> currentInventory = inventory.inventory.ToList();
        List<InventoryItem> currentStash = inventory.stash.ToList();
         
        if (equipmentLoseChance > 0)
        {
            foreach (var item in currentEquipment)
            {
                if (Random.Range(0, 100) < equipmentLoseChance)
                {
                    DropItem(item.data);
                    inventory.UnequipItem(item.data as EquipmentData);
                }
            }
        }

        if (inventoryLoseChance > 0)
        {
            foreach (var item in currentInventory)
            {
                // chance to lose something in stack at all
                if (Random.Range(0, 100) < inventoryLoseChance)
                {
                    int loseAmount = 0;
                    for (int i = 0; i < item.stackSize; i++)
                    {
                        // chance to lose individual stack item 
                        if (Random.Range(0, 100) < inventoryLoseChance)
                            loseAmount++;
                    }

                    DropItem(item.data, loseAmount);
                    inventory.RemoveItem(item.data, loseAmount);
                }
            }
        }

        if (stashLoseChance > 0)
        {
            foreach (var item in currentStash)
            {
                if (Random.Range(0, 100) < stashLoseChance)
                {
                    int loseAmount = 0;
                    for (int i = 0; i < item.stackSize; i++)
                    {
                        if (Random.Range(0, 100) < stashLoseChance)
                            loseAmount++;
                    }

                    DropItem(item.data, loseAmount);
                    // fun fact, I accindetaly left 'as EquipmentData' here,
                    // and data was passed as null for some reason
                    inventory.RemoveItem(item.data, loseAmount);
                }
            }
        }

        inventory.UpdateInventoryUI();
    }
}
