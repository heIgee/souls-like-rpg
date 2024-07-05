using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New item data", menuName = "Item Data/Item")]
public class ItemData : ScriptableObject
{
    public ItemType itemType = ItemType.Material;   
    public string itemName;
    public Sprite icon;
    public string itemID;

    [Range(0, 100)]
    public int dropChance;

    private void OnValidate()
    {
#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        itemID = AssetDatabase.AssetPathToGUID(path);
#endif
    }

    protected StringBuilder sb = new();
    public virtual string GetDescription() => "";
}
