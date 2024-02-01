using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimateString : MonoBehaviour
{
    [SerializeField] private string addOnString;
    [SerializeField] TextMeshProUGUI textTMpro;
    private void Start()
    {
        StartCoroutine(UpdateString());
    }

    public IEnumerator UpdateString()
    {
        string originText= textTMpro.text;
        int finalLen= textTMpro.text.Length+addOnString.Length;
        int i = 0;
        while(textTMpro.text.Length < finalLen)
        {
            textTMpro.text += addOnString[i];
            i++;
            yield return new WaitForSeconds(0.2f);

            if(textTMpro.text.Length == finalLen)
            {
                textTMpro.text= originText;
                i = 0;
                yield return new WaitForSeconds(0.2f);
            }

        }

    } 
}
