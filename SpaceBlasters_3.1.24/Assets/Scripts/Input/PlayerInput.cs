using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [field: SerializeField]
    public Vector2 MovVec { get; private set; }

    public Action OnActJumpPressed;
    public Action OnActJumpReleased;
    public event Action<Vector2> OnActMovement;
    // Update is called once per frame
    void Update()
    {
        GetMovementInput();
        GetJumpInput();
    }
    private void GetJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {

            OnActJumpPressed?.Invoke();

        }


        if (Input.GetKeyUp(KeyCode.E))
        {
            OnActJumpReleased?.Invoke();
        }


    }
    private void GetMovementInput()
    {
        MovVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        OnActMovement?.Invoke(MovVec);
    }
}
