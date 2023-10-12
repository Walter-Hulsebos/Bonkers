using System;
using NUnit.Framework;
using Unity.Collections;
using ProjectDawn.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace ProjectDawn.Collections.Tests
{
    internal class UnsafeHeapTests
    {
        [Test]
        public unsafe void UnsafeHeapTests_Int_Push_Pop()
        {
            var heap = new UnsafeHeap<int, int>(1, Allocator.Temp);

            heap.Push(5, 2);
            heap.Push(2, 1);
            heap.Push(0, 0);
            heap.Push(10, 3);

            Assert.AreEqual(0, heap.Pop());
            Assert.AreEqual(1, heap.Pop());
            Assert.AreEqual(2, heap.Pop());
            Assert.AreEqual(3, heap.Pop());

            heap.Dispose();
        }

        [Test]
        public unsafe void UnsafeHeapTests_Int_Peek()
        {
            var heap = new UnsafeHeap<int, int>(1, Allocator.Temp);

            heap.Push(5, 2);
            heap.Push(2, 1);
            heap.Push(0, 0);
            heap.Push(10, 3);

            Assert.AreEqual(0, heap.Pop());
            Assert.AreEqual(1, heap.Peek());
            Assert.AreEqual(1, heap.Pop());
            Assert.AreEqual(2, heap.Pop());
            Assert.AreEqual(3, heap.Pop());

            heap.Dispose();
        }

        [Test]
        public unsafe void UnsafeHeapTests_Int_TryPop()
        {
            var heap = new UnsafeHeap<int, int>(1, Allocator.Temp);

            heap.Push(5, 2);
            heap.Push(2, 1);
            heap.Push(0, 0);
            heap.Push(10, 3);

            if (heap.TryPop(out int value))
                Assert.AreEqual(0, value);
            if (heap.TryPop(out value))
                Assert.AreEqual(1, value);
            if (heap.TryPop(out value))
                Assert.AreEqual(2, value);
            if (heap.TryPop(out value))
                Assert.AreEqual(3, value);
            if (heap.TryPop(out value))
                Assert.AreEqual(3, value);

            heap.Dispose();
        }

        [Test]
        public unsafe void UnsafeHeapTests_Int_TrimExcess()
        {
            var heap = new UnsafeHeap<int, int>(1, Allocator.Temp);

            heap.Push(5, 2);
            heap.Push(2, 1);
            heap.Push(0, 0);
            heap.Push(10, 3);

            Assert.AreNotEqual(heap.Length, heap.Capacity);

            heap.TrimExcess();

            Assert.AreEqual(heap.Length, heap.Capacity);

            heap.Dispose();
        }

        [Test]
        public unsafe void UnsafeHeapTests_Int_Clear()
        {
            var heap = new UnsafeHeap<int, int>(1, Allocator.Temp);

            heap.Push(5, 2);
            heap.Push(2, 1);
            heap.Push(0, 0);
            heap.Push(10, 3);

            heap.Clear();

            Assert.IsTrue(heap.IsEmpty);

            heap.Dispose();
        }

        [Test]
        public unsafe void UnsafeHeapTests_DistanceToEnemy()
        {
            using var enemies = new NativeList<Enemy>(Allocator.Temp)
            {
                new Enemy(new float3(0.9f, 0, 1), 1),
                new Enemy(new float3(0.6f, 0, 1), 1),
                new Enemy(new float3(0.3f, 0, 1), 1),
                new Enemy(new float3(-0.3f, 0, 1), 1),
                new Enemy(new float3(-0.6f, 0, 1), 1),
                new Enemy(new float3(-0.9f, 0, 1), 1),

                new Enemy(new float3(0.9f, 0, 1), 1),
                new Enemy(new float3(0.6f, 0, 1), 1),
                new Enemy(new float3(0.3f, 0, 1), 1),
                new Enemy(new float3(-0.3f, 0, 1), 1),
                new Enemy(new float3(-0.6f, 0, 1), 1),
                new Enemy(new float3(-0.9f, 0, 1), 1),
            };

            float3 heroPos = new float3(0, 0, -1);

            var count = enemies.Length;
            using var heap = new UnsafeHeap<Priority, int>(count, Allocator.Temp);

            for (int i = 0; i < count; i++)
            {
                var enemy = enemies[i];
                var position = enemy.position;
                var distance = math.distance(heroPos, position);

                var priority = new Priority
                {
                    distance = distance,
                    hp = enemy.hp,
                };

                heap.Push(priority, i);
            }

            Assert.AreEqual(0, heap.Pop());
        }

        struct Enemy
        {
            public float3 position;
            public float hp;
            public Enemy(float3 position, float hp)
            {
                this.position = position;
                this.hp = hp;
            }
        }

        struct Priority : IComparable<Priority>
        {
            public float distance;
            public float hp;

            public readonly int CompareTo(Priority other)
            {
                var res = other.distance.CompareTo(distance);
                return res == 0 ? other.hp.CompareTo(hp) : res;
            }
        }

        /*[Test]
        public unsafe void UnsafeHeapTests_Int_GetArray()
        {
            var heap = new UnsafeHeap<int, int>(1, Allocator.Temp);

            heap.Push(5, 2);
            heap.Push(2, 1);
            heap.Push(0, 0);
            heap.Push(10, 3);

            var keys = heap.GetKeyArray(Allocator.TempJob);
            Assert.AreEqual(0, keys[0]);
            Assert.AreEqual(2, keys[1]);
            Assert.AreEqual(5, keys[2]);
            Assert.AreEqual(10, keys[3]);
            keys.Dispose();

            var values = heap.GetValueArray(Allocator.TempJob);
            Assert.AreEqual(0, values[0]);
            Assert.AreEqual(1, values[1]);
            Assert.AreEqual(2, values[2]);
            Assert.AreEqual(3, values[3]);
            values.Dispose();

            heap.Dispose();
        }*/
    }
}
