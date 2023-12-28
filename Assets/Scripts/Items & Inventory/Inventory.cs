using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public List<ItemData> startingItems = new();

    // we use dictionaries to conveniently access slots with specific item types
    public List<InventoryItem> equipment = new();
    public Dictionary<EquipmentData, InventoryItem> equipmentDict = new();

    public List<InventoryItem> inventory = new();
    public Dictionary<ItemData, InventoryItem> inventoryDict = new();

    public List<InventoryItem> stash = new();
    public Dictionary<ItemData, InventoryItem> stashDict = new();


    [Header("Inventory UI")]
    [SerializeField] private Transform inventorySlotParent;
    [SerializeField] private Transform stashSlotParent;
    [SerializeField] private Transform equipmentSlotParent;

    private ItemSlotUI[] inventoryItemSlots;
    private ItemSlotUI[] stashItemSlots;
    private EquipmentSlotUI[] equipmentItemSlots;


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

        AddStartingItems();

        UpdateInventoryUI();
    }

    private void AddStartingItems()
    {
        foreach (var item in startingItems)
            AddItem(item);
    }

    public void UpdateInventoryUI()
    {
        foreach (var slot in equipmentItemSlots)
            slot.CleanUpSlot();

        foreach (var slot in inventoryItemSlots)
            slot.CleanUpSlot();
        
        foreach (var slot in stashItemSlots)
            slot.CleanUpSlot();

        foreach (var slot in equipmentItemSlots)
        {
            // I think its more efficient (not displaying items)
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
    }

    private void Update()
    {
        // TODO: tis debug line
        if (Input.GetKeyUp(KeyCode.M))
            RemoveItem(inventory[0].data);
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

        Debug.LogWarning(value);

        equipment.Remove(value);
        equipmentDict.Remove(equipmentToRemove);
        equipmentToRemove.RemoveModifiers();
    }

    public void AddItem(ItemData itemData)
    {
        switch (itemData.itemType)
        {
            case ItemType.Material:
                AddToStash(itemData); break;

            case ItemType.Equipment:
                AddToInventory(itemData); break;

            default: break; 
        }

        UpdateInventoryUI();
    }
    private void AddToInventory(ItemData itemData)
    {
        if (inventoryDict.TryGetValue(itemData, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            var newItem = new InventoryItem(itemData);
            inventory.Add(newItem);
            inventoryDict.Add(itemData, newItem);

            int index = inventory.IndexOf(newItem);
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
    private bool TryRemoveFromInventory(ItemData item, int amount)
    {
        if (inventoryDict.TryGetValue(item, out InventoryItem inventoryValue))
        {
            //Debug.LogWarning($"Item: {item}, stack: {inventoryValue.stackSize}");

            inventoryValue.RemoveStacks(amount);

            if (inventoryValue.stackSize < 1)
            {
                inventory.Remove(inventoryValue);
                inventoryDict.Remove(item);
            }

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

    public bool UseFlask()
    {
        if (!TryGetEquipment(EquipmentType.Flask, out var flask))
            return false;

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
}
