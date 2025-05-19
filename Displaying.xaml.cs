using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SkyTrack
{
    /// <summary>
    /// Логика взаимодействия для Displaying.xaml
    /// </summary>
    public partial class Displaying : UserControl
    {
        private List<TicketTemplate> flights;
        
        public Displaying()
        {
            InitializeComponent();
        }

        private void Filter_button_Click(object sender, RoutedEventArgs e)
        {
            string date = dateCombo.Text.Trim();
            string price = priceCombo.Text.Trim();
            string seats = seatsCombo.Text.Trim();

            DateTime parsedDate;
            bool hasDate = DateTime.TryParse(date, out parsedDate);

            var filtered = flightContainer.Children
                .OfType<TicketTemplate>()
                .Where(x =>
                    (date == "-" || (hasDate && x.Flight.DepartureTime.Date == parsedDate.Date)) &&
                    (price == "-" || x.Flight.Price.ToString() == price) &&
                    (seats == "-" || x.Flight.AvailableSeats.ToString() == seats))
                .ToList();

            flightContainer.Children.Clear();
            foreach (var item in filtered)
            {
                flightContainer.Children.Add(item);
            }

            flights.Clear();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            flights = flightContainer.Children.OfType<TicketTemplate>().ToList();

            dateCombo.Items.Add("-");
            priceCombo.Items.Add("-");
            seatsCombo.Items.Add("-");

            foreach (var item in flights)
            {

                if (!dateCombo.Items.Contains(item.Flight.DepartureTime.ToString()))
                {
                    dateCombo.Items.Add(item.Flight.DepartureTime.ToString("d"));
                }

                if(!priceCombo.Items.Contains(item.Flight.Price.ToString()))
                {
                    priceCombo.Items.Add(item.Flight.Price.ToString());
                }
                
                if(!seatsCombo.Items.Contains(item.Flight.AvailableSeats.ToString()))
                {
                    seatsCombo.Items.Add(item.Flight.AvailableSeats.ToString());
                }
            }

            flights.Clear();
        }
    }
}
