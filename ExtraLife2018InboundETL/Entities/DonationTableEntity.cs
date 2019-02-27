using ExtraLife2018InboundETL.Interfaces;
using Newtonsoft.Json;
using System;
using System.Text;

namespace ExtraLife2018InboundETL.Entities
{
    public class DonationTableEntity : HashableTableEntity, IHashable
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }
        [JsonProperty("avatarImageURL")]
        public string AvatarImageURL { get; set; }
        [JsonProperty("createdDateUTC")]
        public DateTimeOffset CreatedDateUTC { get; set; }
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
        [JsonProperty("donorID")]
        public string DonorId { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("participantID")]
        public string ParticipantId { get; set; }
        [JsonProperty("participantUniqueIdentifier")]
        public string ParticipantUniqueIdentifier { get; set; }
        [JsonProperty("teamID")]
        public string TeamId { get; set; }

        public override Guid CreateGuidFromSHA256Hash()
        {
            return CreateGuidFromSHA256Hash(Encoding.UTF8.GetBytes(CreatedDateUTC + DisplayName + ParticipantId + TeamId + Amount));
        }
    }
}
