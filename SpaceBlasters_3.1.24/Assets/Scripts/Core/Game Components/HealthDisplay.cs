using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Image barImg;
    [SerializeField] DmgAndHeal dmgAndHeal;

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;

        dmgAndHeal.CurrHp.OnValueChanged += HandleHpChange;

        HandleHpChange(0, dmgAndHeal.CurrHp.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;
        dmgAndHeal.CurrHp.OnValueChanged -= HandleHpChange;
    }
    private void HandleHpChange(int oldHp, int newHp)
    {
        barImg.fillAmount = (float)newHp / dmgAndHeal.MaxHp ;
        
    }

}
