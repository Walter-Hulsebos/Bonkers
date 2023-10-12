using System.Diagnostics;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using static ProjectDawn.Mathematics.math2;
using static Unity.Mathematics.math;

namespace ProjectDawn.Geometry3D
{
    /// <summary>
    /// Helper class for drawing shapes using gizmos.
    /// </summary>
    public static partial class ShapeGizmos
    {
        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(float3 from, float3 to, Color color)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawLine(from, to);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawDottedLine(float3 from, float3 to, float screenSpaceSize, Color color)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawDottedLine(from, to, screenSpaceSize);
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawDottedLine(Line line, float screenSpaceSize, Color color) => DrawDottedLine(line.From, line.To, screenSpaceSize, color);

        [Conditional("UNITY_EDITOR")]
        public static void DrawLine(Line line, Color color) => DrawLine(line.From, line.To, color);

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireSphere(float3 point, float size, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(point, size);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawSolidSphere(float3 point, float size, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(point, size);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireSphere(Sphere sphere, Color color) => DrawWireSphere(sphere.Center, sphere.Radius, color);

        [Conditional("UNITY_EDITOR")]
        public static void DrawSolidSphere(Sphere circle, Color color) => DrawSolidSphere(circle.Center, circle.Radius, color); 

        [Conditional("UNITY_EDITOR")]
        public static void DrawWireBox(Box box, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(box.Center, box.Size);
        }

        [Conditional("UNITY_EDITOR")]
        public static void DrawSolidBox(Box box, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawCube(box.Center, box.Size);
        }
    }
}
