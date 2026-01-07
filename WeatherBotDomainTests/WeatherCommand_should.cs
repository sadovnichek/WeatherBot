using WeatherBotDomain;
using WeatherBotDomain.Commands;

namespace WeatherBotDomainTests
{
    [TestFixture]
    public class WeatherCommand_should
    {
        private ICommand weatherCommand;
        private HttpClient client;
        private string uri = "https://api.open-meteo.com/v1/forecast";

        [SetUp]
        public void Setup()
        {
            client = new HttpClient(new HttpClientHandler() { UseProxy = false });
            weatherCommand = new WeatherCommand(client, new WeatherCore(), uri);
        }

        [TearDown]
        public void TearDown()
        {
            client.Dispose();
        }

        [Test]
        public async Task Test()
        {
            var reply = await weatherCommand.Execute([]);
        }
    }
}