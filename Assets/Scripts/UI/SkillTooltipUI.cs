using System.Collections;
using TMPro;
using UnityEngine;

public class SkillTooltipUI : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI skillName;
    [SerializeField] protected TextMeshProUGUI skillDescription;
    [SerializeField] private TextMeshProUGUI skillCost;

    private void Start() => HideTooltip();

    public void ShowToolTip(string name, string description, int price)
    {
        StopAllCoroutines();

        skillName.text = name;
        skillDescription.text = description;
        skillCost.text = $"Price: {price}";

        gameObject.SetActive(true);
    }

    public void HideTooltip() => gameObject.SetActive(false);

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
