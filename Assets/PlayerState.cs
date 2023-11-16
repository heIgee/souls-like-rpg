using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;

    private readonly string animBoolName;

    public PlayerState(Player player, PlayerStateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.player = player;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        Debug.Log("I entered " + animBoolName);
    }

    public virtual void Update()
    {
        Debug.Log("I am in " + animBoolName);
    }

    public virtual void Exit()
    {
        Debug.Log("I exited " + animBoolName);
    }
}
