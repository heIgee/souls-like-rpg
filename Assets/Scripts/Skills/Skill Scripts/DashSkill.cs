using UnityEngine;
using UnityEngine.UI;

public class DashSkill : Skill
{
    [Header("Dash")]
    public bool dashUnlocked;
    [SerializeField] private SkillTreeSlotUI dashUnlockButton;

    [Header("Clone on dash start")]
    private bool cloneOnDashStartUnlocked;
    [SerializeField] private SkillTreeSlotUI cloneOnStartUnlockButton;

    [Header("Clone on dash over")]
    public bool cloneOnDashOverUnlocked;
    [SerializeField] private SkillTreeSlotUI cloneOnOverUnlockButton;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        CheckBaseUnlocks();

        dashUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDash);
        cloneOnStartUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDashStart);
        cloneOnOverUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCloneOnDashEnd);
    }

    protected override void CheckBaseUnlocks()
    {
        // so that I can unlock everything I need from the editor
        UnlockDash();
        UnlockCloneOnDashStart();
        UnlockCloneOnDashEnd();
    }

    private void UnlockDash()
    {
        if (dashUnlockButton.IsUnlocked)
            dashUnlocked = true;
    }

    private void UnlockCloneOnDashStart()
    {
        if (cloneOnStartUnlockButton.IsUnlocked)
            cloneOnDashStartUnlocked = true;
    }

    private void UnlockCloneOnDashEnd()
    {
        if (cloneOnOverUnlockButton.IsUnlocked)
            cloneOnDashOverUnlocked = true;
    }

    public void CloneOnDashStart()
    {
        if (cloneOnDashStartUnlocked)
            SkillManager.instance.Clone.CreateClone(player.transform);
    }

    public void CloneOnDashOver()
    {
        if (cloneOnDashOverUnlocked)
            SkillManager.instance.Clone.CreateClone(player.transform);
    }


    public override void Use()
    {
    }
}
