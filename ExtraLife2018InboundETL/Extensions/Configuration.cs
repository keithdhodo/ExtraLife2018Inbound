using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;

namespace ExtraLife2018InboundETL.Extensions
{
    public static class Configuration
    {
        public static IConfigurationRoot InitializeConfiguration(IConfigurationRoot config, ExecutionContext context)
        {
            if (config == null)
            {
                config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            }

            return config;
        }
    }
}
