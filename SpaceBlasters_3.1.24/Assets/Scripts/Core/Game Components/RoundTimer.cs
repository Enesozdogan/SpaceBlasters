using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class RoundTimer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject scoreBoardPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private RectTransform scorePanelTransform;
    [SerializeField] private TMP_Text winStateText;

    [Header("Adjustments")]
    [SerializeField] private NetworkVariable<float> mainTimerValue = new NetworkVariable<float>(180.0f);
    [SerializeField] private NetworkVariable<float> secondaryTimerValue = new NetworkVariable<float>(30.0f);
    [SerializeField] private float gameTimer;
    [SerializeField] private float shopTimer;
    [SerializeField] public NetworkVariable<int> roundCount = new NetworkVariable<int>(4);
    public int maxRoundCount;

    private NetworkVariable<bool> isPanelActive = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> isTimeStopped = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> isEnded = new NetworkVariable<bool>(false);
    private bool isDC = false;
    private bool isCalled = false;
    bool isSet = false;
    public Action OnActivateShop;

    private void Update()
    {
        if (roundCount.Value > 0)
        {
            if (IsServer)
            {
                HandleServerLogic();
            }

            UpdateTimerText();
            UpdatePanelActivation();
            HandleScorePanelTransform();

        }
        else
        {
            HandleDisconnect();
        }
    }

    private void HandleServerLogic()
    {
        if (!isTimeStopped.Value)
        {
            mainTimerValue.Value -= Time.deltaTime;
            if (mainTimerValue.Value <= 0)
            {
                mainTimerValue.Value = 0;
                isPanelActive.Value = true;
                isTimeStopped.Value = true;
                roundCount.Value -= 1;

                if (roundCount.Value == 1 && !isEnded.Value)
                    isEnded.Value = true;

                if (isEnded.Value)
                    secondaryTimerValue.Value = 0;
                else
                    secondaryTimerValue.Value = shopTimer;
            }
        }
        else
        {
            secondaryTimerValue.Value -= Time.deltaTime;
            if (secondaryTimerValue.Value <= 0)
            {
                secondaryTimerValue.Value = 0;
                isPanelActive.Value = false;
                if (isEnded.Value)
                    mainTimerValue.Value = 5;
                else
                    mainTimerValue.Value = gameTimer;
                isTimeStopped.Value = false;
            }
        }
        
        if (isEnded.Value && !isSet)
        {
            PlayerSoldier[] soldiers = FindObjectsByType<PlayerSoldier>(FindObjectsSortMode.None);
            PlayerSoldier winnerSoldier = soldiers
                .DefaultIfEmpty(null)
                .Aggregate((maxSoldier, nextSoldier) =>
                 nextSoldier.CurrencyRepo.KilledEnemyCount.Value > maxSoldier.CurrencyRepo.KilledEnemyCount.Value ? nextSoldier : maxSoldier);
            SetWinStateClientRpc(winnerSoldier.PlayerName.Value.ToString());
            isSet = true;
        }
  

    }

    [ClientRpc]
    private void SetWinStateClientRpc(string name)
    {
        winStateText.text = $"WINNER:{name}";
        winStateText.color = new Color(224, 126, 38);
    }
    private void UpdateTimerText()
    {
        if (isEnded.Value) return;

        int minutes = (!isTimeStopped.Value) ? Mathf.FloorToInt(mainTimerValue.Value / 60F) : Mathf.FloorToInt(secondaryTimerValue.Value / 60F);
        int seconds = (!isTimeStopped.Value) ? Mathf.FloorToInt(mainTimerValue.Value - minutes * 60) : Mathf.FloorToInt(secondaryTimerValue.Value - minutes * 60);

       
            timerText.text = $"{minutes}:{seconds} R:{maxRoundCount - roundCount.Value}";
    }

    private void UpdatePanelActivation()
    {
        shopPanel.SetActive(isPanelActive.Value);

        if (isPanelActive.Value && !isCalled)
        {
            OnActivateShop?.Invoke();
            isCalled = true;
        }
        else if (!isPanelActive.Value)
            isCalled = false;
    }

    private void HandleScorePanelTransform()
    {
        if (isEnded.Value)
        {
            scorePanelTransform.anchorMin = new Vector2(0, 0);
            scorePanelTransform.anchorMax = new Vector2(1, 1);
            scorePanelTransform.offsetMin = new Vector2(0, 0);
            scorePanelTransform.offsetMax = new Vector2(0, 0);
            scoreBoardPanel.GetComponent<Image>().color = new Color(255, 255, 255, 60);
            timerText.text = "";
            
        }
    }

    private void HandleDisconnect()
    {
        if (!isDC)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                SingletonHost.Instance.GameManager.ShutdownHost();
            }
            SingletonClient.Instance.GameManager.DisConnectFromGame();
            isDC = true;
        }
    }

    [ServerRpc]
    public void SetScorePanelTransformServerRpc(ServerRpcParams rpcParams = default)
    {
        isEnded.Value = true;
    }
}
