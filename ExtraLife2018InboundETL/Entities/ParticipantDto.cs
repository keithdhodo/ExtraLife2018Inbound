using EL.DonorDrive.Entities;
using ExtraLife2018InboundETL.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace ExtraLife2018InboundETL.Entities
{
    public class ParticipantDto : Participant
    {
        public IDictionary<Guid, Donor> Donations { get; set; }

        public ParticipantDto()
        {
            Donations = new ConcurrentDictionary<Guid, Donor>();
        }

        public Guid CreateGuidFromSHA256Hash()
        {
            return CreateGuidFromSHA256Hash(Encoding.UTF8.GetBytes(CreatedDateUTC.ToString() + ParticipantId.ToString() + TeamId.ToString()));
        }

        public Guid CreateGuidFromSHA256Hash(Donor donor)
        {
            return CreateGuidFromSHA256Hash(Encoding.UTF8.GetBytes(donor.CreatedDateUTC.ToString() + donor.ParticipantId.ToString() + donor.TeamId.ToString() + donor.Amount.ToString()));
        }

        public Guid CreateGuidFromSHA256Hash(byte[] input)
        {
            byte[] hash = new byte[16];
            Array.Copy(ComputeSHA256Hash(input), hash, 16);
            return new Guid(hash);
        }

        public byte[] ComputeSHA256Hash(byte[] input)
        {
            // Create a SHA256   
            using (var sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(input);
                return bytes;
            }
        }
    }
}
