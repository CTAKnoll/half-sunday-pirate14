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
        public class InventoryStack
        {
            public TulipVarietal Varietal;
            public int Count;

            public InventoryStack(TulipVarietal varietal, int count)
            {
                Varietal = varietal;
                Count = count;
            }

            public bool IsEmpty() => Count == 0;
        }
        
        private InventoryStack[] Elements;
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
                    if (Elements[i].Varietal.Kind != TulipKind.Empty)
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
            Elements = new InventoryStack[MaxSize];
            Owners = new InventorySlotController[MaxSize];
            for (int i = 0; i < MaxSize; i++)
            {
                Elements[i] = new InventoryStack(null, 0);
                Owners[i] = owners[i];
            }
        }
        
        public TulipController AddItem(TulipVarietal toAdd, Action<TulipController, IUIController> onConsumed)
        {
            for (int i = 0; i < MaxSize; i++) // look for stack first
            {
                if (Elements[i].Varietal?.Equals(toAdd) ?? false)
                {
                    Owners[i].ConsumeFunction = onConsumed;
                    Owners[i].UpdateSlot(new InventoryStack(toAdd, Owners[i].Stacks + 1));
                    return Owners[i].Tulip;
                }
            }
            for (int i = 0; i < MaxSize; i++) // look for first empty
            {
                if (Elements[i].IsEmpty())
                {
                    Elements[i] = new InventoryStack(toAdd, 1);
                    Owners[i].ConsumeFunction = onConsumed;
                    Owners[i].UpdateSlot(new InventoryStack(toAdd, 1));
                    return Owners[i].Tulip;
                }
            }

            throw new IndexOutOfRangeException("All elements of Inventory are already full!");
        }
        
        public TulipController GetItem(int index) => Owners[index].Tulip;

        public void RemoveItem(int index)
        {
            Elements[index] = null;
            Owners[index].UpdateSlot(new InventoryStack(null, 0));
            Collapse();
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
                Elements[index] = new InventoryStack(null, 0);
                Owners[index].UpdateSlot(new InventoryStack(null, 0));
            }
            else
            {
                Owners[index].UpdateSlot(new InventoryStack(Owners[index].Tulip.Data.Varietal, finalAmt));
            }

            Collapse();
            return true;
        }

        private void Collapse()
        {
            for (int i = 0; i < Elements.Length; i++)
            {
                int lookAhead = 0;
                if (Elements[i].IsEmpty())
                {
                    while (Elements[i + lookAhead].IsEmpty() && i + lookAhead < Elements.Length - 1)
                        lookAhead++;

                    if (i + lookAhead == Elements.Length - 1)
                        break;
                    
                    Elements[i] = Elements[i + lookAhead];
                    Elements[i + lookAhead] = new InventoryStack(null, 0);
                }
            }
            
            // clean up correctly
            for (int i = 0; i < Owners.Length; i++)
            {
                Owners[i].UpdateSlot(Elements[i]);
            }
        }

        public bool IsEmpty(int index) => Elements[index].IsEmpty();
        public bool HasEmpty() => Elements.Any(t => t.IsEmpty());
    }
}