using System.Collections.Generic;
using UnityEngine;

namespace UI.Containers
{
    public class StoreTuner : MonoBehaviour
    {
        public StoreView storeView;
        public StoreModel storeModel;

        public Sprite MoroccanShip;
        public Sprite OttomanShip;
        public Sprite VenetianShip;
        
        public void Start()
        {
            var controller = new StoreController(storeView, storeModel);
            controller.InsertMerchantImages(new Dictionary<StoreController.MerchantOrigin, Sprite>
            {
                [StoreController.MerchantOrigin.Ottoman] = OttomanShip,
                [StoreController.MerchantOrigin.Venice] = VenetianShip,
                [StoreController.MerchantOrigin.Morocco] = MoroccanShip,
            });
        }
    }
}