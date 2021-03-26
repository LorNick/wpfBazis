using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Выбор шаблона исследования Параклиники</summary>
    public class UserWindow_Shablon_Paracl : VirtualWindow_Shablon
    {
        private ComboBox PRI_ComboBox_1;
        private CheckBox PRI_CheckBox_1;

        /// <summary>Отображать основные шаблоны = false, дополнительные шаблоны = true</summary>
        private readonly bool PRI_Flag;

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Shablon_Paracl(bool pFlag)
        {
            // Основные/дополительные шаблоны
            PRI_Flag = pFlag;
            // Имя таблицы
            PRO_TableName = "parListShablon";
            // Заголовок
            if (PRI_Flag)
                Title = "Выбор дополнительных документов:";
            else
                Title = "Выбор исследования:";
            //Размеры
            MinWidth = Width;
            // Сортируем по полю Наименование
            PRO_PoleSort = 1;
            // Поле поиска
            PRO_PoleFiltr = "Filter";
            // Разрешаем выбирать записи
            PROP_FlagButtonSelect = true;
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
            return MyQuery.MET_ListShablon_Select_2("", "par");
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
            // ---- Настраиваем фильтр
            StackPanel _SPanel_1 = new StackPanel();
            _SPanel_1.Orientation = Orientation.Horizontal;
            _SPanel_1.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_1);
            Label _Label_1 = new Label();
            _Label_1.Content = "Профиль исследования:";
            _Label_1.Foreground = Brushes.Navy;
            _SPanel_1.Children.Add(_Label_1);
            // Профиль исследования
            PRI_ComboBox_1 = new ComboBox();
            PRI_ComboBox_1.Width = 280;
            MySql.MET_DsAdapterFill(MyQuery.parElement_Select_1(), "parProfil");
            PRI_ComboBox_1.ItemsSource = new DataView(MyGlo.DataSet.Tables["parProfil"]);
            PRI_ComboBox_1.DisplayMemberPath = "Name";
            PRI_ComboBox_1.SelectedValuePath = "Cod";
            PRI_ComboBox_1.SelectedValue = MySql.MET_QueryInt(MyQuery.parElement_Select_2(MyGlo.Otd));
            PRI_ComboBox_1.SelectionChanged += delegate { if (PRI_ComboBox_1.SelectedValue == null) return; MET_DopFilter(); }; ;
            _SPanel_1.Children.Add(PRI_ComboBox_1);
            // Check Профиля исследования
            PRI_CheckBox_1 = new CheckBox();
            PRI_CheckBox_1.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_1.Content = "учитывать фильтр по профилю";
            PRI_CheckBox_1.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_1.Foreground = Brushes.Navy;
            PRI_CheckBox_1.IsChecked = true;
            PRI_CheckBox_1.Click += delegate { PRI_ComboBox_1.IsEnabled = PRI_CheckBox_1.IsChecked == true; MET_DopFilter(); };
            _SPanel_1.Children.Add(PRI_CheckBox_1);
        }

        /// <summary>МЕТОД Дополнительный фильтр по загруженным данным</summary>
        protected override void MET_DopFilter()
        {
            // Показываем только дополнительные шаблоны, либо только основные
            PRO_Where = PRI_Flag ? " TipObsled > 1" : " TipObsled = 1";
            // Фильтр по профилю исследования
            if (PRI_ComboBox_1.IsEnabled)
                PRO_Where += $" and ProfilVr = {PRI_ComboBox_1.SelectedValue}";
            MET_Filter();
        }
    }
}
