using System;
using System.Threading.Tasks;
using System.Windows;

namespace PersonalAryChat
{
    public partial class SplashScreenWindow : Window
    {
        public SplashScreenWindow()
        {
            InitializeComponent();
            Loaded += SplashCarregar;
        }

        private async void SplashCarregar(object sender, RoutedEventArgs e)
        {
            await Task.Delay(4000); // 4 segundos

            MainWindow main = new MainWindow();
            main.Show();

            this.Close();
        }
    }
}
