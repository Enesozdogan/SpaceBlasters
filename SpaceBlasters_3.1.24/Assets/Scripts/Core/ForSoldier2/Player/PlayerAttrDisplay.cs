using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class PlayerAttrDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] PlayerSoldier playerSetup;
    private void Start()
    {
        HandleNameChanged(string.Empty, playerSetup.PlayerName.Value);
        playerSetup.PlayerName.OnValueChanged += HandleNameChanged;
    }

    private void HandleNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        playerName.text= newValue.ToString();
    }
    private void OnDestroy()
    {
        playerSetup.PlayerName.OnValueChanged -= HandleNameChanged;
    }
}
