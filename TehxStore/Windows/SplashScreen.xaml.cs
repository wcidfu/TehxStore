using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace TehxStore.Windows
{
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
            ShowSplashScreen();
        }

        private void ShowSplashScreen()
        {
            // Анимация появления текста
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(2),
                AutoReverse = false
            };

            // Запуск анимации текста
            SplashText.BeginAnimation(OpacityProperty, opacityAnimation);

            // Используем таймер для задержки перед открытием SignInWindow
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2.5) // Чуть больше длительности анимации
            };

            timer.Tick += (s, e) =>
            {
                timer.Stop(); // Останавливаем таймер

                SignInWindow signInWindow = new SignInWindow();
                signInWindow.Show();

                this.Close(); // Закрываем текущее окно **после** открытия SignInWindow
            };

            timer.Start();
        }
    }
}
