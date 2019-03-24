using System;
using System.Collections.Generic;
using System.Text;

namespace QTFramework
{
    public class PoolDictionary<T,K>
    {
        private readonly Dictionary<T, List<K>> m_kDictMember = new Dictionary<T, List<K>>();

        private readonly Queue<List<K>> poolQueue = new Queue<List<K>>();

        public int Count
        {
            get
            {
                return this.m_kDictMember.Count;
            }
        }

        public void Add(T t, K k)
        {
            List<K> _list;
            this.m_kDictMember.TryGetValue(t, out _list);
            if (_list == null)
            {
                _list = this.FetchList();
            }
            _list.Add(k);
            this.m_kDictMember[t] = _list;
        }

        public bool Remove(T _t, K _k)
        {
            List<K> _list;
            this.m_kDictMember.TryGetValue(_t, out _list);
            if (_list == null)
            {
                return false;
            }
            if (!_list.Remove(_k))
            {
                return false;
            }
            if (_list.Count == 0)
            {
                this.RecycleList(_list);
                this.m_kDictMember.Remove(_t);
            }
            return true;
        }

        public bool Remove(T _t)
        {
            List<K> _list;
            this.m_kDictMember.TryGetValue(_t, out _list);
            if (_list != null)
            {
                this.RecycleList(_list);
            }
            return this.m_kDictMember.Remove(_t);
        }

        public List<K> this[T _t]
        {
            get
            {
                List<K> list;
                this.m_kDictMember.TryGetValue(_t, out list);
                return list;
            }
        }
        public bool Contains(T _t, K _k)
        {
            List<K> _list;
            this.m_kDictMember.TryGetValue(_t, out _list);
            if (_list == null)
            {
                return false;
            }
            return _list.Contains(_k);
        }

        public bool ContainsKey(T t)
        {
            return this.m_kDictMember.ContainsKey(t);
        }

        public void Clear()
        {
            foreach (KeyValuePair<T, List<K>> keyValuePair in this.m_kDictMember)
            {
                this.RecycleList(keyValuePair.Value);
            }
            this.m_kDictMember.Clear();
        }


        private void RecycleList(List<K> _list)
        {
            _list.Clear();
            this.poolQueue.Enqueue(_list);
        }

        private List<K> FetchList()
        {
            if (this.poolQueue.Count > 0)
            {
                List<K> list = this.poolQueue.Dequeue();
                list.Clear();
                return list;
            }
            return new List<K>();
        }
    }
}
