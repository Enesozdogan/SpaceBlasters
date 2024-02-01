using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemDash : ShopItemBase
{
    [SerializeField]
    private int dashInc = 4;


    private void Awake()
    {

        attrText.text = $"Dash+{dashInc}";
        costText.text = $"{itemCost}";
    }

    public void InitItem(int dashInc, int cost)
    {
        this.dashInc = dashInc;
        itemCost = cost;
        attrText.text = $"DmgInc+{dashInc}";
        costText.text = $"{itemCost}";
    }

    public override void OnItemPurchase()
    {
        throw new System.NotImplementedException();
    }
}
