using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Plants;
using Services;
using UI.Model;
using UI.Plants;

namespace UI.Containers
{
    public class Store : IService, ContainerServer<TulipData, TulipController>
    {
        private Inventory.InventoryStack[] Elements;
        private InventorySlotController[] Owners;

        public virtual int MaxSize => 3;
        
        public int Count 
        {
            get
            {
                int total = 0;
                for (int i = 0; i < MaxSize; i++)
                {
                    if (Elements[i].Varietal != null)
                    {
                        if (Owners[i].IsEmpty)
                            throw new Exception("ContainerServer out of sync!");
                        total++;
                    }
                    else
                    {
                        if (!Owners[i].IsEmpty)
                            throw new Exception("ContainerServer out of sync!");
                    }
                }
                return total;
            }
        }
        
        
        public Store(List<InventorySlotController> owners)
        {
            Elements = new Inventory.InventoryStack[MaxSize];
            Owners = new InventorySlotController[MaxSize];
            
            for (int i = 0; i < MaxSize; i++)
            {
                Elements[i] = new Inventory.InventoryStack(null, 0);
                Owners[i] = owners[i];
            }
        }

        public bool AddItem(TulipData toAdd, [CanBeNull] Action<TulipController, IUIController> onConsumed, out TulipController added)
        {
            for (int i = 0; i < MaxSize; i++)
            {
                if (Elements[i].IsEmpty())
                {
                    Elements[i] = new Inventory.InventoryStack(toAdd.Varietal, 1);
                    Owners[i].UpdateSlot(Elements[i]);
                    added = Owners[i].Tulip;
                    return true;
                }
            }

            added = null;
            return false;
        }

        public TulipController GetItem(int index) => Owners[index].Tulip;
        
        public bool HasItem(TulipData variety)
        {
            return Elements.Any(elem => elem.Varietal.Equals(variety.Varietal));
        }

        public void RemoveItem(int index)
        {
            Elements[index] = new Inventory.InventoryStack(null, 0);
            Owners[index].UpdateSlot(Elements[index]);
        }
        
        public bool RemoveItem(TulipData item, int amount = 1)
        {
            int index = Array.IndexOf(Owners.Select(controller => controller?.Tulip.Data).ToArray(), item);
            if (index == -1) return false;
            Elements[index] = new Inventory.InventoryStack(null, 0);
            Owners[index].UpdateSlot(Elements[index]);
            return true;
        }
        
        public bool IsEmpty(int index) => Elements[index].IsEmpty();
        public bool HasEmpty() => Elements.Any(t => t.IsEmpty());
    }
}