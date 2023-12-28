using UnityEngine;

public class CrystalAnimTriggers : MonoBehaviour
{
    // don't ask me why I created separate script for this
    private CrystalController crystal => GetComponentInParent<CrystalController>();

    private void ExplosionDamage() => crystal.ExplosionDamage();
    private void SelfDestroy() => crystal.SelfDestroy();
}
