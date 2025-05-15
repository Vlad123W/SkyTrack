using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace SkyTrack
{
    public partial class MainForm : Window
    {
        private readonly bool _isAdmin;

        public MainForm(bool isAdmin)
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            flightSearch.search.Click += Search_Click;
            _isAdmin = isAdmin;

            if (!_isAdmin)
            {
                edit.Visibility = Visibility.Collapsed;
                edit.Click -= Edit_Click;
            }

            MouseLeftButtonDown += (_, __) =>
            {
                try { DragMove(); } catch { }
            };
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5)));
            LoadFlights();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            flights.flightContainer.Children.Clear();

            SqlQuery query = new("skytrack");
            var allFlights = query.GetAllFlights();

            string from = flightSearch.From.Text.Trim();
            string to = flightSearch.To.Text.Trim();
            string date = flightSearch.Date.Text.Trim();

            var matchedFlights = allFlights;

            if (!string.IsNullOrWhiteSpace(from))
                matchedFlights = matchedFlights.FindAll(f => f.Origin.Equals(from, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(to))
                matchedFlights = matchedFlights.FindAll(f => f.Destination.Equals(to, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(date))
                matchedFlights = matchedFlights.FindAll(f => f.DepartureTime.ToString("d") == date);

            if (matchedFlights.Count == 0)
            {
                LoadFlights();
                return;
            }

            foreach (var item in matchedFlights)
                AddFlightTemplate(item);
        }

        private void LoadFlights()
        {
            flights.flightContainer.Children.Clear();
            SqlQuery query = new("skytrack");

            foreach (var flight in query.GetAllFlights())
                AddFlightTemplate(flight);
        }

        private void AddFlightTemplate(Flight flight)
        {
            try
            {
                TicketTemplate template = new() { Flight = flight };

                if (_isAdmin)
                {
                    template.Edit_Button.Click += (_, __) =>
                    {
                        EditForm form = new(flight, true)
                        {
                            Owner = this,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };

                        form.Closed += (_, __) =>
                        {
                            flights.flightContainer.Children.Clear();
                            LoadFlights();
                        };

                        form.Show();
                    };

                    template.Delete_Button.Click += (_, __) =>
                    {
                        if (MessageBox.Show("Ви впевнені що хочете видалити рейс?", "Попередження", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                            return;

                        SqlQuery query = new("skytrack");
                        query.DeleteFlight(flight.FlightId);
                        flights.flightContainer.Children.Remove(template);
                    };
                }
                else
                {
                    template.CardBorder.Triggers.Clear();
                }

                template.BookButton.Click += (_, __) =>
                {
                    template.IsEnabled = false;
                    CustomNotifyPanel panel = new() { Message = { Text = "Квиток куплено!" } };
                    panel.Show();
                };

                flights.flightContainer.Children.Add(template);
            }
            catch (Exception ex)
            {
                CustomNotifyPanel panel = new() { Message = { Text = ex.Message } };
                panel.Show();
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            EditForm form = new()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            form.Closing += (_, __) =>
            {
                if (form.added)
                {
                    flights.flightContainer.Children.Clear();
                    LoadFlights();
                }
            };

            form.ShowDialog();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Closing += MainForm_Closing;
            Close();
        }

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            new MainWindow().Show();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
