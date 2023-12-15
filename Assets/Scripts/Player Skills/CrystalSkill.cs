using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CrystalSkill : Skill
{
    [SerializeField] private GameObject crystalPrefab;
    [SerializeField] private float crystalDuration;
    private GameObject currentCrystal;

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
        base.Use();

        if (AttemptUseMultiCrystal())
            return;

        if (currentCrystal == null)
        {
            currentCrystal = Instantiate(crystalPrefab, player.transform.position, Quaternion.identity);
            controller = currentCrystal.GetComponent<CrystalController>();

            controller.SetupCrystal(crystalDuration, canExplode, canMove, moveSpeed);
        }
        else if (!canMove)
        {
            // player can switch positions with crystal if it's in non-move mode

            Vector2 playerPos = player.transform.position;

            player.transform.position = currentCrystal.transform.position;
            controller = currentCrystal.GetComponent<CrystalController>();

            currentCrystal.transform.position = playerPos;
            controller.FinishCrystal();
        }
    }
}
