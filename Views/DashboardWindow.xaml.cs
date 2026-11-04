
using System;
using System.Windows;
using RestaurantApp.Services;
using RestaurantApp.Models;

namespace RestaurantApp.Views;

public partial class DashboardWindow : Window
{
    private readonly DashboardService _dashboardService;

    public DashboardWindow()
    {
        InitializeComponent();

        _dashboardService = new DashboardService();

        SetAccessByRole();

        Loaded += DashboardWindow_Loaded;
    }

    private void SetAccessByRole()
    {
        string role = AppSession.CurrentUser?.Role ?? "";

        if (role == "Admin")
        {
            SalesNavButton.Visibility = Visibility.Collapsed;
            AnalyticsNavButton.Visibility = Visibility.Collapsed;
            SalesCard.Visibility = Visibility.Collapsed;
        }
        else if (role == "Chef")
        {
            EmployeesNavButton.Visibility = Visibility.Collapsed;
            SalesNavButton.Visibility = Visibility.Collapsed;
            AnalyticsNavButton.Visibility = Visibility.Collapsed;

            EmployeesCard.Visibility = Visibility.Collapsed;
            SalesCard.Visibility = Visibility.Collapsed;
        }
    }

    private async void DashboardWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            DishesCountTextBlock.Text =
                (await _dashboardService.GetDishesCountAsync()).ToString();

            IngredientsCountTextBlock.Text =
                (await _dashboardService.GetIngredientsCountAsync()).ToString();

            EmployeesCountTextBlock.Text =
                (await _dashboardService.GetEmployeesCountAsync()).ToString();

            SalesCountTextBlock.Text =
                (await _dashboardService.GetSalesCountAsync()).ToString();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка завантаження статистики:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void EmployeesButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window = new MainWindow();
        window.Show();
        Close();
    }

    private void MenuButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window = new DishWindow();
        window.Show();
        Close();
    }

    private void IngredientButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window = new IngredientWindow();
        window.Show();
        Close();
    }

    private void RecipeButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window = new RecipeWindow();
        window.Show();
        Close();
    }

    private void SalesButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window = new SalesWindow();
        window.Show();
        Close();
    }

    private void HomeButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        // Головна сторінка вже відкрита.
    }

    private void AnalyticsButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var window = new AnalyticsWindow();
        window.Show();
        Close();
    }
}