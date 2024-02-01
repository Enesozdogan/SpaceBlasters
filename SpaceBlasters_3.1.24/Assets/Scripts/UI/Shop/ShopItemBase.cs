using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public abstract class ShopItemBase : NetworkBehaviour
{
    public int itemClass;
    [SerializeField]
    protected  PlayerSoldier soldier;

    [SerializeField]
    protected TMP_Text costText;
    [SerializeField]
    protected TMP_Text attrText;
    [SerializeField]
    protected int itemCost;

    private ShopPanel shopPanel;
    public override void OnNetworkSpawn()
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        
        shopPanel= GetComponentInParent<ShopPanel>();
        if (playerObject != null)
        {
            soldier= playerObject.GetComponent<PlayerSoldier>();
        }
    }



    protected void SetPurchaseState(bool isPurchased)
    {
        if(shopPanel == null)
            shopPanel = GetComponentInParent<ShopPanel>();
        if (isPurchased)
        {
            shopPanel.purchaseText.color = Color.green;
            shopPanel.purchaseText.text = "Purchase: Success";
        }
        else
        {
            shopPanel.purchaseText.color = Color.red;
            shopPanel.purchaseText.text = "Purchase: Fail";
        }
        
        StartCoroutine(FadeTextToZeroAlpha(0.3f, shopPanel.purchaseText, isPurchased));
    }

    protected IEnumerator FadeTextToZeroAlpha(float t, TMP_Text i, bool willDestroy)
    {
        float startAlpha = i.color.a;
        for (float time = 0; time < t; time += Time.deltaTime)
        {
            float blend = time / t;
            i.color = new Color(i.color.r, i.color.g, i.color.b, Mathf.Lerp(startAlpha, 0, blend));
            yield return null;
        }
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        if (willDestroy)
        {
           //shopPanel.items.Remove(this);
            Destroy(this.gameObject);
        }
        shopPanel.totalCurVal.text = $"Currency: {soldier.CurrencyRepo.TotalCurrencyCount.Value}";

    }


    public abstract void OnItemPurchase();
   
}
