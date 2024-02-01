using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnLocation : MonoBehaviour
{
    
    private static List<RespawnLocation> respawnLocations = new List<RespawnLocation>();

    //OnEnable yerine OnDestroy da olur.
    private void OnEnable()
    {
        respawnLocations.Add(this);
    }
    private void OnDisable()
    {
        respawnLocations.Remove(this);
    }
    
    /// <summary>
    /// RespawnLocation siniflari arasinda statik olan listeden eleman getirir.
    /// </summary>
    /// <returns></returns>
    public static Vector3 GenerateRandomPos()
    {
        
        if (respawnLocations.Count == 0)
            return Vector3.zero;

        int rand = Random.Range(0, respawnLocations.Count);
        return respawnLocations[rand].transform.position;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position,Vector3.one);
    }
}
