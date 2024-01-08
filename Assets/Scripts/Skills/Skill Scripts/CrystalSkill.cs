using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CrystalSkill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    private GameObject currentCrystal;

    [Header("Crystal")]
    public bool crystalUnlocked;
    [SerializeField] private SkillTreeSlotUI crystalUnlockButton;

    [Header("Crystal mirage")]
    public bool cloneInsteadOfCrystalUnlocked;
    [SerializeField] private SkillTreeSlotUI cloneInsteadOfCrystalUnlockButton;

    [Header("Explosive crystal")]
    public bool explodeUnlocked;
    [SerializeField] private SkillTreeSlotUI explodeUnlockButton;

    [Header("Moving crystal")]
    public bool moveCrystalUnlocked;
    [SerializeField] private SkillTreeSlotUI moveCrystalUnlockButton;
    [SerializeField] private float moveSpeed;

    [Header("Multi crystal")]
    public bool multiStacksUnlocked;
    [SerializeField] private SkillTreeSlotUI multiStacksUnlockButton;
    [SerializeField] private int stacksAmount = 3;
    [SerializeField] private float stackCooldown;
    [SerializeField] private float usageTimeWindow;

    private List<GameObject> stackedCrystals = new();

    private CrystalController controller;

    protected override void Start()
    {
        base.Start();
        RefillCrystals();

        CheckBaseUnlocks();

        if (!moveCrystalUnlocked && multiStacksUnlocked)
        {
            Debug.LogWarning("Cannot use multistacks if crystal cannot move, canUseMultiStacks set to false");
            multiStacksUnlocked = false;
        }

        crystalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystal);
        cloneInsteadOfCrystalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockCrystalMirage);
        explodeUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockExplode);
        moveCrystalUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMoveCrystal);
        multiStacksUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockMultiCrystal);

    }
    protected override void CheckBaseUnlocks()
    {
        UnlockCrystal();
        UnlockCrystalMirage();
        UnlockExplode();
        UnlockMoveCrystal();
        UnlockMultiCrystal();
    }

    private void UnlockCrystal()
    {
        if (crystalUnlockButton.IsUnlocked)
            crystalUnlocked = true;
    }

    private void UnlockCrystalMirage()
    {
        if (cloneInsteadOfCrystalUnlockButton.IsUnlocked)
            cloneInsteadOfCrystalUnlocked = true;
    }

    private void UnlockExplode()
    {
        if (explodeUnlockButton.IsUnlocked)
            explodeUnlocked = true;
    }

    private void UnlockMoveCrystal()
    {
        if (moveCrystalUnlockButton.IsUnlocked)
            moveCrystalUnlocked = true;
    }

    private void UnlockMultiCrystal()
    {
        if (multiStacksUnlockButton.IsUnlocked)
            multiStacksUnlocked = true;
    }


    private bool AttemptUseMultiCrystal()
    {
        if (multiStacksUnlocked)
        {
            //Debug.LogWarning("Crystals: " + crystalsee.Count);

            if (stackedCrystals.Count > 0)
            {
                if (stackedCrystals.Count > 1)
                    Invoke(nameof(ResetAbility), usageTimeWindow);
                    
                cooldown = 0;

                GameObject crystalToSpawn = stackedCrystals.Last();
                GameObject newCrystal = Instantiate(crystalToSpawn, player.transform.position, 
                    Quaternion.identity);

                //Debug.LogWarning("Created crystal");

                stackedCrystals.Remove(crystalToSpawn);

                newCrystal.GetComponent< CrystalController>()
                    .SetupCrystal(crystalDuration, explodeUnlocked, moveCrystalUnlocked, moveSpeed);

            }
            else
            {
                Debug.LogWarning("No more crystals. Refilling.");

                cooldown = stackCooldown;
                RefillCrystals();
            }

            return true;
        }

        return false;   
    }

    private void RefillCrystals()
    {
        stackedCrystals = new List<GameObject>();
        for (int i = 0; i < stacksAmount; i++)
            stackedCrystals.Add(crystalPrefab);
    }

    private void ResetAbility()
    {
        if (cooldownTimer > 0)
            return;

        Debug.LogWarning("Crystal usage time window expired");

        cooldownTimer = stackCooldown - usageTimeWindow;
        RefillCrystals();
    }

    public override bool AttemptUse()
    {
        Use();
        cooldownTimer = cooldown;
        return true;

        // TODO: had to do it because of inability to teleport to crystal immediately
        // cooldown tighted to crystal cooldown at all in original AttemptUse(),
        // so it should be reworked
        //Debug.LogWarning($"Skill {GetType().Name} is on cooldown");
        //return false;
    }

    public override void Use()
    {
        if (moveCrystalUnlocked && AttemptUseMultiCrystal())
            return;

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else if (!moveCrystalUnlocked)
        {
            // player can switch positions with crystal if it's in non-move mode

            (currentCrystal.transform.position, player.transform.position) = 
                (player.transform.position, currentCrystal.transform.position);

            if (cloneInsteadOfCrystalUnlocked)
            {
                SkillManager.instance.Clone.CreateClone(currentCrystal.transform, Vector3.zero);
                Destroy(currentCrystal);
            }
            else
            {
                currentCrystal.GetComponent<CrystalController>().FinishCrystal();
            }
        }
    }

    // I hate it from the bottom of my soul
    public void ChooseRandomTarget() => currentCrystal.GetComponent<CrystalController>().ChooseRandomTarget();

    public void CreateCrystal()
    {
        currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
        controller = currentCrystal.GetComponent<CrystalController>();

        controller.SetupCrystal(crystalDuration, explodeUnlocked, moveCrystalUnlocked, moveSpeed);
        controller.ChooseRandomTarget();
    }
}
