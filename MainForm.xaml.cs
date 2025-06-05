using System;
using System.Linq;
using System.Threading.Tasks;
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
            flights.filter_button.Click += Filter_button_Click;
            flightSearch.search.Click += Search_Click;

            _isAdmin = isAdmin;

            if (!_isAdmin)
            {
                edit.Visibility = Visibility.Collapsed;
            }
            else
            {
                edit.Click += Edit_Click;
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
            bool isEmpty = flightSearch.MainGrid.Children.OfType<TextBox>().Any(x => string.IsNullOrWhiteSpace(x.Text));
            if (isEmpty)
            {
                ShowNotification("Заповніть всі поля!");
                return;
            }

            string from = flightSearch.From.Text.Trim();
            string to = flightSearch.To.Text.Trim();
            string dateString = flightSearch.Date.Text.Trim();

            DateTime parsedDate;
            bool isDateParsed = DateTime.TryParse(dateString, out parsedDate);

            var allFlights = flights.flightContainer.Children.OfType<TicketTemplate>()
                .Select(x => x.Flight)
                .ToList();

            var matchedFlights = allFlights
                .Where(f => f.Origin.Equals(from, StringComparison.OrdinalIgnoreCase) &&
                            f.Destination.Equals(to, StringComparison.OrdinalIgnoreCase) &&
                            (isDateParsed ? f.DepartureTime.Date == parsedDate.Date : true))
                .ToList();

            if (matchedFlights.Count == 0)
            {
                ShowNotification("Нічого не знайдено за заданими критеріями. Завантажуються всі рейси.");
                LoadFlights();
                return;
            }

            flights.flightContainer.Children.Clear();
            foreach (var item in matchedFlights)
            {
                AddFlightTemplate(item);
            }
        }

        private void LoadFlights()
        {
            SqlQuery query = new("skytrack");
            flights.flightContainer.Children.Clear();

            foreach (var flight in query.GetAllFlights())
            {
                AddFlightTemplate(flight);
            }
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
                    ShowNotification("Квиток куплено!");
                    template.IsEnabled = false;
                };

                flights.flightContainer.Children.Add(template);
            }
            catch (Exception ex)
            {
                ShowNotification($"Помилка при завантаженні рейсу: {ex.Message}");
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
                ShowNotification("Виберіть рейси для експорту!");
                return;
            }

            Cursor = Cursors.Wait;
            if (WordExport.ExportToWord(flightsToExport))
            {
                ShowNotification("Експорт завершено!");
            }
            else
            {
                ShowNotification("Експорт не вдався!");
            }
            Cursor = Cursors.Arrow;
        }

        private void Filter_button_Click(object sender, RoutedEventArgs e)
        {
            string date = flights.dateCombo.Text.Trim();
            string price = flights.priceCombo.Text.Trim();
            string seats = flights.seatsCombo.Text.Trim();

            if (date == "-" && price == "-" && seats == "-")
            {
                LoadFlights();
                return;
            }

            DateTime parsedDate;
            bool hasDate = DateTime.TryParse(date, out parsedDate);

            var filtered = flights.flightContainer.Children
                .OfType<TicketTemplate>()
                .Where(x =>
                    (date == "-" || (hasDate && x.Flight.DepartureTime.Date == parsedDate.Date)) &&
                    (price == "-" || x.Flight.Price.ToString() == price) &&
                    (seats == "-" || x.Flight.AvailableSeats.ToString() == seats))
                .ToList();

            if (filtered.Count == 0)
            {
                ShowNotification("Нічого не знайдено!");
                return;
            }

            flights.flightContainer.Children.Clear();
            foreach (var item in filtered)
            {
                flights.flightContainer.Children.Add(item);
            }
        }

        private void ShowNotification(string message)
        {
            CustomNotifyPanel panel = new() { Message = { Text = message } };
            panel.ConfirmBtn.Click += (_, __) => panel.Close();
            panel.ShowDialog();
        }
    }
}