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
        private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Now);

        public ObservableCollection<TodayAction> Items { get; set; }
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
        public DateOnly SelectedDate
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
        public ICommand DeleteItemCommand { get; }
        public ICommand AddItemCommand { get; }
        public MainViewModel()
        {
            Items = new ObservableCollection<TodayAction>
                {
                    new TodayAction { Name = "Buy groceries", IsComplete = false },
                    new TodayAction { Name = "Finish report", IsComplete = true },
                    new TodayAction { Name = "Call John", IsComplete = false }
                };

            DeleteItemCommand = new RelayCommand(
                                    execute: _ => DeleteItem(),
                                    canExecute: _ => SelectedItem != null
                                );

            AddItemCommand = new RelayCommand(_ => OnAddItem());
        }
        public void OnAddItem()
        {
            var newItem = new TodayAction { Name = "New Task", IsComplete = false };
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
    }
}
