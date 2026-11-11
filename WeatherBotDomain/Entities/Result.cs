namespace WeatherBotDomain.Entities
{
    public class Result<T>
    {
        public T Value { get; }

        public bool Success { get; } = false;

        public Result(T value)
        {
            Value = value;
            Success = true;
        }

        private Result()
        {
            
        }

        public static Result<T> Failure() => new Result<T>();
    }
}