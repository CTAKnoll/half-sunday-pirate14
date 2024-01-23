using System.Collections.Generic;
using UnityEngine.UI;

namespace UI.Containers
{
    public class StoreView : UIView<StoreModel>
    {
        public List<UIInteractable> StoreSlots;
        public Image StoreShip;

        public override void UpdateViewWithModel(StoreModel model)
        {
            StoreShip.sprite = model.ShopShip;
        }
    }
}