using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Collections;
public class ScoreBoard : NetworkBehaviour
{
    [SerializeField] Transform spawnTransform;
    [SerializeField] ScoreObjDisplay scoreObjPrefab;

    NetworkList<ScoreModel> scoreList;

    List<ScoreObjDisplay> displayList = new List<ScoreObjDisplay>();
    private void Awake()
    {
        scoreList = new NetworkList<ScoreModel>();

      
    }
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            scoreList.OnListChanged += HandleScoreListChanged;
            
            foreach (ScoreModel model in scoreList)
            {
                HandleScoreListChanged(new NetworkListEvent<ScoreModel>
                {
                    Type = NetworkListEvent<ScoreModel>.EventType.Add,
                    Value = model

                });
            }
        }

        if (IsServer)
        {
            //Bu Foreach oyun ilk basladiginda host objeler de skor listesinde spawn olsun diye  garantiye almak icin yazdim.
            
            PlayerSoldier[] soldiers = FindObjectsByType<PlayerSoldier>(FindObjectsSortMode.None);
            foreach (var soldier in soldiers)
            {
                HandleOnPlayerSpawn(soldier);
            }
            PlayerSoldier.OnPlayerSpawnAction += HandleOnPlayerSpawn;
            PlayerSoldier.OnPlayerDespawnAction += HandleOnPlayerDespawn;
        }


     
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            scoreList.OnListChanged -= HandleScoreListChanged;
        }
        if(IsServer)
        {
            PlayerSoldier.OnPlayerSpawnAction -= HandleOnPlayerSpawn;
            PlayerSoldier.OnPlayerDespawnAction -= HandleOnPlayerDespawn;
        }
      
    }

    private void HandleScoreListChanged(NetworkListEvent<ScoreModel> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<ScoreModel>.EventType.Add:
                if (!displayList.Any(x => x.ClientId == changeEvent.Value.ClientId))
                {
                    ScoreObjDisplay scoreObjDisplay= Instantiate(scoreObjPrefab, spawnTransform);
                    scoreObjDisplay.InitializeTextDisplay( changeEvent.Value.ClientId,
                        changeEvent.Value.CurrencyVal,
                        changeEvent.Value.Health,
                        changeEvent.Value.KillCount,
                        changeEvent.Value.Name);
                    displayList.Add(scoreObjDisplay);

                }
                break;

            case NetworkListEvent<ScoreModel>.EventType.Remove:

                ScoreObjDisplay scoreObjDisplayRemove = displayList.FirstOrDefault(x=>x.ClientId == changeEvent.Value.ClientId);
                if (scoreObjDisplayRemove != null)
                {
                    scoreObjDisplayRemove.transform.SetParent(null);
                    Destroy(scoreObjDisplayRemove);
                    displayList.Remove(scoreObjDisplayRemove);
                }
            
                break;

            case NetworkListEvent<ScoreModel>.EventType.Value:

                ScoreObjDisplay scoreObjDisplayUpdate = displayList.FirstOrDefault(x => x.ClientId == changeEvent.Value.ClientId);
                if (scoreObjDisplayUpdate != null)
                {
                    scoreObjDisplayUpdate.UpdateDynamicElements(changeEvent.Value.CurrencyVal, changeEvent.Value.Health, changeEvent.Value.KillCount);
                }
                break;
        }
    }

    private void HandleOnPlayerSpawn(PlayerSoldier soldier)
    {
        scoreList.Add(new ScoreModel
        {
            ClientId=soldier.OwnerClientId,
            CurrencyVal=soldier.CurrencyRepo.TotalCurrencyCount.Value,
            Health=soldier.DmgAndHeal.CurrHp.Value,
            KillCount=soldier.CurrencyRepo.KilledEnemyCount.Value,
            Name=soldier.PlayerName.Value
        });

        soldier.CurrencyRepo.TotalCurrencyCount.OnValueChanged += (oldCurr, newCurr) => HandleCurrencyChange(soldier.OwnerClientId, newCurr);
        soldier.DmgAndHeal.CurrHp.OnValueChanged += (oldHp, newHp) => HandleHpChange(soldier.OwnerClientId, newHp);
        soldier.CurrencyRepo.KilledEnemyCount.OnValueChanged += (oldCount, newCount) => HandleKillCountChange(soldier.OwnerClientId, newCount);
        HandleHpChange(soldier.OwnerClientId, 100);
        HandleCurrencyChange(soldier.OwnerClientId, soldier.CurrencyRepo.TotalCurrencyCount.Value);
    }

  

    private void HandleOnPlayerDespawn(PlayerSoldier soldier)
    {

        if (scoreList == null) return;
        if (IsServer && soldier.OwnerClientId == OwnerClientId) { return; }
        foreach (ScoreModel scoreModel in scoreList)
        {
            if (scoreModel.ClientId!= soldier.OwnerClientId) { continue; }

            scoreList.Remove(scoreModel);
            break;

        }
        soldier.CurrencyRepo.TotalCurrencyCount.OnValueChanged -= (oldCurr, newCurr) => HandleCurrencyChange(soldier.OwnerClientId, newCurr);
        soldier.DmgAndHeal.CurrHp.OnValueChanged -= (oldHp, newHp) => HandleHpChange(soldier.OwnerClientId, newHp);
        soldier.CurrencyRepo.KilledEnemyCount.OnValueChanged -= (oldCount, newCount) => HandleKillCountChange(soldier.OwnerClientId, newCount);
    }

    private void HandleKillCountChange(ulong ownerClientId, int newCount)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i].ClientId == ownerClientId)
            {
                scoreList[i] = new ScoreModel
                {

                    ClientId = scoreList[i].ClientId,
                    Name = scoreList[i].Name,
                    CurrencyVal = scoreList[i].CurrencyVal,
                    KillCount = newCount,
                    Health = scoreList[i].Health

                };
                return;
            }
        }
    }
    private void HandleCurrencyChange(ulong ownerClientId, int newCurr)
    {
        for(int i =0; i< scoreList.Count; i++)
        {
            if (scoreList[i].ClientId == ownerClientId)
            {
                scoreList[i] = new ScoreModel
                {

                    ClientId = scoreList[i].ClientId,
                    Name = scoreList[i].Name,
                    CurrencyVal = newCurr,
                    KillCount = scoreList[i].KillCount,
                    Health = scoreList[i].Health

                };
                return;
            }
        }
    }
    private void HandleHpChange(ulong ownerClientId, int newHp)
    {
        for (int i = 0; i < scoreList.Count; i++)
        {
            if (scoreList[i].ClientId == ownerClientId)
            {
                scoreList[i] = new ScoreModel
                {

                    ClientId = scoreList[i].ClientId,
                    Name = scoreList[i].Name,
                    CurrencyVal = scoreList[i].CurrencyVal,
                    KillCount = scoreList[i].KillCount,
                    Health = newHp

                };
                return;
            }
        }
    }
}
