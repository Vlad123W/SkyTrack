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
            bool isEmpty = Controls.Children.OfType<ComboBox>().All(c => string.IsNullOrWhiteSpace(c.Text));

            if (isEmpty) return;
            
            string date = dateCombo.Text;
            string price = priceCombo.Text;
            string seats = seatsCombo.Text;

            flights = flightContainer.Children.OfType<TicketTemplate>()
                                              .Where(x => x.Flight.DepartureTime.ToString() == date)
                                              .Where(x => x.Flight.Price.ToString() == price)
                                              .Where(x => x.Flight.AvailableSeats.ToString() == seats)
                                              .ToList();


            foreach (var item in flights)
            {
                flightContainer.Children.Remove(item);
                flightContainer.Children.Add(item);
            }

            

            flights.Clear();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            flights = flightContainer.Children.OfType<TicketTemplate>().ToList();

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
