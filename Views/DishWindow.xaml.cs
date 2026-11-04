using System.Windows;
using System.Windows.Controls;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Views;

public partial class DishWindow : Window
{
    private readonly DishService _dishService;
    private readonly CategoryService _categoryService;

    public DishWindow()
    {
        InitializeComponent();

        _dishService = new DishService();
        _categoryService = new CategoryService();

        SetAccessByRole();

        Loaded += DishWindow_Loaded;
    }

    private void SetAccessByRole()
    {
        string role = AppSession.CurrentUser?.Role ?? "";

        if (role == "Admin")
        {
            SalesNavButton.Visibility = Visibility.Collapsed;
            AnalyticsNavButton.Visibility = Visibility.Collapsed;
        }
        else if (role == "Chef")
        {
            EmployeesNavButton.Visibility = Visibility.Collapsed;
            SalesNavButton.Visibility = Visibility.Collapsed;
            AnalyticsNavButton.Visibility = Visibility.Collapsed;

            DishFormBorder.Visibility = Visibility.Collapsed;
        }
    }

    private bool CanEditDishes()
    {
        string role = AppSession.CurrentUser?.Role ?? "";

        return role == "Director" || role == "Admin";
    }


    private async void DishWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            await LoadCategoriesAsync();
            await LoadDishesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка завантаження меню:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async Task LoadCategoriesAsync()
    {
        var categories =
            await _categoryService.GetCategoriesAsync();

        CategoryComboBox.ItemsSource = categories;
    }

    private async Task LoadDishesAsync()
    {
        var dishes =
            await _dishService.GetDishesAsync();

        DishesGrid.ItemsSource = dishes;
    }

    private void DishesGrid_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (DishesGrid.SelectedItem is not Dish dish)
        {
            return;
        }

        DishNameTextBox.Text = dish.DishName;

        CategoryComboBox.SelectedItem = CategoryComboBox.Items
            .OfType<Category>()
            .FirstOrDefault(category =>
                category.CategoryId == dish.CategoryId);

        WeightTextBox.Text = dish.Weight;

        PriceTextBox.Text = dish.Price?.ToString() ?? "";

        DescriptionTextBox.Text = dish.Description ?? "";
    }

    private async void AddButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!CanEditDishes())
        {
            return; 
        }

        if (string.IsNullOrWhiteSpace(DishNameTextBox.Text))
        {
            MessageBox.Show(
                "Введіть назву страви.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var weight = WeightTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(weight))
        {
            MessageBox.Show(
                "Введіть вагу страви.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (!decimal.TryParse(
                PriceTextBox.Text,
                out decimal price) ||
            price < 0)
        {
            MessageBox.Show(
                "Введіть коректну ціну.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (CategoryComboBox.SelectedItem is not Category selectedCategory)
        {
            MessageBox.Show(
                "Оберіть категорію.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var dish = new Dish
        {
            DishName = DishNameTextBox.Text.Trim(),

            CategoryId = selectedCategory.CategoryId,

            Weight = weight,

            Price = price,

            Description = string.IsNullOrWhiteSpace(
                DescriptionTextBox.Text)
                ? null
                : DescriptionTextBox.Text.Trim()
        };

        try
        {
            await _dishService.AddDishAsync(dish);

            MessageBox.Show(
                "Страву успішно додано.",
                "Успіх",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadDishesAsync();

            ClearDishForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося додати страву:\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void ClearDishForm()
    {
        DishNameTextBox.Clear();
        WeightTextBox.Clear();
        PriceTextBox.Clear();
        DescriptionTextBox.Clear();

        CategoryComboBox.SelectedItem = null;

        DishesGrid.SelectedItem = null;
    }

    private async void UpdateButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!CanEditDishes())
        {
            return; 
        }

        try
        {
            if (DishesGrid.SelectedItem is not Dish selectedDish)
            {
                MessageBox.Show(
                    "Оберіть страву для редагування.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(DishNameTextBox.Text))
            {
                MessageBox.Show(
                    "Введіть назву страви.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var weight = WeightTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(weight))
            {
                MessageBox.Show(
                    "Введіть вагу страви.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!decimal.TryParse(
                    PriceTextBox.Text,
                    out decimal price) ||
                price < 0)
            {
                MessageBox.Show(
                    "Введіть коректну ціну.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (CategoryComboBox.SelectedItem is not Category selectedCategory)
            {
                MessageBox.Show(
                    "Оберіть категорію.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var dish = new Dish
            {
                DishId = selectedDish.DishId,

                DishName = DishNameTextBox.Text.Trim(),

                CategoryId = selectedCategory.CategoryId,

                Weight = weight,

                Price = price,

                Description = string.IsNullOrWhiteSpace(
                    DescriptionTextBox.Text)
                    ? null
                    : DescriptionTextBox.Text.Trim()
            };

            await _dishService.UpdateDishAsync(dish);

            MessageBox.Show(
                "Страву успішно оновлено.",
                "Успіх",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadDishesAsync();

            ClearDishForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося оновити страву:\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void DeleteButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!CanEditDishes())
        {
            return; 
        }

        try
        {
            if (DishesGrid.SelectedItem is not Dish selectedDish)
            {
                MessageBox.Show(
                    "Оберіть страву для видалення.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var result = MessageBox.Show(
                $"Ви дійсно хочете видалити страву:\n\n" +
                $"{selectedDish.DishName}?",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            await _dishService.DeleteDishAsync(
                selectedDish.DishId);

            MessageBox.Show(
                "Страву успішно видалено.",
                "Успіх",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadDishesAsync();

            ClearDishForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося видалити страву:\n\n{ex.Message}",
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
        string role = AppSession.CurrentUser?.Role ?? "";

        if (role == "Chef")
            return;

        var window = new MainWindow();
        window.Show();
        Close();
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        // розділ відкритий
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
        string role = AppSession.CurrentUser?.Role ?? "";

        if (role != "Director")
            return;

        var window = new SalesWindow();
        window.Show();
        Close();
    }


    private void AnalyticsButton_Click(object sender, RoutedEventArgs e)
    {
        string role = AppSession.CurrentUser?.Role ?? "";

        if (role != "Director")
            return;

        var window = new AnalyticsWindow();
        window.Show();
        Close();
    }
}