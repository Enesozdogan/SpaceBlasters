using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdleNgo : StateBaseNgo
{
    
    [SerializeField] DataMovement moveData;

    public override void OnNetworkSpawn()
    {
        if (soldier == null) return;
        stateIndex = 0;
    }
    protected override void EnterState()
    {
        soldier.animManager.Animate(AnimationManager.AnimationStates.Idle);
        soldier.rb.velocity=Vector2.zero;
    }

    protected override void HandleMovement(Vector2 vector)
    {
        if(Mathf.Abs(soldier.inputGetter.MovementVec.x) > 0)
        {
            soldier.rb.velocity = Vector2.zero;
            soldier.ShiftToState(soldier.factory_state.GetState(StateFactoryNgo.States.Move));
        }
           
    }
    public override void UpdateInState()
    {
        if (CheckFalling()) { return; }
        if (Mathf.Abs(soldier.inputGetter.MovementVec.x) > 0)
        {
            soldier.rb.velocity = Vector2.zero;
            soldier.ShiftToState(soldier.factory_state.GetState(StateFactoryNgo.States.Move));
            
        }
            
    }
    //protected override void HandleMovAct(Vector2 inputDir)
    //{
    //    if (Mathf.Abs(inputDir.x) > 0)
    //    {
    //       soldier.ShiftToState(state_move);
    //    }
    //}
}
