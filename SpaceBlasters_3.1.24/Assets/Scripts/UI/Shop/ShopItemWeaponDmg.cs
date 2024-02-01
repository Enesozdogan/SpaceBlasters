using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShopItemWeaponDmg : ShopItemBase
{

    [SerializeField]
    private int dmgIncRate = 4;


    private void Awake()
    {

        attrText.text = $"DmgInc+{dmgIncRate}";
        costText.text = $"{itemCost}";
    }

    public void InitItem(int dmgIncRate, int cost)
    {
        this.dmgIncRate = dmgIncRate;
        itemCost = cost;
        attrText.text = $"DmgInc+{dmgIncRate}";
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
            soldier.Projectile.IncreaseDmgServerRpc(dmgIncRate);
            Debug.LogWarning(soldier.CurrencyRepo.TotalCurrencyCount.Value);
            SetPurchaseState(true);
        }
        else
        {
            SetPurchaseState(false);
        }
    }
}
