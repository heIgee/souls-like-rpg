using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PlayerStats stats;

    [SerializeField] protected Image dashImage;
    [SerializeField] private Image parryImage;
    [SerializeField] private Image crystalImage;
    [SerializeField] private Image swordImage;
    [SerializeField] private Image blackHoleImage;
    [SerializeField] private Image flaskImage;

    [SerializeField] protected float dashCooldown;
    [SerializeField] private float parryCooldown;
    [SerializeField] private float crystalCooldown; 
    [SerializeField] private float swordCooldown;
    [SerializeField] private float blackHoleCooldown;
    public float flaskCooldown;

    [Header("Souls info")]
    [SerializeField] private TextMeshProUGUI currentSouls;
    [SerializeField] private float soulsAmount;
    [SerializeField] private float increaseRate;

    private SkillManager mgr;

    private void Start()
    {
        if (stats != null)
            stats.onHealthChanged += UpdateHealthUI;

        mgr = SkillManager.instance;

        dashCooldown = mgr.Dash.cooldown;
        parryCooldown = mgr.Parry.cooldown;
        crystalCooldown = mgr.Crystal.cooldown; 
        swordCooldown = mgr.Sword.cooldown;
        blackHoleCooldown = mgr.BlackHole.cooldown;
    }

    private void Update()
    {
        // TODO: display locked icon if skill is locked

        UpdateSoulsUI();

        if (Input.GetKeyDown(KeyCode.LeftShift) && mgr.Dash.dashUnlocked)
            SetCooldown(dashImage);

        if (Input.GetKeyDown(KeyCode.Q) && mgr.Parry.parryUnlocked)
            SetCooldown(parryImage);

        if (Input.GetKeyDown(KeyCode.F) && mgr.Crystal.crystalUnlocked)
            SetCooldown(crystalImage);

        if (Input.GetKeyDown(KeyCode.Mouse1) && mgr.Sword.swordUnlocked)
            SetCooldown(swordImage);

        if (Input.GetKeyDown(KeyCode.R) && mgr.BlackHole.blackHoleUnlocked)
            SetCooldown(blackHoleImage);

        if (Input.GetKeyDown(KeyCode.Alpha1) && Inventory.instance.TryGetEquipment(EquipmentType.Flask, out _))
            SetCooldown(flaskImage);

        CheckCooldown(dashImage, dashCooldown);
        CheckCooldown(parryImage, parryCooldown);
        CheckCooldown(crystalImage, crystalCooldown);
        CheckCooldown(swordImage, swordCooldown);
        CheckCooldown(blackHoleImage, blackHoleCooldown);
        CheckCooldown(flaskImage, flaskCooldown);
    }

    private void UpdateSoulsUI()
    {
        int actualCurrency = PlayerManager.instance.currency;

        if (soulsAmount < actualCurrency)
            soulsAmount += actualCurrency * increaseRate * Time.deltaTime;
        else
            soulsAmount = actualCurrency;

        currentSouls.text = ((int)soulsAmount).ToString();
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = stats.maxHp.Value;
        slider.value = stats.CurrentHp;
    }

    private void SetCooldown(Image image)
    {
        if (image.fillAmount <= 0)
            image.fillAmount = 1;
    }

    protected void CheckCooldown(Image image, float cooldown)
    {
        if (image.fillAmount > 0)
            image.fillAmount -= 1 / cooldown * Time.deltaTime;
    }

}
