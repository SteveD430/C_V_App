﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


namespace C_V_App.CollectionExtensions
{
    public static class Extensions
    {
        public static void Sort<T>(this ObservableCollection<T> observable) where T : IComparable<T>, IEquatable<T>
        {
            List<T> sorted = observable.OrderBy(x => x).ToList();

            int ptr = 0;
            while (ptr < sorted.Count)
            {
                if (!observable[ptr].Equals(sorted[ptr]))
                {
                    T t = observable[ptr];
                    observable.RemoveAt(ptr);
                    observable.Insert(sorted.IndexOf(t), t);
                }
                ptr++;
            }
        }
    }
}
