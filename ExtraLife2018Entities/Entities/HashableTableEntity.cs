using ExtraLife2018InboundETL.Interfaces;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Security.Cryptography;

namespace ExtraLife2018InboundETL.Entities
{
    public class HashableTableEntity : TableEntity, IHashable
    {
        public virtual Guid CreateGuidFromSHA256Hash()
        {
            return Guid.NewGuid();
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
