using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Lime.Messaging.Contents;
using Take.Blip.Client;
using blip_presentation_assistant.Models;
using blip_presentation_assistant.Receivers;
using Take.Blip.Client.Extensions.EventTracker;

namespace Takenet.Chatbot4Devs.Receivers
{
    public class MediaLinkMessageReceiver : BaseMessageReceiver, IMessageReceiver
    {
        private readonly string smallThumbsUpUrl = "fbcdn.net/v/t39.1997-6/p100x100/851587_369239346556147_162929011_n.png?_nc_ad=z-m&oh=6c4deff5f1cf8c2f7940dab6f54862a2&oe=5A2DF5B0";
        private readonly string bigThumbsUpUrl = "fbcdn.net/v/t39.1997-6/851557_369239266556155_759568595_n.png?_nc_ad=z-m&oh=f41542005a7945be79a6181951f7a37b&oe=5A2703DC";

        public MediaLinkMessageReceiver(ISender sender, IEventTrackExtension eventTrackExtension) : base(sender, eventTrackExtension)
        {
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken = default(CancellationToken))
        {
            var content = message.Content as MediaLink;
            var contentType = content.Type;
            Document mediaResult = null;

            await ReplyTypingChatState(message.From, cancellationToken);

            switch (contentType.Type)
            {
                case "audio":
                    mediaResult = new MediaLink
                    {
                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/audio-1.mp3"),
                        Type = new MediaType("audio", "mp3")
                    };
                    break;
                case "image":
                    if (content.Uri.ToString().Trim().Contains(smallThumbsUpUrl) || content.Uri.ToString().Trim().Contains(bigThumbsUpUrl))
                    {
                        mediaResult = new PlainText { Text = "👍" };
                    }
                    else
                    {
                        mediaResult = new MediaLink
                        {
                            Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/no-understand-img.png"),
                            Type = new MediaType("image", "png")
                        };
                    }
                    break;
                case "document":
                case "application":
                    mediaResult = new MediaLink
                    {
                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/no-understand-doc.png"),
                        Type = new MediaType("image", "png")
                    };
                    break;
                case "video":
                    mediaResult = new PlainText { Text = "🙈" };
                    break;

            }

            await _sender.SendMessageAsync(mediaResult, message.From, cancellationToken);
            await Task.Delay(6000);
            await _sender.SendMessageAsync(ContentsHelper.MainCommand, message.From, cancellationToken);
        }
    }
}
