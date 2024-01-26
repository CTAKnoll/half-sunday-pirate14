using System.Collections;
using System.Collections.Generic;
using Services;
using Stonks;
using TMPro;
using UnityEngine;

public class GameOverMoney : MonoBehaviour
{
    public TextMeshProUGUI MoneyAmount;

    public void Start()
    {
    }
    public void Initialize()
    {
        var Economy = ServiceLocator.LazyLoad<Economy>();
        MoneyAmount.text = $"${Economy.Funds.ToString("f2")}";
        gameObject.SetActive(false);
    }
}
