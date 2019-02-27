using ExtraLife2018InboundETL.Interfaces;
using Newtonsoft.Json;
using System;
using System.Text;

namespace ExtraLife2018InboundETL.Entities
{
    public class ParticipantTableEntity : HashableTableEntity, IHashable
    {
        public ParticipantTableEntity()
        {
            Timestamp = DateTime.UtcNow;
        }

        [JsonProperty("avatarImageURL")]
        public string AvatarImageURL { get; set; }
        [JsonProperty("campaignDate")]
        public DateTime? CampaignDate { get; set; }
        [JsonProperty("campaignName")]
        public string CampaignName { get; set; }
        [JsonProperty("createdDateUTC")]
        public DateTimeOffset CreatedDateUTC { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("eventID")]
        public string EventId { get; set; }
        [JsonProperty("eventName")]
        public string EventName { get; set; }
        [JsonProperty("fundraisingGoal")]
        public string FundraisingGoal { get; set; }
        [JsonProperty("isTeamCaptain")]
        public string IsTeamCaptain { get; set; }
        [JsonProperty("numDonations")]
        public string NumberOfDonations { get; set; }
        [JsonProperty("participantID")]
        public string ParticipantId { get; set; }
        [JsonProperty("sumDonations")]
        public string SumDonations { get; set; }
        [JsonProperty("teamID")]
        public string TeamId { get; set; }
        [JsonProperty("teamName")]
        public string TeamName { get; set; }

        public override Guid CreateGuidFromSHA256Hash()
        {
            return CreateGuidFromSHA256Hash(Encoding.UTF8.GetBytes(CreatedDateUTC + DisplayName + ParticipantId + TeamId));
        }
    }
}
