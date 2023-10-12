using NUnit.Framework;
using UnityEngine;
using Unity.Mathematics;
using Stopwatch = System.Diagnostics.Stopwatch;
using Random = Unity.Mathematics.Random;

namespace ProjectDawn.Mathematics.Tests
{
    internal class FastMathBenchmarkTests
    {
        [Test]
        public unsafe void FastMathBenchmarkTests_RSqrt()
        {
            float sum;
            Random rnd;
            var stopWatch = new Stopwatch();

            sum = 0;
            rnd = new Random(1);
            stopWatch.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                sum += math.rsqrt(rnd.NextFloat(0.001f, float.MaxValue));
            }
            stopWatch.Stop();
            float invSqrtTime = stopWatch.ElapsedMilliseconds;
            float invSqrtSum  = sum;

            sum = 0;
            rnd = new Random(1);
            stopWatch.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                sum += fastmath.rsqrt(rnd.NextFloat(0.001f, float.MaxValue));
            }
            stopWatch.Stop();
            float fastInvSqrtTime = stopWatch.ElapsedMilliseconds;
            float fastInvSqrtSum  = sum;

            Debug.Log($"Time: InvSqrt {invSqrtTime} FastInvSqrt {fastInvSqrtTime}");
            Debug.Log($"Sum:  InvSqrt {invSqrtSum} FastInvSqrt {fastInvSqrtSum}");

            Assert.Greater(invSqrtTime, fastInvSqrtTime);
        }
        
        [Test]
        public unsafe void FastMathBenchmarkTests_RSqrtF64()
        {
            double sum;
            Random rnd;
            var stopWatch = new Stopwatch();

            sum = 0;
            rnd = new Random(1);
            stopWatch.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                sum += math.rsqrt(rnd.NextDouble(0.001d, double.MaxValue));
            }
            stopWatch.Stop();
            float invSqrtTime = stopWatch.ElapsedMilliseconds;
            double invSqrtSum = sum;

            sum = 0;
            rnd = new Random(1);
            stopWatch.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                sum += fastmath.rsqrt(rnd.NextDouble(0.001d, double.MaxValue));
            }
            stopWatch.Stop();
            float fastInvSqrtTime = stopWatch.ElapsedMilliseconds;
            double fastInvSqrtSum = sum;

            Debug.Log($"Time: InvSqrt {invSqrtTime} FastInvSqrt {fastInvSqrtTime}");
            Debug.Log($"Sum:  InvSqrt {invSqrtSum} FastInvSqrt {fastInvSqrtSum}");

            Assert.Greater(invSqrtTime, fastInvSqrtTime);
        }

        [Test]
        public unsafe void FastMathBenchmarkTests_Cos()
        {
            float sum;
            Random rnd;
            var stopWatch = new Stopwatch();

            sum = 0;
            rnd = new Random(1);
            stopWatch.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                sum += math.cos(rnd.NextFloat());
            }
            stopWatch.Stop();
            float defaultTime = stopWatch.ElapsedMilliseconds;
            float defaultSum = sum;

            sum = 0;
            rnd = new Random(1);
            stopWatch.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                sum += fastmath.cos(rnd.NextFloat());
            }
            stopWatch.Stop();
            float fastTime = stopWatch.ElapsedMilliseconds;
            float fastSum  = sum;

            Debug.Log($"Time: Cos {defaultTime} FastCos {fastTime}");
            Debug.Log($"Sum:  Cos {defaultSum} FastCos {fastSum}");

            Assert.Greater(defaultTime, fastTime);
        }
        
        [Test]
        public unsafe void FastMathBenchmarkTests_CosF64()
        {
            double sum;
            Random rnd;
            var stopWatch = new Stopwatch();

            sum = 0;
            rnd = new Random(1);
            stopWatch.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                sum += math.cos(rnd.NextDouble());
            }
            stopWatch.Stop();
            float defaultTime = stopWatch.ElapsedMilliseconds;
            double defaultSum = sum;

            sum = 0;
            rnd = new Random(1);
            stopWatch.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                sum += fastmath.cos(rnd.NextDouble());
            }
            stopWatch.Stop();
            float fastTime = stopWatch.ElapsedMilliseconds;
            double fastSum = sum;

            Debug.Log($"Time: Cos {defaultTime} FastCos {fastTime}");
            Debug.Log($"Sum:  Cos {defaultSum} FastCos {fastSum}");

            Assert.Greater(defaultTime, fastTime);
        }

        [Test]
        public unsafe void FastMathBenchmarkTests_Sin()
        {
            float sum;
            Random rnd;
            var stopWatch = new Stopwatch();

            sum = 0;
            rnd = new Random(1);
            stopWatch.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                sum += math.sin(rnd.NextFloat());
            }
            stopWatch.Stop();
            float defaultTime = stopWatch.ElapsedMilliseconds;
            float defaultSum = sum;

            sum = 0;
            rnd = new Random(1);
            stopWatch.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                sum += fastmath.sin(rnd.NextFloat());
            }
            stopWatch.Stop();
            float fastTime = stopWatch.ElapsedMilliseconds;
            float fastSum  = sum;

            Debug.Log($"Time: Sin {defaultTime} FastSin {fastTime}");
            Debug.Log($"Sum:  Sin {defaultSum} FastSin {fastSum}");

            Assert.Greater(defaultTime, fastTime);
        }
        [Test]
        public unsafe void FastMathBenchmarkTests_SinF64()
        {
            double sum;
            Random rnd;
            var stopWatch = new Stopwatch();

            sum = 0;
            rnd = new Random(1);
            stopWatch.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                sum += math.sin(rnd.NextDouble());
            }
            stopWatch.Stop();
            float defaultTime = stopWatch.ElapsedMilliseconds;
            double defaultSum = sum;

            sum = 0;
            rnd = new Random(1);
            stopWatch.Restart();
            for (int i = 0; i < 1_000_000; i++)
            {
                sum += fastmath.sin(rnd.NextDouble());
            }
            stopWatch.Stop();
            float fastTime = stopWatch.ElapsedMilliseconds;
            double fastSum = sum;

            Debug.Log($"Time: Sin {defaultTime} FastSin {fastTime}");
            Debug.Log($"Sum:  Sin {defaultSum} FastSin {fastSum}");

            Assert.Greater(defaultTime, fastTime);
        }
    }
}
