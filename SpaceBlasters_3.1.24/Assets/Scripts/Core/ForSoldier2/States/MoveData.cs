using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="NewMovementData", menuName ="Data/Movement")]
public class MoveData : ScriptableObject
{
    public float horMovDir;
    public float curSpeed;
    public Vector2 curVel;
}
