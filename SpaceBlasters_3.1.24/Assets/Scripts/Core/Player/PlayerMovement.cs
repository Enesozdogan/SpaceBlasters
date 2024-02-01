using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Transform body_transform;
    [SerializeField] GetInput inputGetter;
    [SerializeField] Rigidbody2D rb;

    [Header("Movement Settings")]
    [SerializeField] float speed=4f;
    [SerializeField] float turningRate=30f;

    private Vector2 previousMovement;
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;
        inputGetter.MoveEvent += HandleMovement;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputGetter.MoveEvent -= HandleMovement;
    }
    private void Update()
    {
        if (!IsOwner) return;
        body_transform.Rotate(0f, 0f, previousMovement.x* -turningRate * Time.deltaTime);

    }
    private void FixedUpdate()
    {
        rb.velocity = (Vector2)body_transform.up * previousMovement.y * speed;
    }
    private void HandleMovement(Vector2 vector)
    {
        previousMovement = vector;
    }

}
