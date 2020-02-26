using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Выбор Лекарственных средств стационара</summary>
    public class UserWindow_Zay_NaznachMedLS : VirtualUserWindow
    {
        // Элементы фильтра
        private ComboBox PRI_ComboBox_1;
        private CheckBox PRI_CheckBox_1;
        /// <summary>Тип справочника 0 - наименование ВрачаЛС, 1 - торговые наименования</summary>
        private readonly int PRI_Type;

        /// <summary>Код ЛС</summary>
        public int PUB_Cod;
        
        /// <summary>Наименование ЛС</summary>
        public string PUB_NameVrach = "";
        
        /// <summary>Ед. измерения</summary>
        public string PUB_BazeMeas = "";
        

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Zay_NaznachMedLS(int pType = 0)
        {
            // Тип таблицы
            PRI_Type = pType;
            // Имя таблицы
            PRO_TableName = "lnzLS";
            // Заголовок
            Title = "Выбор Лекарственного средства:";
            //Размеры
            MinWidth = Width;
            // Сортируем по полю Наименование
            PRO_PoleSort = 0;
            // Поле поиска
            PRO_PoleFiltr = "Filter";
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
            switch (PRI_Type)
            {
                case 1:
                    return MyQuery.lnzLS_Select_2(PRO_SqlWhere);                // по торговому наименванию                
                default:
                    return MyQuery.lnzLS_Select_1(PRO_SqlWhere);                // с групировкой по наименованию врача                
            }
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
            PART_DataGrid.Columns.RemoveAt(0);
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "", "", "Наименование", "Ед. измерения", "Группа" };
            return _mName[pIndex];
        }        

        /// <summary>МЕТОД Создание фильтров</summary>
        private void MET_CreateFiltr()
        {
            PART_Grid.RowDefinitions[0].Height = new GridLength(80, GridUnitType.Auto);
            PART_Expander.Visibility = Visibility.Visible;
            Border _Border = new Border { Style = (Style)FindResource("Border_2") };
            PART_Expander.Content = _Border;
            StackPanel _SPanel = new StackPanel();
            _Border.Child = _SPanel;
            // ---- Настраиваем фильтр
            StackPanel _SPanel_1 = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 2, 0, 2)
            };
            _SPanel.Children.Add(_SPanel_1);
            Label _Label_1 = new Label { Content = "Группы ЛС:", Foreground = Brushes.Navy };
            _SPanel_1.Children.Add(_Label_1);
            // Группы ЛС
            PRI_ComboBox_1 = new ComboBox { Width = 280 };
            MySql.MET_DsAdapterFill(MyQuery.lnzGroup_Select_1(), "lnzGroup");
            PRI_ComboBox_1.ItemsSource = new DataView(MyGlo.DataSet.Tables["lnzGroup"]);
            PRI_ComboBox_1.DisplayMemberPath = "Name";
            PRI_ComboBox_1.SelectedValuePath = "Cod";
            PRI_ComboBox_1.SelectionChanged += delegate { if (PRI_ComboBox_1.SelectedValue == null) return; MET_DopFilter(); };
            _SPanel_1.Children.Add(PRI_ComboBox_1);
            PRI_ComboBox_1.IsEnabled = false;
            // Check Группы ЛС
            PRI_CheckBox_1 = new CheckBox();
            PRI_CheckBox_1.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_1.Content = "учитывать фильтр по группам ЛС";
            PRI_CheckBox_1.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_1.Foreground = Brushes.Navy;
            PRI_CheckBox_1.IsChecked = false;
            PRI_CheckBox_1.Click += delegate { PRI_ComboBox_1.IsEnabled = PRI_CheckBox_1.IsChecked == true; MET_DopFilter(); };
            _SPanel_1.Children.Add(PRI_CheckBox_1);
        }

        /// <summary>МЕТОД Дополнительный фильтр по загруженным данным</summary>
        protected override void MET_DopFilter()
        {           
            // Фильтр по профилю исследования
            if (PRI_ComboBox_1.IsEnabled & PRI_ComboBox_1.SelectedValue != null & PRI_CheckBox_1.IsChecked == true)
                PRO_Where = $" Grou = {PRI_ComboBox_1.SelectedValue}";

            MET_Filter();
        }      

        /// <summary>МЕТОД Выбор данных</summary>
        protected override void MET_Select()
        {
            if (!PROP_FlagButtonSelect)
                return;

            try
            {
                DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
                PUB_Cod = Convert.ToInt16(_DataRowView.Row["Cod"]);
                PUB_NameVrach = Convert.ToString(_DataRowView.Row["NameVrach"]);
                PUB_BazeMeas = Convert.ToString(_DataRowView.Row["BazeMeas"]);
                PROP_Return = true;
            }
            catch
            {
            }
            Close();
        }
    }
}
