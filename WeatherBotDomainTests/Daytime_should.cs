using WeatherBotDomain.Commands;
using WeatherBotDomain.Reply;

namespace WeatherBotDomainTests
{
    [TestFixture]
    public class Daytime_should
    {
        [Test]
        [TestCase("2026-02-10T08:36", "2026-02-10T18:46")]
        public void Test1(string sunrise, string sunset)
        {
            var instance = new DaytimeCommand(null, null, null);
            var timeSegment = instance.GetDayTime(sunrise, sunset);

            Assert.That(timeSegment.Start, Is.EqualTo(new TimeOnly(8, 36)));
            Assert.That(timeSegment.End, Is.EqualTo(new TimeOnly(18, 46)));
        }

        [Test]
        public void Test2()
        {
            var reply = new HourlyForecastReply();
            reply.AppendData(new HourlyForecastData(new TimeOnly(10, 0), "", 10));
            reply.AppendData(new HourlyForecastData(new TimeOnly(11, 0), "", 8));
            reply.AppendData(new HourlyForecastData(new TimeOnly(12, 0), "", 7));

            Console.WriteLine(reply.BuildMessage());
        }
    }
}
