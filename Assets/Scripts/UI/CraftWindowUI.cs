using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftWindowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private Image itemIcon;

    [SerializeField] private Button craftButton;

    [SerializeField] private Transform materialSlotsParent;


    private ItemSlotUI[] materialSlots;

    private void Start()
    {
        materialSlots = materialSlotsParent.GetComponentsInChildren<ItemSlotUI>();
    }

    public void SetupCraftWindow(EquipmentData data)
    {
        craftButton.onClick.RemoveAllListeners();       

        itemIcon.sprite = data.icon;
        itemName.text = data.name;
        itemDescription.text = data.GetDescription();

        materialSlots = materialSlotsParent.GetComponentsInChildren<ItemSlotUI>();

        foreach (var slot in materialSlots)
            if (slot != null)
                slot.CleanSlot();

        if (data.craftingMaterials.Count > materialSlots.Length)
            Debug.LogWarning("Insufficient amount of UI slots for crafting materials");

        for (int i = 0; i < data.craftingMaterials.Count; i++)
            materialSlots[i].UpdateSlot(data.craftingMaterials[i]);

        craftButton.onClick.AddListener(delegate { 
            Inventory.instance.AttemptCraft(data, data.craftingMaterials); 
        });
    }
}
