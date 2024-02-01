using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class LootCurrency : CurrencyPickableBase
{

    public override void OnNetworkSpawn()
    {
        Physics2D.IgnoreLayerCollision(8, 9);
    }
    public void SetOwnerId(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    }
    public override int PickUp()
    {
       
        if (IsServer)
        {
            if (alreadyTaken)
                return 0;
            alreadyTaken = true;
            Destroy(gameObject);
            //Server Tarafinda Gosterip Gostermemek onemli degil;
            //ShowSprite(false);
            return currencyVal;
        }
        else
        {
           
            ShowSprite(false);
            return 0;
        }
    }

    public override void DestroyAfteLimit(int limit)
    {
        Destroy(gameObject, limit);
    }
    public override int Pickup2(bool isLootOwner)
    {
        Debug.LogError(isLootOwner);
        if (!isLootOwner)
        {
            if (IsServer)
            {
                if (alreadyTaken)
                    return 0;
                alreadyTaken = true;
                Destroy(gameObject);
                //Server Tarafinda Gosterip Gostermemek onemli degil;
                //ShowSprite(false);
                return currencyVal;
            }
            else
            {

                ShowSprite(false);
                return 0;
            }
        }
        return 0;
    }

}
