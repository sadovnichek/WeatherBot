namespace BotInfrastructure
{
    public class PlainReply : Reply
    {
        private static readonly PlainReply onError = new("🛠️ Произошла ошибка. Мы уже занимаемся ее исправлением");
        private readonly string _text;

        public PlainReply(string? text)
        {
            ArgumentNullException.ThrowIfNull(text);
            _text = text;
        }

        public override string BuildMessage()
        {
            return _text;
        }

        public static Reply OnError() => onError;
    }
}