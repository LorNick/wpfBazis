using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Справочник связей и тарифов 
    /// по Специальности, Профилю, Коду услуг, Флагу, Тарифу...</summary>
    public class UserWindow_StrahTarif : VirtualUserWindow
    {
        /// <summary>Дата на которую актуальны строки</summary>
        private DatePicker PRI_DatePicker_1;
        /// <summary>Показываем все или только актуальные строки</summary>
        private CheckBox PRI_CheckBox_1;

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
            // Сортируем по полю Код
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
            PART_DataGrid.Columns[5].Width = 150;       // Код услуг
            PART_DataGrid.Columns[6].Width = 110;       // Вид мед. вм.
            PART_DataGrid.Columns[7].Width = 110;       // Флаг
            PART_DataGrid.Columns[9].Width = 220;       // Описание
            PART_DataGrid.Columns[10].Width = 80;       // Тариф
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
            _Label_1.Content = "Актуальная дата:";
            _Label_1.Foreground = Brushes.Navy;
            _SPanel_1.Children.Add(_Label_1);
            // Актуальная дата
            PRI_DatePicker_1 = new DatePicker();
            PRI_DatePicker_1.SelectedDateFormat = DatePickerFormat.Short;
            PRI_DatePicker_1.Text = DateTime.Today.ToString();
            PRI_DatePicker_1.DisplayDateEnd = DateTime.Today;
            PRI_DatePicker_1.DisplayDateStart = DateTime.Parse("01/01/2013");
            PRI_DatePicker_1.IsEnabled = true;
            PRI_DatePicker_1.SelectedDateChanged += delegate { MET_DopFilter(); }; 
            _SPanel_1.Children.Add(PRI_DatePicker_1);            
            // Check
            PRI_CheckBox_1 = new CheckBox();
            PRI_CheckBox_1.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_1.Content = "учитывать фильтр по актульности";
            PRI_CheckBox_1.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_1.Foreground = Brushes.Navy;
            PRI_CheckBox_1.IsChecked = true;
            PRI_CheckBox_1.Click += delegate { PRI_DatePicker_1.IsEnabled = PRI_CheckBox_1.IsChecked == true; MET_DopFilter(); };
            _SPanel_1.Children.Add(PRI_CheckBox_1);
        }

        /// <summary>СОБЫТИЕ Выбор корпуса</summary>
        protected override void MET_DopFilter()
        {
            if (PRI_DatePicker_1.IsEnabled)
                PRO_Where = $"DateN <= '{PRI_DatePicker_1.DisplayDate}' and DateK >= '{PRI_DatePicker_1.DisplayDate}'";
            else
                PRO_Where = "";
            MET_Filter();
        }       
    }
}
