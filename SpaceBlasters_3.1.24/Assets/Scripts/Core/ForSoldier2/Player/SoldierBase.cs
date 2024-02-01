using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SoldierBase : NetworkBehaviour
{
    [Header("References")]
    public  AnimationManager animManager;
    public GetInput inputGetter;
    public Rigidbody2D rb;
    public SoldierDataSo soldierDataSo;
    public bool isGrounded;
    public StateFactoryNgo factory_state;
    public PlayerSoldier playerSoldier;

    [Header("--States--")]
    [SerializeField] private string nameOfState = "";
    [SerializeField] StateBaseNgo state_cur, state_prev;
    [SerializeField] private NetworkVariable<int> stateIndex=new NetworkVariable<int>(0);

    //public  PlayerInput playerInput;
    public override void OnNetworkSpawn()
    {
        factory_state.GetStateReferences(this);
        //transform.position = new Vector3(-4.09000015f, -2.43000007f, 0);
        
        stateIndex.OnValueChanged += OnCurrentStateIndexChanged;
        InitSoldierScript();
    }
    public override void OnNetworkDespawn()
    {
        stateIndex.OnValueChanged -= OnCurrentStateIndexChanged;
    }
    private void InitSoldierScript()
    {

        ShiftToState(this.factory_state.GetState(StateFactoryNgo.States.Idle));

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }


        public void ShiftToState(StateBaseNgo toState)
        {
       

            if (state_cur != null)
                state_cur.Exit();
        
            state_prev = state_cur;
            state_cur = toState;

            ChangeStateIndex(state_cur.stateIndex);

            state_cur.Enter();
       
            Show_Situation();

        }



    private void Show_Situation()
    {
        if (state_prev == null || state_cur.GetType() != state_prev.GetType())
        {
            nameOfState = state_cur.GetType().Name;
        }

    }



    private void Update()
    {
        state_cur.UpdateInState();
    }
    private void FixedUpdate()
    {
        state_cur.FixedUpdateState();
    }

   

 

    public void OnCurrentStateIndexChanged(int previous, int current)
    {
        stateIndex.Value = current;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeStateIndexServerRpc(int nextStateIndex)
    {
        stateIndex.Value = nextStateIndex;
    }

    public void ChangeStateIndex(int nextStateIndex)
    {
        ChangeStateIndexServerRpc(nextStateIndex);
    }

}
