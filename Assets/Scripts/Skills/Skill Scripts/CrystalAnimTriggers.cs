using UnityEngine;

public class CrystalAnimTriggers : MonoBehaviour
{
    // don't ask me why I created separate script for this
    private CrystalController Crystal => GetComponentInParent<CrystalController>();

    private void ExplosionDamage() => Crystal.ExplosionDamage();
    private void SelfDestroy() => Crystal.SelfDestroy();
}
