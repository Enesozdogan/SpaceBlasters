using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class FuelDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] Image barImg;
    [SerializeField] PlayerSoldier soldier;

    public override void OnNetworkSpawn()
    {
        if (!IsClient) return;

        soldier.JetpackCharge.OnValueChanged += HandleFuelChange;

        HandleFuelChange(0, soldier.JetpackCharge.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient) return;
        soldier.JetpackCharge.OnValueChanged -= HandleFuelChange;
    }
    private void HandleFuelChange(float oldFuel, float newFuel)
    {
        barImg.fillAmount = (float)newFuel / soldier.MaxFuel;

    }
}
