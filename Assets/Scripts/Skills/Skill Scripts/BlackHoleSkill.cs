using UnityEngine;
using UnityEngine.UI;

public class BlackHoleSkill : Skill
{
    [Header("Unlock")]
    public bool blackHoleUnlocked;
    [SerializeField] private SkillTreeSlotUI blackHoleUnlockedButton;

    [SerializeField] private GameObject blackHolePrefab;
    [SerializeField] private float blackHoleDuration;

    [Header("Resize info")]
    public float maxSize;
    [SerializeField] private float growSpeed;
    [SerializeField] private float shrinkSpeed;

    [Header("Clone attack info")]
    [SerializeField] private int attacksAmount;
    [SerializeField] private float cloneAttackCooldown;

    private BlackHoleController controller;

    protected override void Start()
    {
        base.Start();

        CheckBaseUnlocks();

        blackHoleUnlockedButton.GetComponent<Button>().onClick.AddListener(UnlockBlackhole);
    }

    protected override void CheckBaseUnlocks()
    {
        UnlockBlackhole();
    }

    private void UnlockBlackhole()
    {
        if (blackHoleUnlockedButton.IsUnlocked)
            blackHoleUnlocked = true;
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Use()
    {
        GameObject blackHole = Instantiate(blackHolePrefab, player.transform.position, Quaternion.identity);
        controller = blackHole.GetComponent<BlackHoleController>();
        controller.SetupBlackHole(maxSize, growSpeed, shrinkSpeed, attacksAmount, 
            cloneAttackCooldown, blackHoleDuration);

        AudioManager.instance.PlaySFX(3); // bankai
        AudioManager.instance.PlaySFX(6); // chronosphere
    }

    public bool SkillFinished()
    {
        //Debug.LogWarning("controller: " + controller);
        // this prevents errors before black hole creation,
        // but causes them if I want to exit the state after black hole disappers
        if (!controller)
            return false;

        //Debug.LogWarning("can exit state: " + controller.canExitState);
        //Debug.LogWarning("transform.localScale.x: " + transform.localScale.x);

        // player falls when black hole shrinks enough
        if (controller.canExitState && controller.transform.localScale.x < 2f)
            return true;

        return false;
    }
}
