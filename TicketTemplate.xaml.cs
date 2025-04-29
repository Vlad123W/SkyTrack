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
    /// Логика взаимодействия для TicketTemplate.xaml
    /// </summary>
    public partial class TicketTemplate : UserControl
    {
        public Flight Flight
        {
            get => (Flight)GetValue(FlightProperty);
            set => SetValue(FlightProperty, value);
        }

        public TicketTemplate()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty FlightProperty =
            DependencyProperty.Register(
                nameof(Flight),
                typeof(Flight),
                typeof(TicketTemplate),
                new PropertyMetadata(null, OnFlightChanged));


        private static void OnFlightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (TicketTemplate)d;
            control.DataContext = e.NewValue;
        }
    }
}
