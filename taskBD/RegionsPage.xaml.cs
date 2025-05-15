using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data; // Для CollectionViewSource
using ClientAddressManager;

namespace taskBD
{
    public partial class RegionsPage : Page
    {
        private ClientAddressesDBEntities _context;
        public ObservableCollection<Region> RegionsList { get; set; }
        public ObservableCollection<Country> CountriesForComboBox { get; set; }

        public RegionsPage()
        {
            InitializeComponent();
            _context = new ClientAddressesDBEntities();
            RegionsList = new ObservableCollection<Region>();
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
                var countries = _context.Countries.OrderBy(c => c.NameFull).ToList();
                CountriesForComboBox.Clear();
                foreach (var country in countries)
                {
                    CountriesForComboBox.Add(country);
                }
                // Назначаем источник для ComboBox в DataGrid
                var countriesSource = (CollectionViewSource)this.Resources["CountriesForComboBoxSource"];
                countriesSource.Source = CountriesForComboBox;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки стран для ComboBox: {ex.Message}", "Ошибка");
            }
        }

        private void LoadData()
        {
            try
            {
                // Include(r => r.Country) чтобы EF загрузил связанную страну,
                // это полезно, если DisplayMemberPath для ComboBox не сработает сразу
                // или если вы хотите получить доступ к Country.NameFull где-то еще.
                var dataFromDb = _context.Regions.Include("Country").ToList();
                RegionsList.Clear();
                foreach (var item in dataFromDb)
                {
                    RegionsList.Add(item);
                }
                RegionsDataGrid.ItemsSource = RegionsList;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            // Убедимся, что есть страны для выбора
            if (!CountriesForComboBox.Any())
            {
                MessageBox.Show("Сначала добавьте страны в справочник.", "Внимание");
                return;
            }
            Region newItem = new Region { Name = "Новый регион", CountryID = CountriesForComboBox.First().ID }; // Значение по умолчанию
            _context.Regions.Add(newItem);
            RegionsList.Add(newItem);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (RegionsDataGrid.SelectedItem is Region selectedItem)
            {
                if (MessageBox.Show($"Удалить регион '{selectedItem.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    // Проверка на связанные города/адреса
                    bool hasRelatedCities = _context.Cities.Any(c => c.RegionID == selectedItem.ID);
                    bool hasRelatedAddresses = _context.Addresses.Any(a => a.RegionID == selectedItem.ID);
                    if (hasRelatedCities || hasRelatedAddresses)
                    {
                        MessageBox.Show("Невозможно удалить регион, так как с ним связаны города или адреса.", "Ошибка удаления");
                        return;
                    }

                    _context.Regions.Remove(selectedItem);
                    RegionsList.Remove(selectedItem);
                }
            }
            else
            {
                MessageBox.Show("Выберите регион для удаления.", "Внимание");
            }
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Перед сохранением, убедимся что для всех новых или измененных регионов выбран CountryID
                foreach (var region in RegionsList)
                {
                    if (region.CountryID == 0 && (_context.Entry(region).State == System.Data.Entity.EntityState.Added || _context.Entry(region).State == System.Data.Entity.EntityState.Modified))
                    {
                        // В реальном приложении ID = 0 не должен быть у существующей страны.
                        // Это просто пример проверки. Лучше использовать nullable int (int?) для CountryID, если страна может быть не выбрана,
                        // и проверять на null. Но по вашей схеме CountryID NOT NULL.
                        var countryExists = CountriesForComboBox.Any(c => c.ID == region.CountryID);
                        if (!countryExists && region.CountryID != 0) // Если ID не 0, но такой страны нет в списке (маловероятно)
                        {
                            MessageBox.Show($"Для региона '{region.Name}' указана несуществующая страна (ID: {region.CountryID}).", "Ошибка");
                            return;
                        }
                        if (region.CountryID == 0) // если CountryID по какой-то причине не установился
                        {
                            MessageBox.Show($"Для региона '{region.Name}' не выбрана страна.", "Ошибка");
                            return;
                        }
                    }
                }

                _context.SaveChanges();
                MessageBox.Show("Изменения сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                // После сохранения, если есть автогенерируемые ID, они обновятся в объектах.
                // Также, навигационные свойства (например, region.Country) могут быть неактуальны для новых добавленных элементов,
                // если не были явно установлены или не было перезагрузки.
                // Можно вызвать LoadData() для обновления отображения, чтобы видеть корректные данные, включая навигационные свойства.
                // LoadData(); // Раскомментируйте, если нужно обновить грид полностью
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException dbEx)
            {
                MessageBox.Show($"Ошибка сохранения в базе данных: {dbEx.InnerException?.Message ?? dbEx.Message}", "Ошибка базы данных");
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
            LoadComboBoxSources(); // Перезагрузить источники для комбобоксов
            LoadData();
            MessageBox.Show("Список регионов обновлен.", "Информация");
        }
    }
}