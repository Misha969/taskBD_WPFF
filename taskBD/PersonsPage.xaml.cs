using System.Collections.ObjectModel;
using System.Data.Entity; // Для EntityState и ChangeTracker
using System.Data.Entity.Infrastructure; // Для DbUpdateException
using System.Data.Entity.Validation; // Для DbEntityValidationException
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Для KeyEventArgs
using ClientAddressManager; // Убедитесь, что это ваше корректное пространство имен

namespace taskBD // Пространство имен самой страницы
{
    public partial class PersonsPage : Page
    {
        // Экземпляр вашего контекста данных EF
        private ClientAddressesDBEntities _context;

        // Коллекция для привязки к DataGrid
        public ObservableCollection<Person> PersonsList { get; set; }

        public PersonsPage()
        {
            InitializeComponent();
            _context = new ClientAddressesDBEntities();
            PersonsList = new ObservableCollection<Person>();
            // DataContext = this; // Можно использовать, если ItemsSource в XAML привязан через {Binding PersonsList}
            // Но в LoadData мы присваиваем ItemsSource явно.
        }

        // Обработчик загрузки страницы
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        // Метод для загрузки данных из БД с возможностью поиска
        private void LoadData(string searchTerm = null)
        {
            try
            {
                IQueryable<Person> query = _context.Persons.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    string lowerSearchTerm = searchTerm.ToLower().Trim();
                    query = query.Where(p =>
                        (p.Surname != null && p.Surname.ToLower().Contains(lowerSearchTerm)) ||
                        (p.Name != null && p.Name.ToLower().Contains(lowerSearchTerm)) ||
                        (p.Email != null && p.Email.ToLower().Contains(lowerSearchTerm))
                    );
                }

                var personsFromDb = query.OrderBy(p => p.Surname).ThenBy(p => p.Name).ToList();
                PersonsList.Clear();
                foreach (var person in personsFromDb)
                {
                    PersonsList.Add(person);
                }
                PersonsDataGrid.ItemsSource = PersonsList;

                if (!string.IsNullOrWhiteSpace(searchTerm) && !PersonsList.Any())
                {
                    MessageBox.Show($"Клиенты по запросу '{searchTerm}' не найдены.", "Результаты поиска", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Кнопка "Добавить"
        private void BtnAddPerson_Click(object sender, RoutedEventArgs e)
        {
            Person newPerson = new Person
            {
                Surname = "", // Пустое для срабатывания валидации RequiredFieldRule
                Name = "Имя",
                Patronymic = "", // или null
                PhoneNumber = "", // или null
                Email = "" // или null, или "example@example.com" для проверки EmailValidationRule
            };

            _context.Persons.Add(newPerson);
            PersonsList.Add(newPerson);
            PersonsDataGrid.SelectedItem = newPerson;
            PersonsDataGrid.ScrollIntoView(newPerson);
            // Для установки фокуса на первую ячейку можно использовать Dispatcher
            // PersonsDataGrid.Focus(); // Сначала на грид
            // // Затем попытаться сфокусироваться на ячейке (требует более сложной логики)
        }

        // Кнопка "Редактировать выделенного" (пока неактивна в XAML)
        private void BtnEditPerson_Click(object sender, RoutedEventArgs e)
        {
            if (PersonsDataGrid.SelectedItem is Person selectedPerson)
            {
                MessageBox.Show("Редактирование происходит напрямую в таблице. После изменений нажмите 'Сохранить изменения'.", "Информация");
                // Если бы было отдельное окно:
                // EditPersonWindow editWindow = new EditPersonWindow(selectedPerson, _context);
                // if (editWindow.ShowDialog() == true) { LoadData(); }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите клиента для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Кнопка "Удалить"
        private void BtnDeletePerson_Click(object sender, RoutedEventArgs e)
        {
            if (PersonsDataGrid.SelectedItem is Person selectedPerson)
            {
                // Проверяем, есть ли у этого клиента несохраненные изменения (если он был только что добавлен, но не сохранен)
                var entry = _context.Entry(selectedPerson);
                if (entry.State == EntityState.Detached || entry.State == EntityState.Added)
                {
                    // Если объект еще не сохранен в БД, просто удаляем из списка и отсоединяем от контекста
                    PersonsList.Remove(selectedPerson);
                    if (entry.State == EntityState.Added)
                    {
                        _context.Entry(selectedPerson).State = EntityState.Detached;
                    }
                    MessageBox.Show("Новый клиент удален из списка (не был сохранен).", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }


                if (MessageBox.Show($"Вы уверены, что хотите удалить клиента '{selectedPerson.Surname} {selectedPerson.Name}'?\nВсе связанные адреса (если настроено каскадное удаление) также будут удалены.",
                                     "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Если есть связанные адреса и ON DELETE NO ACTION, то SaveChanges() выдаст ошибку
                        // Поэтому лучше обрабатывать DbUpdateException при сохранении
                        _context.Persons.Remove(selectedPerson);
                        PersonsList.Remove(selectedPerson);
                        // Фактическое удаление из БД - при нажатии "Сохранить изменения"
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Ошибка при подготовке к удалению: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите клиента для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Кнопка "Сохранить изменения"
        private void BtnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            bool hasWpfErrors = false;
            // Проверка визуальных ошибок в DataGrid (если строки видимы)
            // Обходим видимые строки и строки, которые могли быть отредактированы
            for (int i = 0; i < PersonsDataGrid.Items.Count; i++)
            {
                var item = PersonsDataGrid.Items[i];
                DataGridRow row = (DataGridRow)PersonsDataGrid.ItemContainerGenerator.ContainerFromIndex(i);
                if (row == null) // Если строка не сгенерирована (виртуализация), пытаемся получить ее через элемент
                {
                    // Этот блок может быть полезен, если DataGrid виртуализирован и содержит много элементов
                    // Но для простой проверки может быть достаточно перебора видимых строк
                    // Для более надежной проверки всех элементов в коллекции, лучше использовать IDataErrorInfo
                    // или валидировать каждый объект в PersonsList перед сохранением, если это возможно
                }

                if (row != null && Validation.GetHasError(row))
                {
                    hasWpfErrors = true;
                    // Можно попытаться сфокусироваться на первой ошибочной строке/ячейке
                    // row.Focus();
                    // DataGridCell cell = GetCell(row, 0); // Нужен вспомогательный метод GetCell
                    // if (cell != null) cell.Focus();
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
                _context.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                // После успешного сохранения можно обновить данные, чтобы ID новых записей отобразились корректно,
                // или чтобы отслеживаемые сущности были "чистыми".
                // LoadData(TxtSearchTerm.Text); // Перезагружаем с учетом текущего поиска
            }
            catch (DbEntityValidationException validationEx)
            {
                var errorMessages = validationEx.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => $"{x.PropertyName}: {x.ErrorMessage}");
                var fullErrorMessage = string.Join("\n", errorMessages);
                MessageBox.Show($"Ошибки валидации при сохранении в базу данных:\n{fullErrorMessage}", "Ошибка валидации EF", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DbUpdateException dbEx)
            {
                var innerExceptionMessage = dbEx.InnerException?.InnerException?.Message ?? dbEx.InnerException?.Message ?? dbEx.Message;
                MessageBox.Show($"Ошибка сохранения в базе данных: {innerExceptionMessage}", "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
                // Откат изменений в контексте, если сохранение не удалось
                var changedEntries = _context.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList();
                foreach (var entry in changedEntries)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entry.CurrentValues.SetValues(entry.OriginalValues);
                            entry.State = EntityState.Unchanged;
                            break;
                        case EntityState.Added:
                            entry.State = EntityState.Detached;
                            break;
                        case EntityState.Deleted:
                            // Здесь сложнее, нужно восстановить объект, если он был удален только из контекста
                            // Проще всего - перезагрузить данные из БД
                            entry.State = EntityState.Unchanged; // или Modified
                            break;
                    }
                }
                // И перезагружаем данные из коллекции (или из БД)
                LoadData(TxtSearchTerm.Text); // Перезагружаем данные с учетом текущего поиска
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при сохранении: {ex.Message}", "Общая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Кнопка "Обновить список" (также сбрасывает поиск)
        private void BtnRefreshPersons_Click(object sender, RoutedEventArgs e)
        {
            if (_context.ChangeTracker.HasChanges())
            {
                var result = MessageBox.Show("Есть несохраненные изменения. Вы уверены, что хотите обновить список? Все несохраненные изменения будут потеряны.",
                                             "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
            }
            _context.Dispose();
            _context = new ClientAddressesDBEntities();
            TxtSearchTerm.Text = ""; // Сбрасываем текст поиска
            LoadData(); // Загружаем все данные
            MessageBox.Show("Список клиентов обновлен.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // --- Логика поиска ---
        private void BtnSearchPerson_Click(object sender, RoutedEventArgs e)
        {
            if (_context.ChangeTracker.HasChanges())
            {
                var result = MessageBox.Show("Есть несохраненные изменения. Для выполнения поиска их нужно либо сохранить, либо отменить. Отменить изменения и продолжить поиск?",
                                             "Внимание", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Dispose();
                    _context = new ClientAddressesDBEntities();
                }
                else if (result == MessageBoxResult.Cancel || result == MessageBoxResult.No)
                {
                    if (result == MessageBoxResult.No)
                        MessageBox.Show("Пожалуйста, сначала сохраните или отмените изменения через кнопку 'Обновить'.", "Действие прервано", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            LoadData(TxtSearchTerm.Text);
        }

        private void BtnClearSearchPerson_Click(object sender, RoutedEventArgs e)
        {
            if (_context.ChangeTracker.HasChanges())
            {
                var result = MessageBox.Show("Есть несохраненные изменения. Сброс поиска отменит их. Продолжить?",
                                             "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No) return;

                _context.Dispose();
                _context = new ClientAddressesDBEntities();
            }

            TxtSearchTerm.Text = "";
            LoadData();
        }

        private void TxtSearchTerm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearchPerson_Click(sender, e);
            }
        }
    }
}