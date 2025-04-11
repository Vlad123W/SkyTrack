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
        private bool regActive;

        public MainWindow()
        {
            InitializeComponent();
            Registration.authButton.Click += AuthButton_Click;
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CustomNotifyPanel custom = new();
                custom.ConfirmBtn.Click += (sender, e) =>
                {
                    custom.Close();
                };

                if (Registration.Login.Text != "" && Registration.Password.Password != "")
                {
                    DefaultAuth def = new(Registration.Login.Text, Registration.Password.Password);
                    

                    if (def.LogIn())
                    {
                        custom.Message.Content = "Login successfully!";
                        custom.Show();

                        Registration.Password.Password = string.Empty;
                        Registration.Login.Text = string.Empty;
                    }
                    else
                    {
                        custom.Message.Content = "Register or try again.";
                        custom.Show();

                        Registration.Password.Password = string.Empty;
                    }
                }
                else
                {
                   custom.Message.Content = "All fields must be filled!";
                   custom.Show();
                }
            }
            catch (Exception ex)
            {
                CustomNotifyPanel custom = new();
                custom.ConfirmBtn.Click += (sender, e) =>
                {
                    custom.Close();
                };

                custom.Message.Content = "An error occurred: " + ex.Message;
                custom.Show();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Registration_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
             DragMove();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if(!regActive)
            {
                Registration.authButton.Click -= AuthButton_Click;
                Registration.authButton.Click += RegNewAcc;
            
                Registration.authButton.Content = "Зареєструватися";
            }
            else
            {
                Registration.authButton.Click += AuthButton_Click;
                Registration.authButton.Click -= RegNewAcc;

                Registration.authButton.Content = "Увійти";
            }

            regActive = !regActive;
        }

        private void RegNewAcc(object sender, RoutedEventArgs e)
        {
            Registration auth = new Registration(Registration.Login.Text, Registration.Password.Password);

            CustomNotifyPanel custom = new();
            custom.ConfirmBtn.Click += (sender, e) =>
            {
                custom.Close();
            };

            if (auth.LogIn())
            {
                Registration.authButton.Content = "Увійти";
                
                Registration.authButton.Click -= RegNewAcc;
                Registration.authButton.Click += AuthButton_Click;

                custom.Message.Content = "Успішна реєстрація!";
                custom.Show();
            }
            else
            {
                custom.Message.Content = "Користувач вже існує, виконайте вхід.";
                custom.Show();
            }
        }
    }
}