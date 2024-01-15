using System.Collections.Generic;
using Services;
using UnityEngine.EventSystems;

namespace UI.Plants
{
    public class SimpleBucketController : UIController<SimpleBucketView, SimpleBucketModel>
    {
        public SimpleBucketController(SimpleBucketView view) : base(view)
        {
           
        }
        
        
    }
}