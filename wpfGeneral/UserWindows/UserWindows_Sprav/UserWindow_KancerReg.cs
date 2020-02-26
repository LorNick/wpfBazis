using System;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Смена пациента в Канцер регистре (В РАБОТЕ!!!)</summary>
    public class UserWindow_KancerReg : VirtualUserWindow
    {
        private DatePicker PRI_DatePicker_1;
        private DatePicker PRI_DatePicker_2;
        private CheckBox PRI_CheckBox_1;
        private ComboBox PRI_ComboBox_2;
        private CheckBox PRI_CheckBox_2;
        private CheckBox PRI_CheckBox_3;
        private CheckBox PRI_CheckBox_4;


        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_KancerReg()
        {
            // Имя таблицы
            PRO_TableName = "Kancer";
            // Если строка ввода ищет через SQL
            PRO_FlagTextSql = true;
            // Заголовок
            Title = "Выбор нового пациента для Канцер регистра:";
            // Размеры окна
            Width = 890;
            MinWidth = Width;
            Height = 600;
            // Сортируем по Фамилии
            PRO_PoleSort = 1;
            // Разрешаем выбирать записи
            PROP_FlagButtonSelect = true;
            // Создаем фильтр
            MET_CreateFiltr();
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Создание фильтров</summary>
        private void MET_CreateFiltr()
        {
            PART_Grid.RowDefinitions[0].Height = new GridLength(80, GridUnitType.Auto);
            PART_Expander.Visibility = Visibility.Visible;
            Border _Border = new Border();
            _Border.Style = (Style)FindResource("Border_2");
            PART_Expander.Content = _Border;
            StackPanel _SPanel = new StackPanel();
            _Border.Child = _SPanel;
            // ---- Настраиваем 1й фильтр
            StackPanel _SPanel_1 = new StackPanel();
            _SPanel_1.Orientation = Orientation.Horizontal;
            _SPanel_1.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_1);
            // Текст 1
            Label _Label_1 = new Label();
            _Label_1.Content = "Выписан в диапазоне с:";
            _Label_1.Foreground = Brushes.Navy;
            _SPanel_1.Children.Add(_Label_1);
            // Дата 1
            PRI_DatePicker_1 = new DatePicker();
            PRI_DatePicker_1.SelectedDateFormat = DatePickerFormat.Short;
            PRI_DatePicker_1.Text = DateTime.Today.ToString();
            PRI_DatePicker_1.DisplayDateEnd = DateTime.Today;
            PRI_DatePicker_1.DisplayDateStart = DateTime.Parse("01/01/2007");
            PRI_DatePicker_1.SelectedDateChanged += PART_SelectionChanged;
            _SPanel_1.Children.Add(PRI_DatePicker_1);
            // Текст 2
            Label _Label_2 = new Label();
            _Label_2.Content = " по ";
            _Label_2.Foreground = Brushes.Navy;
            _SPanel_1.Children.Add(_Label_2);
            // Дата 2
            PRI_DatePicker_2 = new DatePicker();
            PRI_DatePicker_2.SelectedDateFormat = DatePickerFormat.Short;
            PRI_DatePicker_2.Text = DateTime.Today.ToString();
            PRI_DatePicker_2.DisplayDateEnd = DateTime.Today;
            PRI_DatePicker_2.DisplayDateStart = DateTime.Parse("01/01/2007");
            PRI_DatePicker_2.SelectedDateChanged += PART_SelectionChanged;
            _SPanel_1.Children.Add(PRI_DatePicker_2);
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
            // Текст 3
            Label _Label_3 = new Label();
            _Label_3.Content = "Отделение стационара:";
            _Label_3.Foreground = Brushes.Navy;
            _SPanel_2.Children.Add(_Label_3);
            // Отделения
            PRI_ComboBox_2 = new ComboBox();
            PRI_ComboBox_2.Width = 280;
            MySql.MET_DsAdapterFill(MyQuery.s_Otdel_Select_1(), "s_Otdel");
            PRI_ComboBox_2.ItemsSource = new DataView(MyGlo.DataSet.Tables["s_Otdel"]);
            PRI_ComboBox_2.DisplayMemberPath = "TKOD";
            PRI_ComboBox_2.SelectedValuePath = "KOD";
            PRI_ComboBox_2.SelectedValue = MyGlo.Otd;
            PRI_ComboBox_2.IsEnabled = false;
            PRI_ComboBox_2.SelectionChanged += PART_SelectionChanged;
            _SPanel_2.Children.Add(PRI_ComboBox_2);
            // Check Отделения
            PRI_CheckBox_2 = new CheckBox();
            PRI_CheckBox_2.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_2.Content = "учитывать фильтр по отделению";
            PRI_CheckBox_2.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_2.Foreground = Brushes.Navy;
            PRI_CheckBox_2.IsChecked = false;
            PRI_CheckBox_2.Click += PART_CheckBox_Click;
            _SPanel_2.Children.Add(PRI_CheckBox_2);
            // ---- Настраиваем 3й и 4й фильтр
            StackPanel _SPanel_3 = new StackPanel();
            _SPanel_3.Orientation = Orientation.Horizontal;
            _SPanel_3.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_3);
            // Check Отображение заполенных карт
            PRI_CheckBox_3 = new CheckBox();
            PRI_CheckBox_3.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_3.Content = "отображать только заполненные Выписные карты   ";
            PRI_CheckBox_3.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_3.Foreground = Brushes.Navy;
            PRI_CheckBox_3.IsChecked = false;
            PRI_CheckBox_3.Click += PART_CheckBox_Click;
            _SPanel_3.Children.Add(PRI_CheckBox_3);
            // Check Заполенные протокола
            PRI_CheckBox_4 = new CheckBox();
            PRI_CheckBox_4.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_4.Content = "скрывать заполенные в Канц. регистр";
            PRI_CheckBox_4.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_4.Foreground = Brushes.Navy;
            PRI_CheckBox_4.IsChecked = true;
            PRI_CheckBox_4.Click += PART_CheckBox_Click;
            _SPanel_3.Children.Add(PRI_CheckBox_4);
        }

        /// <summary>СОБЫТИЕ На отключение/включение элеменов фильтра через CheckBox</summary>       
        private void PART_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            PRI_DatePicker_1.IsEnabled = PRI_CheckBox_1.IsChecked == true;
            PRI_DatePicker_2.IsEnabled = PRI_CheckBox_1.IsChecked == true;
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

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.APSTAC_Select_4(PRO_SqlWhere);
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "", "Ном.", "ФИО пациента", "Дата р.", "Диаг.", "Поступил", "Выписан", "От.", "Карта", "Записан" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Фильтруем данные для больших таблиц</summary>
        protected override void MET_SqlFilter()
        {
            PRO_SqlWhere = "where a.DK is not NULL and isnull(a.xDelete, 0) = 0";

            // Фильтр по дате  
            if (PRI_DatePicker_1.IsEnabled)
            {
                DateTime _Date1 = Convert.ToDateTime(PRI_DatePicker_1.Text);
                DateTime _Date2 = Convert.ToDateTime(PRI_DatePicker_2.Text);
                PRO_SqlWhere += String.Format(" and a.DK between '{0}' and '{1}'", _Date1.ToString("d", CultureInfo.CreateSpecificCulture("en-US")),
                                                                                _Date2.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
            }
            // Фильтр по отделению
            if (PRI_ComboBox_2.IsEnabled)
                PRO_SqlWhere += String.Format(" and a.Otd = {0}", PRI_ComboBox_2.SelectedValue);
            // Фильтр отображать только заполненные Выписные карты
            if (PRI_CheckBox_3.IsChecked == true)
                PRO_SqlWhere += " and p.Cod is not NULL";
            // Cкрывать заполенные в Канц. регистр
            if (PRI_CheckBox_4.IsChecked == true)
                PRO_SqlWhere += " and a.OtpuskDate is NULL";

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
