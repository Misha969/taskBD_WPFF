using System.Collections.ObjectModel;
using System.Data.Entity; // Для EntityState, Include, AsQueryable
using System.Data.Entity.Infrastructure; // Для DbUpdateException
using System.Data.Entity.Validation; // Для DbEntityValidationException
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data; // Для CollectionViewSource
using ClientAddressManager; // Убедитесь, что это ваше корректное пространство имен

namespace taskBD
{
    public partial class AddressesPage : Page
    {
        private ClientAddressesDBEntities _context;

        public ObservableCollection<Address> AddressesList { get; set; }
        // Коллекции для ComboBox'ов в DataGrid
        public ObservableCollection<Person> PersonsForGridComboBox { get; set; }
        public ObservableCollection<Country> CountriesForGridComboBox { get; set; }
        public ObservableCollection<Region> RegionsForGridComboBox { get; set; }
        public ObservableCollection<City> CitiesForGridComboBox { get; set; }
        // Коллекция для ComboBox фильтра
        public ObservableCollection<City> FilterCitiesList { get; set; }


        public AddressesPage()
        {
            InitializeComponent();
            _context = new ClientAddressesDBEntities();

            AddressesList = new ObservableCollection<Address>();
            PersonsForGridComboBox = new ObservableCollection<Person>();
            CountriesForGridComboBox = new ObservableCollection<Country>();
            RegionsForGridComboBox = new ObservableCollection<Region>();
            CitiesForGridComboBox = new ObservableCollection<City>();
            FilterCitiesList = new ObservableCollection<City>();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboBoxSources();
            LoadData(); // Загружаем все адреса по умолчанию
        }

