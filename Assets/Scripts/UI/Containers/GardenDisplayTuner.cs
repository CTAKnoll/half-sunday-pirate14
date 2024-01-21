using UnityEngine;

namespace UI.Containers
{
    public class GardenDisplayTuner : MonoBehaviour
    {
        public InventoryView InventoryView;
        public void Start()
        {
            _ = new GardenDisplayController(InventoryView);
        }
    }
}