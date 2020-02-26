using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Справочник Отделений стационара</summary>
    public class UserWindow_Department : VirtualUserWindow
    {
        private CheckBox PRI_CheckBox_1;
        private ComboBox PRI_ComboBox_1;

        /// <summary>СВОЙСТВО Код отделения</summary>
        public string PROP_Cod { get; private set; }  
        
        /// <summary>СВОЙСТВО Наименование отделения</summary>
        public string PROP_Text { get; private set; }
        
        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Department()
        {
            // Имя таблицы
            PRO_TableName = "s_Department";
            // Заголовок
            Title = "Справочник Подразделений:";
            //Размеры
            MinWidth = Width;
            // Поле поиска
            PRO_PoleFiltr = "Filtr";
            // Сортируем по полю Код подразделения
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
            return MyQuery.s_Department_Select_1();
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "Код", "Наименование", "Корпус" };
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
            PART_DataGrid.Columns[1].Width = 50;        // Код
            PART_DataGrid.Columns[2].Width = 500;       // Наименование
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
            _Label_1.Content = "Корпус:";
            _Label_1.Foreground = Brushes.Navy;
            _SPanel_1.Children.Add(_Label_1);
            // Корпус
            PRI_ComboBox_1 = new ComboBox();
            PRI_ComboBox_1.Width = 120;
            PRI_ComboBox_1.ItemsSource = new string[] {"главный", "филиал"};
            PRI_ComboBox_1.SelectedValue = MyGlo.Korpus == 1 ? "главный" : "филиал";
            PRI_ComboBox_1.IsEnabled = true;
            PRI_ComboBox_1.SelectionChanged += delegate { MET_DopFilter();};
            _SPanel_1.Children.Add(PRI_ComboBox_1);
            // Check Отделения
            PRI_CheckBox_1 = new CheckBox();
            PRI_CheckBox_1.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_1.Content = "учитывать фильтр по корпусу";
            PRI_CheckBox_1.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_1.Foreground = Brushes.Navy;
            PRI_CheckBox_1.IsChecked = true;
            PRI_CheckBox_1.Click += delegate { PRI_ComboBox_1.IsEnabled = PRI_CheckBox_1.IsChecked == true; MET_DopFilter(); };
            _SPanel_1.Children.Add(PRI_CheckBox_1);
        }       

        /// <summary>МЕТОД Дополнительный фильтр по загруженным данным</summary>
        protected override void MET_DopFilter()
        {
            if (PRI_ComboBox_1.IsEnabled)
                PRO_Where = $"Korpus = '{PRI_ComboBox_1.SelectedValue}'";
            else
                PRO_Where = "";

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
                PROP_Cod = Convert.ToString(_DataRowView.Row["Cod"]);
                PROP_Text = Convert.ToString(_DataRowView.Row["Names"]);
                PROP_Return = true;
            }
            catch
            {
            }
            Close();
        }
    }
}
