using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class PlayerAim : NetworkBehaviour
{
    
    [Header("References")]
    [SerializeField] Transform weaponPos;
    [SerializeField] Transform headPos;
    [SerializeField] GetInput inputGetter;
 


    // Update is called once per frame
    private void LateUpdate()
    {
        if (!IsOwner) return;

        Vector3 mousepos = inputGetter.aimVectorPos;
        Vector3 gunposition = Camera.main.WorldToScreenPoint(weaponPos.position);
        mousepos.x = mousepos.x - gunposition.x;
        mousepos.y = mousepos.y - gunposition.y;
        float gunAngle = Mathf.Atan2(mousepos.y, mousepos.x) * Mathf.Rad2Deg;
        if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x < weaponPos.position.x)
        {
            weaponPos.rotation = Quaternion.Euler(new Vector3(180f, 0f, -gunAngle));
            headPos.rotation = Quaternion.Euler(new Vector3(180f, 0f, -gunAngle));
            weaponPos.parent.rotation = Quaternion.Euler(new Vector3(0, 180f, 0f));
          
        }
        else
        {
            weaponPos.rotation = Quaternion.Euler(new Vector3(0f, 0f, gunAngle));
            headPos.rotation = Quaternion.Euler(new Vector3(0f, 0f, gunAngle));
            weaponPos.parent.rotation = Quaternion.Euler(new Vector3(0, 0f, 0f));
           

        }
    }
  
    



}
