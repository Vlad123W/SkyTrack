using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace SkyTrack
{
    public partial class MainWindow : Window
    {
        private bool regActive;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            RegistrationPanel.authButton.Click += AuthButton_Click;

            MouseLeftButtonDown += (_, __) =>
            {
                try { DragMove(); } catch { }
            };
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(RegistrationPanel.Login.Text) ||
                string.IsNullOrWhiteSpace(RegistrationPanel.Password.Password))
            {
                ShowNotification("Всі поля повинні бути заповненими!");
                return;
            }
            
            DefaultAuth def = new(RegistrationPanel.Login.Text, RegistrationPanel.Password.Password);

            if (def.LogIn())
            {
                ShowNotification("Успішний вхід!", () =>
                {
                    ClearAuthFields();
                    new MainForm(def.user.IsAdmin).Show();
                    Close();
                });
            }
            else
            {
                ShowNotification("Зареєструйтесь або спробуйте ще раз.");
                RegistrationPanel.Password.Password = string.Empty;
            }
        }

        private void RegNewAcc(object sender, RoutedEventArgs e)
        {
            Registration auth = new(RegistrationPanel.Login.Text, RegistrationPanel.Password.Password);

            if (auth.LogIn())
            {
                ShowNotification("Успішна реєстрація!", () =>
                {
                    ClearAuthFields();
                    regActive = false;
                    ToggleAuthMode();
                    new MainForm(false).Show();
                    Close();
                });
            }
            else
            {
                ShowNotification("Користувач вже існує, виконайте вхід.");
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            ToggleAuthMode();
            regActive = !regActive;
        }

        private void ToggleAuthMode()
        {
            RegistrationPanel.authButton.Click -= regActive ? RegNewAcc : AuthButton_Click;
            RegistrationPanel.authButton.Click += regActive ? AuthButton_Click : RegNewAcc;
            RegistrationPanel.authButton.Content = regActive ? "Увійти" : "Зареєструватися";
            Window_Loaded(this, new());
        }

        private void ShowNotification(string message, Action? onClose = null)
        {
            CustomNotifyPanel panel = new();
            panel.Message.Text = message;
            panel.ConfirmBtn.Click += (_, __) =>
            {
                panel.Close();
                onClose?.Invoke();
            };
            panel.ShowDialog();
        }

        private void ClearAuthFields()
        {
            RegistrationPanel.Password.Password = string.Empty;
            RegistrationPanel.Login.Text = string.Empty;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5)));
        }
    }
}
