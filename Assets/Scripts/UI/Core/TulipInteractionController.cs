using System;
using System.Collections.Generic;
using Plants;
using Services;
using UnityEngine.EventSystems;

namespace UI.Plants
{
    public class TulipInteractionController : UIController<SimpleBucketView, SimpleBucketModel>
    {
        public delegate bool TulipInteraction(TulipData data);

        private TulipInteraction InteractionFunction;
        public TulipInteractionController(SimpleBucketView view,  TulipInteraction consumeFunc) : base(view)
        {
            InteractionFunction = consumeFunc;
        }

        public bool FireInteraction(TulipData data)
        {
            Audio.PlayOneShot(View.sfx_onClick);
            return InteractionFunction(data);
        }
    }
}