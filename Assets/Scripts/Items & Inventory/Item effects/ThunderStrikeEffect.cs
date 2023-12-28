using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Item Data/Item effect/Thunder strike")]
public class ThunderStrikeEffect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;
    public override void Execute(Transform spawnTransform = null)
    {
        Instantiate(thunderStrikePrefab, spawnTransform.transform.position, Quaternion.identity);
    }
}
