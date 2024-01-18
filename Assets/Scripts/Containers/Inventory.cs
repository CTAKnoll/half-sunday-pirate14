using System;
using System.Collections.Generic;
using System.Linq;
using UI.Model;
using UI.Plants;
using UnityEngine;
using static Plants.TulipData;

namespace UI.Containers
{
    public class Inventory : ContainerServer<TulipVarietal, TulipController>
    {
        private TulipVarietal[] Elements;
        private InventorySlotController[] Owners;
        public virtual int MaxSize => 5;
        public virtual int PageSize => 5;

        public int Count 
        {
            get
            {
                int total = 0;
                for (int i = 0; i < MaxSize; i++)
                {
                    if (Elements[i].Kind != TulipKind.Empty)
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

        public Inventory(List<InventorySlotController> owners)
        {
            Elements = new TulipVarietal[MaxSize];
            Owners = new InventorySlotController[MaxSize];
            for (int i = 0; i < MaxSize; i++)
            {
                Elements[i] = null;
                Owners[i] = owners[i];
            }
        }
        
        public TulipController AddItem(TulipVarietal toAdd, Action<TulipController, IUIController> onConsumed)
        {
            for (int i = 0; i < MaxSize; i++) // look for stack first
            {
                if (Elements[i]?.Equals(toAdd) ?? false)
                {
                    Owners[i].ConsumeFunction = onConsumed;
                    Owners[i].UpdateSlot(toAdd, Owners[i].Stacks + 1);
                    return Owners[i].Tulip;
                }
            }
            for (int i = 0; i < MaxSize; i++) // look for first empty
            {
                if (Elements[i] == null)
                {
                    Elements[i] = toAdd;
                    Owners[i].ConsumeFunction = onConsumed;
                    Owners[i].UpdateSlot(toAdd, 1);
                    return Owners[i].Tulip;
                }
            }

            throw new IndexOutOfRangeException("All elements of Inventory are already full!");
        }
        
        public TulipController GetItem(int index) => Owners[index].Tulip;

        public void RemoveItem(int index)
        {
            Elements[index] = null;
            Owners[index].UpdateSlot(null, 0);
        }
        
        public bool RemoveItem(TulipController controller, int amount = 1)
        {
            Debug.Log("hihii");
            int index = Array.IndexOf(Owners.Select(owner => owner.Tulip).ToArray(), controller);
            if (index == -1) return false;
            int finalAmt = Owners[index].Stacks - amount;
            if (finalAmt <= 0)
            {
                Debug.Log("hoihi");
                Elements[index] = null;
                Owners[index].UpdateSlot(null, 0);
            }
            else
            {
                Owners[index].UpdateSlot(Owners[index].Tulip.Data.Varietal, finalAmt);
            }
            return true;
        }

        public bool IsEmpty(int index) => Elements[index] == null;
        public bool HasEmpty() => Elements.Contains(null);
    }
}