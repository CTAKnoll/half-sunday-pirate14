using System.Collections;
using UI.Model.Templates;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Core
{
    public class TooltipController : UIController<TooltipView, TooltipModel>
    {
        private Coroutine FollowingMouse;
        public TooltipController(TooltipTemplate template, Transform parent, UIInteractable target) : base(template, parent)
        {
            Model.TooltipText = target.TooltipText;
            FollowingMouse = View.StartCoroutine(FollowMouse());
        }

        private IEnumerator FollowMouse()
        {
            while (!View.IsDestroyed)
            {
                Model.ScreenPos = MainCamera.ScreenToWorldPoint(Pointer.current.position.value);
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