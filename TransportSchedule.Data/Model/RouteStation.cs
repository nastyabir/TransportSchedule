using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportSchedule.Data
{
    class RouteStation {
        public Station Station { get; set; }
        public int TimeFromOrigin { get; set; }
        public int TimeFromDest { get; set; }
    }
}
