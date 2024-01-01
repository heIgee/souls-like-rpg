using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    public int comboCounter;
    private float lastTimeAttacked;
    private readonly float comboWindow = 2f;
    //private bool attackInputInBuffer;

    public PlayerPrimaryAttackState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        xInput = 0; // this fixes bug on attack direction :(

        if (comboCounter > 2
            || Time.time >= lastTimeAttacked + comboWindow)
            comboCounter = 0;

        animator.SetInteger("ComboCounter", comboCounter);

        player.SetVelocity(player.AttackMovement[comboCounter].x 
            * (xInput == 0 ? player.FacingDirection : xInput), player.AttackMovement[comboCounter].y);

        stateTimer = 0.1f;
    }

    public override void Exit()
    {
        base.Exit();

        comboCounter++;
        lastTimeAttacked = Time.time;

        player.StartCoroutine(nameof(player.BusyFor), 0.15f);
    }

    public override void Update()
    {
        base.Update();

        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    attackInputInBuffer = true;
        //    return;
        //}

        if (stateTimer < 0)
            player.SetVelocity(0, 0);

        if (trigerCalled)
        {
            //if (attackInputInBuffer)
            //{
            //    attackInputInBuffer = false;
            //    stateMachine.ChangeState(player.PrimaryAttackState);
            //}
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
