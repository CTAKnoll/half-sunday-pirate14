using System;
using System.Collections;
using System.Collections.Generic;
using Plants;
using Services;
using Stonks;
using TMPro;
using UI.Containers;
using UnityEngine;
using UnityEngine.UI;

public class GameOverTulip : MonoBehaviour
{
    public Image TulipImage;
    public TextMeshProUGUI TypeText;
    public TextMeshProUGUI AmountAndPriceText;

    private TulipInventoryController TulipInventory;
    private TulipArtServer ArtServer;
    private Economy Economy;

    [NonSerialized] public bool HasElement = false;

    public void Start()
    {
        Economy = ServiceLocator.LazyLoad<Economy>();
    }
    
    public void Initialize(int element)
    {
        ServiceLocator.TryGetService(out TulipInventory);
        ServiceLocator.TryGetService(out ArtServer);
        
        var tulip = TulipInventory.Inventory.GetElement(element);
        if (tulip.IsEmpty())
        {
            gameObject.SetActive(false);
            return;
        }

        HasElement = true;
        TulipImage.sprite = ArtServer.GetBaseSprite(tulip.Varietal, TulipData.TulipStage.Picked);
        TypeText.text = tulip.Varietal.ToString();
        
        var price = Economy.GetCurrentPrice(tulip.Varietal);
        var number = tulip.Count;
        AmountAndPriceText.text = $"x{number} = ${(price * number).ToString("f2")}";
        gameObject.SetActive(false);
    }

}
