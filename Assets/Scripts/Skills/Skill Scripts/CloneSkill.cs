using System.Collections;
using UnityEngine;

public class CloneSkill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration = 1.5f;
    [Space]
    [SerializeField] private bool canAttack = true;

    [SerializeField] private bool canCloneOnDashStart;
    [SerializeField] private bool canCloneOnDashOver;
    [SerializeField] private bool canCloneOnCounterAttack;

    [Header("Duplicate clone")]
    [SerializeField] private bool canDuplicateClone;
    public float duplicateChance;

    [Header("Crystal instead")]
    public bool crystalInsteadOfClone;


    private CloneController controller;


    public void CreateClone(Transform cloneTransform, Vector3 offset)
    {
        if (crystalInsteadOfClone)
        {
            SkillManager.instance.Crystal.CreateCrystal();
            return;
        }

        GameObject clone = Instantiate(clonePrefab);
        controller = clone.GetComponent<CloneController>();

        controller.SetupClone(cloneTransform, offset, cloneDuration, canAttack, canDuplicateClone);
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

        controller.SetupClone(cloneTransform, cloneDuration, canAttack, canDuplicateClone);
    }

    public void CreateCloneOnDashStart()
    {
        if (canCloneOnDashStart)
            CreateClone(player.transform);
    }

    public void CreateCloneOnDashOver()
    {
        if (canCloneOnDashOver)
            CreateClone(player.transform); 
    }

    public void CreateCloneOnCounterAttack(Transform enemyTransform)
    {
        if (canCloneOnCounterAttack)
            StartCoroutine(CreateCloneDelay(enemyTransform, new Vector3(2 * player.FacingDirection, 0))); 
    }

    private IEnumerator CreateCloneDelay(Transform transform, Vector3 offset)
    {
        yield return new WaitForSeconds(0.4f);
        CreateClone(transform, offset);
    }
}
