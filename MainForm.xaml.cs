using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
            bool isEmpty = flightSearch.MainGrid.Children.OfType<TextBox>().All(x => string.IsNullOrWhiteSpace(x.Text))
                           || flightSearch.MainGrid.Children.OfType<TextBox>().Any(x => string.IsNullOrWhiteSpace(x.Text));
            if (isEmpty)
            {
                CustomNotifyPanel panel = new() { Message = { Text = "Заповніть всі поля!" } };
                panel.ConfirmBtn.Click += (_, __) =>
                {
                    panel.Close();
                };
                panel.ShowDialog();
                return;
            }

            string from = flightSearch.From.Text.Trim();
            string to = flightSearch.To.Text.Trim();
            string date = flightSearch.Date.Text.Trim();


            var allFlights = flights.flightContainer.Children.OfType<TicketTemplate>()
                .Select(x => x.Flight)
                .ToList();


            var matchedFlights = allFlights;

            matchedFlights = matchedFlights.FindAll(f => f.Origin.Equals(from, StringComparison.OrdinalIgnoreCase));
            matchedFlights = matchedFlights.FindAll(f => f.Destination.Equals(to, StringComparison.OrdinalIgnoreCase));
            matchedFlights = matchedFlights.FindAll(f => f.DepartureTime.ToString("d") == date);

            if (matchedFlights.Count == 0)
            {
                LoadFlights();
                return;
            }

            flights.flightContainer.Children.Clear();
            foreach (var item in matchedFlights)
                AddFlightTemplate(item);
        }

        private void LoadFlights()
        {
            SqlQuery query = new("skytrack");
            flights.flightContainer.Children.Clear();

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
                    CustomNotifyPanel panel = new() { Message = { Text = "Квиток куплено!" } };
                    panel.ConfirmBtn.Click += (_, __) =>
                    {
                        template.IsEnabled = false;
                        panel.Close();
                    };
                    panel.ShowDialog();
                };

                flights.flightContainer.Children.Add(template);
            }
            catch (Exception ex)
            {
                CustomNotifyPanel panel = new() { Message = { Text = ex.Message } };
                panel.ConfirmBtn.Click += (_, __) =>
                {
                    panel.Close();
                };
                panel.ShowDialog();
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

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var flightsToExport = flights.flightContainer.Children.OfType<TicketTemplate>()
                .Where(x => x.SelectTicketCheckBox.IsChecked == true)
                .Select(x => x.Flight)
                .ToArray();

            if (flightsToExport.Length == 0)
            {
                CustomNotifyPanel panel = new() { Message = { Text = "Виберіть рейси для експорту!" } };
                panel.ConfirmBtn.Click += (_, __) =>
                {
                    panel.Close();
                };
                panel.ShowDialog();
                return;
            }

            if (WordExport.ExportToWord(flightsToExport))
            {
                CustomNotifyPanel panel = new() { Message = { Text = "Експорт завершено!" } };
                panel.ConfirmBtn.Click += (_, __) =>
                {
                    panel.Close();
                };
                panel.ShowDialog();
            }
            else
            {
                CustomNotifyPanel panel = new() { Message = { Text = "Експорт не вдався!" } };
                panel.ConfirmBtn.Click += (_, __) =>
                {
                    panel.Close();
                };
                panel.ShowDialog();
            }
        }
    }
}
