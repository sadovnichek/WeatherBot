using WeatherBotDomain;
using WeatherBotDomain.Commands;

namespace WeatherBotDomainTests
{
    [TestFixture]
    public class TodayCommand_should
    {
        private WeatherCommand weatherCommand;
        private HttpClient client;
        private string uri = "https://api.open-meteo.com/v1/forecast";

        [SetUp]
        public void Setup()
        {
            client = new HttpClient(new HttpClientHandler() { UseProxy = false });
            weatherCommand = new TodayCommand(client, new WeatherCore(), uri);
        }

        [TearDown]
        public void TearDown()
        {
            client.Dispose();
        }

        [Test]
        public async Task Test1()
        {
            var reply = await weatherCommand.Execute([]);
            Console.WriteLine(reply);
        }

        [Test]
        public void Test2()
        {
            var timePointer = "Сегодня";
            var time = new DateTime(2026, 1, 9, 14, 27, 05);
            var weatherCodes = new int[] { 0, 0, 0, 0, 0, 0, 1, 2, 2, 2, 1, 1 };
            var temperatures = new double[] { -10, -9, -8, -8, -7, -7, -7, -8, -9, -10, -11, -12 };

            var reply = (SimpleWeatherReply)weatherCommand.GetMessage(timePointer, time, weatherCodes, temperatures);

            Console.WriteLine(reply.BuildMessage());

            Assert.That(reply.Weather, Is.EqualTo("солнечная погода"));
        }

        [Test]
        public void Test3()
        {
            var timePointer = "Сегодня";
            var time = new DateTime(2026, 1, 9, 14, 27, 05);
            var weatherCodes = new int[] { 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2 };
            var temperatures = new double[] { -10, -9, -8, -8, -7, -7, -7, -8, -9, -10, -11, -12 };

            var reply = weatherCommand.GetMessage(timePointer, time, weatherCodes, temperatures);

            Console.WriteLine(reply.BuildMessage());
        }

        [Test]
        public void Test4()
        {
            var timePointer = "Сегодня";
            var time = new DateTime(2026, 1, 9, 14, 27, 05);
            var weatherCodes = new int[] { 51, 51, 51, 51, 51, 51, 80, 80, 80, 80, 80, 80 };
            var temperatures = new double[] { -10, -9, -8, -8, -7, -7, -7, -8, -9, -10, -11, -12 };

            var reply = weatherCommand.GetMessage(timePointer, time, weatherCodes, temperatures);

            Console.WriteLine(reply.BuildMessage());
        }
    }
}