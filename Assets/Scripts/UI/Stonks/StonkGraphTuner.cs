using UnityEngine;

namespace UI.Stonks
{
    public class StonkGraphTuner : MonoBehaviour
    {
        public StonkGraphView StonkGraphView;
        public void Start()
        {
            _ = new StonkGraphController(StonkGraphView);
        }
    }
}