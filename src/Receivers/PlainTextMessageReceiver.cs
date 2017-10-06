using System;
using System.Threading;
using System.Threading.Tasks;
using Lime.Protocol;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.Bucket;
using Take.Blip.Client.Extensions.ArtificialIntelligence;
using Lime.Messaging.Contents;
using blip_presentation_assistant.Models;
using Takenet.Iris.Messaging.Resources.ArtificialIntelligence;
using Take.Blip.Client.Extensions.Scheduler;
using System.Collections.Generic;
using Take.Blip.Client.Extensions.Directory;
using Take.Blip.Client.Extensions.EventTracker;
using Take.Blip.Client.Extensions.Broadcast;

namespace blip_presentation_assistant.Receivers
{
    /// <summary>
    /// Defines a class for handling messages. 
    /// This type must be registered in the application.json file in the 'messageReceivers' section.
    /// </summary>
    public class PlainTextMessageReceiver : BaseMessageReceiver, IMessageReceiver
    {
        private readonly Settings _settings;
        private readonly IBucketExtension _bucketExtension;
        private readonly IArtificialIntelligenceExtension _artificialIntelligenceExtension;
        private readonly ISchedulerExtension _schedulerExtension;
        private readonly IDirectoryExtension _directoryExtension;
        private readonly IBroadcastExtension _broadcastExtension;

        public PlainTextMessageReceiver(ISender sender,
            Settings settings,
            IBucketExtension bucketExtension,
            IArtificialIntelligenceExtension artificialIntelligenceExtension,
            ISchedulerExtension schedulerExtension,
            IEventTrackExtension eventTrackExtension,
            IDirectoryExtension directoryExtension,
            IBroadcastExtension broadcastExtension) : base(sender, eventTrackExtension)
        {
            _settings = settings;
            _bucketExtension = bucketExtension;
            _artificialIntelligenceExtension = artificialIntelligenceExtension;
            _schedulerExtension = schedulerExtension;
            _directoryExtension = directoryExtension;
            _broadcastExtension = broadcastExtension;
        }

        public async Task ReceiveAsync(Message message, CancellationToken cancellationToken)
        {
            var fromNode = message.From;
            var text = (message.Content as PlainText).Text;
            var context = await _bucketExtension.GetAsync<JsonDocument>(fromNode, cancellationToken);

            if(await ProcessHelperCommands(message)) return;

            //New user
            if (context == null)
            {
                //Save user on bot's roster
                await _directoryExtension.GetDirectoryAccountAsync(fromNode.ToIdentity(), cancellationToken);

                var helloMessage = new Message
                {
                    Content = new PlainText { Text = "Olá ${contact.name}, eu sou o Jack um assistente de apresentações. Hoje vou facilitar sua interação com a palestra do Pacheco na Conferência Bots Brasil." },
                    To = fromNode,
                    Metadata = new Dictionary<string, string>
                    {
                        { "#message.replaceVariables", "true" }
                    }
                };

                var startMessage = new PlainText { Text = "🎈" };
                await _sender.SendMessageAsync(startMessage, fromNode, cancellationToken);
                await Task.Delay(3000);
                await _sender.SendMessageAsync(helloMessage, cancellationToken);
                await Task.Delay(1000);
                await _sender.SendMessageAsync(ContentsHelper.MainCommand, message.From, cancellationToken);

                //Set an user context
                var newContext = new JsonDocument();
                await _bucketExtension.SetAsync(fromNode, newContext, cancellationToken: cancellationToken);

                //Schedule a slide message
                var slidesMessage = ContentsHelper.SlideMessage;
                slidesMessage.To = fromNode;
                await _schedulerExtension.ScheduleMessageAsync(slidesMessage, DateTimeOffset.UtcNow.AddMinutes(1));
            }
            else
            {
                var analysisResult = await _artificialIntelligenceExtension.AnalyzeAsync(new AnalysisRequest { Text = text }, cancellationToken);
                var bestIntention = analysisResult.Intentions[0];

                //AI provider cutoff
                if (bestIntention.Score < 0.5)
                {
                    bestIntention.Name = "unknow";
                }

                await ProcessState(fromNode, bestIntention.Name, null, cancellationToken);
            }
        }

        private async Task<bool> ProcessHelperCommands(Message message)
        {
            if (message.From.ToString().Equals("1639720059374062@messenger.gw.msging.net"))
            {
                switch (message.Content.ToString().ToLowerInvariant())
                {
                    case "broad":
                        await _broadcastExtension.SendMessageAsync("botsbrasilconf+senders", ContentsHelper.SurveyMenu);
                        return true;
                    default:
                        return false;
                }
            }

            return false;
        }
    }
}
