using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField] protected LayerMask whatIsPlayer;

    [Header("Move info")]
    public float moveSpeed = 3f;
    public float jumpForce = 10f;

    [Header("Attack info")]
    public float attackDistance;
    public float attackCooldown;
    public float lastAttackTime;
    public float battleTime;

    [Header("Vision info")]
    [SerializeField] protected Transform visionCheck;
    [SerializeField] protected float visionDistance;

    [Header("Stun info")]
    public float stunDuration;
    public Vector2 stunDirection;
    protected bool canBeStunned;
    [SerializeField] protected GameObject counterImage;

    public EnemyStateMachine StateMachine { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        StateMachine = new EnemyStateMachine();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (StateMachine == null)
            Debug.Log("SM is null"); 

        if(StateMachine.CurrentState == null)
            Debug.Log("CS is null");

        StateMachine.CurrentState.Update();
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
}
