using System.Windows;
using System.Windows.Media.Animation;

namespace SkyTrack
{
    /// <summary>
    /// Логика взаимодействия для MainForm.xaml
    /// </summary>
    public partial class MainForm : Window
    {
        private bool _isAdmin;
        private List<Flight> tempFlights;
        
        public MainForm(bool isAdmin)
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            flightSearch.search.Click += Search_Click;
            _isAdmin = isAdmin; 

            if (!isAdmin)
            {
                edit.Visibility = Visibility.Collapsed;
                edit.Click -= edit_Click;
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

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            SqlQuery query = new("skytrack");
            bool notEmpty = false;

            if(flightSearch.From.Text == "" && flightSearch.To.Text == "" && flightSearch.Date.Text == "")
                Load();

            foreach (var item in query.GetAllFlights())
            {
                if(item.Origin == flightSearch.From.Text 
                   && item.Destination == flightSearch.To.Text 
                   && item.DepartureTime.ToString() == flightSearch.Date.Text)
                {
                    TicketTemplate template = new TicketTemplate { Flight = item };
                    flights.flightContainer.Children.Add(template);
                    
                    if(!notEmpty)
                    {
                        flights.flightContainer.Children.Clear();
                        notEmpty = true;
                    }
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(Window.OpacityProperty, fadeIn);
            Load();
        }

        private void Load()
        {
            SqlQuery query = new("skytrack");

            foreach (var item in query.GetAllFlights())
            {
                try
                {
                    TicketTemplate el = new() { Flight = item };
                    
                    if(_isAdmin)
                    {
                        el.Edit_Button.Click += (s, e) =>
                        {
                            EditForm form = new(item, true)
                            {
                                Owner = this,
                                WindowStartupLocation = WindowStartupLocation.CenterOwner
                            };

                            form.Closed += (s, e) =>
                            {
                                flights.flightContainer.Children.Clear();
                                Load();
                            };
                            form.Show();
                        };

                        el.Delete_Button.Click += (s, e) =>
                        {
                            SqlQuery query = new("skytrack");
                            query.DeleteFlight(item.FlightId);
                            flights.flightContainer.Children.Remove(el);
                        };
                       
                    }
                    else
                    {
                        el.CardBorder.Triggers.Clear();
                    }

                    el.BookButton.Click += (s, e) =>
                    {
                        el.IsEnabled = false;
                        CustomNotifyPanel panel = new CustomNotifyPanel();
                        panel.Message.Content = "Квиток куплено!";
                        panel.Show();
                    };

                    flights.flightContainer.Children.Add(el);
                }
                catch (Exception ex)
                {
                    CustomNotifyPanel panel = new CustomNotifyPanel();
                    panel.Message.Content = ex.Message;
                    panel.Show();
                }
            }
        }
       
        private void edit_Click(object sender, RoutedEventArgs e)
        {
            EditForm form = new()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            form.Show();  
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Closing += MainForm_Closing;
            Close();
        }

        private void MainForm_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
