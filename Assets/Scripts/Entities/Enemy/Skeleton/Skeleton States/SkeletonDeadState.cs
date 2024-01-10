using UnityEngine;

public class SkeletonDeadState : SkeletonState
{
    public SkeletonDeadState(Skeleton skeleton, EnemyStateMachine stateMachine, string animBoolName) 
        : base(skeleton, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        animator.SetBool(enemy.LastAnimBoolName, true);
        animator.speed = 0f;

        enemy.Cd.enabled = false;

        stateTimer = 0.15f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // bounces, freezes and flies off the map
        if (stateTimer > 0)
            rb.velocity = new Vector2(0, 10);
    }
}