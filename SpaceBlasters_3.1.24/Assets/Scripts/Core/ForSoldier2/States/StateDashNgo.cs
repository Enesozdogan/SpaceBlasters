using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class StateDashNgo : StateMoveNgo
{
    [SerializeField]
    private ParticleSystem sparkParticles;

    [Header("Adjustments")]
    [SerializeField]
    private float jetForce;
    private float emRate=20;

    private float fuelSpendTimer=0.1f;
    private float fuelSpendCounter;
    public override void OnNetworkSpawn()
    {
        if (soldier == null) return;
        stateIndex = 4;
    }
    protected override void EnterState()
    {
        soldier.animManager.Animate(AnimationManager.AnimationStates.Jump);
        moveData.horMovDir = 0;
        moveData.curSpeed = 0;
        moveData.curVerticalSpeed = 5;
        moveData.curVel = new Vector2(0, soldier.rb.velocity.y);
        var emission = sparkParticles.emission;
        emRate = 20;
        emission.rateOverTime = 20;
        fuelSpendTimer = 0.5f;
        fuelSpendCounter = fuelSpendTimer;
        CreateSparksClient();
        CreateSparkServerRpc();
    }
    public override void UpdateInState()
    {
        CalVel();
        CalVerticalSpeed();
        SetVel();
        applyGravity();

        if (fuelSpendCounter <= 0)
        {
            soldier.playerSoldier.UseJetpackChargeServerRpc(1f);
            fuelSpendCounter = fuelSpendTimer;
        }
        fuelSpendCounter -= Time.deltaTime;     
       
        if (soldier.rb.velocity.y <= 0 || soldier.playerSoldier.JetpackCharge.Value<=0)
        {
            soldier.ShiftToState(soldier.factory_state.GetState(StateFactoryNgo.States.Fall));
        }

    }
    private void CalVerticalSpeed()
    {
        if (soldier.inputGetter.isJetting)
        {
            moveData.curVerticalSpeed += soldier.soldierDataSo.jetPackAcc * Time.deltaTime;
            var emission = sparkParticles.emission;
            emRate += Time.deltaTime*jetForce;
            emRate = Mathf.Clamp(emRate, 20, 50);
            
            emission.rateOverTime = emRate;
            moveData.curVerticalSpeed = Mathf.Clamp(moveData.curVerticalSpeed, 0, soldier.soldierDataSo.jetPackSpeedMax);
        }


       
    }
    protected override void CalVel()
    {
        CalSpeed(soldier.inputGetter.MovementVec, moveData);

        CalHorDir(moveData);
        moveData.curVel = soldier.inputGetter.MovementVec.x * moveData.curSpeed * Vector3.right;
        
        if(soldier.inputGetter.isJetting)
            moveData.curVel.y = moveData.curVerticalSpeed;

    }

   

    private void applyGravity()
    {
        if (!soldier.inputGetter.isJetting)
        {
            moveData.curVel = soldier.rb.velocity;
            moveData.curVel.y += soldier.soldierDataSo.gravityMultF * Physics2D.gravity.y * Time.deltaTime;
            soldier.rb.velocity = moveData.curVel;
        }

    }

    [ServerRpc]
    public void CreateSparkServerRpc()
    {
        CreateSparksClientRpc();
    }
    [ClientRpc]
    private void CreateSparksClientRpc()
    {
        if (IsOwner) return;
        CreateSparksClient();
    }
    private void CreateSparksClient()
    {
        sparkParticles.Play();
    }

    [ServerRpc]
    public void StopSparkServerRpc()
    {
        StopSparksClientRpc();
    }
    [ClientRpc]
    private void StopSparksClientRpc()
    {
        if (IsOwner) return;
        StopSparksClient();
    }

    private void StopSparksClient()
    {
        sparkParticles.Stop();
    }

    protected override void ExitState()
    {
        StopSparkServerRpc();
        StopSparksClient();
    }
}
