using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CloneSkill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;
    public float attackMultiplier;
    [SerializeField] private float cloneDuration = 1.5f;

    [Header("Clone")]
    public bool cloneUnlocked;
    [SerializeField] private SkillTreeSlotUI cloneUnlockButton;
    [SerializeField] private float baseCloneAttackMuiltiplier;

    [Header("Aggresive clone")]
    public bool aggresiveCloneUnlocked;
    [SerializeField] private SkillTreeSlotUI aggresiveCloneUnlockButton;
    [SerializeField] private float aggresiveCloneAttackMuiltiplier;
    //public bool canApplyOnHitEffect; its literally aggresiveCloneUnlocked

    [Header("Multiple clone")]
    public bool duplicateCloneUnlocked;
    [SerializeField] private SkillTreeSlotUI duplicateCloneUnlockButton;
    public float duplicateChance;

    [Header("Crystal instead")]
    public bool crystalInsteadOfClone;
    [SerializeField] private SkillTreeSlotUI crystalInsteadOfCloneButton;

    private CloneController controller;

    protected override void Start()
    {
        base.Start();

        CheckBaseUnlocks();

        cloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockClone);
        aggresiveCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockAggressiveClone);
        duplicateCloneUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockDuplicateClone);
        crystalInsteadOfCloneButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalInstead);
    }

    protected override void CheckBaseUnlocks()
    {
        UnlockClone();
        UnlockAggressiveClone();
        UnlockDuplicateClone();
        UnlockCrystalInstead();
    }

    private void UnlockClone()
    {
        if (cloneUnlockButton.IsUnlocked)
        {
            attackMultiplier = baseCloneAttackMuiltiplier;
            cloneUnlocked = true;
        }
    }

    private void UnlockAggressiveClone()
    {
        if (aggresiveCloneUnlockButton.IsUnlocked)
        {
            attackMultiplier = aggresiveCloneAttackMuiltiplier;
            aggresiveCloneUnlocked = true;
        }
    }

    private void UnlockDuplicateClone()
    {
        if (duplicateCloneUnlockButton.IsUnlocked)  
            duplicateCloneUnlocked = true;
    }

    private void UnlockCrystalInstead()
    {
        if (crystalInsteadOfCloneButton.IsUnlocked)
            crystalInsteadOfClone = true;
    }


    public override void Use()
    {
        // using clone skill through CreateClone
        // still used in AttemptUse() for cooldown, do not write exceptions
    }

    public void CreateClone(Transform cloneTransform, Vector3 offset)
    {
        if (!cloneUnlocked)
            return;

        if (crystalInsteadOfClone)
        {
            SkillManager.instance.Crystal.CreateCrystal();
            return;
        }

        GameObject clone = Instantiate(clonePrefab);
        controller = clone.GetComponent<CloneController>();

        controller.SetupClone(cloneTransform, offset, cloneDuration);
    }

    public void CreateClone(Transform cloneTransform)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.Crystal.CreateCrystal();
            SkillManager.instance.Crystal.ChooseRandomTarget();
            return;
        }

        GameObject clone = Instantiate(clonePrefab);
        controller = clone.GetComponent<CloneController>();

        controller.SetupClone(cloneTransform, cloneDuration);
    }



    public void CreateCloneOnCounterAttack(Transform enemyTransform)
    {
        StartCoroutine(CreateCloneDelay(enemyTransform, new Vector3(2 * player.FacingDirection, 0)));
    }

    private IEnumerator CreateCloneDelay(Transform transform, Vector3 offset)
    {
        yield return new WaitForSeconds(0.4f);
        CreateClone(transform, offset);
    }


}
