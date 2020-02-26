using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserWindows
{

    /// <summary>КЛАСС Таблицы "APSTAC"</summary>
    public class UserWindow_APSTAC : VirtualUserWindow
    {
        private DatePicker PRI_DatePicker_1;
        private CheckBox PRI_CheckBox_1;
        private ComboBox PRI_ComboBox_2;
        private CheckBox PRI_CheckBox_2;


        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_APSTAC()
        {            
            // Имя таблицы
            PRO_TableName = "APSTAC";
            // Если строка ввода ищет через SQL
            PRO_FlagTextSql = true;
            // Заголовок
            Title = "Выбор нового пациента Стационара:";
            // Размеры окна
            Width = 900;
            MinWidth = Width;
            Height = 600;
            // Подсказка в строке поиска
            PART_TextBox.WatermarkContent = "Введите ФИО пациента (например: ИВАНОВ Ф И)";
            // Сортируем по Фамилии
            PRO_PoleSort = 1;
            // Показываем в подсказке 
            PRO_PoleBarPanel = 3;
            // Разрешаем выбирать записи
            PROP_FlagButtonSelect = true;
            // Создаем фильтр
            MET_CreateFiltr();
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.APSTAC_Select_3(PRO_SqlWhere);
        }
        
        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "", "Ном.", "ФИО пациента", "Дата р.", "Диаг.", "Поступил", "Выписан", "Врачи", "Пал", "Дн.", "От." };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[3].Width = 280;    // ФИО пациента
            PART_DataGrid.Columns[7].Width = 80;     // Дата выписки
            PART_DataGrid.Columns[8].Width = 130;    // Врач
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Создание фильтров</summary>
        protected void MET_CreateFiltr()
        {
            PART_Grid.RowDefinitions[0].Height = new GridLength(80, GridUnitType.Auto);
            PART_Expander.Visibility = Visibility.Visible;
            Border _Border = new Border { Style = (Style)FindResource("Border_2") };
            PART_Expander.Content = _Border;
            StackPanel _SPanel = new StackPanel();
            _Border.Child = _SPanel;
            // ---- Настраиваем 1й фильтр
            StackPanel _SPanel_1 = new StackPanel();
            _SPanel_1.Orientation = Orientation.Horizontal;
            _SPanel_1.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_1);
            Label _Label_1 = new Label();
            _Label_1.Content = "Лежал в стационаре на дату:";
            _Label_1.Foreground = Brushes.Navy;
            _SPanel_1.Children.Add(_Label_1);
            // Дата
            PRI_DatePicker_1 = new DatePicker();
            PRI_DatePicker_1.SelectedDateFormat = DatePickerFormat.Short;
            PRI_DatePicker_1.Text = DateTime.Today.ToString();
            PRI_DatePicker_1.DisplayDateEnd = DateTime.Today;
            PRI_DatePicker_1.DisplayDateStart = DateTime.Parse("01/01/2007");
            PRI_DatePicker_1.SelectedDateChanged += PART_SelectionChanged;
            _SPanel_1.Children.Add(PRI_DatePicker_1);
            // Check Даты
            PRI_CheckBox_1 = new CheckBox();
            PRI_CheckBox_1.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_1.Content = "учитывать фильтр по дате";
            PRI_CheckBox_1.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_1.Foreground = Brushes.Navy;
            PRI_CheckBox_1.IsChecked = true;
            PRI_CheckBox_1.Click += PART_CheckBox_Click;
            _SPanel_1.Children.Add(PRI_CheckBox_1);
            // ---- Настраиваем 2й фильтр
            StackPanel _SPanel_2 = new StackPanel();
            _SPanel_2.Orientation = Orientation.Horizontal;
            _SPanel_2.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_2);
            Label _Label_2 = new Label();
            _Label_2.Content = "Отделение стационара:";
            _Label_2.Foreground = Brushes.Navy;
            _SPanel_2.Children.Add(_Label_2);
            // Отделения
            PRI_ComboBox_2 = new ComboBox();
            PRI_ComboBox_2.Width = 280;
            MySql.MET_DsAdapterFill(MyQuery.s_Otdel_Select_1(), "s_Otdel");
            PRI_ComboBox_2.ItemsSource = new DataView(MyGlo.DataSet.Tables["s_Otdel"]);
            PRI_ComboBox_2.DisplayMemberPath = "TKOD";
            PRI_ComboBox_2.SelectedValuePath = "KOD";
            PRI_ComboBox_2.SelectedValue = MyGlo.Otd;
            PRI_ComboBox_2.SelectionChanged += PART_SelectionChanged;
            _SPanel_2.Children.Add(PRI_ComboBox_2);
            // Check Отделения
            PRI_CheckBox_2 = new CheckBox();
            PRI_CheckBox_2.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_2.Content = "учитывать фильтр по отделению";
            PRI_CheckBox_2.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_2.Foreground = Brushes.Navy;
            PRI_CheckBox_2.IsChecked = true;
            PRI_CheckBox_2.Click += PART_CheckBox_Click;
            _SPanel_2.Children.Add(PRI_CheckBox_2);
        }

        /// <summary>СОБЫТИЕ На отключение/включение элеменов фильтра через CheckBox</summary>       
        private void PART_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            PRI_DatePicker_1.IsEnabled = PRI_CheckBox_1.IsChecked == true;
            PRI_ComboBox_2.IsEnabled = PRI_CheckBox_2.IsChecked == true;
            MET_SqlFilter();
        }

        /// <summary>СОБЫТИЕ Выбор новой даты или нового отделения</summary>
        private void PART_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PRI_ComboBox_2.SelectedValue == null)
                return;
            MET_SqlFilter();
        }

        /// <summary>МЕТОД Создание фильтров для загрузки данных из SQL</summary>
        protected override void MET_SqlFilter()
        {
            PRO_SqlWhere = "where isnull(a.xDelete, 0) = 0";

            // Фильтр по дате  
            if (PRI_DatePicker_1.IsEnabled)
            {
                DateTime _Date = Convert.ToDateTime(PRI_DatePicker_1.Text);
                PRO_SqlWhere += String.Format(" and a.DN <= '{0}' and (a.DK is NULL or a.DK >= '{0}')", _Date.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
            }
            // Фильтр по отделению
            if (PRI_ComboBox_2.IsEnabled)
                PRO_SqlWhere += $" and a.Otd = {PRI_ComboBox_2.SelectedValue}";
            
            // ФИО пациента
            if (PRO_TextFilter.Length > 0)
                PRO_SqlWhere += $" and (k.FAM like '{PRO_TextFilter}%' or k.FAM like '{PRO_TextFilterTransliter}%')";
            
            // Запрос
            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);            
        }

        /// <summary>МЕТОД Выбор данных</summary>
        protected override void MET_Select()
        {
            if (!PROP_FlagButtonSelect)
                return;
            try
            {
                DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
                MyGlo.KL = Convert.ToDecimal(_DataRowView.Row["KL"]);
                MyGlo.IND = Convert.ToDecimal(_DataRowView.Row["IND"]);
                MyGlo.Otd = Convert.ToInt16(_DataRowView.Row["Otd"]);
                PROP_Return = true;
            }
            catch
            {
            }
            Close();
        }
    }
}
