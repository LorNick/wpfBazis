using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfGeneral.UserModul;
using wpfGeneral.UserStruct;
using Excel = Microsoft.Office.Interop.Excel;
using wpfStatic;
using Tel = Telerik.Windows.Controls;
using wpfGeneral.UserControls;

namespace wpfGeneral.UserWindows
{

    /// <summary>КЛАСС Редактор шаблонов</summary>
    public class UserWindow_EditShablon: VirtualUserWindow
    {
        /// <summary>Выбор Типа шаблона</summary>
        private ComboBox PRI_ComboBox_1;
        /// <summary>Кнопка выгрузки шаблона в Excel</summary>
        private Tel.RadButton PRI_ButtonToExcel_1;
        /// <summary>Кнопка загрузки шаблона из Excel</summary>
        private Tel.RadButton PRI_ButtonFromExcel_1;
        /// <summary>Загружать шаблон сразу в SQL</summary>
        private CheckBox PRI_CheckBox_1;
        /// <summary>Кнопка загрузки шаблона на SQL филиала</summary>
        private Tel.RadButton PRI_ButtonFromToFilialSQL_1;
        /// <summary>Начальная дата создания протоколов</summary>
        private Tel.RadDateTimePicker PRI_DatePicker_1;
        /// <summary>Конечная дата создания протоколов</summary>
        private Tel.RadDateTimePicker PRI_DatePicker_2;
        /// <summary>Код пользователя</summary>
        private UserPole_Text PRI_UserCod;
        /// <summary>Не показывать нулевые протоколы</summary>
        private CheckBox PRI_CheckBox_2;

        private List<UserShablon> PRI_Shablons;

        [DllImport("User32.dll")]
        private static extern IntPtr SetForegroundWindow(IntPtr hWnd);          // Активирует окно процесса
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int cmdShow);      // Отображает данное окно впереди, даже если было свернуто
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_EditShablon()
        {
            // Имя таблицы
            PRO_TableName = "EditListShablon";
            // Заголовок
            Title = "Редактор шаблонов:";
            // Размеры окна
            Width = 1000;
            MinWidth = Width;
            Height = 700;
            // Сортируем по Коду шаблона
            PRO_PoleSort = 0;
            // Спец поле (составное) по которому производится поиск
            PRO_PoleFiltr = "Names";
            // Разрешаем выбирать записи
            PROP_FlagButtonSelect = true;

            // Создаем фильтр
            MET_CreateFiltr();
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
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
            _Label_1.Content = "Тип шаблонов :";
            _Label_1.Foreground = Brushes.Navy;
            _SPanel_1.Children.Add(_Label_1);
            // Тип шаблонов
            PRI_ComboBox_1 = new ComboBox
            {
                Width = 80,
                ItemsSource = new[] { "apaN", "ast", "par", "kdl" },
                SelectedValue = "apaN"
            };
            PRI_ComboBox_1.SelectionChanged += PART_SelectionChanged;
            _SPanel_1.Children.Add(PRI_ComboBox_1);
            // Кнопка "Выгрузка шаблона в Excel"
            PRI_ButtonToExcel_1 = new Tel.RadButton
            {
                Content = "Выгрузка в Excel",
                Margin = new Thickness(10, 0, 0, 0),
                ToolTip = "Выгрузка шаблона в Excel"
            };
            PRI_ButtonToExcel_1.Click += PRI_ButtonToExcel_1_Click;
            _SPanel_1.Children.Add(PRI_ButtonToExcel_1);
            // Кнопка "Загрузка шаблона из Excel"
            PRI_ButtonFromExcel_1 = new Tel.RadButton
            {
                Content = "Загрузка из Excel",
                Margin = new Thickness(10, 0, 0, 0),
                ToolTip = "Загрузка шаблона из Excel"
            };
            PRI_ButtonFromExcel_1.Click += PRI_ButtonFromExcel_1_Click;
            _SPanel_1.Children.Add(PRI_ButtonFromExcel_1);
            // Флаг загрузки шаблона сразу в SQL
            PRI_CheckBox_1 = new CheckBox
            {
                Margin = new Thickness(10, 0, 0, 0),
                Content = "загружать в SQL",
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = Brushes.Navy,
                IsChecked = false
            };
            _SPanel_1.Children.Add(PRI_CheckBox_1);
            // Кнопка "Загрузки шаблона в SQL филиала"
            PRI_ButtonFromToFilialSQL_1 = new Tel.RadButton
            {
                Content = "В SQL филиала",
                Margin = new Thickness(10, 0, 0, 0),
                ToolTip = "Загрузка шаблона в SQL филиала"
            };
            PRI_ButtonFromToFilialSQL_1.Click += PRI_ButtonFromToFilialSQL_1_Click;
            _SPanel_1.Children.Add(PRI_ButtonFromToFilialSQL_1);

            // ---- Настраиваем 2й фильтр
            StackPanel _SPanel_2 = new StackPanel();
            _SPanel_2.Orientation = Orientation.Horizontal;
            _SPanel_2.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_2);
            Label _Label_2 = new Label();
            _Label_2.Content = "Протоколы от:";
            _Label_2.Foreground = Brushes.Navy;
            _SPanel_2.Children.Add(_Label_2);
            // Дата с
            PRI_DatePicker_1 = new Tel.RadDateTimePicker();
            PRI_DatePicker_1.SelectedValue = DateTime.Parse("01/01/2008");
            PRI_DatePicker_1.SelectionChanged += delegate { MET_SqlFilter(); };           
            _SPanel_2.Children.Add(PRI_DatePicker_1);
            Label _Label_3 = new Label();
            _Label_3.Content = " по :";
            _Label_3.Foreground = Brushes.Navy;
            _SPanel_2.Children.Add(_Label_3);
            // Дата по
            PRI_DatePicker_2 = new Tel.RadDateTimePicker();           
            PRI_DatePicker_2.SelectedValue = DateTime.Parse("01/01/2222");
            PRI_DatePicker_2.SelectionChanged += delegate { MET_SqlFilter(); };
            _SPanel_2.Children.Add(PRI_DatePicker_2);

