using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.Windows;

public class Inventory : MonoBehaviour, ISaveManager
{
    public static Inventory instance;

    public List<ItemData> startingItems = new();

    // we use dictionaries to conveniently access slots with specific item types
    // leaving equipmentDict for now
    // it would be great to have stackable and non-stackable equipment atst

    public List<InventoryItem> equipment = new();
    public Dictionary<EquipmentData, InventoryItem> equipmentDict = new();

    public List<InventoryItem> inventory = new();
    //public Dictionary<ItemData, InventoryItem> inventoryDict = new();

    public List<InventoryItem> stash = new();
    public Dictionary<ItemData, InventoryItem> stashDict = new();


    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;
    [SerializeField] private Transform statSlotParent;

    private ItemSlotUI[] inventoryItemSlots;
    private ItemSlotUI[] stashItemSlots;
    private EquipmentSlotUI[] equipmentItemSlots;
    private StatSlotUI[] statSlots;

    [Header("Database")]
    public string[] assetIDs;
    public List<ItemData> itemDB;

    public List<InventoryItem> loadedItems;
    public List<EquipmentData> loadedEquipment;

    public float flaskCooldown;

    public bool CanAddItemToInventory
    {
        get
        {
            if (inventory.Count >= inventoryItemSlots.Length)
            {
                Debug.LogWarning("Inventory is full. Cannot add item");
                return false;
            }

            return true;
        }
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        inventoryItemSlots = inventorySlotParent.GetComponentsInChildren<ItemSlotUI>();
        stashItemSlots = stashSlotParent.GetComponentsInChildren<ItemSlotUI>();
        equipmentItemSlots = equipmentSlotParent.GetComponentsInChildren<EquipmentSlotUI>();
        statSlots = statSlotParent.GetComponentsInChildren<StatSlotUI>();

        AddStartingItems();

        UpdateInventoryUI();
    }

    private void AddStartingItems()
    {
        if (loadedItems.Count <= 0)
        {
            foreach (var itemData in startingItems)
                AddItem(itemData);
        }
        else
        {
            foreach (var inventoryItem in loadedItems)
                for (int i = 0; i < inventoryItem.stackSize; i++)
                    AddItem(inventoryItem.data);
        }

        if (loadedEquipment.Count > 0)
        {
            foreach (var equipmentItem in loadedEquipment)
                EquipItem(equipmentItem);

            // essential to update curhp to maxhp after loading data
            var stats = PlayerManager.instance.player.Stats;
            stats.CurrentHp = stats.maxHp.Value;
            stats.onHealthChanged?.Invoke();
        }
    }

    public void UpdateInventoryUI()
    {
        // TODO: performance? ah
        foreach (var slot in equipmentItemSlots)
            slot.RevertSlot();

        foreach (var slot in inventoryItemSlots)
            slot.RevertSlot();

        foreach (var slot in stashItemSlots)
            slot.RevertSlot();

        foreach (var slot in equipmentItemSlots)
        {
            // I think its more efficient (not displays items)
            //var matchingItem = equipmentDict.FirstOrDefault
            //    (item => item.Key.itemType.Equals(slot.slotType));
            //if (matchingItem.Value != null)
            //    slot.UpdateSlot(matchingItem.Value);

            // than this
            foreach (KeyValuePair<EquipmentData, InventoryItem> item in equipmentDict)
            {
                if (slot.slotType == item.Key.equipmentType)
                    slot.UpdateSlot(item.Value);
            }
        }

        for (int i = 0; i < inventory.Count; i++)
        {
            //Debug.LogWarning($"{i}: {inventoryItemSlots[i]} - {inventory[i]}");
            inventoryItemSlots[i].UpdateSlot(inventory[i]);
        }

        for (int i = 0; i < stash.Count; i++)
            stashItemSlots[i].UpdateSlot(stash[i]);

        UpdateStatsUI();
    }

    public void UpdateStatsUI()
    {
        foreach (StatSlotUI slot in statSlots)
            slot.UpdateStatValueUI();
    }

