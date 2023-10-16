using System.Collections.Generic;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine;
using Stopwatch = System.Diagnostics.Stopwatch;
using Random = Unity.Mathematics.Random;
using ProjectDawn.Collections.LowLevel.Unsafe;

namespace ProjectDawn.Collections.Tests
{
    internal class UnsafeLinkedPriorityQueueBenchmarkTests
    {
        struct IntComparer : IComparer<int>
        {
            public int Compare(int x, int y) => x.CompareTo(y);
        }

        [Test]
        public unsafe void UnsafeLinkedPriorityQueueBenchmarkTests_Int_Enqueue()
        {
            int count = 10000;
            Stopwatch stopWatch = new Stopwatch();
            Random rnd;

            rnd = new Random(1);
            stopWatch.Restart();
            var linkedList = new UnsafeLinkedPriorityQueue<int, IntComparer>(1, Allocator.Temp);
            for (int i = 0; i < count; ++i)
                linkedList.Enqueue(rnd.NextInt(i));
            stopWatch.Stop();
            float linkedListTime = stopWatch.ElapsedMilliseconds;

            rnd = new Random(1);
            stopWatch.Restart();
            var list = new UnsafeHeap<int, int>(1, Allocator.Temp);
            for (int i = 0; i < count; ++i)
                list.Push(rnd.NextInt(i), i);
            stopWatch.Stop();
            float listTime = stopWatch.ElapsedMilliseconds;

            linkedList.Dispose();
            list.Dispose();

            Debug.Log($"UnsafeLinkedPriorityQueueBenchmarkTests_Int_Enqueue LinkedList:{linkedListTime}ms Heap:{listTime}ms");
        }

        [Test]
        public unsafe void UnsafeLinkedPriorityQueueBenchmarkTests_Int_Dequeue()
        {
            int count = 10000;
            Stopwatch stopWatch = new Stopwatch();
            Random rnd;

            rnd = new Random(1);
            
            var linkedList = new UnsafeLinkedPriorityQueue<int, IntComparer>(1, Allocator.Temp);
            for (int i = 0; i < count; ++i)
                linkedList.Enqueue(rnd.NextInt(i));
            stopWatch.Restart();
            for (int i = 0; i < count; ++i)
                linkedList.Dequeue();
            stopWatch.Stop();
            float linkedListTime = stopWatch.ElapsedMilliseconds;

            rnd = new Random(1);
            
            var list = new UnsafeHeap<int, int>(1, Allocator.Temp);
            for (int i = 0; i < count; ++i)
                list.Push(rnd.NextInt(i), i);
            stopWatch.Restart();
            for (int i = 0; i < count; ++i)
                list.Pop();
            stopWatch.Stop();
            float listTime = stopWatch.ElapsedMilliseconds;

            linkedList.Dispose();
            list.Dispose();

            Debug.Log($"UnsafeLinkedPriorityQueueBenchmarkTests_Int_Dequeue LinkedList:{linkedListTime}ms Heap:{listTime}ms");
        }
    }
}
