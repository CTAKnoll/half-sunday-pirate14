﻿using System;
using UI.Model;

namespace UI.Containers
{
    // A ContainerServer is the backing data behind a Container, holding both the list of ContainableData and 
    // defining the way to create a UIInteractable from that ContainerItem.
    public interface ContainerServer<TContainable, TServedUI> where TServedUI: IUIController 
                                            where TContainable: Containable<TContainable, TServedUI>
    {
        int MaxSize { get; }
        int Count { get; }
        bool AddItem(TContainable toAdd, Action<TServedUI, IUIController> consumeCallback, out TServedUI added);
        TServedUI GetItem(int index);
        bool HasItem(TContainable item);
        void RemoveItem(int index);
        bool RemoveItem(TContainable item, int amount = 1);
        bool IsEmpty(int index);
        bool HasEmpty();
    }
}