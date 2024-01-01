using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemTooltipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemTypeText;
    [SerializeField] private TextMeshProUGUI itemDescription;

    [SerializeField] private float originalNameFont;

    [SerializeField] private int minDescriptionLines;

    public void ShowTooltip(EquipmentData item)
    {
        if (item == null)
            return;

        itemNameText.text = item.itemName;
        itemTypeText.text = item.equipmentType.ToString();

        string description = item.GetDescription();

        // for minimum box size
        if (description.Split('\n').Length < minDescriptionLines)
            for (int i = 0; i < minDescriptionLines - description.Split("\n").Length; i++)
                description += '\n';

        itemDescription.text = description;

        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        // prevent shrinking of short names 
        itemNameText.fontSize = originalNameFont;
        gameObject.SetActive(false);
    }
}
