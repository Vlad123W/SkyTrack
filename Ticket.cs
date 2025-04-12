using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyTrack
{
    internal class Ticket
    {
        public int TicketId { get; set; }
        public string TicketName { get; set; }
        public string TicketDescription { get; set; }
        public string TicketStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int UserId { get; set; }

        public Ticket(int ticketId, string ticketName, string ticketDescription, string ticketStatus, DateTime createdDate, DateTime updatedDate, int userId)
        {
            TicketId = ticketId;
            TicketName = ticketName;
            TicketDescription = ticketDescription;
            TicketStatus = ticketStatus;
            CreatedDate = createdDate;
            UpdatedDate = updatedDate;
            UserId = userId;
        }
    }
}
