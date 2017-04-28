using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Afterman.MessageAudit.Endpoint.Components
{
    //TODO: create a class that implements this and then register it with the IoC
    public interface IAuditMessages
    {
        void Audit(MessageAudit audit);
    }
}
