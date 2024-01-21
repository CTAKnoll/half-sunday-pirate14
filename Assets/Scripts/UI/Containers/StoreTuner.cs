using UnityEngine;

namespace UI.Containers
{
    public class StoreTuner : MonoBehaviour
    {
        public StoreView storeView;
        public StoreModel storeModel;
        public void Start()
        {
            _ = new StoreController(storeView, storeModel);
        }
    }
}