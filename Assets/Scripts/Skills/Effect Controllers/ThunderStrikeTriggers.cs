using UnityEngine;

public class ThunderStrikeTriggers : MonoBehaviour
{
    // anim triggers need to be public btw
    public void SelfDestroy() => Destroy(GetComponentInParent<ThunderStrikeController>().gameObject);
}
