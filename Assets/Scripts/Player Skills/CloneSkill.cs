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

    public void CreateClone(Transform cloneTransform)
    {
        GameObject clone = Instantiate(clonePrefab);

        if (clone == null)
            Debug.Log("clone is null");
        if(cloneTransform == null)
            Debug.Log("cloneTransform is null");
        if(clone.GetComponent<CloneController>() == null)
            Debug.Log("clone.GetComponent<CloneController>() is null");

        clone.GetComponent<CloneController>().SetupClone(cloneTransform, cloneDuration, canAttack);
    }


}
