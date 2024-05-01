using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Services;
using TMPro;
using UnityEngine;

public class AlertText : MonoBehaviour, IService
{
    public TextMeshProUGUI AlertTextBox;
    private Coroutine DisplayCoroutine;
    
    // Start is called before the first frame update
    void Awake()
    {
        ServiceLocator.RegisterAsService(this);
    }
    
    public void Alert(string text, float seconds)
    {
        if (DisplayCoroutine != null)
        {
            StopCoroutine(DisplayCoroutine);
            DisplayCoroutine = null;
        }

        StartCoroutine(DisplayText(text, seconds));
    }

    private IEnumerator DisplayText(string text, float seconds)
    {
        AlertTextBox.text = text;
        yield return new WaitForSeconds(seconds);
        AlertTextBox.text = "";
    }
}
