using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StateJumpNgo : StateMoveNgo
{

    private bool jumPres = false;

    public override void OnNetworkSpawn()
    {
        if (soldier == null) return;
        stateIndex = 2;
    }
    protected override void EnterState()
    {
        soldier.animManager.Animate(AnimationManager.AnimationStates.Jump);
        Debug.Log(soldier.rb.velocity);
        moveData.curVel = soldier.rb.velocity;
        moveData.curVel.y = soldier.soldierDataSo.upForce;
        
        soldier.rb.velocity = moveData.curVel;
        
        jumPres = true;
    }

    public override void UpdateInState()
    {
        
        CalVel();
        SetVel();
        
        applyGravity();
        
        if (soldier.rb.velocity.y <= 0)
        {
            soldier.ShiftToState(soldier.factory_state.GetState(StateFactoryNgo.States.Fall));
        }
    }
    protected override void HandleJump(bool isPressed)
    {
        jumPres = isPressed;
    }

    private void applyGravity()
    {
        if(!jumPres)
        {
            moveData.curVel = soldier.rb.velocity;
            moveData.curVel.y += soldier.soldierDataSo.gravityMultF * Physics2D.gravity.y * Time.deltaTime;
            soldier.rb.velocity = moveData.curVel;
        }
        
    }

 
}
