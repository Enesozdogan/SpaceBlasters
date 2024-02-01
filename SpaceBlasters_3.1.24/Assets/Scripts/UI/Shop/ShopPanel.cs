using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ShopPanel : NetworkBehaviour
{
  

    
    [Header("References")]
    [SerializeField]
    private Transform armourSpawnTransform;
    [SerializeField]
    private Transform weaponSpawnTransform;
    [SerializeField]
    private Transform skillSpawnTransform;
    [SerializeField]
    private RoundTimer roundTimer;
    [SerializeField]
    private ShopItemArmour armourPrefab;
    [SerializeField]
    private ShopItemWeaponDmg dmgPrefab;
    [SerializeField]
    private ShopItemFuel fuelPrefab;
    public TMP_Text purchaseText;
    public TMP_Text totalCurVal;

    private PlayerSoldier soldier;

    [Header("Adjustments")]
    [SerializeField]
    private int creationCount;

    public override void OnNetworkSpawn()
    {
        
        roundTimer.OnActivateShop += HandleShopActivation;
        roundTimer.OnActivateShop += ReplenishItemStock;
    }

    private void ReplenishItemStock()
    {
        if (armourSpawnTransform.childCount == 0)
            CreateArmourItems();
        if (weaponSpawnTransform.childCount == 0)
            CreateWeaponItems();
        if (skillSpawnTransform.childCount == 0)
            CreateSkillItems();
    }

    private void HandleShopActivation()
    {
        soldier = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().gameObject.GetComponent<PlayerSoldier>();
        if(soldier!=null)
            totalCurVal.text = $"Currency: {soldier.CurrencyRepo.TotalCurrencyCount.Value}";

        GetArmorList2();
    }

    public void GetArmorList2()
    {


        armourSpawnTransform.transform.gameObject.SetActive(true);
        weaponSpawnTransform.transform.gameObject.SetActive(false);
        skillSpawnTransform.transform.gameObject.SetActive(false);
    }
    public void GetWeaponList2()
    {

        armourSpawnTransform.transform.gameObject.SetActive(false);
        skillSpawnTransform.transform.gameObject.SetActive(false);
        weaponSpawnTransform.transform.gameObject.SetActive(true);
    }

   
    public void GetSkillList2()
    {

        armourSpawnTransform.transform.gameObject.SetActive(false);
        weaponSpawnTransform.transform.gameObject.SetActive(false);
        skillSpawnTransform.transform.gameObject.SetActive (true);
    }

    private void CreateSkillItems()
    {
        int defaultAttr = 1;
        int defaultCost = 10;
        for (int i =0; i<creationCount; i++)
        {
            ShopItemFuel fuel = Instantiate(fuelPrefab, skillSpawnTransform);
            fuel.InitItem(defaultAttr,defaultCost);
            defaultCost += 5 * (roundTimer.maxRoundCount - roundTimer.roundCount.Value);
            defaultAttr += 2 * (roundTimer.maxRoundCount - roundTimer.roundCount.Value);

        }
    }

    private void CreateWeaponItems()
    {
        int defaultAttr = 11;
        int defaultCost = 23;
        for (int i = 0; i < creationCount; i++)
        {
            ShopItemWeaponDmg dmg = Instantiate(dmgPrefab, weaponSpawnTransform);

            dmg.InitItem(defaultAttr, defaultCost);
            defaultCost += 30 * (roundTimer.maxRoundCount - roundTimer.roundCount.Value);
            defaultAttr += 45 * (roundTimer.maxRoundCount - roundTimer.roundCount.Value);
        }
    }
    private void CreateArmourItems()
    {
        Debug.LogWarning("Armor Uretimi basladi");
        int defaultAttr = 30;
        int defaultCost = 30;
        for (int i = 0; i < creationCount; i++)
        {
            ShopItemArmour armour = Instantiate(armourPrefab, armourSpawnTransform);
             
            armour.InitItem(defaultAttr, defaultCost);
            defaultCost += 30 * (roundTimer.maxRoundCount- roundTimer.roundCount.Value);
            defaultAttr += 45 * (roundTimer.maxRoundCount - roundTimer.roundCount.Value);
        }
    }

}
