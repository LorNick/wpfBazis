using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Выбор шаблонов Поликлиники</summary>
    public class UserWindow_Shablon_Policl : VirtualWindow_Shablon
    {  
        private CheckBox PRI_CheckBox_1;
        private CheckBox PRI_CheckBox_2;

        private readonly int PRI_Profil;


        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Shablon_Policl(string pTitle, int pProfil)
        {
            // Имя таблицы
            PRO_TableName = "apaNListShablon";
            // Заголовок
            Title = pTitle;
            //Размеры
            MinWidth = Width;
            // Профиль шаблонов
            PRI_Profil = pProfil;
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
            return MyQuery.MET_ListShablon_Select_2(PRO_SqlWhere, "apaN");
        }
        
        /// <summary>МЕТОД Создание фильтров</summary>
        private void MET_CreateFiltr()
        {
            PART_Grid.RowDefinitions[0].Height = new GridLength(80, GridUnitType.Auto);
            PART_Expander.Visibility = Visibility.Visible;            
            PART_Expander.IsExpanded = true;    // Показываем строку Фильтра
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
            // Check Профильные шаблоны
            PRI_CheckBox_1 = new CheckBox();
            PRI_CheckBox_1.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_1.Content = "Только Профильные шаблоны";
            PRI_CheckBox_1.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_1.Foreground = Brushes.Navy;
            PRI_CheckBox_1.IsChecked = true;
            PRI_CheckBox_1.Click += delegate { MET_DopFilter(); };
            _SPanel_1.Children.Add(PRI_CheckBox_1);
            // Check Шаблоны Документов
            PRI_CheckBox_2 = new CheckBox();
            PRI_CheckBox_2.Margin = new Thickness(30, 0, 0, 0);
            PRI_CheckBox_2.Content = "Показать шаблоны Документов";
            PRI_CheckBox_2.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox_2.Foreground = Brushes.Navy;
            PRI_CheckBox_2.IsChecked = MySql.MET_QueryBool(MyQuery.MET_ListShablon_Select_1());   // если нету профильных шаблонов, открываем дополнительные документы 
            PRI_CheckBox_2.Click += delegate { MET_DopFilter(); };
            _SPanel_1.Children.Add(PRI_CheckBox_2);
        }           

        /// <summary>МЕТОД Создание фильтров для загрузки данных из SQL</summary>
        protected override void MET_SqlFilter()
        {
            // Запрещаем повторный выбор уже введеных шаблонов
            PRO_SqlWhere = $" and (Cod not in (select NumShablon from dbo.apaNProtokol where CodApstac = {MyGlo.IND} and isnull(xDelete, 0) = 0) or Cod >= 20000)";
            
            // Запрос
            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);
        }

        /// <summary>МЕТОД Дополнительный фильтр по загруженным данным</summary>
        protected override void MET_DopFilter()
        {
            // По профилю
            PRO_Where = PRI_CheckBox_1.IsChecked == true ?  $" ProfilVr = {PRI_Profil} or ProfilVr >= 40" : " 1=1";
            
            // Только Осмотр
            if (PRI_CheckBox_2.IsChecked == false)
                PRO_Where += " and TipObsled = 1";            
            
            MET_Filter();            
        }
    }
}
