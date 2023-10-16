using NUnit.Framework;
using Unity.Mathematics;
using static ProjectDawn.Mathematics.math2;

namespace ProjectDawn.Mathematics.Tests
{
    internal class MathTests
    {
        [Test]
        public unsafe void MathTests_DirectionToAngle()
        {
            Assert.AreEqual(expected: math.radians(0f),   actual: math2.angle(new float2( 1,  0)));
            Assert.AreEqual(expected: math.radians(90f),  actual: math2.angle(new float2( 0,  1)));
            Assert.AreEqual(expected: math.radians(180f), actual: math2.angle(new float2(-1,  0)));
            Assert.AreEqual(expected: math.radians(-90f), actual: math2.angle(new float2( 0, -1)));
        }
        [Test]
        public unsafe void MathTests_DirectionToAngleF64()
        {
            Assert.AreEqual(expected: math.radians(0d),   actual: math2.angle(new double2( 1,  0)));
            Assert.AreEqual(expected: math.radians(90d),  actual: math2.angle(new double2( 0,  1)));
            Assert.AreEqual(expected: math.radians(180d), actual: math2.angle(new double2(-1,  0)));
            Assert.AreEqual(expected: math.radians(-90d), actual: math2.angle(new double2( 0, -1)));
        }

        [Test]
        public unsafe void MathTests_Perpendicular()
        {
            AreEqual(expected: new float2(0,  1), actual: math2.perpendicularleft( new float2(1, 0)));
            AreEqual(expected: new float2(0, -1), actual: math2.perpendicularright(new float2(1, 0)));
        }
        [Test]
        public unsafe void MathTests_PerpendicularF64()
        {
            AreEqual(expected: new double2(0,  1), actual: math2.perpendicularleft( new double2(1, 0)));
            AreEqual(expected: new double2(0, -1), actual: math2.perpendicularright(new double2(1, 0)));
        }

        [Test]
        public unsafe void MathTests_AngleToDirection()
        {
            AreEqual(expected: new float2( 1,  0), actual: math2.direction(math.radians(0f)));
            AreEqual(expected: new float2( 0,  1), actual: math2.direction(math.radians(90f)));
            AreEqual(expected: new float2(-1,  0), actual: math2.direction(math.radians(180f)));
            AreEqual(expected: new float2( 0, -1), actual: math2.direction(math.radians(270f)));
        }
        [Test]
        public unsafe void MathTests_AngleToDirectionF64()
        {
            AreEqual(expected: new double2( 1,  0), actual: math2.direction(math.radians(0d)));
            AreEqual(expected: new double2( 0,  1), actual: math2.direction(math.radians(90d)));
            AreEqual(expected: new double2(-1,  0), actual: math2.direction(math.radians(180d)));
            AreEqual(expected: new double2( 0, -1), actual: math2.direction(math.radians(270d)));
        }

        [Test]
        public unsafe void MathTests_AngleBetween()
        {
            AreEqual(expected: math.radians(45f),  actual: math2.angle(math2.direction(math.radians(0f)),   math2.direction(math.radians(45f))));
            AreEqual(expected: math.radians(90f),  actual: math2.angle(math2.direction(math.radians(90f)),  math2.direction(math.radians(0f))));
            AreEqual(expected: math.radians(170f), actual: math2.angle(math2.direction(math.radians(190f)), math2.direction(math.radians(0f))));
            AreEqual(expected: math.radians(45f),  actual: math2.angle(math2.direction(math.radians(0f)),   math2.direction(math.radians(45f))));
        }
        [Test]
        public unsafe void MathTests_AngleBetweenF64()
        {
            AreEqual(expected: math.radians(45d),  actual: math2.angle(math2.direction(math.radians(0d)),   math2.direction(math.radians(45d))));
            AreEqual(expected: math.radians(90d),  actual: math2.angle(math2.direction(math.radians(90d)),  math2.direction(math.radians(0d))));
            AreEqual(expected: math.radians(170d), actual: math2.angle(math2.direction(math.radians(190d)), math2.direction(math.radians(0d))));
            AreEqual(expected: math.radians(45d),  actual: math2.angle(math2.direction(math.radians(0d)),   math2.direction(math.radians(45d))));
        }

