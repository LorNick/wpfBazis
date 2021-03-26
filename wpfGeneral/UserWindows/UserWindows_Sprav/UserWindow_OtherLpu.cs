using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Список направлений в другие ЛПУ "OtherLpu"</summary>
    public class UserWindow_OtherLpu: VirtualUserWindow
    {
        private DatePicker PRI_DatePicker_1;
        private CheckBox PRI_CheckBox_1;

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_OtherLpu()
        {
            // Ставим Русский язык
            MyMet.MET_Lаng();

            // Имя таблицы
            PRO_TableName = "OtherLpu";
            // Если строка ввода ищет через SQL
            PRO_FlagTextSql = true;
            // Заголовок
            Title = "Выбор направленных пациентов из БУЗОО КОД:";
            // Подсказка в строке поиска
            PART_TextBox.WatermarkContent = "Введите ФИО пациента (например: ИВАНОВ Ф И)";
            // Размеры окна
            Height = 660;
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
            return MyQuery.kbol_Select_5(PRO_SqlWhere, MyGlo.Lpu, MyGlo.Otd);
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "ФИО пациента", "Дата рожд.", "Направил", "Когда", "Выписка" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[1].Width = 300;
            PART_DataGrid.Columns[3].Width = 140;
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
            _Label_1.Content = "Направление созднанные после:";
            _Label_1.Foreground = Brushes.Navy;
            _SPanel_1.Children.Add(_Label_1);
            // Дата
            PRI_DatePicker_1 = new DatePicker();
            PRI_DatePicker_1.SelectedDateFormat = DatePickerFormat.Short;
            PRI_DatePicker_1.Text = "01/01/2019";
            PRI_DatePicker_1.DisplayDateEnd = DateTime.Today;
            PRI_DatePicker_1.DisplayDateStart = DateTime.Parse("01/01/2019");
            PRI_DatePicker_1.SelectedDateChanged += delegate { MET_SqlFilter(); };
            _SPanel_1.Children.Add(PRI_DatePicker_1);
            // Check Даты
            PRI_CheckBox_1 = new CheckBox();
            PRI_CheckBox_1.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_1.Content = "показыть направления без выписки";
            PRI_CheckBox_1.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_1.Foreground = Brushes.Navy;
            PRI_CheckBox_1.IsChecked = false;
            PRI_CheckBox_1.Click += delegate { MET_SqlFilter(); };
            _SPanel_1.Children.Add(PRI_CheckBox_1);
        }

        /// <summary>МЕТОД Фильтруем данные для больших таблиц</summary>
        protected override void MET_SqlFilter()
        {
            PRO_SqlWhere = $"\nwhere isnull(k.xDelete, 0) = 0 and p.pDate >= '{PRI_DatePicker_1.SelectedDate:yyyy-MM-dd}'";
            // Фильтр по наличию выписки
            if (PRI_CheckBox_1.IsChecked == true)
                PRO_SqlWhere += " and iif(p3.pDate > p.pDate, p3.pDate, null) is null";
            // ФИО пациента
            if (PRO_TextFilter.Length > 0)
                PRO_SqlWhere += $" and (k.FAM like '{PRO_TextFilter}%' or k.FAM like '{PRO_TextFilterTransliter}%')";
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
