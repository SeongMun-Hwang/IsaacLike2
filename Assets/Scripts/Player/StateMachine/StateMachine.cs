using UnityEngine;

public interface StateMachine
{
    public void Enter();
    public void TransitionToAttack();
    public void TransitionToIdle();
    public void TransitionToMove();
}
