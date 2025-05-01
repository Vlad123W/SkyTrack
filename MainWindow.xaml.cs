using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
            Loaded += Window_Loaded;
            RegistrationPanel.authButton.Click += AuthButton_Click;
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

                if (RegistrationPanel.Login.Text != "" && RegistrationPanel.Password.Password != "")
                {
                    DefaultAuth def = new(RegistrationPanel.Login.Text, RegistrationPanel.Password.Password);
                    

                    if (def.LogIn())
                    {
                        custom.Message.Content = "Успішний вхід!";
                        custom.Show();

                        RegistrationPanel.Password.Password = string.Empty;
                        RegistrationPanel.Login.Text = string.Empty;

                        custom.ConfirmBtn.Click += (sender, e) =>
                        {
                            custom.Close();
                            MainForm main = new(def.user.IsAdmin);
                            main.Show();
                            this.Close();
                        };
                    }
                    else
                    {
                        custom.Message.Content = "Зареєструйтесь або спробуйте ще раз.";
                        custom.Show();

                        RegistrationPanel.Password.Password = string.Empty;
                    }
                }
                else
                {
                   custom.Message.Content = "Всі поля повинні бути заповненими!";
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

                custom.Message.Content = "Помилка: " + ex.Message;
                custom.Show();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void RegistrationPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
             DragMove();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if(!regActive)
            {
                RegistrationPanel.authButton.Click -= AuthButton_Click;
                RegistrationPanel.authButton.Click += RegNewAcc;
            
                RegistrationPanel.authButton.Content = "Зареєструватися";
            }
            else
            {
                RegistrationPanel.authButton.Click += AuthButton_Click;
                RegistrationPanel.authButton.Click -= RegNewAcc;

                RegistrationPanel.authButton.Content = "Увійти";
            }

            regActive = !regActive;
        }

        private void RegNewAcc(object sender, RoutedEventArgs e)
        {
            Registration auth = new Registration(RegistrationPanel.Login.Text, RegistrationPanel.Password.Password);

            CustomNotifyPanel custom = new();
            custom.ConfirmBtn.Click += (sender, e) =>
            {
                custom.Close();
            };

            if (auth.LogIn())
            {
                RegistrationPanel.authButton.Content = "Увійти";
                
                RegistrationPanel.authButton.Click -= RegNewAcc;
                RegistrationPanel.authButton.Click += AuthButton_Click;

                custom.Message.Content = "Успішна реєстрація!";
                custom.Show();

                custom.ConfirmBtn.Click += (sender, e) =>
                {
                    custom.Close();
                    MainForm main = new(false);
                    main.Show();
                    this.Close();
                };
            }
            else
            {
                custom.Message.Content = "Користувач вже існує, виконайте вхід.";
                custom.Show();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            this.BeginAnimation(Window.OpacityProperty, fadeIn);
        }
    }
}