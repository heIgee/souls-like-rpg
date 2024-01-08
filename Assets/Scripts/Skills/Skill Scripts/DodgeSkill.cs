using UnityEngine;
using UnityEngine.UI;

public class DodgeSkill : Skill
{
    [Header("Dodge")]
    public bool dodgeUnlocked;
    [SerializeField] SkillTreeSlotUI dodgeUnlockButton;
    [SerializeField] private int evasionAmount = 10;
    private bool evasionApplied;

    [Header("Dodge mirage")]
    public bool dodgeMirageUnlocked;
    [SerializeField] SkillTreeSlotUI dodgeMirageUnlockButton;

    public override void Use()
    {
    }

    protected override void Start()
    {
        base.Start();

        CheckBaseUnlocks();

        dodgeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDodge);
        dodgeMirageUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDodgeMirage);
    }

    #region Unlocks
    protected override void CheckBaseUnlocks()
    {
        UnlockDodge();
        UnlockDodgeMirage();
    }

    private void UnlockDodge()
    {
        if (dodgeUnlockButton.IsUnlocked)
        {
            dodgeUnlocked = true;

            if (!evasionApplied)
            {
                player.Stats.evasion.AddModifier(evasionAmount);
                Inventory.instance.UpdateStatsUI();
                evasionApplied = true;
            }
        }
    }

    private void UnlockDodgeMirage()
    {
        if (dodgeMirageUnlockButton.IsUnlocked)
            dodgeMirageUnlocked = true;
    }
    #endregion

    public void CreateMirageOnDodge(Transform enemyTransform)
    {
        if (dodgeMirageUnlocked)
            SkillManager.instance.Clone.CreateClone(enemyTransform, 
                new Vector3(1.5f * player.FacingDirection, 0));
    }
}
