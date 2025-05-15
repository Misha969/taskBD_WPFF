using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ClientAddressManager; // Убедитесь в правильности пространства имен

namespace taskBD
{
    public partial class CitiesPage : Page
    {
        private ClientAddressesDBEntities _context;
        public ObservableCollection<City> CitiesList { get; set; }
        public ObservableCollection<Region> RegionsForComboBox { get; set; }
        public ObservableCollection<Country> CountriesForComboBox { get; set; }

        public CitiesPage()
        {
            InitializeComponent();
            _context = new ClientAddressesDBEntities();
            CitiesList = new ObservableCollection<City>();
            RegionsForComboBox = new ObservableCollection<Region>();
            CountriesForComboBox = new ObservableCollection<Country>();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBoxSources();
            LoadData();
        }

        private void LoadComboBoxSources()
        {
            try
            {
                var regions = _context.Regions.OrderBy(r => r.Name).ToList();
                RegionsForComboBox.Clear();
                // Можно добавить "пустой" регион для выбора, если RegionID nullable
                // RegionsForComboBox.Add(new Region { ID = 0, Name = "-- Не выбран --" }); // Если ID = 0 не используется как реальный
                foreach (var region in regions) RegionsForComboBox.Add(region);
                ((CollectionViewSource)this.Resources["RegionsForComboBoxSource"]).Source = RegionsForComboBox;

                var countries = _context.Countries.OrderBy(c => c.NameFull).ToList();
                CountriesForComboBox.Clear();
                foreach (var country in countries) CountriesForComboBox.Add(country);
                ((CollectionViewSource)this.Resources["CountriesForComboBoxSource"]).Source = CountriesForComboBox;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки справочников: {ex.Message}", "Ошибка");
            }
        }

        private void LoadData()
        {
            try
            {
                var dataFromDb = _context.Cities.Include("Region").Include("Country").ToList();
                CitiesList.Clear();
                foreach (var item in dataFromDb) CitiesList.Add(item);
                CitiesDataGrid.ItemsSource = CitiesList;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки городов: {ex.Message}", "Ошибка");
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!CountriesForComboBox.Any())
            {
                MessageBox.Show("Сначала добавьте страны.", "Внимание");
                return;
            }
            // Для RegionID можно установить null, если это разрешено схемой, или выбрать первый регион
            int? defaultRegionId = RegionsForComboBox.Any() ? RegionsForComboBox.First().ID : (int?)null;

            City newItem = new City { Name = "Новый город", CountryID = CountriesForComboBox.First().ID, RegionID = defaultRegionId };
            _context.Cities.Add(newItem);
            CitiesList.Add(newItem);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CitiesDataGrid.SelectedItem is City selectedItem)
            {
                if (MessageBox.Show($"Удалить город '{selectedItem.Name}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    bool hasRelatedAddresses = _context.Addresses.Any(a => a.CityID == selectedItem.ID);
                    if (hasRelatedAddresses)
                    {
                        MessageBox.Show("Невозможно удалить город, так как с ним связаны адреса.", "Ошибка удаления");
                        return;
                    }
                    _context.Cities.Remove(selectedItem);
                    CitiesList.Remove(selectedItem);
                }
            }
            else MessageBox.Show("Выберите город.", "Внимание");
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var city in CitiesList)
                {
                    if (city.CountryID == 0 && (_context.Entry(city).State == System.Data.Entity.EntityState.Added || _context.Entry(city).State == System.Data.Entity.EntityState.Modified))
                    {
                        MessageBox.Show($"Для города '{city.Name}' не выбрана страна.", "Ошибка");
                        return;
                    }
                    // RegionID может быть null, если это разрешено (в нашей схеме он Nullable)
                    // Если RegionID = 0, а это не специальное значение "не выбран", то это может быть ошибкой
                    if (city.RegionID == 0 && RegionsForComboBox.Any(r => r.ID == 0) == false) // Если 0 не значит "не выбран"
                    {
                        // Это условие нужно уточнить, если 0 - валидный ID или специальное значение
                    }
                }
                _context.SaveChanges();
                MessageBox.Show("Изменения сохранены!", "Успех");
                // LoadData(); // Опционально для обновления навигационных свойств
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
            {
                MessageBox.Show($"Ошибка БД: {dbEx.InnerException?.Message ?? dbEx.Message}", "Ошибка");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка");
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (_context.ChangeTracker.HasChanges())
            {
                if (MessageBox.Show("Есть несохраненные изменения. Обновить? Изменения будут потеряны.", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
            }
            _context.Dispose();
            _context = new ClientAddressesDBEntities();
            LoadComboBoxSources();
            LoadData();
            MessageBox.Show("Список городов обновлен.", "Информация");
        }
    }
}