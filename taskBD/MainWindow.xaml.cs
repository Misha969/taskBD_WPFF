using System.Windows;
using taskBD;
// Добавьте пространства имен для ваших страниц, если они в другой папке
// Например: using ClientAddressManager.Pages;

namespace taskBD
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Можно установить начальную страницу при запуске
            // MainFrame.Navigate(new PersonsPage());
        }

        private void OpenPersonsPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PersonsPage());
        }

        private void OpenAddressesPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AddressesPage());
        }

        private void OpenCitiesPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CitiesPage());
        }

        private void OpenRegionsPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RegionsPage());
        }

        private void OpenCountriesPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CountriesPage());
        }

        private void ExitApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}