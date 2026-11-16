namespace BotInfrastructure
{
    public class PlainReply : Reply
    {
        private static PlainReply onError = new PlainReply("🛠️ Произошла ошибка. Мы уже занимаемся ее исправлением");
        private string _text;

        public PlainReply(string text)
        {
            _text = text;
        }

        public override string BuildMessage()
        {
            return _text;
        }

        public static Reply OnError() => onError;
    }
}