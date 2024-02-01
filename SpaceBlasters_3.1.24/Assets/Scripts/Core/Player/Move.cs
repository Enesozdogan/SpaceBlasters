using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Move : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] GetInput inputGetter;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Animator anim;

    [Header("Movement Settings")]
    [SerializeField] float speed = 4f;
    [SerializeField] float jumpStr;
    [SerializeField] float gravityMultiplier;
    private Vector2 previousMovement;

    private Vector2 tmpVel;
    private bool canJump = true;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        inputGetter.MoveEvent += HandleMovement;
        inputGetter.OnJumpAction += HandleJump;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        inputGetter.MoveEvent -= HandleMovement;
        inputGetter.OnJumpAction -= HandleJump;
    }

 

    private void Update()
    {
        if (!IsOwner) return;
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        applyGravity();
        rb.velocity = new Vector2(previousMovement.x * speed, rb.velocity.y);
        
    }
    private void HandleJump(bool isPressed)
    {
        
        if (isPressed && canJump)
        {
            
            rb.velocity = new Vector2(previousMovement.x, jumpStr);
            canJump = false;
        }

    }


    private void applyGravity()
    {
        if (canJump == false)
        {
            tmpVel= rb.velocity;
            tmpVel.y += gravityMultiplier * Physics2D.gravity.y * Time.deltaTime;
            rb.velocity = tmpVel;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")){
            canJump = true;
        }
    }
    private void HandleMovement(Vector2 vector)
    {
        previousMovement = vector;

        // Update the animation state based on the movement
        if (vector.x != 0)
            ServerPlayAnimationServerRpc("BodyRun");
        else
            ServerPlayAnimationServerRpc("Idle");
    }

    // Server RPC to play an animation
    [ServerRpc]
    private void ServerPlayAnimationServerRpc(string animationName)
    {
        // Play the animation on the server
        anim.Play(animationName);

        // Call the Client RPC to play the animation on all clients
        PlayAnimationClientRpc(animationName);
    }

    // Client RPC to play an animation
    [ClientRpc]
    private void PlayAnimationClientRpc(string animationName)
    {
        // Play the animation on the client
        anim.Play(animationName);
    }
}
