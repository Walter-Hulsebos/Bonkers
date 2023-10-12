using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using ProjectDawn.Collections;
using System.Diagnostics;
using UnityEngine.Rendering;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;

namespace ProjectDawn.Geometry3D
{
    /// <summary>
    /// Mesh data structure.
    /// Used for modifying mesh data and reading/writing back to <see cref="Mesh"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("IsCreated = {IsCreated}")]
    public struct MeshSurface : INativeDisposable
    {
        /// <summary>
        /// Vertex Data of the surface.
        /// </summary>
        public VertexData VertexData;

        /// <summary>
        /// Index list of the surface.
        /// </summary>
        public NativeList<int3> Indices;

        /// <summary>
        /// Sub meshes of the surface.
        /// </summary>
        public NativeList<SubMeshDescriptor> SubMeshes;

        /// <summary>
        /// Whether the surface is empty.
        /// </summary>
        /// <value>True if the surface is empty or the surface has not been constructed.</value>
        public bool IsEmpty => VertexData.IsEmpty || Indices.IsEmpty || SubMeshes.IsEmpty;

        /// <summary>
        /// Returns if the surface is allocated.
        /// </summary>
        public bool IsCreated => VertexData.IsCreated && Indices.IsCreated && SubMeshes.IsCreated;

        public MeshSurface(int initialCapacity, NativeArray<VertexAttributeDescriptor> attributes, Allocator allocator)
        {
            VertexData = new VertexData(initialCapacity, attributes, allocator);
            Indices = new NativeList<int3>(allocator);
            SubMeshes = new NativeList<SubMeshDescriptor>(allocator);
        }

        /// <summary>
        /// Returns minimum box that fully covers sub meshes.
        /// </summary>
        public Box BoundingBox()
        {
            CheckSubMesh();
            Box box = SubMeshes[0].bounds;
            for (int subMeshIndex = 1; subMeshIndex < SubMeshes.Length; ++subMeshIndex)
            {
                box = Box.Union(box, SubMeshes[subMeshIndex].bounds);
            }
            return box;
        }

        /// <summary>
        /// Clears the container.
        /// </summary>
        /// <remarks>MeshSurface Capacity remains unchanged.</remarks>
        public void Clear()
        {
            VertexData.Clear();
            Indices.Clear();
            SubMeshes.Clear();
        }

        /// <summary>
        /// Releases all resources (memory and safety handles).
        /// </summary>
        public void Dispose()
        {
            VertexData.Dispose();
            Indices.Dispose();
            SubMeshes.Dispose();
        }

        /// <summary>
        /// Creates and schedules a job that releases all resources (memory and safety handles) of this vertex data.
        /// </summary>
        /// <param name="inputDeps">The dependency for the new job.</param>
        /// <returns>The handle of the new job. The job depends upon `inputDeps` and releases all resources (memory and safety handles) of this vertex data.</returns>
        public JobHandle Dispose(JobHandle inputDeps)
        {
            var h0 = VertexData.Dispose(inputDeps);
            var h1 = Indices.Dispose(inputDeps);
            var h2 = SubMeshes.Dispose(inputDeps);
            return JobHandle.CombineDependencies(h0, JobHandle.CombineDependencies(h1, h2));
        }

        /// <summary>
        /// Recalculates sub meshes bounds.
        /// </summary>
        /// <param name="dependency">The dependency for the new job.</param>
        /// <returns>The handle of the new job.</returns>
        public JobHandle RecalculateBounds(JobHandle dependency)
        {
            var job = new RecalculateMeshBoundsJob
            {
                VertexData = VertexData,
                Indices = Indices,
                SubMeshes = SubMeshes,
            };

            return job.Schedule(dependency);
        }

        /// <summary>
        /// Writes <see cref="MeshSurface"/> into <see cref="Mesh.MeshData"/>.
        /// </summary>
        /// <param name="meshData">Output.</param>
        /// <param name="flags">Mesh update flags.</param>
        /// <param name="dependency">The dependency for the new job.</param>
        /// <returns>The handle of the new job.</returns>
        public JobHandle Write(Mesh.MeshData meshData, MeshUpdateFlags flags, JobHandle dependency)
        {
            var job = new WriteMeshJob
            {
                MeshData = meshData,
                VertexData = VertexData,
                Indices = Indices,
                SubMeshes = SubMeshes,
                MeshUpdateFlags = flags,
            };

            return job.Schedule(dependency);
        }

