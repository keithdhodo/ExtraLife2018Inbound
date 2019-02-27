using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraLife2018InboundETL.Constants
{
    public class KeyVaultConstants
    {
        // KeyVault Base URL
        public static string KeyVaultBaseUrl = "https://extralife2018-test.vault.azure.net/";

        // connection strings
        public static string AzureWebjobsStorageKey = "ExtraLife2018Test-AzureWebjobsStorage";
        public static string ServiceBusConnectionString = "ExtraLife2018Test-ServiceBusConnectionString";
        public static string SqlConnectionString = "ExtraLife2018Test-SqlConnectionString";
    }
}
