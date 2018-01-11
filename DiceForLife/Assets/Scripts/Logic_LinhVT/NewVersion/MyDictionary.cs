using System;
using System.Collections.Generic;

namespace CoreLib
{
    public class MyDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public MyDictionary() : base() { }

        public MyDictionary(int capacity) : base(capacity) { }

        public MyDictionary(IEqualityComparer<TKey> comparer) : base (comparer) {}

        public MyDictionary(int capacity, IEqualityComparer<TKey> comparer) : base (capacity, comparer) {}

        public MyDictionary(IDictionary<TKey, TValue> dictionary) : base (dictionary) {}

        public MyDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base (dictionary, comparer) {}

        public void Add(TKey key, TValue values) {
            if (ContainsKey(key)) Remove(key);
            base.Add(key, values);
            //Console.Write("chua "+key+" "+ContainsKey(key));
        }

        public static MyDictionary<TKey, TValue> convertToMyDictionary(Dictionary<TKey, TValue> dic)
        {
            MyDictionary<TKey, TValue> dic2 = new MyDictionary<TKey, TValue>();
            foreach (TKey key in dic.Keys)
            {
                dic2.Add(key, dic[key]);
            }
            return dic2;
        }

    }



}
