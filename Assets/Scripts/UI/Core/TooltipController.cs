using System.Collections;
using UI.Model.Templates;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Core
{
    public class TooltipController : UIController<TooltipView, TooltipModel>
    {
        private Coroutine FollowingMouse;
        
        public TooltipController(TooltipTemplate template, Transform parent, string text) : base(template, parent)
        {
            Model.TooltipText = text;
            FollowingMouse = View.StartCoroutine(FollowMouse());
        }

        private IEnumerator FollowMouse()
        {
            while (!View.IsDestroyed)
            {
                Model.ScreenPos = Pointer.current.position.value + new Vector2(60 * ResolutionButton.RESOLUTION_MULT, 
                     60 * ResolutionButton.RESOLUTION_MULT);
                UpdateViewAtEndOfFrame();
                yield return null;
            }
        }

        public override void Close()
        {
            View.StopCoroutine(FollowingMouse);
            base.Close();
        }
    }
}