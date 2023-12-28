using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    public InventoryItem item;

    protected void Awake()
    {
        itemText.text = "";
        itemImage.color = Color.clear;
    }

    public void UpdateSlot(InventoryItem newItem)
    {
        item = newItem;

        if (newItem == null || newItem.stackSize <= 0)
        {
            itemImage.color = Color.clear;
            itemImage.sprite = null;
            item = null;
            return;
        }

        // white means displaying full-bright icon
        itemImage.color = Color.white;
        itemImage.sprite = newItem.data.icon;

        if (newItem.stackSize > 1)
            itemText.text = newItem.stackSize.ToString();
        else
            itemText.text = "";
    }

    public void CleanUpSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemImage.color = Color.clear;

        itemText.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null || item.data == null)
            return;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }

        if (item.data.itemType == ItemType.Equipment)
            Inventory.instance.EquipItem(item.data);
    }
}