        private void LoadComboBoxSources()
        {
            try
            {
                // Для DataGrid ComboBox'ов
                PersonsForGridComboBox.Clear();
                _context.Persons.OrderBy(p => p.Surname).ThenBy(p => p.Name).ToList().ForEach(p => PersonsForGridComboBox.Add(p));
                ((CollectionViewSource)this.Resources["PersonsForComboBoxSource"]).Source = PersonsForGridComboBox;

                CountriesForGridComboBox.Clear();
                _context.Countries.OrderBy(c => c.NameFull).ToList().ForEach(c => CountriesForGridComboBox.Add(c));
                ((CollectionViewSource)this.Resources["CountriesForComboBoxSource"]).Source = CountriesForGridComboBox;

                RegionsForGridComboBox.Clear();
                _context.Regions.OrderBy(r => r.Name).ToList().ForEach(r => RegionsForGridComboBox.Add(r));
                ((CollectionViewSource)this.Resources["RegionsForComboBoxSource"]).Source = RegionsForGridComboBox;

                CitiesForGridComboBox.Clear();
                _context.Cities.OrderBy(c => c.Name).ToList().ForEach(c => CitiesForGridComboBox.Add(c));
                ((CollectionViewSource)this.Resources["CitiesForComboBoxSource"]).Source = CitiesForGridComboBox;

                // Для ComboBox фильтра
                FilterCitiesList.Clear();
                FilterCitiesList.Add(new City { ID = 0, Name = "-- Все города --" }); // Специальное значение для "все"
                _context.Cities.OrderBy(c => c.Name).ToList().ForEach(c => FilterCitiesList.Add(c));
                ((CollectionViewSource)this.Resources["FilterCitiesForComboBoxSource"]).Source = FilterCitiesList;

                if (CmbFilterCity != null) CmbFilterCity.SelectedValue = 0; // Устанавливаем "Все города" по умолчанию
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки справочников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadData(int? filterCityId = null) // int? для поддержки null (хотя 0 используется для "все")
        {
            try
            {
                IQueryable<Address> query = _context.Addresses
                                                    .Include(a => a.Person)
                                                    .Include(a => a.Country)
                                                    .Include(a => a.Region)
                                                    .Include(a => a.City)
                                                    .AsQueryable();

                if (filterCityId.HasValue && filterCityId.Value != 0) // 0 - это наше специальное значение "Все города"
                {
                    query = query.Where(a => a.CityID == filterCityId.Value);
                }

                var dataFromDb = query.OrderBy(a => a.Person.Surname) // Сортировка для удобства
                                      .ThenBy(a => a.Country.NameFull)
                                      .ThenBy(a => a.City.Name)
                                      .ThenBy(a => a.Street)
                                      .ToList();
                AddressesList.Clear();
                foreach (var item in dataFromDb)
                {
                    AddressesList.Add(item);
                }
                AddressesDataGrid.ItemsSource = AddressesList;

                if (filterCityId.HasValue && filterCityId.Value != 0 && !AddressesList.Any())
                {
                    var selectedCity = FilterCitiesList.FirstOrDefault(c => c.ID == filterCityId.Value);
                    MessageBox.Show($"Адреса в городе '{selectedCity?.Name}' не найдены.", "Результаты фильтрации", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки адресов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!PersonsForGridComboBox.Any() || !CountriesForGridComboBox.Any())
            {
                MessageBox.Show("Сначала заполните справочники клиентов и стран.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Address newItem = new Address
            {
                PersonID = PersonsForGridComboBox.First().ID,
                CountryID = CountriesForGridComboBox.First().ID,
                RegionID = RegionsForGridComboBox.Any() ? RegionsForGridComboBox.First().ID : (int?)null,
                CityID = CitiesForGridComboBox.Any() ? CitiesForGridComboBox.First().ID : (int?)null,
                Street = "", // Пустое для срабатывания валидации
                Building = "" // Пустое для срабатывания валидации
            };
            _context.Addresses.Add(newItem);
            AddressesList.Add(newItem);
            AddressesDataGrid.SelectedItem = newItem;
            AddressesDataGrid.ScrollIntoView(newItem);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (AddressesDataGrid.SelectedItem is Address selectedItem)
            {
                var entry = _context.Entry(selectedItem);
                if (entry.State == EntityState.Detached || entry.State == EntityState.Added)
                {
                    AddressesList.Remove(selectedItem);
                    if (entry.State == EntityState.Added) entry.State = EntityState.Detached;
                    MessageBox.Show("Новый адрес удален из списка (не был сохранен).", "Информация");
                    return;
                }

                if (MessageBox.Show($"Удалить адрес: {selectedItem.Street}, {selectedItem.Building} для клиента {selectedItem.Person?.Surname}?",
                                     "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    _context.Addresses.Remove(selectedItem);
                    AddressesList.Remove(selectedItem);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите адрес для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            bool hasWpfErrors = false;
            for (int i = 0; i < AddressesDataGrid.Items.Count; i++)
            {
                DataGridRow row = (DataGridRow)AddressesDataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                if (row != null && Validation.GetHasError(row))
                {
                    hasWpfErrors = true;
                    break;
                }
            }

            if (hasWpfErrors)
            {
                MessageBox.Show("Пожалуйста, исправьте все ошибки валидации, выделенные в таблице.",
                                "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Дополнительная программная валидация перед сохранением
                foreach (var address in AddressesList.Where(a => _context.Entry(a).State == EntityState.Added || _context.Entry(a).State == EntityState.Modified))
                {
                    if (address.PersonID == 0 || !PersonsForGridComboBox.Any(p => p.ID == address.PersonID))
                    { MessageBox.Show($"Для адреса '{address.Street}' не выбран клиент.", "Ошибка"); return; }
                    if (address.CountryID == 0 || !CountriesForGridComboBox.Any(c => c.ID == address.CountryID))
                    { MessageBox.Show($"Для адреса '{address.Street}' не выбрана страна.", "Ошибка"); return; }
                    // Поля Street и Building уже проверяются ValidationRule
                }

                _context.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DbEntityValidationException validationEx)
            {
                var errorMessages = validationEx.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => $"{x.PropertyName}: {x.ErrorMessage}");
                MessageBox.Show($"Ошибки валидации EF:\n{string.Join("\n", errorMessages)}", "Ошибка валидации EF", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DbUpdateException dbEx)
            {
                var innerExceptionMessage = dbEx.InnerException?.InnerException?.Message ?? dbEx.InnerException?.Message ?? dbEx.Message;
                MessageBox.Show($"Ошибка сохранения в базе данных: {innerExceptionMessage}", "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
                // Откат изменений
                var changedEntries = _context.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList();
                foreach (var entry in changedEntries) { /* ... логика отката как в PersonsPage ... */ }
                LoadData((int?)CmbFilterCity.SelectedValue); // Перезагружаем с учетом фильтра
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении: {ex.Message}", "Общая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (_context.ChangeTracker.HasChanges())
            {
                if (MessageBox.Show("Есть несохраненные изменения. Обновить? Они будут потеряны.", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            }
            _context.Dispose();
            _context = new ClientAddressesDBEntities();

            // Очищаем все коллекции ComboBox перед перезагрузкой
            PersonsForGridComboBox.Clear(); CountriesForGridComboBox.Clear(); RegionsForGridComboBox.Clear(); CitiesForGridComboBox.Clear(); FilterCitiesList.Clear();

            LoadComboBoxSources(); // Это также установит CmbFilterCity.SelectedValue = 0
            LoadData(); // Загрузит данные с учетом фильтра (т.е. все)
            MessageBox.Show("Список адресов обновлен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // --- Логика фильтрации ---
        private void CmbFilterCity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return; // Не выполнять фильтрацию, пока страница не загружена полностью

            if (_context.ChangeTracker.HasChanges())
            {
                var result = MessageBox.Show("Есть несохраненные изменения. Применение фильтра отменит их. Продолжить?",
                                             "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    // Попытка вернуть предыдущее значение ComboBox (может быть сложно)
                    // Проще всего сказать пользователю сохранить/отменить и не менять фильтр,
                    // или применить фильтр, отменив изменения.
                    // Если отменяем изменения:
                    _context.Dispose();
                    _context = new ClientAddressesDBEntities();
                    LoadComboBoxSources(); // Важно для DataGrid ComboBox'ов
                    // Предыдущее значение фильтра уже потеряно, так что просто загружаем с новым
                }
                else if (result == MessageBoxResult.Yes)
                {
                    // Отменяем изменения
                    _context.Dispose();
                    _context = new ClientAddressesDBEntities();
                    LoadComboBoxSources();
                }
                else
                { // Cancel
                    return;
                }
            }

            if (CmbFilterCity.SelectedValue is int selectedCityId)
            {
                LoadData(selectedCityId);
            }
            else if (CmbFilterCity.SelectedItem == null && FilterCitiesList.Any(c => c.ID == 0)) // Если выбранный элемент удален, но "Все города" есть
            {
                CmbFilterCity.SelectedValue = 0; // Снова выбрать "Все города"
                                                 // LoadData(0); // Вызовется из-за изменения SelectedValue
            }
            else
            {
                LoadData(); // Показать все, если что-то пошло не так
            }
        }

        private void BtnClearAddressFilter_Click(object sender, RoutedEventArgs e)
        {
            if (_context.ChangeTracker.HasChanges())
            {
                if (MessageBox.Show("Есть несохраненные изменения. Сброс фильтра отменит их. Продолжить?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
                _context.Dispose();
                _context = new ClientAddressesDBEntities();
                LoadComboBoxSources(); // Перезагрузить все источники
            }

            // Устанавливаем SelectedValue и если он не изменился (уже был 0), то явно вызываем LoadData
            var previousValue = (int?)CmbFilterCity.SelectedValue;
            CmbFilterCity.SelectedValue = 0;
            if (previousValue == 0) // Если значение не изменилось, SelectionChanged не сработает
            {
                LoadData(0);
            }
        }
    }
}