using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyRepo : NetworkBehaviour
{

    [Header("References")]
    [SerializeField] private DmgAndHeal dmgAndHeal;
    [SerializeField] private LootCurrency lootCurPrefab;
    [SerializeField] private LootCurrency lootCurPrefab2;
    [Header("Settings")]
    [SerializeField] private float currSplashRadius = 3f;
    [SerializeField] private float lootRate = .5f;
    [SerializeField] private int lootCount = 5;
    [SerializeField] private int minLootVal = 5;
    [SerializeField] private LayerMask layerMask;



    private Collider2D[] currArr = new Collider2D[1];
    private float currRad;


    public NetworkVariable<int> TotalCurrencyCount= new NetworkVariable<int>();
    public NetworkVariable<int> KilledEnemyCount = new NetworkVariable<int>();
    private NetworkVariable<bool> isLootOwner = new NetworkVariable<bool>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        currRad = lootCurPrefab.GetComponent<CircleCollider2D>().radius;
        TotalCurrencyCount.Value += 500;
        dmgAndHeal.OnDeath+= SpawnCurrency;
        
        
    }

 

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        dmgAndHeal.OnDeath -= SpawnCurrency;
      

    }
    private void SpawnCurrency(DmgAndHeal heal)
    {
        //Toplam paradan belirtilen miktar kadar dusur
        int lootMagnitude = (int)(TotalCurrencyCount.Value * lootRate);
        //Her uretilen currency ne kadar para verecek
        int separateLootVal= lootMagnitude / lootCount;
        //Parasi yeterli degil fonksiyondan cik
        if (separateLootVal < minLootVal) return;

        for (int i = 0; i < lootCount; i++)
        {
            LootCurrency loot= Instantiate(lootCurPrefab, LootPositionGetter(), Quaternion.identity);
            loot.SetCurrencyVal(separateLootVal);
            loot.SetOwnerId(OwnerClientId);
            loot.NetworkObject.Spawn();
            loot.DestroyAfteLimit(10);
        }
        LootCurrency skull = Instantiate(lootCurPrefab2, LootPositionGetter(), Quaternion.identity);
        skull.SetCurrencyVal(0);
        skull.SetOwnerId(OwnerClientId);
        skull.NetworkObject.Spawn();
        skull.DestroyAfteLimit(10);

    }
    private Vector2 LootPositionGetter()
    {
        //Oyuncunun etrafinda currSplash radius alanindaki bir daire cevresinde pozisiyon getirir. OverlapCircle ile cakisma olmamasin dikkat ettim.
        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * currSplashRadius;
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, currRad, currArr, layerMask);
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }
    }


    public void Spend(int cost)
    {
        TotalCurrencyCount.Value-= cost;
    }

    [ServerRpc]
    public void SpendServerRpc(int cost, ServerRpcParams serverRpcParams = default)
    {
        if (TotalCurrencyCount.Value >= cost)
        {
            TotalCurrencyCount.Value -= cost;
        }
    }
    [ServerRpc(RequireOwnership =true)]
    public void IncKillCountServerRpc(ServerRpcParams serverRpcParams = default)
    {
        Debug.LogWarning("IncKillCount");
        KilledEnemyCount.Value += 1;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       
        if(collision.TryGetComponent<CurrencyPickableBase>(out CurrencyPickableBase currencyPickableBase))
        {
            int currencyVal;


            if (currencyPickableBase.pickupClass > 0)
            {
                if (IsServer)
                {
                    if (currencyPickableBase.ownerClientId == OwnerClientId)
                        isLootOwner.Value = true;
                    else
                        isLootOwner.Value = false;
                }

                currencyVal = currencyPickableBase.Pickup2(isLootOwner.Value);
              
                if (currencyPickableBase.pickupClass == 2)
                {
                    
                    Debug.LogWarning("Carpisma");
                    if(IsServer && !isLootOwner.Value)
                        KilledEnemyCount.Value += 1;
                   
                }
                else
                {
                    if (currencyVal == 0)
                        return;
                    if (IsServer && !isLootOwner.Value)
                        TotalCurrencyCount.Value += currencyVal;
                }

            }
            else
            {
                currencyVal = currencyPickableBase.PickUp();
                if (currencyVal == 0) return;
                if (IsServer)
                    TotalCurrencyCount.Value += currencyVal;
            }

        }
    }
}
