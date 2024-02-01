using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class ScoreObjDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    public ulong ClientId { get; private set; }
    public int HealthVal { get; private set; }
    public int CurrencyVal { get; private set; }
    public int KillCount { get; private set; }

    FixedString32Bytes userName;
    

    public void InitializeTextDisplay(ulong clientId, int currencyVal, int healthVal,int killCount,FixedString32Bytes name)
    {
        this.userName = name;
        this.ClientId = clientId;

        UpdateDynamicElements(currencyVal,healthVal,killCount);
    }

    public void ChangeText()
    {
        text.text = $"{userName} - HP:{HealthVal} - C:{CurrencyVal} - K:{KillCount}";
    }

    public void UpdateDynamicElements(int currencyVal, int healthVal,int killCount)
    {
        HealthVal=healthVal;
        CurrencyVal=currencyVal;
        KillCount=killCount;
        ChangeText() ;
    }
}
