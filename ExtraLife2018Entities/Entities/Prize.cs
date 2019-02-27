using Newtonsoft.Json;
using System;

namespace ExtraLife2018InboundETL.Entities
{
    public class Prize
    {
        [JsonProperty("prizeId")]
        public int PrizeId { get; set; }
        
        [JsonProperty("dateToDisplay")]
        public DateTime DateToDisplay { get; set; }

        [JsonProperty("dateAdded")]
        public DateTime DateAdded { get; set; }

        [JsonProperty("prizeName")]
        public string PrizeName { get; set; }

        [JsonProperty("donor")]
        public string Donor { get; set; }

        [JsonProperty("tier")]
        public decimal Tier { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("restriction")]
        public string Restriction { get; set; }

        [JsonProperty("wonBy")]
        public string WonBy { get; set; }

        [JsonProperty("donatedThrough")]
        public string DonatedThrough { get; set; }
    }
}
