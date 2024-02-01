using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DmgAndHeal : NetworkBehaviour
{
    [field: SerializeField] public int MaxHp { get; private set; } = 100;
    
    //Server Authorative Health degeri olmasi icin NetworkVar sadece server degistirebilir.
    public NetworkVariable<int> CurrHp= new NetworkVariable<int>();
    public NetworkVariable<float> ArmourRate = new NetworkVariable<float>();

    private bool isDead;

    public Action<DmgAndHeal> OnDeath;


    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        CurrHp.Value = MaxHp;
    }
    public void TakeDamage(int dmgVal)
    {
        if(ArmourRate.Value > 0) 
            dmgVal =(int)(dmgVal-(dmgVal * ArmourRate.Value / 100f));

        //Debug.LogWarning("Alinan Hasar:" + dmgVal);
        ModifyHealth(-dmgVal);
    }

    public void Heal(int healVal)
    {
        ModifyHealth(healVal);
    }
    private void ModifyHealth(int val)
    {
        //Olu ise islem yapmaya gerek yok;
        if(isDead) return;

        CurrHp.Value += val;

        CurrHp.Value = Math.Clamp(CurrHp.Value, 0, MaxHp);

        if (CurrHp.Value == 0)
        {
           
            OnDeath?.Invoke(this);
            isDead = true;
        }

    }
    [ServerRpc]
    public void IncreaseArmourRateServerRpc(int armourIncRate, ServerRpcParams serverRpcParams = default)
    {
        ArmourRate.Value += armourIncRate;
        ArmourRate.Value = Math.Clamp(ArmourRate.Value, 0, 100);
    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int dmgVal)
    {

        ModifyHealth(-dmgVal);
    }
}
