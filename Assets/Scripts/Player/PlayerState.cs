using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected Rigidbody2D rb;
    protected Animator animator;

    protected float xInput;
    protected float yInput;

    private readonly string animBoolName;

    protected float stateTimer;
    protected bool trigerCalled;

    public PlayerState(Player player, PlayerStateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.player = player;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        //Debug.Log($"{player.GetType().Name} entered {GetType().Name}");

        animator = player.Anim;
        rb = player.Rb;

        trigerCalled = false;

        animator.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;

        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");

        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    public virtual void Exit()
    {
        animator.SetBool(animBoolName, false);
        //Debug.Log($"Exited state {GetType().Name}");
    }

    public void AnimationFinishTrigger()
    {
        trigerCalled = true;
    }
}
