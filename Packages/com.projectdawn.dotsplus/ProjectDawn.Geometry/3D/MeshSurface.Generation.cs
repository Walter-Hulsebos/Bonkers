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
using ProjectDawn.Mathematics;

namespace ProjectDawn.Geometry3D
{
    public static class MeshSurfaceGeneration
    {
        /// <summary>
        /// Creates Icocapsule inside mesh surface.
        /// </summary>
        public static JobHandle Icocapsule(this MeshSurface surface, Capsule capsule, int subdivisions, JobHandle dependency)
        {
            var job = new IcocapsuleJob
            {
                MeshSurface = surface,
                Capsule = capsule,
                Subdivision = subdivisions,
            };
            return job.Schedule(dependency);
        }

        /// <summary>
        /// Creates Icocapsule inside mesh surface.
        /// </summary>
        public unsafe static void Icocapsule(this MeshSurface surface, Capsule capsule, int subdivisions)
        {
            Icosphere(surface, new Sphere(capsule.Center, capsule.Radius), subdivisions);

            float3* vertices = (float3*)surface.VertexData.GetUnsafeVertexData()->GetUnsafePointer();
            float offset = capsule.Height * 0.5f;
            for (int i = 0; i < surface.VertexData.Length; ++i)
            {
                float3* vertex = vertices + i;
                if (vertex->y > capsule.Center.y)
                {
                    vertex->y += offset;
                }
                else if (vertex->y < capsule.Center.y)
                {
                    vertex->y -= offset;
                }
            }
        }

        /// <summary>
        /// Creates Box inside mesh surface.
        /// </summary>
        public static JobHandle Box(this MeshSurface surface, Box box, JobHandle dependency)
        {
            var job = new BoxJob
            {
                MeshSurface = surface,
                Box = box,
            };
            return job.Schedule(dependency);
        }

        /// <summary>
        /// Creates Box inside mesh surface.
        /// </summary>
        public unsafe static void Box(this MeshSurface surface, Box box)
        {
            CheckMask(surface.VertexData.Flags);

            // Box only needs 8 vertices
            surface.VertexData.ResizeUninitialized(8);
            float3* vertices = (float3*)surface.VertexData.GetUnsafeVertexData()->GetUnsafePointer();
            vertices[0] = new float3(-0.5f, -0.5f, -0.5f);
            vertices[1] = new float3(0.5f, -0.5f, 0.5f);
            vertices[2] = new float3(0.5f, -0.5f, -0.5f);
            vertices[3] = new float3(-0.5f, -0.5f, 0.5f);
            vertices[4] = new float3(-0.5f, 0.5f, -0.5f);
            vertices[5] = new float3(0.5f, 0.5f, 0.5f);
            vertices[6] = new float3(0.5f, 0.5f, -0.5f);
            vertices[7] = new float3(-0.5f, 0.5f, 0.5f);

            // Transform
            for (int i = 0; i < surface.VertexData.Length; ++i)
                vertices[i] = vertices[i] * box.Size + box.Center;

            // Box only needs 12 triangles
            surface.Indices.ResizeUninitialized(12);
            int3* indices = (int3*)surface.Indices.GetUnsafePtr();
            // Bottom
            indices[0] = new int3(0, 2, 1);
            indices[1] = new int3(3, 0, 1);
            // Up
            indices[2] = new int3(4, 7, 6);
            indices[3] = new int3(6, 7, 5);
            // Back
            indices[4] = new int3(0, 4, 6);
            indices[5] = new int3(0, 6, 2);
            // Forward
            indices[6] = new int3(1, 5, 7);
            indices[7] = new int3(1, 7, 3);
            // Right
            indices[8] = new int3(2, 6, 5);
            indices[9] = new int3(2, 5, 1);
            // Left
            indices[10] = new int3(3, 7, 4);
            indices[11] = new int3(3, 4, 0);
            surface.SubMeshes.ResizeUninitialized(1);
            surface.SubMeshes[0] = new SubMeshDescriptor(0, 12 * 3);
        }

