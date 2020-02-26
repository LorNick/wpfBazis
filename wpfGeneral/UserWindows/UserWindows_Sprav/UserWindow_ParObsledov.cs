using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using wpfStatic;
using wpfGeneral.UserControls;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Смена пациента в Параклинике "parObsledov"</summary>
    public class UserWindow_ParObsledov : VirtualUserWindow
    {
        /// <summary>Начальная дата обследования</summary>
        private UserPole_Data PRI_BeginDate;
        /// <summary>Конечная дата обследования</summary>
        private UserPole_Data PRI_EndDate;

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_ParObsledov()
        {
            // Имя таблицы
            PRO_TableName = "parObsledov";
            // Если строка ввода ищет через SQL
            PRO_FlagTextSql = true;
            // Заголовок
            Title = "Выбор ранее обследованного пациента:";
            // Подсказка в строке поиска
            PART_TextBox.WatermarkContent = "Введите ФИО пациента (например: ИВАНОВ Ф И)";
            if (UserWindow_Log.MET_Access()) // Пояснение только для админов
            {
                // Подсказка в строке поиска
                PART_TextBox.WatermarkContent = "Введите ФИО пациента (например: ИВАНОВ Ф И) или Код обследования или KL";
            }
            // Размеры окна
            Width = 1000;
            MinWidth = Width;
            Height = 600;
            // Сортируем по Фамилии
            PRO_PoleSort = 0;
            // Показываем в подсказке
            PRO_PoleBarPanel = 2;
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
            return MyQuery.parObsledovt_Select_1(PRO_SqlWhere);
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "", "ФИО пациента", "Дата р.", "Обследован", "Исследование" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[2].Width = 280;        // ФИО  
            PART_DataGrid.Columns[3].Width = 85;         // Дата р.
        }

        /// <summary>МЕТОД Создание фильтров</summary>
        private void MET_CreateFiltr()
        {
            // Настраиваем панель фильтра
            PART_Grid.RowDefinitions[0].Height = new GridLength(80, GridUnitType.Auto);
            PART_Expander.Visibility = Visibility.Visible;
            Border _Border = new Border();
            _Border.Style = (Style)FindResource("Border_2");
            PART_Expander.Content = _Border;
            PART_Expander.IsExpanded = true;
            StackPanel _SPanel = new StackPanel();
            _Border.Child = _SPanel;
            // ---- Настраиваем фильтр
            StackPanel _SPanel_1 = new StackPanel();
            _SPanel_1.Orientation = Orientation.Horizontal;
            _SPanel_1.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_1);

            // Начальная дата обследования
            PRI_BeginDate = new UserPole_Data();
            PRI_BeginDate.PROP_WidthText = 100;

            DateTime _ToDay = DateTime.Today;           
            var _StartDate = new DateTime(_ToDay.Year, 1, 1);
            var _EendDate = DateTime.Today;

            PRI_BeginDate.PROP_Date = _StartDate;
            PRI_BeginDate.PROP_Description = "За период с";
            PRI_BeginDate.MET_Changed = () => MET_SqlFilter();
            _SPanel_1.Children.Add(PRI_BeginDate);

            // Конечная дата обследования
            PRI_EndDate = new UserPole_Data();
            PRI_EndDate.PROP_WidthText = 100;
            PRI_EndDate.PROP_Date = _EendDate;
            PRI_EndDate.PROP_ValueMaxDate = _EendDate;
            PRI_EndDate.MET_Changed = () => MET_SqlFilter();
            PRI_EndDate.PROP_Description = "по";
            _SPanel_1.Children.Add(PRI_EndDate);

        }

        /// <summary>МЕТОД Фильтруем данные для больших таблиц</summary>
        protected override void MET_SqlFilter()
        {
            PRO_SqlWhere = "where isnull(o.xDelete, 0) = 0";

            // Фильтр по дате
            PRO_SqlWhere = $" and o.DP between '{PRI_BeginDate.PROP_Date:MM.dd.yyyy}' and '{PRI_EndDate.PROP_Date:MM.dd.yyyy}'";

            // Фильтр по строке поиска
            if (PRO_TextFilter.Length > 0)
            {
                // Проверка на число
                if (decimal.TryParse(PRO_TextFilter, out decimal _Cod))
                {
                    // Код обследования или KL пациента
                    PRO_SqlWhere += $" and (o.Cod = {PRO_TextFilter} or k.KL = {PRO_TextFilterTransliter})";
                }
                else 
                {
                    // ФИО пациента
                    PRO_SqlWhere += $" and (k.FAM like '{PRO_TextFilter}%' or k.FAM like '{PRO_TextFilterTransliter}%')";
                }
            }
            
            // Запрос
            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);            
        }

        /// <summary>МЕТОД Выбор данных</summary>
        protected override void MET_Select()
        {
            if (!PROP_FlagButtonSelect)
                return;
            try
            {
                DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
                MyGlo.KL = Convert.ToDecimal(_DataRowView.Row["KL"]);
                MyGlo.IND = Convert.ToDecimal(_DataRowView.Row["Cod"]);
                PROP_Return = true;
            }
            catch
            {
            }
            Close();
        }
    }
}
