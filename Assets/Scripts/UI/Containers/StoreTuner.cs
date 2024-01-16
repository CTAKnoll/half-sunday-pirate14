using UnityEngine;

namespace UI.Containers
{
    public class StoreTuner : MonoBehaviour
    {
        public StoreView storeView;
        public void Start()
        {
            _ = new StoreController(storeView);
        }
    }
}