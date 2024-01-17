using DefaultNamespace;
using UnityEngine;

namespace UI.Core
{
    public class MoveToIconTuner : MonoBehaviour
    {
        public MoveToIconView MoveToView;
        public GameStateManager.GameState GoTo;
        public float TransitionTime;
        
        public void Start()
        {
            _ = new MoveToIconController(MoveToView, GoTo, TransitionTime);
        }
    }
}