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

            SqlQuery query = new SqlQuery("skytrack");

            User user = new() { Login = Registration.Login.Text, Password = Registration.Password.Password, IsAdmin = false };

            if (query.AddUser(user))
            {
                MessageBox.Show("User registered successfully.");
            }
            else
            {
                MessageBox.Show("User registration failed.");
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