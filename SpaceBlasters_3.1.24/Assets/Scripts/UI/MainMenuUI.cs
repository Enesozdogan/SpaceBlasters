using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinInput;
    [SerializeField] private TMP_Text matchFindStatusText;
    [SerializeField] private TMP_Text queTimeText;
    [SerializeField] private TMP_Text matchButtonText;

    private bool isFindingMatch = false;
    private bool isStoppingMatch = false;
    private void Start()
    {
        matchFindStatusText.text = "";
        queTimeText.text = "";
    }
    public async void StartHostAsync()
    {
        await SingletonHost.Instance.GameManager.InitiateHost();
    }

    public async void StartMatch()
    {
        if (isStoppingMatch) return;

        if (isFindingMatch)
        {
            matchFindStatusText.text = "Stopping";
            isStoppingMatch = true;
            //Call Cancel method
            await SingletonClient.Instance.GameManager.StopMatchMaking();
            isStoppingMatch = false;
            isFindingMatch = false;
            matchButtonText.text = "Match";
            matchFindStatusText.text = "";
            return;
        }


        //Starting match
        SingletonClient.Instance.GameManager.FindMatchUI(OnMatchResponse);
        matchFindStatusText.text = "Queueing";
        matchButtonText.text = "Stop";
        isFindingMatch = true;

    }

    private void OnMatchResponse(MatchmakerPollingResult result)
    {
        switch (result)
        {
            case MatchmakerPollingResult.Success: matchFindStatusText.text = "Match Found";
                break;
            default: matchFindStatusText.text = "Cancelled";
                break;
        }
    }

    public async void StartClientAsync()
    {
        if(joinInput.text == "")
        {
            Debug.Log("Empty Code - Can't Join Lobby");
            return;
        }

        await SingletonClient.Instance.GameManager.InitiateClient(joinInput.text);

    }
}
