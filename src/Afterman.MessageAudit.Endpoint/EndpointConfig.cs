
namespace Afterman.MessageAudit.Endpoint
{
    using System;
    using System.Configuration;
    using NServiceBus;
    using NServiceBus.Features;

    public class EndpointConfig : IConfigureThisEndpoint
    {
        public void Customize(EndpointConfiguration configuration)
        {
            configuration.DefineEndpointName("audit.log");
            configuration.DisableFeature<Audit>();
            configuration.UseTransport<MsmqTransport>();
            configuration.UsePersistence<NHibernatePersistence>();
            configuration.UseDataBus<FileShareDataBus>()
                .BasePath(ConfigurationManager.AppSettings["FileShareDataBus"]);
            configuration.PurgeOnStartup(System.Diagnostics.Debugger.IsAttached);
            configuration.RijndaelEncryptionService();
            configuration.Conventions().DefiningDataBusPropertiesAs(pi => pi.Name.EndsWith("Attachment") || pi.Name.EndsWith("CudlConnectMessage") || pi.Name.EndsWith("RawConfiguration"))
                .DefiningCommandsAs(pi => pi.Namespace != null && pi.Namespace.Contains(".Contracts.Commands"))
                .DefiningEventsAs(pi => pi.Namespace != null && pi.Namespace.Contains(".Contracts.Events"))
                .DefiningMessagesAs(pi => pi.Namespace != null && pi.Namespace.Contains(".Contracts.Messages"))
                .DefiningExpressMessagesAs(pi => pi.Name.EndsWith("Express"))
                .DefiningTimeToBeReceivedAs(t => t.Name.EndsWith("Expires") ? TimeSpan.FromSeconds(45) : TimeSpan.MaxValue);
           
        }
    }
}
