using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Entity>() != null)
            collision.GetComponent<CharStats>().TakeDamage(int.MaxValue);
    }
}
