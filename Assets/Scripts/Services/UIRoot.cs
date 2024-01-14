using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Services
{
    public class UIRoot : UIInteractable
    {
        public  GraphicRaycaster raycaster;
        public PlayerInput input;
        
        public InputActionReference point; 
        public InputActionReference navigate; 
        public InputActionReference tap; 
        public InputActionReference hold; 
        public InputActionReference altTap;
        public InputActionReference altHold;
        public InputActionReference scroll;
        public InputActionReference back;
        
        private UIDriver Controller;

        public void Awake()
        { 
            Controller = new UIDriver(this);
        }
    }
}