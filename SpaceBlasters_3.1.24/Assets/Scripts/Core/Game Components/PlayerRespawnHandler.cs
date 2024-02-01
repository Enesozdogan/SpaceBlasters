using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerRespawnHandler : NetworkBehaviour
{
    [SerializeField] PlayerSoldier soldierPrefab;
    [SerializeField] private float currencyLeftMultiplier;

  
    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

       
        PlayerSoldier[] soldiers = FindObjectsByType<PlayerSoldier>(FindObjectsSortMode.None);
        foreach (var soldier in soldiers)
        {
            HandleSoldierSpawn(soldier);
        }
        PlayerSoldier.OnPlayerSpawnAction += HandleSoldierSpawn;
        PlayerSoldier.OnPlayerDespawnAction += HandleSoldierDespawn;

    }
  
    public override void OnNetworkDespawn()
    {

        if (!IsServer) return;
       
        PlayerSoldier.OnPlayerSpawnAction -= HandleSoldierSpawn;
        PlayerSoldier.OnPlayerDespawnAction -= HandleSoldierDespawn;
    }

    private void HandleSoldierSpawn(PlayerSoldier soldier)
    {
        soldier.DmgAndHeal.OnDeath+= (hp) => HandleSoldierDeath(soldier);
    }


    private void HandleSoldierDespawn(PlayerSoldier soldier)
    {
        soldier.DmgAndHeal.OnDeath -= (hp) => HandleSoldierDeath(soldier);
    }



    private void HandleSoldierDeath(PlayerSoldier soldier)
    {
        SoldierData data=CreateDataObj(soldier);

        Destroy(soldier.gameObject);
        StartCoroutine(SpawnCooldownIterator(soldier.OwnerClientId, data));
    }

    IEnumerator SpawnCooldownIterator(ulong ownerId, SoldierData data)
    {
        //Debug.LogWarning("Creating Soldier...");
        yield return new WaitForSeconds(1);

       
        PlayerSoldier soldier = CreateNewSoldier(ownerId, data);

       
        soldier.CurrencyRepo.Spend(-data.NewCurrency);
        soldier.DmgAndHeal.ArmourRate.Value = data.ArmourRate;
        soldier.JetpackCharge.Value = data.Fuel;
        soldier.CurrencyRepo.KilledEnemyCount.Value = data.KillCount;
    }

    
    PlayerSoldier CreateNewSoldier(ulong ownerId, SoldierData data)
    {
        try
        {
            PlayerSoldier soldier = Instantiate(soldierPrefab, RespawnLocation.GenerateRandomPos(), Quaternion.identity);
            soldier.NetworkObject.SpawnAsPlayerObject(ownerId);
            return soldier;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to create a new soldier: {ex.Message}");
            return null;
        }
    }

    private SoldierData CreateDataObj(PlayerSoldier soldier)
    {
        SoldierData data = new SoldierData
        {
            NewCurrency = (int)(soldier.CurrencyRepo.TotalCurrencyCount.Value * (currencyLeftMultiplier / 100)),
            ArmourRate = soldier.DmgAndHeal.ArmourRate.Value,
            Fuel = soldier.JetpackCharge.Value,
            KillCount = soldier.CurrencyRepo.KilledEnemyCount.Value
            
            
        };
        return data;
    } 
    public override void OnDestroy()
    {
        if (!IsServer) return;
        
    }


 


}
public class SoldierData
{
    public int NewCurrency { get; set; }
    public float ArmourRate { get; set; }
    public int KillCount { get; set; }
    public float Fuel { get; set; }
    
}