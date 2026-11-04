using System;
using System.Windows;
using RestaurantApp.Services;

namespace RestaurantApp.Views;

public partial class AnalyticsWindow : Window
{
    private readonly AnalyticsService _analyticsService;
    private readonly CategoryService _categoryService;

    public AnalyticsWindow()
    {
        InitializeComponent();

        _analyticsService = new AnalyticsService();
        _categoryService = new CategoryService();
        // Устанавливаем текущую дату по умолчанию.
        DailySalesDatePicker.SelectedDate = DateTime.Today;

        Loaded += AnalyticsWindow_Loaded;
    }

    // Формирование отчёта за выбранный день.
    private async void DailySalesButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DailySalesDatePicker.SelectedDate is not DateTime selectedDate)
        {
            MessageBox.Show("Оберіть дату для звіту.");
            return;
        }

        try
        {
            var report = await _analyticsService.GetDailySalesAsync(
                selectedDate);

            DailySalesGrid.ItemsSource = report.DefaultView;

            if (report.Rows.Count == 0)
            {
                MessageBox.Show(
                    "За обрану дату продажів не знайдено.",
                    "Інформація",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка формування звіту:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // Загрузка основных таблиц аналитики.
    private async void AnalyticsWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            PopularDishesGrid.ItemsSource =
                (await _analyticsService.GetPopularDishesAsync()).DefaultView;

            AverageSalesGrid.ItemsSource =
                (await _analyticsService.GetAverageSalesAsync()).DefaultView;

            MonthlySalesGrid.ItemsSource =
                (await _analyticsService.GetMonthlySalesAsync()).DefaultView;

            var categories = await _categoryService.GetCategoriesAsync();
            PriceCategoryComboBox.ItemsSource = categories;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка завантаження аналітики:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    
    private async void RaisePricesButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (PriceCategoryComboBox.SelectedValue == null)
        {
            MessageBox.Show("Оберіть категорію страв.");
            return;
        }

        if (!decimal.TryParse(
                PricePercentTextBox.Text,
                out decimal percent) ||
            percent <= 0 ||
            percent > 100)
        {
            MessageBox.Show(
                "Введіть відсоток від 0 до 100 (не включаючи 0).");
            return;
        }

        string categoryName =
            PriceCategoryComboBox.SelectedValue.ToString()!;

        var result = MessageBox.Show(
            $"Підвищити ціни на {percent}% для категорії «{categoryName}»?",
            "Підтвердження",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            await _analyticsService.RaisePricesAsync(
                categoryName,
                percent);

            MessageBox.Show(
                "Ціни успішно змінено.",
                "Готово",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка зміни цін:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    private void HomeButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new DashboardWindow();
        window.Show();
        Close();
    }

    private void EmployeesButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new MainWindow();
        window.Show();
        Close();
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new DishWindow();
        window.Show();
        Close();
    }

    private void IngredientButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new IngredientWindow();
        window.Show();
        Close();
    }

    private void RecipeButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new RecipeWindow();
        window.Show();
        Close();
    }

    private void SalesButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new SalesWindow();
        window.Show();
        Close();
    }

    private void AnalyticsButton_Click(object sender, RoutedEventArgs e)
    {
        // Раздел уже открыт.
    }
}
