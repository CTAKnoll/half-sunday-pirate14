using UnityEngine;

namespace UI.Containers
{
    public class TMPStore : MonoBehaviour
    {
        public StoreView storeView;
        public void Start()
        {
            _ = new StoreController(storeView);
        }
    }
}