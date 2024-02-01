using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroyAfterLimit : MonoBehaviour
{

    [SerializeField] float duration;



    private void Start()
    {
        Destroy(gameObject, duration);
    }
}
