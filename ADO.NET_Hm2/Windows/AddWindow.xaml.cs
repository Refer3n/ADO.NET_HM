using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADO.NET_Hm2
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private StorageDatabase database;

        public AddWindow(StorageDatabase database)
        {
            InitializeComponent();
            this.database = database;
        }

        private async void ApplyButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)optionsComboBox.SelectedItem;

            if (selectedItem != null)
            {
                string operationTag = (string)selectedItem.Tag;
                string resultMessage = "";

                switch (operationTag)
                {
                    case "AddProduct":
                        resultMessage = await database.AddProductAsync("Phone", "Electronics", "Supplier E", 300.99M, 8, DateTime.Now);
                        break;
                    case "AddProductType":
                        resultMessage = await database.AddProductTypeAsync("Stationery");
                        break;
                    case "AddSupplier":
                        resultMessage = await database.AddSupplierAsync("Supplier F");
                        break;
                    default:
                        break;
                }

                MessageBox.Show(resultMessage, "Operation Result", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close();
            }
        }

    }
}
