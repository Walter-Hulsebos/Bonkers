using NUnit.Framework;
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace ProjectDawn.Collections.Tests
{
    internal class NativeStructureListTests
    {
        [Test]
        public unsafe void NativeStructureListTests_Float_Int_AsArray()
        {
            var sizes = new NativeList<int>(2, Allocator.TempJob);
            sizes.Add(sizeof(int));
            sizes.Add(sizeof(float));

            var data = new NativeStructureList(1, sizes, Allocator.TempJob);

            data.Length = 1;

            var ints = data.AsArray<int>(0);
            var floats = data.AsArray<float>(1);
            ints[0] = 1;
            floats[0] = 2.5f;

            data.Length = 2;

            ints = data.AsArray<int>(0);
            floats = data.AsArray<float>(1);
            Assert.AreEqual(1, ints[0]);
            Assert.AreEqual(2.5f, floats[0]);

            data.Dispose();

            sizes.Dispose();
        }

        [Test]
        public unsafe void NativeStructureListTests_Int_IncorrectSize()
        {
            var sizes = new NativeList<int>(1, Allocator.TempJob);
            sizes.Add(sizeof(int));

            var data = new NativeStructureList(1, sizes, Allocator.TempJob);

            data.Length = 1;

            // Size missmatch
            Assert.Throws<InvalidOperationException>(() =>
            {
                data.AsArray<short>(0);
            });

            data.Dispose();

            sizes.Dispose();
        }

        [Test]
        public unsafe void NativeStructureListTests_Int_IncorrectIndex()
        {
            var sizes = new NativeList<int>(1, Allocator.TempJob);
            sizes.Add(sizeof(int));

            var data = new NativeStructureList(1, sizes, Allocator.TempJob);

            data.Length = 1;

            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                data.AsArray<int>(1);
            });

            data.Dispose();

            sizes.Dispose();
        }

        [Test]
        public unsafe void NativeStructureListTests_Int_AsArray_Resize()
        {
            var sizes = new NativeList<int>(1, Allocator.TempJob);
            sizes.Add(sizeof(int));

            var data = new NativeStructureList(1, sizes, Allocator.TempJob);

            data.Length = 1;

            var array = data.AsArray<int>(0);

            data.Length = 2;

            Assert.Throws<ObjectDisposedException>(() =>
            {
                array[0] = 1;
            });

            data.Dispose();

            sizes.Dispose();
        }

        [Test]
        public unsafe void NativeStructureListTests_Int_Dipose()
        {
            var sizes = new NativeList<int>(1, Allocator.TempJob);
            sizes.Add(sizeof(int));

            var data = new NativeStructureList(1, sizes, Allocator.TempJob);

            data.Length = 1;

            data.Dispose(default).Complete();

            sizes.Dispose();
        }
    }
}
