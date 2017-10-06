using Lime.Messaging.Contents;
using Lime.Protocol;
using System;

namespace blip_presentation_assistant.Models
{
    public static class ContentsHelper
    {
        public static Select MainCommand => new Select
        {
            Text = "Posso te ajudar com os seguintes conteúdos 👇 Clica aí 😉",
            Scope = SelectScope.Immediate,
            Options = new SelectOption[]
            {
                new SelectOption
                {
                    Text = "💡 Insight da palestra",
                    Value = new Trigger { StepId = "talk" }
            },
                new SelectOption
                {
                    Text = "🗣️ Sobre o palestrante",
                    Value = new Trigger { StepId = "panelist" }
                }
            }
        };

        public static Select SurveyMenu => new Select
        {
            Text = "Qual nota vc dá para a palestra de 1 a 5 (onde 1 é ruim e 5 é ótimo) 🤔. Vote clicando em um dos botões abaixo: ⬇",
            Scope = SelectScope.Immediate,
            Options = new SelectOption[]
            {
                new SelectOption
                {
                    Text = "1",
                    Value = new Trigger { StepId = "survey", Payload = "1" }
                },
                new SelectOption
                {
                    Text = "2",
                    Value = new Trigger { StepId = "survey", Payload = "2" }
                },
                new SelectOption
                {
                    Text = "3",
                    Value = new Trigger { StepId = "survey", Payload = "3" }
                },
                new SelectOption
                {
                    Text = "4",
                    Value = new Trigger { StepId = "survey", Payload = "4" }
                },
                new SelectOption
                {
                    Text = "5",
                    Value = new Trigger { StepId = "survey", Payload = "5" }
                },
            }
        };

        public static Message SlideMessage => new Message
        {
            Id = EnvelopeId.NewId(),
            Content = new DocumentSelect
            {
                Header = new DocumentContainer { Value = new PlainText { Text = "Geralmente, me pedem os slides das palestras que facilito. Se quiser ver os slides da palestra do Pacheco, clique no botão abaixo. ⬇" } },
                Options = new DocumentSelectOption[]
                {
                    new DocumentSelectOption
                    {
                        Label = new DocumentContainer{
                            Value = new WebLink
                            {
                                Title = "ver slides",
                                Uri = new Uri(@"https://drive.google.com/open?id=0B-Wb9w_kpaPCWWtZeDd6d1hmVm8"),
                                Target = WebLinkTarget.Blank
                            }
                        }
                    }
                }
            }
        };

    }
}
