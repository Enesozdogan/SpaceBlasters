using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnerCurrency : CurrencyPickableBase
{
    public event Action<RespawnerCurrency> OnCurrencyPicked;

    private Vector2 prevPos;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        prevPos = transform.position;
    }
    public override int PickUp()
    {
        if (IsServer)
        {
            if (alreadyTaken)
                return 0;
            alreadyTaken = true;
            OnCurrencyPicked?.Invoke(this);
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

    private void Update()
    {
        if(IsServer) return;

        if(prevPos != (Vector2 )transform.position)
        {
            ShowSprite(true);
            
        }
        prevPos = transform.position;
    }
    public void ResetCurrency()
    {
        alreadyTaken = false;
    }
}
