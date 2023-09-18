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

namespace ADO.NET_Hm4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StudentsDatabase _database;
        private Func<string> lastSelectedMethod;

        public MainWindow()
        {
            InitializeComponent();
            _database = new StudentsDatabase();

            lastSelectedMethod = () => ExecuteSelectedMethod();
        }

        private void infoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (infoComboBox.SelectedItem != null)
            {
                lastSelectedMethod = () => ExecuteSelectedMethod();

                resultTextBox.Text = lastSelectedMethod();
            }
        }

        private string ExecuteSelectedMethod()
        {
            string selectedMethod = (string)((ComboBoxItem)infoComboBox.SelectedItem).Tag;
            string result = string.Empty;

            Type databaseType = typeof(StudentsDatabase);
            MethodInfo method = databaseType.GetMethod(selectedMethod);

            if (method != null)
            {
                object[] parameters = null;

                result = (string)method.Invoke(_database, parameters);
            }

            return result;
        }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var result = _database.AddTestStudent();

            MessageBox.Show(result, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            resultTextBox.Text = lastSelectedMethod();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var result = _database.UpdateLastStudent();

            MessageBox.Show(result, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            resultTextBox.Text = lastSelectedMethod();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = _database.RemoveLastStudent();

            MessageBox.Show(result, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            resultTextBox.Text = lastSelectedMethod();
        }
    }
}
