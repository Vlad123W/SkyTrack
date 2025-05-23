﻿using System;
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
using System.Reflection;

namespace SkyTrack
{
    /// <summary>
    /// Логика взаимодействия для Displaying.xaml
    /// </summary>
    public partial class Displaying : UserControl
    {
        public Displaying()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            dateCombo.Items.Add("-");
            priceCombo.Items.Add("-");
            seatsCombo.Items.Add("-");

            foreach (var item in flightContainer.Children.OfType<TicketTemplate>().ToList())
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
        }
    }
}
