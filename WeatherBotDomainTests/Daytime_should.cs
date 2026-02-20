using WeatherBotDomain.Commands;
using WeatherBotDomain.Reply;

namespace WeatherBotDomainTests
{
    [TestFixture]
    public class Daytime_should
    {
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
