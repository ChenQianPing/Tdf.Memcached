using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tdf.Memcached
{
    [Serializable]
    public class Message
    {
        public string MessageId { get; set; }

        public string MessageTitle { get; set; }

        public string MessageBody { get; set; }

        public string MessageRouter { get; set; }
    }
}
