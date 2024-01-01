using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class ItemSlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    protected Sprite defaultItemSprite;
    [SerializeField] protected TextMeshProUGUI itemText;

    public InventoryItem item;
    protected UI ui;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();

        defaultItemSprite = ui.defaultIconSprite;

        // after some testing, it works
        if (itemImage.sprite == null) 
            itemImage.sprite = defaultItemSprite;
    }

    public virtual void UpdateSlot(InventoryItem newItem)
    {
        item = newItem;
        if (newItem == null || newItem.stackSize <= 0)
        {
            RevertSlot();
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

    public void RevertSlot()
    {
        item = null;
        itemImage.sprite = defaultItemSprite;
        itemText.text = "";
    }

    public void CleanSlot()
    {
        item = null;
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
        {
            Inventory.instance.EquipItem(item.data);
            ui.itemTooltip.HideTooltip();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null || item.data == null)
            return;

        //Vector3 pointerPosition = eventData.position;

        float xOffset = -400f;
        float yOffset = 300f;

        // offset from the slot transform is much better and stable
        var tooltipPosition = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, 0f);

        ui.itemTooltip.transform.position = tooltipPosition;

        ui.itemTooltip.ShowTooltip(item.data as EquipmentData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.itemTooltip.HideTooltip();
    }
}