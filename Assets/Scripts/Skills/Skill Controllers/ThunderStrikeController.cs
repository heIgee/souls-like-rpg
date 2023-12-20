using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ThunderStrikeController : MonoBehaviour
{
    [SerializeField] private CharStats target;
    [SerializeField] private float speed;
    private int damage;


    private Animator anim;
    private bool triggered;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Setup(int damage, CharStats target)
    {
        this.damage = damage;
        this.target = target;
    }

    private void Update()
    {
        if (!target || triggered)
            return;

        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        transform.right = transform.position - target.transform.position;

        if (Vector2.Distance(transform.position, target.transform.position) < 0.1f)
        {
            anim.transform.localRotation = Quaternion.identity;
            anim.transform.localPosition = new Vector3(0, 0.5f);

            transform.localRotation = Quaternion.identity;
            transform.localScale = new Vector3(3, 3);


            triggered = true;
            anim.SetTrigger("Hit");

            Invoke(nameof(ReleaseStrike), 0.2f);

            ReleaseStrike();
        }
    }

    private void ReleaseStrike()
    {
        target.ApplyShock();
        target.TakeDamage(damage);
        Destroy(gameObject, 0.4f);
    }
}
