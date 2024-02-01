using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StateMoveNgo : StateBaseNgo
{
    //YAPILACAKLAR

    //JUMP I BURDAN KURTAR VELOCITY AYARLARINI HALLET

    //STATE BILGISINI SERVERE GONDER BIR SEKILDE DEBUG EDEBILMEK ICIN



    public DataMovement moveData;






    public override void OnNetworkSpawn()
    {
        if (soldier == null) return;
        stateIndex = 1;
    }
    protected override void EnterState()
    {
        // Add any logic you want to execute when entering the move state
        soldier.animManager.Animate(AnimationManager.AnimationStates.BodyRun);
        moveData.horMovDir = 0;
        moveData.curSpeed = 0;
        moveData.curVel = Vector2.zero;
    }

    protected override void ExitState()
    {
        // Add any logic you want to execute when exiting the move state
    }

    

    protected void SetVel()
    {
       soldier.rb.velocity = moveData.curVel;
    }

    protected virtual void CalVel()
    {
        CalSpeed(soldier.inputGetter.MovementVec, moveData);
       
        CalHorDir(moveData);
        moveData.curVel = soldier.inputGetter.MovementVec.x * moveData.curSpeed * Vector3.right;
       
        moveData.curVel.y =soldier.rb.velocity.y;
    }

    protected void CalSpeed(Vector2 movVec,DataMovement moveData)
    {
        
        if (Mathf.Abs(movVec.x) > 0)
        {
            
            moveData.curSpeed += soldier.soldierDataSo.acc * Time.deltaTime;
        }
        else
        {
            moveData.curSpeed -= soldier.soldierDataSo.deacc * Time.deltaTime;
        }
        moveData.curSpeed = Mathf.Clamp(moveData.curSpeed, 0, soldier.soldierDataSo.speedMax);
    }
    protected void CalHorDir(DataMovement moveData)
    {
        if (soldier.inputGetter.MovementVec.x > 0)
        {
            moveData.horMovDir = 1;
        }
        else if (soldier.inputGetter.MovementVec.x < 0)
        {
            moveData.horMovDir = -1;
        }
      
    }

    //protected override void HandleMovAct(Vector2 obj)
    //{
    //    base.HandleMovAct(obj);
    //    Debug.Log("This is MoveVec:" + obj);
    //}

    public override void UpdateInState()
    {
       
        if (!IsOwner) return;
        if (CheckFalling()) return;
        CalVel();
        SetVel();
        if (Mathf.Abs(soldier.rb.velocity.x) < 0.01f)
        {
            soldier.rb.velocity=Vector2.zero;
            soldier.ShiftToState(soldier.factory_state.GetState(StateFactoryNgo.States.Idle));

        }
    }

 
   

}

