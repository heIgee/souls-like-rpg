using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalAnimationTriggers : MonoBehaviour
{
    // don't aske me why I created separate script for this
    private CrystalController crystal => GetComponentInParent<CrystalController>();
    private CircleCollider2D cd => GetComponentInParent<CircleCollider2D>();

    private void ExplosionDamage()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(crystal.transform.position, cd.radius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
                hit.GetComponent<Enemy>().Damage();
    }

    private void SelfDestroy() => crystal.SelfDestroy();
}