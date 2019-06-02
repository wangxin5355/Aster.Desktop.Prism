using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aster.Infrastructure.Events
{
    public class StatusBarMessageUpdateEvent : PubSubEvent<string>
    {
    }
}
