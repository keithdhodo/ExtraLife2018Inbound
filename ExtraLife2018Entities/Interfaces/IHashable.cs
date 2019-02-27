using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace ExtraLife2018InboundETL.Interfaces
{
    public interface IHashable : ITableEntity
    {
        Guid CreateGuidFromSHA256Hash();
        Guid CreateGuidFromSHA256Hash(byte[] input);
        byte[] ComputeSHA256Hash(byte[] input);
    }
}
