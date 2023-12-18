using System.Collections;
using UnityEngine;

public class Player : Entity
{


    [Header("Move info")]
    public float moveSpeed = 12f;
    public float jumpForce = 10f;

    [Header("Dash info")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.3f;
    // TODO: gradual acceleration (lower distance, lower impact)
    public float swordReturningForce = 5f; 

    public float DashDirection { get; private set; }

    [Header("Attack details")]
    public Vector2[] AttackMovement;
    public float CounterAttackDuration = 0.2f;

    public bool IsBusy { get; private set; }

    public IEnumerator BusyFor(float seconds)
    {
        IsBusy = true;
        yield return new WaitForSeconds(seconds);
        IsBusy = false;
    }

    public SkillManager Skill { get; private set; }
    public GameObject ThrownSword { get; private set; } 

    #region States
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerAirState AirState { get; private set; }
    public PlayerWallSlideState WallSlideState { get; private set; }
    public PlayerWallJumpState WallJumpState { get; private set; }
    public PlayerDashState DashState { get; private set; }

    public PlayerPrimaryAttackState PrimaryAttackState { get; private set; }
    public PlayerCounterAttackState CounterAttackState { get; private set; }

    public PlayerAimSwordState AimSwordState { get; private set; }
    public PlayerCatchSwordState CatchSwordState { get; private set; }
    public PlayerBlackHoleState BlackHoleState { get; private set; }

    #endregion

    protected override void Awake()
    {
        base.Awake();

        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine, "Idle");
        MoveState = new PlayerMoveState(this, StateMachine, "Move");
        JumpState = new PlayerJumpState(this, StateMachine, "Jump");
        AirState  = new PlayerAirState(this, StateMachine, "Jump");
        WallSlideState = new PlayerWallSlideState(this, StateMachine, "WallSlide");
        WallJumpState = new PlayerWallJumpState(this, StateMachine, "Jump");
        DashState = new PlayerDashState(this, StateMachine, "Dash");

        PrimaryAttackState = new PlayerPrimaryAttackState(this, StateMachine, "Attack");
        CounterAttackState = new PlayerCounterAttackState(this, StateMachine, "CounterAttack");

        AimSwordState = new PlayerAimSwordState(this, StateMachine, "AimSword");
        CatchSwordState = new PlayerCatchSwordState(this, StateMachine, "CatchSword");
        BlackHoleState = new PlayerBlackHoleState(this, StateMachine, "Jump");
    }

    protected override void Start()
    {
        base.Start();

        Debug.Log("Player started");

        if (Anim != null)
        {
            Debug.Log("Animator found and assigned successfully.");
        }
        else
        {
            Debug.LogError("Animator not found. Check the hierarchy and component setup.");
        }

        StateMachine.Initialize(IdleState);
        Skill = SkillManager.instance;
    }

    protected override void Update()
    {
        base.Update();

        StateMachine?.CurrentState?.Update(); // this throws nullreference when updating script while game is running
        CheckDash();

        if (Input.GetKeyDown(KeyCode.F))
            Skill.Crystal.AttemptUse();
    }

    public bool SwordAvailable
    {
        get
        {
            if (!ThrownSword) // sword with us
                return true;

            // no sword, return it
            ThrownSword.GetComponent<SwordController>().ReturnSword();
            return false;
        }
    }
    public void ThrowSword(GameObject newSword) => ThrownSword = newSword;
    public void CatchSword()
    {
        StateMachine.ChangeState(CatchSwordState);
        Destroy(ThrownSword);
    }

    public void AnimationTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    public void CheckDash()
    {
        if (!IsWallDetected
            && Input.GetKeyDown(KeyCode.LeftShift)
            && SkillManager.instance.Dash.AttemptUse())
        {
            DashDirection = Input.GetAxisRaw("Horizontal");

            if (DashDirection == 0)
                DashDirection = FacingDirection;

            StateMachine.ChangeState(DashState);
        }
    }
}
