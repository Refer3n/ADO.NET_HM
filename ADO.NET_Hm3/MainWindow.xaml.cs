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

namespace ADO.NET_Hm3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CountriesDatabase database;

        public MainWindow()
        {
            InitializeComponent();
            database = new CountriesDatabase();
        }

        private void infoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (infoComboBox.SelectedItem != null)
            {
                string selectedMethod = (string)((ComboBoxItem)infoComboBox.SelectedItem).Tag;
                string result = string.Empty;

                Type databaseType = typeof(CountriesDatabase);
                MethodInfo method = databaseType.GetMethod(selectedMethod);

                if (method != null)
                {
                    object[] parameters = null;

                    if (string.Equals(selectedMethod, "GetCitiesByCountry", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters = new object[] { "Ukraine" };
                    }
                    else if (string.Equals(selectedMethod, "GetCountriesWithAreaGreaterThan", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters = new object[] { 1000.0 };
                    }
                    else if (string.Equals(selectedMethod, "GetCountriesWithPopulationGreaterThan", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters = new object[] { 10000000 };
                    }
                    else if (string.Equals(selectedMethod, "GetCountriesWithAreaInRange", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters = new object[] { 1000000.0, 5000000.0 };
                    }
                    else if (string.Equals(selectedMethod, "GetTop3CitiesByPopulation", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters = new object[] { "Ukraine" };
                    }
                    else if (string.Equals(selectedMethod, "GetCountriesWithAreaInRange", StringComparison.OrdinalIgnoreCase))
                    {
                        parameters = new object[] { 100000.0, 5000000.0 };
                    }

                    result = (string)method.Invoke(database, parameters);
                }

                resultTextBox.Text = result;
            }
        }

    }
}
