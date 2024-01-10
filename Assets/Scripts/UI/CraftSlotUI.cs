using UnityEngine.EventSystems;

public class CraftSlotUI : ItemSlotUI
{
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.data == null)
            return;

        EquipmentData craftData = item.data as EquipmentData;

        ui.craftWindowUI.SetupCraftWindow(craftData);
    }

    protected override void Start()
    {
        base.Start();
    }

    public void SetupCraftSlot(EquipmentData data)
    {
        if (data == null)
            return;

        item = new InventoryItem(data);
        itemImage.sprite = data.icon;

        //Debug.Log(data.name);
        itemText.text = data.name;
        //Debug.Log(itemText.text);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
    }
}
