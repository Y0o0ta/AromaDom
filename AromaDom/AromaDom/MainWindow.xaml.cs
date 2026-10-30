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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace AromaDom
{
    
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            LoadMaterials();
        }
        private void LoadMaterials()
        {
            MaterialsDataGrid.ItemsSource = Service.LoadMaterials().DefaultView;
        }
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaterialsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                try
                {
                    int materialId = Convert.ToInt32(selectedRow["d"]);
                    decimal required = Service.CalculateMaterial(materialId);

                    ResultTextBlock.Text = $"Требуемое количество: {required:F2}";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Выберите материал");
            }
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            MaterialEditWindow editWindow = new MaterialEditWindow(null);
            editWindow.ShowDialog();
            LoadMaterials(); 
        }

        private void MaterialsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MaterialsDataGrid.SelectedItem is DataRowView selectedRow)
            {
                int materialId = Convert.ToInt32(selectedRow["id"]);

                MaterialEditWindow editWindow = new MaterialEditWindow(materialId);
                editWindow.ShowDialog();

                LoadMaterials(); 
            }
        }

    }


}
