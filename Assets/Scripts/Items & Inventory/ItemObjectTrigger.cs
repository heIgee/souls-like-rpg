using UnityEngine;

public class ItemObjectTrigger : MonoBehaviour
{
    private ItemObject Object => GetComponentInParent<ItemObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() == null
         || collision.GetComponent<PlayerStats>().IsDead)
            return;

        Object.PickUp();
    }
}
