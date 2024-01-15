using System.Collections.Generic;
using UI.Model;

namespace Services
{
    // AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH
    public class ControllerDatabase : IService
    {
        private Dictionary<UIInteractable, IUIController> Mapping;

        public ControllerDatabase()
        {
            Mapping = new();
        }

        public void Register(UIInteractable view, IUIController controller)
        {
            Mapping.Add(view, controller);
        }

        public bool GetControllerFromView(UIInteractable view, out IUIController result)
        {
            return Mapping.TryGetValue(view, out result);
        }
    }
}