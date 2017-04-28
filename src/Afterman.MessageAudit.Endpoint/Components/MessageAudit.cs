using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afterman.MessageAudit.Endpoint.Components
{
    public class MessageAudit
    {
        public virtual DateTime DateAudited { get; set; }

        public virtual string MessageId { get; set; }

        public virtual string ConversationId { get; set; }

        public virtual string MessageType { get; set; }

        public virtual string MessageBody { get; set; }
    }
}
