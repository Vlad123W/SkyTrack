using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Registration.authButton.Click += AuthButton_Click;
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(Registration.Login.Text != "" && Registration.Password.Password != "")
                {
                    DefaultAuth def = new(Registration.Login.Text, Registration.Password.Password);
                    
                    if (def.LogIn())
                    {
                        MessageBox.Show("Success!");

                        Registration.Password.Password = string.Empty;
                        Registration.Login.Text = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Wrong password or user does not exist!");
                        Registration.Password.Password = string.Empty;
                    }
                }
                else
                {
                    MessageBox.Show("All fields must be filled!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Registration_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
             DragMove();
        }
    }
}