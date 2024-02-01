using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "Input Getter", menuName = "Input/InputGetter")]
public class GetInput : ScriptableObject, IPlayerActions
{
    private Controls controls;

    public Action<bool> PrimaryFireEvent;
    public Action<Vector2> MoveEvent;

    public Action<bool> OnJumpAction;
    public Action<bool> OnJetPackAction;
    public bool isJetting=true;
    [field: SerializeField]
    public Vector2 MovementVec { get; private set; }
    public Vector2 aimVectorPos { get; private set; }   
    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        MovementVec = context.ReadValue<Vector2>();
        MoveEvent?.Invoke(MovementVec);
        
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PrimaryFireEvent?.Invoke(true);
        }
        if (context.canceled)
        {
            PrimaryFireEvent?.Invoke(false);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        aimVectorPos=context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        
        if (context.performed)
        {
            OnJumpAction?.Invoke(true);
        }
        else if(context.canceled)
        {
            OnJumpAction?.Invoke(false);
        }
      
    }

    public void OnJetPack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isJetting = true;
            OnJetPackAction?.Invoke(isJetting);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isJetting = false;
            OnJetPackAction?.Invoke(isJetting);
        }
    }

}
