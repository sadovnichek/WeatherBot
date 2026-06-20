namespace BotInfrastructure
{
    public class PlainReply : Reply
    {
        private string _text;

        public PlainReply(string text)
        {
            _text = text;
        }

        public override string BuildMessage()
        {
            return _text;
        }
    }
}