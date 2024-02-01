using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AnimationManager : NetworkBehaviour
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
            case AnimationStates.Jump:
                ServerPlayAnimationServerRpc("Jump");
                break;

            default:
                break;

        }
    }

    [ServerRpc]
    public void ServerPlayAnimationServerRpc(string animationName)
    {

        anim.Play(animationName);
        PlayAnimationClientRpc(animationName);
    }
    [ClientRpc]
    public void PlayAnimationClientRpc(string animationName)
    {
        anim.Play(animationName);
    }

    public enum AnimationStates
    {
        Idle,
        BodyRun,
        Jump
    }
}
