using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AnimManager : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Animator anim;

    public void Animate(AnimationStates animationStates)
    {
        switch (animationStates)
        {
            case AnimationStates.Idle:
                ServerPlayAnimationServerRpc("Idle");
                break;
            case AnimationStates.BodyRun:
                ServerPlayAnimationServerRpc("BodyRun");
                break;

            default:
                break;

        }
    }

    [ServerRpc]
    public void ServerPlayAnimationServerRpc(string animationName)
    {
        // Play the animation on the server
        anim.Play(animationName);

        // Call the Client RPC to play the animation on all clients
        PlayAnimationClientRpc(animationName);
    }

    // Client RPC to play an animation
    [ClientRpc]
    public void PlayAnimationClientRpc(string animationName)
    {
        // Play the animation on the client
        anim.Play(animationName);
    }

    public enum AnimationStates
    {
        Idle,
        BodyRun
    }
}
