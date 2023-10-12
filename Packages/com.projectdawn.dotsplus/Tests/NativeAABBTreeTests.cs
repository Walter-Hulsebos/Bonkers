using NUnit.Framework;
using Unity.Collections;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using System;
using ProjectDawn.Collections.LowLevel.Unsafe;
using ProjectDawn.Geometry2D;

namespace ProjectDawn.Collections.Tests
{
    internal class NativeAABBTreeTests
    {
        [Test]
        public void NativeAABBTreeTests_Rectangle_Dispose()
        {
            var tree = new NativeAABBTree<AABRectangle>(1, Allocator.TempJob);
            tree.Dispose(default).Complete();
        }
    }
}
