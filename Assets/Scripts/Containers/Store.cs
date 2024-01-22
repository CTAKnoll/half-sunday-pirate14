using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Plants;
using Services;
using UI.Model;
using UI.Plants;
using UnityEngine;

namespace UI.Containers
{
    public class Store : IService, ContainerServer<TulipData, TulipController>
    {
        private TulipData[] Elements;
        private TulipController[] Controllers;
        private Transform[] Owners;

        public virtual int MaxSize => 3;
        
        public int Count 
        {
            get
            {
                int total = 0;
                for (int i = 0; i < MaxSize; i++)
                {
                    if (Elements[i] != null)
                    {
                        if (Controllers[i] == null)
                            throw new Exception("ContainerServer out of sync!");
                        total++;
                    }
                    else
                    {
                        if (Controllers[i] != null)
                            throw new Exception("ContainerServer out of sync!");
                    }
                }
                return total;
            }
        }
        
        
        public Store(List<UIInteractable> owners)
        {
            Elements = new TulipData[MaxSize];
            Controllers = new TulipController[MaxSize];
            
            Owners = new Transform[MaxSize];
            for (int i = 0; i < MaxSize; i++)
            {
                Elements[i] = null;
                Controllers[i] = null;
                Owners[i] = owners[i].transform;
            }
        }

        public TulipController AddItem(TulipData toAdd, [CanBeNull] Action<TulipController, IUIController> onConsumed)
        {
            for (int i = 0; i < MaxSize; i++)
            {
                if (Elements[i] == null)
                {
                    Elements[i] = toAdd;
                    Controllers[i] = toAdd.Serve(Owners[i]);
                    Controllers[i].Consumed += onConsumed;
                    return Controllers[i];
                }
            }

            throw new IndexOutOfRangeException("All elements of Store are already full!");
        }

        public TulipController GetItem(int index) => Controllers[index];
        
        public bool HasItem(TulipData variety)
        {
            return Elements.Any(elem => elem.Varietal.Equals(variety.Varietal));
        }

        public void RemoveItem(int index)
        {
            Elements[index] = null;
            Controllers[index] = null;
        }
        
        public bool RemoveItem(TulipData item, int amount = 1)
        {
            int index = Array.IndexOf(Controllers.Select(controller => controller?.Data).ToArray(), item);
            if (index == -1) return false;
            Elements[index] = null;
            Controllers[index] = null;
            return true;
        }
        
        public bool IsEmpty(int index) => Elements[index] == null;
        public bool HasEmpty() => Elements.Contains(null);
    }
}