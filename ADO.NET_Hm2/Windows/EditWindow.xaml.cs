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
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private StorageDatabase database;

        public EditWindow(StorageDatabase database)
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
                    case "UpdateProduct":
                        resultMessage = await database.UpdateProductAsync("Phone", "Phone", "Electronics",
                            "Supplier E", 400.0M, 5, new DateTime(2023, 1, 1));
                        break;
                    case "UpdateProductType":
                        resultMessage = await database.UpdateProductTypeAsync("Stationery", "Flowers");
                        break;
                    case "UpdateSupplier":
                        resultMessage = await database.UpdateSupplierAsync("Supplier F", "Supplier G");
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
