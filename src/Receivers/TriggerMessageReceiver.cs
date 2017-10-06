using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Take.Blip.Client;
using blip_presentation_assistant.Models;
using Take.Blip.Client.Extensions.Bucket;
using Take.Blip.Client.Extensions.EventTracker;

namespace blip_presentation_assistant.Receivers
{
    public class TriggerMessageReceiver : BaseMessageReceiver, IMessageReceiver
    {
        public TriggerMessageReceiver(ISender sender, IEventTrackExtension eventTrackExtension) 
            : base(sender, eventTrackExtension)
        {
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var fromNode = message.From;
            var trigger = message.Content as Trigger;

            await ReplyTypingChatState(fromNode, cancellationToken);
            await ProcessState(fromNode, trigger.StepId, trigger.Payload, cancellationToken);
        }
    }
}
