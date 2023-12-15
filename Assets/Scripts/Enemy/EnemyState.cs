using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState
{
    protected EnemyStateMachine stateMachine;
    protected Enemy enemy;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected readonly string animBoolName;

    protected float stateTimer;
    protected bool triggerCalled;

    public EnemyState(Enemy enemy, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.enemy = enemy;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        triggerCalled = false;

        //Debug.Log($"{enemy.GetType().Name} entered {GetType().Name}");

        animator = enemy.Anim;
        rb = enemy.Rb;

        animator.SetBool(animBoolName, true);
    }

    public virtual void Exit()
    {
        triggerCalled = true;

        animator.SetBool(animBoolName, false);
    }

    public virtual void Update() => stateTimer -= Time.deltaTime;


    public virtual void AnimationFinishTrigger() => triggerCalled = true;
}
