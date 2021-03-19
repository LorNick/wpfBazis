using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Reporting.WinForms;
using wpfGeneral.UserControls;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserTab;
using wpfGeneral.UserWindows;
using wpfGeneral.UserLua;
using wpfMKancReg;
using wpfMLaboratory;
using wpfMOtherLpu;
using wpfMViewer;
using wpfMVrParacl;
using wpfMVrPolicl;
using wpfMVrStac;
using wpfStatic;
using System.IO;
using Telerik.Windows.Documents.Fixed;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace wpfBazis
{
    /// <summary>КЛАСС Главного окна wpfBazis. Логика взаимодействия для MainWindow.xaml</summary>
    public partial class MainWindow
    {  
        /// <summary>Модуль</summary>
        public VirtualModul PUB_Modul;

        /// <summary>Текущая ветка</summary>
        private VirtualNodes PRI_FormMyNodes;

        /// <summary>Элемент шаблона, на котором стоит фокус</summary>
        private IInputElement PUB_FocusUI;   
        

        #region Старт
        /// <summary>КОНСТРУКТОР 1</summary>
        public MainWindow()
        {
            // Инициализация ручных компонентов
            InitializeComponent();

            // Инициализация программных компонентов
            MET_InitializeComponentUser();
        }

        /// <summary>МЕТОД Инициализация программных компонентов 2</summary>
        private void MET_InitializeComponentUser()
        {
            // Делегаты
            MyGlo.callbackEvent_sClose = MET_Delg_Close;                        // закрываем вкладки формы
            MyGlo.callbackEvent_sEditShablon = MET_Delg_EditShablon;            // переменная делегата (нужно ли сохранять шаблон)
            MyGlo.callbackEvent_sError = MET_Delg_Error;                        // переменная делегата (показываем окно с ошибкой соединения)
            MyGlo.callbackEvent_sReloadWindows = MET_Window_Loaded;             // переменная делегата (перезапуск программы)

            // Путь к программе
            MyGlo.PathExe = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // Команда выхода
            CommandBinding _ComClose = new CommandBinding(ApplicationCommands.Close);
            _ComClose.Executed += PART_ComandClose_Executed;
            CommandBindings.Add(_ComClose);

            // Событие на смену языка
            InputLanguageManager.Current.InputLanguageChanged += Current_InputLanguageChanged;
            // Устанавливаем русский язык
            InputLanguageManager.Current.CurrentInputLanguage = System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU");

            // Создаем контекстное меню
            MyGlo.ContextMenu = new ContextMenu();
            // Событие при открытии контекстного меню
            MyGlo.ContextMenu.Opened += PART_ContextMenu_Opened;      
        }

        /// <summary>СОБЫТИЕ Запуск программы 3</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Считываем начальные параметры командной строки
            MyGlo.MET_ComStr();
            // Создаем модуль
            PUB_Modul = MET_CreateModul(MyGlo.TypeModul);   
            // Параметры командной строки, только для этого модуля
            PUB_Modul.MET_ComStr();
            // Повторно Поля логирования, что бы узнать CodApstac (есть ещё дубляж в MyGlo.MET_ComStr)
            MyMet.MET_Log();

            // Запуск программы MET_Window_Loaded()
            MyGlo.callbackEvent_sReloadWindows?.Invoke(true);
        }

        /// <summary>МЕТОД Запуск программы 4</summary>
        /// <param name="pNewWindows">Новая программа (true) или просто переоткрываем старую (false)</param>
        private void MET_Window_Loaded(bool pNewWindows = true)
        {
            // Находим начальные данные
            PUB_Modul.MET_NachDan();
            // Будущий заголовок
            string _TitleTMP = PUB_Modul.MET_Title();
            // Сколько базисов уже открыто
            int _CoutWindows = 0;
            // Проверяем открыто ли такое окно (Модуль, ФИО, Пользователь) и что бы не был равен старому заголовку
            if (Title != _TitleTMP && MyMet.MET_DobleWindows(_TitleTMP, out _CoutWindows))
            {
               // Такое окно уже открыто, закрываем это
               Close();
            }

            // Смещаем окно программы, если уже были открыты wpfBazis и если новое окно, а не старое
            if (MyGlo.Korpus != 2 && pNewWindows && _CoutWindows > 1)
            {
                _CoutWindows -= 1;
                Top += 10*_CoutWindows;
                Left += 10*_CoutWindows;
            }

            Title = _TitleTMP;
            // Показываем имя пользователя
            PART_LabelUserName.Content = MyGlo.UserName;
            // Если Админ - закрашиваем поле имя пользователя
            if (MyGlo.Admin) PART_Border_UserName.Background = new SolidColorBrush(Colors.Orchid); 
            // Пациент
            PART_LabelPacient.Content = MyGlo.FIO + " " + MyGlo.DR;
            // Главное дерево
            MyGlo.TreeView = PART_TreeView;
            // Запускаем построение дерева
            PUB_Modul.MET_CreateTree();
            // В зависимости от модуля меняем размеры окна
            // Заполняем дерево в зависимости от модуля
            switch (MyGlo.TypeModul)
            {
                case eModul.VrPolicl:
                case eModul.VrPara:
                case eModul.KancerReg:
                    PART_Grid.ColumnDefinitions[0].Width = new GridLength(300);
                    break;
                case eModul.VrStac:
                    PART_Grid.ColumnDefinitions[0].Width = new GridLength(300);
                    break;
                case eModul.Viewer:
                    PART_Grid.ColumnDefinitions[0].Width = new GridLength(300);
                    //  Закрываем меню и меняем смену пациентов
                    if (((UserModul_Viewer) PUB_Modul).PUB_Menu == 0)
                    {
                        PART_Menu.IsEnabled = false;
                    }
                    break;
                case eModul.Laboratory:
                    PART_Grid.ColumnDefinitions[0].Width = new GridLength(300);
                    break;
                    //// Скрываем левую панель вкладок
                    //PART_Grid.ColumnDefinitions[0].MinWidth = 0;
                    //PART_Grid.ColumnDefinitions[0].Width = new GridLength(0);
                    //PART_TreeView.Visibility = Visibility.Collapsed;
                    //PART_TabOtch.Visibility = Visibility.Collapsed;
                    //// Показываем вкладку формы
                    //PART_TabForm.Visibility = Visibility.Visible;
                    //// Открываем вкладку с формой
                    //PART_TabControl.SelectedItem = PART_TabForm;
                    //// Скрываем кнопку "Закрыть" у шаблонов
                    //PART_Button_CloseSha.Visibility = Visibility.Collapsed;
                    //// Оформляем заголовок
                    //PART_TabForm.Header = "Тут наше название шаблона";
                    //// Выбранная ветка
                    //PRI_FormMyNodes = (VirtualNodes)PART_TreeView.SelectedItem;
                    //PRI_FormMyNodes.PROP_TipNodes = eTipNodes.Stac_Add;
                    //// Создаем форму
                    //PRI_FormMyNodes.MET_ShowShablon(PART_GridShablon, true);

                    //// Оформляем заголовог
                    //PART_TabForm.Header = PRI_FormMyNodes.MET_Header(PART_TabOtch, eVkladki.Table);
                    //break;
                case eModul.OtherLpu:
                    PART_Grid.ColumnDefinitions[0].Width = new GridLength(300);
                    //  Закрываем меню и меняем смену пациентов
                    PART_Menu.IsEnabled = false;
                    break;
            }
        }

        /// <summary>МЕТОД Создаем Модуль</summary>
        /// <param name="pModul">Номер типа модуля eModul</param>
        private static VirtualModul MET_CreateModul(eModul pModul)
        {
            VirtualModul _VirtualModul;
            switch (pModul)
            {
                case eModul.Admin:
                    _VirtualModul = new UserModul_VrStac();
                    break;
                case eModul.VrPolicl:
                    _VirtualModul = new UserModul_VrPolicl();
                    break;
                case eModul.VrPara:
                    _VirtualModul = new UserModul_VrParacl();
                    break;
                case eModul.VrStac:
                    _VirtualModul = new UserModul_VrStac();
                    break;
                case eModul.KancerReg:
                    _VirtualModul = new UserModul_KancerReg();
                    break;
                case eModul.Viewer:
                    _VirtualModul = new UserModul_Viewer();
                    break;
                case eModul.OtherLpu:
                case eModul.CAOP:
                    _VirtualModul = new UserModul_OtherLpu();
                    break;
                case eModul.Laboratory:
                    _VirtualModul = new UserModul_Laboratory();
                    break;
                default:
                    _VirtualModul = new UserModul_Viewer();
                    break;
            }
            MyGlo.Modul = _VirtualModul;
            return _VirtualModul;
        }
        #endregion


        #region Ошибки и Меню
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
        
        /// <summary>МЕТОД Показываем окно с Репортом (отчеты)</summary>
        /// <param name="pPath">Какой отчет отобразить</param>
        public void MET_ReportViewerLoad(string pPath = "/Menu/index")
        {
            try
            {
                ReportViewer PART_ReportViewer = new ReportViewer();
                PART_WindowsFormsHostReportViewer.Child = PART_ReportViewer;
                PART_ReportViewer.ProcessingMode = ProcessingMode.Remote;
                PART_ReportViewer.ServerReport.ReportServerCredentials.NetworkCredentials = new NetworkCredential("ReportViewer", "rptVwr2019");  // Для нового сервера отчетов

                if (MyGlo.Server == 3) // главный корпус
                    PART_ReportViewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.8/ReportServer/", UriKind.Absolute);
                else // из вне
                    PART_ReportViewer.ServerReport.ReportServerUrl = new Uri("http://10.30.103.8/ReportServer/", UriKind.Absolute);
                PART_ReportViewer.ServerReport.ReportPath = pPath;
                PART_ReportViewer.RefreshReport();

                // Показываем вкладку формы
                PART_TabReportViewer.Visibility = Visibility.Visible;
                // Открываем вкладку с формой
                PART_TabControl.SelectedItem = PART_TabReportViewer;
                // Оформляем вкладку с печатью
                PART_TabReportViewer.Header = new UserTabVrladka((BitmapImage) FindResource("mnDoc_1"), "Отчеты",
                    PART_TabReportViewer, eVkladki.Form, true, "Общие");
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>СОБЫТИЕ Выбор в меню</summary>
        private void PART_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            VirtualUserWindow _WinSpr;
            switch ((sender as MenuItem)?.Header.ToString())
            {
                case "_Отчеты":
                    // Отчеты базиса
                    MET_ReportViewerLoad();
                    break;
                case "_Диагнозы МКБ-10":
                    // Справочник диагнозов
                    _WinSpr = new UserWindow_Diag();
                    _WinSpr.Show();
                    break;
                case "_Морфологический тип":
                    // Справочник морфологических типов
                    _WinSpr = new UserWindow_MorfTip();
                    _WinSpr.Show();
                    break;
                case "_Операции":
                    // Справочник Операций
                    _WinSpr = new UserWindow_Oper(DateTime.Today);
                    _WinSpr.Show();
                    break;
                case "_Врачи стационара":
                    // Справочник врачей стационара
                    _WinSpr = new UserWindow_VrachStac();
                    _WinSpr.Show();
                    break;
                case "_Врачи поликлиники":  
                    // Справочник врачей поликлиники
                    _WinSpr = new UserWindow_VrachPol();
                    _WinSpr.Show();
                    break;
                case "_Отделения":
                    // Справочник отделений стационара
                    _WinSpr = new UserWindow_Department();
                    _WinSpr.Show();
                    break;
                case "_Талоны параклиники":
                    // Талоны параклиники
                    _WinSpr = new UserWindow_ParTalon();
                    _WinSpr.Show();
                    break;
                case "_Поиск пациентов":
                    // Талоны параклиники
                    _WinSpr = new UserWindow_FindPac();
                    _WinSpr.Show();
                    break;
                case "_Картинки шаблонов":
                    // Справочник картинок шаблонов
                    if (UserWindow_Image.MET_Access())
                    {
                        _WinSpr = new UserWindow_Image();
                        _WinSpr.Show();
                    }                    
                    break;
                case "_Методы ВМП":
                    // Справочник Методов ВМП
                    _WinSpr = new UserWindow_MetodVMP();
                    _WinSpr.Show();
                    break;
                case "_Логи wpfBazis":
                    // Логи wpfBazis
                    if (UserWindow_Log.MET_Access())
                    {
                        _WinSpr = new UserWindow_Log();
                        _WinSpr.Topmost = false;
                        _WinSpr.Show();
                    }
                    break;               
                case "_Тест":
                    // Тестовая форма
                    if (UserWindow_Test.MET_Access())
                    {
                        var _WinSpr1 = new UserWindow_Test();
                        _WinSpr1.Topmost = false;
                        _WinSpr1.Show();
                    }
                    break;
                case "_Редактор шаблонов":
                    // Редактор шаблонов
                    if (UserWindow_EditShablon.MET_Access())
                    {                        
                        _WinSpr = new UserWindow_EditShablon();
                        _WinSpr.Topmost = false;
                        _WinSpr.Show();
                    }
                    break;
                case "_О программе":
                    // О программе
                    new About().ShowDialog();
                    // Если Админ - закрашиваем поле имя пользователя
                    if (MyGlo.Admin)
                        PART_Border_UserName.Background = new SolidColorBrush(Colors.Orchid);
                    else
                        PART_Border_UserName.Background = new SolidColorBrush(Colors.White);
                    break;
            }
        }
        #endregion


        #region Ветки
        /// <summary>МЕТОД Скрываем-Открываем у отчета кнопки редактирования</summary>
        /// <param name="pVirtualNodes">Выбранная ветка</param>
        private void MET_MyNodesButton(VirtualNodes pVirtualNodes)
        {
            if (PART_TabForm.Visibility == Visibility.Collapsed)
            {
                // Скрываем/показываем у отчета кнопку редактирования
                PART_Button_Edit.IsEnabled = pVirtualNodes.PROP_shaButtonEdit && !pVirtualNodes.PROP_Delete;
                PART_Button_New.IsEnabled = pVirtualNodes.PROP_shaButtonNew;
            }
            else
            {
                // Скрываем у отчета кнопки редактирования
                PART_Button_Edit.IsEnabled = false;
                PART_Button_New.IsEnabled = false;

                // Скрываем/показываем кнопки сохраниния, отчистки
                PART_Button_SaveSha.Visibility = pVirtualNodes.PROP_shaButtonSvaveSha;
                PART_Button_ClearSha.Visibility = pVirtualNodes.PROP_shaButtonClearSha;
            }
        }
                                  
        /// <summary>СОБЫТИЕ при выборе элемента дерева</summary> 
        private void PART_TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {   
            // Выбранная ветка
            VirtualNodes _VirtualNodes = (VirtualNodes)PART_TreeView.SelectedItem;
            if (_VirtualNodes != null)
            {
                // Если выбрали PDF элемент
                if (_VirtualNodes is UserNodes_AddPdf)
                {
                    // Загружаем PDF файл и открываем вкладку с ним
                    MET_LoadPdfFileFromServer(_VirtualNodes);
                    return;
                }

                // Открываем 1ю вкладку с отчетом
                PART_TabControl.SelectedItem = PART_TabOtch;

                // Оформляем закладку отчета
                PART_TabOtch.Header = _VirtualNodes.MET_Header(PART_TabOtch, eVkladki.Otchet);
                PART_FDoc.Blocks.Clear();
                // Сбрасываем глобальный фон
                MyGlo.BrushesOtchet = Brushes.White;
                // Создаем отчет
                PART_FDoc.Blocks.Add(_VirtualNodes.PROP_Docum.PROP_Otchet.MET_Inizial(_VirtualNodes));
                // Обнуляем полосу сдвига текста по листу
                PART_Slider.Value = 0;
                // Скрываем-Открываем у отчета кнопку редактирования 
                MET_MyNodesButton(_VirtualNodes);
            }
        }
        #endregion


        #region Контекстное меню
        /// <summary>СОБЫТИЕ Открываем контекстного меню</summary>
        private void PART_ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu _ContextMenu = (ContextMenu)sender;                     // контекстное меню
            _ContextMenu.FontSize = 13;
            _ContextMenu.Items.Clear();                                         // чистим список контекстного меню

            if (!((MyGlo.ContextMenu.PlacementTarget as FrameworkElement)?.Tag is VirtualPole _VirtualPole)) return;

            switch (_VirtualPole.PROP_Type)
            {
                case eVopros.Text:
                    MET_ContextMenu_TextBox(_ContextMenu);
                    break;
                case eVopros.Sprav:
                    MET_ContextMenu_Sprav(_ContextMenu);
                    MET_ContextMenu_EditShablon(_ContextMenu);
                    break;
                default:
                    MET_ContextMenu_EditShablon(_ContextMenu);
                    break;
            }
        }

        /// <summary>СОБЫТИЕ Открываем контекстного меню, в поле TextBox</summary>
        /// <param name="pContextMenu">Контекстное меню</param>
        private void MET_ContextMenu_TextBox(ContextMenu pContextMenu)
        {
            
            MenuItem _ConMenuItem;                                              // элементы меню
            Separator _Separator;                                               // сепаратор

            TextBox _TextBox = pContextMenu.PlacementTarget as TextBox;
            DataRow[] _mRow;                                                    // записи шаблона
            int _Count;                                                         // всего ответов
            VirtualPole _VirtualPole = (pContextMenu.PlacementTarget as Control).Tag as VirtualPole;
            var d = _VirtualPole.PROP_Docum?.PROP_ListShablon.PROP_TipProtokol;
            // Если поле вне шаблона или VarId = 0 то перескакиваем
            if (PRI_FormMyNodes == null || _VirtualPole.PROP_VarId == 0) goto Label; 

            string _NameList = "astList";
            switch (PRI_FormMyNodes.PROP_TipNodes)
            {
                // Шаблоны стационара
                case eTipNodes.Stac_Edit:
                case eTipNodes.Stac_Add:
                case eTipNodes.Stac_RootsList:
                    _NameList = "astList";
                    break;
				// Шаблоны параклиники
                case eTipNodes.Para_RootsList:
                case eTipNodes.Para_Add:
                    _NameList = "parList";
                    break;
				// Шаблоны поликлиники
				case eTipNodes.Pol_RootsList:
				case eTipNodes.Pol_Add:
					_NameList = "apaNList";
					break;
                // Шаблоны КДЛ
                case eTipNodes.Kdl_RootsList:
                case eTipNodes.Kdl_Add:                
                    _NameList = "kdlList";
                    break;
                // Шаблоны документов (лист назначения)
                case eTipNodes.EditDocum:
                    _NameList = "s_ListDocum";
                    break;
            }

            // Заполняем строку данными запроса         
            MySql.MET_DsAdapterFill(MyQuery.MET_List_Select_4(_NameList, _VirtualPole.PROP_Shablon, _VirtualPole.PROP_VarId), "List");
           
            // Всего ответов  
            _Count = MyGlo.DataSet.Tables["List"].Rows.Count;
            _mRow = new DataRow[_Count];
            // Заполняем все варианты ответов
            for (int i = 0; i < _Count; i++)
            {
                _mRow[i] = MyGlo.DataSet.Tables["List"].Rows[i];
                string _Str = _mRow[i]["Value"].ToString();
                // Создаем элемент меню
                TextBlock _TextBlock = new TextBlock
                {
                    MaxWidth = 600,
                    Background = ((i%10)&1) == 1 ? Brushes.Azure : Brushes.FloralWhite,
                    Foreground = Brushes.Black,                                 // почему то на (ХР стандартный ситль) ставит белый цвет, для выделенного текста
                    TextWrapping = TextWrapping.Wrap,
                    Text = _Str
                };
                _ConMenuItem = new MenuItem();
                _ConMenuItem.Header = _TextBlock;
                _ConMenuItem.Tag = _mRow[i]["ValueCod"].ToString();
                _ConMenuItem.Click += PART_ConMenuItem_TextBox_Click;
                pContextMenu.Items.Add(_ConMenuItem);
            } 
            // Если ответов нету то ставим разделитель
            if (_Count > 0)
            {
                _Separator = new Separator();
                pContextMenu.Items.Add(_Separator);
            }
            // Создаем пункт меню "Добавить"
            _ConMenuItem = new MenuItem();
            _ConMenuItem.Header = "Добавить";
            _ConMenuItem.Click += PART_ConMenuItem_TextBox_Click;
            pContextMenu.Items.Add(_ConMenuItem);
            if (_TextBox?.Text.Length == 0)
                _ConMenuItem.IsEnabled = false;
            // Создаем пункт меню "Изменить"
            _ConMenuItem = new MenuItem();
            _ConMenuItem.Header = "Изменить";
            _ConMenuItem.Click += PART_ConMenuItem_TextBox_Click;
            pContextMenu.Items.Add(_ConMenuItem);
            if (_Count == 0)
                _ConMenuItem.IsEnabled = false;
            // Форма Редактор Шаблона (только для админа)
            if (_VirtualPole.PROP_FormShablon != null && _VirtualPole.PROP_VarId > 0)
                MET_ContextMenu_EditShablon(pContextMenu);
            // Разделитель
            _Separator = new Separator();
            pContextMenu.Items.Add(_Separator);
        Label:
            // Создаем пункт меню "Копировать"
            _ConMenuItem = new MenuItem();
            _ConMenuItem.Command = ApplicationCommands.Copy;
            pContextMenu.Items.Add(_ConMenuItem);
            // Создаем пункт меню "Вырезать"
            _ConMenuItem = new MenuItem();
            _ConMenuItem.Command = ApplicationCommands.Cut;
            pContextMenu.Items.Add(_ConMenuItem);
            // Создаем пункт меню "Вставить"
            _ConMenuItem = new MenuItem();
            _ConMenuItem.Command = ApplicationCommands.Paste;
            pContextMenu.Items.Add(_ConMenuItem);
        }

        /// <summary>СОБЫТИЕ Вставляем выбранное значение из контектсного меню</summary>
        private void PART_ConMenuItem_TextBox_Click(object sender, EventArgs e)
        {
            // Выбранный элемент меню
            MenuItem _Item = sender as MenuItem;
            string _NameList = "astList";
            switch (PRI_FormMyNodes.PROP_TipNodes)
            {
                // Шаблоны стационара
                case eTipNodes.Stac_Edit:
                case eTipNodes.Stac_Add:
                    _NameList = "astList";
                    break;
                // Шаблоны поликлиники
                case eTipNodes.Pol_RootsList:
                case eTipNodes.Pol_Add:
                    _NameList = "apaNList";
                    break;
                // Шаблоны документов
                case eTipNodes.Para_RootsList:
                case eTipNodes.Para_Add:
                    _NameList = "parList";
                    break;
                // Шаблоны КДЛ
                case eTipNodes.Kdl_RootsList:
                case eTipNodes.Kdl_Add:
                    _NameList = "kdlList";
                    break;
                // Шаблоны документов (лист назначения)
                case eTipNodes.EditDocum:
                    _NameList = "s_ListDocum";
                    break;
            }

            TextBox _TextBox = MyGlo.ContextMenu.PlacementTarget as TextBox;    // текущий компонент
            VirtualPole _VirtualPole = (_TextBox?.Parent as BulletDecorator)?.Parent as VirtualPole;
            string _Text = _Item?.Header is TextBlock ? ((TextBlock)_Item.Header).Text : _Item?.Header.ToString();
            int _NomerShablon = _VirtualPole.PROP_Shablon;
            int _VarID = _VirtualPole.PROP_VarId;
          
            switch (_Text)
            {
                case "Добавить":
                    // Находим максимальный код
                    int _Cod = MySql.MET_QueryInt(MyQuery.MET_List_MaxCod_Select_2(_NameList, _NomerShablon)) + 1;
                    if (_Cod == 1)
                        _Cod = _NomerShablon * 1000 + 1; // если это первый ответ в шаблоне то начинаем с нужного номера

                    // Проверяем на наличие повторов
                    // Если такого ответа ещё нету, то записываем его в базу
                    if (MySql.MET_QueryBool(MyQuery.MET_List_Select_3(_NameList, _NomerShablon, _VarID, _TextBox.Text)))
                        MessageBox.Show("Ошибка! Данный ответ уже есть в списке ответов");
                    else
                    {
                        MySql.MET_QueryNo(MyQuery.MET_List_Insert_1(_NameList, _Cod, _NomerShablon, _VarID, _TextBox.Text));
                        MessageBox.Show("Новый ответ, успешно добавлен!");
                    }
                    break;
                case "Изменить":
                    // Открываем справочник ответов
                    UserWindow_List _WinSpr = new UserWindow_List(_NameList, _NomerShablon, _VarID)
                    {
                        Title = "Справочник ответов на вопрос:",
                        PROP_Modal = true,
                        WindowStyle = WindowStyle.ToolWindow
                    };
                    _WinSpr.ShowDialog();
                    break;
                default:
                    // Если Вставляем только код
                    if (_VirtualPole.PROP_ValueCod)
                        _Text = _Item.Tag.ToString();
                    if (_VirtualPole.PROP_Insert)
                    {
                        if (_Text != null) _TextBox.Text = _Text;
                    }
                    else
                        _TextBox.SelectedText = _Text;
                    break;
            }
        }

        /// <summary>СОБЫТИЕ Добавляем в контекстное меню пункт - Редактор Шаблона</summary>
        /// <param name="pContextMenu">Контекстное меню</param>
        private void MET_ContextMenu_EditShablon(ContextMenu pContextMenu)
        {
            // Форма Редактор Шаблона (только для админа)
            if (MyGlo.Admin)
            {
                if ((MyGlo.ContextMenu.PlacementTarget as FrameworkElement)?.Tag is VirtualPole _VirtualPole && _VirtualPole.PROP_VarId > 0)             
                {
                    // Разделитель
                    if (pContextMenu.Items.Count > 0)
                    {
                        Separator _Separator = new Separator();
                        pContextMenu.Items.Add(_Separator);
                    }

                    MenuItem _ConMenuItem = new MenuItem();
                    _ConMenuItem.Header = "Редактор Шаблона";
                    _ConMenuItem.Click += PART_ConMenuItem_EditShablon_Click;
                    pContextMenu.Items.Add(_ConMenuItem);
                }
            }
        }

        /// <summary>СОБЫТИЕ Вставляем выбранное значение из контектсного меню</summary>
        private void PART_ConMenuItem_EditShablon_Click(object sender, EventArgs e)
        {
            VirtualPole _VirtualPole = (MyGlo.ContextMenu.PlacementTarget as FrameworkElement)?.Tag as VirtualPole;

            if(_VirtualPole?.PROP_Docum.PROP_Shablon != null)
            {
                UserWindow_Lua _WinLua = new UserWindow_Lua(_VirtualPole)
                {
                    WindowStyle = WindowStyle.ToolWindow
                };
                _WinLua.Show();
            }
        }

        /// <summary>СОБЫТИЕ Открываем контекстное меню, в поле Sprav</summary>
        /// <param name="pContextMenu">Контекстное меню</param>
        private void MET_ContextMenu_Sprav(ContextMenu pContextMenu)
        {
            // Создаем пункт меню "Очистить поле"
            MenuItem _ConMenuItem = new MenuItem();
            _ConMenuItem.Header = "Очистить поле";
            _ConMenuItem.Click += PART_ConMenuItem_Sprav_Click;
            pContextMenu.Items.Add(_ConMenuItem);
        }

        /// <summary>СОБЫТИЕ Отчищаем поле Sprav</summary>
        private void PART_ConMenuItem_Sprav_Click(object sender, EventArgs e)
        {
            VirtualPole _VirtualPole = (MyGlo.ContextMenu.PlacementTarget as FrameworkElement)?.Tag as VirtualPole;
            // Чистим текст
            _VirtualPole.PROP_Text = "";
            // Активируем кнопку "Сохранить"
            MyGlo.callbackEvent_sEditShablon(true);
        }
        #endregion


        #region Выход
        /// <summary>СОБЫТИЕ Выход из программы</summary>
        private void PART_ComandClose_Executed(object sender, ExecutedRoutedEventArgs e)
        {                  
            Close();
        }

        /// <summary>СОБЫТИЕ при закрытии программы</summary> 
        private void Form_Closing(object sender, CancelEventArgs e)
        {
            // Запрос на сохранение протокола
            if (PART_TabForm.IsVisible && PART_Button_SaveSha.IsEnabled)
            {
                switch (MessageBox.Show("Сохранить изменения в протоколе?", "Внимание!", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                    case MessageBoxResult.Yes:
                        // Сохраняемся
                        if (!MET_SaveShablon())
                            e.Cancel = true;
                        break;
                }
            }
        }
        #endregion


        #region Язык
        /// <summary>СОБЫТИЕ при смене языка</summary> 
        private void Current_InputLanguageChanged(Object sender, InputLanguageEventArgs e)
        {
            if (InputLanguageManager.Current.CurrentInputLanguage.ToString() == "en-US")
            {
                ((Image)FindResource("ImageLang")).Source = (BitmapImage)FindResource("btsUs");
                ((Label)FindResource("LabelLang")).Content = "Английский";
                ((Label)FindResource("LabelLang")).Tag = true;
            }
            else
            {
                ((Image)FindResource("ImageLang")).Source = (BitmapImage)FindResource("btsRu");
                ((Label)FindResource("LabelLang")).Content = "Русский";
                ((Label)FindResource("LabelLang")).Tag = false;
            }
        }

        /// <summary>СОБЫТИЕ при нажатии на кнопку "Смена языка"</summary> 
        private void PART_Button_Language_Click(object sender, RoutedEventArgs e)
        {
            // Если язык Английский, то ставим Русский и наоборот
            if (InputLanguageManager.Current.CurrentInputLanguage.Equals(System.Globalization.CultureInfo.CreateSpecificCulture("en-US")))
            {
                InputLanguageManager.Current.CurrentInputLanguage = System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU");
            }
            else
            {
                InputLanguageManager.Current.CurrentInputLanguage = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
            }
        }
        #endregion


        #region Печать
        /// <summary>СОБЫТИЕ при нажатии на кнопку "Просмотр печати" из отчета</summary> 
        private void PART_Button_Preview_Click(object sender, RoutedEventArgs e)
        {
            // Выбранная ветка
            VirtualNodes _SelectNodes = (VirtualNodes)PART_TreeView.SelectedItem;
            // А если ветки то и нету, то сваливаем
            if (_SelectNodes == null) return;
            // Выводим на просмотр печати
            MET_PreviewPrint(_SelectNodes);            
        }

        /// <summary>СОБЫТИЕ при нажатии на кнопку "Просмотр печати" из шабллона</summary> 
        private void PART_Button_PrewSha_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняемся
            if (MET_SaveShablon())
            {
                PRI_FormMyNodes.IsSelected = true;
                // Выводим на просмотр печати
                MET_PreviewPrint(PRI_FormMyNodes);  
            }
        }

        /// <summary>МЕТОД "Просмотр печати"</summary> 
        /// <param name="pVirtualNodes">Выбранная ветка</param>
        private void MET_PreviewPrint(VirtualNodes pVirtualNodes)
        {
            // Создаем объект печати
            if (!pVirtualNodes.PROP_Docum.PROP_Otchet.MET_CreatePrint(PART_DocViewer, PART_FDoc))
                return;
            // Показываем вкладку просмотра печати
            PART_TabPrint.Visibility = Visibility.Visible;
            // Оформляем вкладку с печатью
            PART_TabPrint.Header = pVirtualNodes.MET_Header(PART_TabPrint, eVkladki.Print, true);
            // Открываем вкладку с печатью
            PART_TabControl.SelectedItem = PART_TabPrint;
        }

        /// <summary>СОБЫТИЕ при нажатии на кнопку Быстрая "Печать" из шабллона</summary>
        private void PART_Button_PrintSha_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняемся
            if (MET_SaveShablon())
            {
                PRI_FormMyNodes.IsSelected = true;
                // Печатаем
                PRI_FormMyNodes.PROP_Docum.PROP_Otchet.MET_CreatePrint(PART_DocViewer, PART_FDoc, true);
            }
        }

        /// <summary>СОБЫТИЕ при нажатии на кнопку Быстрая "Печать" из отчетов</summary>
        private void PART_Button_PrintOtch_Click(object sender, RoutedEventArgs e)
        {
            // Выбранная ветка
            VirtualNodes _SelectNodes = (VirtualNodes)PART_TreeView.SelectedItem;
            // Печатаем (если есть ветка)
            _SelectNodes?.PROP_Docum.PROP_Otchet.MET_CreatePrint(PART_DocViewer, PART_FDoc, true);
        }
        #endregion   


        #region Шаблоны
        /// <summary>МЕТОД Закрытие вкладки Формы </summary>
        /// <param name="pTabItem">Вкладку которую закрываем</param>         
        public void MET_Delg_Close(TabItem pTabItem)
        {
            // Запрос на сохранение протокола
            if (Equals(pTabItem, PART_TabForm) && PART_Button_SaveSha.IsEnabled)
            {
                switch (MessageBox.Show("Сохранить изменения в протоколе?", "Внимание!", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Cancel:
                        return;
                    case MessageBoxResult.Yes:
                        // Сохраняемся
                        if (!MET_SaveShablon())
                            return;
                        break;
                }
            }
            // Скрываем вкладку формы
            pTabItem.Visibility = Visibility.Collapsed;
            // Открываем вкладку с отчетом
            PART_TabControl.SelectedItem = PART_TabOtch;
            if (Equals(pTabItem, PART_TabForm))
            {
                // Снимаем выделения редактируемой ветки и обнуляем документ
                PRI_FormMyNodes.Background = Brushes.White;
                PRI_FormMyNodes.PROP_Docum.PROP_FormShablon = null;
                switch (PRI_FormMyNodes.PROP_TipNodes)
                {
                    case eTipNodes.Stac_RootsList:
                    case eTipNodes.Para_RootsList:
                    case eTipNodes.Pol_RootsList:
                    case eTipNodes.Kdl_RootsList:
                        PRI_FormMyNodes.PROP_Docum.PROP_ListShablon = null;
                        PRI_FormMyNodes.PROP_Docum.PROP_Shablon = null;
                        PRI_FormMyNodes.PROP_shaNomerShablon = 0;
                        break;
                }      
                // Скрываем-Открываем у отчета кнопку редактирования 
                MET_MyNodesButton((VirtualNodes)PART_TreeView.SelectedItem);
            }
        }

        /// <summary>МЕТОД Нужно ли сохранять шаблон </summary>
        /// <param name="pEnable">true - шаблон сохранен, false - шаблон не сохранен</param> 
        public void MET_Delg_EditShablon(bool pEnable)
        {
            // Отображаем либо скрываем кнопку "Сохранения формы"
            PART_Button_SaveSha.IsEnabled = pEnable;
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Создать шаблон"</summary>
        private void PART_Button_New_Click(object sender, RoutedEventArgs e)
        {
            string _Text = "";
            int _NomerShablon = 0;

            // Выбранная ветка
            PRI_FormMyNodes = (VirtualNodes)PART_TreeView.SelectedItem;
            // А если ветки то и нету, то сваливаем
            if (PRI_FormMyNodes == null) return;
            // Окно выбора шаблона
            VirtualWindow_Shablon _WinSpr = null;
            // По умолчанию отображаем панель ToolBar
            PART_ToolBarShablon.Visibility = Visibility.Visible;

            // Если нужно вызывать окно выбора шаблона
            switch (PRI_FormMyNodes.Name)
            {
                // Только шаблоны Лечения
                case "eleTVItem_Oper":            
                    _WinSpr = new UserWindow_Shablon_Stac("Шаблоны лечения:", 102, 109);
                    break;
                // Только шаблоны Анестезиолога
                case "eleTVItem_Anest":
                    _WinSpr = new UserWindow_Shablon_Stac("Шаблоны Анестезиолога:", 130, 139);
                    break;  
                // Только шаблоны Обходов, консилиумов
                case "eleTVItem_Obhod":
                    _WinSpr = new UserWindow_Shablon_Stac("Обходы и консилиумы:", 110, 119);
                    break;
                // Только шаблоны Документы
                case "eleTVItem_Dokum":
                    _WinSpr = new UserWindow_Shablon_Stac("Документы:", 140, 160);
                    break;   
                // Только шаблоны Выписки
                case "eleTVItem_Extact":
                    _WinSpr = new UserWindow_Shablon_Stac("Выписные документы:", 120, 129, true);
                    break;

                // Только шаблоны Поликлиники
                case "eleTVItem_Pol":
                    _WinSpr = new UserWindow_Shablon_Policl("Шаблоны Поликлиники:", MyGlo.Otd);
                    break;	
							 
                // Только для Параклиники
                case "eleTVItem_ParaIss":
                    // Смотрим есть ли заполненный протокол для этого обследования
                    bool _Flag = MySql.MET_QueryBool(MyQuery.MET_parProtokol_Select_2(MyGlo.IND));
                    _WinSpr = new UserWindow_Shablon_Paracl(_Flag);                 
                    break;

                // Только для общих докуметов (kdl)
                case "eleTVItem_ObDocum":
                    _WinSpr = new UserWindow_Shablon_Kdl("Шаблоны Справок:", "\\StacKdl");
                    break;

                // Только других ЛПУ (kdl)
                case "eleTVItem_OtherLpu":
                    _WinSpr = new UserWindow_Shablon_Kdl("Шаблоны документов:", "\\OtherLpu");
                    break;

                // Для исследований пока только на CODVID 19 (kdl)
                case "eleTVItem_Laboratory":
                    _WinSpr = new UserWindow_Shablon_Kdl("Шаблоны исследований:", "\\Issled");
                    break;

                // Для документов PDF (kdl)
                case "eleTVItem_Pdf":
                    // Скрываем панель ToolBar, так как у нас будт своя панель
                    PART_ToolBarShablon.Visibility = Visibility.Collapsed;
                    //MET_SavePdfFileOnServer(PRI_FormMyNodes);
                    //int _Max = ((VirtualModul)MyGlo.Modul).PUB_Protokol.Any() ? ((VirtualModul)MyGlo.Modul).PUB_Protokol.Max(p => p.PROP_pIndex) : 0;
                    //PRI_FormMyNodes.PROP_shaIndex = ++_Max;
                    //PRI_FormMyNodes.PROP_shaNomerShablon = 2000;
                    //// Создаем форму
                    //PRI_FormMyNodes.MET_ShowShablon(PART_GridShablon, true, PRI_FormMyNodes.PROP_shaNomerShablon, "Документ PDF");
                    //return;
                    break;

                // Иначе (если, сама ветка радактируется, то идем дальше, а если не редактируется, то выходим)
                default:
                    if (PRI_FormMyNodes.PROP_TipNodes == eTipNodes.EditDocum ||
                        PRI_FormMyNodes.PROP_TipNodes == eTipNodes.Stac_Edit)
                        break;
                    return;
            }  
            // Если выбрали какое нужно окно выбора шаблона
            if (_WinSpr != null)
            {
                _WinSpr.PROP_Modal = true;
                _WinSpr.WindowStyle = WindowStyle.ToolWindow;
                _WinSpr.ShowDialog();
                if (_WinSpr.PROP_Return)
                {

                    int _Max = ((VirtualModul) MyGlo.Modul).PUB_Protokol.Any() ? ((VirtualModul)MyGlo.Modul).PUB_Protokol.Max(p => p.PROP_pIndex) : 0;
                    
                    PRI_FormMyNodes.PROP_shaIndex = ++_Max; 
                    PRI_FormMyNodes.PROP_shaNomerShablon = _WinSpr.PUB_Shablon;
                    _Text = _WinSpr.PUB_Text;
                    _NomerShablon = _WinSpr.PUB_Shablon;
                }
                else
                    return;
            }
            // Создаем форму
            if (PRI_FormMyNodes.MET_ShowShablon(PART_GridShablon, true, _NomerShablon, _Text))
            {
                // Показываем вкладку формы
                PART_TabForm.Visibility = Visibility.Visible;
                // Открываем вкладку с формой
                PART_TabControl.SelectedItem = PART_TabForm;
                // Оформляем вкладку с печатью
                PART_TabForm.Header = PRI_FormMyNodes.MET_Header(PART_TabForm, eVkladki.Form, true);
                // Создаем форму
                PRI_FormMyNodes.MET_ShowShablon(PART_GridShablon, true, _NomerShablon, _Text);
                // Скрываем-Открываем у отчета кнопку редактирования 
                MET_MyNodesButton(PRI_FormMyNodes);
            }
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Обнулить форму"</summary>
        private void PART_Button_ClearSha_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("\n   Внимание, все данные обнуляться!\n    Вы точно хотите очистить шаблон?\n\n",
                                "Внимание!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;
            PRI_FormMyNodes.PROP_Docum.PROP_FormShablon.MET_Clear();
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Закрыть шаблон"</summary>
        private void PART_Button_CloseSha_Click(object sender, RoutedEventArgs e)
        {
            MET_Delg_Close(PART_TabForm);
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Редактировать шаблон"</summary>
        private void PART_Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Выбранная ветка
                PRI_FormMyNodes = (VirtualNodes)PART_TreeView.SelectedItem;
                // А если ветки то и нету, то сваливаем
                if (PRI_FormMyNodes == null) return;
                // Показываем вкладку формы
                PART_TabForm.Visibility = Visibility.Visible;
                // Открываем вкладку с формой
                PART_TabControl.SelectedItem = PART_TabForm;   
                // Оформляем закладку формы
                PART_TabForm.Header = PRI_FormMyNodes.MET_Header(PART_TabForm, eVkladki.Form, true); 
                // Создаем форму
                PRI_FormMyNodes.MET_ShowShablon(PART_GridShablon, false);           
                // Скрываем-Открываем у отчета кнопку редактирования 
                MET_MyNodesButton(PRI_FormMyNodes);
            }
            catch (Exception ex)
            {
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Редактирования шаблона");
                MyGlo.callbackEvent_sError(ex);
            }
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Сохранить шаблон"</summary>
        private void PART_Button_SaveSha_Click(object sender, RoutedEventArgs e)
        {
            // Сохраняемся
            if (MET_SaveShablon())
                MessageBox.Show("Шаблон успешно сохранен!");
            
        }

        /// <summary>МЕТОД "Сохранить шаблон"</summary>
        private bool MET_SaveShablon()
        {
            // Провека на правильность заполнения
            if (!PRI_FormMyNodes.PROP_Docum.PROP_FormShablon.MET_Verification())
                return false;   // проверка не прошла и выходим без сохранения
            
            // Сохраняем шаблон
            if (!PRI_FormMyNodes.PROP_Docum.PROP_FormShablon.MET_Save())
                return false;
            
            // Если нужно добавляем вкладки
            switch (PRI_FormMyNodes.PROP_TipNodes)
            {
                case eTipNodes.Stac_RootsList:
                case eTipNodes.Para_RootsList:
                case eTipNodes.Pol_RootsList:
                case eTipNodes.Kdl_RootsList:
                    PRI_FormMyNodes.PROP_TextDown = "";
                    VirtualNodes _VirtualNodes = (PRI_FormMyNodes as VirtualNodes_RootsList).MET_CreateNodesAdd();
                    PRI_FormMyNodes = _VirtualNodes;
                    PRI_FormMyNodes.PROP_Docum.PROP_FormShablon.PUB_VirtualNodes = _VirtualNodes;
                    PRI_FormMyNodes.Background = Brushes.LightGreen;                            // показываем, что данную ветку редактируем
                    break;
                case eTipNodes.Stac_Add:
                    break;
            }

            PART_TabForm.Header = PRI_FormMyNodes.MET_Header(PART_TabForm, eVkladki.Form, true);
           // PRI_FormMyNodes.PROP_TextDown = PRI_FormMyNodes.PROP_TextDown;

            // Пробегаемся по всем родительским веткам и говорим им обновить отчеты
            MET_CreateOtchet(PRI_FormMyNodes);

            return true;
        }

        /// <summary>СОБЫТИЕ Потеря фокуса вкладки шаблона, запоминаем фокус полседнего поля, что бы потом к нему вернуться</summary>
        private void PART_TabForm_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {   
            if (PRI_FormMyNodes.PROP_Docum.PROP_FormShablon != null)
                PUB_FocusUI = FocusManager.GetFocusedElement(PART_GridShablon);
        }
        #endregion


        #region Отчеты/смена пациента
        /// <summary>МЕТОД Генерируем отчет</summary>
        /// <param name="pVirtualNodes">Выбранная ветка</param>
        private void MET_CreateOtchet(VirtualNodes pVirtualNodes)
        {
            if (pVirtualNodes.PROP_Parent != null)
            {
                if (pVirtualNodes.PROP_Parent?.PROP_Docum.PROP_Otchet != null)
                    pVirtualNodes.PROP_Parent.PROP_Docum.PROP_Otchet.PROP_NewCreate = true;
                MET_CreateOtchet(pVirtualNodes.PROP_Parent);
            }
        }
                    
        /// <summary>СОБЫТИЕ Нажали на кнопку "Смена пациента"</summary>
        private void PART_Button_SelectPac_Click(object sender, RoutedEventArgs e)
        {
            VirtualUserWindow _WinSpr;
            switch (MyGlo.TypeModul)
            {
                case eModul.VrPolicl:                
                    // Открываем список пациентов поликлиники
					_WinSpr = new UserWindow_PolApac();                    
                    break;
                case eModul.VrStac:
                    // Открываем список стационара
                    _WinSpr = new UserWindow_APSTAC();
                    break;
                case eModul.VrPara:
                    // Открываем список пациентов параклиники
                    _WinSpr = new UserWindow_ParObsledov();                   
                    break;
                case eModul.KancerReg:
                    // Открываем список из канцер регистра
                    _WinSpr = new UserWindow_KancerReg();                   
                    break;
                case eModul.OtherLpu:
                case eModul.CAOP:
                    // Открываем список Направлений в текущее ЛПУ
                    _WinSpr = new UserWindow_OtherLpu();
                    break;
                case eModul.Laboratory:
                    // Открываем список Лаборатории
                    _WinSpr = new UserWindow_Laboratory();
                    break;
                default:
                    if (((UserModul_Viewer)PUB_Modul).PUB_Menu == 0)
                        _WinSpr = new UserWindow_KbolLPU();
                    else
                        _WinSpr = new UserWindow_Kbol();
                    break;   
            }

            _WinSpr.PROP_Modal = true;
            _WinSpr.WindowStyle = WindowStyle.ToolWindow;
            _WinSpr.ShowDialog();
            if (_WinSpr.PROP_Return)
            {
                PART_TabForm.Visibility = Visibility.Collapsed;
                PART_TabPrint.Visibility = Visibility.Collapsed;
                // Запуск программы
                MyGlo.callbackEvent_sReloadWindows?.Invoke(false);
            }
        }
         
        /// <summary>СОБЫТИЕ Перемещение ползунка сдвига отчета</summary>
        private void PART_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VirtualNodes _VirtualNodes = (VirtualNodes)PART_TreeView.SelectedItem;
            _VirtualNodes.PROP_Docum.PROP_Otchet.PROP_Otstup = PART_Slider.Value * 37.8;
        }

        /// <summary>СОБЫТИЕ Переключение на вкладку отчета/шаблона и обновление его (если нужно)</summary>
        private void PART_TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // При переключении на вкладку Шаблона, ставим фокус на редактируемое поле (если такое было)
            if (PART_TabControl.SelectedItem.Equals(PART_TabForm) && PUB_FocusUI != null)
                Dispatcher.BeginInvoke(new Action(() => { PUB_FocusUI.Focus(); }));

            // Выбрана ли вкладка отчета 
            if (PART_TabControl.SelectedItem.Equals(PART_TabOtch))
            {
                // Выбранная ветка
                VirtualNodes _VirtualNodes = (VirtualNodes)PART_TreeView.SelectedItem;
                // Обновляем отчет, только если его меняли
                if (_VirtualNodes != null && _VirtualNodes.PROP_Docum.PROP_Otchet.PROP_NewCreate)
                {
                    // Оформляем закладку отчета
                    PART_TabOtch.Header = _VirtualNodes.MET_Header(PART_TabOtch, eVkladki.Otchet);
                    PART_FDoc.Blocks.Clear();
                    // Сбрасываем глобальный фон
                    MyGlo.BrushesOtchet = Brushes.White;
                    // Создаем отчет
                    PART_FDoc.Blocks.Add(_VirtualNodes.PROP_Docum.PROP_Otchet.MET_Inizial(_VirtualNodes));
                    // Обнуляем полосу сдига текста по листу
                    PART_Slider.Value = 0;
                    // Скрываем-Открываем у отчета кнопку редактирования 
                    MET_MyNodesButton(_VirtualNodes);
                }
            }
        }
        #endregion

        #region PDF Файлы
        /// <summary>Веб клиент</summary>
        private WebClient PRI_WebClient;

        /// <summary>МЕТОД Загрузка PDF файла с сервера</summary>
        private void MET_LoadPdfFileFromServer(VirtualNodes pNode)
        {
            string _Protokol = pNode.PROP_Docum?.PROP_Protokol?.PROP_Protokol;
            if (string.IsNullOrEmpty(_Protokol))
            {
                MessageBox.Show("Протокол не найден!");
                return;
            }
            string _Text = MyMet.MET_GetPole(2, _Protokol);
           // string _Text = "9678edca819ef5c8a8970b6841e64d0f.pdf";
            PRI_WebClient = new WebClient();
            // URI сервера, функции и имя файла с расширением который необходимо скачать
            var _uri = new Uri("http://192.168.0.6:81/api/Storage/Download/" + _Text);
            try
            {
                // Заголовок с токеном для аутентификации в функции загрузки
                PRI_WebClient.Headers.Add("auth-key", "wpfBazisDownloadAndUploadFileWebApi20201014");
                // Событие по завершению загрузки
                PRI_WebClient.DownloadDataCompleted += MET_DownloadDataCompleted;
                // Асинхронная загрузка файла
                PRI_WebClient.DownloadDataAsync(_uri);

                // Оформляем закладку отчета
                PART_TabPdfViewer.Header = pNode.MET_Header(PART_TabPdfViewer, eVkladki.PDF, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>МЕТОД Завершение скачивание PDF файла с сервера</summary>
        private void MET_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {           
            try
            {
                // Загружаем в поток рисунок
                MemoryStream _Stream = new MemoryStream(e.Result);
                pdfViewer.DocumentSource = new PdfDocumentSource(_Stream);

                // Открываем 1ю вкладку с отчетом
                PART_TabControl.SelectedItem = PART_TabPdfViewer;               
                // Показываем вкладку формы
                PART_TabPdfViewer.Visibility = Visibility.Visible;
                // Открываем вкладку с формой
                PART_TabControl.SelectedItem = PART_TabPdfViewer;               
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}/n{ex.InnerException}");
            }
        }

        /// <summary>МЕТОД Сохраняем PDF файл на сервер</summary>
        private void MET_SavePdfFileOnServer(VirtualNodes pNode)
        {
            OpenFileDialog _OpenFileDialog = new OpenFileDialog();
            _OpenFileDialog.Filter = "pdf files (*.pdf)|*.pdf";

            _OpenFileDialog.ShowDialog();
                       
            // Алгоритм нахождения MD5 
            // Просто отображает Хэш на экране
            FileStream stream = File.OpenRead(_OpenFileDialog.FileName);

            MD5 _MD5 = new MD5CryptoServiceProvider();
            byte[] _Hashenc = _MD5.ComputeHash(stream);
            string _Result = "";

            foreach (var _b in _Hashenc)
            {
                _Result += _b.ToString("x2");
            }

            PRI_WebClient = new WebClient();
            // Завершение загрузки
            PRI_WebClient.UploadFileCompleted += new UploadFileCompletedEventHandler(MET_Completed);
            // Прогресс загрузки
            PRI_WebClient.UploadProgressChanged += new UploadProgressChangedEventHandler(MET_ProgressChanged);
            // URI сервера и api
            var uri = new Uri("http://192.168.0.6:81/api/Storage/UploadFile");
            try
            {                
                // Заголовок с токеном для аутентификации в функции загрузки
                PRI_WebClient.Headers.Add("auth-key", "wpfBazisDownloadAndUploadFileWebApi20201014");               
                // Асинхронная загрузка файла
                PRI_WebClient.UploadFileAsync(uri, _OpenFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }  
        }

        /// <summary>МЕТОД Завершение сохранения PDF файла на сервер</summary>
        private void MET_Completed(object sender, UploadFileCompletedEventArgs e)
        {
            if (e?.Error is WebException)
            {
                var _Response = (HttpWebResponse)((WebException)e.Error).Response;
                switch (_Response.StatusCode)
                {
                    case HttpStatusCode.Conflict:
                        MessageBox.Show($"Данный файл уже был загружен!", "Ошибка 409");
                        break;
                }
                return;
            }

            MessageBox.Show("Файл Загружен!");
        }

        /// <summary>МЕТОД Прогесс сохранения PDF файла на сервер</summary>
        private void MET_ProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
          //  this.progressBar.Value = e.ProgressPercentage;
        }

        ///// <summary>МЕТОД Создаем новую ветку</summary>
        //private void MET_AddNewNode()
        //{
        //    int _Max = ((VirtualModul)MyGlo.Modul).PUB_Protokol.Any() ? ((VirtualModul)MyGlo.Modul).PUB_Protokol.Max(p => p.PROP_pIndex) : 0;

        //    PRI_FormMyNodes.PROP_shaIndex = ++_Max;
        //    PRI_FormMyNodes.PROP_shaNomerShablon = _WinSpr.PUB_Shablon;
        //}
        #endregion
    }
}
