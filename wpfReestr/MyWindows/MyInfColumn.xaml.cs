using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace wpfReestr
{
    /// <summary>КЛАСС Информация полей реестра</summary>
    public partial class MyInfColumn : Window
    {
        private DataGrid PRI_DataGrid;
        private CheckBox PRI_CheckBox_Pole;

        /// <summary>КОНСТРУКТОР</summary>
        public MyInfColumn()
        {
            InitializeComponent();
            // Родительская форма
            Owner = Application.Current.MainWindow;
        }

        /// <summary>КОНСТРУКТОР</summary>
        public MyInfColumn(DataGrid pDataGrid)
        {
            InitializeComponent();
            // Родительская форма
            Owner = Application.Current.MainWindow;
            // Запоминаем таблицу
            PRI_DataGrid = pDataGrid;
        }

        /// <summary>СОБЫТИЕ После загрузки окна</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (DataGridColumn _Column in PRI_DataGrid.Columns)
            {
                StackPanel _SPanel_1 = new StackPanel();
                _SPanel_1.Orientation = Orientation.Horizontal;
                _SPanel_1.Margin = new Thickness(0, 2, 0, 2);
                PART_StackPanel.Children.Add(_SPanel_1);
                // Имя поля
                Label _Label_Pole = new Label();
                _Label_Pole.Content = _Column.Header.ToString();
                _Label_Pole.Foreground = Brushes.Navy;
                _Label_Pole.Width = 90;
                _SPanel_1.Children.Add(_Label_Pole);
                // Отображение поля
                PRI_CheckBox_Pole = new CheckBox();
                PRI_CheckBox_Pole.IsChecked = _Column.Visibility == Visibility.Visible;
                PRI_CheckBox_Pole.Margin = new Thickness(0, 8, 10, 0);
                PRI_CheckBox_Pole.Tag = _Column;
                PRI_CheckBox_Pole.Click += priCheckBox_Pole_Click;
                _SPanel_1.Children.Add(PRI_CheckBox_Pole);
                // Описание поля
                Label _Label_Discr = new Label();
                _Label_Discr.Content = _Column.GetValue(ToolTipService.ToolTipProperty).ToString();
                _SPanel_1.Children.Add(_Label_Discr);
            }
        }

        /// <summary>СОБЫТИЕ На отключение/включение Полей через CheckBox</summary>
        private void priCheckBox_Pole_Click(object sender, RoutedEventArgs e)
        {
            DataGridColumn _Column = (DataGridColumn)(sender as CheckBox).Tag; // priDataGrid.Columns[i];
            _Column.Visibility = (sender as CheckBox).IsChecked == true ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>СОБЫТИЕ Изменения статуса окна, если оно свернуто - сворачивает главное оконо</summary>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
                this.Owner.WindowState = WindowState.Minimized;
        }
    }
}
