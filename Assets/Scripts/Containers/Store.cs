using System;
using System.Collections.Generic;
using System.Linq;
using Plants;
using Services;
using UI.Plants;

namespace UI.Containers
{
    public class Store : IService, ContainerServer<TulipData, TulipController>
    {
        public List<TulipData> Elements;
        public virtual int MaxSize => 3;

        public delegate bool FilterFunction(TulipData toAdd);
        private FilterFunction Filter;
        
        public Store(FilterFunction filterFunc)
        {
            Elements = new(Enumerable.Repeat(TulipData.Empty, MaxSize));
            Filter = filterFunc;
        }

        public int AddItem(TulipData toAdd)
        {
            if (!Filter(toAdd))
                throw new ArgumentException($"Added TulipData did not match filter function! Data: {toAdd}");
            
            for (int i = 0; i < MaxSize; i++)
            {
                if (Elements[i] == TulipData.Empty)
                {
                    Elements[i] = toAdd;
                    return i;
                }
            }

            throw new IndexOutOfRangeException("All elements of Inventory are already full!");
        }

        public bool RemoveItem(int index)
        {
            var tmp = Elements[index];
            Elements[index] = TulipData.Empty;
            return tmp == TulipData.Empty;
        }
    }
}