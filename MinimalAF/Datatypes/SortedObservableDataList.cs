using System;
using System.Collections.Generic;
using System.Text;
using MinimalAF.Datatypes;

namespace MinimalAF.Datatypes
{
    /// <summary>
    /// Uses the events fired by observableData to keep the list sorted even when internal elements have changed.
    /// 
    /// Warning: Not threadsafe.
    /// </summary>
    public class SortedObservableDataList<T> where T : ObservableData<T>, IComparable<T>, IIndexed
    {
        List<T> _elements;

        public T this[int index] {
            get {
                return _elements[index];
            }
        }

        public int Count {
            get { return _elements.Count; }
        }

        public void Add(T obj)
        {
            if (obj == null)
                return;

            obj.ArrayIndex = _elements.Count;
            obj.OnDataChanged += OnInternalDataChanged;

            _elements.Add(obj);

            EnsureSorted(obj);
        }

        public SortedObservableDataList()
        {
            _elements = new List<T>();
        }

        public SortedObservableDataList(List<T> objects)
        {
            _elements = objects;
            _elements.Sort();

            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].OnDataChanged += OnInternalDataChanged;
            }

            ReindexFrom(0);
        }

        private void OnInternalDataChanged(T obj)
        {
            EnsureSorted(obj);
        }

        public int IndexOf(T obj)
        {
            int index = obj.ArrayIndex;

            if (_elements[index] != obj)
                throw new Exception("Internal data structure indexing error");

            return index;
        }

        public void Remove(T obj)
        {
            if (obj == null)
                return;

            int index = IndexOf(obj);

            if (index < 0)
                return;

            RemoveAt(index);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _elements.Count)
                return;

            _elements[index].OnDataChanged -= OnInternalDataChanged;
            _elements.RemoveAt(index);

            ReindexFrom(index);
        }


        private void ReindexFrom(int start)
        {
            for (int i = start; i < _elements.Count; i++)
            {
                _elements[i].ArrayIndex = i;
            }
        }

        public void Clear()
        {
            for (int i = _elements.Count - 1; i >= 0; i--)
            {
                _elements[i].OnDataChanged -= OnInternalDataChanged;
                _elements[i].ArrayIndex = -1;
            }

            _elements.Clear();
        }

        private void EnsureSorted(T obj)
        {
            int index = IndexOf(obj);

            if (index >= 0 && index <= _elements.Count - 1)
            {
                bool shouldBeMovedDown = index > 0 && (_elements[index].CompareTo(_elements[index - 1]) < 0);
                bool shouldBeMovedUp = index < _elements.Count - 1 && (_elements[index].CompareTo(_elements[index + 1]) > 0);

                //It is possible to do this with just one function but I cant be arsed
                if (shouldBeMovedDown)
                {
                    MoveDown(obj);
                }
                else if (shouldBeMovedUp)
                {
                    MoveUp(obj);
                }
            }
        }

        private void MoveDown(T obj)
        {
            int index = obj.ArrayIndex;

            for (int i = index; i > 0; i--)
            {
                if (_elements[i].CompareTo(_elements[i - 1]) < 0)
                {
                    var temp = _elements[i];
                    _elements[i] = _elements[i - 1];
                    _elements[i - 1] = temp;

                    _elements[i].ArrayIndex = i;
                    _elements[i - 1].ArrayIndex = i - 1;
                }
                else
                {
                    break;
                }
            }
        }

        private void MoveUp(T obj)
        {
            int index = obj.ArrayIndex;

            for (int i = index; i < _elements.Count - 1; i++)
            {
                if (_elements[i].CompareTo(_elements[i + 1]) > 0)
                {
                    var temp = _elements[i];
                    _elements[i] = _elements[i + 1];
                    _elements[i + 1] = temp;

                    _elements[i].ArrayIndex = i;
                    _elements[i + 1].ArrayIndex = i + 1;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
