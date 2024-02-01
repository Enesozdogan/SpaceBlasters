using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerSoldier : NetworkBehaviour
{
    

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    [field: SerializeField] public DmgAndHeal DmgAndHeal { get;private set; }
    [field: SerializeField] public CurrencyRepo CurrencyRepo { get; private set; }
    [field: SerializeField] public ProjectileBullet Projectile { get; private set; }

    [field: SerializeField] public int MaxFuel { get; private set; } = 20;
    public NetworkVariable<float> JetpackCharge = new NetworkVariable<float>();
    public static event Action<PlayerSoldier> OnPlayerSpawnAction;
    public static event Action<PlayerSoldier> OnPlayerDespawnAction;

    private float skillCd;
    public override void OnNetworkSpawn()
    {

        
        if (IsServer)
        {
            UserData userData = null;
            if (IsHost)
                userData = SingletonHost.Instance.GameManager.ServerNetwork.GetUserData(OwnerClientId);
            else
                userData = SingletonServer.Instance.GameManager.ServerNetworking.GetUserData(OwnerClientId);

            PlayerName.Value = userData.userName;


            JetpackCharge.Value = 10f;
           OnPlayerSpawnAction?.Invoke(this);
        }
    }
    
    public override void OnNetworkDespawn()
    {

        
        if (IsServer)
        {
            OnPlayerDespawnAction?.Invoke(this);
            
        }
    }

    [ServerRpc]
    public void UpgradeJetpackChargeServerRpc(float fuelInc, ServerRpcParams serverRpcParams = default)
    {
        JetpackCharge.Value += fuelInc;
        JetpackCharge.Value = Math.Clamp(JetpackCharge.Value, 0, MaxFuel);
    }

    [ServerRpc]
    public void UseJetpackChargeServerRpc(float spentCharge, ServerRpcParams serverRpcParams = default)
    {
        JetpackCharge.Value -= spentCharge;
    }

}
