using System;
using System.Windows;
using TehxStore.Entities;

namespace TehxStore.Windows
{
    public partial class ProductEditorWindow : Window
    {
        public Product Product { get; private set; }

        public ProductEditorWindow(Product product = null)
        {
            InitializeComponent();
            Product = product ?? new Product();

            if (product != null)
            {
                ProductArticleNumberTextBox.Text = product.ProductArticleNumber;
                ProductArticleNumberTextBox.IsEnabled = false; // Если редактирование, ID менять нельзя

                NameTextBox.Text = product.ProductName;
                DescriptionTextBox.Text = product.ProductDescription;
                ManufacturerTextBox.Text = product.ProductManufacturer;
                DiscountTextBox.Text = product.ProductDiscountAmount?.ToString();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductArticleNumberTextBox.Text))
            {
                MessageBox.Show("Введите артикул (ID)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Product.ProductArticleNumber = ProductArticleNumberTextBox.Text;
            Product.ProductName = NameTextBox.Text;
            Product.ProductCategory = CategoryTextBox.Text;
            Product.ProductPhoto = PhotoTextBox.Text;
            Product.ProductDescription = DescriptionTextBox.Text;
            Product.ProductManufacturer = ManufacturerTextBox.Text;
            Product.ProductStatus = StatusComboBox.Text;
            Product.ProductDiscountAmount = int.TryParse(DiscountTextBox.Text, out var discount) ? (int?)discount : null;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
