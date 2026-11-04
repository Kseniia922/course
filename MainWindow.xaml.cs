using System;
using System.Windows;
using System.Windows.Controls;
using RestaurantApp.Models;
using RestaurantApp.Services;
using RestaurantApp.Views;

namespace RestaurantApp;

public partial class MainWindow : Window
{
    private readonly EmployeeService _employeeService;
    private readonly PositionService _positionService;
    public MainWindow()
    {
        InitializeComponent();

        _employeeService = new EmployeeService();
        _positionService = new PositionService();

        SetAccessByRole();

        Loaded += MainWindow_Loaded;
    }

    private void SetAccessByRole()
    {
        string role = AppSession.CurrentUser?.Role ?? "";

        if (role == "Admin")
        {
            EmployeesFormBorder.Visibility = Visibility.Collapsed;

            SalesNavButton.Visibility = Visibility.Collapsed;
            AnalyticsNavButton.Visibility = Visibility.Collapsed;
        }
        else if (role == "Chef")
        {
            MessageBox.Show(
                "У вас немає доступу до розділу працівників.",
                "Доступ заборонено",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            var dashboard = new DashboardWindow();
            dashboard.Show();

            Close();
        }
    }

    private bool IsDirector()
    {
        return AppSession.CurrentUser?.Role == "Director";
    }

    private async void MainWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            var employees =
                await _employeeService.GetEmployeesAsync();

            EmployeesGrid.ItemsSource = employees;

            var positions =
                await _positionService.GetPositionsAsync();

            PositionComboBox.ItemsSource = positions;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка завантаження працівників:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }



