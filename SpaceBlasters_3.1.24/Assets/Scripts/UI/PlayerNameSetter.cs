using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerNameSetter : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] Button joinButton;

    public const string Name = "PlayerName";
    void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }

        nameInput.text=PlayerPrefs.GetString(Name, string.Empty);
        
        OnNameChange();
       
    }

    public void OnNameChange()
    {
        if (nameInput.text != null && nameInput.text != string.Empty)
        {
            if (nameInput.text.Length >= 20)
            {
                joinButton.interactable = false;
            }
            else
            {
                joinButton.interactable = true;
            }
        }
        else
        {
            joinButton.interactable = false;
        }
    }

    public void JoinGame()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        if(!PlayerPrefs.HasKey(Name))
        {
            PlayerPrefs.SetString(Name, nameInput.text);
        }
    }
}
