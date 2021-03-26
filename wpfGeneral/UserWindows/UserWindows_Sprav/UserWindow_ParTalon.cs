using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Талоны параклиники</summary>
    public class UserWindow_ParTalon : VirtualUserWindow
    {
        private CheckBox PRI_CheckBox1;
        private DatePicker PRI_DatePicker1;
        private CheckBox PRI_CheckBox2;

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_ParTalon()
        {
            // Имя таблицы
            PRO_TableName = "parTalon";
            // Заголовок
            Title = "Талоны Параклиники:";
            // Подсказка в строке поиска
            PART_TextBox.WatermarkContent = "Введите ФИО пациента (например: ИВАНОВ Ф И)";
            // Размеры окна
            Width = 1090;
            MinWidth = Width;
            Height = 690;
            // Если строка ввода ищет через SQL
            PRO_FlagTextSql = true;
            // Создаем фильтр
            MET_CreateFiltr();
            // Сортируем по полю Варианты ответа
            PRO_PoleSort = 1;
            // Показываем в подсказке
            PRO_PoleBarPanel = 1;
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.parTalon_Select_1(PRO_SqlWhere);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "Дата", "Пациент", "Дата р.", "Принят", "Платно", "Откуда", "Кто послал", "Куда", "Зачем" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Создание фильтров</summary>
        private void MET_CreateFiltr()
        {
            PART_Grid.RowDefinitions[0].Height = new GridLength(80, GridUnitType.Auto);
            PART_Expander.Visibility = Visibility.Visible;
            PART_Expander.IsExpanded = true;
            Border _Border = new Border();
            _Border.Style = (Style)FindResource("Border_2");
            PART_Expander.Content = _Border;
            StackPanel _SPanel = new StackPanel();
            _Border.Child = _SPanel;
            // ---- Настраиваем 1й фильтр
            StackPanel _SPanel1 = new StackPanel();
            _SPanel1.Orientation = Orientation.Horizontal;
            _SPanel1.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel1);
            Label _Label1 = new Label();
            _Label1.Content = "Дата посещения:";
            _Label1.Foreground = Brushes.Navy;
            _SPanel1.Children.Add(_Label1);
            // Дата
            PRI_DatePicker1 = new DatePicker();
            PRI_DatePicker1.SelectedDateFormat = DatePickerFormat.Short;
            PRI_DatePicker1.Text = DateTime.Today.ToString();
            PRI_DatePicker1.DisplayDateStart = DateTime.Parse("01/01/2007");
            PRI_DatePicker1.SelectedDateChanged += delegate { MET_SqlFilter(); };
            _SPanel1.Children.Add(PRI_DatePicker1);
            // Check Даты
            PRI_CheckBox1 = new CheckBox
            {
                Margin = new Thickness(10, 0, 0, 0),
                Content = "учитывать фильтр по дате",
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.Navy,
                IsChecked = true
            };
            PRI_CheckBox1.Click += delegate { PRI_DatePicker1.IsEnabled = PRI_CheckBox1.IsChecked == true; MET_SqlFilter(); };
            _SPanel1.Children.Add(PRI_CheckBox1);
            // Check Шаблоны Документов
            PRI_CheckBox2 = new CheckBox();
            PRI_CheckBox2.Margin = new Thickness(30, 0, 0, 0);
            PRI_CheckBox2.Content = "Не принят";
            PRI_CheckBox2.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox2.Foreground = Brushes.Navy;
            PRI_CheckBox2.IsChecked = true;
            PRI_CheckBox2.Click += delegate { PRI_DatePicker1.IsEnabled = PRI_CheckBox1.IsChecked == true; MET_SqlFilter(); };
            _SPanel1.Children.Add(PRI_CheckBox2);
        }

        /// <summary>МЕТОД Фильтруем данные для больших таблиц</summary>
        protected override void MET_SqlFilter()
        {
            // Корпус
            if (MyGlo.Server == 3)
                PRO_SqlWhere = "where isnull(t.xDelete, 0) = 0 and e.Corpus = 1";
            else
                PRO_SqlWhere = "where isnull(t.xDelete, 0) = 0 and e.Corpus = 2";
            // Фильтр по дате
            if (PRI_DatePicker1.IsEnabled)
            {
                DateTime _Date = Convert.ToDateTime(PRI_DatePicker1.Text);
                PRO_SqlWhere += String.Format(" and t.[Date] = '{0}'", _Date.ToString("d", CultureInfo.CreateSpecificCulture("en-US")));
            }
            // Ещё не принятые
            if (PRI_CheckBox2.IsChecked == true)
            {
                PRO_SqlWhere += " and t.Flag = 3";
            }
            // ФИО пациента
            if (PRO_TextFilter.Length > 0)
                PRO_SqlWhere += $" and (k.FAM like '{PRO_TextFilter}%' or k.FAM like '{PRO_TextFilterTransliter}%')";
            // Запрос
            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);
        }
    }
}
