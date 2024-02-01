using UnityEngine;
using Unity.Netcode;

public class RttLatency : NetworkBehaviour
{
    private void Update()
    {
        if (IsClient)
        {
            SendTimestampServerRpc(Time.time);
        }
    }

    [ServerRpc(RequireOwnership =false)]
    private void SendTimestampServerRpc(float clientTimestamp, ServerRpcParams serverRpcParams = default)
    {
 
        ReceiveTimestampClientRpc(clientTimestamp);
    }

    [ClientRpc]
    private void ReceiveTimestampClientRpc(float originalClientTimestamp, ClientRpcParams clientRpcParams = default)
    {
        if (!IsServer && !IsHost) // Only run on the client
        {
            float rtt = Time.time - originalClientTimestamp;
            if(rtt > 0 )
                Debug.Log("RTT: " + rtt*1000+"ms");
            
        }
    }

  
}
