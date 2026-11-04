using System;
using System.Windows;
using RestaurantApp.Services;
using RestaurantApp.Models;
using RestaurantApp.Views;

namespace RestaurantApp.Views;

public partial class LoginWindow : Window
{
    private readonly AuthService _authService;

    public LoginWindow()
    {
        InitializeComponent();
        _authService = new AuthService();

        Loaded += LoginWindow_Loaded;
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string username = UsernameTextBox.Text.Trim();
        string password = PasswordInput.Password;

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show(
                "Введіть логін і пароль.",
                "Авторизація",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            var user = await _authService.LoginAsync(username, password);

            if (user == null)
            {
                MessageBox.Show(
                    "Неправильний логін або пароль.",
                    "Помилка входу",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                PasswordInput.Clear();
                return;
            }

            AppSession.CurrentUser = user;

            MessageBox.Show(
                $"Вітаємо, {user.Username}!",
                "Успішний вхід",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DashboardWindow dashboard = new DashboardWindow();
            dashboard.Show();

            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка авторизації:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void LoginWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            await _authService.CreateDefaultUsersAsync();

            MessageBox.Show(
                "Облікові записи користувачів перевірено.",
                "RestaurantApp",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка створення користувачів:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
