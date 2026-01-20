using BotInfrastructure;
using WeatherBotDomain;

namespace WeatherBotDomainTests
{
    [TestFixture]
    public class EnumerableExtensions_should
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(new double[] { 1, 2, 3 }, 2)]
        [TestCase(new double[] { 1, 2, 3, 4 }, 2.5)]
        public void Median_ShouldCalculateCorrectValue_OnGivenInput(double[] values, double expected)
        {
            Assert.That(values.Median(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(new double[] {1, 1, 2, 3}, new double[] { 1 })]
        [TestCase(new double[] { 1, 1, 2, 2, 3, 3 }, new double[] { 1, 2, 3 })]
        public void Mode_ShouldCalculateCorrectValue_OnGivenInput(double[] values, double[] expected)
        {
            Assert.That(values.Mode(), Is.EquivalentTo(expected));
        }

        [Test]
        [TestCase(new int[] { 1, 1, 2, 3 }, 0, 1)]
        [TestCase(new int[] { 1, 1, 2, 3, 1 }, 0, 1)]
        [TestCase(new int[] { 1, 1, 1, 1, 1, 1, 1 }, 0, 6)]
        [TestCase(new int[] { 1 }, 0, 0)]
        public void Test1(int[] sequence, int start, int end)
        {
            var map = WeatherCore.ClassifyItemsByIndex(sequence);

            Assert.Multiple(() =>
            {
                Assert.That(map[0].Key, Is.EqualTo(1));
                Assert.That(map[0].Value, Is.EqualTo((start, end)));
            });
        }
    }
}