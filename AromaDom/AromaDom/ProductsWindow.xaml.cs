using System;
using System.Data;
using System.Windows;

namespace AromaDom
{
    public partial class ProductsWindow : Window
    {
        private readonly int _materialId;

        public ProductsWindow(int materialId)
        {
            InitializeComponent();
            _materialId = materialId;
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                DataTable products = Service.LoadProductsByMaterial(_materialId);
                ProductsDataGrid.ItemsSource = products.DefaultView;

                if (products.Rows.Count == 0)
                {
                    HeaderTextBlock.Text = "Для этого материала продукция не найдена.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось загрузить список продукции: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}