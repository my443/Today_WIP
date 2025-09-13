using System.Windows;
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
    }
}