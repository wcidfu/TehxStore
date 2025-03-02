using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using TehxStore.Entities;
using TehxStore.Windows;


namespace TehxStore
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        private bool _isAdmin;
        private List<Product> _selectedProducts = new List<Product>();

        public MainWindow(User? user, bool isAdmin)
        {
            InitializeComponent();
            _currentUser = user;
            _isAdmin = isAdmin;

            if (user == null)
            {
                _isAdmin = false;
                MessageBox.Show("Вы вошли как гость.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            LoadProducts();
            DisplayUserInfo();
            AnimateTehxText(); // Запуск анимации текста "TEHX"
        }

        private void LoadProducts()
        {
            using (var context = new TehxStoreDbContext())
            {
                var products = context.Products.ToList();
                ProductItemsControl.ItemsSource = products;
            }
        }

        private void DisplayUserInfo()
        {
            UserNameTextBlock.Text = _currentUser != null
                ? $"{_currentUser.UserSurname} {_currentUser.UserName} {_currentUser.UserPatronymic}"
                : "Гость";

            AdminPanelButton.Visibility = _isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        private void AnimateTehxText()
        {
            ColorAnimation colorAnimation = new ColorAnimation
            {
                From = Colors.Blue,
                To = Colors.Red,
                Duration = TimeSpan.FromSeconds(1),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever
            };

            AnimateLetter(LetterT, colorAnimation, 0);
            AnimateLetter(LetterE, colorAnimation, 0.3);
            AnimateLetter(LetterH, colorAnimation, 0.6);
            AnimateLetter(LetterX, colorAnimation, 0.9);
        }

        private void AnimateLetter(Run letter, ColorAnimation animation, double delay)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Blue);
            letter.Foreground = brush;

            animation.BeginTime = TimeSpan.FromSeconds(delay);
            brush.BeginAnimation(SolidColorBrush.ColorProperty, animation);
        }

        private void AddToOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button)?.DataContext as Product;
            if (product != null)
            {
                if (!_selectedProducts.Any(p => p.ProductArticleNumber == product.ProductArticleNumber))
                {
                    _selectedProducts.Add(product);
                    MessageBox.Show($"{product.ProductName} добавлен в заказ.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"{product.ProductName} уже добавлен в заказ!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProducts.Count == 0)
            {
                MessageBox.Show("Вы не выбрали ни одного товара!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            OrderWindow orderWindow = new OrderWindow(new List<Product>(_selectedProducts));
            orderWindow.ShowDialog();

            _selectedProducts.Clear();
        }

        private void AdminPanelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isAdmin)
            {
                new ProductManagementWindow().Show();
                new OrderManagementWindow().Show();

                LoadProducts();
            }
        }
    }
}
