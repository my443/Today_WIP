using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Today2.Models;

namespace Today2.ViewModels
{
    class MainViewModel : ViewModelBase
    {
        private TodayAction _selectedItem { get; set; }
        private ObservableCollection<TodayAction> _items;
        private DateTime _selectedDate = DateTime.Now;
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
            Items = new ObservableCollection<TodayAction>
                {
                    new TodayAction { Name = "Buy groceries", Date = DateTime.Now, IsComplete = false },
                    new TodayAction { Name = "Finish report", Date = DateTime.Now, IsComplete = true },
                    new TodayAction { Name = "Call John", Date = DateTime.Now.AddDays(-1), IsComplete = false }
                };

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
            var newItem = new TodayAction { Name = "New Task", Date = DateTime.Now, IsComplete = false };
            Items.Add(newItem);
            SelectedItem = newItem;
        }

        public void DeleteItem()
        {
            if (SelectedItem != null)
            {
                Items.Remove(SelectedItem);
            }
        }

        public void OnDateChanged()
        {
            // Implement any additional logic needed when the date changes
            var filtered = Items
                    .Where(item => item.Date.Date == SelectedDate.Date)
                    .ToList();
            Items = filtered != null ? new ObservableCollection<TodayAction>(filtered) : new ObservableCollection<TodayAction>();
        }

        public void DateForward()
        {
            SelectedDate = SelectedDate.AddDays(1);
            OnDateChanged();
        }
        public void DateBack()
        {
            SelectedDate = SelectedDate.AddDays(-1);
            OnDateChanged();
        }
    }
}
