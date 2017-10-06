using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Take.Blip.Client;
using Lime.Messaging.Contents;
using blip_presentation_assistant.Models;
using Take.Blip.Client.Extensions.EventTracker;

namespace blip_presentation_assistant.Receivers
{
    public class LocationMessageReceiver : BaseMessageReceiver, IMessageReceiver
    {
        public LocationMessageReceiver(
            ISender sender,
            IEventTrackExtension eventTrackExtension
            ) : base(sender, eventTrackExtension)
        {
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken = default(CancellationToken))
        {
            await ReplyTypingChatState(message.From, cancellationToken);

            var location = new Location { Latitude = 23.5661861, Longitude = -46.6447319 };
            var text = new PlainText { Text = "😍 eu estou aqui !!!" };

            await _sender.SendMessageAsync(text, message.From, cancellationToken);
            await _sender.SendMessageAsync(location, message.From,  cancellationToken);
            await Task.Delay(6000);
            await _sender.SendMessageAsync(ContentsHelper.MainCommand, message.From, cancellationToken);
        }
    }
}
