using UnityEngine;

namespace UI.Model
{
    public class UIViewTemplateBase : ScriptableObject
    {
        
    }
    
    public class UIViewTemplate<TView> : UIViewTemplateBase  where TView : UIInteractable
    {
        public TView Prefab;
    }
}