using System;
using UnityEngine.Rendering;

namespace ProjectDawn.Geometry3D
{
    /// <summary>
    /// Vertex attributes enum flags. Same as <see cref="VertexAttribute"/> just as flags.
    /// </summary>
    [Flags]
    public enum VertexAttributes
    {
        None = 0,
        Position = 1 << 0,
        Normal = 1 << 1,
        Tangent = 1 << 2,
        Color = 1 << 3,
        TexCoord0 = 1 << 4,
        TexCoord1 = 1 << 5,
        TexCoord2 = 1 << 6,
        TexCoord3 = 1 << 7,
        TexCoord4 = 1 << 8,
        TexCoord5 = 1 << 9,
        TexCoord6 = 1 << 10,
        TexCoord7 = 1 << 11,
        BlendWeight = 1 << 12,
        BlendIndices = 1 << 13
    }

    /// <summary>
    /// Vertex attribute utility functions.
    /// </summary>
    public static class VertexAttributeUtility
    {
        /// <summary>
        /// Returns true if contains flag.
        /// </summary>
        public static bool ContainsFlag(this VertexAttributes flags, VertexAttributes flag) => (flags & flag) != 0;

        /// <summary>
        /// Converts vertex attribute to flag.
        /// </summary>
        public static VertexAttributes ToFlag(this VertexAttribute attribute) => (VertexAttributes)(1 << (int)attribute);

        /// <summary>
        /// Returns size in bytes of vertex attribute format.
        /// </summary>
        public static int SizeOf(this VertexAttributeFormat attribute)
        {
            switch (attribute)
            {
                case VertexAttributeFormat.Float32:
                case VertexAttributeFormat.SInt32:
                case VertexAttributeFormat.UInt32:
                    return 4;
                case VertexAttributeFormat.Float16:
                case VertexAttributeFormat.SInt16:
                case VertexAttributeFormat.UInt16:
                case VertexAttributeFormat.UNorm16:
                case VertexAttributeFormat.SNorm16:
                    return 2;
                case VertexAttributeFormat.SInt8:
                case VertexAttributeFormat.UInt8:
                case VertexAttributeFormat.SNorm8:
                case VertexAttributeFormat.UNorm8:
                    return 1;
                default:
                    throw new NotImplementedException("Unknown VertexAttributeFormat passed!");
            }
        }
    }
}
