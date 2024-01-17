using DefaultNamespace;
using Services;

namespace UI.Core
{
    public class MoveToIconController : UIController<MoveToIconView, MoveToIconModel>
    {
        public MoveToIconController(MoveToIconView view, GameStateManager.GameState goTo, float time) : base(view)
        {
            UiDriver.RegisterForTap(View, () => ServiceLocator.GetService<GameStateManager>().PanToState(goTo, time));
        }
    }
}