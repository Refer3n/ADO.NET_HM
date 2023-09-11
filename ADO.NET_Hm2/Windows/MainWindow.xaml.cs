using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ADO.NET_Hm2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StorageDatabase database;
        public MainWindow()
        {
            InitializeComponent();
            database = new StorageDatabase();
        }

        private async void infoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (infoComboBox.SelectedItem != null)
            {
                string selectedMethod = (string)((ComboBoxItem)infoComboBox.SelectedItem).Tag;
                string result = string.Empty;

                Type databaseType = typeof(StorageDatabase);
                MethodInfo method = databaseType.GetMethod(selectedMethod);

                if (method != null)
                {
                    if (string.Equals(selectedMethod, "GetItemsByCategoryAsync", StringComparison.OrdinalIgnoreCase))
                    {
                        result = await (Task<string>)method.Invoke(database, new object[] { "Electronics" });
                    }
                    else if (string.Equals(selectedMethod, "GetItemsBySupplierAsync", StringComparison.OrdinalIgnoreCase))
                    {
                        result = await (Task<string>)method.Invoke(database, new object[] { "Supplier A" });
                    }
                    else
                    {
                        result = await (Task<string>)method.Invoke(database, null);
                    }
                }

                resultTextBox.Text = result;
            }
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddWindow addWindow = new AddWindow(database);
            addWindow.ShowDialog();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditWindow addWindow = new EditWindow(database);
            addWindow.ShowDialog();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteWindow addWindow = new DeleteWindow(database);
            addWindow.ShowDialog();
        }
    }
}
