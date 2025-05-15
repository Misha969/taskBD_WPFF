using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
// Убедитесь, что это пространство имен содержит ваши EF классы
using ClientAddressManager; // или taskBD, или ваше_пространство_имен_модели

namespace taskBD
{
    public partial class CountriesPage : Page
    {
        private ClientAddressesDBEntities _context;
        public ObservableCollection<Country> CountriesList { get; set; }

        public CountriesPage()
        {
            InitializeComponent();
            _context = new ClientAddressesDBEntities();
            CountriesList = new ObservableCollection<Country>();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                var dataFromDb = _context.Countries.ToList();
                CountriesList.Clear();
                foreach (var item in dataFromDb)
                {
                    CountriesList.Add(item);
                }
                CountriesDataGrid.ItemsSource = CountriesList;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Country newItem = new Country { NameFull = "Новая страна" };
            _context.Countries.Add(newItem);
            CountriesList.Add(newItem);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CountriesDataGrid.SelectedItem is Country selectedItem)
            {
                if (MessageBox.Show($"Удалить страну '{selectedItem.NameFull}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    // Проверка на связанные регионы/города/адреса (если есть ON DELETE NO ACTION)
                    // Для стран, если регионы/города/адреса НЕ могут быть без страны, то сначала нужно удалить их
                    // или если в БД настроено ON DELETE CASCADE для Regions.CountryID, Cities.CountryID, Addresses.CountryID
                    // то связанные записи удалятся автоматически.
                    // Если есть ограничение NO ACTION, то удаление не пройдет, пока есть ссылки.

                    // Пример проверки (упрощенный, лучше обрабатывать DbUpdateException при сохранении):
                    bool hasRelatedRegions = _context.Regions.Any(r => r.CountryID == selectedItem.ID);
                    bool hasRelatedCities = _context.Cities.Any(c => c.CountryID == selectedItem.ID); // И/или через регионы
                    bool hasRelatedAddresses = _context.Addresses.Any(a => a.CountryID == selectedItem.ID);

                    if (hasRelatedRegions || hasRelatedCities || hasRelatedAddresses)
                    {
                        MessageBox.Show("Невозможно удалить страну, так как с ней связаны регионы, города или адреса. Сначала удалите или измените связанные записи.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }


                    _context.Countries.Remove(selectedItem);
                    CountriesList.Remove(selectedItem);
                }
            }
            else
            {
                MessageBox.Show("Выберите страну для удаления.", "Внимание");
            }
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Изменения сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
            {
                MessageBox.Show($"Ошибка сохранения в базе данных. Возможно, вы пытаетесь удалить страну, с которой связаны другие записи.\n{dbEx.InnerException?.Message ?? dbEx.Message}", "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (_context.ChangeTracker.HasChanges())
            {
                if (MessageBox.Show("Есть несохраненные изменения. Обновить? Изменения будут потеряны.", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            }
            _context.Dispose();
            _context = new ClientAddressesDBEntities();
            LoadData();
            MessageBox.Show("Список стран обновлен.", "Информация");
        }
    }
}