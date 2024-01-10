using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    private bool cloneCreated = false;
    public PlayerCounterAttackState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Debug.LogWarning("in CA state");

        cloneCreated = false;

        stateTimer = player.CounterAttackDuration;
        animator.SetBool("CounterAttackSuccessful", false);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach (var hit in colliders)
            if (hit.GetComponent<Enemy>() != null)
                if (hit.GetComponent<Enemy>().CanBeStunned())
                {
                    stateTimer = Mathf.Infinity; // just value bigger than 1 to not exit the state
                    animator.SetBool("CounterAttackSuccessful", true);

                    player.Skill.Parry.Use(); // to restore [health] on parry if unlocked

                    if (!cloneCreated)
                    {
                        player.Skill.Parry.MirageOnParry(hit.transform);
                        cloneCreated = true;
                    }
                } 

        if (stateTimer < 0 || trigerCalled)
            stateMachine.ChangeState(player.IdleState);
    }
}
