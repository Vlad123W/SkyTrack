using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SkyTrack
{
    public class Refresher
    {
        private List<TicketTemplate> tickets_2;
        private StackPanel panel_2;

        public Refresher(List<TicketTemplate> tickets, StackPanel panel)
        {
            tickets_2 = tickets;
            panel_2 = panel;
        }

        public void Refresh()
        {
            panel_2.Children.Clear();

            foreach (var ticket in tickets_2)
            {
                panel_2.Children.Add(ticket);
            }
        }
    }
}
