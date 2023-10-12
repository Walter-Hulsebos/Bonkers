using System.Diagnostics;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static ProjectDawn.Mathematics.math2;
using Plane = ProjectDawn.Geometry3D.Plane;
using Unity.Collections;

namespace ProjectDawn.Geometry3D
{
    /// <summary>
    /// Intersection point of the surface.
    /// </summary>
    public struct SurfacePointIntersection
    {
        /// <summary>
        /// The index of the triangle that was hit.
        /// </summary>
        public int TriangleIndex;

        /// <summary>
        /// The time at which ray hits the surface.
        /// </summary>
        public float Time;

        /// <summary>
        /// The distance from the ray's origin to the impact point.
        /// </summary>
        public float GetDistance(Ray ray) => distance(ray.Origin, GetPoint(ray));

        /// <summary>
        /// Returns intersection point.
        /// </summary>
        public float3 GetPoint(Ray ray) => ray.GetPoint(Time);

        /// <summary>
        /// Returns the barycentric coordinate of the triangle we hit.
        /// </summary>
        public float3 GetBarycentric<T>(Ray ray, TriangularSurface<T> surface) where T : unmanaged, ITransformFloat3
        {
            var triangle = surface.GetTriangle(TriangleIndex);
            return barycentric(triangle.VertexA, triangle.VertexB, triangle.VertexC, GetPoint(ray));
        }

        /// <summary>
        /// Returns the normal of the surface the ray hit.
        /// </summary>
        public float3 GetNormal<T>(TriangularSurface<T> surface) where T : unmanaged, ITransformFloat3
        {
            var triangle = surface.GetTriangle(TriangleIndex);
            return triangle.Normal;
        }
    }

    /// <summary>
    /// Intersection line of the surface.
    /// </summary>
    public struct SurfaceLineIntersection
    {
        /// <summary>
        /// The index of the triangle that was hit.
        /// </summary>
        public int TriangleIndexA;

        /// <summary>
        /// The index of the triangle that was hit.
        /// </summary>
        public int TriangleIndexB;

        /// <summary>
        /// Intersection line.
        /// </summary>
        public Line Line;
    }

    /// <summary>
    /// Helper class for finding intersection between 2d geometry shapes.
    /// </summary>
    public static partial class ShapeUtility
    {
        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public static bool OverlapSphereAndPoint(Sphere sphere, float3 point)
        {
            return distancesq(sphere.Center, point) < sphere.Radius * sphere.Radius;
        }

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public static bool OverlapSphereAndSphere(Sphere a, Sphere b)
        {
            return distancesq(a.Center, b.Center) < (a.Radius + b.Radius).sq();
        }

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public static bool OverlapBoxAndPoint(Box box, float3 point)
        {
            return all(box.Min < point & point < box.Max);
        }

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public static bool OverlapBoxAndBox(Box a, Box b)
        {
            return all((a.Max > b.Min) & (a.Min <=b.Max));
        }

        public static bool OverlapBoxAndBox(Box boxA, Box boxB, float4x4 boxBTransform)
        {
            // Get the position and size of box A
            float3 posA = boxA.Position;
            float3 sizeA = boxA.Size;

            // Get the position and size of box B
            float3 posB = math.mul(boxBTransform, new float4(0, 0, 0, 0)).xyz;
            float3 sizeB = boxB.Size;

            // Convert the box B transform to a matrix with only rotation and scale
            float3x3 rotationScaleMatrix = new float3x3(boxBTransform.c0.xyz, boxBTransform.c1.xyz, boxBTransform.c2.xyz);

            // Calculate the half extents of box A and box B
            float3 halfExtentsA = sizeA * 0.5f;
            float3 halfExtentsB = math.mul(rotationScaleMatrix, sizeB * 0.5f);

            // Calculate the center points of box A and box B in world space
            float3 centerA = posA + halfExtentsA;
            float3 centerB = posB + halfExtentsB;

            // Calculate the absolute difference between the center points
            float3 difference = math.abs(centerA - centerB);

            // Calculate the maximum distance between the centers that allows an overlap
            float3 maxDistance = halfExtentsA + halfExtentsB;

            // Check for overlap in each axis
            bool overlapX = difference.x < maxDistance.x;
            bool overlapY = difference.y < maxDistance.y;
            bool overlapZ = difference.z < maxDistance.z;

            // Return true if there is overlap in all three axes
            return overlapX && overlapY && overlapZ;
        }

