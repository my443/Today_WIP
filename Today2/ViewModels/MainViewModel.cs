using System.Collections.ObjectModel;
using System.Windows.Input;
using Today2.Models;
using Today2.Data;

namespace Today2.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private TodayAction _selectedItem { get; set; }
        private ObservableCollection<TodayAction> _items;
        private DateTime _selectedDate = DateTime.Now;
        private AppDbContext _appDbContext;
        public TodayAction SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
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
        public ICommand DeleteItemCommand { get; }
        public ICommand AddItemCommand { get; }
        public ICommand DateForwardCommand { get; }
        public ICommand DateBackCommand { get; }
        public MainViewModel()
        {
            var factory = new AppDbContextFactory();
            var dbPath = Properties.Settings.Default.LastDatabasePath;
            
            _appDbContext = factory.Create(dbPath);
            _appDbContext.Database.EnsureCreated();

            var actions = _appDbContext.Actions.ToList();
            Items = new ObservableCollection<TodayAction>(actions);
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

        public void OpenDatabase()
        {

            // This sets the database path.
            Properties.Settings.Default.LastDatabasePath = @"C:\path\to\database.db";
            Properties.Settings.Default.Save();
        }
    }
}
