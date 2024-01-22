using System.Collections;
using System.Collections.Generic;
using Services;
using Stonks;
using UnityEngine;
using UnityEngine.UI;

public class TulipFilterCheckbox : UIInteractable
{
    public Sprite CheckedBox;
    public Sprite UncheckedBox;
    public Image BoxImage;

    private bool IsChecked = false;
    private UIDriver UiDriver;
    void Start()
    {
        ServiceLocator.TryGetService(out UiDriver);
        UiDriver.RegisterForTap(this, ToggleChecked);
        BoxImage.sprite = IsChecked ? CheckedBox : UncheckedBox;
    }

    private void ToggleChecked()
    {
        IsChecked = !IsChecked;
        BoxImage.sprite = IsChecked ? CheckedBox : UncheckedBox;
        ServiceLocator.GetService<Economy>().FilterToOwnedTulips(IsChecked);
    }
}
