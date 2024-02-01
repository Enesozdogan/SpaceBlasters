using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class StateBaseNgo : NetworkBehaviour
{
   
    [SerializeField] protected SoldierBase soldier;

    public int stateIndex;
   
    public void InitSoldierReference(SoldierBase soldier)
    {
        this.soldier= soldier;
    }
    public void Enter()
    {
        if (!IsOwner || soldier==null) return;
        soldier.inputGetter.MoveEvent += HandleMovement;
        soldier.inputGetter.OnJumpAction += HandleJump;
        soldier.inputGetter.OnJetPackAction += HandleJetPack;


        EnterState();

       
    }

   

    public override void OnNetworkSpawn()
    {
        if (!IsOwner || soldier == null) return;
        soldier.inputGetter.MoveEvent += HandleMovement;
        soldier.inputGetter.OnJumpAction += HandleJump;
        soldier.inputGetter.OnJetPackAction += HandleJetPack;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        soldier.inputGetter.MoveEvent -= HandleMovement;
        soldier.inputGetter.OnJumpAction -= HandleJump;
        soldier.inputGetter.OnJetPackAction -= HandleJetPack;
    }

    public void Exit()
    {
        if (!IsOwner) return;
        soldier.inputGetter.MoveEvent -= HandleMovement;
        soldier.inputGetter.OnJumpAction -= HandleJump;
        soldier.inputGetter.OnJetPackAction -= HandleJetPack; 
        
        ExitState();

    }
    protected virtual void EnterState()
    {

    }
    protected virtual void ExitState()
    {

    }
    protected virtual void HandleMovement(Vector2 vector)
    {
       
    }
    protected virtual void HandleJetPack(bool isPressed)
    {
        CanJet(isPressed);
    }
    protected virtual void HandleJump(bool isPressed)
    {
        CanJump(isPressed);
    }
    private void CanJet(bool isPressed)
    { 
        if (soldier.playerSoldier.JetpackCharge.Value > 0 && soldier.inputGetter.isJetting)
        {
            soldier.ShiftToState(soldier.factory_state.GetState(StateFactoryNgo.States.JetPack));
        }
    }
    private void CanJump(bool isPressed)
    {

        if (soldier.isGrounded && isPressed)
        {
            soldier.ShiftToState(soldier.factory_state.GetState(StateFactoryNgo.States.Jump));
        }
    }

    public virtual void UpdateInState()
    {
        
        CheckFalling();
    }

    protected bool CheckFalling()
    {
        if (soldier.isGrounded == false)
        {
            soldier.ShiftToState(soldier.factory_state.GetState(StateFactoryNgo.States.Fall));
            return true;
        }
        return false;
    }



    public virtual void FixedUpdateState()
    {

    }
}
