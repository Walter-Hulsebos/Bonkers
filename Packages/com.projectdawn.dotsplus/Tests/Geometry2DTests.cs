using NUnit.Framework;
using ProjectDawn.Geometry2D;
using Unity.Mathematics;
using static ProjectDawn.Mathematics.math2;

namespace ProjectDawn.Mathematics.Tests
{
    internal class Geometry2DTests
    {
        [Test]
        public unsafe void Geometry2DTests_CircleEquals()
        {
            object circleA = new Circle(0, 1);
            object circleB = new Circle(0, 1);
            Assert.AreEqual(expected: circleA,   actual: circleB);
        }
    }
}