    public void EquipItem(ItemData itemData)
    {
        EquipmentData newEquipmentData = itemData as EquipmentData;
        var newItem = new InventoryItem(newEquipmentData);

        EquipmentData oldEquipment = null;
        foreach (KeyValuePair<EquipmentData, InventoryItem> item in equipmentDict)
        {
            if (item.Key.equipmentType == newEquipmentData.equipmentType)
                oldEquipment = item.Key;
        }

        if (oldEquipment != null)
        {
            UnequipItem(oldEquipment);
            AddToInventory(oldEquipment);
        }

        RemoveItem(itemData);
        equipment.Add(newItem);
        equipmentDict.Add(newEquipmentData, newItem);

        newEquipmentData.AddModifiers();

        //int index = equipment.IndexOf(newItem);
        //Debug.LogWarning($"Equip add {index}: {equipment[index].data.name}");

        UpdateInventoryUI();
    }
    public void UnequipItem(EquipmentData equipmentToRemove)
    {
        if (!equipmentDict.TryGetValue(equipmentToRemove, out InventoryItem value))
            return;

        equipment.Remove(value);
        equipmentDict.Remove(equipmentToRemove);
        equipmentToRemove.RemoveModifiers();
    }



    public void AddItem(ItemData itemData)
    {
        if (itemData == null)
            return;

        switch (itemData.itemType)
        {
            case ItemType.Material:
                AddToStash(itemData); break;

            case ItemType.Equipment:
                if (CanAddItemToInventory)
                    AddToInventory(itemData); break;

            default: break; 
        }

        UpdateInventoryUI();
    }
    private void AddToInventory(ItemData itemData)
    {
        // stacking inventory items? better not to
        //if (inventoryDict.TryGetValue(itemData, out InventoryItem value))
        //{
        //    value.AddStack();
        //}
        //else
        {
            var newItem = new InventoryItem(itemData);
            inventory.Add(newItem);
            //inventoryDict.Add(itemData, newItem);

            //int index = inventory.IndexOf(newItem);
            //Debug.LogWarning($"Inventory add {index}: {inventory[index].data.name} - {inventoryItemSlots[index]}");
        }
    }

