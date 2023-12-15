using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneSkill : Skill
{
    [Header("Clone info")]
    [SerializeField] private GameObject clonePrefab;
    [SerializeField] private float cloneDuration = 1.5f;
    [Space]
    [SerializeField] private bool canAttack = true;

    private CloneController controller;

    public void CreateClone(Transform cloneTransform, Vector3 offset)
    {
        GameObject clone = Instantiate(clonePrefab);
        controller = clone.GetComponent<CloneController>();

        controller.SetupClone(cloneTransform, offset, cloneDuration, canAttack);
    }

    public void CreateClone(Transform cloneTransform)
    {
        GameObject clone = Instantiate(clonePrefab);
        controller = clone.GetComponent<CloneController>();

        controller.SetupClone(cloneTransform, cloneDuration, canAttack);
    }


}
