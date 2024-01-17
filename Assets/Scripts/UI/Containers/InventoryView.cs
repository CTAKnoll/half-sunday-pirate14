using System.Collections.Generic;
using UI.Plants;
using UnityEngine;

namespace UI.Containers
{
    public class InventoryView : UIView<InventoryModel>, Bucket
    {
        public List<UIInteractable> InventorySlots;
        public AudioEvent sfx_place_item;

        public void Start()
        {
            Debug.Log(name + " " + transform.position);
        }
        public override void UpdateViewWithModel(InventoryModel model)
        {
            
        }
    }
}