        public unsafe static bool OverlapBoxAndTriangle(Box box, Triangle triangle)
        {
            // Compute the box's half extents.
            float3 boxHalfSize = box.Size * 0.5f;

            float3 boxCenter = box.Center;

            // First, transform the triangle into the space of the box.
            float3 v0 = triangle.VertexA - boxCenter;
            float3 v1 = triangle.VertexB - boxCenter;
            float3 v2 = triangle.VertexC - boxCenter;

            // Test triangle edges against box face normals.
            float3* triangleEdges = stackalloc float3[3]
            {
                v1 - v0,
                v2 - v1,
                v0 - v2,
            };
            float3* boxNormals = stackalloc float3[3]
            {
                new float3(1.0f, 0.0f, 0.0f),
                new float3(0.0f, 1.0f, 0.0f),
                new float3(0.0f, 0.0f, 1.0f),
            };
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    // Compute the axis to test.
                    float3 axis = math.cross(triangleEdges[i], boxNormals[j]);
                    if (!Test(axis, v0, v1, v2, boxHalfSize))
                        return false;
                }
            }

            // Test box face normals against triangle.
            for (int i = 0; i < 3; i++)
            {
                float3 axis = boxNormals[i];
                if (!Test(axis, v0, v1, v2, boxHalfSize))
                    return false;
            }

            {
                float3 axis = math.cross(v0, v1);
                if (!Test(axis, v0, v1, v2, boxHalfSize))
                    return false;
            }

