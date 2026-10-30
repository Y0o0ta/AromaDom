using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace AromaDom
{
    public partial class MaterialEditWindow : Window
    {
        private int? _editingMaterialId;

        public MaterialEditWindow(int? materialId)
        {
            InitializeComponent();

            if (materialId.HasValue)
            {
                Title = "Редактирование материала";
                LoadMaterial(materialId.Value);
            }
            else
            {
                Title = "Добавление материала";
            }
        }

        private void LoadMaterial(int materialId)
        {
            DataRow material = Service.GetMaterialById(materialId);

            if (material == null)
            {
                MessageBox.Show(
                    "Материал не найлен",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            TypeComboBox.SelectedValue = material["type_id"];
            NameTextBox.Text = material["name"].ToString();
            PriceTextBox.Text = Convert.ToDecimal(material["price"]).ToString("F2");
            UnitTextBox.Text = material["unit"].ToString();
            PackageQtyTextBox.Text = material["pack_qty"].ToString();
            StockQtyTextBox.Text = material["stock_qty"].ToString();
            MinQtyTextBox.Text = material["min_qty"].ToString();
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(
                    "Введите наименование материала",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (TypeComboBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Выберите тип материала",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            int typeId = Convert.ToInt32(TypeComboBox.SelectedValue);

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show(
                    "Цена должна быть числом",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            if (price < 0)
            {
                MessageBox.Show(
                    "Цена не может быть отрицательной",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            string unit = UnitTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(unit))
            {
                MessageBox.Show(
                    "Введите единицу измерения",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(PackageQtyTextBox.Text, out decimal pack_qty))
            {
                MessageBox.Show(
                    "Количество в упаковке должно быть числом",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(StockQtyTextBox.Text, out decimal stock_qty))
            {
                MessageBox.Show(
                    "Количество на складе должно быть числом.",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(MinQtyTextBox.Text, out decimal min_qty))
            {
                MessageBox.Show(
                    "Минимальное количество должно быть числом",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }
            if (min_qty < 0)
            {
                MessageBox.Show(
                    "Минимальное количество не может быть отрицательным.",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (_editingMaterialId.HasValue)
            {
                Service.UpdateMaterial(_editingMaterialId.Value, name, price,
                    unit, pack_qty, stock_qty, min_qty);
            }
            else
            {
                Service.InsertMaterial(name, price,
                    unit, pack_qty, stock_qty, min_qty);
            }

            DialogResult = true;
            Close();


        }
    }
}