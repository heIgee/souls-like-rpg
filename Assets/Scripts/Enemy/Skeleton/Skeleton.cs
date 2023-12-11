using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : Enemy
{
    #region States
    public SkeletonIdleState IdleState { get; private set; }
    public SkeletonMoveState MoveState { get; private set; }
    public SkeletonBattleState BattleState { get; private set; }
    public SkeletonAttackState AttackState { get; private set; }
    public SkeletonStunnedState StunnedState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        IdleState = new SkeletonIdleState(this, StateMachine, "Idle");
        MoveState = new SkeletonMoveState(this, StateMachine, "Move");
        BattleState = new SkeletonBattleState(this, StateMachine, "Move");
        AttackState = new SkeletonAttackState(this, StateMachine, "Attack");
        StunnedState = new SkeletonStunnedState(this, StateMachine, "Stunned");
    }

    protected override void Start()
    {
        base.Start();

        StateMachine.Initialize(IdleState);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.Y))
            StateMachine.ChangeState(StunnedState);
    }

    // FIXME: ROFL ROFL ROFL
    //public override void Flip()
    //{
    //    base.Flip();

    //    transform.Rotate(0, 90, 0);
    //}

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            StateMachine.ChangeState(StunnedState);
            return true;
        }

        return false;
    }
}