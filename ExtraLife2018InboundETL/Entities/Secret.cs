using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtraLife2018InboundETL.Entities
{
    public class Secret
    {
        public string Value { get; set; }
        public DateTime Expiration { get; set; }
    }
}
