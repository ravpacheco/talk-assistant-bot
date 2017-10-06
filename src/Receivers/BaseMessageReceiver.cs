using blip_presentation_assistant.Models;
using Lime.Messaging.Contents;
using Lime.Protocol;
using System;
using System.Threading;
using System.Threading.Tasks;
using Take.Blip.Client;
using Take.Blip.Client.Extensions.Bucket;
using Take.Blip.Client.Extensions.EventTracker;

namespace blip_presentation_assistant.Receivers
{
    public class BaseMessageReceiver
    {
        protected ISender _sender;
        private IEventTrackExtension _eventTrackExtension;

        public BaseMessageReceiver(ISender sender, IEventTrackExtension eventTrackExtension)
        {
            _sender = sender;
            _eventTrackExtension = eventTrackExtension;
        }

        public async Task ReplyTypingChatState(Node to, CancellationToken cancellationToken)
        {
            var chatStateMessage = new Message
            {
                Content = new ChatState { State = ChatStateEvent.Composing },
                To = to
            };

            await _sender.SendMessageAsync(chatStateMessage, cancellationToken);
        }

        public async Task ProcessState(Node to, string triggerId, string payload, CancellationToken cancellationToken)
        {
            switch (triggerId)
            {
                case "talk":

                    var keyPoints = new PlainText[]{
                        new PlainText{ Text = "📜 Motivos para utilizar uma plataforma:\n 1. Não reinvente a roda.\n2. Saia na frente.\n3. Reduza seus custos.\n4. Prepare-se para escalar." },
                        new PlainText{ Text = "❓ Questionamento relevante da palestra:\n\nPreciso de uma plataforma para fazer meu bot ?\n\nResposta curta: Não\nResposta longa: Talvez" },
                        new PlainText{ Text = "🤖 Plataformas citadas:\n\n1. Chatfuel\n2. Api.ai\n3. Heroes\n4. Chatclub\n5. BotFramework\n6. BLiP" }
                    };

                    var randomIndex = new Random().Next(0, keyPoints.Length);

                    await _sender.SendMessageAsync(keyPoints[randomIndex], to, cancellationToken);
                    await Task.Delay(3000);
                    await _sender.SendMessageAsync(ContentsHelper.MainCommand, to, cancellationToken);
                    break;
                case "panelist":

                    var panelistImage = new MediaLink
                    {
                        Uri = new Uri("https://s3.amazonaws.com/elasticbeanstalk-us-east-1-405747350567/c4d/Rafael.png"),
                        Type = new MediaType("image", "png")
                    };

                    var panelistDescription = new PlainText { Text = "Rafael Pacheco é líder técnico, desenvolvedor e evangelizador do BLiP na Take." };

                    var panelistSocialNetworks = new PlainText { Text = "Site: https://ravpacheco.com \nEmail: ravpacheco@gmail.com \nTwitter: @ravpachecco" };

                    await _sender.SendMessageAsync(panelistImage, to, cancellationToken);
                    await Task.Delay(3000);
                    await _sender.SendMessageAsync(panelistDescription, to, cancellationToken);
                    await ReplyTypingChatState(to, cancellationToken);
                    await Task.Delay(2000);
                    await _sender.SendMessageAsync(panelistSocialNetworks, to, cancellationToken);
                    await Task.Delay(3000);
                    await _sender.SendMessageAsync(ContentsHelper.MainCommand, to, cancellationToken);
                    break;
                case "help":
                    await _sender.SendMessageAsync(ContentsHelper.MainCommand, to, cancellationToken);
                    break;
                case "survey":
                    var thanksMenu = ContentsHelper.MainCommand;
                    thanksMenu.Text = "Muito obrigado por participar.😍 \n\nLembre-se, posso te ajudar com os temas abaixo 👇";
                    await _sender.SendMessageAsync(thanksMenu, to, cancellationToken);
                    await _eventTrackExtension.AddAsync("feedback", payload);
                    break;
                //Default case (error/unknow)
                default:
                    var error = new PlainText { Text = "Sou um bot muito simples e só consigo falar (um pouquinho 😜 ) sobre o Pacheco e sua palestra!" };
                    var tip = new PlainText { Text = "Experimente enviar algo parecido com:\n\n 'me fala sobre a palestra'" };

                    var mainMenu = ContentsHelper.MainCommand;
                    mainMenu.Text = "Para facilitar, vc também pode clicar em um dos botões abaixo 👇";

                    await _sender.SendMessageAsync(error, to, cancellationToken);
                    await Task.Delay(2000);
                    await _sender.SendMessageAsync(tip, to, cancellationToken);
                    await Task.Delay(2000);
                    await _sender.SendMessageAsync(mainMenu, to, cancellationToken);
                    break;
            }
        }
    }
}
