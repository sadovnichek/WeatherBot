using System.Threading.Channels;

namespace BotInfrastructure
{
    public interface IMessageBus<T>
    {
        Task Put(T item);

        Task<T> Get();

        void Complete();

        bool IsEmpty();
    }

    public class MessageBus<T> : IMessageBus<T>
    {
        private Channel<T> channel;
        private ChannelReader<T> reader;
        private ChannelWriter<T> writer;

        public MessageBus()
        {
            channel = Channel.CreateUnbounded<T>();
            reader = channel.Reader;
            writer = channel.Writer;
        }

        public async Task<T> Get()
        {
            return await reader.ReadAsync();
        }

        public bool IsEmpty()
        {
            return reader.Count == 0;
        }

        public async Task Put(T item)
        {
            await writer.WriteAsync(item);
        }

        public void Complete()
        {
            writer.Complete();
        }
    }
}