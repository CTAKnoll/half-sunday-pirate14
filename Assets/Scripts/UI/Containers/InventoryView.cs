﻿using System.Collections.Generic;
using System.Linq;
using Plants;
using UI.Plants;
using UnityEngine;

namespace UI.Containers
{
    public class InventoryView : UIView<InventoryModel>, Bucket
    {
        public List<InventorySlotView> InventorySlots;
        public List<InventorySlotController> SlotControllers;
        public AudioEvent sfx_place_item;
        public AudioEvent sfx_rejectItem;
        
        public bool AllowStacking;
        public TulipData.TulipStage Stage;

        public void Awake()
        {
            SlotControllers = InventorySlots.Select(view => new InventorySlotController(view, Stage, AllowStacking, true)).ToList();
        }
        
        public override void UpdateViewWithModel(InventoryModel model)
        {
            
        }
    }
}