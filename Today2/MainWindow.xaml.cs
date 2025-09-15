using Microsoft.Win32;
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
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            (DataContext as MainViewModel)?._appDbContext.SaveChanges();
        }

        private void OpenDatabase_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*",
                Title = "Select Database File"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ((MainViewModel)DataContext).OpenDatabase(openFileDialog.FileName);
            }
        }

        private void NewDatabase_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*",
                Title = "Create New Database"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                ((MainViewModel)DataContext).NewDatabase(saveFileDialog.FileName);
            }
        }

    }


}
