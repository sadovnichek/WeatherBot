using Newtonsoft.Json.Linq;
using WeatherBotDomain;

namespace WeatherBotDomainTests
{
    [TestFixture]
    public class Tests
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
    }
}