        /// <summary>
        /// Creates Icosahedron inside mesh surface.
        /// </summary>
        public unsafe static void Icosahedron(this MeshSurface surface, Sphere sphere)
        {
            CheckMask(surface.VertexData.Flags);

            float phi = (1.0f + math.sqrt(5.0f)) * 0.5f; // golden ratio
            float a = 1.0f;
            float b = 1.0f / phi;

            // Add vertices
            surface.VertexData.ResizeUninitialized(12);
            float3* vertices = (float3*)surface.VertexData.GetUnsafeVertexData()->GetUnsafePointer();
            vertices[0] = new float3(0, b, -a);
            vertices[1] = new float3(b, a, 0);
            vertices[2] = new float3(-b, a, 0);
            vertices[3] = new float3(0, b, a);
            vertices[4] = new float3(0, -b, a);
            vertices[5] = new float3(-a, 0, b);
            vertices[6] = new float3(0, -b, -a);
            vertices[7] = new float3(a, 0, -b);
            vertices[8] = new float3(a, 0, b);
            vertices[9] = new float3(-a, 0, -b);
            vertices[10] = new float3(b, -a, 0);
            vertices[11] = new float3(-b, -a, 0);

            // Transform
            for (int i = 0; i < surface.VertexData.Length; ++i)
                vertices[i] = math.normalize(vertices[i]) * sphere.Radius + sphere.Center;

            // Add triangles
            surface.Indices.ResizeUninitialized(20);
            int3* indices = (int3*)surface.Indices.GetUnsafePtr();
            indices[0] = new int3(2, 1, 0);
            indices[1] = new int3(1, 2, 3);
            indices[2] = new int3(5, 4, 3);
            indices[3] = new int3(4, 8, 3);
            indices[4] = new int3(7, 6, 0);
            indices[5] = new int3(6, 9, 0);
            indices[6] = new int3(11, 10, 4);
            indices[7] = new int3(10, 11, 6);
            indices[8] = new int3(9, 5, 2);
            indices[9] = new int3(5, 9, 11);
            indices[10] = new int3(8, 7, 1);
            indices[11] = new int3(7, 8, 10);
            indices[12] = new int3(2, 5, 3);
            indices[13] = new int3(8, 1, 3);
            indices[14] = new int3(9, 2, 0);
            indices[15] = new int3(1, 7, 0);
            indices[16] = new int3(11, 9, 6);
            indices[17] = new int3(7, 10, 6);
            indices[18] = new int3(5, 11, 4);
            indices[19] = new int3(10, 8, 4);

            surface.SubMeshes.ResizeUninitialized(1);
            surface.SubMeshes[0] = new SubMeshDescriptor(0, 20 * 3);
        }

        /// <summary>
        /// Creates Icosphere inside mesh surface.
        /// </summary>
        public static JobHandle Icosphere(this MeshSurface surface, Sphere sphere, int subdivisions, JobHandle dependency)
        {
            var job = new IcosphereJob
            {
                MeshSurface = surface,
                Sphere = sphere,
                Subdivision = subdivisions,
            };
            return job.Schedule(dependency);
        }

