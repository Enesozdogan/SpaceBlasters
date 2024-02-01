using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DmgOutput : MonoBehaviour
{
    public int damage;

    private ulong ownerClientId;


    public void SetOwnerId(ulong ownerClientId)
    {
        this.ownerClientId = ownerClientId;
    } 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Rigidbody varsa devam 
        if (collision.attachedRigidbody == null) return;

        if(collision.attachedRigidbody.TryGetComponent<NetworkObject>(out  NetworkObject obj))
        {
            if(obj.OwnerClientId == ownerClientId) { return; }
        }
        //Dmg verebilmek icin DmgAndHeal sinifinin metoduna cagri
        if(collision.attachedRigidbody.TryGetComponent<DmgAndHeal>(out DmgAndHeal dmgAndHeal))
        {
            dmgAndHeal.TakeDamage(damage);
        }
        
    }
}
