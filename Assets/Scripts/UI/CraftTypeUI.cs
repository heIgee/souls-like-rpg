using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CraftTypeUI : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;
    [SerializeField] private GameObject craftSlotPrefab;

    [SerializeField] private List<EquipmentData> craftEquipment;


    private void Start()
    {
        SetupDefaultCraft();
    }

    public void SetupCraftList()
    {
        //Debug.LogWarning("Setting up");

        for (int i = 0; i < craftSlotParent.childCount; i++)
            Destroy(craftSlotParent.GetChild(i).gameObject);

        foreach (EquipmentData data in craftEquipment)
        {
            GameObject newSlot = Instantiate(craftSlotPrefab, craftSlotParent);
            newSlot.GetComponent<CraftSlotUI>().SetupCraftSlot(data);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();
    }

    private void SetupDefaultCraft()
    {
        if (craftEquipment[0] != null)
            UI.instance.craftWindowUI.SetupCraftWindow(craftEquipment[0]);
    }
}

//private void CleanCraftSlots()
//{
//    // missing reference w/out null check
//    foreach (var craftSlot in craftSlots)
//        if (craftSlot != null)
//            Destroy(craftSlot.gameObject);
//}