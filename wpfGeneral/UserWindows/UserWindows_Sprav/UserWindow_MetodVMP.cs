using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Справочник Методов ВМП</summary>
    public class UserWindow_MetodVMP : VirtualUserWindow
    {
        private ComboBox PRI_ComboBox_1;
        private CheckBox PRI_CheckBox_1;
        
        /// <summary>СВОЙСТВО Код метода ВМП</summary>
        public int PROP_IDHM { get; private set; }

        /// <summary>СВОЙСТВО Наименование метода ВМП</summary>
        public string PROP_HMName { get; private set; }

        /// <summary>Начальный фильтр SQL</summary>
        private readonly string PRI_Filtr = "";


        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_MetodVMP(string pDiag = "")
        {
            // Имя таблицы
            PRO_TableName = "StrahVMP";
            // Заголовок
            Title = "Справочник Методы ВМП:";
            // Размеры окна
            Width = 890;
            MinWidth = Width;
            Height = 600;
            // Сортируем по полю Наименование метода ВМП
            PRO_PoleSort = 3;
            PRO_PoleFiltr = "Names";
            // Фильтр по диагнозу
            if (pDiag.Length > 4)
                PRI_Filtr = $"where (Diag like '%{pDiag.Substring(0, 5)};%' or Diag like '%{pDiag.Substring(0, 3)};%' or right(HVid, 1) = 'Р' )";
            // Открываем таблицу
            MET_OpenForm();
            // Создаем фильтр
            MET_CreateFiltr();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.StrahVMP_Select_1(PRI_Filtr);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "Вид ВМП", "Код", "Модель", "Наименование Метода ВМП"  };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[3].Width = 200;    // Name
        }

        /// <summary>МЕТОД Создание фильтров</summary>
        private void MET_CreateFiltr()
        {
            PART_Grid.RowDefinitions[0].Height = new GridLength(80, GridUnitType.Auto);
            PART_Expander.Visibility = Visibility.Visible;
            Border _Border = new Border();
            _Border.Style = (Style)FindResource("Border_2");
            PART_Expander.Content = _Border;
            PART_Expander.IsExpanded = true;
            StackPanel _SPanel = new StackPanel();
            _Border.Child = _SPanel;
            // ---- Настраиваем фильтр
            StackPanel _SPanel_1 = new StackPanel();
            _SPanel_1.Orientation = Orientation.Horizontal;
            _SPanel_1.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_1);
            Label _Label_1 = new Label();
            _Label_1.Content = "Вид ВМП:";
            _Label_1.Foreground = Brushes.Navy;
            _SPanel_1.Children.Add(_Label_1);
            // Вид ВМП
            PRI_ComboBox_1 = new ComboBox();
            PRI_ComboBox_1.Width = 600;
            MySql.MET_DsAdapterFill(MyQuery.varVidVMP_Select_1(), "varVidVMP");
            PRI_ComboBox_1.ItemsSource = new DataView(MyGlo.DataSet.Tables["varVidVMP"]);
            PRI_ComboBox_1.DisplayMemberPath = "NameVid";
            PRI_ComboBox_1.SelectedValuePath = "Cod";
            PRI_ComboBox_1.SelectedValue = "09.00.16.001Р";
            PRI_ComboBox_1.SelectionChanged += PART_SelectionChanged;
            _SPanel_1.Children.Add(PRI_ComboBox_1);
            // Check Вида ВМП
            PRI_CheckBox_1 = new CheckBox();
            PRI_CheckBox_1.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_1.Content = "учитывать по виду ВМП";
            PRI_CheckBox_1.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_1.Foreground = Brushes.Navy;
            PRI_CheckBox_1.IsChecked = true;
            PRI_CheckBox_1.Click += PART_CheckBox_Click;
            _SPanel_1.Children.Add(PRI_CheckBox_1);

            PRO_Where = $" HVid = '{PRI_ComboBox_1.SelectedValue}'";
            MET_Filter();
        }

        /// <summary>СОБЫТИЕ На отключение/включение элеменов фильтра через CheckBox</summary>
        private void PART_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            PRI_ComboBox_1.IsEnabled = PRI_CheckBox_1.IsChecked == true;

            PRO_Where = "";

            // Фильтр по отделению стационара
            if (PRI_ComboBox_1.IsEnabled)
                PRO_Where = $" HVid = '{PRI_ComboBox_1.SelectedValue}'";

            MET_Filter();
        }

        /// <summary>СОБЫТИЕ Выбор Метода ВМП</summary>
        private void PART_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PRO_Where = "";

            // Фильтр по отделению стационара
            if (PRI_ComboBox_1.IsEnabled)
                PRO_Where = $" HVid = '{PRI_ComboBox_1.SelectedValue}'";

            MET_Filter();
        }
        
        /// <summary>МЕТОД Выбор данных</summary>
        protected override void MET_Select()
        {
            if (!PROP_FlagButtonSelect)
                return;

            // Список выбора
            try
            {
                DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
                
                PROP_IDHM = Convert.ToInt16(_DataRowView.Row["IDHM"]);
                PROP_HMName = Convert.ToString(_DataRowView.Row["HMName"]);
                PROP_Return = true;
            }
            catch
            {
                PROP_Return = false;
            }
            Close();
        }
    }
}