            // UserCod
            PRI_UserCod = new UserPole_Text();
            PRI_UserCod.PROP_MinWidthDescription = 90;
            PRI_UserCod.PROP_WidthText = 100;
            PRI_UserCod.PROP_Description = "UserCod";
            _SPanel_2.Children.Add(PRI_UserCod);

            // Скрыть нулевые протоколы
            PRI_CheckBox_2 = new CheckBox();
            PRI_CheckBox_2.Margin = new Thickness(10, 0, 0, 0);
            PRI_CheckBox_2.Content = $"скрыть нулевые протоколы";
            PRI_CheckBox_2.VerticalAlignment = VerticalAlignment.Center;           
            PRI_CheckBox_2.Click += delegate { MET_SqlFilter(); };
            _SPanel_2.Children.Add(PRI_CheckBox_2);
        }
        
        /// <summary>СОБЫТИЕ Нажали на кнопку "Выгрузка шаблона в Excel"</summary>
        protected virtual void PRI_ButtonToExcel_1_Click(object sender, RoutedEventArgs e)
        {
            DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
            int _CodShablon = Convert.ToInt16(_DataRowView.Row["Cod"]);
            string _TipDoc = PRI_ComboBox_1.SelectedValue.ToString();
            eTipDocum _eTip = eTipDocum.Null;
            switch (_TipDoc)
            {
                case "apaN":
                    _eTip = eTipDocum.Pol;
                    break;
                case "ast":
                    _eTip = eTipDocum.Stac;
                    break;
                case "par":
                    _eTip = eTipDocum.Paracl;
                    break;
                case "kdl":
                    _eTip = eTipDocum.Kdl;
                    break;
            }

            string _NameFile = $"{_TipDoc}_{_CodShablon}";
            string _PathFile = $@"C:\Shablons\{_NameFile}.xlsx";
            
            Excel.Workbook _WorkBook;
            Excel.Application _ExcelApp;

            Process[] _Processes = Process.GetProcesses();
            // Выбираем только наши
            IEnumerable<Process> _ProcsBazis = _Processes.Where(p => p.ProcessName == "EXCEL" && p.MainWindowTitle == $"{_NameFile}.xlsx - Excel");
            foreach (var _Process in _ProcsBazis)
            {
                // Активирует окно процесса
                SetForegroundWindow(_Process.MainWindowHandle);
                // Отображает данное окно на перед, даже если было свернуто
                ShowWindow(_Process.MainWindowHandle, 1);
                return;
            }
         
            _ExcelApp = new Excel.Application();

            bool _FileNew;
            FileInfo _FileInfo = new FileInfo(_PathFile);
            // Если нашли файл с текущим шаблоном
            if (_FileInfo.Exists)
            {
                _ExcelApp.Workbooks.Open(_PathFile);
                _FileNew = false;
            }
            else
            {
                _ExcelApp.SheetsInNewWorkbook = 1;
                _ExcelApp.Workbooks.Add(Type.Missing);
                _FileNew = true;
            }
            
            PRI_Shablons = UserShablon.MET_FactoryListShablon(_eTip, _CodShablon);
            
            _WorkBook = _ExcelApp.Workbooks[1];
            _WorkBook.Saved = true;
            Excel.Worksheet _Sheet = (Excel.Worksheet) _WorkBook.Worksheets.Item[1];
            
            // Рисуем заголовки
            int _y = 1;
            int _x = 1;
            _Sheet.Cells[_y, _x++].Value = "Cod";
            _Sheet.Columns[_x].ColumnWidth = 6;
            _Sheet.Cells[_y, _x++].Value = "ID";
            _Sheet.Columns[_x].ColumnWidth = 7;
            _Sheet.Cells[_y, _x++].Value = "Nomer";
            _Sheet.Columns[_x].ColumnWidth = 5;
            _Sheet.Cells[_y, _x++].Value = "VarId";
            _Sheet.Columns[_x].ColumnWidth = 6;
            _Sheet.Cells[_y, _x++].Value = "Maska";
            _Sheet.Columns[_x].ColumnWidth = 5;
            _Sheet.Cells[_y, _x++].Value = "Type";
            _Sheet.Columns[_x].ColumnWidth = 20;
            _Sheet.Cells[_y, _x++].Value = "Razdel";
            _Sheet.Columns[_x].ColumnWidth = 24;
            _Sheet.Cells[_y, _x++].Value = "Name";
            _Sheet.Columns[_x].ColumnWidth = 28;
            _Sheet.Cells[_y, _x++].Value = "ValueStart";
            _Sheet.Columns[_x].ColumnWidth = 19;
            _Sheet.Cells[_y, _x++].Value = "OutText";
            _Sheet.Cells[_y, _x++].Value = "InText";
            _Sheet.Columns[_x].ColumnWidth = 27;
            _Sheet.Cells[_y, _x++].Value = "xFormat";
            _Sheet.Columns[_x].ColumnWidth = 45;
            _Sheet.Cells[_y, _x++].Value = "xLua";
            _Sheet.Columns[_x].ColumnWidth = 22;
            _Sheet.Cells[_y, _x].Value = "xInfo";
            
            // Заполняем данные
            _y = 2;
            foreach (var _Shablon in PRI_Shablons)
            {
                _x = 1;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_Cod;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_ID;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_Nomer;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_VarId;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_Maska;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_Type;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_Razdel;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_Name;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_ValueStart;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_OutText;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_InText;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_xFormat;
                _Sheet.Cells[_y, _x++].Value = _Shablon.PROP_xLua;
                _Sheet.Cells[_y, _x].Value = _Shablon.PROP_xInfo;
                if(_Shablon.PROP_xLua != "")
                    _Sheet.Cells[_y, _x].Rows.RowHeight = 100;
                _y++;
            }

            _Sheet.Name = DateTime.Now.ToString().Replace(':','.');
            _Sheet.Copy(Before: _WorkBook.Worksheets[1]);

            _Sheet = (Excel.Worksheet)_WorkBook.Worksheets.Item[1];
            _Sheet.Name = "Текущий";

            _ExcelApp.DisplayAlerts = true;

            if (_FileNew)
                _WorkBook.SaveAs(_PathFile);
            else
                _WorkBook.Save();

            _WorkBook.Activate();
            IntPtr _Handler1 = FindWindow(null, _ExcelApp.Caption);
            SetForegroundWindow(_Handler1);
            _ExcelApp.Visible = true;
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Загрузка шаблона из Excel"</summary>
        protected virtual void PRI_ButtonFromExcel_1_Click(object sender, RoutedEventArgs e)
        {
            DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
            int _CodShablon = Convert.ToInt16(_DataRowView.Row["Cod"]);
            string _TipDoc = PRI_ComboBox_1.SelectedValue.ToString();
            eTipDocum _eTip = eTipDocum.Null;
            switch (_TipDoc)
            {
                case "apaN":
                    _eTip = eTipDocum.Pol;
                    break;
                case "ast":
                    _eTip = eTipDocum.Stac;
                    break;
                case "par":
                    _eTip = eTipDocum.Paracl;
                    break;
                case "kdl":
                    _eTip = eTipDocum.Kdl;
                    break;
            }

            var _TipProtokol = new MyTipProtokol(_eTip);
            string _NameFile = $"{_TipDoc}_{_CodShablon}";
            string _PathFile = $@"C:\Shablons\{_NameFile}.xlsx";

            Excel.Workbook _WorkBook;
            Excel.Application _ExcelApp;

            _ExcelApp = new Excel.Application();
            
            FileInfo _FileInfo = new FileInfo(_PathFile);
            // Если нашли файл с текущим шаблоном
            if (!_FileInfo.Exists)
            {
                MessageBox.Show($"Не найден файл: {_PathFile}", "Алё, чо грузить то?!");
                return;
            }
            
            _WorkBook = _ExcelApp.Workbooks.Open(_PathFile);
            Excel.Worksheet _Sheet = (Excel.Worksheet)_WorkBook.Worksheets.Item[1];
            
            // Заполняем данные
            List<UserShablon> _ListShablons = new List<UserShablon>();
            int _y = 2;
            int _x = 1;
            try
            {
                while (_Sheet.Cells[_y, 1].Value2 is double)
                {
                    _x = 1;
                    UserShablon _Shablon = new UserShablon();
                    _Shablon.PROP_Cod = (int)_Sheet.Cells[_y, _x++].Value2;
                    _Shablon.PROP_ID = (int)_Sheet.Cells[_y, _x++].Value2;
                    _Shablon.PROP_Nomer = (byte)_Sheet.Cells[_y, _x++].Value2;
                    _Shablon.PROP_VarId = (int)_Sheet.Cells[_y, _x++].Value2;
                    _Shablon.PROP_Maska = _Sheet.Cells[_y, _x++].Text ?? "";
                    _Shablon.PROP_Type = (byte)_Sheet.Cells[_y, _x++].Value2;
                    _Shablon.PROP_Razdel = _Sheet.Cells[_y, _x++].Value2 ?? "";
                    _Shablon.PROP_Name = _Sheet.Cells[_y, _x++].Value2 ?? "";
                    _Shablon.PROP_ValueStart = _Sheet.Cells[_y, _x++].Value2 ?? "";
                    _Shablon.PROP_OutText = _Sheet.Cells[_y, _x++].Value2 ?? "";
                    _Shablon.PROP_InText = _Sheet.Cells[_y, _x++].Value2 ?? "";
                    _Shablon.PROP_xFormat = _Sheet.Cells[_y, _x++].Value2 ?? "";
                    _Shablon.PROP_xLua = _Sheet.Cells[_y, _x++].Value2 ?? "";
                    _Shablon.PROP_xInfo = _Sheet.Cells[_y, _x].Value2 ?? "";
                    _Shablon.PROP_TipProtokol = _TipProtokol;
                    _Shablon.PROP_FlagEdit = false;
                    _y++;

                    _ListShablons.Add(_Shablon);
                }
            }
            catch (Exception)
            {
                _WorkBook.Close();
                _ExcelApp.Quit();
                MessageBox.Show($"Ошибка загрузки в строке: {_y}, в столбце: {--_x}", "Ошибка");
                return;
            }

            UserShablon.MET_FactoryListShablon(_eTip, _CodShablon);
            int _Remov = ((VirtualModul)MyGlo.Modul).PUB_Shablon.RemoveAll(p => p.PROP_ID == _CodShablon);
            ((VirtualModul)MyGlo.Modul).PUB_Shablon.AddRange(_ListShablons);

            _WorkBook.Close();
            _ExcelApp.Quit();

            // Если загружаем в SQL
            if (PRI_CheckBox_1.IsChecked == true)
            {
                UserShablon.MET_SaveExcelToSQL(_eTip, _CodShablon);
                MessageBox.Show($"Удалено: {_Remov} строк. Загружено {_ListShablons.Count} строк!", "Загружено в SQL");
            }
            else
            {
                MessageBox.Show($"Удалено: {_Remov} строк. Загружено {_ListShablons.Count} строк!", "Загружено в Память");
            }
        }

        /// <summary>СОБЫТИЕ Выбор Типа шаблонов "apaN, ast, kdl, par"</summary>
        private void PART_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Запрос
            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);

            // В выделяем первую строку
            if (PRO_DataView != null)
            {
                // Выделяем первую строку
                if (PRO_DataView.Table.Rows.Count != -1)
                    PART_DataGrid.SelectedIndex = 0;
            }
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Загрузки шаблона в SQL филиала"</summary>
        protected virtual void PRI_ButtonFromToFilialSQL_1_Click(object sender, RoutedEventArgs e)
        {
            DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
            int _CodShablon = Convert.ToInt16(_DataRowView.Row["Cod"]);
            string _TipDoc = PRI_ComboBox_1.SelectedValue.ToString();
            eTipDocum _eTip = eTipDocum.Null;
            switch (_TipDoc)
            {
                case "apaN":
                    _eTip = eTipDocum.Pol;
                    break;
                case "ast":
                    _eTip = eTipDocum.Stac;
                    break;
                case "par":
                    _eTip = eTipDocum.Paracl;
                    break;
                case "kdl":
                    _eTip = eTipDocum.Kdl;
                    break;
            }

            var _TipProtokol = new MyTipProtokol(_eTip);

            // Вопрос на загрузку
            if (MessageBox.Show($"Вы точно хотите загрузить {_CodShablon} шаблон на филиал?", "Вот это вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (MySql.MET_QueryNo(MyQuery.MET_Shablon_InsertToFilial_1(_CodShablon, _TipProtokol.PROP_Prefix)))
                    MessageBox.Show($"Успех!", "Загружено в SQL", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                else
                    MessageBox.Show($"Что то пошло не так!", "Загружено в SQL", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            // Фильтр по UserCod
            return MyQuery.MET_ListShablon_Select_5(PRI_ComboBox_1.SelectedValue.ToString(), 
                PRI_DatePicker_1.DisplayDate, 
                PRI_DatePicker_2.DisplayDate, 
                PRI_UserCod.PROP_Text, 
                PRI_CheckBox_2.IsChecked);
        }
        
        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "", "Cod", "TipObsled", "", "Name", "ProfilVr", "Cou", "Dmin", "Dmax" };
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
            PART_DataGrid.Columns[2].Width = 50;     // Cod
            PART_DataGrid.Columns[4].Width = 20;     // IconDel
            PART_DataGrid.Columns[5].Width = 500;    // Name
        }

        /// <summary>МЕТОД Создание фильтров для загрузки данных из SQL</summary>
        protected override void MET_SqlFilter()
        {
            // Запрос
            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);
        }

        /// <summary>МЕТОД Проверяем доступность данного окна текущему пользователю</summary>        
        public static new bool MET_Access()
        {
            if (!MyGlo.Admin)
            {
                MessageBox.Show("У вас нет доступа.");
                return false;
            }
            return true;
        }

        /// <summary>МЕТОД Выбор данных</summary>
        protected override void MET_Select()
        {
            if (!PROP_FlagButtonSelect || PART_DataGrid.SelectedItem == null)
                return;

            DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
            int _CodShablon = Convert.ToInt16(_DataRowView.Row["Cod"]);
            string _NameSha = Convert.ToString(_DataRowView.Row["Name"]);
            string _ImageSha = Convert.ToString(_DataRowView.Row["Icon"]);
            string _TipDoc = PRI_ComboBox_1.SelectedValue.ToString();
            eTipDocum _eTip = eTipDocum.Null;
            switch (_TipDoc)
            {
                case "apaN":
                    _eTip = eTipDocum.Pol;
                    break;
                case "ast":
                    _eTip = eTipDocum.Stac;
                    break;
                case "par":
                    _eTip = eTipDocum.Paracl;
                    break;
                case "kdl":
                    _eTip = eTipDocum.Kdl;
                    break;
            }

            var _TipProtokol = new MyTipProtokol(_eTip);

            UserWindow_EditProtokol _WinSpr = new UserWindow_EditProtokol(_TipProtokol, _CodShablon, _NameSha, _ImageSha, PRI_DatePicker_1.DisplayDate, PRI_DatePicker_2.DisplayDate, PRI_UserCod.PROP_Text);
            _WinSpr.Show();
        }
    }
}
