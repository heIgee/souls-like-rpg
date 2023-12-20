using UnityEngine;

public class Skeleton : Enemy
{
    #region States
    public SkeletonIdleState IdleState { get; private set; }
    public SkeletonMoveState MoveState { get; private set; }
    public SkeletonBattleState BattleState { get; private set; }
    public SkeletonAttackState AttackState { get; private set; }
    public SkeletonStunnedState StunnedState { get; private set; }
    public SkeletonDeadState DeadState { get; private set; }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        IdleState = new SkeletonIdleState(this, StateMachine, "Idle");
        MoveState = new SkeletonMoveState(this, StateMachine, "Move");
        BattleState = new SkeletonBattleState(this, StateMachine, "Move");
        AttackState = new SkeletonAttackState(this, StateMachine, "Attack");
        StunnedState = new SkeletonStunnedState(this, StateMachine, "Stunned");
        DeadState = new SkeletonDeadState(this, StateMachine, "Idle");
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

    public override void SlowBy(float slowPercentage, float slowDuration)
    {
        slowPercentage = Mathf.Clamp01(slowPercentage);

        moveSpeed *= 1 - slowPercentage;
        jumpForce *= 1 - slowPercentage;
        Anim.speed *= 1 - slowPercentage;

        Invoke(nameof(RestoreBaseSpeed), slowDuration);
    }

    protected override void RestoreBaseSpeed()
    {
        base.RestoreBaseSpeed();

        moveSpeed = baseMoveSpeed;
        jumpForce = baseJumpForce;
    }

    public override void Die()
    {
        StateMachine.ChangeState(DeadState); 
    }
}
