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
        public MainForm()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
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
                    flights.flightContainer.Children.Add(new TicketTemplate() { Flight = item });
                }
                catch (Exception ex)
                {
                    CustomNotifyPanel panel = new CustomNotifyPanel();
                    panel.Message.Content = ex.Message;
                }
            }
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
