using UnityEngine;

public class SkeletonBattleState : SkeletonState
{
    private float attackCooldown;

    public SkeletonBattleState(Skeleton skeleton, EnemyStateMachine stateMachine, string animBoolName) 
        : base(skeleton, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = skeleton.battleTime;
        attackCooldown = skeleton.baseAttackCooldown;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (player.Stats.IsDead)
            stateMachine.ChangeState(skeleton.IdleState);

        if (player.transform.position.x < skeleton.transform.position.x && skeleton.FacingRight
         || player.transform.position.x > skeleton.transform.position.x && !skeleton.FacingRight)
            // turn towards player
            skeleton.Flip();

        if (skeleton.IsPlayerVisible || skeleton.IsPlayerNearby) // is ready to approach player
        {
            stateTimer = skeleton.battleTime;

            // run towards player like a stupid mf
            skeleton.SetVelocity(skeleton.moveSpeed * 1.4f * skeleton.FacingDirection, rb.velocity.y);

            // RaycastHit2D.distance returns 0 if no collision, keep in mind
            if (skeleton.IsPlayerVisible && skeleton.IsPlayerVisible.distance < skeleton.attackDistance)
            {
                if (CanAttack())
                    stateMachine.ChangeState(skeleton.AttackState);
                else
                    skeleton.SetZeroVelocity();

                animator.SetBool(animBoolName, false);
            }
            else
                animator.SetBool(animBoolName, true);


        }
        else if (stateTimer < 0 || Vector2.Distance(skeleton.transform.position, player.transform.position) > 10)
            // player is too far or battle time expired
            stateMachine.ChangeState(skeleton.IdleState);

    }

    private bool CanAttack()
    {
        if (Time.time >= skeleton.lastAttackTime + attackCooldown)
        {
            attackCooldown += Random.Range(-0.1f, 0.1f);
            skeleton.lastAttackTime = Time.time;
            return true;
        }

        return false;
    }
}
