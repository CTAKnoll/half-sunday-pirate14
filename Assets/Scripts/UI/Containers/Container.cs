using UI.Model;

namespace UI.Containers
{
    public interface Container<TContainable, TServedUI> where TServedUI: IUIController 
                                                        where TContainable: Containable<TContainable, TServedUI>
    {
        ContainerServer<TContainable, TServedUI> Server { get; }
    }
}