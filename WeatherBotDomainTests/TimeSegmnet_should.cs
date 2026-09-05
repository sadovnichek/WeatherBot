using FakeItEasy;
using WeatherBotDomain;
using WeatherBotDomain.Abstractions;
using WeatherBotDomain.Commands;
using WeatherBotDomain.OpenMeteo;
using WeatherBotDomain.Replies;

namespace WeatherBotDomainTests
{
    [TestFixture]
    public class TimeSegmnet_should
    {
        [Test]
        public void Test1()
        {
            var t1 = new TimeSegment(new TimeOnly(0, 0), new TimeOnly(4, 0));
            var t2 = new TimeSegment(new TimeOnly(6, 0), new TimeOnly(8, 0));
            var t3 = new TimeSegment(new TimeOnly(1, 0), new TimeOnly(2, 0));
            var t4 = new TimeSegment(new TimeOnly(3, 0), new TimeOnly(9, 0));
            var t5 = new TimeSegment(new TimeOnly(11, 0), new TimeOnly(13, 0));
            var t6 = new TimeSegment(new TimeOnly(12, 0), new TimeOnly(14, 0));

            var result = TimeSegment.Join(new[] { t1, t2, t3, t4, t5, t6 }).ToList();
        }

        [Test]
        public async Task Test2()
        {
            var domain = new WeatherCore();
            var controller = new OpenMeteoController();

            var client = A.Fake<IWeatherApiClient>();
            A.CallTo(() => client.TrySendRequestAsync(A<OpenMeteoWeatherRequest>.Ignored)).Returns(Resources.json);

            var command = new TomorrowCommand(controller, client, domain);

            var result = await command.Execute([]);
        }

        [Test]
        public async Task WeeklyCommand_ShouldAggregateDataProperly_WhenTwoDaysGiven()
        {
            var domain = new WeatherCore();
            var controller = new OpenMeteoController();

            var client = A.Fake<IWeatherApiClient>();
            A.CallTo(() => client.TrySendRequestAsync(A<OpenMeteoWeatherRequest>.Ignored)).Returns(Resources.json);

            var command = new WeeklyCommand(client, controller, domain);

            var reply = await command.Execute([]) as WeeklyReply;

            Assert.That(reply.AggregatedData.Count, Is.EqualTo(2));

            Assert.That(reply.AggregatedData[0].MedianTemperature, Is.EqualTo(23.2).Within(0.01));
            Assert.That(reply.AggregatedData[1].MedianTemperature, Is.EqualTo(19.95).Within(0.01));

            Assert.That(reply.AggregatedData[0].Weather.First(), Is.EqualTo("☁️"));
            Assert.That(reply.AggregatedData[1].Weather.First(), Is.EqualTo("☁️"));
        }
    }
}