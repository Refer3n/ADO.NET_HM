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
    /// Interaction logic for DeleteWindow.xaml
    /// </summary>
    public partial class DeleteWindow : Window
    {
        private StorageDatabase database;

        public DeleteWindow(StorageDatabase database)
        {
            InitializeComponent();
            this.database = database;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)optionsComboBox.SelectedItem;

            if (selectedItem != null)
            {
                string operationTag = (string)selectedItem.Tag;
                string resultMessage = "";

                switch (operationTag)
                {
                    case "DeleteProduct":
                        resultMessage = database.DeleteProduct("Phone");
                        break;
                    case "DeleteProductType":
                        resultMessage = database.DeleteProductType("Flowers");
                        break;
                    case "DeleteSupplier":
                        resultMessage = database.DeleteSupplier("Supplier G");
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
