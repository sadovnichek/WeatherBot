namespace BotInfrastructure
{
    public abstract class Reply : IReply
    {
        public Reply? Next { get; private set; }

        public bool HasNext => Next != null;

        public Reply Root { get; private set; }

        public abstract string BuildMessage();

        public Reply FollowWith(Reply reply)
        {
            if (Root == null)
                Root = this;

            Next = reply;
            reply.Root = Root;
            return Root;
        }
    }
}