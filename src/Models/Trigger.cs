using Lime.Protocol;
using System.Runtime.Serialization;

namespace blip_presentation_assistant.Models
{
    [DataContract]
    public class Trigger : Document
    {
        public const string MIME_TYPE = "application/vnd.custom.trigger+json";

        public static readonly MediaType MediaType = MediaType.Parse(MIME_TYPE);

        public Trigger() : base(MediaType) {}

        [DataMember]
        public string StepId { get; set; }

        [DataMember]
        public string Payload { get; set; }
    }
}
