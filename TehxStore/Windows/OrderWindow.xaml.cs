using System;
using System.Linq;
using System.Windows;
using TehxStore.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace TehxStore.Windows
{
    public partial class OrderWindow : Window
    {
        private readonly TehxStoreDbContext _context = new TehxStoreDbContext();
        private readonly List<Product> _selectedProducts;

        public OrderWindow(List<Product> selectedProducts)
        {
            InitializeComponent();
            _selectedProducts = selectedProducts;
            LoadProducts();
        }

        private void LoadProducts()
        {
            // Проверка на наличие продуктов в списке
            if (_selectedProducts == null || _selectedProducts.Count == 0)
            {
                MessageBox.Show("Нет выбранных товаров!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Печать в консоль для отладки
            Console.WriteLine($"Количество выбранных товаров: {_selectedProducts.Count}");

            // Устанавливаем ItemsSource для ListView
            OrderListView.ItemsSource = _selectedProducts;

            // Обновляем общую сумму
            CalculateTotalAmount();
        }

        private void CalculateTotalAmount()
        {
            decimal totalAmount = _selectedProducts.Sum(p => p.ProductCost * (1 - (p.ProductDiscountAmount ?? 0) / 100m));
            TotalAmountTextBlock.Text = totalAmount.ToString("C2");
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProducts.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один товар для оформления заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Создание заказа
            var order = new Order
            {
                OrderStatus = "Оформлен",
                OrderDeliveryDate = DateTime.Now.AddDays(3),
                OrderPickupPoint = "Точка самовывоза"
            };

            MessageBox.Show($"Статус: {order.OrderStatus}\nДата доставки: {order.OrderDeliveryDate}\nПункт самовывоза: {order.OrderPickupPoint}");


            try
            {
                _context.Orders.Add(order);
                _context.SaveChanges();

                foreach (var product in _selectedProducts)
                {
                    var orderProduct = new OrderProduct
                    {
                        OrderID = order.OrderID,
                        ProductArticleNumber = product.ProductArticleNumber,
                        Quantity = 1
                    };

                    _context.OrderProducts.Add(orderProduct);
                }

                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                

                if (ex.InnerException != null)
                {
                    
                }
            }


            // Сохранение продуктов в таблице OrderProduct
            foreach (var product in _selectedProducts)
            {
                var orderProduct = new OrderProduct
                {
                    OrderID = order.OrderID, // Связываем продукт с заказом через OrderID
                    ProductArticleNumber = product.ProductArticleNumber,
                    Quantity = 1 // Количество товара (по умолчанию 1, можно изменить)
                };

                _context.OrderProducts.Add(orderProduct);
            }

            

            // Подсчитаем общую стоимость для отображения пользователю
            decimal totalAmount = _selectedProducts.Sum(p => p.ProductCost * (1 - (p.ProductDiscountAmount ?? 0) / 100m));
            MessageBox.Show($"Заказ оформлен на сумму: {totalAmount:C2}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            // Закрытие окна
            this.Close();


        }
    }
}
