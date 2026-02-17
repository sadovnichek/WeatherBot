namespace BotInfrastructure
{
    public class PlainReply : IReply
    {
        private string _text;

        public PlainReply(string text)
        {
            _text = text;
        }

        public string BuildMessage()
        {
            return _text;
        }
    }
}
