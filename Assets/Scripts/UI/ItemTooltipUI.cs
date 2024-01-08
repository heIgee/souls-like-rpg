using System.Collections;
using TMPro;
using UnityEngine;

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

        StopAllCoroutines();

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

    public void HideTooltipDelay(float seconds)
    {
        if (gameObject.activeSelf)
            StartCoroutine(HideTooltipCoroutine(seconds));
    }

    public IEnumerator HideTooltipCoroutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HideTooltip();
    }
}
