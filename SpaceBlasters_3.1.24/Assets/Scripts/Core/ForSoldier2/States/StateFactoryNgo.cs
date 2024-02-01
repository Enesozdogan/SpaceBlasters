using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StateFactoryNgo : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StateBaseNgo state_idle;
    [SerializeField] private StateBaseNgo state_move;
    [SerializeField] private StateBaseNgo state_fall;
    [SerializeField] private StateBaseNgo state_jump;
    [SerializeField] private StateBaseNgo state_jetpack;

    public void GetStateReferences(SoldierBase soldierScript)
    {
        StateBaseNgo[] stateArr = GetComponents<StateBaseNgo>();

        foreach (StateBaseNgo state in stateArr)
        {
            state.InitSoldierReference(soldierScript);
        } 
    }
    public StateBaseNgo GetState(States states)
    {
        return states switch
        {
            States.Idle => state_idle,
            States.Move => state_move,
            States.Fall => state_fall,
            States.Jump => state_jump,
            States.JetPack => state_jetpack,
            _ => throw new System.Exception("State not found")

        };
    }

    public enum States
    {
        Idle,
        Move,
        Fall,
        Jump,
        JetPack
    }
}
