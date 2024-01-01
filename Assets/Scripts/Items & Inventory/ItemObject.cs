using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private Rigidbody2D rb;

    private void OnValidate()
    {
        if (itemData == null)
            return;

        LoadItemProperties();
    }

    private void LoadItemProperties()
    {
        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item: " + itemData.name;
    }

    public void SetupItem(ItemData itemData, Vector2 velocity)
    {
        this.itemData = itemData;
        LoadItemProperties();

        rb.velocity = velocity;
    }

    public void PickUp()
    {
        if (itemData.itemType is ItemType.Equipment && !Inventory.instance.CanAddItemToInventory)
        {
            // pop
            rb.velocity = new Vector2(0, 5);
            return;
        }

        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
