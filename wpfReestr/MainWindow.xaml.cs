using ClosedXML.Excel;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Telerik.Windows;
using Telerik.Windows.Controls;
using wpfGeneral.UserWindows;
using wpfStatic;
using e = Microsoft.Office.Interop.Excel;
using m = wpfStatic.MyMet;

namespace wpfReestr
{
    /// <summary>КЛАСС Главного окна wpfReestr. Логика взаимодействия для MainWindow.xaml</summary>
    public partial class MainWindow
    {
        private const string PRI_Filtr1 = "Фильтр 1";
        private const string PRI_Filtr2 = "Фильтр 2";
        private string PRI_Ver;
        private int PRI_FindCod = 0;
        private string PRI_FindText = "";

        /// <summary>Oбъявляем DataView Страховых файлов</summary>
        private DataView PRI_DVStrahFile;
        /// <summary>Oбъявляем DataView Страховых реестров</summary>
        private DataView PRI_DVStrahReestr;
        /// <summary>Имя таблицы Страховых реестров (с добавлением номера реестр) </summary>
        private string PRI_NameTable;
        /// <summary>Строка с именем столбца, активной ячейки</summary>
        private string PRI_CurrentColumn;
        /// <summary>Условия фильтров</summary>
        private string PRI_Where = "";
        /// <summary>Текущий код выбранного файла реестров</summary>
        private int PRI_CodFile;

        /// <summary>СВОЙСТВО Скрывает, открывает скрытые реестры</summary>
        public bool PROP_FlagHideStrahFile { get; set; }

        /// <summary>КОНСТРУКТОР</summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>СОБЫТИЕ Открытие окна </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyGlo.Event_Error = MET_Delg_Error;                        // переменная делегата (показываем окно с ошибкой соединения)
            // Считываем начальные параметры командной строки
            MyGlo.MET_ComStr();
            // Проверка на пользователя (848 - без пользователя)
            if (MyGlo.User == 848)
            {
                MessageBox.Show("Вы куда? Вам сюда нельзя!");
                this.Close();
            }
            // Версия программы
            PRI_Ver = "wpfReestr -- Реестры -- " + m.MET_Ver() + "  (" + MyGlo.UserName + ")";
            Title = PRI_Ver;
            // Создаем таблицу страховых файлов
            MET_Create_StrahFile();
            PROP_FlagHideStrahFile = true;
            MET_FiltrFile();
        }

