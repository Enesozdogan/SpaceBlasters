using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class CurrencySpawner : NetworkBehaviour
{
    [SerializeField] RespawnerCurrency respawningCurrency;

    [Header("Adjustments")]
    [SerializeField] int maxCurrency;
    [SerializeField] int currencyVal;
    [SerializeField] Vector2 currencySpawnRangeX;
    [SerializeField] Vector2 currencySpawnRangeY;
    [SerializeField] LayerMask layer;

    private Collider2D[] buffer_cols=new Collider2D[1];
    private float currencyRadius;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        currencyRadius = respawningCurrency.GetComponent<CircleCollider2D>().radius;
        for(int i = 0; i < maxCurrency; i++)
        {
            SpawnCurrency();
        }
    }

    private void SpawnCurrency()
    {
       
        RespawnerCurrency respawnerCurrency = Instantiate(respawningCurrency, GetSpawnPos(), Quaternion.identity);

        respawnerCurrency.SetCurrencyVal(currencyVal);
        
        respawnerCurrency.GetComponent<NetworkObject>().Spawn();
        respawnerCurrency.OnCurrencyPicked += HandleOnPick;
    }

    private void HandleOnPick(RespawnerCurrency currency)
    {
        currency.transform.position = GetSpawnPos();
        currency.ResetCurrency();
    }

    private Vector2 GetSpawnPos()
    {

        while (true)
        {
            float x= Random.Range(currencySpawnRangeX.x,currencySpawnRangeX.y);
            float y= Random.Range(currencySpawnRangeY.x,currencySpawnRangeY.y);

            Vector2 pos= new Vector2 (x,y);

            int colCount = Physics2D.OverlapCircleNonAlloc(pos, currencyRadius, buffer_cols,layer);
            if (colCount == 0)
            {
                return pos;
            }
        }
    }

}
