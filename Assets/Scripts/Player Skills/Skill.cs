using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float cooldown;
    protected float cooldownTimer;

    protected Player player;

    protected virtual void Start()
    {
        player = PlayerManager.instance.player;
    }

    protected virtual void Update()
    {
        cooldownTimer -= Time.deltaTime;
    }

    public virtual bool AttemptUse()
    {
        if (cooldownTimer < 0)
        {
            Use();
            cooldownTimer = cooldown;
            return true;
        }

        Debug.Log($"Skill {GetType().Name} is on cooldown");
        return false;
    }

    public virtual void Use()
    {

    }
}
