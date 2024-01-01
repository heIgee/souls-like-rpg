using TMPro;
using UnityEngine;

public class StatTooltipUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI description;

    public void ShowTooltip(string text)
    {
        description.text = text;
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        description.text = string.Empty;
        gameObject.SetActive(false);
    }
}