        /// <summary>
        /// Writes <see cref="Mesh.MeshData"/> into <see cref="MeshSurface"/>.
        /// </summary>
        /// <param name="meshData">Output.</param>
        /// <param name="flags">Mesh update flags.</param>
        /// <param name="dependency">The dependency for the new job.</param>
        /// <returns>The handle of the new job.</returns>
        public JobHandle Read(Mesh.MeshData meshData, JobHandle dependency)
        {
            var job = new ReadMeshJob
            {
                MeshData = meshData,
                VertexData = VertexData,
                Indices = Indices,
                SubMeshes = SubMeshes,
            };

            return job.Schedule(dependency);
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckSubMesh()
        {
            if (SubMeshes.IsEmpty)
                throw new InvalidOperationException("Can not get bounds from empty mesh surface.");
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    unsafe struct RecalculateMeshBoundsJob : IJob
    {
        [ReadOnly]
        public VertexData VertexData;
        [ReadOnly]
        public NativeList<int3> Indices;
        public NativeList<SubMeshDescriptor> SubMeshes;

        public void Execute()
        {
            var indices = Indices.AsArray().Reinterpret<int>(sizeof(int3));
            for (int subMeshIndex = 0; subMeshIndex < SubMeshes.Length; ++subMeshIndex)
            {
                ref var subMesh = ref SubMeshes.ElementAt(subMeshIndex);
                float3 min = float.MaxValue;
                float3 max = float.MinValue;
                int indexStart = subMesh.indexStart;
                int indexEnd = subMesh.indexStart + subMesh.indexCount;
                for (int i = indexStart; i < indexEnd; ++i)
                {
                    int index = indices[i];
                    float3 vertex = VertexData.GetVertexAt(index);
                    min = math.min(min, vertex);
                    max = math.max(max, vertex);
                }
                subMesh.bounds = new Bounds((max + min) * 0.5f, max - min);
            }
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    unsafe struct ReadMeshJob : IJob
    {
        [ReadOnly]
        public Mesh.MeshData MeshData;

        //[WriteOnly]
        public VertexData VertexData;
        //[WriteOnly]
        public NativeList<int3> Indices;
        //[WriteOnly]
        public NativeList<SubMeshDescriptor> SubMeshes;

        public void Execute()
        {
            // Find from submesh total number of indices
            int indexCount = 0;
            for (int i = 0; i < MeshData.subMeshCount; ++i)
            {
                var subMesh = MeshData.GetSubMesh(i);
                indexCount += subMesh.indexCount;
            }

            Indices.ResizeUninitialized(indexCount / 3);
            SubMeshes.ResizeUninitialized(MeshData.subMeshCount);

            // Update indices and submesh
            var indices = Indices.AsArray().Reinterpret<int>(sizeof(int3));
            for (int i = 0; i < MeshData.subMeshCount; ++i)
            {
                var subMesh = MeshData.GetSubMesh(i);
                SubMeshes[i] = subMesh;
                MeshData.GetIndices(indices.GetSubArray(subMesh.indexStart, subMesh.indexCount), i);
            }

            VertexData.ResizeUninitialized(MeshData.vertexCount);

            // Update vertex data
            int structureOffset = 0;
            for (int bufferIndex = 0; bufferIndex < MeshData.vertexBufferCount; ++bufferIndex)
            {
                var src = MeshData.GetVertexData<byte>(bufferIndex);
                var srcSize = src.Length / MeshData.vertexCount;

                var srcPtr = (byte*)src.GetUnsafeReadOnlyPtr();
                for (int vertexIndex = 0; vertexIndex < MeshData.vertexCount; ++vertexIndex)
                {
                    var dstPtr = (byte*)VertexData.GetUnsafeVertexData()->ElementPointerAt(vertexIndex) + structureOffset;

                    UnsafeUtility.MemCpy(dstPtr, srcPtr, srcSize);

                    srcPtr += srcSize;
                }

                structureOffset += srcSize;
            }
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    unsafe struct WriteMeshJob : IJob
    {
        [WriteOnly]
        public Mesh.MeshData MeshData;

        [ReadOnly]
        public VertexData VertexData;
        [ReadOnly]
        public NativeList<int3> Indices;
        [ReadOnly]
        public NativeList<SubMeshDescriptor> SubMeshes;

        public MeshUpdateFlags MeshUpdateFlags;

        public void Execute()
        {
            MeshData.SetIndexBufferParams(Indices.Length * 3, IndexFormat.UInt32);

            MeshData.SetVertexBufferParams(VertexData.Length, VertexData.GetVertexAttributes());

            if (VertexData.Length != 0)
            {
                int structureOffset = 0;
                for (int bufferIndex = 0; bufferIndex < MeshData.vertexBufferCount; ++bufferIndex)
                {
                    var dst = MeshData.GetVertexData<byte>(bufferIndex);
                    var dstSize = dst.Length / VertexData.Length;

                    var dstPtr = (byte*)dst.GetUnsafeReadOnlyPtr();
                    for (int vertexIndex = 0; vertexIndex < MeshData.vertexCount; ++vertexIndex)
                    {
                        var srcPtr = (byte*)VertexData.GetUnsafeVertexData()->ElementPointerAt(vertexIndex) + structureOffset;

                        UnsafeUtility.MemCpy(dstPtr, srcPtr, dstSize);

                        dstPtr += dstSize;
                    }

                    structureOffset += dstSize;
                }
            }

            var indices = MeshData.GetIndexData<uint>();
            indices.CopyFrom(Indices.AsArray().Reinterpret<uint>(sizeof(int3)));

            MeshData.subMeshCount = SubMeshes.Length;
            for (int i = 0; i < SubMeshes.Length; ++i)
                MeshData.SetSubMesh(i, SubMeshes[i], MeshUpdateFlags);
        }
    }
}
