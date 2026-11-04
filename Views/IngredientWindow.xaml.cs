using System.Windows;
using System.Windows.Controls;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Views;

public partial class IngredientWindow : Window
{
    private readonly IngredientService _ingredientService;

    public IngredientWindow()
    {
        InitializeComponent();

        _ingredientService = new IngredientService();

        Loaded += IngredientWindow_Loaded;
    }

    private async void IngredientWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            LoadCategories();
            LoadMeasuring();
            await LoadIngredientsAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка завантаження інгредієнтів:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }



    private void LoadCategories()
    {
        IngredientCategoryComboBox.ItemsSource = new List<string>
        {
            "м’ясо",
            "риба",
            "гриби",
            "водорості",
            "макаронні вироби",
            "молочний продукт",
            "яйця",
            "овочі",
            "фрукти",
            "зернові",
            "горіхи",
            "насіння",
            "олія",
            "сіль",
            "цукор"
        };
    }


    private void LoadMeasuring()
    {
        MeasuringComboBox.ItemsSource = new List<string>
        {
            "кг",
            "гр",
            "шт",
            "мл"
        };
    }


    private async Task LoadIngredientsAsync()
    {
        var ingredients =
            await _ingredientService.GetIngredientsAsync();

        IngredientsGrid.ItemsSource = ingredients;
    }



    private void IngredientsGrid_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
            if (IngredientsGrid.SelectedItem is not Ingredient ingredient)
        {
            return;
        }

        IngredientNameTextBox.Text =
            ingredient.IngredientName;

        IngredientCategoryComboBox.SelectedItem =
            ingredient.IngredientCategory;

        MeasuringComboBox.SelectedItem =
            ingredient.Measuring;
    }


    private async void AddButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(
                IngredientNameTextBox.Text))
        {
            MessageBox.Show(
                "Введіть назву інгредієнта.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (IngredientCategoryComboBox.SelectedItem is not string selectedCategory)
        {
            MessageBox.Show(
                "Оберіть категорію інгредієнта.",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        string? measuring = null;

        if (MeasuringComboBox.SelectedItem is string selectedMeasuring)
        {
            measuring = selectedMeasuring;
        }

        var ingredient = new Ingredient
        {
            IngredientName =
                IngredientNameTextBox.Text.Trim(),

            IngredientCategory =
                selectedCategory,

            Measuring =
                measuring
        };

        try
        {
            await _ingredientService
                .AddIngredientAsync(ingredient);

            MessageBox.Show(
                "Інгредієнт успішно додано.",
                "Успіх",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadIngredientsAsync();

            ClearIngredientForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося додати інгредієнт:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    private void ClearIngredientForm()
    {
        IngredientNameTextBox.Clear();

        IngredientCategoryComboBox.SelectedItem = null;

        MeasuringComboBox.SelectedItem = null;

        IngredientsGrid.SelectedItem = null;
    }



    private async void UpdateButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        try
        {
            if (IngredientsGrid.SelectedItem is not Ingredient selectedIngredient)
            {
                MessageBox.Show(
                    "Оберіть інгредієнт для редагування.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    IngredientNameTextBox.Text))
            {
                MessageBox.Show(
                    "Введіть назву інгредієнта.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (IngredientCategoryComboBox.SelectedItem is not string selectedCategory)
            {
                MessageBox.Show(
                    "Оберіть категорію інгредієнта.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            string? measuring = null;

            if (MeasuringComboBox.SelectedItem is string selectedMeasuring)
            {
                measuring = selectedMeasuring;
            }

            var ingredient = new Ingredient
            {
                IngredientId = selectedIngredient.IngredientId,

                IngredientName =
                    IngredientNameTextBox.Text.Trim(),

                IngredientCategory =
                    selectedCategory,

                Measuring =
                    measuring
            };

            await _ingredientService
                .UpdateIngredientAsync(ingredient);

            MessageBox.Show(
                "Інгредієнт успішно оновлено.",
                "Успіх",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadIngredientsAsync();

            ClearIngredientForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося оновити інгредієнт:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    private async void DeleteButton_Click(object sender,RoutedEventArgs e)
    {
        try
        {
            if (IngredientsGrid.SelectedItem is not Ingredient selectedIngredient)
            {
                MessageBox.Show(
                    "Оберіть інгредієнт для видалення.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var result = MessageBox.Show(
                $"Ви дійсно хочете видалити інгредієнт:\n\n" +
                $"{selectedIngredient.IngredientName}?",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            await _ingredientService.DeleteIngredientAsync(
                selectedIngredient.IngredientId);

            MessageBox.Show(
                "Інгредієнт успішно видалено.",
                "Успіх",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadIngredientsAsync();

            ClearIngredientForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося видалити інгредієнт:\n\n{ex.Message}",
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
        // розділ відкритий
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
        var window = new AnalyticsWindow();
        window.Show();
        Close();
    }
}