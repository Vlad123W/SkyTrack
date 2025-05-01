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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SkyTrack
{
    /// <summary>
    /// Логика взаимодействия для MainForm.xaml
    /// </summary>
    public partial class MainForm : Window
    {
        private bool _isAdmin;

        public MainForm(bool isAdmin)
        {
            InitializeComponent();
            Loaded += Window_Loaded;

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
       
        private EventTrigger CreateEventTrigger(RoutedEvent routedEvent, string storyboardKey)
        {
            return new EventTrigger(routedEvent)
            {
                Actions =
            {
                new BeginStoryboard
                {
                    Storyboard = (Storyboard)FindResource(storyboardKey)
                }
            }
            };
        }

        private void edit_Click(object sender, RoutedEventArgs e)
        {
            EditForm form = new EditForm();
            form.Owner = this;
            form.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            form.Closed += (s, e) =>
            {
                flights.flightContainer.Children.Clear();
                Load();
            };
            form.Show();  
        }
    }
}
