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

namespace ADO.NET_Hm1
{
    public partial class MainWindow : Window
    {
        FruitVegetableDatabase database;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                database = new FruitVegetableDatabase();
                database.CreateDatabase();
                database.InsertTestData();
            }
            catch(Exception ex)
            {
                resultTextBox.Text = ex.Message;
            }
        }

        private void infoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (infoComboBox.SelectedItem != null)
            {
                string selectedMethod = (string)((ComboBoxItem)infoComboBox.SelectedItem).Tag;
                string result = string.Empty;

                Type databaseType = typeof(FruitVegetableDatabase);
                MethodInfo method = databaseType.GetMethod(selectedMethod);

                if (method != null)
                {
                    if (string.Equals(selectedMethod, "GetItemsByLowerCalories", StringComparison.OrdinalIgnoreCase))
                    {
                        result = (string)method.Invoke(database, new object[] { 50.00m });
                    }
                    else if (string.Equals(selectedMethod, "GetItemsByHigherCalories", StringComparison.OrdinalIgnoreCase))
                    {
                        result = (string)method.Invoke(database, new object[] { 50.00m });
                    }
                    else if (string.Equals(selectedMethod, "GetItemsByCalories", StringComparison.OrdinalIgnoreCase))
                    {
                        result = (string)method.Invoke(database, new object[] { 30.00m, 70.00m });
                    }
                    else if (string.Equals(selectedMethod, "GetItemsByColor", StringComparison.OrdinalIgnoreCase))
                    {
                        result = (string)method.Invoke(database, new object[] { "Yellow", "Red" });
                    }
                    else if (string.Equals(selectedMethod, "GetItemCountBySelectetColor", StringComparison.OrdinalIgnoreCase))
                    {
                        result = (string)method.Invoke(database, new object[] { "Red" });
                    }
                    else
                    {
                        result = (string)method.Invoke(database, null);
                    }
                }

                resultTextBox.Text = result;
            }

        }
    }
}
