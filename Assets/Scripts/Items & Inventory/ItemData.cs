using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "New item data", menuName = "Item Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType = ItemType.Material;   
    public string itemName;
    public Sprite icon;

    [Range(0, 100)]
    public int dropChance;

    protected StringBuilder sb = new();
    public virtual string GetDescription() => "";
}
