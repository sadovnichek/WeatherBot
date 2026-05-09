using WeatherBotDomain;
using WeatherBotDomain.Replies;

namespace WeatherBotDomainTests
{
    [TestFixture]
    public class TodayCommand_should
    {
        private WeatherCore weatherCore;

        [SetUp]
        public void Setup()
        {
            weatherCore = new();
        }

        [Test]
        public void GetReply_ShouldReturnCorrectReply_OnSimpleReply()
        {
            var timePointer = "Сегодня";
            var time = new DateTime(2026, 1, 9, 14, 27, 05);
            var weatherCodes = new int[] { 0, 0, 0, 0, 0, 0, 1, 2, 2, 2, 1, 1 };
            var temperatures = new double[] { -10, -9, -8, -8, -7, -7, -7, -8, -9, -10, -11, -12 };

            var reply = (SimpleWeatherReply)weatherCore.GetReply(timePointer, time, weatherCodes, temperatures);

            Assert.Multiple(() =>
            {
                Assert.That(reply.Weather, Is.EqualTo("солнечная"));
                Assert.That(reply.MaxTemperature, Is.EqualTo(-7));
                Assert.That(reply.MinTemperature, Is.EqualTo(-12));
                Assert.That(reply.IsWordingNeeded, Is.True);
                Assert.That(reply.MedianTemperature, Is.EqualTo(-8.5));
                Assert.That(reply.Emoji, Is.EqualTo(weatherCore.GetEmoji(WeatherType.Sunny)));
            });
        }

        [Test]
        public void GetReply_ShouldReturnProperTimeSegment_WhenPrecipitationsAllDay()
        {
            var weatherCodes = Enumerable.Repeat(51, 24).ToArray();

            var segment = weatherCore.GetWeatherSegments(weatherCodes)
                .Single()
                .TimeSegments
                .Single();

            Assert.That(segment.GetStringRepresentation(), Is.EqualTo("весь день"));
        }
    }
}