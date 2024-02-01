using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class CurrencyPickableBase : NetworkBehaviour
{
    [SerializeField] SpriteRenderer sprite;

    protected int currencyVal=20;

    protected bool alreadyTaken;
    public int pickupClass;
    public ulong ownerClientId;


    public abstract int PickUp();

    public void SetCurrencyVal(int val)
    {
        currencyVal = val;
    }

    protected void ShowSprite(bool canSee)
    {
        sprite.enabled = canSee;
    }
    public virtual int Pickup2(bool canPick)
    {
        return 0;
    }
    public virtual void DestroyAfteLimit(int limit)
    {
        
    }

}