    private void AddToStash(ItemData itemData)
    {
        if (stashDict.TryGetValue(itemData, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            var newItem = new InventoryItem(itemData);
            stash.Add(newItem);
            stashDict.Add(itemData, newItem);
        }
    }

    public void RemoveItem(ItemData itemData, int amount = 1)
    {
        // idk where I pass null data, but this check is crucial
        if (itemData == null)
        {
            return;
        }

        switch (itemData.itemType)
        {
            case ItemType.Material:
                TryRemoveFromStash(itemData, amount); break;

            case ItemType.Equipment:
                TryRemoveFromInventory(itemData, amount); break;

            default: break;
        }

        UpdateInventoryUI();
    }
    private bool TryRemoveFromInventory(ItemData itemData, int amount)
    {
        //if (inventoryDict.TryGetValue(item, out InventoryItem inventoryValue))
        var itemToRemove = inventory.Where(item => item.data == itemData).FirstOrDefault();

        if (itemToRemove != null)
        {
            inventory.Remove(itemToRemove);

            //Debug.LogWarning($"Item: {item}, stack: {inventoryValue.stackSize}");

            //inventoryValue.RemoveStacks(amount);

            //if (inventoryValue.stackSize < 1)
            //{
            //    inventory.Remove(inventoryValue);
            //    inventoryDict.Remove(itemData);
            //}

            return true;
        }

        return false;
    }

    private bool TryRemoveFromStash(ItemData item, int amount)
    {
        if (stashDict.TryGetValue(item, out InventoryItem stashValue))
        {
            stashValue.RemoveStacks(amount);

            if (stashValue.stackSize < 1)
            {
                stash.Remove(stashValue);
                stashDict.Remove(item);
            }

            return true;
        }

        return false;
    }

    public bool AttemptCraft(EquipmentData equipmentToCraft, List<InventoryItem> requiredMaterials)
    {
        if (requiredMaterials.Count == 0)
            Debug.LogWarning("Equipment to craft does not have any required materials. Check it out");

        List<InventoryItem> materialsToUse = new();

        foreach (InventoryItem requiredItem in requiredMaterials)
        {
            if (!stashDict.TryGetValue(requiredItem.data, out InventoryItem stashValue)
              || stashValue.stackSize < requiredItem.stackSize)
            {
                Debug.LogWarning("Insufficient materials");
                return false;
            }
            else
            {
                materialsToUse.Add(stashValue);
            }
        }

        foreach (var materialToUse in materialsToUse)
            RemoveItem(materialToUse.data, materialToUse.stackSize);

        Debug.LogWarning($"Item crafted: {equipmentToCraft.name}");
        AddItem(equipmentToCraft);

        UpdateInventoryUI();

        return true;
    }

    public bool TryGetEquipment(EquipmentType type, out EquipmentData equippedItem)
    {
        foreach (KeyValuePair<EquipmentData, InventoryItem> item in equipmentDict)
        {
            if (item.Key.equipmentType == type)
            {
                equippedItem = item.Key;
                return true;
            }
        }

        equippedItem = null;
        return false;
    }

    public bool AttemptUseFlask()
    {
        if (!TryGetEquipment(EquipmentType.Flask, out var flask))
            return false;

        flaskCooldown = flask.itemCooldown;
        UI.instance.GetComponentInChildren<InGameUI>().flaskCooldown = flaskCooldown;

        if (Time.time > flask.lastTimeUsed + flask.itemCooldown)
        {
            flask.ExecuteEffects();
            flask.lastTimeUsed = Time.time;
            return true;
        }
        else
        {
            Debug.LogWarning($"{flask.name} on cooldown");
            return false;
        }
    }

    public bool AttemptUseArmor()
    {
        if (!TryGetEquipment(EquipmentType.Armor, out var armor))
            return false;

        if (Time.time > armor.lastTimeUsed + armor.itemCooldown)
        {
            armor.lastTimeUsed = Time.time;
            return true;
        }
        else
        {
            Debug.LogWarning($"{armor.name} on cooldown");
            return false;
        }
    }

    public void LoadData(GameData data)
    {
        itemDB = GetItemDB();

        foreach (KeyValuePair<string, int> kvp in data.stash)
        {
            foreach (var item in itemDB)
            {
                if (item != null && item.itemID == kvp.Key)
                {
                    InventoryItem itemToLoad = new(item)
                    {
                        stackSize = kvp.Value
                    };

                    loadedItems.Add(itemToLoad);
                }
            }
        }

        Debug.Log("Stash loaded (apparently)");

        foreach (string inventoryID in data.inventory)
        {
            foreach (var item in itemDB)
            {
                if (item != null && item.itemID == inventoryID)
                {
                    InventoryItem itemToLoad = new(item);
                    loadedItems.Add(itemToLoad);
                }
            }
        }

        Debug.Log("Inventory loaded (apparently)");

        foreach (string equipmentID in data.equipment)
        {
            foreach (var item in itemDB)
            {
                if (item != null && item.itemID == equipmentID)
                {
                    loadedEquipment.Add(item as EquipmentData);
                }
            }
        }

        Debug.Log("Equipment loaded (apparently)");
    }

    public void SaveData(GameData data)
    {
        data.equipment.Clear();
        data.inventory.Clear();
        data.stash.Clear();

        foreach (InventoryItem item in inventory)
        {
            data.inventory.Add(item.data.itemID);
        }

        foreach (KeyValuePair<ItemData, InventoryItem> kvp in stashDict)
        {
            data.stash.Add(kvp.Key.itemID, kvp.Value.stackSize); 
        }

        foreach (KeyValuePair<EquipmentData, InventoryItem> kvp in equipmentDict)
        {
            data.equipment.Add(kvp.Key.itemID);
        }

        Debug.Log("Inventory saved");
    }

    private List<ItemData> GetItemDB()
    {
        itemDB = new List<ItemData>();

        string itemsPath = "Assets/Data/Items";

        if (!Directory.Exists(itemsPath))
        {
            Debug.LogError($"Directory {equipment} does not exist");
            return null;
        }

        assetIDs = AssetDatabase.FindAssets(string.Empty, new[] { itemsPath });

        foreach (string SOName in assetIDs)
        {
            string SOPath = AssetDatabase.GUIDToAssetPath(SOName);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(SOPath);

            itemDB.Add(itemData);
        }

        return itemDB;
    }
}
