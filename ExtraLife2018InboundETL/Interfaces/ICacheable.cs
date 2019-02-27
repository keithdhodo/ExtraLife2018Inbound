using System;

namespace ExtraLife2018InboundETL.Interfaces
{
    public interface ICacheable
    {
        DateTime GetDateAddedToCache();
        void SetDateAddedToCache();
    }
}
