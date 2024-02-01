using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    [SerializeField]
    private GetInput inputGetter;
    // Start is called before the first frame update
    void Start()
    {
        inputGetter.MoveEvent += HandleMovement;
    }
    private void OnDestroy()
    {
        inputGetter.MoveEvent -= HandleMovement;
    }

    private void HandleMovement(Vector2 movement)
    {
        Debug.Log(movement);
    }
}
