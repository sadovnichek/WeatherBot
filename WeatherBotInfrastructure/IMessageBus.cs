using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BotInfrastructure
{
    public record Message(long ChatId, string Text);

    public interface IMessageBus<T>
    {
        Task Put(T item);

        Task<T> Obtain();

        void Complete();
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

        public async Task<T> Obtain()
        {
            return await reader.ReadAsync();
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
