using UnityEngine;

public class SpearStateMachine : StateMachine
{
    public PlayerState CurrentState { get; private set; }
    StateController player;

    public DeathState deathState;
    public IdleState idleState;
    public MoveState moveState;
    public AttackState attackState;

    public SpearStateMachine(StateController player)
    {
        this.player = player;
        deathState = new DeathState(player);
        idleState = new IdleState(player);
        moveState = new MoveState(player);
        attackState = new AttackState(player);
    }
    public void Enter()
    {
        player.GetComponent<Animator>().SetTrigger("Spear");
    }
    public void Initialize(PlayerState state)
    {
        CurrentState = state;
        state.Enter();
    }
    public void TransitionTo(PlayerState nextState)
    {
        CurrentState = nextState;
        CurrentState.Enter();
    }
}
