using NUnit.Framework;
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace ProjectDawn.Geometry3D.Tests
{
    internal class VertexDataTests
    {
        struct Vertex
        {
            public float3 Position;
            public float2 TexCoord0;
        }

        [Test]
        public unsafe void VertexDataTests_Create()
        {
            var attributes = new NativeList<VertexAttributeDescriptor>(2, Allocator.TempJob);
            attributes.Add(new VertexAttributeDescriptor(VertexAttribute.Position));
            attributes.Add(new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 1));

            var data = new VertexData(1, attributes, Allocator.TempJob);

            data.Length = 1;

            var vertices = data.AsArray<Vertex>();
            vertices[0] = new Vertex { Position = 1, TexCoord0 = 0 };

            data.Length = 2;

            vertices = data.AsArray<Vertex>();
            Assert.AreEqual(1, vertices[0].Position.x);

            data.Dispose();

            attributes.Dispose();
        }

        [Test]
        public unsafe void VertexDataTests_GetVertexAttributes()
        {
            var attributes = new NativeList<VertexAttributeDescriptor>(2, Allocator.TempJob);
            attributes.Add(new VertexAttributeDescriptor(VertexAttribute.Position));
            attributes.Add(new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 1));

            var data = new VertexData(1, attributes, Allocator.TempJob);

            data.Length = 1;

            var attributes2 = data.GetVertexAttributes();

            data.Length = 2;

            for (int i = 0; i < attributes.Length; ++i)
                Assert.AreEqual(attributes[0].format, attributes2[0].format);

            data.Dispose();

            attributes.Dispose();
        }

        unsafe struct ChangeVertexDataJob : IJob
        {
            public VertexData VertexData;
            public void Execute()
            {
                var array = VertexData.AsArray<Vertex>();
                array[0] = new Vertex { Position = 1 };
            }
        }

        [Test]
        public unsafe void VertexDataTests_Jobified_Write()
        {
            var attributes = new NativeList<VertexAttributeDescriptor>(2, Allocator.TempJob);
            attributes.Add(new VertexAttributeDescriptor(VertexAttribute.Position));
            attributes.Add(new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 1));

            var data = new VertexData(1, attributes, Allocator.TempJob);

            data.Length = 1;

            var job = new ChangeVertexDataJob { VertexData = data };

            var dependency = job.Schedule();

            Assert.Throws<InvalidOperationException>(() =>
            {
                var array = data.AsArray<Vertex>();
                array[0] = new Vertex { Position = 2 };
            });

            dependency.Complete();

            Assert.AreEqual(1, data.GetVertexAt(0).x);
            Assert.AreEqual(1, data.GetVertexAt(0).y);
            Assert.AreEqual(1, data.GetVertexAt(0).z);

            data.Dispose();

            attributes.Dispose();
        }


        [Test]
        public unsafe void VertexDataTests_Dispose()
        {
            var attributes = new NativeList<VertexAttributeDescriptor>(2, Allocator.TempJob);
            attributes.Add(new VertexAttributeDescriptor(VertexAttribute.Position));
            attributes.Add(new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2, 1));

            var data = new VertexData(1, attributes, Allocator.TempJob);

            data.Length = 1;

            var job = new ChangeVertexDataJob { VertexData = data };

            var dependency = job.Schedule();

            dependency = data.Dispose(dependency);

            dependency.Complete();

            attributes.Dispose();
        }
    }
}
