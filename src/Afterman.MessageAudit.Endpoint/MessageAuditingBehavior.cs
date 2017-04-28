namespace Afterman.MessageAudit.Endpoint
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Components;
    using NServiceBus;
    using NServiceBus.Features;
    using NServiceBus.Logging;
    using NServiceBus.Pipeline;
    using StructureMap;

    public class MessageAuditingFeature :
        Feature
    {
        public MessageAuditingFeature()
        {
            this.EnableByDefault();
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            var pipeline = context.Pipeline;
            pipeline.Register<MessageAuditingRegistration>();
        }
    }

    public class MessageAuditingRegistration
        : RegisterStep
    {
        public MessageAuditingRegistration()
            : base(
                stepId: "MessageAuditingMutator",
                behavior: typeof(MessageAuditingMutator),
                description: "Audits off any and all Incoming Messages")
        {
        }
    }

    public class MessageAuditingMutator :
        Behavior<IIncomingLogicalMessageContext>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MessageAuditingMutator));
        private readonly IContainer _container;
        public MessageAuditingMutator(IContainer container)
        {
            this._container = container;
        }

        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            
            var repository = this._container.GetInstance<IAuditMessages>();
            repository.Audit(new MessageAudit()
            {
                DateAudited = DateTime.Now,
                ConversationId = context.Headers[Headers.ConversationId],
                MessageId = context.Headers[Headers.MessageId],
                MessageType = context.Message?.Instance?.GetType()?.Name,
                MessageBody = TrySerializeMessage(context),
            });
            
            //not calling next()  
            //this will cause the message to simply go to audit.
            await Task.FromResult(true);
        }


        private string TrySerializeMessage(IIncomingLogicalMessageContext context)
        {
            var objToSerialize = context.Message?.Instance;
            if (null == objToSerialize) return null;
            var serializer = new System.Xml.Serialization.XmlSerializer(objToSerialize.GetType());
            var results = new StringBuilder();
            using (var writer = new StringWriter(results))
            {
                serializer.Serialize(writer, objToSerialize);
                writer.Close();
            }
            return results.ToString();
        }

        private string TryGetEmployeeId(IIncomingLogicalMessageContext context)
        {
            var message = context.Message?.Instance;
            var property = message?.GetType().GetProperty("EmployeeId");
            if (property == null || !property.CanRead) return null;
            return property.GetValue(message, null)?.ToString();
        }
    }
}
