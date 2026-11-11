using BotInfrastructure;

namespace WeatherBotDomain.Abstractions
{
    public interface IPrecipitationService
    {
        Reply GetPrecipitationForecast(int[] weatherCodes);
    }
}