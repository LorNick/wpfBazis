using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Выбор шаблонов стационара</summary>
    public class UserWindow_Shablon_Stac : VirtualWindow_Shablon
    {
        private ComboBox PRI_ComboBox_1;
        private CheckBox PRI_CheckBox_1;

        /// <summary>Профиль шаблонов Первый</summary>
        private readonly int PRI_Profil_1;
        /// <summary>Профиль шаблонов Второй</summary>
        private readonly int PRI_Profil_2;
        /// <summary>Только один протокол в шаблоне</summary>
        private readonly bool PRI_OneShablon;

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Shablon_Stac(string pTitle, int pProfil_1, int pProfil_2, bool pOneShablon = false)
        {
            // Имя таблицы
            PRO_TableName = "astListShablon";
            //Размеры
            MinWidth = Width;
            // Заголовок
            Title = pTitle;
            // Профиль шаблонов
            PRI_Profil_1 = pProfil_1;
            PRI_Profil_2 = pProfil_2;
            // Только один протокол в шаблоне (по умолчанию отключен)
            PRI_OneShablon = pOneShablon;
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
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.MET_ListShablon_Select_2(PRO_SqlWhere, "ast");
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
            Label _Label_1 = new Label();
            _Label_1.Content = "Отделение стационара:";
            _Label_1.Foreground = Brushes.Navy;
            _SPanel_1.Children.Add(_Label_1);
            // Отделения
            PRI_ComboBox_1 = new ComboBox();
            PRI_ComboBox_1.Width = 280;
            MySql.MET_DsAdapterFill(MyQuery.s_Department_Select_1(" and Tip in (1, 2)"), "s_Department");
            PRI_ComboBox_1.ItemsSource = new DataView(MyGlo.DataSet.Tables["s_Department"]);
            PRI_ComboBox_1.DisplayMemberPath = "Names";
            PRI_ComboBox_1.SelectedValuePath = "Cod";
            PRI_ComboBox_1.SelectedValue = MyGlo.Otd;
            PRI_ComboBox_1.SelectionChanged += delegate { if (PRI_ComboBox_1.SelectedValue == null) return; MET_SqlFilter(); };
            _SPanel_1.Children.Add(PRI_ComboBox_1);
            // Check Отделения
            PRI_CheckBox_1 = new CheckBox();
            PRI_CheckBox_1.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_1.Content = "учитывать фильтр по отделению";
            PRI_CheckBox_1.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_1.Foreground = Brushes.Navy;
            PRI_CheckBox_1.IsChecked = true;
            PRI_CheckBox_1.Click += delegate { PRI_ComboBox_1.IsEnabled = PRI_CheckBox_1.IsChecked == true; MET_SqlFilter(); };
            _SPanel_1.Children.Add(PRI_CheckBox_1);
        }

        /// <summary>МЕТОД Создание фильтров для загрузки данных из SQL</summary>
        protected override void MET_SqlFilter()
        {
            PRO_SqlWhere = $"and TipObsled between {PRI_Profil_1} and {PRI_Profil_2}";
            // Только один протокол для шаблона
            if (PRI_OneShablon)
                PRO_SqlWhere += $" and Cod not in (select NumShablon from dbo.astProtokol where CodApstac = {MyGlo.IND} and isnull(xDelete, 0) = 0)";
            // Фильтр по отделению
            if (PRI_ComboBox_1.IsEnabled)
                PRO_SqlWhere += $" and (dbo.jsonIfArray(xInfo, 'Otdel', '{PRI_ComboBox_1.SelectedValue}') = 1 or dbo.jsonIf(xInfo, 'Otdel') = 0)";
            // Запрос
            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);
        }
    }
}
