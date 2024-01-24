using DefaultNamespace;
using Services;
using UnityEngine;

namespace UI.Core
{
    public class MoveToIconController : UIController<MoveToIconView, MoveToIconModel>
    {
        public MoveToIconController(MoveToIconView view, GameStateManager.GameState goTo, float time) : base(view)
        {
            void OnTap()
            {
                ServiceLocator.LazyLoad<GameStateManager>().PanToState(goTo, time);
                Audio.PlayOneShot(View.sfx_onClick);
            }
            UiDriver.RegisterForTap(View, OnTap );
        }
    }
}