using System.Collections.Generic;
using System.Linq;
using UnityEditor.Timeline;
using UnityEngine;

public class CrystalSkill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    private GameObject currentCrystal;

    [Header("Crystal mirage")]
    [SerializeField] private bool cloneInsteadOfCrystal;

    [Header("Moving crystal")]
    [SerializeField] private bool canMove;
    [SerializeField] private float moveSpeed;

    [Header("Explosive crystal")]
    [SerializeField] private bool canExplode;

    [Header("Multi crystal")]
    [SerializeField] private bool canUseMultiStacks;
    [SerializeField] private int stacksAmount = 3;
    [SerializeField] private float stackCooldown;
    [SerializeField] private float usageTimeWindow;
    private List<GameObject> stackedCrystals = new();

    private CrystalController controller;

    protected override void Start()
    {
        base.Start();
        RefillCrystals();

        if (!canMove && canUseMultiStacks)
        {
            Debug.LogWarning("Cannot use multistacks if crystal cannot move, canUseMultiStacks set to false");
            canUseMultiStacks = false;
        }
    }

    private bool AttemptUseMultiCrystal()
    {
        if (canUseMultiStacks)
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
                    .SetupCrystal(crystalDuration, canExplode, canMove, moveSpeed);

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

    public override void Use()
    {
        if (canMove && AttemptUseMultiCrystal())
            return;

        if (currentCrystal == null)
        {
            CreateCrystal();
        }
        else if (!canMove)
        {
            // player can switch positions with crystal if it's in non-move mode

            (currentCrystal.transform.position, player.transform.position) = 
                (player.transform.position, currentCrystal.transform.position);

            if (cloneInsteadOfCrystal)
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

        controller.SetupCrystal(crystalDuration, canExplode, canMove, moveSpeed);
        controller.ChooseRandomTarget();
    }
}
