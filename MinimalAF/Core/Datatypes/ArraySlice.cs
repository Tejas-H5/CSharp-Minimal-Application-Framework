using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MinimalAF {
    public static class ContainerSliceExtensions {
        public static ArraySlice<T> Slice<T>(this IList<T> container, int start, int end) {
            return new ArraySlice<T>(container, start, end);
        }
    }
    
    // Just for syntactic sugar. If Conversions could happen between interfaces and actual instances, this
    // would probably be a real thing
    public struct ArraySlice<T> : IEnumerable<T>{
        public IList<T> Container;
        public int Start;
        public int End;

        public ArraySlice(IList<T> container, int a, int b) {
            Container = container;

            if (a < b) {
                Start = a;
                End = b;
            } else {
                Start = b;
                End = a;
            }
        }

        public static implicit operator ArraySlice<T>(List<T> list) => new ArraySlice<T>(list, 0, list.Count);
        public static implicit operator ArraySlice<T>(T[] array) => new ArraySlice<T>(array, 0, array.Length);

        public T this[int index] {
            get {
                return Container[Start + index];
            }
        }

        public int Length {
            get {
                return End - Start;
            }
        }

        public IEnumerator<T> GetEnumerator() {
            for (int i = 0; i < Length; i++) {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
