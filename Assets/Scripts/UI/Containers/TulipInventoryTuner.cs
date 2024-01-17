using UnityEngine;

namespace UI.Containers
{
    public class TulipInventoryTuner : MonoBehaviour
    {
        public InventoryView InventoryView;
        public void Start()
        {
            _ = new TulipInventoryController(InventoryView);
        }
    }
}