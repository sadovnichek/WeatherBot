namespace WeatherBotDomain
{
    public class WeatherDTO
    {
        public TimeOnly Sunrise { get; init; }

        public TimeOnly Sunset { get; init; }

        public double[] Temperatures { get; init; }

        public int[] WeatherCodes { get; init; }
    }

    public interface IWeatherApiController
    {
        Task<WeatherDTO> SendRequest();
    }

    public class OpenMeteoController : IWeatherApiController
    {
        public Task<WeatherDTO> SendRequest()
        {
            throw new NotImplementedException();
        }
    }
}