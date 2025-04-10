using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTrack
{
    internal class Flight
    {
        public int FlightId { get; internal set; }
        public string? Origin { get; internal set; }
        public string? Destination { get; internal set; }
        public DateTime DepartureTime { get; internal set; }
        public DateTime ArrivalTime { get; internal set; }
        public decimal Price { get; internal set; }
    }
}