    private async void AddButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!IsDirector())
        {
            return;
        }

        try
        {
            if (string.IsNullOrWhiteSpace(FullnameTextBox.Text))
            {
                MessageBox.Show(
                    "Введіть ПІБ працівника.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show(
                    "Введіть номер телефону.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (PositionComboBox.SelectedItem is not Position selectedPosition)
            {
                MessageBox.Show(
                    "Оберіть посаду.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            string? sex = null;

            if (SexComboBox.SelectedItem is ComboBoxItem selectedSex)
            {
                sex = selectedSex.Content?.ToString();
            }


            var phone = PhoneTextBox.Text.Trim();

            if (phone.StartsWith("0"))
                {
                    phone = "+380" + phone.Substring(1);
                }

            if (!phone.StartsWith("+380") || phone.Length != 13)
                {
                    MessageBox.Show(
                        "Введіть коректний номер телефону у форматі +380XXXXXXXXX.",
                        "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                    return;
                }

            var employee = new Employee
            {
                EmployeeFullname = FullnameTextBox.Text.Trim(),
                EmployeePhone = phone,
                Birth = BirthDatePicker.SelectedDate,
                Sex = sex,
                PosadaId = selectedPosition.PosadaId,
                Direction = DirectionTextBox.Text.Trim()
            };

            await _employeeService.AddEmployeeAsync(employee);

            MessageBox.Show(
                "Працівника успішно додано!",
                "Готово",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            var employees =
                await _employeeService.GetEmployeesAsync();

            EmployeesGrid.ItemsSource = employees;

            FullnameTextBox.Clear();
            PhoneTextBox.Clear();
            BirthDatePicker.SelectedDate = null;
            SexComboBox.SelectedItem = null;
            PositionComboBox.SelectedItem = null;
            DirectionTextBox.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка додавання працівника:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }


    private void EmployeesGrid_SelectionChanged(
        object sender,
        System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (EmployeesGrid.SelectedItem is not Employee employee)
        {
            return;
        }

        FullnameTextBox.Text = employee.EmployeeFullname;
        PhoneTextBox.Text = employee.EmployeePhone;

        BirthDatePicker.SelectedDate = employee.Birth;

        DirectionTextBox.Text = employee.Direction;

        if (employee.Sex == "Чоловік")
        {
            SexComboBox.SelectedIndex = 0;
        }
        else if (employee.Sex == "Жінка")
        {
            SexComboBox.SelectedIndex = 1;
        }

        var selectedPosition = PositionComboBox.Items
            .OfType<Position>()
            .FirstOrDefault(p => p.PosadaId == employee.PosadaId);

        PositionComboBox.SelectedItem = selectedPosition;
    }


    private async void UpdateButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!IsDirector())
            {
                return;
            }
        try
        {
            if (EmployeesGrid.SelectedItem is not Employee selectedEmployee)
            {
                MessageBox.Show(
                    "Оберіть працівника для редагування.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(FullnameTextBox.Text))
            {
                MessageBox.Show(
                    "Введіть ПІБ працівника.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                MessageBox.Show(
                    "Введіть номер телефону.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            if (PositionComboBox.SelectedItem is not Position selectedPosition)
            {
                MessageBox.Show(
                    "Оберіть посаду.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var phone = PhoneTextBox.Text.Trim();

            if (phone.StartsWith("0"))
            {
                phone = "+380" + phone.Substring(1);
            }

            if (!phone.StartsWith("+380") || phone.Length != 13)
            {
                MessageBox.Show(
                    "Введіть коректний номер телефону у форматі +380XXXXXXXXX.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            string? sex = null;

            if (SexComboBox.SelectedItem is ComboBoxItem selectedSex)
            {
                sex = selectedSex.Content?.ToString();
            }

            var employee = new Employee
            {
                EmployeeId = selectedEmployee.EmployeeId,

                EmployeeFullname =
                    FullnameTextBox.Text.Trim(),

                EmployeePhone = phone,

                Birth =
                    BirthDatePicker.SelectedDate,

                Sex = sex,

                PosadaId =
                    selectedPosition.PosadaId,

                Direction =
                    DirectionTextBox.Text.Trim()
            };

            await _employeeService.UpdateEmployeeAsync(employee);

            MessageBox.Show(
                "Дані працівника успішно оновлено!",
                "Готово",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            var employees =
                await _employeeService.GetEmployeesAsync();

            EmployeesGrid.ItemsSource = employees;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка оновлення працівника:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void DeleteButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (!IsDirector())
            {
                return;
            }
        try
        {
            if (EmployeesGrid.SelectedItem is not Employee selectedEmployee)
            {
                MessageBox.Show(
                    "Оберіть працівника для видалення.",
                    "Помилка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var result = MessageBox.Show(
                $"Ви дійсно хочете видалити працівника:\n\n" +
                $"{selectedEmployee.EmployeeFullname}?",
                "Підтвердження видалення",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            await _employeeService.DeleteEmployeeAsync(
                selectedEmployee.EmployeeId);

            MessageBox.Show(
                "Працівника успішно видалено!",
                "Готово",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            var employees =
                await _employeeService.GetEmployeesAsync();

            EmployeesGrid.ItemsSource = employees;

            FullnameTextBox.Clear();
            PhoneTextBox.Clear();
            BirthDatePicker.SelectedDate = null;
            SexComboBox.SelectedItem = null;
            PositionComboBox.SelectedItem = null;
            DirectionTextBox.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Помилка видалення працівника:\n\n{ex.Message}",
                "Помилка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }




    private void MenuButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var dishWindow = new DishWindow();
        dishWindow.Show();
        Close();
    }


    private void IngredientButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var ingredientWindow = new IngredientWindow();
        ingredientWindow.Show();
        Close();
    }


    private void RecipeButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var recipeWindow = new RecipeWindow();
        recipeWindow.Show();
        Close();
    }

    private void SalesButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        var salesWindow = new SalesWindow();
        salesWindow.Show();
        Close();
    }

    
    private void HomeButton_Click(object sender, RoutedEventArgs e)
    {
        var dashboardWindow = new DashboardWindow();
        dashboardWindow.Show();
        Close();
    }

    private void AnalyticsButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new AnalyticsWindow();
        window.Show();
        Close();
    }
}