        [Test]
        public unsafe void MathTests_AngleToRotate()
        {
            AreEqual(expected:  math.radians(45f),  actual: math2.sangle(math2.direction(math.radians(0f)),   math2.direction( math.radians(45f))));
            AreEqual(expected: -math.radians(90f),  actual: math2.sangle(math2.direction(math.radians(90f)),  math2.direction( math.radians(0f))));
            AreEqual(expected:  math.radians(170f), actual: math2.sangle(math2.direction(math.radians(190f)), math2.direction( math.radians(0f))));
            AreEqual(expected: -math.radians(45f),  actual: math2.sangle(math2.direction(math.radians(0f)),   math2.direction(-math.radians(45f))));
        }
        [Test]
        public unsafe void MathTests_AngleToRotateF64()
        {
            AreEqual(expected:  math.radians(45d),  actual: math2.sangle(math2.direction(math.radians(0d)),   math2.direction( math.radians(45d))));
            AreEqual(expected: -math.radians(90d),  actual: math2.sangle(math2.direction(math.radians(90d)),  math2.direction( math.radians(0d))));
            AreEqual(expected:  math.radians(170d), actual: math2.sangle(math2.direction(math.radians(190d)), math2.direction( math.radians(0d))));
            AreEqual(expected: -math.radians(45d),  actual: math2.sangle(math2.direction(math.radians(0d)),   math2.direction(-math.radians(45d))));
        }

        [Test]
        public unsafe void MathTests_Rotate()
        {
            AreEqual(expected: new float2(0, 1), actual: math2.rotate(new float2(1, 0), math.radians(90f)));
        }
        [Test]
        public unsafe void MathTests_RotateF64()
        {
            AreEqual(expected: new double2(0, 1), actual: math2.rotate(new double2(1, 0), math.radians(90d)));
        }

        [Test]
        public unsafe void MathTests_InvLerp()
        {
            float lerp = math.lerp(0.5f, 1, 0.5f);
            Assert.AreEqual(expected: 0.75f, actual: lerp);

            float invLerp = math2.invlerp(0.5f, 1, lerp);
            Assert.AreEqual(expected: 0.5f, actual: invLerp);

            Assert.AreEqual(expected: 1, actual: math2.invlerp(1f, 1f, 1f));
            Assert.AreEqual(expected: 0, actual: math2.invlerp(1f, 1f, 0f));
        }
        [Test]
        public unsafe void MathTests_InvLerpF64()
        {
            double lerp = math.lerp(0.5d, 1d, 0.5d);
            Assert.AreEqual(expected: 0.75d, actual: lerp);

            double invLerp = math2.invlerp(0.5d, 1d, lerp);
            Assert.AreEqual(expected: 0.5d, actual: invLerp);

            Assert.AreEqual(expected: 1d, actual: math2.invlerp(1d, 1d, 1d));
            Assert.AreEqual(expected: 0d, actual: math2.invlerp(1d, 1d, 0d));
        }

        [Test]
        public unsafe void MathTests_Barycentric()
        {
            float2 a = math2.direction(math.radians(0f));
            float2 b = math2.direction(math.radians(135f));
            float2 c = math2.direction(math.radians(225f));
            
            AreEqual(expected: new float3(0.5f, 0.5f,   0f), actual: math2.barycentric(a, b, c, new float2((a + b) / 2f)));
            AreEqual(expected: new float3(  0f, 0.5f, 0.5f), actual: math2.barycentric(a, b, c, new float2((b + c) / 2f)));
            AreEqual(expected: new float3(0.5f,   0f, 0.5f), actual: math2.barycentric(a, b, c, new float2((a + c) / 2f)));

            AreEqual(expected: new float3(1.0f,   0f,   0f), actual: math2.barycentric(a, b, c, a));
            AreEqual(expected: new float3(  0f, 1.0f,   0f), actual: math2.barycentric(a, b, c, b));
            AreEqual(expected: new float3(  0f,   0f, 1.0f), actual: math2.barycentric(a, b, c, c));

            AreEqual(expected: 1f / 3f, actual: math2.barycentric(a, b, c, new float2((a + b + c) / 3f)));
        }
        [Test]
        public unsafe void MathTests_BarycentricF64()
        {
            double2 a = math2.direction(math.radians(0d));
            double2 b = math2.direction(math.radians(135d));
            double2 c = math2.direction(math.radians(225d));
            
            AreEqual(expected: new double3(0.5d, 0.5d,   0d), actual: math2.barycentric(a, b, c, new double2((a + b) / 2d)));
            AreEqual(expected: new double3(  0d, 0.5d, 0.5d), actual: math2.barycentric(a, b, c, new double2((b + c) / 2d)));
            AreEqual(expected: new double3(0.5d,   0d, 0.5d), actual: math2.barycentric(a, b, c, new double2((a + c) / 2d)));

            AreEqual(expected: new double3(1.0d,   0d,   0d), actual: math2.barycentric(a, b, c, a));
            AreEqual(expected: new double3(  0d, 1.0d,   0d), actual: math2.barycentric(a, b, c, b));
            AreEqual(expected: new double3(  0d,   0d, 1.0d), actual: math2.barycentric(a, b, c, c));

            AreEqual(expected: 1f / 3f, actual: math2.barycentric(a, b, c, new double2((a + b + c) / 3d)));
        }

