using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class StatSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string statName;
    [SerializeField] private StatType statType;

    [SerializeField] private TextMeshProUGUI statValueField;
    [SerializeField] private TextMeshProUGUI statNameField;

    [TextArea]
    [SerializeField] private string statDescription;

    private PlayerStats stats;
    private UI ui;

    private void OnValidate()
    {
        statName = statType.ToString();

        gameObject.name = "Stat: " + statName;

        if (statNameField != null)
            statNameField.text = statName;
    }

    private void Start()
    {
        stats = PlayerManager.instance.player.Stats as PlayerStats;
        ui = GetComponentInParent<UI>();

        UpdateStatValueUI();
    }

    public void UpdateStatValueUI()
    {
        if (stats != null)
            statValueField.text = Stat.GetStatReference(stats, statType).Value.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData) 
    {
        float xOffset = 200f;
        float yOffset = 200f;

        var tooltipPosition = new Vector3(transform.position.x + xOffset, transform.position.y + yOffset, 0f);

        ui.statTooltip.transform.position = tooltipPosition;

        ui.statTooltip.ShowTooltip(statDescription);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.statTooltip.HideTooltip();
    }
}
