using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Список пациентов с Лабораторными исследованиями"</summary>
    public class UserWindow_Laboratory : VirtualUserWindow
    {
        private DatePicker PRI_DatePicker_1;
        /// <summary>Дата исследования</summary>
        private string PRI_Dat;
        /// <summary>ФИО пациента</summary>
        private string PRI_Fio;
        /// <summary>Код шаблона</summary>
        private int PRI_ShablLab = 1000;
        /// <summary>Таймер для задержки перед запросом</summary>
        private DispatcherTimer PRI_Timer;
        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Laboratory()
        {
            // Ставим Русский язык
            MyMet.MET_Lаng();
            // Имя таблицы
            PRO_TableName = "Laboratory";
            // Если строка ввода ищет через SQL
            PRO_FlagTextSql = true;
            // Заголовок
            Title = "Лабораторные исследования COVID 19:";
            // Подсказка в строке поиска
            PART_TextBox.WatermarkContent = "Введите ФИО и год рождения (например: ИВАНО И НИ 28)";
            // Размеры окна
            Height = 720;
            MinWidth = Width;
            // Разрешаем выбирать записи
            PROP_FlagButtonSelect = true;
            // Создаем фильтр
            MET_CreateFiltr();
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
            // Показываем в подсказке
            PRO_PoleBarPanel = 1;
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.kbol_Select_6(PRI_ShablLab, PRI_Dat, PRI_Fio);
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "ФИО пациента", "Дата рожд.", "Кто завел", "Когда", "Направил" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[1].Width = 300;
            PART_DataGrid.Columns[3].Width = 140;
           // PART_DataGrid.Columns[4].Width = 110;
        }

        /// <summary>МЕТОД Создание фильтров</summary>
        protected void MET_CreateFiltr()
        {
            PART_Grid.RowDefinitions[0].Height = new GridLength(80, GridUnitType.Auto);
            PART_Expander.Visibility = Visibility.Visible;
            PART_Expander.IsExpanded = true;
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
            _Label_1.Content = "Дата исследования:";
            _Label_1.Foreground = Brushes.Navy;
            _SPanel_1.Children.Add(_Label_1);
            // Дата исследования
            PRI_DatePicker_1 = new DatePicker();
            PRI_DatePicker_1.SelectedDateFormat = DatePickerFormat.Short;
            PRI_DatePicker_1.Text = DateTime.Today.ToString();
            PRI_DatePicker_1.DisplayDateEnd = DateTime.Today;
            PRI_DatePicker_1.DisplayDateStart = DateTime.Parse("07/07/2020");
            PRI_DatePicker_1.SelectedDateChanged += delegate { PRI_Timer.Stop(); MET_SqlFilter(); };
            _SPanel_1.Children.Add(PRI_DatePicker_1);
            PRI_Timer = new DispatcherTimer();
            PRI_Timer.Interval = new TimeSpan(0, 0, 1);
            PRI_Timer.Tick += delegate { PRI_Timer.Stop(); MET_SqlFilter(); };
        }

        /// <summary>СОБЫТИЕ Находим данные по строке ввода Фильтра</summary>
        /// <param name="sender">Текущий объект</param>
        /// <param name="e">Генерируемая детализация</param>
        protected override void PART_TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Наше условие фильтра
            PRO_TextFilter = (sender as TextBox)?.Text;
            // Перевод строки фильтра
            PRO_TextFilterTransliter = PRO_Transliter.MET_Replace(PRO_TextFilter);
            // Смотрим есть ли спец символы, кторые бы запортили SQL запрос
            PRO_TextFilter = PRO_TextFilter.Replace("'", "''");
            PRI_Timer.Stop();
            PRI_Timer.Start();
        }

        /// <summary>МЕТОД Фильтруем данные для больших таблиц</summary>
        protected override void MET_SqlFilter()
        {
            // Учитывать дату исследования или показывать все исследования
            PRI_Dat = $"'{PRI_DatePicker_1.SelectedDate:yyyy-MM-dd}'";
            // ФИО пациента
            PRI_Fio = PRO_TextFilter;
            // Запрос
            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);
            // Выделяем первую строку
            if (PRO_DataView != null && PRO_DataView.Table.Rows.Count != -1)
                PART_DataGrid.SelectedIndex = 0;
        }

        /// <summary>МЕТОД Выбор данных</summary>
        protected override void MET_Select()
        {
            if (!PROP_FlagButtonSelect)
                return;
            // Список пациентов
            try
            {
                DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
                MyGlo.KL = Convert.ToDecimal(_DataRowView.Row["KL"]);
                PROP_Return = true;
            }
            catch
            {
            }
            Close();
        }
    }

}
