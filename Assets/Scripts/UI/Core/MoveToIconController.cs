using DefaultNamespace;
using Services;
using UnityEngine;

namespace UI.Core
{
    public class MoveToIconController : UIController<MoveToIconView, MoveToIconModel>
    {
        public MoveToIconController(MoveToIconView view, GameStateManager.GameState goTo, float time) : base(view)
        {
            UiDriver.RegisterForTap(View, () => ServiceLocator.LazyLoad<GameStateManager>().PanToState(goTo, time));
        }
    }
}