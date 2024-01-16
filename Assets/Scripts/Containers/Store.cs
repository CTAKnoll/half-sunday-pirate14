using System;
using System.Collections.Generic;
using System.Linq;
using Plants;
using Services;
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
                    if (Elements[i] != TulipData.Empty)
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

        public delegate bool FilterFunction(TulipData toAdd);
        private FilterFunction Filter;
        
        public Store(FilterFunction filterFunc, List<UIInteractable> owners)
        {
            Elements = new TulipData[MaxSize];
            Controllers = new TulipController[MaxSize];
            
            Owners = new Transform[MaxSize];
            for (int i = 0; i < MaxSize; i++)
            {
                Elements[i] = TulipData.Empty;
                Controllers[i] = null;
                Owners[i] = owners[i].transform;
            }
            
            Filter = filterFunc;
        }

        public TulipController AddItem(TulipData toAdd)
        {
            if (!Filter(toAdd))
                throw new ArgumentException($"Added TulipData did not match filter function! Data: {toAdd}");
            
            for (int i = 0; i < MaxSize; i++)
            {
                if (Elements[i] == TulipData.Empty)
                {
                    Debug.Log($"Serving {i} {toAdd}");
                    Elements[i] = toAdd;
                    Controllers[i] = toAdd.Serve(Owners[i]);
                    return Controllers[i];
                }
            }

            throw new IndexOutOfRangeException("All elements of Store are already full!");
        }

        public TulipController GetItem(int index) => Controllers[index];

        public void RemoveItem(int index)
        {
            Elements[index] = TulipData.Empty;
            Controllers[index] = null;
        }
        
        public bool RemoveItem(TulipController controller)
        {
            int index = Array.IndexOf(Controllers, controller);
            if (index == -1) return false;
            Elements[index] = TulipData.Empty;
            Controllers[index] = null;
            return true;
        }
        
        public bool IsEmpty(int index) => Elements[index] == TulipData.Empty;
        public bool HasEmpty() => Elements.Contains(TulipData.Empty);
    }
}