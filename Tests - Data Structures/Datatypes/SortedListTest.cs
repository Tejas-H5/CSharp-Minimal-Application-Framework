using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalAF.Datatypes;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace Tests___Data_Structures
{
    class SomeObject : ObservableData<SomeObject>, IComparable<SomeObject>, IIndexed
    {
        private float _value = 0;
        public float Value {
            get { return _value; }
            set { 
                _value = value;
                DataChanged(this);
            }
        }

        public SomeObject(float value)
        {
            _value = value;
        }

        public int ArrayIndex { get; set; } = -1;

        public int CompareTo(SomeObject other)
        {
            return _value.CompareTo(other.Value);
        }
    }


    [TestClass]
    public class SortedObservableDataListTest
    {

        [TestMethod]
        public void AddSeveralHitobjects_ShouldRemainSorted()
        {
            SortedObservableDataList<SomeObject> sortedList = new SortedObservableDataList<SomeObject>();

            sortedList.Add(new SomeObject(3.0f));
            sortedList.Add(new SomeObject(2.0f));
            sortedList.Add(new SomeObject(7.0f));
            sortedList.Add(new SomeObject(5.0f));
            sortedList.Add(new SomeObject(1.0f));
            sortedList.Add(new SomeObject(4.0f));

            AssertSorted(sortedList);
        }

        [TestMethod]
        public void AddSingleHitObject_ShouldRemainSorted()
        {
            PointF origin = new PointF(0, 0);

            SortedObservableDataList<SomeObject> sortedList = new SortedObservableDataList<SomeObject>();

            sortedList.Add(new SomeObject(3.0f));

            AssertSorted(sortedList);
        }

        [TestMethod]
        public void AddMultipleObjectsWithSameTime_ShouldRemainSorted()
        {
            PointF origin = new PointF(0, 0);

            SortedObservableDataList<SomeObject> sortedList = new SortedObservableDataList<SomeObject>();

            sortedList.Add(new SomeObject(3.0f));
            sortedList.Add(new SomeObject(3.0f));
            sortedList.Add(new SomeObject(3.0f));
            sortedList.Add(new SomeObject(3.0f));
            sortedList.Add(new SomeObject(3.0f));
            sortedList.Add(new SomeObject(3.0f));

            AssertSorted(sortedList);
        }


        [TestMethod]
        public void DeleteAllObjects_ShouldntThrowExceptions()
        {
            PointF origin = new PointF(0, 0);

            SortedObservableDataList<SomeObject> sortedList = new SortedObservableDataList<SomeObject>();

            sortedList.Add(new SomeObject(3.0f));
            sortedList.Add(new SomeObject(2.0f));
            sortedList.Add(new SomeObject(4.0f));
            sortedList.Add(new SomeObject(5.0f));
            sortedList.Add(new SomeObject(1.0f));
            sortedList.Add(new SomeObject(7.0f));

            sortedList.RemoveAt(0);
            sortedList.RemoveAt(0);
            sortedList.RemoveAt(234);
            sortedList.RemoveAt(-234);
            sortedList.RemoveAt(0);
            sortedList.RemoveAt(0);
            sortedList.RemoveAt(0);
            sortedList.RemoveAt(0);
            sortedList.RemoveAt(2);
            sortedList.RemoveAt(0);
        }


        [TestMethod]
        public void MoveSomeObjects_ShouldRemainSorted()
        {
            PointF origin = new PointF(0, 0);

            SortedObservableDataList<SomeObject> sortedList = new SortedObservableDataList<SomeObject>();

            SomeObject c1, c2, c3;

            sortedList.Add(c1 = new SomeObject(3.0f));
            sortedList.Add(c2 = new SomeObject(2.0f));
            sortedList.Add(c3 = new SomeObject(4.0f));
            sortedList.Add(new SomeObject(5.0f));
            sortedList.Add(new SomeObject(1.0f));
            sortedList.Add(new SomeObject(7.0f));

            c1.Value = 0;
            c2.Value = 10;
            c3.Value = -2;

            AssertSorted(sortedList);
        }

        [TestMethod]
        public void InitUnsortedList_ShouldRemainSorted()
        {
            PointF origin = new PointF(0, 0);

            List<SomeObject> list = new List<SomeObject>();

            SomeObject c1, c2, c3;
            list.Add(c1 = new SomeObject(3.0f));
            list.Add(c2 = new SomeObject(2.0f));
            list.Add(c3 = new SomeObject(4.0f));
            list.Add(new SomeObject(5.0f));
            list.Add(new SomeObject(1.0f));
            list.Add(new SomeObject(7.0f));

            SortedObservableDataList<SomeObject> sortedList = new SortedObservableDataList<SomeObject>(list);

            AssertSorted(sortedList);
        }

        [TestMethod]
        public void DeleteSomeObjects_ShouldRemainSorted()
        {
            PointF origin = new PointF(0, 0);

            SortedObservableDataList<SomeObject> sortedList = new SortedObservableDataList<SomeObject>();

            SomeObject c1, c2, c3;

            sortedList.Add(c1 = new SomeObject(3.0f));
            sortedList.Add(c2 = new SomeObject(2.0f));
            sortedList.Add(c3 = new SomeObject(4.0f));
            sortedList.Add(new SomeObject(5.0f));
            sortedList.Add(new SomeObject(1.0f));
            sortedList.Add(new SomeObject(7.0f));

            sortedList.Remove(c1);
            sortedList.Remove(c2);

            sortedList.RemoveAt(3);

            AssertSorted(sortedList);
        }

        private void AssertSorted(SortedObservableDataList<SomeObject> sortedList)
        {
            for (int i = 0; i < sortedList.Count - 1; i++)
            {
                Assert.IsTrue(sortedList[i].Value <= sortedList[i + 1].Value);
            }
        }
    }
}
