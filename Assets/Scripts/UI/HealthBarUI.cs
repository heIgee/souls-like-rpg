using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    private Entity entity;
    private RectTransform rect;
    private Slider slider;

    // script execution order set this Start to be called after CharStats Start
    private void Start()
    {
        rect = GetComponent<RectTransform>();
        slider = GetComponentInChildren<Slider>();
        
        entity = GetComponentInParent<Entity>();

        entity.onFlipped += FlipHealthBarUI;
        entity.Stats.onHealthChanged += UpdateHealthBarUI;

        UpdateHealthBarUI();
    }

    private void UpdateHealthBarUI()
    {
        slider.maxValue = entity.Stats.maxHp.Value;
        slider.value = entity.Stats.CurrentHp;
    }

    private void FlipHealthBarUI() => rect.Rotate(0, 180, 0);

    private void OnDisable()
    {
        entity.onFlipped -= FlipHealthBarUI;
        entity.Stats.onHealthChanged -= UpdateHealthBarUI;
    }

    private void OnEnable()
    {
        // yeah
    }
}
