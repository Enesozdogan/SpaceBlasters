using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjOnDestroy : MonoBehaviour
{
    [SerializeField] GameObject objPrefab;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody != null)
        {
            Instantiate(objPrefab, transform.position, Quaternion.identity);
        }
    }
}
