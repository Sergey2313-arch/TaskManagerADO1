using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using TaskManagerADO.DAL;

namespace TaskManagerADO
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            GridTasks.ItemsSource = Database.GetTasks().DefaultView;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var title = TitleBox.Text?.Trim();
            var desc  = DescBox.Text?.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Введите название задачи.", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                Database.AddTask(title!, string.IsNullOrWhiteSpace(desc) ? null : desc);
                TitleBox.Text = "";
                DescBox.Text = "";
                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка добавления", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e) => RefreshGrid();

        private void GridTasks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (GridTasks.SelectedItem is DataRowView row)
            {
                var id = Convert.ToInt32(row["id"]);
                try
                {
                    Database.ToggleCompleted(id);
                    RefreshGrid();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка обновления", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (GridTasks.SelectedItem is DataRowView row)
            {
                var id = Convert.ToInt32(row["id"]);
                if (MessageBox.Show($"Удалить задачу #{id}?", "Подтверждение",
                        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Database.DeleteTask(id);
                        RefreshGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите строку в таблице.", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
