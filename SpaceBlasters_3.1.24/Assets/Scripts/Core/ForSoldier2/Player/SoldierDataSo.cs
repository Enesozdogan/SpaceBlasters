using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="SoldierSO", menuName ="Data/SoldierSO")]
public class SoldierDataSo : ScriptableObject
{

    [Header("Health Data")]
    [Space]
    public int health = 2;

    [Header("Movement Data")]
    [Space]
    public float acc;
    public float deacc;
    public float speedMax;

    [Header("Jetpack Data")]
    [Space]
    public float jetPackAcc;
    public float jetPackDeacc;
    public float jetPackSpeedMax;


    [Header("Jump Data")]
    [Space]
    public float upForce = 10;
    public float gravityMultJ = 1.5f;
    public float gravityMultF = 0.5f;
}