        /// <summary>
        /// Creates Icosphere inside mesh surface.
        /// </summary>
        public unsafe static void Icosphere(this MeshSurface surface, Sphere sphere, int subdivisions = 1)
        {
            CheckMask(surface.VertexData.Flags);

            Icosahedron(surface, Sphere.Unit);

            // Subdivide
            var remmap = new NativeParallelHashMap<int2, int>(1, Allocator.Temp);
            float3* vertices = (float3*)surface.VertexData.GetUnsafeVertexData()->GetUnsafePointer();
            for (int subdivision = 1; subdivision < subdivisions; ++subdivision)
            {
                var subMesh = surface.SubMeshes[0];
                subMesh.indexCount *= 4;

                surface.Indices.Capacity = surface.Indices.Length * 4;

                int indexCount = surface.Indices.Length;
                for (int i = 0; i < indexCount; ++i)
                {
                    int3 index = surface.Indices[i];

                    float3 v0 = vertices[index.x];
                    float3 v1 = vertices[index.y];
                    float3 v2 = vertices[index.z];

                    // Edges that will be divided
                    int2 e0 = math2.sort(new int2(index.x, index.y));
                    int2 e1 = math2.sort(new int2(index.y, index.z));
                    int2 e2 = math2.sort(new int2(index.z, index.x));

                    // Vertices from division
                    int3 divisionIndex;
                    if (!remmap.TryGetValue(e0, out divisionIndex.x))
                    {
                        divisionIndex.x = surface.VertexData.Length;
                        surface.VertexData.ResizeUninitialized(surface.VertexData.Length + 1);
                        vertices = (float3*)surface.VertexData.GetUnsafeVertexData()->GetUnsafePointer();

                        float3 v = (v0 + v1) * 0.5f;
                        float l = math.length(v);
                        v = math.normalize(v);
                        vertices[divisionIndex.x] = v;

                        remmap.Add(e0, divisionIndex.x);
                    }
                    if (!remmap.TryGetValue(e1, out divisionIndex.y))
                    {
                        divisionIndex.y = surface.VertexData.Length;
                        surface.VertexData.ResizeUninitialized(surface.VertexData.Length + 1);
                        vertices = (float3*)surface.VertexData.GetUnsafeVertexData()->GetUnsafePointer();

                        float3 v = (v1 + v2) * 0.5f;
                        v = math.normalize(v);
                        vertices[divisionIndex.y] = v;

                        remmap.Add(e1, divisionIndex.y);
                    }
                    if (!remmap.TryGetValue(e2, out divisionIndex.z))
                    {
                        divisionIndex.z = surface.VertexData.Length;
                        surface.VertexData.ResizeUninitialized(surface.VertexData.Length + 1);
                        vertices = (float3*)surface.VertexData.GetUnsafeVertexData()->GetUnsafePointer();

                        float3 v = (v2 + v0) * 0.5f;
                        v = math.normalize(v);
                        vertices[divisionIndex.z] = v;

                        remmap.Add(e2, divisionIndex.z);
                    }

                    // New triangles
                    int3 t0 = new int3(index.x, divisionIndex.x, divisionIndex.z);
                    int3 t1 = new int3(divisionIndex.x, index.y, divisionIndex.y);
                    int3 t2 = new int3(divisionIndex.z, divisionIndex.y, index.z);
                    int3 t3 = new int3(divisionIndex.x, divisionIndex.y, divisionIndex.z);
                    surface.Indices.Add(t0);
                    surface.Indices.Add(t1);
                    surface.Indices.Add(t2);
                    surface.Indices[i] = t3;
                }

                surface.SubMeshes[0] = subMesh;
            }
            remmap.Dispose();

            // Transform
            for (int i = 0; i < surface.VertexData.Length; ++i)
                vertices[i] = vertices[i] * sphere.Radius + sphere.Center;
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void CheckMask(VertexAttributes mask)
        {
            if (mask != VertexAttributes.Position)
                throw new InvalidOperationException("Currently mesh surface requires to have only position attribute!");
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    unsafe struct IcocapsuleJob : IJob
    {
        public MeshSurface MeshSurface;
        public Capsule Capsule;
        public int Subdivision;

        public void Execute() => MeshSurface.Icocapsule(Capsule, Subdivision);
    }

    [BurstCompile(CompileSynchronously = true)]
    unsafe struct IcosphereJob : IJob
    {
        public MeshSurface MeshSurface;
        public Sphere Sphere;
        public int Subdivision;

        public void Execute() => MeshSurface.Icosphere(Sphere, Subdivision);
    }

    [BurstCompile(CompileSynchronously = true)]
    unsafe struct BoxJob : IJob
    {
        public MeshSurface MeshSurface;
        public Box Box;

        public void Execute() => MeshSurface.Box(Box);
    }
}
