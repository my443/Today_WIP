using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Today2.Models;
using Today2.ViewModels;

namespace Today2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        // Handle the KeyDown event for the DataGrid to capture Delete key presses. 
        // Cannot be done in the ViewModel because KeyEventArgs is not available there.
        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (DataContext is MainViewModel vm && vm.DeleteItemCommand.CanExecute(null))
                {
                    vm.DeleteItemCommand.Execute(null);
                    e.Handled = true; // Prevent default deletion
                }
            }
        }

        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {

            var dataGrid = sender as DataGrid;
            dataGrid.SelectedItem = e.Row.Item;
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.Column is DataGridTextColumn)
            {
                // Get the TextBox element used for editing
                TextBox editingTextBox = e.EditingElement as TextBox;

                if (editingTextBox != null)
                {
                    string newValue = editingTextBox.Text;
                    (DataContext as MainViewModel).SelectedItem.Name = newValue;
                    (DataContext as MainViewModel)?._appDbContext.SaveChanges();
                    // Process the new value
                    // ...
                }
            }

            if (e.Column is DataGridCheckBoxColumn)
            {
                // Get the TextBox element used for editing
                CheckBox checkBox = e.EditingElement as CheckBox;

                if (checkBox != null)
                {
                    bool? isChecked = checkBox.IsChecked;
                    bool newValue = isChecked == true;              // Move if it is indeterminate, it is true.

                    (DataContext as MainViewModel).SelectedItem.IsComplete = newValue;
                    (DataContext as MainViewModel)?._appDbContext.SaveChanges();
                }
            }
            // Optionally validate or do something with e.Row.Item
            // Save changes to the database context
            //var dataGrid = sender as DataGrid;
            //dataGrid.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    (DataContext as MainViewModel)?._appDbContext.SaveChanges();
            //}), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel)?._appDbContext.SaveChanges();
        }

    }


}
