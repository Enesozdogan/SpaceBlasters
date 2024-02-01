using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StateFallNgo : StateMoveNgo
{

    public override void OnNetworkSpawn()
    {
        if (soldier == null) return;
        stateIndex = 3;
    }
    protected override void EnterState()
    {
        soldier.animManager.Animate(AnimationManager.AnimationStates.Jump);
    }

    protected override void HandleJump(bool isPressed)
    {
        
    }

    public override void UpdateInState()
    {
        CalVel();
        SetVel();
        applyGravity();
       
        if (soldier.isGrounded)
        {
            moveData.curVel = Vector2.zero;
            //soldier.rb.velocity = moveData.curVel;
            soldier.ShiftToState(soldier.factory_state.GetState(StateFactoryNgo.States.Idle));
        }
    }
    private void applyGravity()
    {
        moveData.curVel = soldier.rb.velocity;
        moveData.curVel.y += soldier.soldierDataSo.gravityMultF * Physics2D.gravity.y * Time.deltaTime;
        soldier.rb.velocity = moveData.curVel;
    }
}
