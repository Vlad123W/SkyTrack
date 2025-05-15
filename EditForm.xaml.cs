using Google.Protobuf.WellKnownTypes;
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
        public bool added = false;

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

            flightNumber.Text = flight.FlightId.ToString();
            origin.Text = flight.Origin;
            destination.Text = flight.Destination;
            departureTime.Text = flight.DepartureTime.ToString("yyyy-MM-dd HH:mm");
            arrivalTime.Text = flight.ArrivalTime.ToString("yyyy-MM-dd HH:mm");
            price.Text = flight.Price.ToString();
            availableSeats.Text = flight.AvailableSeats.ToString();

            if (mode)
            {
                Confirm.Content = "Змінити";
            }
            else
            {
                Confirm.Content = "Додати";
            }
            
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
            bool isNotCorrect = Controls.Children.OfType<TextBox>().Any(x => string.IsNullOrWhiteSpace(x.Text));

            if (isNotCorrect)
            {
                ShowError("Заповніть всі поля!");
                return;
            }

            if (!int.TryParse(flightNumber.Text, out int flightId) || flightId <= 0)
            {
                ShowError("Неправильний формат номера рейсу!");
                return;
            }

            if (string.IsNullOrWhiteSpace(origin.Text) || string.IsNullOrWhiteSpace(destination.Text))
            {
                ShowError("Введіть місце вильоту і призначення!");
                return;
            }

            if (!DateTime.TryParse(departureTime.Text, out DateTime depTime) ||
                !DateTime.TryParse(arrivalTime.Text, out DateTime arrTime))
            {
                ShowError("Невірний формат дати вильоту або прибуття!");
                return;
            }

            if (depTime >= arrTime)
            {
                ShowError("Час вильоту повинен бути раніше часу прибуття!");
                return;
            }

            if (!decimal.TryParse(price.Text, out decimal flightPrice) || flightPrice <= 0)
            {
                ShowError("Ціна повинна бути додатним числом!");
                return;
            }

            if (!int.TryParse(availableSeats.Text, out int seats) || seats <= 0)
            {
                ShowError("Кількість місць повинна бути додатнім числом!");
                return;
            }

            Flight flightToAdd = new()
            {
                FlightId = Convert.ToInt32(flightNumber.Text),
                Origin = origin.Text,
                Destination = destination.Text,
                DepartureTime = DateTime.Parse(departureTime.Text),
                ArrivalTime = DateTime.Parse(arrivalTime.Text),
                Price = decimal.Parse(price.Text),
                AvailableSeats = int.Parse(availableSeats.Text)
            };

            SqlQuery query = new("skytrack");
           
            if (!_editMode)
            {

                if (query.GetFlightById(flightId) == null)
                {
                    query.AddFlight(flightToAdd);
                    ShowSuccess("Рейс успішно додано!", true);
                }
                else
                {
                    query.UpdateFlight(flightToAdd);
                    ShowSuccess("Рейс успішно змінено!", true);
                }
            }
            else
            {
                if (query.UpdateFlight(flightToAdd))
                {
                    ShowSuccess("Рейс успішно змінено!", true);
                    _editMode = false;
                }
            }
        }

        private void ShowError(string message)
        {
            CustomNotifyPanel panel = new();
            panel.Message.Text = message;
            panel.ConfirmBtn.Click += (s, e) => panel.Close();
            panel.Show();
        }

        private void ShowSuccess(string message, bool closeAfter)
        {
            CustomNotifyPanel panel = new();
            panel.Message.Text = message;
            panel.ConfirmBtn.Click += (s, e) =>
            {
                added = true;
                panel.Close();
                if (closeAfter) Close();
            };
            panel.Show();
        }
    }
}
