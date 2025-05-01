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
using System.Windows.Shapes;

namespace SkyTrack
{
    /// <summary>
    /// Логика взаимодействия для EditForm.xaml
    /// </summary>
    public partial class EditForm : Window
    {
        private bool _editMode = false;
        public EditForm()
        {
            InitializeComponent();
            MouseLeftButtonDown += (s, e) =>
            {
                try
                {
                    DragMove();
                }
                catch
                {
                }
            };
        }

        public EditForm(Flight flight, bool mode)
        {
            InitializeComponent();
            _editMode = mode;
            MouseLeftButtonDown += (s, e) =>
            {
                try
                {
                    DragMove();
                }
                catch
                {
                }
            };
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            bool isCorrect = true;
            foreach (var item in Controls.Children)
            {
                if (item is TextBox box && box == null)
                {
                    isCorrect = false;
                    break;
                }
            }

            if(!_editMode)
            {
                if(isCorrect)
                {
                    SqlQuery query = new("skytrack");

                    if(query.GetFlightById(Convert.ToInt32(flightNumber.Text)) == null)
                    {
                        query.AddFlight(new Flight
                        {
                            FlightId = Convert.ToInt32(flightNumber.Text),
                            Origin = origin.Text,
                            Destination = destination.Text,
                            DepartureTime = DateTime.Parse(departureTime.Text),
                            ArrivalTime = DateTime.Parse(arrivalTime.Text),
                            Price = decimal.Parse(price.Text),
                            AvailableSeats = int.Parse(availableSeats.Text)
                        });
                        
                        CustomNotifyPanel panel = new();
                        panel.Message.Content = "Рейс успішно додано!";
                    }
                    else
                    {
                        query.UpdateFlight(new Flight
                        {
                            FlightId = Convert.ToInt32(flightNumber.Text),
                            Origin = origin.Text,
                            Destination = destination.Text,
                            DepartureTime = DateTime.Parse(departureTime.Text),
                            ArrivalTime = DateTime.Parse(arrivalTime.Text),
                            Price = decimal.Parse(price.Text),
                            AvailableSeats = int.Parse(availableSeats.Text)
                        });
                        
                        CustomNotifyPanel panel = new();
                        panel.Message.Content = "Рейс успішно додано!";
                    }
                }
                else
                {
                    CustomNotifyPanel panel = new();
                    panel.Message.Content = "Заповніть всі поля!";
                }
            }
            else
            {

            }
        }
    }
}
