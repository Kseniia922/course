using System;
using System.Threading.Tasks;
using System.Windows;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Views;

public partial class SalesWindow : Window
{
    private readonly ExpenseService _expenseService;
    private readonly DishService _dishService;

    public SalesWindow()
    {
        InitializeComponent();

        _expenseService = new ExpenseService();
        _dishService = new DishService();

        Loaded += SalesWindow_Loaded;
    }

    private async void SalesWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            await LoadDishesAsync();
            await LoadSalesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка завантаження продажів:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async Task LoadDishesAsync()
    {
        var dishes = await _dishService.GetDishesAsync();

        DishComboBox.ItemsSource = dishes;
    }

    private async Task LoadSalesAsync()
    {
        var expenses = await _expenseService.GetExpensesAsync();

        SalesGrid.ItemsSource = expenses;
    }

    private async void AddButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DishComboBox.SelectedItem is not Dish selectedDish)
        {
            MessageBox.Show("Оберіть страву.");
            return;
        }

        if (SaleDatePicker.SelectedDate is not DateTime saleDate)
        {
            MessageBox.Show("Оберіть дату.");
            return;
        }

        if (!int.TryParse(
                BlanksTextBox.Text,
                out int blanks) || blanks < 0)
        {
            MessageBox.Show(
                "Введіть коректну кількість заготовок.");
            return;
        }

        if (!int.TryParse(
                PortionsSoldTextBox.Text,
                out int portionsSold) || portionsSold < 0)
        {
            MessageBox.Show(
                "Введіть коректну кількість проданих порцій.");
            return;
        }

        var expense = new Expense
        {
            DishId = selectedDish.DishId,
            DateExpense = saleDate,
            Blanks = blanks,
            PortionsSold = portionsSold
        };

        try
        {
            await _expenseService.AddExpenseAsync(expense);

            MessageBox.Show(
                "Запис про продаж успішно додано.",
                "Успіх",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadSalesAsync();

            ClearSaleForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося додати запис:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    private async void UpdateButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (SalesGrid.SelectedItem is not Expense selectedExpense)
        {
            MessageBox.Show(
                "Оберіть запис для редагування.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (DishComboBox.SelectedItem is not Dish selectedDish)
        {
            MessageBox.Show(
                "Оберіть страву.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (SaleDatePicker.SelectedDate is not DateTime saleDate)
        {
            MessageBox.Show(
                "Оберіть дату.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (!int.TryParse(BlanksTextBox.Text, out int blanks)
            || blanks < 0)
        {
            MessageBox.Show(
                "Введіть коректну кількість заготовок.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (!int.TryParse(
                PortionsSoldTextBox.Text,
                out int portionsSold)
            || portionsSold < 0)
        {
            MessageBox.Show(
                "Введіть коректну кількість проданих порцій.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var expense = new Expense
        {
            ExpenseId = selectedExpense.ExpenseId,
            DishId = selectedDish.DishId,
            DateExpense = saleDate,
            Blanks = blanks,
            PortionsSold = portionsSold
        };

        try
        {
            await _expenseService.UpdateExpenseAsync(expense);

            MessageBox.Show(
                "Запис про продаж успішно оновлено.",
                "Успіх",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadSalesAsync();

            ClearSaleForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося оновити запис:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    private void SalesGrid_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (SalesGrid.SelectedItem is not Expense expense)
        {
            return;
        }

        var selectedDish = DishComboBox.Items
            .OfType<Dish>()
            .FirstOrDefault(d => d.DishId == expense.DishId);

        DishComboBox.SelectedItem = selectedDish;

        SaleDatePicker.SelectedDate = expense.DateExpense;

        BlanksTextBox.Text = expense.Blanks.ToString();

        PortionsSoldTextBox.Text = expense.PortionsSold.ToString();
    }


    private async void DeleteButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            if (SalesGrid.SelectedItem is not Expense selectedExpense)
            {
                MessageBox.Show(
                    "Оберіть запис для видалення.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var result = MessageBox.Show(
                $"Ви дійсно хочете видалити цей запис про продаж?\n\n" +
                $"Страва: {selectedExpense.DishName}\n" +
                $"Заготовки: {selectedExpense.Blanks}\n" +
                $"Продані порції: {selectedExpense.PortionsSold}",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            await _expenseService.DeleteExpenseAsync(
                selectedExpense.ExpenseId);

            MessageBox.Show(
                "Запис про продаж успішно видалено.",
                "Успіх",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadSalesAsync();

            ClearSaleForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося видалити запис:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    private void ClearSaleForm()
    {
        DishComboBox.SelectedItem = null;
        SaleDatePicker.SelectedDate = null;
        BlanksTextBox.Clear();
        PortionsSoldTextBox.Clear();
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
        // розділ уже відкритий
    }

    private void AnalyticsButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new AnalyticsWindow();
        window.Show();
        Close();
    }
}