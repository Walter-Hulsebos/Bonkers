using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.Collections;
using ProjectDawn.Collections;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;

namespace ProjectDawn.Geometry2D
{
    /// <summary>
    /// Delaunay trinagulation composed of triangles and points.
    /// Use <see cref="VoronoiBuilder.Construct{T}(ref T)"/> to construct this delaunay triangulation.
    /// </summary>
    public unsafe struct DelaunayTriangulation : IVoronoiOutput, IDisposable
    {
        /// <summary>
        /// List of points.
        /// </summary>
        public NativeList<float2> Points;
        /// <summary>
        /// List of triangles indices.
        /// </summary>
        public NativeList<int3> Indices;

        NativeList<int2> m_Edges;
        NativeParallelHashSet<int2> m_EdgeCheck;

        public DelaunayTriangulation(int numSites, AllocatorManager.AllocatorHandle allocator)
        {
            Points = new NativeList<float2>(numSites, allocator);
            Points.ResizeUninitialized(numSites);
            Indices = new NativeList<int3>(allocator);

            m_Edges = new NativeList<int2>(numSites << 1, allocator);
            m_EdgeCheck = new NativeParallelHashSet<int2>(numSites << 1, allocator);
        }

        /// <inheritdoc />
        public void ProcessSite(double2 point, int siteIndex)
        {
            CollectionChecks.CheckIndexInRange(siteIndex, Points.Length);
            Points[siteIndex] = new float2((float)point.x, (float)point.y);
        }

        /// <inheritdoc />
        public int ProcessVertex(double2 point, int siteIndexA, int siteIndexB, int siteIndexC)
        {
            int3 indices = new int3(siteIndexA, siteIndexB, siteIndexC);

            float2 a = Points[indices.x];
            float2 b = Points[indices.y];
            float2 c = Points[indices.z];

            // Set triangle to clockwise
            if (iscclockwise(a, b, c))
                swap(ref indices.x, ref indices.z);

            Indices.Add(indices);

            // Unused
            return -1;
        }

        /// <inheritdoc />
        public void ProcessEdge(double a, double b, double c, int leftVertexIndex, int rightVertexIndex, int leftSiteIndex, int rightSiteIndex)
        {
        }

        /// <inheritdoc />
        public void Build()
        {
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Points.Dispose();
            Indices.Dispose();
            m_Edges.Dispose();
            m_EdgeCheck.Dispose();
        }
    }
}
