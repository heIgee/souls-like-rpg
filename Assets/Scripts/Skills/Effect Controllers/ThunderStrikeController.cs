using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class ThunderStrikeController : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() == null)
            return;

        EnemyStats target = collision.GetComponent<EnemyStats>();
        PlayerManager.instance.player.GetComponent<PlayerStats>().DoMagicalDamage(target);
    }
}