        /// <summary>СОБЫТИЕ Выбор в меню</summary>
        private void MenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            VirtualUserWindow _UseWinSpr;
            switch ((sender as RadMenuItem)?.Header.ToString())
            {
                case "_Выгрузка в XML":
                    // Выгузка в XML
                    MET_SaveXML();
                    break;
                case "_Перерасчет реестра":
                    // Перерасчет реестра
                    MET_Update_StrahReestr();
                    break;
                case "_Обнуление реестра через Excel":
                    // Перерасчет реестра
                    MET_ZeroFromExcel();
                    break;
                case "_Удаление реестра через Excel":
                    // Перерасчет реестра
                    MET_DeleteFromExcel();
                    break;
                case "_Комментарии к случаям реестра через Excel":
                    // Перерасчет реестра
                    MET_CommentFromExcel();
                    break;
                case "_Обновить":
                    // Обновить
                    MET_Update_StrahFile();
                    break;
                case "_Скрывать закрытые реестры":
                    // Скрывать закрытые реестры
                    PROP_FlagHideStrahFile = !PROP_FlagHideStrahFile;
                    MET_FiltrFile();
                    break;
                case "_Диагнозы МКБ-10":
                    // Справочник диагнозов
                    _UseWinSpr = new UserWindow_Diag();
                    _UseWinSpr.Show();
                    break;
                case "_Операции":
                    // Справочник Операций
                    _UseWinSpr = new UserWindow_Oper(DateTime.Today);
                    _UseWinSpr.Show();
                    break;
                case "_Справочник связей и тарифов":
                    // Связи стационара
                    _UseWinSpr = new UserWindow_StrahTarif();
                    _UseWinSpr.Show();
                    break;
                case "_Справочник врачей МИАЦ":
                    // Справочник врачей МИАЦ
                    _UseWinSpr = new UserWindow_StrahVrachMIAC();
                    _UseWinSpr.Show();
                    break;
                case "_Формировать реестры":
                    // Формируем реестры 2021
                    MyReestr _WinReestr2021 = new MyReestr();
                    _WinReestr2021.ShowDialog();
                    // Пересчитываем файлы реестров
                    MET_Create_StrahFile();
                    break;
                 case "_Информация полей реестра":
                    // Информация полей реестра
                    MyInfColumn _WinInfColumn = new MyInfColumn(dataGrid1);
                    _WinInfColumn.Show();
                    break;
                 case "_Пометить записи":
                    // Пометить все записи по текущиму фильтру
                    MET_SelectUpdate(1);
                    break;
                 case "_Убрать метку записей":
                    // Убрать метку записей по текущиму фильтру
                    MET_SelectUpdate(0);
                    break;
                 case "_Обновление Страховых компаний":
                    // Обновляем страховые компании
                    MyLoadStrahCompXML _WinStrahComp = new MyLoadStrahCompXML();
                    _WinStrahComp.ShowDialog();
                    break;
                case "_Обновление Врачей МИАЦ":
                    // Обновляем справочник Врачей МИАЦ
                    MyLoadVrachMiacXML _WinVrachMiac = new MyLoadVrachMiacXML();
                    _WinVrachMiac.ShowDialog();
                    break;
                case "_Обновление OKATO":
                    // Меняем ОКАТО
                    MyUpdateOKATO _WinUpdateOKATO = new MyUpdateOKATO();
                    _WinUpdateOKATO.ShowDialog();
                    break;
                case "_Реестр для экономистов":
                    // Выводим в Excel реестр для экономиство
                    MET_Report_ReestrEco();
                    break;
            }
        }

        #region ---- СОБЫТИЯ Случаи dataGrid1 ----
        /// <summary>СОБЫТИЕ Сохраняем измения в ячейке в dataGrid1</summary>
        private void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Меняем (добавляем запись), если строка не пустая
            string _Str = "";
            if (e.Column is DataGridTextColumn)
            {
                _Str = ((TextBox)e.EditingElement).Text;
            }
            if (e.Column is DataGridCheckBoxColumn)
            {
                if (((CheckBox)e.EditingElement).IsChecked == true)
                    _Str = "1";
                else
                    _Str = "0";
            }
            string _Colum = e.Column.SortMemberPath;
            Type _Type = PRI_DVStrahReestr.Table.Columns[_Colum].DataType;
            string _Set = _Colum + " = ";
            switch (_Type.Name)
            {
                case "Decimal":
                case "Byte":
                case "Int32":
                    _Set += _Str;
                    if (_Str == "")
                    {
                        e.Cancel = true;                                        // в случае ошибки не даем зафиксировать данные
                        return;
                    }
                    break;
                case "DateTime":
                    // Если хотим обнулить дату
                    if (_Str == "")
                    {
                        _Set += "null";
                        ((TextBox) e.EditingElement).Text = null;
                        break;
                    }
                    try
                    {
                        DateTime _Date = DateTime.Parse(_Str);
                        _Set += "'" + _Date.ToString("d", CultureInfo.CreateSpecificCulture("en-US")) + "'";
                        ((TextBox)e.EditingElement).Text = _Date.ToString("d", CultureInfo.CreateSpecificCulture("en-US"));
                    }
                    catch (Exception)
                    {
                        ((TextBox) e.EditingElement).Text = _Str;
                        return;
                    }
                    break;
                default:
                    _Set += "'" + _Str + "'";
                    break;
            }
            int _Cod = MET_IntGrid1("Cod");
            e.Cancel = !MySql.MET_UpdateCell("StrahReestr", _Set, _Cod);
        }

        /// <summary>СОБЫТИЕ Сохраняем измения в json тегах поля NOM_USL при выходе из поля PART_TextBoxTeg</summary>
        private void PART_TextBoxTeg_LostFocus(object sender, RoutedEventArgs e)
        {
            if (dataGrid1.SelectedItem == null) return;
            string _jTag = PART_TextBoxTeg.Text;
            try
            {
                JObject.Parse(_jTag);
                int _Cod = MET_IntGrid1("Cod");
                MySql.MET_UpdateCell("StrahReestr", $"NOM_USL = '{PART_TextBoxTeg.Text}'", _Cod);
                BindingExpression _Bindingbe = PART_TextBoxTeg.GetBindingExpression(TextBox.TextProperty);
                _Bindingbe.UpdateSource();
            }
            catch
            {
                MessageBox.Show("Не верный формат JSON, данные не были сохранены!", "Сосредоточтесь");
            }
        }

        /// <summary>СОБЫТИЕ Контекстное Меню dataGrid1 - Удалить строки</summary>
        private void MenuItemData1_Click_1(object sender, RoutedEventArgs e)
        {
            // Если хотим удалить строки
            if (dataGrid1.SelectedItem != null)
            {
                if (MessageBox.Show("Удалить?", "Страховые Реестры", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        while (dataGrid1.SelectedItems.Count > 0)
                        {
                            // Находим запись
                            DataRowView _Row = (DataRowView)dataGrid1.SelectedItems[0];
                            // Если успешно удалили в базе, то и удаляем в таблице
                            if (MySql.MET_QueryNo(MyQuery.Table_Delete_1(("StrahReestr"), Convert.ToInt32(_Row.Row["Cod"]))))
                                _Row.Delete();
                        }
                        // Перезаписываем колличество строк
                        MET_SaveColl2("cou", (PRI_DVStrahReestr.Count - dataGrid1.SelectedItems.Count).ToString());
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Не могу это удалить?", "Ошибка");
                    }
                }
            }
        }

        /// <summary>СОБЫТИЕ Контекстное Меню dataGrid1 - Открыть wpfBazis</summary>
        private void MenuItemData1_Click_2(object sender, RoutedEventArgs e)
        {
            var _Key = Registry.CurrentUser.OpenSubKey("Software\\wpfBazis");
            if (_Key == null)
            {
                MessageBox.Show("wpfBazis не найден в Реестре)", "Ошибка!");
                return;
            }
            // Путь к wpfBazis
            string _Path = (string)_Key.GetValue("Path");

            if (dataGrid1.SelectedItem == null) return;
            try
            {
                decimal _KL = Convert.ToDecimal(MET_StrGrid1("ID_PAC"));
                decimal _IND = Convert.ToDecimal(MET_StrGrid1("PACIENTID"));
                MyTipProtokol _MyTipProtokol;
                switch (MET_IntGrid1("LPU_ST"))
                {
                    case 1:
                    case 2:
                        _MyTipProtokol = new MyTipProtokol(eTipDocum.Stac);     // модуль стационара
                        break;
                    case 3:
                        _MyTipProtokol = new MyTipProtokol(eTipDocum.Pol);      // модуль поликлиники
                        break;
                    case 4:
                        _MyTipProtokol = new MyTipProtokol(eTipDocum.Paracl);   // модуль параклиники
                        _IND = MySql.MET_QueryInt(MyQuery.Table_Select_2(_IND));
                        break;
                    default:
                        _MyTipProtokol = new MyTipProtokol(eTipDocum.Null);     // модуль истории болезни
                        break;
                }
                // Пытаемся открыть новую копию программы, для редактирования протоколов
                m.MET_EditWindows(_MyTipProtokol.PROP_TipDocum, _IND, _KL, _Path);
            }
            catch (Exception)
            {
                MessageBox.Show("Не понял?!", "Ошибка");
            }
        }

        /// <summary>СОБЫТИЕ Выбираем активную ячейку в dataGrid1</summary>
        private void dataGrid1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGrid1.CurrentColumn != null)
            {
                PRI_CurrentColumn = dataGrid1.CurrentColumn.SortMemberPath;
                string _Pole = dataGrid1.CurrentColumn.Header.ToString();
                try
                {
                    DataRowView _DataRowView = (DataRowView) dataGrid1.CurrentItem;
                    if (PART_ButtonFiltr1.IsChecked == false)
                    {
                        try
                        {
                            PART_FilterPole1.Tag = dataGrid1.CurrentColumn.SortMemberPath;
                            PART_FilterPole1.Content = _Pole;
                            PART_FilterZnach1.Content = _DataRowView.Row[PRI_CurrentColumn];
                        }
                        catch
                        {
                        }
                    }
                    if (PART_ButtonFiltr2.IsChecked == false)
                    {
                        try
                        {
                            PART_FilterPole2.Tag = dataGrid1.CurrentColumn.SortMemberPath;
                            PART_FilterPole2.Content = _Pole;
                            PART_FilterZnach2.Content = _DataRowView.Row[PRI_CurrentColumn];
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>СОБЫТИЕ Находим данные по строке ввода Поиска (поиск по dataGrid1)</summary>
        private void PART_TextBoxFindIDCase_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (PRI_FindText != PART_TextBoxFindIDCase.Text)
            {
                PRI_FindCod = 0;
                PRI_FindText = PART_TextBoxFindIDCase.Text;
            }
        }

        /// <summary>СОБЫТИЕ При фокусе строки поиска меняем на русский язык (ФИО или IDCASE) (поиск по dataGrid1)</summary>
        private void PART_TextBoxFindIDCase_GotFocus_1(object sender, RoutedEventArgs e)
        {
            m.MET_Lаng();
        }

        /// <summary>СОБЫТИЕ Находим данные по строке поиска (ФИО или IDCASE) при нажатии на Enter (поиск по dataGrid1)</summary>
        private void PART_TextBoxFindIDCase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (PART_ButtonFindFiltr.IsChecked == true)
                    MET_Filter();
                else
                    MET_Find();
            }
        }

        /// <summary>СОБЫТИЕ Смена фильтра Страховых реестра (фильтр по dataGrid1)</summary>
        private void proSqlDVStrahReestr_OnListChanged(object sender, ListChangedEventArgs args)
        {
            // Есть ли данный файл
            if (MyGlo.DataSet.Tables[PRI_NameTable] == null) return;
            // Находим Сумму и Количество записей
            DataTable _Table = MyGlo.DataSet.Tables[PRI_NameTable];
            object _Summ = _Table.Compute("Sum(SUM_LPU)", PRI_DVStrahReestr.RowFilter);
            PART_LabelCountZapAndSumm.Content = $"  Сумма: {_Summ:N2}   Записей: {PRI_DVStrahReestr.Count:N0}";
        }

        /// <summary>СОБЫТИЕ Нажали на кнопки фильтра dataGrid1</summary>
        private void PART_RadToggleButtonDataGrid1_Click(object sender, RoutedEventArgs e)
        {
            MET_Filter();
        }
        #endregion

        #region ---- МЕТОДЫ Случаи dataGrid1 ----
        /// <summary>МЕТОД Создаем/отображаем таблицу Страховых Реестров</summary>
        private void MET_Create_StrahReestr()
        {
            PRI_NameTable = "StrahReestr" + PRI_CodFile;
            // Если нету такой таблицы в памяти
            if (MyGlo.DataSet.Tables[PRI_NameTable] == null)
            {
                // Запрос
                MySql.MET_DsAdapterFill(MyQuery.StrahReestr_Select_1(PRI_CodFile), PRI_NameTable);
                // Cоздаем DataView для нашей таблице
                PRI_DVStrahReestr = new DataView(MyGlo.DataSet.Tables[PRI_NameTable]);
                // Событие на изменения списка Страховых файлов
                PRI_DVStrahReestr.ListChanged += proSqlDVStrahReestr_OnListChanged;
                // Cортировка по ФИО
                PRI_DVStrahReestr.Sort = dataGrid1.Columns[5].SortMemberPath;
                // Отображаем таблицу
                dataGrid1.ItemsSource = PRI_DVStrahReestr;
            }
            else
            {
                // Показываем уже загруженную таблицу
                PRI_DVStrahReestr.Table = MyGlo.DataSet.Tables[PRI_NameTable];
            }
            MET_Filter();
        }

        /// <summary>МЕТОД Ищем по номеру или ФИО</summary>
        private void MET_Find()
        {
            // Фильтр строке поиска ФИО или IDCASE
            if (!string.IsNullOrWhiteSpace(PART_TextBoxFindIDCase.Text))
            {
                string _Text = PART_TextBoxFindIDCase.Text;
                string _Where;
                string _Colum;
                string _Sort = PRI_DVStrahReestr.Sort;
                try
                {
                    // Если число, значить IDCASE
                    int _Val = m.MET_ParseInt(_Text);
                    if (_Val > 0)
                    {
                        _Where = $"((convert(NOM_ZAP, System.String) like '%{_Val.ToString("00000")}' and len(convert(NOM_ZAP, System.String)) = 8) or NOM_ZAP = {_Val})";
                        _Colum = "NOM_ZAP";
                    }
                    else
                    {
                        // Разбиваем строку поиска на отдельные слова
                        string[] _mFilter = _Text.Split(' ');
                        _Where = $"FAMILY like '{_mFilter[0]}%'";
                        if (_mFilter.Length > 1)
                            _Where += $" and NAME like '{_mFilter[1]}%'";
                        if (_mFilter.Length > 2)
                            _Where += $" and FATHER like '{_mFilter[2]}%'";
                        _Colum = "FAMILY";
                    }
                    dataGrid1.Items.SortDescriptions.Clear();   // Удаляем выбранную пользователем сортировку (оставляя её визуально)
                    PRI_DVStrahReestr.Sort = dataGrid1.Columns[1].SortMemberPath;
                    _Colum = "Cod";
                    int _Cou = PRI_DVStrahReestr.Table.Select(_Where, _Colum).Length;
                    if (_Cou > 0) // если найдено по условию
                    {
                        // Находим запись, по условию и по колонке
                        DataRow _Row = PRI_DVStrahReestr.Table.Select(_Where, _Colum)[PRI_FindCod];
                        if (_Cou > PRI_FindCod + 1)
                            PRI_FindCod++;
                        else
                            PRI_FindCod = 0;
                        // Номер записи
                        int _Index = PRI_DVStrahReestr.Find(_Row[_Colum]);
                        if (_Index != -1)
                        {
                            // Выделяем найденую запись
                            dataGrid1.SelectedIndex = _Index;
                            // Возвращаем сортировку на FIO
                            PRI_DVStrahReestr.Sort = _Sort;
                            // Отображаем в таблице найденую запись
                            dataGrid1.Dispatcher.BeginInvoke((Action)(() =>
                            {
                                dataGrid1.ScrollIntoView(dataGrid1.SelectedItem, dataGrid1.Columns[5]);
                            }));
                        }
                    }
                }
                catch
                {

                }  // если ввели некоректные данные для поиска (к примеру буквы в столбце чисел), то выходим
                   // Возвращаем сортировку на FIO
                PRI_DVStrahReestr.Sort = _Sort;
            }

        }

        /// <summary>МЕТОД Фильтруем данные</summary>
        private void MET_Filter()
        {
            if (PRI_DVStrahReestr == null)
                return;
            MyPole _Pole;
            PRI_Where = "1 = 1";
            // Впервые поданные или Повторно (исправленные) записи
            if (PART_ToggleButtonDataGrid1Main.IsChecked == true || PART_ToggleButtonDataGrid1Corrected.IsChecked == true)
            {
                if (PART_ToggleButtonDataGrid1Main.IsChecked == false)
                    PRI_Where += " and PR_NOV <> 0";
                if (PART_ToggleButtonDataGrid1Corrected.IsChecked == false)
                    PRI_Where += " and PR_NOV <> 1";
            }
            // По Услуге (LPU_ST)
            if (PART_ToggleButtonDataGrid1Stac.IsChecked == true
                || PART_ToggleButtonDataGrid1StacDnev.IsChecked == true
                || PART_ToggleButtonDataGrid1PosPolikl.IsChecked == true
                || PART_ToggleButtonDataGrid1Paracl.IsChecked == true
                || PART_ToggleButtonDataGrid1Microscope.IsChecked == true)
            {
                if (PART_ToggleButtonDataGrid1Stac.IsChecked == false)
                    PRI_Where += " and LPU_ST <> 1";
                if (PART_ToggleButtonDataGrid1StacDnev.IsChecked == false)
                    PRI_Where += " and LPU_ST <> 2";
                if (PART_ToggleButtonDataGrid1PosPolikl.IsChecked == false)
                    PRI_Where += " and LPU_ST <> 3";
                if (PART_ToggleButtonDataGrid1Paracl.IsChecked == false)
                    PRI_Where += " and LPU_ST <> 4";
                if (PART_ToggleButtonDataGrid1Microscope.IsChecked == false)
                    PRI_Where += " and LPU_ST <> 5";
            }
            // Cтраховые компании (если все выключены, то показываем всё, если хоть один включен, то уже начинаем фильтровать )
            if (PART_ToggleButtonDataGrid1SmoAlfa.IsChecked == true
                || PART_ToggleButtonDataGrid1SmoKapital.IsChecked == true
                || PART_ToggleButtonDataGrid1SmoSogaz.IsChecked == true
                || PART_ToggleButtonDataGrid1SmoInogor.IsChecked == true)
            {
                if (PART_ToggleButtonDataGrid1SmoAlfa.IsChecked == false)
                    PRI_Where += " and PLAT <> 55050";
                if (PART_ToggleButtonDataGrid1SmoKapital.IsChecked == false)
                    PRI_Where += " and PLAT <> 55041";
                if (PART_ToggleButtonDataGrid1SmoSogaz.IsChecked == false)
                    PRI_Where += " and PLAT <> 55044";
                if (PART_ToggleButtonDataGrid1SmoInogor.IsChecked == false)
                    PRI_Where += " and (PLAT > 55000 and PLAT < 55999)";
            }
            // Показать только Детей
            if (PART_ToggleButtonDataGrid1Baby.IsChecked == true)
            {
                PRI_Where += " and Det = 1";
            }
            // Фильтр по выделенному полю и кнопки "Ф1"
            if (PART_ButtonFiltr1.IsChecked == true & PART_FilterPole2.Content.ToString() != "Поле")
            {
                _Pole = new MyPole(PART_FilterPole1.Tag.ToString(), PRI_DVStrahReestr);
                if (!_Pole.PUB_Error)
                    MET_Where(_Pole.MET_Filtr(PART_FilterZnach1.Content.ToString()));
            }
            // Фильтр по выделенному полю и кнопки "Ф2"
            if (PART_ButtonFiltr2.IsChecked == true & PART_FilterPole2.Content.ToString() != "Поле")
            {
                _Pole = new MyPole(PART_FilterPole2.Tag.ToString(), PRI_DVStrahReestr);
                if (!_Pole.PUB_Error)
                    MET_Where(_Pole.MET_Filtr(PART_FilterZnach2.Content.ToString()));
            }
            // Фильтр строке поиска ФИО или IDCASE
            if (PART_ButtonFindFiltr.IsChecked == true && !string.IsNullOrWhiteSpace(PART_TextBoxFindIDCase.Text))
            {
                string _Text = PART_TextBoxFindIDCase.Text;
                string _Where;
                // Если число, значить IDCASE
                int _Val = m.MET_ParseInt(_Text);
                if (_Val > 0)
                {

                    _Where = $" and ((convert(NOM_ZAP, System.String) like '%{_Val.ToString("00000")}' and len(convert(NOM_ZAP, System.String)) = 8) or NOM_ZAP = {_Val})";
                }
                else
                {
                    // Разбиваем строку поиска на отдельные слова
                    string[] _mFilter = _Text.Split(' ');
                    _Where = $" and FAMILY like '{_mFilter[0]}%'";
                    if (_mFilter.Length > 1)
                        _Where += $" and NAME like '{_mFilter[1]}%'";
                    if (_mFilter.Length > 2)
                        _Where += $" and FATHER like '{_mFilter[2]}%'";
                }
                PRI_Where += _Where;
            }
            try
            {
                PRI_DVStrahReestr.RowFilter = PRI_Where;
            }
            catch
            {
            }
        }

        /// <summary>МЕТОД Добавляем в строку фильтра PRI_Where новое условие</summary>
        private void MET_Where(string pWhere)
        {
            if (PRI_Where.Length > 0)
                PRI_Where += " and " + pWhere;
            else
                PRI_Where = pWhere;
        }

        /// <summary>МЕТОД Выделить/убрать выделение всех выделенных записей, проставив в поле xUpdate = 1 или 0</summary>
        /// <param name="pFlag">1 - помечаем запись, 0 - убераем пометку</param>
        private void MET_SelectUpdate(int pFlag)
        {
            int _Cod = MET_IntGrid2("Cod");
            if (_Cod == 0)
            {
                MessageBox.Show("Нужно выбрать реестр!");
                return;
            }
            int _Count = dataGrid1.Items.Count - 1;
            if (MessageBox.Show(String.Format("Вы точно хотите убрать/поставить метку у {0} записей?", _Count), "Внимание!", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            string _Where = PRI_Where.Length > 0 ? " and " + PRI_Where : "";
            MySql.MET_QueryNo(MyQuery.StrahReestr_Update_1(pFlag, _Cod, _Where));
            // Чистим фильтры
            PRI_Where = "";
            MET_FiltrClear();
            MET_Update_StrahFile();
        }

        /// <summary>МЕТОД Перерасчет Страховых Реестров (пока только сумму, номера не персчитываем)</summary>
        private void MET_Update_StrahReestr()
        {
            // ---- Пересчитаем поле N_ZAP и NOM_ZAP
            int _Cod = MET_IntGrid2("Cod");

            // Скрываем выбранную таблицу
            MyGlo.DataSet.Tables.Remove(PRI_NameTable);
            //// В зависмости от наличия родителя пересчитываем нумерацию
            //if (MET_IntGrid2("pParent") > 0)
            //    MySql.MET_QueryNo(MyQuery.StrahReestr_Update_1(_Cod, MET_IntGrid2("pParent")));
            //else
            //    MySql.MET_QueryNo(MyQuery.StrahReestr_Update_2(_Cod));

            // ---- Пересчитаем поле Сумму
            MySql.MET_QueryNo(MyQuery.StrahFile_Update_1(_Cod));
            MET_Create_StrahFile();
            MessageBox.Show("Перерасчет окончен");
        }

        /// <summary>МЕТОД Чистим фильтр</summary>
        private void MET_FiltrClear()
        {
            // Чистим фильтр 1
            if (PART_ButtonFiltr1.IsChecked == true)
            {
                PART_ButtonFiltr1.IsChecked = false;
                PART_FilterPole1.Content = "Поле";
                PART_FilterZnach1.Content = "Значение";
            }
            // Чистим фильтр 2
            if (PART_ButtonFiltr2.IsChecked == true)
            {
                PART_ButtonFiltr2.IsChecked = false;
                PART_FilterPole2.Content = "Поле";
                PART_FilterZnach2.Content = "Значение";
            }
            // Поиск
            //PART_TextBoxFindIDCase.Text = "";
            // Кнопки
           // PART_ButtonFindFiltr.IsChecked = false;
            PART_ToggleButtonDataGrid1Stac.IsChecked = false;
            PART_ToggleButtonDataGrid1StacDnev.IsChecked = false;
            PART_ToggleButtonDataGrid1PosPolikl.IsChecked = false;
            PART_ToggleButtonDataGrid1Paracl.IsChecked = false;
            PART_ToggleButtonDataGrid1Microscope.IsChecked = false;
            PART_ToggleButtonDataGrid1SmoAlfa.IsChecked = false;
            PART_ToggleButtonDataGrid1SmoKapital.IsChecked = false;
            PART_ToggleButtonDataGrid1SmoSogaz.IsChecked = false;
            PART_ToggleButtonDataGrid1SmoInogor.IsChecked = false;
            PART_ToggleButtonDataGrid1Baby.IsChecked = false;
        }

        /// <summary>МЕТОД Вытаскиваем строку из поля</summary>
        /// <param name="pNamePole">Имя поля</param>
        private string MET_StrGrid1(string pNamePole)
        {
            DataRowView _Row = (DataRowView)dataGrid1.SelectedItem;
            return Convert.ToString(_Row.Row[pNamePole]);
        }

        /// <summary>МЕТОД Вытаскиваем число из поля</summary>
        /// <param name="pNamePole">Имя поля</param>
        private int MET_IntGrid1(string pNamePole)
        {
            DataRowView _Row = (DataRowView)dataGrid1.SelectedItem;
            return Convert.ToInt32(_Row.Row[pNamePole]);
        }
        #endregion

        #region ---- СОБЫТИЯ Файлы dataGrid2 ----
        /// <summary>СОБЫТИЕ Выбирае запись в dataGrid2</summary>
        private void dataGrid2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Если строка не выделенна, то выходим (как правило при удалении)
            if (dataGrid2.SelectedItem == null)
            {
                dataGrid1.ItemsSource = null;
                return;
            }
            // Версия программы
            Title = PRI_Ver + "   " + MET_NameFile(out string _PacFileName);
            // Сброс фильтров
            MET_FiltrClear();
            // Отображаем данные Страховых реестров
            PRI_CodFile = MET_IntGrid2("Cod");
            MET_Create_StrahReestr();
        }

        /// <summary>СОБЫТИЕ Сохраняем измения в ячейке в dataGrid2</summary>
        private void dataGrid2_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            string _Str = "";

            if (e.Column is DataGridTextColumn)
            {
                _Str = ((TextBox)e.EditingElement).Text;
            }
            if (e.Column is DataGridCheckBoxColumn)
            {
                if (((CheckBox)e.EditingElement).IsChecked == true)
                    _Str = "1";
                else
                    _Str = "0";
            }

            // Меняем (добавляем запись), если строка не пустая
            if (_Str != "")
            {
                string _Colum = e.Column.SortMemberPath;
                Type _Type = PRI_DVStrahFile.Table.Columns[_Colum].DataType;
                string _Set = _Colum + " = ";
                switch (_Type.Name)
                {
                    case "Decimal":
                    case "Byte":
                    case "Int32":
                        _Set += _Str;
                        break;
                    case "DateTime":
                        DateTime _Date = DateTime.Parse(_Str);
                        _Set += "'" + _Date.ToString("d", CultureInfo.CreateSpecificCulture("en-US")) + "'";
                        ((TextBox)e.EditingElement).Text = _Date.ToString("d", CultureInfo.CreateSpecificCulture("en-US"));
                        break;
                    default:
                        _Set += "'" + _Str + "'";
                        break;
                }
                int _Cod = MET_IntGrid2("Cod");
                e.Cancel = !MySql.MET_UpdateCell("StrahFile", _Set, _Cod);
            }
            else
                e.Cancel = true;                                                // в случае ошибки не даем зафиксировать данные
        }

        /// <summary>СОБЫТИЕ Контекстное Меню dataGrid2 - Удалить строки</summary>
        private void MenuItemData2_Click_1(object sender, RoutedEventArgs e)
        {
            // Если хотим удалить строки
            if (dataGrid2.SelectedItem != null)
            {
                if (MessageBox.Show("Удалить?", "Страховые Файлы", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    // Находим запись
                    DataRowView _Row = (DataRowView)dataGrid2.SelectedItems[0];
                    int _Cod = Convert.ToInt32(_Row.Row["Cod"]);
                    // Если успешно удалили в базе, то и удаляем в таблице
                    if (MySql.MET_QueryNo(MyQuery.Table_Delete_1("StrahFile", _Cod)))
                    {
                        MyGlo.DataSet.Tables.Remove("StrahReestr" + _Cod);
                        _Row.Delete();
                    }
                }
            }
        }

        /// <summary>СОБЫТИЕ Нажали на кнопки фильтра dataGrid2</summary>
        private void PART_RadToggleButton_Click(object sender, RoutedEventArgs e)
        {
            MET_FiltrFile();
        }
        #endregion

        #region ---- МЕТОДЫ Файлы dataGrid2 ----
        /// <summary>МЕТОД Создаем таблицу Страховых Файлов</summary>
        protected void MET_Create_StrahFile()
        {
            MySql.MET_DsAdapterFill(MyQuery.StrahFile_Select_1(), "StrahFile");
            // Cоздаем DataView для нашей таблице
            PRI_DVStrahFile = new DataView(MyGlo.DataSet.Tables["StrahFile"]);
            //
            PROP_FlagHideStrahFile = true;
            // Отображаем таблицу
            dataGrid2.ItemsSource = PRI_DVStrahFile;
        }

        /// <summary>МЕТОД Записываем во 2ю таблицу данные (только в таблицу, в базу пока не пишем)</summary>
        /// <param name="pNamePole">Имя поля</param>
        /// <param name="pValue">Значение, которое сохраняем</param>
        private void MET_SaveColl2(string pNamePole, string pValue)
        {
            DataRowView _Row = (DataRowView)dataGrid2.SelectedItem;
            try
            {
                _Row.BeginEdit();
                _Row.Row[pNamePole] = pValue;
                _Row.EndEdit();
            }
            catch
            {
            }
        }

        /// <summary>МЕТОД Вытаскиваем строку из поля</summary>
        /// <param name="pNamePole">Имя поля</param>
        private string MET_StrGrid2(string pNamePole)
        {
            DataRowView _Row = (DataRowView)dataGrid2.SelectedItem;
            return Convert.ToString(_Row.Row[pNamePole]);
        }

        /// <summary>МЕТОД Вытаскиваем число из поля</summary>
        /// <param name="pNamePole">Имя поля</param>
        private int MET_IntGrid2(string pNamePole)
        {
            DataRowView _Row = (DataRowView)dataGrid2.SelectedItem;
            if (_Row == null) return 0;
            return Convert.ToInt32(_Row.Row[pNamePole]);
        }

        /// <summary>МЕТОД Фильр Файлов</summary>
        private void MET_FiltrFile()
        {
            PRI_DVStrahFile.RowFilter = "1 = 1";
            if (PROP_FlagHideStrahFile)
                PRI_DVStrahFile.RowFilter += " and pHide = 0";

            // Тип (если все выключены, то показываем всё, если хоть один включен, то уже начинаем фильтровать )
            if (PART_ToggleButtonFileC.IsChecked == true || PART_ToggleButtonFileH.IsChecked == true || PART_ToggleButtonFileT.IsChecked == true)
            {
                if (PART_ToggleButtonFileC.IsChecked == false)
                    PRI_DVStrahFile.RowFilter += " and TipImage <> 'mnFileC'";
                if (PART_ToggleButtonFileH.IsChecked == false)
                    PRI_DVStrahFile.RowFilter += " and TipImage <> 'mnFileH'";
                if (PART_ToggleButtonFileT.IsChecked == false)
                    PRI_DVStrahFile.RowFilter += " and TipImage <> 'mnFileT'";
            }

            //// Cтраховые компании (если все выключены, то показываем всё, если хоть один включен, то уже начинаем фильтровать )
            //if (PART_ToggleButtonSmoAlfa.IsChecked == true || PART_ToggleButtonSmoKapital.IsChecked == true || PART_ToggleButtonSmoSogaz.IsChecked == true
            //                                               || PART_ToggleButtonSmoInogor.IsChecked == true || PART_ToggleButtonSmoAll.IsChecked == true)
            //{
            //    if (PART_ToggleButtonSmoAlfa.IsChecked == false)
            //        PRI_DVStrahFile.RowFilter += " and StrahCompImage <> 'mnSmoAlfa'";
            //    if (PART_ToggleButtonSmoKapital.IsChecked == false)
            //        PRI_DVStrahFile.RowFilter += " and StrahCompImage <> 'mnSmoKapital'";
            //    if (PART_ToggleButtonSmoSogaz.IsChecked == false)
            //        PRI_DVStrahFile.RowFilter += " and StrahCompImage <> 'mnSmoSogaz'";
            //    if (PART_ToggleButtonSmoInogor.IsChecked == false)
            //        PRI_DVStrahFile.RowFilter += " and StrahCompImage <> 'mnSmoInogor'";
            //    if (PART_ToggleButtonSmoAll.IsChecked == false)
            //        PRI_DVStrahFile.RowFilter += " and StrahCompImage <> 'mnSmoAll'";
            //}

            // Пакеты (если все выключены, то показываем всё, если хоть один включен, то уже начинаем фильтровать )
            if (PART_ToggleButtonPaket123.IsChecked == true || PART_ToggleButtonaket124.IsChecked == true || PART_ToggleButtonPaket126.IsChecked == true)
            {
                if (PART_ToggleButtonPaket123.IsChecked == false)
                    PRI_DVStrahFile.RowFilter += " and PaketImage <> 'mnReestr123'";
                if (PART_ToggleButtonaket124.IsChecked == false)
                    PRI_DVStrahFile.RowFilter += " and PaketImage <> 'mnReestr124'";
                if (PART_ToggleButtonPaket126.IsChecked == false)
                    PRI_DVStrahFile.RowFilter += " and PaketImage <> 'mnRakReg'";
            }

            // Реестр основные/исправленные (если все выключены, то показываем всё, если хоть один включен, то уже начинаем фильтровать )
            if (PART_ToggleButtonMain.IsChecked == true || PART_ToggleButtonCorrected.IsChecked == true)
            {
                if (PART_ToggleButtonMain.IsChecked == false)
                    PRI_DVStrahFile.RowFilter += " and ParetnImag <> 'mnDocuments'";
                if (PART_ToggleButtonCorrected.IsChecked == false)
                    PRI_DVStrahFile.RowFilter += " and ParetnImag <> 'mnNaznach'";
            }
        }
        #endregion

        #region ---- МЕТОДЫ Общие ----
        /// <summary>МЕТОД Обновить все данные</summary>
        protected void MET_Update_StrahFile()
        {
            MyGlo.DataSet.Tables.Clear();
            MET_Create_StrahFile();
        }

        /// <summary>МЕТОД Выгузка в XML 2022</summary>
        private void MET_SaveXML()
        {
            try
            {
                if (MET_IntGrid2("YEAR") < 2021 || MET_IntGrid2("Cod") < 1373)
                {
                    MessageBox.Show("Данная версия выгружает, только реестры начиная с 2022 года (включая превышения за декабрь 2021, начиная с 1373 реестра");
                    return;
                }

                Mouse.OverrideCursor = Cursors.Wait;

                string _MainFileName = MET_NameFile(out string _PatFileName);
                int _Cod = MET_IntGrid2("Cod");
                // Формируем основной файл xml
                switch (MET_StrGrid2("VMP"))
                {
                    case "ЗНО":
                        MySql.MET_QueryXML(MyQuery.StrahReestrXML_Select_C_2022(_Cod, _MainFileName), _MainFileName);
                        break;
                    case "ВМП":
                        MySql.MET_QueryXML(MyQuery.StrahReestrXML_Select_T_2022(_Cod, _MainFileName), _MainFileName);
                        break;
                    case "без С":
                        MySql.MET_QueryXML(MyQuery.StrahReestrXML_Select_H_2022(_Cod, _MainFileName), _MainFileName);
                        break;
                    default:
                        MessageBox.Show("Данная версия не выгружает такие файлы, обратитесь к...");
                        Mouse.OverrideCursor = null;
                        return;
                }
                // Формируем файл xml с пациентами
                MySql.MET_QueryXML(MyQuery.StrahReestrXMLPerson_Select_1(_Cod, _MainFileName, _PatFileName), _PatFileName);
                // Архивируем файлы xml
                Process _Pro = new Process();
                _Pro.StartInfo.FileName = "7z.exe";
                _Pro.StartInfo.WorkingDirectory = @"c:\1Reestrs\";
                _Pro.StartInfo.Arguments = $"a -tzip {_MainFileName}.zip {_MainFileName}.xml {_PatFileName}.xml";
                _Pro.Start();
                _Pro.WaitForExit();
                Mouse.OverrideCursor = null;
                MessageBox.Show("Расчет окончен");
            }
            catch
            {
                Mouse.OverrideCursor = null;
                MessageBox.Show("Ошибка формирования XML файлов");
            }
        }

        /// <summary>МЕТОД Имя файла XML</summary>
        /// <param name="pPatFileName">Имя файла списка пациентов</param>
        /// <returns>Имя основноего файла лечения</returns>
        private string MET_NameFile(out string pPatFileName)
        {
            // Код нашей больницы
            string _MainFileName = "M555509";

            // Код страховой
            switch (MET_StrGrid2("StrahComp"))
            {
                case "Тестовые":
                case "ТФОМС":
                    _MainFileName += "T55";
                    break;
                case "Альфа (50)":
                    _MainFileName += "S55050";
                    break;
                case "Капитал МС (41)":
                    _MainFileName += "S55041";
                    break;
                case "СОГАЗ-Мед (44)":
                    _MainFileName += "S55044";
                    break;
            }

            // Год, месяц, пакет
            string _Month = MET_IntGrid2("MONTH") < 10 ? "0" + MET_StrGrid2("MONTH") : "" + MET_StrGrid2("MONTH");
            _MainFileName += "_" + MET_StrGrid2("YEAR").Substring(2, 2) + _Month + MET_StrGrid2("pPaket");

            pPatFileName = _MainFileName;
            // Префикс файла для (C - ЗНО, H - без С, T - ВМП)
            switch (MET_StrGrid2("VMP"))
            {
                case "ЗНО":
                    _MainFileName = "C" + _MainFileName;
                    pPatFileName = "LC" + pPatFileName;
                    break;
                case "без С":
                case "без ВМП":
                case "общий":
                    _MainFileName = "H" + _MainFileName;
                    pPatFileName = "L" + pPatFileName;
                    break;
                case "ВМП":
                    _MainFileName = "T" + _MainFileName;
                    pPatFileName = "LT" + pPatFileName;
                    break;
            }
            return _MainFileName;
        }

        /// <summary>МЕТОД Выводим в Excel реестр для экономиство</summary>
        public void MET_Report_ReestrEco()
        {
            // Если не выбран реестр, то сваливаем
            if (dataGrid2.SelectedItem == null)
            {
                MessageBox.Show("Не выбран реестр", "Ошибка!");
                return;
            }
            // Загружаем реестр в таблицу ReestrEco
            MySql.MET_DsAdapterFill(MyQuery.ReestrEco_Select_1(PRI_CodFile), "ReestrEco");

            var _WorkBook = new XLWorkbook();
            _WorkBook.Worksheets.Add(MyGlo.DataSet.Tables["ReestrEco"],"Реестр " + PRI_CodFile);

            var _Sheet = _WorkBook.Worksheet(1);
            _Sheet.Cell("B1").Value = "Тип";
            _Sheet.Cell("C1").Value = "Подразделение";
            _Sheet.Column("C").Width = 25;
            _Sheet.Cell("D1").Value = "Врач";
            _Sheet.Column("D").Width = 18;
            _Sheet.Cell("E1").Value = "Медсестра";
            _Sheet.Column("E").Width = 18;
            _Sheet.Cell("F1").Value = "Фамилия, имя, отчество";
            _Sheet.Cell("G1").Value = "Пол";
            _Sheet.Cell("H1").Value = "Дата рождения";
            _Sheet.Cell("I1").Value = "Возраст";
            _Sheet.Cell("J1").Value = "Полис";
            _Sheet.Cell("K1").Value = "Страх. компания";
            _Sheet.Cell("L1").Value = "Дата начала";
            _Sheet.Cell("M1").Value = "Дата окончания";
            _Sheet.Cell("N1").Value = "Койко дни";
            _Sheet.Cell("O1").Value = "Диагноз";
            _Sheet.Cell("P1").Value = "Сумма";
            _Sheet.Cell("Q1").Value = "Поликл.";
            _Sheet.Cell("R1").Value = "PRVS";
            _Sheet.Cell("S1").Value = "Profil";
            _Sheet.Cell("T1").Value = "Метод ВМП";
            _Sheet.Cell("U1").Value = "Код КСГ";
            _Sheet.Column("U").Width = 8;
            _Sheet.Cell("V1").Value = "Код Отделения";
            _Sheet.Cell("W1").Value = "Не оплачен";
            _Sheet.Cell("X1").Value = "Код реестра";
            _Sheet.Cell("Y1").Value = "Услуга 1";
            _Sheet.Cell("Z1").Value = "Услуга 2";
            _Sheet.Cell("AA1").Value = "Услуга 3";
            _Sheet.Cell("AB1").Value = "Услуга 4";
            _Sheet.Cell("AC1").Value = "Услуга 5";
            _Sheet.Cell("AD1").Value = "Архив";
            _Sheet.Cell("AE1").Value = "N013";
            _Sheet.Cell("AF1").Value = "Основной";
            // Заголовок
            _Sheet.Row(1).InsertRowsAbove(1);
            _Sheet.Cell("A1").Value = "РЕЕСТР СЧЕТА № " + PRI_CodFile;
            _Sheet.Range("A1:Y2").Row(1).Merge();
            _Sheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            _Sheet.Cell("A1").Style.Font.Bold = true;
            _Sheet.Cell("A1").Style.Font.FontSize = 15;

            // Сохраняем файл
            var _DialogFileSave = new SaveFileDialog()
            {
                FileName = "Reestr_" + PRI_CodFile,
                DefaultExt = ".xlsx",
                Filter = "Excel file|*.xlsx"
            };

            if (_DialogFileSave.ShowDialog() != true) return;

            try
            {
                _WorkBook.SaveAs(_DialogFileSave.FileName);
            }
            catch
            {
                MessageBox.Show("Данный уже файл открыт", "Ошибка!");
                return;
            }

            // Наш объект Excel
            var _ExcelApp = new e.Application();
            _ExcelApp.Workbooks.Open(_DialogFileSave.FileName);
            _ExcelApp.Visible = true;
        }

        /// <summary>МЕТОД Показываем окно с ошибкой соединения </summary>
        /// <param name="pException">Ошибка</param>
        public void MET_Delg_Error(Exception pException)
        {
            MyErrorWindow _Win = new MyErrorWindow();
            _Win.Owner = this;
            _Win.MET_ErrorObject(pException);
            if (_Win.ShowDialog() == true)
                Environment.Exit(0);
        }

        /// <summary>МЕТОД Удаляем данные из реестров, по выбранному Excel файлу</summary>
        public void MET_DeleteFromExcel()
        {
            OpenFileDialog _OpenFileDialog = new OpenFileDialog();
            _OpenFileDialog.Filter = "Файл удаления (Удалить*.xlsx)|Удалить*.xlsx";
            if (_OpenFileDialog.ShowDialog() != true) return;

            e.Application _ExcelApp = new e.Application();
            e.Workbook _WorkBook = _ExcelApp.Workbooks.Open(_OpenFileDialog.FileName);
            e.Worksheet _Sheet = (e.Worksheet)_WorkBook.Worksheets.Item[1];

            DataTable _Table = new DataTable();
            _Table.Columns.Add("IDCase", Type.GetType("System.Int32"));
            _Table.Columns.Add("Reestr", Type.GetType("System.Int32"));

            int _y = 1;
            int _x = 1;
            try
            {
                while (_Sheet.Cells[_y, 1].Value2 is double)
                {
                    _x = 1;
                    _Table.Rows.Add(new object[] { (int)_Sheet.Cells[_y, _x++].Value2, (int)_Sheet.Cells[_y, _x++].Value2 });
                    _y++;
                }
            }
            catch (Exception)
            {
                _WorkBook.Close();
                _ExcelApp.Quit();
                MessageBox.Show($"Ошибка загрузки в строке: {_y}, в столбце: {--_x}", "Ошибка");
                return;
            }

            _WorkBook.Close();
            _ExcelApp.Quit();

            MySql.MET_QueryNo("delete Bazis.dbo.StrahZero");

            MySql.MET_SqlBulkCopy(_Table);

            var _Hach = MySql.MET_QueryHash(MyQuery.DeleteFromExcel_Select_1());

            if ((int)_Hach["Cou"] == 0)
            {
                MessageBox.Show($"Что то не нашел что удалять?! Тут что то не так.", "Ошибка");
                return;
            }

            //if ((int)_Hach["Par"] == 0)
            //{
            //    MessageBox.Show($"В файле Excel есть строки с основными реестрами, а удалять можно только в исправленных!", "Ошибка");
            //    return;
            //}

            if (MessageBox.Show($"Вы точно хотите удалить {(int)_Hach["Cou"]} записей в {(int)_Hach["Rees"]} реестрах?", "Одумайтесь!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                MySql.MET_QueryNo(MyQuery.DeleteFromExcel_Delete_1());
                MessageBox.Show($"Записи удалены", "Это конец");
            }
        }

        /// <summary>МЕТОД Обнуляем данные реестров, по выбранному Excel файлу</summary>
        public void MET_ZeroFromExcel()
        {
            OpenFileDialog _OpenFileDialog = new OpenFileDialog();
            _OpenFileDialog.Filter = "Файл обнуления (Обнулить*.xlsx)|Обнулить*.xlsx";
            if (_OpenFileDialog.ShowDialog() != true) return;

            e.Application _ExcelApp = new e.Application();
            e.Workbook _WorkBook = _ExcelApp.Workbooks.Open(_OpenFileDialog.FileName);
            e.Worksheet _Sheet = (e.Worksheet)_WorkBook.Worksheets.Item[1];

            DataTable _Table = new DataTable();
            _Table.Columns.Add("IDCase", Type.GetType("System.Int32"));
            _Table.Columns.Add("Reestr", Type.GetType("System.Int32"));

            int _y = 1;
            int _x = 1;
            try
            {
                while (_Sheet.Cells[_y, 1].Value2 is double)
                {
                    _x = 1;
                    _Table.Rows.Add(new object[] { (int)_Sheet.Cells[_y, _x++].Value2, (int)_Sheet.Cells[_y, _x++].Value2 });
                    _y++;
                }
            }
            catch (Exception)
            {
                _WorkBook.Close();
                _ExcelApp.Quit();
                MessageBox.Show($"Ошибка загрузки в строке: {_y}, в столбце: {--_x}", "Ошибка");
                return;
            }

            _WorkBook.Close();
            _ExcelApp.Quit();

            MySql.MET_QueryNo("delete Bazis.dbo.StrahZero");

            MySql.MET_SqlBulkCopy(_Table);

            var _Hach = MySql.MET_QueryHash(MyQuery.ZeroFromExcel_Select_1());

            if ((int)_Hach["Cou"] == 0)
            {
                MessageBox.Show($"Что то не нашел что обнулять?! Тут что то не так.", "Ошибка");
                return;
            }

            //if ((int)_Hach["Par"] > 0)
            //{
            //    MessageBox.Show($"В файле Excel есть строки с исправленными реестрами (например: {(int)_Hach["Par"]}), а обнулять можно только в основных!", "Ошибка");
            //    return;
            //}

            if (MessageBox.Show($"Вы точно хотите обнулить {(int)_Hach["Cou"]} записей в {(int)_Hach["Rees"]} реестрах?", "Одумайтесь!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                MySql.MET_QueryNo(MyQuery.ZeroFromExcel_Update_1());
                MessageBox.Show($"Записей обнулено", "Это конец");
            }
        }

        /// <summary>МЕТОД Проставляем коментарии в реестре, по выбранному Excel файлу</summary>
        public void MET_CommentFromExcel()
        {
            OpenFileDialog _OpenFileDialog = new OpenFileDialog();
            _OpenFileDialog.Filter = "Файл комментарии (Комментарии*.xlsx)|Комментарии*.xlsx";
            if (_OpenFileDialog.ShowDialog() != true) return;

            e.Application _ExcelApp = new e.Application();
            e.Workbook _WorkBook = _ExcelApp.Workbooks.Open(_OpenFileDialog.FileName);
            e.Worksheet _Sheet = (e.Worksheet)_WorkBook.Worksheets.Item[1];

            DataTable _Table = new DataTable();
            _Table.Columns.Add("IDCase", Type.GetType("System.Int32"));
            _Table.Columns.Add("Reestr", Type.GetType("System.Int32"));
            _Table.Columns.Add("Comment", Type.GetType("System.String"));

            int _y = 1;
            int _x = 1;
            try
            {
                while (_Sheet.Cells[_y, 1].Value2 is double)
                {
                    _x = 1;
                    _Table.Rows.Add(new object[] { (int)_Sheet.Cells[_y, _x++].Value2, (int)_Sheet.Cells[_y, _x++].Value2, (string)_Sheet.Cells[_y, _x++].Value2 });
                    _y++;
                }
            }
            catch (Exception)
            {
                _WorkBook.Close();
                _ExcelApp.Quit();
                MessageBox.Show($"Ошибка загрузки в строке: {_y}, в столбце: {--_x}", "Ошибка");
                return;
            }

            _WorkBook.Close();
            _ExcelApp.Quit();

            MySql.MET_QueryNo("delete Bazis.dbo.StrahZero");

            MySql.MET_SqlBulkCopy(_Table);

            var _Hach = MySql.MET_QueryHash(MyQuery.CommentFromExcel_Select_1());

            if ((int)_Hach["Cou"] == 0)
            {
                MessageBox.Show($"Что то не нашел что комментровать?! Тут что то не так.", "Ошибка");
                return;
            }

            if (MessageBox.Show($"Вы точно хотите проставить комментрии {(int)_Hach["Cou"]} записей в {(int)_Hach["Rees"]} реестрах?", "Одумайтесь!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                MySql.MET_QueryNo(MyQuery.CommentFromExcel_Update_1());
                MessageBox.Show($"Комментарии проставлены", "Это конец");
            }
        }
        #endregion
    }

    ///<summary>КЛАСС Конвертор Возвращаем иконку из словаря по имени</summary>
    class ConvertTextToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (BitmapImage)Application.Current.FindResource(value.ToString());
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}