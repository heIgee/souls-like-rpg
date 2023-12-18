using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    public PlayerAimSwordState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.Skill.Sword.SetDotsActive(true);
    }

    public override void Exit()
    {
        base.Exit();

        player.StartCoroutine(nameof(player.BusyFor), 0.2f);
    }

    public override void Update()
    {
        base.Update();

        player.SetZeroVelocity();

        if(Input.GetKeyUp(KeyCode.Mouse1))
            stateMachine.ChangeState(player.IdleState);

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (player.transform.position.x > mousePos.x && player.FacingRight
         || player.transform.position.x < mousePos.x && !player.FacingRight)
            player.Flip();


    }
}
