using UnityEngine;
using UnityEngine.UI;

public class ParrySkill : Skill
{
    [Header("Parry")]
    public bool parryUnlocked;
    [SerializeField] private SkillTreeSlotUI parryUnlockButton;

    [Header("Parry restore")]
    public bool restoreOnParryUnlocked;
    [SerializeField] private SkillTreeSlotUI restoreOnParryUnlockButton;
    [Range(0f, 1f)]
    [SerializeField] private float restoreHealthMultiplier;

    [Header("Parry mirage")]
    public bool mirageOnParryUnlocked;
    [SerializeField] private SkillTreeSlotUI mirageOnParryUnlockButton;

    public override void Use()
    {
        if (restoreOnParryUnlocked)
        {
            int restoreAmount = Mathf.RoundToInt(player.Stats.maxHp.Value * restoreHealthMultiplier);
            player.Stats.IncreaseHealth(restoreAmount);
        }
    }

    protected override void Start()
    {
        base.Start();
        
        parryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockParry);
        restoreOnParryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockRestoreOnParry);
        mirageOnParryUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMirageOnParry);

    }

    protected override void CheckBaseUnlocks()
    {
        UnlockParry();
        UnlockRestoreOnParry();
        UnlockMirageOnParry();
    }

    private void UnlockParry()
    {
        if (parryUnlockButton.IsUnlocked)
            parryUnlocked = true;
    }
    private void UnlockRestoreOnParry()
    {
        if (restoreOnParryUnlockButton.IsUnlocked)
            restoreOnParryUnlocked = true;
    }
    private void UnlockMirageOnParry()
    {
        if (mirageOnParryUnlockButton.IsUnlocked)
            mirageOnParryUnlocked = true;
    }

    public void MirageOnParry(Transform enemyTransform)
    {
        if (mirageOnParryUnlocked)
            SkillManager.instance.Clone.CreateCloneOnCounterAttack(enemyTransform);
    }

}
