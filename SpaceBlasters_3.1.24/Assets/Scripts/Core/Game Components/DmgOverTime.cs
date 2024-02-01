using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DmgOverTime : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {

        //Dmg verebilmek icin DmgAndHeal sinifinin metoduna cagri
        if (collision.attachedRigidbody.TryGetComponent<DmgAndHeal>(out DmgAndHeal dmgAndHeal))
        {
            dmgAndHeal.TakeDamageServerRpc(1);
        }

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Dmg verebilmek icin DmgAndHeal sinifinin metoduna cagri
        if (collision.attachedRigidbody.TryGetComponent<DmgAndHeal>(out DmgAndHeal dmgAndHeal))
        {
            dmgAndHeal.TakeDamageServerRpc(1);
        }
    }
}
