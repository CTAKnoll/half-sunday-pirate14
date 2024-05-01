using UnityEngine;

namespace UI
{
    public class StatusTextTuner : MonoBehaviour
    {
        public StatusTextView StatusTextView;

        public void Start()
        {
            _ = new StatusTextController(StatusTextView);
        }
    }
}