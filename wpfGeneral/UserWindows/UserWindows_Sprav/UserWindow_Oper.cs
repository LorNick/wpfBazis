using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Справочник Операций</summary>
    public class UserWindow_Oper : VirtualUserWindow
    {
        /// <summary>СВОЙСТВО Код операции</summary>
        public string PROP_Cod { get; private set; }

        /// <summary>СВОЙСТВО Наименование операции</summary>
        public string PROP_Text { get; private set; }

        // Номер шаблона (если есть)
        private readonly int PRI_Shablon;
        // Дата действия кодов лечения
        private readonly DateTime PRI_Date;

        // Элементы фильтра (КСГ - круглосуточный, КСГ - дневной, все записи)
        private ComboBox PRI_ComboBox_1;
        // Элемент фильтра (только для данного шаблона - тег-массив "shablon": [9910])
        private CheckBox PRI_CheckBox1;
        // Элемент фильтра (показать неактуальные услуги - неучитываем время действия)
        private CheckBox PRI_CheckBox2;

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pDate">Дата действия кодов лечения (текущая дата, либо дата создания протокола)</param>
        /// <param name="pShablon">Номер шаблона (если есть)</param>
        public UserWindow_Oper(DateTime pDate, int pShablon = 0)
        {
            // Ставим Русский язык
            MyMet.MET_Lаng();
            // Имя таблицы
            PRO_TableName = "s_VidOper";
            // Заголовок
            Title = "Справочник Услуг (Операций):";
            // Спец поле (составное) по которому производится поиск
            PRO_PoleFiltr = "Names";
            // Дата действия кодов лечения
            PRI_Date = pDate;
            // Номер шаблона (если есть)
            PRI_Shablon = pShablon;
            // Размеры окна
            Height = 660;
            Width = 840;
            MinWidth = Width;
            // Сортируем по полю Варианты ответа
            PRO_PoleSort = 0;
            // Создаем фильтр
            MET_CreateFiltr();
            // Фильтр по КСГ для загрузки из SQL
            MET_SqlFilter();
            // Открываем таблицу
            MET_OpenForm(false);
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.s_VidOper_Select_1(PRO_SqlWhere);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "", "Код", "Наименование" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
            PART_DataGrid.Columns.RemoveAt(0);
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
            _Label_1.Content = "Отображать услуги/операции:";
            _Label_1.Foreground = Brushes.Navy;
            _SPanel_1.Children.Add(_Label_1);
            //  Элементы фильтра (КСГ - круглосуточный, КСГ - дневной, все записи)
            PRI_ComboBox_1 = new ComboBox();
            PRI_ComboBox_1.Width = 300;
            Dictionary<string, string> _Dictionary  = new Dictionary<string, string> {
                { "hour", "по КСГ для круглосуточного стационара"},
                { "day", "по КСГ для дневного стационара"}, {"all", "все"} };
            PRI_ComboBox_1.ItemsSource = _Dictionary;
            PRI_ComboBox_1.DisplayMemberPath = "Value";
            PRI_ComboBox_1.SelectedValuePath = "Key";
            // Смотрим тип стационара
            if (MyGlo.TypeModul == eModul.VrStac)
            {
                if (MySql.MET_QueryInt(MyQuery.s_Department_Select_2(MyGlo.Otd)) == 1)
                    PRI_ComboBox_1.SelectedValue = "hour";  // круглосуточный
                else
                    PRI_ComboBox_1.SelectedValue = "day";   // дневной
            }
            else
                PRI_ComboBox_1.SelectedValue = "all";       // НЕ стацонар
            PRI_ComboBox_1.IsEnabled = true;
            PRI_ComboBox_1.SelectionChanged += delegate { MET_SqlFilter(); };
            _SPanel_1.Children.Add(PRI_ComboBox_1);

            // ---- Настраиваем 2й фильтр
            StackPanel _SPanel_2 = new StackPanel();
            _SPanel_2.Orientation = Orientation.Horizontal;
            _SPanel_2.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_2);
            // Отображать только для данного шаблона - тег-массив "shablon": [9910]
            PRI_CheckBox1 = new CheckBox();
            PRI_CheckBox1.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox1.Content = "отображать только для данного шаблона";
            PRI_CheckBox1.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox1.Foreground = Brushes.Navy;
            // Проверим на наличие у тега shablon значения текущего шаблона
            PRO_SqlWhere += $" and dbo.jsonIfArray(xInfo, 'shablon', '{PRI_Shablon}') = 1";
            bool _Flag = PRI_Shablon > 0 && MySql.MET_QueryBool(MyQuery.s_VidOper_Select_2(PRO_SqlWhere));
            PRI_CheckBox1.IsChecked = _Flag;
            PRI_CheckBox1.IsEnabled = _Flag;
            PRI_CheckBox1.Click += delegate { MET_SqlFilter(); };
            _SPanel_2.Children.Add(PRI_CheckBox1);

            // ---- Настраиваем 3й фильтр
            // Показать неактуальные услуги - неучитываем время действия
            PRI_CheckBox2 = new CheckBox();
            PRI_CheckBox2.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox2.Content = $"показать ещё и неактуальные услуги на дату '{PRI_Date:dd.MM.yyyy}'";
            PRI_CheckBox2.VerticalAlignment = VerticalAlignment.Center;
            PRI_CheckBox2.Foreground = Brushes.Navy;
            PRI_CheckBox2.Click += delegate { MET_SqlFilter(); };
            _SPanel_2.Children.Add(PRI_CheckBox2);
        }

        /// <summary>МЕТОД Создание фильтров для загрузки данных из SQL (Выбор фильтрации операций по КСГ)</summary>
        protected override void MET_SqlFilter()
        {
            PRO_SqlWhere = "";
            // Если показываем только актуальные услуги
            if (PRI_CheckBox2.IsChecked == false)
                PRO_SqlWhere += $" and '{PRI_Date:MM.dd.yyyy}' between xBeginDate and xEndDate";
            // Выбор круглосуточный/дневной или все КСГ
            if (PRI_ComboBox_1.SelectedValue.ToString() != "all")
                PRO_SqlWhere += $" and dbo.jsonIf(xInfo, '{PRI_ComboBox_1.SelectedValue}') = 1";
            // Только для данного шаблона
            if (PRI_CheckBox1.IsChecked == true && PRI_Shablon > 0)
                PRO_SqlWhere += $" and dbo.jsonIfArray(xInfo, 'shablon', '{PRI_Shablon}') = 1";
            // Запрос
            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);
            // Выделяем первую строку
            if (PRO_DataView != null && PRO_DataView.Table.Rows.Count != -1)
                PART_DataGrid.SelectedIndex = 0;
        }

        /// <summary>МЕТОД Создание фильтров для уже загруженных данных</summary>
        protected override void MET_Filter()
        {
            if (PRO_TextFilter.Length > 0)
            {
                // Разбиваем строку поиска на отдельные слова
                string[] _mFilter = PRO_TextFilter.Split(' ');
                // Если есть спец поле, то используем его, иначе выбранный столбик
                string _PoleSort = PRO_PoleFiltr != "" ? PRO_PoleFiltr : PRO_DataView.Sort;

                string _And = PRO_Where.Length > 0 ? " and " : "";
                try
                {
                    string _Filtr = $"{PRO_Where}{_And}";
                    int _Count = 0;
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var _f in _mFilter)
                    {
                        _Count++;
                        if (_mFilter.Length > 1 && _Count == _mFilter.Length && _f.Length == 1 && char.IsDigit(_f[0]))
                            _Filtr += $"(CountLS = {_f[0]}) and ";
                        else
                            _Filtr += $"({_PoleSort} like '%{_f}%' or {_PoleSort} like '%{PRO_Transliter.MET_Replace(_f)}%') and ";
                    }
                    _Filtr += "1 = 1";

                    PRO_DataView.RowFilter = _Filtr;
                }
                catch
                {
                    try
                    {
                        PRO_DataView.RowFilter = PRO_Where + _And + PRO_DataView.Sort + " >= " + PRO_TextFilter;
                    }
                    catch
                    {
                        try
                        {
                            PRO_DataView.RowFilter = PRO_Where;
                        }
                        catch
                        {
                            PRO_DataView.RowFilter = "";
                        }
                    }
                }
            }
            else
                try
                {
                    PRO_DataView.RowFilter = PRO_Where;
                }
                catch
                {
                    PRO_DataView.RowFilter = "";
                }
            // Выделяем первую строку
            if (PRO_DataView != null && PRO_DataView.Table.Rows.Count != -1)
                PART_DataGrid.SelectedIndex = 0;
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
                PROP_Cod = Convert.ToString(_DataRowView.Row["KOP"]);
                PROP_Text = Convert.ToString(_DataRowView.Row["NAME"]);
                PROP_Return = true;
            }
            catch
            {
                // ignored
            }
            Close();
        }
    }
}
