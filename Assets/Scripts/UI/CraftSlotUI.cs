using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftSlotUI : ItemSlotUI
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        EquipmentData craftData = item.data as EquipmentData;
        if (Inventory.instance.AttemptCraft(craftData, craftData.craftingMaterials))
        {
            // playe some sound
        }
    }

    private void OnEnable()
    {
        UpdateSlot(item);
    }
}
