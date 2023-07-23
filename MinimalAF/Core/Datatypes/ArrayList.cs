using System;

namespace MinimalAF {
    public struct ArrayList<T> {
        public T[] Data = new T[32];
        public int Length = 0;

        public static implicit operator ReadOnlySpan<T>(ArrayList<T> al) 
            => al.AsSpan();

        public ArrayList() {}

        public ReadOnlySpan<T> AsSpan() {
            return new ReadOnlySpan<T>(Data, 0, Length);
        }

        public ArrayList(T[] data, int length) {
            Data = data;
            Length = length;
        }

        public int Capacity => Data.Length;

        public void Clear() {
            Length = 0;
        }

        public void Resize(int newSize) {
            T[] newData = new T[newSize];
            int n = newSize < Data.Length ? newSize : Data.Length;
            Array.Copy(Data, newData, n);
            Data = newData;
        }

        public void Append(T item) {
            if (Length >= Data.Length) {
                Resize(MathHelpers.Max(1, Data.Length * 2));
            }

            Data[Length] = item;
            Length++;
        }

        public void InsertAt(int pos, T val) {
            Append(default(T));

            for (int i = Length - 1; i > pos; i--) {
                Data[i] = Data[i - 1];
            }

            Data[pos] = val;
        }

        public void RemoveAt(int pos) {
            RemoveRange(pos, 1);
        }

        public void RemoveRange(int start, int count) {
            if (count > Length) {
                count = Length;
            }

            for (int i = start; i < Length - count; i++) {
                Data[i] = Data[i + count];
            }

            Length -= count;
        }
    }
}
