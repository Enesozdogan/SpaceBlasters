using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemArmour : ShopItemBase
{


    [SerializeField]
    private int armourIncRate=10;

  
    private void Awake()
    {
       
        attrText.text = $"Armour+{armourIncRate}";
        costText.text = $"{itemCost}";
    }

    public void InitItem(int armourIncRate, int cost)
    {
        this.armourIncRate=armourIncRate;
        itemCost = cost;
        attrText.text = $"Armour+{armourIncRate}";
        costText.text = $"{itemCost}";
    }
    public override void OnItemPurchase()
    {
        if (soldier == null)
        {
            soldier = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().gameObject.GetComponent<PlayerSoldier>();
        }
        //Debug.LogError(soldier.CurrencyRepo.TotalCurrencyCount.Value);
        if (soldier.CurrencyRepo.TotalCurrencyCount.Value >= itemCost)
        {
            
            soldier.CurrencyRepo.SpendServerRpc(itemCost);
            soldier.DmgAndHeal.IncreaseArmourRateServerRpc(armourIncRate);
            Debug.LogWarning(soldier.CurrencyRepo.TotalCurrencyCount.Value);
            SetPurchaseState(true);
        }
        else
        {
            SetPurchaseState(false);
        }
    }


}
