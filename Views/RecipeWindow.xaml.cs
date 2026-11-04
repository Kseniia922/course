using System.Windows;
using System.Windows.Controls;
using RestaurantApp.Models;
using RestaurantApp.Services;

namespace RestaurantApp.Views;

public partial class RecipeWindow : Window
{
    private readonly RecipeService _recipeService;
    private readonly DishService _dishService;
    private readonly IngredientService _ingredientService;

    public RecipeWindow()
    {
        InitializeComponent();

        _recipeService = new RecipeService();
        _dishService = new DishService();
        _ingredientService = new IngredientService();

        Loaded += RecipeWindow_Loaded;
    }

    private async void RecipeWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            await LoadDishesAsync();
            await LoadIngredientsAsync();
            await LoadRecipesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка завантаження рецептів:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async Task LoadDishesAsync()
    {
        var dishes =
            await _dishService.GetDishesAsync();

        DishComboBox.ItemsSource = dishes;
    }

    private async Task LoadIngredientsAsync()
    {
        var ingredients =
            await _ingredientService.GetIngredientsAsync();

        IngredientComboBox.ItemsSource = ingredients;
    }

    private async Task LoadRecipesAsync()
    {
        var recipes =
            await _recipeService.GetRecipesAsync();

        RecipesGrid.ItemsSource = recipes;
    }

    private void RecipesGrid_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (RecipesGrid.SelectedItem is not Recipe recipe)
        {
            return;
        }

        var selectedDish = DishComboBox.Items
            .OfType<Dish>()
            .FirstOrDefault(d =>
                d.DishId == recipe.DishId);

        DishComboBox.SelectedItem = selectedDish;

        var selectedIngredient = IngredientComboBox.Items
            .OfType<Ingredient>()
            .FirstOrDefault(i =>
                i.IngredientId == recipe.IngredientId);

        IngredientComboBox.SelectedItem = selectedIngredient;

        var parts = recipe.AmountIngredient
            .Trim()
            .Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length >= 2)
        {
            string amount = string.Join(
                " ",
                parts.Take(parts.Length - 1));

            string unit = parts[^1];

            AmountTextBox.Text = amount;

            AmountUnitComboBox.SelectedItem =
                AmountUnitComboBox.Items
                    .OfType<ComboBoxItem>()
                    .FirstOrDefault(item =>
                        item.Content?.ToString() == unit);
        }
        else
        {
            AmountTextBox.Text =
                recipe.AmountIngredient;

            AmountUnitComboBox.SelectedItem = null;
        }

        ProcessingTextBox.Text =
            recipe.Processing;
    }

    private async void AddButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            if (DishComboBox.SelectedItem is not Dish selectedDish)
            {
                MessageBox.Show(
                    "Оберіть страву.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (IngredientComboBox.SelectedItem is not Ingredient selectedIngredient)
            {
                MessageBox.Show(
                    "Оберіть інгредієнт.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!decimal.TryParse(
                    AmountTextBox.Text,
                    out decimal amount) ||
                amount <= 0)
            {
                MessageBox.Show(
                    "Введіть коректну кількість.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (AmountUnitComboBox.SelectedItem is not ComboBoxItem selectedUnit)
            {
                MessageBox.Show(
                    "Оберіть одиницю виміру.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    ProcessingTextBox.Text))
            {
                MessageBox.Show(
                    "Введіть спосіб обробки.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            string unit =
                selectedUnit.Content?.ToString() ?? "";

            string amountIngredient =
                $"{AmountTextBox.Text.Trim()} {unit}";

            var recipe = new Recipe
            {
                DishId = selectedDish.DishId,

                IngredientId =
                    selectedIngredient.IngredientId,

                AmountIngredient =
                    amountIngredient,

                Processing =
                    ProcessingTextBox.Text.Trim()
            };

            await _recipeService.AddRecipeAsync(recipe);

            MessageBox.Show(
                "Рецепт успішно додано.",
                "Успіх",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadRecipesAsync();

            ClearRecipeForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося додати рецепт:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    private void ClearRecipeForm()
    {
        DishComboBox.SelectedItem = null;

        IngredientComboBox.SelectedItem = null;

        AmountTextBox.Clear();

        AmountUnitComboBox.SelectedItem = null;

        ProcessingTextBox.Clear();

        RecipesGrid.SelectedItem = null;
    }


    private async void UpdateButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            if (RecipesGrid.SelectedItem is not Recipe selectedRecipe)
            {
                MessageBox.Show(
                    "Оберіть рецепт для редагування.",
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

            if (IngredientComboBox.SelectedItem is not Ingredient selectedIngredient)
            {
                MessageBox.Show(
                    "Оберіть інгредієнт.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (!decimal.TryParse(
                    AmountTextBox.Text,
                    out decimal amount) ||
                amount <= 0)
            {
                MessageBox.Show(
                    "Введіть коректну кількість.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (AmountUnitComboBox.SelectedItem is not ComboBoxItem selectedUnit)
            {
                MessageBox.Show(
                    "Оберіть одиницю виміру.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(
                    ProcessingTextBox.Text))
            {
                MessageBox.Show(
                    "Введіть спосіб обробки.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            string unit =
                selectedUnit.Content?.ToString() ?? "";

            string amountIngredient =
                $"{AmountTextBox.Text.Trim()} {unit}";

            var recipe = new Recipe
            {
                RecipeId = selectedRecipe.RecipeId,

                DishId = selectedDish.DishId,

                IngredientId =
                    selectedIngredient.IngredientId,

                AmountIngredient =
                    amountIngredient,

                Processing =
                    ProcessingTextBox.Text.Trim()
            };

            await _recipeService.UpdateRecipeAsync(recipe);

            MessageBox.Show(
                "Рецепт успішно оновлено.",
                "Успіх",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadRecipesAsync();

            ClearRecipeForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося оновити рецепт:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void DeleteButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            if (RecipesGrid.SelectedItem is not Recipe selectedRecipe)
            {
                MessageBox.Show(
                    "Оберіть рецепт для видалення.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var result = MessageBox.Show(
                $"Ви дійсно хочете видалити рецепт:\n\n" +
                $"Страва: {selectedRecipe.DishName}\n" +
                $"Інгредієнт: {selectedRecipe.IngredientName}?",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            await _recipeService.DeleteRecipeAsync(
                selectedRecipe.RecipeId);

            MessageBox.Show(
                "Рецепт успішно видалено.",
                "Успіх",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            await LoadRecipesAsync();

            ClearRecipeForm();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Не вдалося видалити рецепт:\n\n{ex.Message}",
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
        // розділ уже відкритий
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