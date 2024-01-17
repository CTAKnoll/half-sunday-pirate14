using UnityEngine;

namespace UI.Containers
{
    public class BulbInventoryTuner : MonoBehaviour
    {
        public InventoryView InventoryView;
        public void Start()
        {
            _ = new BulbInventoryController(InventoryView);
        }
    }
}