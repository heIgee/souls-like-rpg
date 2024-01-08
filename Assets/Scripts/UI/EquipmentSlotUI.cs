using UnityEngine.EventSystems;

public class EquipmentSlotUI : ItemSlotUI
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = $"{slotType}Slot";
    }

    public override void UpdateSlot(InventoryItem newItem)
    {
        base.UpdateSlot(newItem);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.data == null)
            return;

        Inventory.instance.UnequipItem(item.data as EquipmentData);
        Inventory.instance.AddItem(item.data as EquipmentData);

        ui.itemTooltip.HideTooltip();

        Inventory.instance.UpdateInventoryUI();
    }
}
