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
            // Optionally validate or do something with e.Row.Item
            // Save changes to the database context
            var dataGrid = sender as DataGrid;
            (DataContext as MainViewModel)?._appDbContext.SaveChanges();
        }

    }
}
