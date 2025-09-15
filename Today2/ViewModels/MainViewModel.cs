using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Today2.Data;
using Today2.Models;

namespace Today2.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private TodayAction _selectedItem { get; set; }
        private ObservableCollection<TodayAction> _items;
        private DateTime _selectedDate = DateTime.Now;
        public AppDbContext _appDbContext;
        public IAppDbContextFactory _factory;
        public string _databasePath;

        public TodayAction SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    _appDbContext.SaveChanges();

                    OnPropertyChanged(nameof(SelectedItem));

                    // You can put additional logic here if you want to respond to selection changes
                    (DeleteItemCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate != value)
                {
                    _selectedDate = value;
                    OnPropertyChanged(nameof(SelectedDate));
                    // You can put additional logic here if you want to respond to date changes
                }
            }
        }

        public ObservableCollection<TodayAction> Items
        {
            get => _items;
            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }
        public string DatabasePath
        {
            get => _databasePath;
            set
            {
                if (_databasePath != value)
                {
                    _databasePath = value;
                    OnPropertyChanged(nameof(DatabasePath));
                }
            }
        }
        public ICommand DeleteItemCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand DateForwardCommand { get; }
        public ICommand DateBackCommand { get; }

        public ICommand NewDatabaseCommand { get; }
        public ICommand ExitApplicationCommand { get; }
        public MainViewModel()
        {
            _factory = new AppDbContextFactory();
            DatabasePath = Properties.Settings.Default.LastDatabasePath;

            ConnectToDatabase(DatabasePath);

            //Items = new ObservableCollection<TodayAction>
            //    {
            //        new TodayAction { Name = "Buy groceries", Date = DateTime.Now, IsComplete = false },
            //        new TodayAction { Name = "Finish report", Date = DateTime.Now, IsComplete = true },
            //        new TodayAction { Name = "Call John", Date = DateTime.Now.AddDays(-1), IsComplete = false }
            //    };

            DeleteItemCommand = new RelayCommand(
                                    execute: _ => DeleteItem(),
                                    canExecute: _ => SelectedItem != null
                                );

            AddItemCommand = new RelayCommand(_ => OnAddItem());
            DateForwardCommand = new RelayCommand(_ => DateForward());
            DateBackCommand = new RelayCommand(_ => DateBack());
            //OpenDatabaseCommand = new RelayCommand(_ => OpenDatabase());
            NewDatabaseCommand = new RelayCommand(_ => { /* Implement new database logic here */ });
            ExitApplicationCommand = new RelayCommand(_ => ExitApplication(null));
        }

        private void ConnectToDatabase(string dbPath)
        {
            _appDbContext = _factory.Create(dbPath);
            _appDbContext.Database.EnsureCreated();
            SelectedDate = DateTime.Now;
            DatabasePath = dbPath;
            RefreshList();
        }

        public void OnAddItem()
        {
            var newItem = new TodayAction { Name = "New Task", Date = SelectedDate, IsComplete = false };
            _appDbContext.Add(newItem);
            _appDbContext.SaveChanges();
            SelectedItem = newItem;
            RefreshList();
        }

        public void DeleteItem()
        {
            if (SelectedItem != null)
            {
                _appDbContext.Actions.Remove(SelectedItem);
                _appDbContext.SaveChanges();
                // Remove from UI collection (assuming ActionsList is your ObservableCollection)
                Items.Remove(SelectedItem);

                // Optionally, set SelectedItem to null
                SelectedItem = null;
            }
            RefreshList();
        }

        public void RefreshList()
        {
            // Implement any additional logic needed when the date changes
            var filtered = _appDbContext.Actions
                    .Where(item => item.Date.Date == SelectedDate.Date)
                    .ToList();
            Items = filtered != null ? new ObservableCollection<TodayAction>(filtered) : new ObservableCollection<TodayAction>();
        }

        public void DateForward()
        {
            SelectedDate = SelectedDate.AddDays(1);
            RefreshList();
        }
        public void DateBack()
        {
            SelectedDate = SelectedDate.AddDays(-1);
            RefreshList();
        }

        //public void OpenDatabase()
        //{

        //    // This sets the database path.
        //    Properties.Settings.Default.LastDatabasePath = @"C:\path\to\database.db";
        //    Properties.Settings.Default.Save();
        //}

        public void OpenDatabase(string selectedPath)
        {
            try
            {
                using (var context = _factory.Create(selectedPath))
                {
                    // Trigger a simple query to validate schema

                    var canConnect = context.Database.CanConnect();
                    if (!canConnect)
                        throw new Exception("Database is not accessible.");

                    // Optional: Check for a required table
                    context.Actions.FirstOrDefault();
                }

                // 3. Save to settings if valid
                Properties.Settings.Default.LastDatabasePath = selectedPath;
                Properties.Settings.Default.Save();
                // 4. Reconnect to the new database
                ConnectToDatabase(selectedPath);
            }
            catch
            {
                // Show error message if not a valid database
                MessageBox.Show(
                    "The selected file is not a valid database for this application.",
                    "Invalid Database",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        public void NewDatabase(string selectedPath)
        {
            try
            {
                // Just attempt to create or open the database file
                using (var context = _factory.Create(selectedPath))
                {
                    context.Database.EnsureCreated();
                }
                // 3. Save to settings if valid
                Properties.Settings.Default.LastDatabasePath = selectedPath;
                Properties.Settings.Default.Save();
                // 4. Reconnect to the new database
                ConnectToDatabase(selectedPath);
            }
            catch (Exception ex)
            {
                // Show error message if not a valid database
                MessageBox.Show(
                    "Could not create the database file. Error: " + ex.Message,
                    "Database Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }


        private void ExitApplication(object obj)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