        [Test]
        public unsafe void MathTests_Blend()
        {
            float2 a = math2.direction(math.radians(0f));
            float2 b = math2.direction(math.radians(135f));
            float2 c = math2.direction(math.radians(225f));

            AreEqual(expected: a, actual: math2.blend(a, b, c, new float3(1f, 0f, 0f)));
            AreEqual(expected: b, actual: math2.blend(a, b, c, new float3(0f, 1f, 0f)));
            AreEqual(expected: c, actual: math2.blend(a, b, c, new float3(0f, 0f, 1f)));
        }
        [Test]
        public unsafe void MathTests_BlendF64()
        {
            double2 a = math2.direction(math.radians(0d));
            double2 b = math2.direction(math.radians(135d));
            double2 c = math2.direction(math.radians(225d));

            AreEqual(expected: a, actual: math2.blend(a, b, c, new double3(1d, 0d, 0d)));
            AreEqual(expected: b, actual: math2.blend(a, b, c, new double3(0d, 1d, 0d)));
            AreEqual(expected: c, actual: math2.blend(a, b, c, new double3(0d, 0d, 1d)));
        }

        [Test]
        public void MathTests_Factorial()
        {
            Assert.AreEqual(expected: 1,   actual: math2.factorial(0));
            Assert.AreEqual(expected: 1,   actual: math2.factorial(1));
            Assert.AreEqual(expected: 2,   actual: math2.factorial(2));
            Assert.AreEqual(expected: 6,   actual: math2.factorial(3));
            Assert.AreEqual(expected: 24,  actual: math2.factorial(4));
            Assert.AreEqual(expected: 120, actual: math2.factorial(5));
        }

        [Test]
        public void MathTests_Even()
        {
            Assert.IsFalse(1f.even());
            Assert.IsTrue( 2f.even());
            Assert.IsFalse(1d.even());
            Assert.IsTrue( 2d.even());
            Assert.IsFalse(1.even());
            Assert.IsTrue( 2.even());
            
            Assert.IsTrue(math.all(new float2( x: 1f, y: 2f).even() == new bool2(false, true)));
            Assert.IsTrue(math.all(new double2(x: 1d, y: 2d).even() == new bool2(false, true)));
            Assert.IsTrue(math.all(new int2(   x: 1,  y: 2).even()  == new bool2(false, true)));
            
            Assert.IsTrue(math.all(new float3( x: 1f, y: 2f, z: 3f).even() == new bool3(false, true, false)));
            Assert.IsTrue(math.all(new double3(x: 1d, y: 2d, z: 3d).even() == new bool3(false, true, false)));
            Assert.IsTrue(math.all(new int3(   x: 1,  y: 2,  z: 3).even()  == new bool3(false, true, false)));
            
            Assert.IsTrue(math.all(new float4( x: 1f, y: 2f, z: 3f, w: 4f).even() == new bool4(false, true, false, true)));
            Assert.IsTrue(math.all(new double4(x: 1d, y: 2d, z: 3d, w: 4d).even() == new bool4(false, true, false, true)));
            Assert.IsTrue(math.all(new int4(   x: 1,  y: 2,  z: 3,  w: 4).even()  == new bool4(false, true, false, true)));
        }

