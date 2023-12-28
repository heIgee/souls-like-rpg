using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice & fire effect", menuName = "Item Data/Item effect/Ice & fire")]
public class IceAndFireEffect : ItemEffect
{
    [SerializeField] private GameObject iceFirePrefab;
    [SerializeField] private Vector2 velocity;

    public override void Execute(Transform spawnTransform = null)
    {
        Player player = PlayerManager.instance.player;

        if (player.PrimaryAttackState.comboCounter != 2)
            return;

        var iceAndFire = Instantiate(iceFirePrefab, spawnTransform.position, player.transform.rotation);

        iceAndFire.GetComponent<Rigidbody2D>().velocity = velocity * player.FacingDirection;

        Destroy(iceAndFire, 10f);
    }
}
