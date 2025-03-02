using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TehxStore.Entities;

namespace TehxStore.Windows
{
    public partial class OrderManagementWindow : Window
    {
        private readonly TehxStoreDbContext _context = new TehxStoreDbContext();

        public OrderManagementWindow()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            var orders = _context.Orders
                .Join(_context.OrderProducts, o => o.OrderID, op => op.OrderID, (o, op) => new { o, op })
                .Join(_context.Products, op => op.op.ProductArticleNumber, p => p.ProductArticleNumber, (op, p) => new { op.o, p })
                .GroupBy(x => x.o.OrderID)
                .Select(g => new OrderViewEntity
                {
                    OrderID = g.Key,
                    OrderStatus = g.First().o.OrderStatus,
                    OrderDeliveryDate = g.First().o.OrderDeliveryDate,
                    OrderDate = g.First().o.OrderDeliveryDate,  // исправил здесь на правильную дату заказа
                    TotalAmount = g.Sum(x => x.p.ProductCost),
                    TotalDiscount = g.Sum(x => x.p.ProductDiscountAmount ?? 0),
                    ProductCount = g.Count(),
                    AvailableProductCount = g.Count(x => x.p.ProductQuantityInStock > 0),
                    FullyAvailable = g.All(x => x.p.ProductQuantityInStock >= 3)
                }).ToList();

            // Проверим, сколько заказов мы получили
            if (orders.Count == 0)
            {
                MessageBox.Show("Нет заказов в базе данных.");
            }

            OrdersDataGrid.ItemsSource = orders;
        }


        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SortComboBox.SelectedIndex == 0)
            {
                OrdersDataGrid.ItemsSource = ((System.Collections.IEnumerable)OrdersDataGrid.ItemsSource)
                    .Cast<OrderViewEntity>().OrderBy(o => o.TotalAmount).ToList();
            }
            else
            {
                OrdersDataGrid.ItemsSource = ((System.Collections.IEnumerable)OrdersDataGrid.ItemsSource)
                    .Cast<OrderViewEntity>().OrderByDescending(o => o.TotalAmount).ToList();
            }
        }

        private void OrdersDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            var order = e.Row.Item as OrderViewEntity;
            if (order != null)
            {
                if (order.ProductCount > order.AvailableProductCount)
                {
                    e.Row.Background = new SolidColorBrush(Color.FromRgb(255, 140, 0)); // #ff8c00
                }
                else if (order.FullyAvailable)
                {
                    e.Row.Background = new SolidColorBrush(Color.FromRgb(32, 178, 170)); // #20b2aa
                }
            }
        }

        // Обработчик нажатия на кнопку "Удалить"
        private void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var order = button?.DataContext as OrderViewEntity;

            if (order != null)
            {
                // Получаем заказ из базы данных по ID
                var orderToDelete = _context.Orders.FirstOrDefault(o => o.OrderID == order.OrderID);

                if (orderToDelete != null)
                {
                    // Удаляем заказ
                    _context.Orders.Remove(orderToDelete);
                    _context.SaveChanges();

                    // Перезагружаем список заказов
                    LoadOrders();
                }
            }
        }

        private void OrdersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Этот метод можно использовать, если нужно добавить дополнительные действия при изменении выделенной строки
        }
    }
}
