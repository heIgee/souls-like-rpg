using UnityEngine;

public class SkeletonBattleState : SkeletonState
{
    public SkeletonBattleState(Skeleton skeleton, EnemyStateMachine stateMachine, string animBoolName) : base(skeleton, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = skeleton.battleTime;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();





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
        if (Time.time >= skeleton.lastAttackTime + skeleton.attackCooldown)
        {
            skeleton.lastAttackTime = Time.time;
            return true;
        }

        return false;
    }
}
