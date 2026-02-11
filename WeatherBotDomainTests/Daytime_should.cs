using WeatherBotDomain.Commands;

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
    }
}