        [Test]
        public void MathTests_Odd()
        {
            Assert.IsTrue( 1f.odd());
            Assert.IsFalse(2f.odd());
            Assert.IsTrue( 1d.odd());
            Assert.IsFalse(2d.odd());
            Assert.IsTrue( 1.odd());
            Assert.IsFalse(2.odd());
            
            Assert.IsTrue(math.all(new float2( x: 1f, y: 2f).odd() == new bool2(true, false)));
            Assert.IsTrue(math.all(new double2(x: 1d, y: 2d).odd() == new bool2(true, false)));
            Assert.IsTrue(math.all(new int2(   x: 1,  y: 2).odd()  == new bool2(true, false)));
            
            Assert.IsTrue(math.all(new float3( x: 1f, y: 2f, z: 3f).odd() == new bool3(true, false, true)));
            Assert.IsTrue(math.all(new double3(x: 1d, y: 2d, z: 3d).odd() == new bool3(true, false, true)));
            Assert.IsTrue(math.all(new int3(   x: 1,  y: 2,  z: 3).odd()  == new bool3(true, false, true)));
            
            Assert.IsTrue(math.all(new float4( x: 1f, y: 2f, z: 3f, w: 4f).odd() == new bool4(true, false, true, false)));
            Assert.IsTrue(math.all(new double4(x: 1d, y: 2d, z: 3d, w: 4d).odd() == new bool4(true, false, true, false)));
            Assert.IsTrue(math.all(new int4(   x: 1,  y: 2,  z: 3,  w: 4).odd()  == new bool4(true, false, true, false)));
        }

        [Test]
        public void MathTests_Sum()
        {
            Assert.AreEqual(expected: (1f + 2f), actual: math2.sum(new float2( x: 1f, y: 2f)));
            Assert.AreEqual(expected: (1d + 2d), actual: math2.sum(new double2(x: 1d, y: 2d)));
            Assert.AreEqual(expected: (1  + 2),  actual: math2.sum(new int2(   x: 1,  y: 2)));
            
            Assert.AreEqual(expected: (1f + 2f + 3f), actual: math2.sum(new float3( x: 1f, y: 2f, z: 3f)));
            Assert.AreEqual(expected: (1d + 2d + 3d), actual: math2.sum(new double3(x: 1d, y: 2d, z: 3d)));
            Assert.AreEqual(expected: (1  + 2  + 3),  actual: math2.sum(new int3(   x: 1,  y: 2,  z: 3)));
            
            Assert.AreEqual(expected: (1f + 2f + 3f + 4f), actual: math2.sum(new float4( x: 1f, y: 2f, z: 3f, w: 4f)));
            Assert.AreEqual(expected: (1d + 2d + 3d + 4d), actual: math2.sum(new double4(x: 1d, y: 2d, z: 3d, w: 4d)));
            Assert.AreEqual(expected: (1  + 2  + 3  + 4),  actual: math2.sum(new int4(   x: 1,  y: 2,  z: 3,  w: 4)));
        }

        [Test]
        public void MathTests_IsCollinear()
        {
            Assert.IsTrue(iscollinear(new float2(1, 0), new float2( 1,  0)));
            Assert.IsTrue(iscollinear(new float2(1, 1), new float2( 1,  1)));
            Assert.IsTrue(iscollinear(new float2(1, 1), new float2(-1, -1)));

            Assert.IsTrue(iscollinear(new float3(1, 0, 0), new float3( 1,  0,  0)));
            Assert.IsTrue(iscollinear(new float3(1, 1, 1), new float3( 1,  1,  1)));
            Assert.IsTrue(iscollinear(new float3(1, 1, 1), new float3(-1, -1, -1)));
        }

        [Test]
        public void MathTests_IsDelaunay()
        {
            Assert.IsTrue( isdelaunay(new float2(-1, 1), new float2(-1, -1), new float2(1, -1), new float2(1.0f, 1.0f)));
            Assert.IsTrue( isdelaunay(new float2(-1, 1), new float2(-1, -1), new float2(1, -1), new float2(0.8f, 0.8f)));
            Assert.IsFalse(isdelaunay(new float2(-1, 1), new float2(-1, -1), new float2(1, -1), new float2(2.0f, 2.0f)));
        }

