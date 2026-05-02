using BotInfrastructure;
using WeatherBotDomain;
using WeatherBotDomain.Replies;

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
        [TestCase(new double[] { 1, 1, 2, 3 }, new double[] { 1 })]
        [TestCase(new double[] { 1, 1, 2, 2, 3, 3 }, new double[] { 1, 2, 3 })]
        public void Mode_ShouldCalculateCorrectValue_WithZeroDiscrepancy(double[] values, double[] expected)
        {
            Assert.That(values.Mode(), Is.EquivalentTo(expected));
        }

        [Test]
        [TestCase(new double[] { 1, 1, 2, 2, 2, 3 }, new double[] { 1, 2 })]
        [TestCase(new double[] { 1, 2, 2, 2, 2, 3 }, new double[] { 2 })]
        public void Mode_ShouldCalculateCorrectValue_WhenDiscrepancyGiven(double[] values, double[] expected)
        {
            Assert.That(values.Mode(1), Is.EquivalentTo(expected));
        }

        [Test]
        [TestCase(new int[] { 1, 1, 2, 3 }, 0, 1, 1)]
        [TestCase(new int[] { 1, 1, 2, 3, 4 }, 0, 1, 1)]
        [TestCase(new int[] { 1, 1, 1, 1, 1, 1, 1 }, 0, 6, 1)]
        [TestCase(new int[] { 1 }, 0, 0, 1)]
        public void ClassifyItemsByIndex_ShouldBeCorrect_WhenSingleClass(int[] sequence, int start, int end, int unit)
        {
            var @class = WeatherCore.ClassifyItemsByIndex(sequence)
                .Where(kv => kv.Key == unit)
                .Single();

            Assert.Multiple(() =>
            {
                Assert.That(@class.Key, Is.EqualTo(unit));
                Assert.That(@class.Value, Is.EqualTo((start, end)));
            });
        }

        [Test]
        [TestCase(new int[] {71, 3, 3, 3, 3, 3, 3, 3, 2, 2, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 71})]
        public void Test2(int[] sequence)
        {
            var weatherCore = new WeatherCore();
            var reply = (PrecipitationReply)weatherCore.GetPrecipitationForecast(sequence);

            var result = reply.WeatherSegments.ToList();
            var timeSegments = result[0].TimeSegments;
            var description = result[0].Description;

            Assert.That(timeSegments, Has.Length.EqualTo(2));

            Assert.That(timeSegments[0].Start, Is.EqualTo(new TimeOnly(0, 0)));
            Assert.That(timeSegments[0].End, Is.EqualTo(new TimeOnly(1, 0)));

            Assert.That(timeSegments[1].Start, Is.EqualTo(new TimeOnly(23, 0)));
            Assert.That(timeSegments[1].End, Is.EqualTo(new TimeOnly(0, 0)));

            Assert.That(description, Is.EqualTo(weatherCore.GetDescription(71)));
        }

        [Test]
        public void Test3()
        {
            var timeSegment = new TimeSegment(new TimeOnly(10, 24), new TimeOnly(15, 43));
            var strRepresenation = timeSegment.GetStringRepresentation();

            Assert.That(strRepresenation, Is.EqualTo("с 10:24 до 15:43"));
        }
    }
}