            // If we got here, there is an overlap.
            return true;
        }

        static bool Test(float3 axis, float3 v0, float3 v1, float3 v2, float3 size)
        {
            float p0 = math.dot(v0, axis);
            float p1 = math.dot(v1, axis);
            float p2 = math.dot(v2, axis);

            // Compute the face normals of the AABB, because the AABB
            // is at center, and of course axis aligned, we know that 
            // it's normals are the X, Y and Z axis.
            float3 u0 = new float3(1.0f, 0.0f, 0.0f);
            float3 u1 = new float3(0.0f, 1.0f, 0.0f);
            float3 u2 = new float3(0.0f, 0.0f, 1.0f);

            float r =
                size.x * math.abs(math.dot(u0, axis)) +
                size.y * math.abs(math.dot(u1, axis)) +
                size.z * math.abs(math.dot(u2, axis));

            if (math.max(-math.max(p0, math.max(p1, p2)), math.min(p0, math.min(p1, p2))) > r)
            {
                // This means BOTH of the points of the projected triangle
                // are outside the projected half-length of the AABB
                // Therefore the axis is seperating and we can exit
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public static bool OverlapBoxAndSphere(Box box, Sphere sphere)
        {
            return distancesq(box.ClosestPoint(sphere.Center), sphere.Center) < sphere.Radius * sphere.Radius;
        }

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public static unsafe bool OverlapPlaneAndBox(Plane plane, Box box)
        {
            float3 min = box.Min;
            float3 max = box.Max;

            // Get box points
            float3* points = stackalloc float3[8];
            // Down
            points[0] = new float3(min.x, min.y, min.z);
            points[1] = new float3(min.x, min.y, max.z);
            points[2] = new float3(max.x, min.y, max.z);
            points[3] = new float3(max.x, min.y, min.z);
            // Up
            points[4] = new float3(min.x, max.y, min.z);
            points[5] = new float3(min.x, max.y, max.z);
            points[6] = new float3(max.x, max.y, max.z);
            points[7] = new float3(max.x, max.y, min.z);

            // Returns false if any two points are on opposite sides of the plane
            // This will indicate that box is intersected by plane
            float previousSign = sign(plane.SignedDistanceToPoint(points[0]));
            for (int i = 1; i < 8; ++i)
            {
                float sign = math.sign(plane.SignedDistanceToPoint(points[i]));

                // This is optimized version of:
                // previousSign == 1 && sign == -1 || previousSign == -1 && sign == 1
                if ((previousSign + sign) == 0 && previousSign != 0)
                    return true;

                previousSign = sign;
            }

            return false;
        }

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public static bool OverlapCapsuleAndPoint(Capsule a, float3 b)
        {
            float3 closestPoint = a.Line.ClosestPoint(b);
            return OverlapSphereAndPoint(new Sphere(closestPoint, a.Radius), b);
        }

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public static bool OverlapCapsuleAndLine(Capsule a, Line b)
        {
            float3 closestPointFrom = b.ClosestPoint(a.Line.From);
            if (OverlapCapsuleAndPoint(a, closestPointFrom))
                return true;
            float3 closestPointTo = b.ClosestPoint(a.Line.To);
            if (OverlapCapsuleAndPoint(a, closestPointTo))
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public static bool OverlapCapsuleAndSphere(Capsule a, Sphere b)
        {
            float3 closestPoint = a.Line.ClosestPoint(b.Center);
            return OverlapSphereAndSphere(new Sphere(closestPoint, a.Radius), b);
        }

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public static bool OverlapCapsuleAndBox(Capsule a, Box b)
        {
            float3 closestPointFrom = b.ClosestPoint(a.Line.From);
            if (OverlapBoxAndSphere(b, new Sphere(a.Line.ClosestPoint(closestPointFrom), a.Radius)))
                return true;
            float3 closestPointTo = b.ClosestPoint(a.Line.To);
            if (OverlapBoxAndSphere(b, new Sphere(a.Line.ClosestPoint(closestPointTo), a.Radius)))
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if shapes surfaces overlap.
        /// </summary>
        public static bool OverlapCapsuleAndCapsule(Capsule a, Capsule b)
        {
            float3 closestPointFrom = b.Line.ClosestPoint(a.Line.From);
            if (OverlapCapsuleAndSphere(a, new Sphere(closestPointFrom, b.Radius)))
                return true;
            float3 closestPointTo = b.Line.ClosestPoint(a.Line.To);
            if (OverlapCapsuleAndSphere(a, new Sphere(closestPointTo, b.Radius)))
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if ray intersects triangle.
        /// Based on https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm.
        /// </summary>
        /// <param name="ray">Ray.</param>
        /// <param name="triangle">Triangle.</param>
        /// <param name="t">Intersection time.</param>
        /// <returns>Returns true if ray intersects triangle.</returns>
        public static bool IntersectionRayAndTriangle(Ray ray, Triangle triangle, out float t)
        {
            float3 vertex0 = triangle.VertexA;
            float3 vertex1 = triangle.VertexB;
            float3 vertex2 = triangle.VertexC;

            float3 edge1 = vertex1 - vertex0;
            float3 edge2 = vertex2 - vertex0;

            float3 h = cross(ray.Direction, edge2);
            float a = dot(edge1, h);
            if (a > -EPSILON && a < EPSILON)
            {
                // This ray is parallel to this triangle.
                t = 0;
                return false;
            }

            float f = 1.0f / a;
            float3 s = ray.Origin - vertex0;
            float u = f * dot(s, h);
            if (u < 0.0 || u > 1.0)
            {
                t = 0;
                return false;
            }

            float3 q = cross(s, edge1);
            float v = f * dot(ray.Direction, q);
            if (v < 0.0 || u + v > 1.0)
            {
                t = 0;
                return false;
            }

            // At this stage we can compute t to find out where the intersection point is on the line.
            t = f * dot(edge2, q);
            return true;
        }

        /// <summary>
        /// Returns true if line intersects triangle.
        /// </summary>
        /// <param name="line">Line.</param>
        /// <param name="triangle">Triangle.</param>
        /// <param name="point">Intersection point.</param>
        /// <returns>Returns true if line intersects triangle.</returns>
        public static bool IntersectionLineAndTriangle(Line line, Triangle triangle, out float3 point)
        {
            var ray = line.ToRay();
            if (IntersectionRayAndTriangle(ray, triangle, out float t) && t >= 0 && t <= 1)
            {
                point = ray.GetPoint(t);
                return true;
            }
            point = 0;
            return false;
        }

        /// <summary>
        /// Returns true if triangles intersect.
        /// </summary>
        /// <param name="a">First triangle.</param>
        /// <param name="b">Second triangle.</param>
        /// <param name="line">Intersection line.</param>
        /// <returns>Returns true if triangles intersect.</returns>
        public static bool IntersectionTriangleAndTriangle(Triangle a, Triangle b, out Line line)
        {
            // Idea is very simple two triangles intersect if any of these triangles line segments intersect with triangle
            // At worst case scenario there will be 6 IntersectionLineAndTriangle and at best 2 IntersectionLineAndTriangle
            // TODO: Check maybe performance is better without branching (As CPU would not need to do branch predictions)
            a.GetLines(out Line a0, out Line a1, out Line a2);
            if (IntersectionLineAndTriangle(a0, b, out line.From))
            {
                if (IntersectionLineAndTriangle(a1, b, out line.To))
                    return true;
                if (IntersectionLineAndTriangle(a2, b, out line.To))
                    return true;

                b.GetLines(out Line b0, out Line b1, out Line b2);
                if (IntersectionLineAndTriangle(b0, a, out line.To))
                    return true;
                if (IntersectionLineAndTriangle(b1, a, out line.To))
                    return true;
                if (IntersectionLineAndTriangle(b2, a, out line.To))
                    return true;
            }
            else if (IntersectionLineAndTriangle(a1, b, out line.From))
            {
                if (IntersectionLineAndTriangle(a2, b, out line.To))
                    return true;

                b.GetLines(out Line b0, out Line b1, out Line b2);
                if (IntersectionLineAndTriangle(b0, a, out line.To))
                    return true;
                if (IntersectionLineAndTriangle(b1, a, out line.To))
                    return true;
                if (IntersectionLineAndTriangle(b2, a, out line.To))
                    return true;
            }
            else if (IntersectionLineAndTriangle(a2, b, out line.From))
            {
                b.GetLines(out Line b0, out Line b1, out Line b2);
                if (IntersectionLineAndTriangle(b0, a, out line.To))
                    return true;
                if (IntersectionLineAndTriangle(b1, a, out line.To))
                    return true;
                if (IntersectionLineAndTriangle(b2, a, out line.To))
                    return true;
            }
            else
            {
                b.GetLines(out Line b0, out Line b1, out Line b2);
                if (IntersectionLineAndTriangle(b0, a, out line.From))
                {
                    if (IntersectionLineAndTriangle(b1, a, out line.To))
                        return true;
                    if (IntersectionLineAndTriangle(b2, a, out line.To))
                        return true;
                }
                if (IntersectionLineAndTriangle(b1, a, out line.From))
                {
                    if (IntersectionLineAndTriangle(b2, a, out line.To))
                        return true;
                }
            }

            line = 0;
            return false;
        }

        /// <summary>
        /// Returns true if ray intersects surface.
        /// </summary>
        /// <param name="ray">Ray.</param>
        /// <param name="surface">Surface.</param>
        /// <param name="intersection">Intersection data.</param>
        /// <returns>Returns true if ray intersects surface.</returns>
        public static bool IntersectionRayAndTriangularSurface<T>(Ray ray, TriangularSurface<T> surface, out SurfacePointIntersection intersection)
            where T : unmanaged, ITransformFloat3
        {
            intersection = new SurfacePointIntersection
            {
                Time = float.MaxValue,
                TriangleIndex = -1,
            };

            for (int i = 0; i < surface.NumTriangles; i++)
            {
                if (IntersectionRayAndTriangle(ray, surface.GetTriangle(i), out float t) && t < intersection.Time)
                {
                    intersection.Time = t;
                    intersection.TriangleIndex = i;
                }
            }

            return intersection.TriangleIndex != -1;
        }

        /// <summary>
        /// Returns surfaces intersections.
        /// </summary>
        /// <param name="a">TriangularSurface A.</param>
        /// <param name="b">TriangularSurface B.</param>
        /// <param name="intersections">Intersection data.</param>
        public static void IntersectionTriangularSurfaceAndTriangularSurface<T>(TriangularSurface<T> a, TriangularSurface<T> b, NativeList<SurfaceLineIntersection> intersections)
            where T : unmanaged, ITransformFloat3
        {
            for (int indexA = 0; indexA < a.NumTriangles; indexA++)
            {
                var triangleA = a.GetTriangle(indexA);
                for (int indexB = 0; indexB < a.NumTriangles; indexB++)
                {
                    var triangleB = b.GetTriangle(indexB);
                    if (IntersectionTriangleAndTriangle(triangleA, triangleB, out Line line))
                    {
                        intersections.Add(new SurfaceLineIntersection
                        {
                            Line = line,
                            TriangleIndexA = indexA,
                            TriangleIndexB = indexB
                        });
                    }
                }
            }
        }
    }
}