        [Test]
        public void MathTests_Sort()
        {
            AreEqual(expected: new float2(1, 2), actual: sort(new float2(1, 2)));
            AreEqual(expected: new float2(1, 2), actual: sort(new float2(2, 1)));

            AreEqual(expected: new float3(1, 2, 3), actual: sort(new float3(1, 2, 3)));
            AreEqual(expected: new float3(1, 2, 3), actual: sort(new float3(2, 1, 3)));
            AreEqual(expected: new float3(1, 2, 3), actual: sort(new float3(2, 3, 1)));
            AreEqual(expected: new float3(1, 2, 3), actual: sort(new float3(3, 1, 2)));
            AreEqual(expected: new float3(1, 2, 3), actual: sort(new float3(3, 2, 1)));

            AreEqual(expected: new float4(1, 2, 3, 4), actual: sort(new float4(1, 2, 3, 4)));
            AreEqual(expected: new float4(1, 2, 3, 4), actual: sort(new float4(2, 1, 3, 4)));
            AreEqual(expected: new float4(1, 2, 3, 4), actual: sort(new float4(2, 3, 1, 4)));
            AreEqual(expected: new float4(1, 2, 3, 4), actual: sort(new float4(3, 1, 2, 4)));
            AreEqual(expected: new float4(1, 2, 3, 4), actual: sort(new float4(3, 2, 1, 4)));
            AreEqual(expected: new float4(1, 2, 3, 4), actual: sort(new float4(4, 1, 2, 3)));
            AreEqual(expected: new float4(1, 2, 3, 4), actual: sort(new float4(4, 2, 1, 3)));
            AreEqual(expected: new float4(1, 2, 3, 4), actual: sort(new float4(4, 2, 3, 1)));
            AreEqual(expected: new float4(1, 2, 3, 4), actual: sort(new float4(4, 3, 1, 2)));
            AreEqual(expected: new float4(1, 2, 3, 4), actual: sort(new float4(4, 3, 2, 1)));
        }

        [Test]
        public void MathTests_IsTriangle()
        {
            Assert.IsTrue( istriangle(1f, 1f, math.sqrt(2f))); // Triangle
            Assert.IsFalse(istriangle(1f, 1f, 2f)); // Line
            Assert.IsFalse(istriangle(1f, 1f, 0f)); // Line
            Assert.IsFalse(istriangle(0f, 0f, 0f)); // Point
        }
        [Test]
        public void MathTests_IsTriangleF64()
        {
            Assert.IsTrue( istriangle(1d, 1d, math.sqrt(2d))); // Triangle
            Assert.IsFalse(istriangle(1d, 1d, 2d)); // Line
            Assert.IsFalse(istriangle(1d, 1d, 0d)); // Line
            Assert.IsFalse(istriangle(0d, 0d, 0d)); // Point
        }
        
        //TODO: Consider increasing the delta of double tests?
        
        static void AreEqual(float expected, float actual, float delta = 0.00001f)
        {
            Assert.AreEqual(expected: expected, actual: actual, delta: delta);
        }
        static void AreEqual(float2 expected, float2 actual, float delta = 0.00001f)
        {
            Assert.AreEqual(expected: expected.x, actual: actual.x, delta: delta);
            Assert.AreEqual(expected: expected.y, actual: actual.y, delta: delta);
        }
        static void AreEqual(float3 expected, float3 actual, float delta = 0.00001f)
        {
            Assert.AreEqual(expected: expected.x, actual: actual.x, delta: delta);
            Assert.AreEqual(expected: expected.y, actual: actual.y, delta: delta);
            Assert.AreEqual(expected: expected.z, actual: actual.z, delta: delta);
        }
        static void AreEqual(float4 expected, float4 actual, float delta = 0.00001f)
        {
            Assert.AreEqual(expected: expected.x, actual: actual.x, delta: delta);
            Assert.AreEqual(expected: expected.y, actual: actual.y, delta: delta);
            Assert.AreEqual(expected: expected.z, actual: actual.z, delta: delta);
            Assert.AreEqual(expected: expected.w, actual: actual.w, delta: delta);
        }
        
        static void AreEqual(double expected, double actual, double delta = 0.0000001d)
        {
            Assert.AreEqual(expected: expected, actual: actual, delta: delta);
        }
        static void AreEqual(double2 expected, double2 actual, double delta = 0.0000001d)
        {
            Assert.AreEqual(expected: expected.x, actual: actual.x, delta: delta);
            Assert.AreEqual(expected: expected.y, actual: actual.y, delta: delta);
        }
        static void AreEqual(double3 expected, double3 actual, double delta = 0.0000001d)
        {
            Assert.AreEqual(expected: expected.x, actual: actual.x, delta: delta);
            Assert.AreEqual(expected: expected.y, actual: actual.y, delta: delta);
            Assert.AreEqual(expected: expected.z, actual: actual.z, delta: delta);
        }
        static void AreEqual(double4 expected, double4 actual, double delta = 0.0000001d)
        {
            Assert.AreEqual(expected: expected.x, actual: actual.x, delta: delta);
            Assert.AreEqual(expected: expected.y, actual: actual.y, delta: delta);
            Assert.AreEqual(expected: expected.z, actual: actual.z, delta: delta);
            Assert.AreEqual(expected: expected.w, actual: actual.w, delta: delta);
        }
    }
}
