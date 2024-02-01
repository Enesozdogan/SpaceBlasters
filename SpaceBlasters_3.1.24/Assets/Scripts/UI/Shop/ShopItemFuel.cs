using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ShopItemFuel : ShopItemBase
{
    [SerializeField]
    private int fuelInc = 4;


    private void Awake()
    {

        attrText.text = $"Dash+{fuelInc}";
        costText.text = $"{itemCost}";
    }

    public void InitItem(int dashInc, int cost)
    {
        this.fuelInc = dashInc;
        itemCost = cost;
        attrText.text = $"DmgInc+{dashInc}";
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
            soldier.UpgradeJetpackChargeServerRpc(fuelInc);
            Debug.LogWarning(soldier.CurrencyRepo.TotalCurrencyCount.Value);
            SetPurchaseState(true);
        }
        else
        {
            SetPurchaseState(false);
        }
    }
}
