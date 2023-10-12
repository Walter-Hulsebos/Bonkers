using NUnit.Framework;
using Unity.Mathematics;

namespace ProjectDawn.Mathematics.Tests
{
    internal class FastMathTests
    {
        [Test]
        public unsafe void FastMathTests_RSqrt()
        {
            var rnd = new Random(1);
            for (int i = 0; i < 1000000; i++)
            {
                float value = rnd.NextFloat(1, float.MaxValue);
                Assert.AreEqual(expected: math.rsqrt(value), actual: fastmath.rsqrt(value), delta: 0.0001f);
            }
        }
        
        [Test]
        public unsafe void FastMathTests_RSqrtF64()
        {
            var rnd = new Random(1);
            for (int i = 0; i < 1000000; i++)
            {
                double value = rnd.NextDouble(1, double.MaxValue);
                Assert.AreEqual(expected: math.rsqrt(value), actual: fastmath.rsqrt(value), delta: 0.0001d);
            }
        }

        [Test]
        public unsafe void FastMathTests_Cos()
        {
            var rnd = new Random(1);
            for (int i = 0; i < 1000000; i++)
            {
                float value = rnd.NextFloat();
                Assert.AreEqual(expected: math.cos(value), actual: fastmath.cos(value), delta: 0.0001f);
            }
        }
        [Test]
        public unsafe void FastMathTests_CosF64()
        {
            var rnd = new Random(1);
            for (int i = 0; i < 1000000; i++)
            {
                double value = rnd.NextDouble();
                Assert.AreEqual(expected: math.cos(value), actual: fastmath.cos(value), delta: 0.0001d);
            }
        }

        [Test]
        public unsafe void FastMathTests_Sin()
        {
            var rnd = new Random(1);
            for (int i = 0; i < 1000000; i++)
            {
                float value = rnd.NextFloat();
                Assert.AreEqual(expected: math.sin(value), actual: fastmath.sin(value), delta: 0.0001f);
            }
        }
        [Test]
        public unsafe void FastMathTests_SinF64()
        {
            var rnd = new Random(1);
            for (int i = 0; i < 1000000; i++)
            {
                double value = rnd.NextDouble();
                Assert.AreEqual(expected: math.sin(value), actual: fastmath.sin(value), delta: 0.0001d);
            }
        }
    }
}
