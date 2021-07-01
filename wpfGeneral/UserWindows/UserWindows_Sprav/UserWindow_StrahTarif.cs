using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Справочник связей и тарифов
    /// по Специальности, Профилю, Коду услуг, Флагу, Тарифу...</summary>
    public class UserWindow_StrahTarif : VirtualUserWindow
    {
        /// <summary>Показываем все или только актуальные строки</summary>
        private CheckBox PRI_CheckBox_1;
        /// <summary>Тип</summary>
        private RadComboBox PRI_ComboBox_1;

        /// <summary>СВОЙСТВО Код отделения</summary>
        public string PROP_Cod { get; private set; }

        /// <summary>СВОЙСТВО Наименование отделения</summary>
        public string PROP_Text { get; private set; }

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_StrahTarif()
        {
            // Имя таблицы
            PRO_TableName = "StrahStacSv";
            // Заголовок
            Title = "Справочник связей и тарифов (для реестров):";
            Width = 1240;
            Height = 750;
            // Подсказка в строке поиска
            PART_TextBox.WatermarkContent = "Введите части слов через пробел (для полей Код услуг, Вид мед. вм., Флаг и Описание)";
            //Размеры
            MinWidth = Width;
            // Поле поиска
            PRO_PoleFiltr = "Filter";
            // Сортируем по полю Filter
            PRO_PoleSort = 0;
            // Создаем фильтр
            MET_CreateFiltr();
            // Открываем таблицу
            MET_OpenForm();
            // Фильтр
            MET_DopFilter();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.StrahTarif_Select_1();
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "Код", "Спец-ть", "Профиль", "Проф. Кой", "Код услуг", "Вид мед. вм.", "Флаг", "VidPom", "Описание", "Тариф", "Дети", "Нача. дата", "Кон. дата" };
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
            PART_DataGrid.Columns[1].Width = 40;        // Код
            PART_DataGrid.Columns[5].Width = 100;       // Код услуг
            PART_DataGrid.Columns[6].Width = 110;       // Вид мед. вм.
            PART_DataGrid.Columns[7].Width = 150;       // Флаг
            PART_DataGrid.Columns[9].Width = 220;       // Описание
            PART_DataGrid.Columns[10].Width = 80;       // Тариф
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
            StackPanel _SPanel_1 = new StackPanel();
            _SPanel_1.Orientation = Orientation.Horizontal;
            _SPanel_1.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_1);
            // Актульные данные Check
            PRI_CheckBox_1 = new CheckBox();
            PRI_CheckBox_1.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_1.Content = "Актульные данные";
            PRI_CheckBox_1.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_1.Foreground = Brushes.Navy;
            PRI_CheckBox_1.IsChecked = true;
            PRI_CheckBox_1.Click += delegate { MET_DopFilter(); };
            _SPanel_1.Children.Add(PRI_CheckBox_1);
            // Фильтр по типу
            List<string> _List = new List<string>
            {
                "0 - кр.стационар", "1 - дн.стационар", "2 - раз.поликл.", "8 - обр.пол.перв", "9 - обр.пол.повт", "7 - ВМП",
                "10 - P1-МРТ",  "10 - P2-КТ",  "10 - P3-ЭХОЭКГ",  "10 - P4-Эндоскопия",  "11 - P5-Гистология",  "11 - P6-Молек. гинет.",  "12 - Телемедицина"
            };
            PRI_ComboBox_1 = new RadComboBox();
            PRI_ComboBox_1.Margin = new Thickness(10, 0, 0, 0);
            PRI_ComboBox_1.Width = 180;
            PRI_ComboBox_1.ItemsSource = _List;
            PRI_ComboBox_1.AllowMultipleSelection = true;
            PRI_ComboBox_1.SelectionChanged += delegate { MET_DopFilter(); };
            _SPanel_1.Children.Add(PRI_ComboBox_1);
        }

        /// <summary>СОБЫТИЕ Выбор корпуса</summary>
        protected override void MET_DopFilter()
        {
            PRO_Where = "";
            if (PRI_CheckBox_1.IsChecked == true)
                PRO_Where = $"DateN <= '{ DateTime.Today}' and DateK >= '{ DateTime.Today}'";
            if (PRI_ComboBox_1.SelectedItem != null)
            {
                string _str = "";
                foreach (string s in PRI_ComboBox_1.SelectedItems)
                {
                    _str += (_str.Length > 0 ? ", '" : "'") + s + "'";
                }
                PRO_Where += PRO_Where.Length > 0 ? " and " : "";
                PRO_Where += $"Flag in ({_str})";
            }
            MET_Filter();
        }
    }
}
