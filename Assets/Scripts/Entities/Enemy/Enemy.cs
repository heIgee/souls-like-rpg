using System.Collections;
using UnityEngine;

public abstract class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;

    [Header("Enemy move info")]
    public float moveSpeed = 3f;
    public float jumpForce = 10f;

    protected float baseMoveSpeed;
    protected float baseJumpForce;

    [Header("Enemy attack info")]
    public float attackDistance;
    public float baseAttackCooldown;
    public float lastAttackTime;
    public float battleTime;

    [Header("Enemy vision info")]
    [SerializeField] protected Transform visionCheck;
    [SerializeField] protected float visionDistance;

    [Header("Enemy stun info")]
    public float stunDuration;
    public Vector2 stunDirection;
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;

    public EnemyStateMachine StateMachine { get; private set; }
    public string LastAnimBoolName { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        StateMachine = new EnemyStateMachine();
    }

    protected override void Start()
    {
        base.Start();

        // some variability
        moveSpeed += Random.Range(-1f, 1f);

        baseMoveSpeed = moveSpeed;
        baseJumpForce = jumpForce;
    }

    protected override void Update()
    {
        base.Update();

        StateMachine.CurrentState.Update();
    }

    public virtual void SetFreezeTime(bool __)
    {
        if (__)
        {
            moveSpeed = 0f;
            Anim.speed = 0f;
        }    
        else
        {
            Anim.speed = 1f;
            moveSpeed = baseMoveSpeed;
        }
    }

    public virtual void FreezeTimeFor(float seconds)
    {
        StartCoroutine(FreezeTimeCoroutine(seconds));
    }

    protected virtual IEnumerator FreezeTimeCoroutine(float seconds)
    {
        SetFreezeTime(true);
        yield return new WaitForSeconds(seconds);
        SetFreezeTime(false);
    }

    public virtual void OpenCounterAttackWindow()
    {
        canBeStunned = true;
        counterImage.SetActive(true);
    }

    public virtual void CloseCounterAttackWindow()
    {
        canBeStunned = false;
        counterImage.SetActive(false);
    }

    public virtual bool CanBeStunned()
    {
        if(canBeStunned)
        {
            CloseCounterAttackWindow();
            return true;
        }

        return false;
    }

    public virtual void AnimationTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    public virtual RaycastHit2D IsPlayerVisible => 
        Physics2D.Raycast(visionCheck.position, Vector2.right * FacingDirection, 10, whatIsPlayer);

    public virtual bool IsPlayerNearby => // replace by player manager call - ready
        Vector2.Distance(transform.position, PlayerManager.instance.player.transform.position) < 5f;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        // attack in this radius
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * FacingDirection, transform.position.y));
    }

    public void AssignLastAnimName(string animBoolName) => LastAnimBoolName = animBoolName;

}
