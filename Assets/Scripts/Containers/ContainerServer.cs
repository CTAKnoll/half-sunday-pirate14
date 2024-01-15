using UI.Model;

namespace UI.Containers
{
    // A ContainerServer is the backing data behind a Container, holding both the list of ContainableData and 
    // defining the way to create a UIInteractable from that ContainerItem.
    public interface ContainerServer<TContainable, TServedUI> where TServedUI: IUIController 
                                            where TContainable: Containable<TContainable, TServedUI>
    {
        int MaxSize { get; }
        int AddItem(TContainable toAdd);
        bool RemoveItem(int index);
    }
}