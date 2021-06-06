using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinimalAF.Datatypes
{
    // convenience class for working with arrays in place to reduce memory allocation/deallocation
    // It is inspired by the way python deals with arrays internally (or at least this is how I think it does)
    // Something unique about this is that it can be given a stride into an existing array.
    // This might make debugging hard, but it makes extracting and transforming soley the 
    // right channel from an audio signal in-place very easy
    // At the time, I hadn't heard of Span<T>s though. When they eventually give it a stride parameter, this class will
    // become obsolete

    public struct Slice<T>
    {
        int start;
        int len;
        int stride;

        T[] array;

        public T[] GetInternalArray()
        {
            return array;
        }

        public int Length => len;

        //Mainly for debugging purposes, but do have their convenient uses
        public int InternalStart => start;
        public int InternalEnd => start + len * stride;
        public int InternalStride => stride;

        public T this[int index] {
            get {
                return array[start + index * stride];
            }
            set { array[start + index * stride] = value; }
        }

        public Slice(T[] arr)
        {
            array = arr;
            start = 0;
            len = arr.Length;
            stride = 1;
        }

        //Permits indexing outside of the defined slice bounds. Make sure you know what you are doing though
        public Slice<T> GetSlice(int newStart, int newEnd, int newStride = 1)
        {
            if (newEnd * stride > array.Length)
            {
                //Breakpoint
            }

            return new Slice<T>(
                array,
                start + newStart * stride,
                start + newEnd * stride,
                stride * newStride
            );
        }

        public Slice(T[] arr, int start, int end, int stride = 1)
        {
            array = arr;
            this.start = start;
            len = (end - start) / stride;
            this.stride = stride;
        }

        public Slice<T> DeepCopy()
        {
            T[] arr = new T[len];
            return DeepCopy(arr);
        }

        public Slice<T> DeepCopy(T[] buffer)
        {
            for (int i = 0; i < len; i++)
            {
                buffer[i] = array[start + i * stride];
            }

            return new Slice<T>(buffer);
        }
    }
}
