using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using wpfGeneral.UserWindows;
using wpfGeneral.UserControls;
using wpfStatic;
using wpfGeneral.UserNodes;

namespace wpfMVrStac
{
    /// <summary>КЛАСС Страница Листа назначений</summary>
    public sealed partial class UserPage_NaznachMed
    {
        /// <summary>Список полей</summary>
        public SortedList PUB_HashPole;
        /// <summary>Ветка</summary>
        public VirtualNodes PUB_Node;

        /// <summary>Текущая таблица</summary>
        private readonly DataTable PRI_Table;
        /// <summary>Нужно ли заного генерировать Грид (походу нужно убрать)</summary>
        private bool PRI_FlagGreate;
        /// <summary>Kоличество дней (полей в календаре)</summary>
        private readonly int PRI_Day;
        /// <summary>Грид Компонентов</summary>
        private DataGrid PRI_DaGrKomp;
        /// <summary>Текущая строка Грида (любого)</summary>
        private DataRowView PRI_Row;
        /// <summary>DataView таблицы назначений</summary>
        private readonly DataView PRI_DaViNazn;
        /// <summary>DataView таблицы компонентнов</summary>
        private DataView PRI_DaViKomp;

        #region ---- Свойства ----
        /// <summary>СВОЙСТВО Флаг видимости поля текста назначения</summary>
        public Visibility PROP_FlagCompStr
        {
            get { return (Visibility)this.GetValue(DEPR_FlagCompStrProperty); }
            set { this.SetValue(DEPR_FlagCompStrProperty, value); }
        }

        /// <summary>СВОЙСТВО Флаг видимости таблицы компонентов назначения</summary>
        public Visibility PROP_FlagCompTab
        {
            get { return (Visibility)this.GetValue(DEPR_FlagCompTabProperty); }
            set { this.SetValue(DEPR_FlagCompTabProperty, value); }
        }

        /// <summary>СВОЙСТВО Дата поступления</summary>
        public DateTime PROP_DateN
        {
            get { return (DateTime)this.GetValue(DEPR_DateNProperty); }
            set { this.SetValue(DEPR_DateNProperty, value); }
        }

        /// <summary>СВОЙСТВО Конечная дата</summary>
        public DateTime PROP_DateK
        {
            get { return (DateTime)this.GetValue(DEPR_DateKProperty); }
            set { this.SetValue(DEPR_DateKProperty, value); }
        }
        #endregion

        #region ---- РЕГИСТРАЦИЯ ----
        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_FlagCompStr</summary>
        public static readonly DependencyProperty DEPR_FlagCompStrProperty =
            DependencyProperty.Register("PROP_FlagCompStr", typeof(Visibility), typeof(UserPage_NaznachMed));

        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_FlagCompTab</summary>
        public static readonly DependencyProperty DEPR_FlagCompTabProperty =
            DependencyProperty.Register("PROP_FlagCompTab", typeof(Visibility), typeof(UserPage_NaznachMed));

        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_DateN</summary>
        public static readonly DependencyProperty DEPR_DateNProperty =
            DependencyProperty.Register("PROP_DateN", typeof(DateTime), typeof(UserPole_Data), null);

        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_DateK</summary>
        public static readonly DependencyProperty DEPR_DateKProperty =
            DependencyProperty.Register("PROP_DateK", typeof(DateTime), typeof(UserPole_Data), null);
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserPage_NaznachMed()
        {
            InitializeComponent();

            const string _TabName = "lnzVrachLS_Form";
            if (MyGlo.DataSet.Tables[_TabName] != null)
            {
                MyGlo.DataSet.Tables[_TabName].Reset();
            }
            string _Name;
            // Запрос
            MySql.MET_DsAdapterFill(MyQuery.lnzVrachLS_Select_1(MyGlo.IND), _TabName);
            PRI_Table = MyGlo.DataSet.Tables[_TabName];
            PROP_DateN = Convert.ToDateTime(MyGlo.HashAPSTAC["DN"]);
            // Конечная дата
            if (Convert.ToString(MyGlo.HashAPSTAC["DK"]) == "")
            {
                PROP_DateK = DateTime.Today + new TimeSpan(20, 0, 0, 0);
            }
            else
            {
                PROP_DateK = Convert.ToDateTime(MyGlo.HashAPSTAC["DK"]);
            }
            PRI_Day = (PROP_DateK - PROP_DateN).Days + 1;
            // Создаем столбцы с датой
            for (int i = 0; i < PRI_Day; i++)
            {
                _Name = "d" + i;
                if (PRI_Table != null)
                {
                    DataColumn _Column = PRI_Table.Columns.Add(_Name, typeof(string));
                    TimeSpan _TimeSpan = new TimeSpan(i, 0, 0, 0);
                    _Column.Caption = PROP_DateN.Add(_TimeSpan).ToShortDateString();
                }
            }
            // Cоздаем DataView для нашей таблице
            PRI_DaViNazn = new DataView(PRI_Table);
            foreach (DataRowView _Row in PRI_DaViNazn)
            {
                PRI_Row = _Row;
                // Начальный день
                int _DayN = (MET_PoleDat("DateN") - PROP_DateN).Days;
                for (int i = _DayN; (i <= _DayN + ((MET_PoleInt("Kurs") - 1) * MET_PoleInt("Period"))) & i < PRI_Day; i += MET_PoleInt("Period"))
                {
                    _Name = "d" + i;
                    PRI_Row[_Name] = MET_PoleInt("Amt");
                }
            }
            // Если новый лист назначения, то сразу добавляем новое назначение
            if (PRI_Table != null && PRI_Table.Rows.Count == 0)
                MET_AddNaznLS();
            // Отображаем таблицу
            eleDataGrid.ItemsSource = PRI_DaViNazn;
            PUB_HashPole = new SortedList();                                    // список полей
        }

        /// <summary>МЕТОД Формируем поля</summary>
        /// <param name="pVirtualPole">Поле</param>
        /// <param name="pText">Текст ответа</param>
        private void MET_CreatePole(VirtualPole pVirtualPole, string pText)
        {
            pVirtualPole.PROP_Text = pText;
            pVirtualPole.PROP_DefaultText = pText;
        }

        /// <summary>МЕТОД Записываем поля в очередь</summary>
        /// <param name="pVisual">Корневой элемент</param>
        public void MET_EnumVisual(Visual pVisual)
        {
            Visual _ChildVisual;                                                // ребенок
            VirtualPole _Pole;                                                  // моё поле
            string _Name;                                                       // имя поля
            int _V = VisualTreeHelper.GetChildrenCount(pVisual);                // сколько детей
            for (int i = 0; i < _V; i++)
            {
                // Retrieve child visual at specified index value.
                _ChildVisual = (Visual)VisualTreeHelper.GetChild(pVisual, i);
                if (_ChildVisual is VirtualPole)
                {
                    // Создаем поле
                    _Pole = (VirtualPole)_ChildVisual;
                    // Имя
                    _Name = "locP_" + _Pole.PROP_VarId;
                    // Добавляем поле в очередь
                    PUB_HashPole.Add(_Name, _Pole);
                }
                else
                {
                    // Enumerate children of the child visual object.
                    if (_ChildVisual is Expander)
                        _ChildVisual = (Visual)(_ChildVisual as Expander).Content;
                    MET_EnumVisual(_ChildVisual);
                }
            }
        }

        /// <summary>МЕТОД Формируем поля</summary>
        private void MET_FormatNazn()
        {
            // Наименование компонента
            string _NameLS = "";
            string _Poi = "";
            foreach (DataRowView _Row in PRI_DaViKomp)
            {
                PRI_Row = _Row;
                _NameLS += _Poi + MET_PoleStr("NameKomp") + " - " + MET_PoleStr("Doza") + "\xA0" + MET_PoleStr("BazeMeas");
                _Poi = ",\n";
            }
            PRI_Row = (DataRowView)eleDataGrid.SelectedItem;
            // Дата отмены
            if (MET_PoleStr("DelNote") == "" || (MET_PoleDat("DelDate") < MET_PoleDat("DateN") | MET_PoleDat("DelDate") > MET_PoleDat("DateK")))
                PRI_Row["DelDate"] = PRI_Row["DateK"];
            if (_NameLS != "")
                PRI_Row["NameLS"] = _NameLS;
            // Чистим строку
            for (int i = 0; i < PRI_Day; i++)
            {
                string _Name = "d" + i;
                PRI_Row[_Name] = "";
            }
            // Начальный день
            int _DayN = (MET_PoleDat("DateN") - PROP_DateN).Days;
            for (int i = _DayN; (i <= _DayN + ((MET_PoleInt("Kurs") - 1) * MET_PoleInt("Period"))) & i < PRI_Day ; i += MET_PoleInt("Period"))
            {
                string _Name = "d" + i;
                PRI_Row[_Name] = MET_PoleInt("Amt");
            }
            PRI_Row.EndEdit();
            // Записываем изменения в базу
            MET_SaveNaznLS(PRI_Row);
        }

        /// <summary>СОБЫТИЕ После загрузки окна</summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (!PRI_FlagGreate && eleDataGrid.Columns.Count > 1)
            {
                // Удаляем ненужные столбцы
                for (int i = 1; i < 25; i++)
                    eleDataGrid.Columns.RemoveAt(1);
                PRI_FlagGreate = true;
            }
        }

        /// <summary>СОБЫТИЕ Нажали мышой</summary>
        private void eleDataGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (eleDataGrid.RowDetailsVisibilityMode == DataGridRowDetailsVisibilityMode.Collapsed)
                eleDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }

        /// <summary>СОБЫТИЕ Загрузка подокна</summary>
        private void eleDataGrid2_Loaded(object sender, RoutedEventArgs e)
        {
            PRI_DaGrKomp = (DataGrid)sender;
            // Отображаем таблицу
            PRI_DaGrKomp.ItemsSource = PRI_DaViKomp;
        }

        /// <summary>СОБЫТИЕ Генерация столбцов</summary>
        private void eleDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (!PRI_FlagGreate && eleDataGrid.Columns.Count > 1)
            {
                // Переименовываем столбцы
                e.Column.Header = PRI_Table.Columns[e.Column.Header.ToString()].Caption;
            }
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку Добавляем ЛС</summary>
        private void PART_SaveLS_Click(object sender, RoutedEventArgs e)
        {
            // Открываем Справочник Лекарственных средств
            UserWindow_Zay_NaznachMedLS _WinSpr = new UserWindow_Zay_NaznachMedLS
            {
                PROP_FlagButtonSelect = true,
                PROP_Modal = true,
                WindowStyle = WindowStyle.ToolWindow
            };
            _WinSpr.ShowDialog();
            if (_WinSpr.PROP_Return)
            {
                // Находим запись
                PRI_Row = (DataRowView)eleDataGrid.SelectedItem;
                DataRowView _Row = PRI_DaViKomp.AddNew();
                if (MET_PoleInt("Cod") == -1) MET_FormatNazn();
                _Row["CodVrachLS"] = MET_PoleInt("Cod");
                _Row["CodLS"] = _WinSpr.PUB_Cod;
                _Row["NameKomp"] = _WinSpr.PUB_NameVrach;
                _Row["Doza"] = (double)0;
                _Row["BazeMeas"] = _WinSpr.PUB_BazeMeas;
                _Row.EndEdit();
                MET_SaveKompLS(_Row);
                MET_FormatNazn();
                // Скрываем/показываем таблицу компонентов
                MET_FlagComp();
            }
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку Удаляем ЛС</summary>
        private void PART_DeleteLS_Click(object sender, RoutedEventArgs e)
        {
            // Находим запись
            PRI_Row = (DataRowView)PRI_DaGrKomp.SelectedItem;
            if (PRI_Row != null)
            {
                MySql.MET_QueryNo(MyQuery.lnzKompLS_Delete_1(MET_PoleInt("Cod")));
                PRI_Row.Delete();
                MET_FormatNazn();
                // Скрываем/показываем таблицу компонентов
                MET_FlagComp();
            }
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку Удалить Назначение</summary>
        private void PART_DeleteNaz_Click(object sender, RoutedEventArgs e)
        {
            // Находим запись
            PRI_Row = (DataRowView)eleDataGrid.SelectedItem;
            if (PRI_Row != null)
            {
                // Удаляем компоненты назначения
                MySql.MET_QueryNo(MyQuery.lnzKompLS_Delete_2(MET_PoleInt("Cod")));
                // Удаляем назначение
                MySql.MET_QueryNo(MyQuery.lnzVrachLS_Delete_1(MET_PoleInt("Cod")));
                // Удалеяем строку в таблице
                PRI_Row.Delete();
                // Обновляем отчет
                PUB_Node.PROP_Docum.PROP_Otchet.PROP_NewCreate = true;
                // Есть нету назначений, то документ не заполнен
                if (PRI_DaViNazn.Count == 0 && PUB_Node.PROP_shaPresenceProtokol)
                    PUB_Node.PROP_shaPresenceProtokol = false;
            }
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку Добавляем Назначения</summary>
        private void PART_AddNaz_Click(object sender, RoutedEventArgs e)
        {
            MET_AddNaznLS();
        }

        /// <summary>МЕТОД Добавление Назначений ЛС</summary>
        private void MET_AddNaznLS()
        {
            // Добавляем данные в sqlDs
            PRI_Row = PRI_DaViNazn.AddNew();
            PRI_Row["Cod"] = -1;
            PRI_Row["DateN"] = PROP_DateK > DateTime.Today ? DateTime.Today : Convert.ToDateTime(MyGlo.HashAPSTAC["DN"]);
            PRI_Row["Kurs"] = 1;
            PRI_Row["Period"] = 1;
            PRI_Row["Amt"] = 1;
            PRI_Row["NameLS"] = "Текст назначения";
            PRI_Row["UserVrach"] = MyGlo.User;
            PRI_Row["DateK"] = MET_PoleDat("DateN").AddDays((MET_PoleDou("Kurs") - 1) * MET_PoleInt("Period"));
            PRI_Row["DelDate"] = PRI_Row["DateK"];
            PRI_Row.EndEdit();
            eleDataGrid.SelectedItem = PRI_Row;
            eleDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }

        /// <summary>СОБЫТИЕ Изменили количество дней</summary>
        private void PART_Pole_2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (eleDataGrid.SelectedItem == null) return;
            UserPole_Text _Pole = (UserPole_Text)sender;
            PRI_Row = (DataRowView)eleDataGrid.SelectedItem;
            if (_Pole.PROP_Text == MET_PoleStr("Kurs")) return;
            try
            {
                int _D = Convert.ToInt16(_Pole.PROP_Text);
                if (_D < 1 | _D > 99)
                {
                    _Pole.PROP_Text = MET_PoleStr("Kurs");
                    return;
                }
            }
            catch
            {
                _Pole.PROP_Text = MET_PoleStr("Kurs");
                return;
            }
            PRI_Row["Kurs"] = Convert.ToInt16(_Pole.PROP_Text);
            PRI_Row["DateK"] = MET_PoleDat("DateN").AddDays((MET_PoleDou("Kurs") - 1) * MET_PoleInt("Period"));
            MET_FormatNazn();
        }

        /// <summary>СОБЫТИЕ Изменили начальную дату</summary>
        private void PART_Pole_1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (eleDataGrid.SelectedItem == null) return;
            UserPole_Data _Pole = (UserPole_Data)sender;
            PRI_Row = (DataRowView)eleDataGrid.SelectedItem;
            if (_Pole.PROP_Date == MET_PoleDat("DateN")) return;
            if (_Pole.PROP_Date < (DateTime)_Pole.PROP_ValueMin | _Pole.PROP_Date > (DateTime)_Pole.PROP_ValueMax)
            {
                _Pole.PROP_Date = MET_PoleDat("DateN");
                return;
            }
            PRI_Row["DateN"] = _Pole.PROP_Date;
            PRI_Row["DateK"] = _Pole.PROP_Date.GetValueOrDefault().Date.AddDays((MET_PoleInt("Kurs") - 1) * MET_PoleInt("Period"));    
            MET_FormatNazn();
        }

        /// <summary>СОБЫТИЕ Изменили конечную дату</summary>
        private void PART_Pole_3_LostFocus(object sender, RoutedEventArgs e)
        {
            if (eleDataGrid.SelectedItem == null) return;
            UserPole_Data _Pole = (UserPole_Data)sender;
            PRI_Row = (DataRowView)eleDataGrid.SelectedItem;
            if (_Pole.PROP_Date == MET_PoleDat("DateK")) return;
            if (_Pole.PROP_Date < (DateTime)_Pole.PROP_ValueMin | _Pole.PROP_Date > (DateTime)_Pole.PROP_ValueMax)
            {
                _Pole.PROP_Date = MET_PoleDat("DateK");
                return;
            }
            PRI_Row["Kurs"] = ((_Pole.PROP_Date - MET_PoleDat("DateN")).GetValueOrDefault().Days + 1) / MET_PoleInt("Period");
            PRI_Row["DateK"] = _Pole.PROP_Date;
            MET_FormatNazn();
        }

        /// <summary>СОБЫТИЕ Изменили схему приема</summary>
        private void PART_Pole_5_LostFocus(object sender, RoutedEventArgs e)
        {
            if (eleDataGrid.SelectedItem == null) return;
            UserPole_Text _Pole = (UserPole_Text)sender;
            PRI_Row = (DataRowView)eleDataGrid.SelectedItem;
            if (_Pole.PROP_Text == MET_PoleStr("Amt")) return;
            try
            {
                int _D = Convert.ToInt16(_Pole.PROP_Text);
                if (_D < 1 | _D > 9)
                {
                    _Pole.PROP_Text = MET_PoleStr("Amt");
                    return;
                }
            }
            catch
            {
                _Pole.PROP_Text = MET_PoleStr("Amt");
                return;
            }
            PRI_Row["Amt"] = _Pole.PROP_Text;
            MET_FormatNazn();
        }

        /// <summary>СОБЫТИЕ Изменили периодичность</summary>
        private void PART_Pole_4_LostFocus(object sender, RoutedEventArgs e)
        {
            if (eleDataGrid.SelectedItem == null) return;
            UserPole_Text _Pole = (UserPole_Text)sender;
            PRI_Row = (DataRowView)eleDataGrid.SelectedItem;
            if (_Pole.PROP_Text == MET_PoleStr("Period")) return;
            try
            {
                int _D = Convert.ToInt16(_Pole.PROP_Text);
                if (_D < 1 | _D > 11)
                {
                    _Pole.PROP_Text = MET_PoleStr("Period");
                    return;
                }
            }
            catch
            {
                _Pole.PROP_Text = MET_PoleStr("Period");
                return;
            }
            PRI_Row["Period"] = Convert.ToInt16(_Pole.PROP_Text);
            PRI_Row["DateK"] = (MET_PoleDat("DateN")).AddDays((MET_PoleInt("Kurs") - 1) * MET_PoleInt("Period"));
            MET_FormatNazn();
        }

        /// <summary>СОБЫТИЕ Изменили текстовое поле, без специальной логики</summary>
        private void PART_Pole_Text_LostFocus(object sender, RoutedEventArgs e)
        {
            if (eleDataGrid.SelectedItem == null) return;
            UserPole_Text _Pole = (UserPole_Text)sender;
            PRI_Row = (DataRowView)eleDataGrid.SelectedItem;
            if (_Pole.PROP_Text == MET_PoleStr(_Pole.Tag.ToString())) return;
            PRI_Row[_Pole.Tag.ToString()] = _Pole.PROP_Text;
            MET_FormatNazn();
        }

        /// <summary>СОБЫТИЕ Изменили дату отмены</summary>
        private void PART_Pole_11_LostFocus(object sender, RoutedEventArgs e)
        {
            if (eleDataGrid.SelectedItem == null) return;
            UserPole_Data _Pole = (UserPole_Data)sender;
            PRI_Row = (DataRowView)eleDataGrid.SelectedItem;
            if (_Pole.PROP_Date == MET_PoleDat("DelDate")) return;
            if (_Pole.PROP_Date < (DateTime)_Pole.PROP_ValueMin | _Pole.PROP_Date > (DateTime)_Pole.PROP_ValueMax)
            {
                _Pole.PROP_Date = MET_PoleDat("DelDate");
                return;
            }
            MET_FormatNazn();
        }

        /// <summary>СОБЫТИЕ Скрыть</summary>
        private void PART_Hide_Click(object sender, RoutedEventArgs e)
        {
            eleDataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
        }

        /// <summary>СОБЫТИЕ Выбор строки назначения</summary>
        private void eleDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (eleDataGrid.SelectedItem != null)
            {
                PRI_Row = (DataRowView)eleDataGrid.SelectedItem;
                int _Cod = MET_PoleInt("Cod");
                MySql.MET_DsAdapterFill(MyQuery.lnzKompLS_Select_1(_Cod), "lnzKompLS");
                // Cоздаем DataView для нашей таблице
                PRI_DaViKomp = new DataView(MyGlo.DataSet.Tables["lnzKompLS"]);
                // Скрываем/показываем таблицу компонентов
                MET_FlagComp();
            }
        }

        /// <summary>МЕТОД Проверка наличия компонентов в назначении</summary>
        private void MET_FlagComp()
        {
            // Скрываем/показываем таблицу компонентов
            if (PRI_DaViKomp.Count > 0)
            {
                PROP_FlagCompTab = Visibility.Visible;
                PROP_FlagCompStr = Visibility.Collapsed;
            }
            else
            {
                PROP_FlagCompTab = Visibility.Collapsed;
                PROP_FlagCompStr = Visibility.Visible;
            }
        }

        /// <summary>МЕТОД Добавление Изменение Компонента ЛС</summary>
        private void MET_SaveKompLS(DataRowView pRow)
        {
            PRI_Row = pRow;
            int _Cod;
            int _CodVrachLS = MET_PoleInt("CodVrachLS");
            decimal _CodApstac = MyGlo.IND;

            int _CodLS = MET_PoleInt("CodLS");
            string _NameKomp = MET_PoleStr("NameKomp");
            double _Doza = MET_PoleDou("Doza");
            //---- Update Если есть код ответа, значить строка старая и меняем ответ
            if (MET_PoleStr("Cod") != "")
            {
                _Cod = MET_PoleInt("Cod");        // код ответа
                MySql.MET_QueryNo(MyQuery.lnzKompLS_Update_1(_Cod, _CodVrachLS, _CodLS, _NameKomp, _Doza));
                return;
            }
            // ----- Insert Находим максимальный код
            _Cod = MySql.MET_GetNextRef(31);
            PRI_Row["Cod"] = _Cod;
            PRI_Row["CodApstac"] = _CodApstac;
            // Добавляем ответ в базу
            MySql.MET_QueryNo(MyQuery.lnzKompLS_Insert_1(_Cod, _CodVrachLS, _CodApstac, _CodLS, _NameKomp, _Doza));
        }

        /// <summary>МЕТОД Добавление Изменение Назначений ЛС</summary>
        private void MET_SaveNaznLS(DataRowView pRow)
        {
            PRI_Row = pRow;
            int _Cod;
            DateTime _DateN = MET_PoleDat("DateN");
            int _Kurs = MET_PoleInt("Kurs");
            int _Period = MET_PoleInt("Period");
            string _NameLS = MET_PoleStr("NameLS");
            int _Amt = MET_PoleInt("Amt");
            string _Route = MET_PoleStr("Route");
            string _Note = MET_PoleStr("Note");
            int _FlagDrug = MET_PoleInt("FlagDrug");
            int _FlagPac = MET_PoleInt("FlagPac");
            int _UserVrach = MET_PoleInt("UserVrach");
            int _DelUserVrach = MET_PoleInt("DelUserVrach");
            DateTime _DelDate = MET_PoleDat("DelDate");
            string _DelNote = MET_PoleStr("DelNote");
            DateTime _xDateUp = DateTime.Today;
            //---- Update Если есть код ответа, значить строка старая и меняем ответ
            if (MET_PoleStr("Cod") != "-1")
            {
                _Cod = MET_PoleInt("Cod");        // код ответа
                MySql.MET_QueryNo(MyQuery.lnzVrachLS_Update_1(_Cod, _DateN, _Kurs, _Period, _NameLS, _Amt, _Route, _Note,
                                                 _FlagDrug, _FlagPac, _UserVrach, _DelUserVrach, _DelDate, _DelNote, _xDateUp));
                PUB_Node.PROP_Docum.PROP_Otchet.PROP_NewCreate = true;
                return;
            }
            // ----- Insert Находим максимальный код
            _Cod = MySql.MET_GetNextRef(30);
            PRI_Row["Cod"] = _Cod;
            decimal _KL = MyGlo.KL;
            decimal _CodApstac = MyGlo.IND;
            PRI_Row["CodApstac"] = _CodApstac;
            DateTime _pDate = DateTime.Today;
            // Добавляем ответ в базу
            MySql.MET_QueryNo(MyQuery.lnzVrachLS_Insert_1(_Cod, _KL, _CodApstac, _DateN, _Kurs, _Period, _NameLS, _Amt, _Route, _Note,
                                             _FlagDrug, _FlagPac, _UserVrach, _DelUserVrach, _DelDate, _DelNote, _pDate, _xDateUp));
            PUB_Node.PROP_Docum.PROP_Otchet.PROP_NewCreate = true;
            if (!PUB_Node.PROP_shaPresenceProtokol)
                PUB_Node.PROP_shaPresenceProtokol = true;
        }

        /// <summary>СОБЫТИЕ Редактируем дозу компонентов</summary>
        private void eleDataGrid2_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            DataGrid _DataGrid = (DataGrid)sender;
            TextBox _TextBox = (TextBox)e.EditingElement;
            PRI_Row = (DataRowView)_DataGrid.SelectedItem;
            try
            {
                // Позволяем ставить десятичную и точку, и запятую
                _TextBox.Text = _TextBox.Text.Replace(",", ".");
                // Проверяем на правильность ввода, с учетом инглишь кодировки (десятичная точка)
                double _New = Convert.ToDouble(_TextBox.Text, new CultureInfo("en", false));
                // Смотрим старое число, с учетом русской кодировки (десятичная запятая)
                double _Old = Convert.ToDouble(MET_PoleStr("Doza"));
                // Если небыло изменений - выходим
                if (_New == _Old)
                    return;
                PRI_Row["Doza"] = _New;
                // Добавляем изменения в компонент
                MET_SaveKompLS(PRI_Row);
                // Перестраиваем запись таблицы
                MET_FormatNazn();
            }
            catch
            {
                _TextBox.Text = MET_PoleStr("Doza");
            }
        }

        /// <summary>СОБЫТИЕ Показываем/Скрываем детализацию строки Назначения</summary>
        private void eleDataGrid_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {
            PRI_DaGrKomp = (DataGrid)e.DetailsElement.FindName("eleDataGrid2");
        }

        /// <summary>СОБЫТИЕ Препарат пациента</summary>
        private void PART_Pole_9_IsChecked(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            UserPole_CheckBox _Pole = (UserPole_CheckBox)sender;
            PRI_Row = (DataRowView)eleDataGrid.SelectedItem;
            PRI_Row["FlagPac"] = _Pole.PROP_Checked ? 1 : 0;
            MET_FormatNazn();
        }

        #region ---- ВЫБОР ДАННЫХ ----
        /// <summary>МЕТОД Возвращаем строку</summary>
        private string MET_PoleStr(string pPole)
        {
            return Convert.ToString(PRI_Row[pPole]);
        }

        /// <summary>МЕТОД Возвращаем Дату</summary>
        private DateTime MET_PoleDat(string pPole)
        {
            try { return MyMet.MET_PoleDate(pPole, PRI_Row.Row) ?? DateTime.Today; } // Convert.ToDateTime(PRI_Row[pPole]); }
            catch
            {
                // ignored
            }
            return DateTime.Today;
        }

        /// <summary>МЕТОД Возвращаем строку c Целым числом</summary>
        private int MET_PoleInt(string pPole)
        {
            try { return MyMet.MET_PoleInt(pPole, PRI_Row.Row); }
            catch
            {
                // ignored
            }
            return 0;
        }

        /// <summary>МЕТОД Возвращаем строку c Децимал числом</summary>
        private decimal MET_PoleDec(string pPole)
        {
            try { return Convert.ToDecimal(PRI_Row[pPole]); }
            catch { }
            return 0;
        }

        /// <summary>МЕТОД Возвращаем строку c Реальным числом</summary>
        private double MET_PoleDou(string pPole)
        {
            try
            {
                return Convert.ToDouble(PRI_Row[pPole]);
            }
            catch
            {
                // ignored
            }
            return 0;
        }

        /// <summary>МЕТОД Возвращаем строку c Bool</summary>
        private bool MET_PoleBool(string pPole)
        {
            try { return Convert.ToBoolean(PRI_Row[pPole]); }
            catch { }
            return false;
        }
        #endregion
    }

    /// <summary>КЛАСС Конвертер (Пример) </summary>
    class AgeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Все проверки для краткости выкинул
            return (string)value == "Цефсон 1гр" ?
                new SolidColorBrush(Colors.Azure)
                : new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    /// <summary>КЛАСС МультиКонвертер закрашивания заголовка выходных дней - красным цветом </summary>
    class AgeToColorConverter1 : IMultiValueConverter
    {
        /// <summary>МЕТОД Конвертор</summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Color _color = Colors.Black;
            // Начиная с 23 столбца идут данные с датой
            if (values[0] is DataGridColumnHeader _ColumnHeader && _ColumnHeader.DisplayIndex > 23)
            {
                try
                {
                    if (DateTime.TryParse(values[2].ToString(), out DateTime _Result))
                    {
                        if (_Result == DateTime.Today)
                            _color = Colors.Green;
                        else if (_Result.DayOfWeek == DayOfWeek.Sunday || _Result.DayOfWeek == DayOfWeek.Saturday)
                            _color = Colors.Red;
                    }
                }
                catch { }
            }
            return new SolidColorBrush(_color);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            string[] splitValues = ((string)value).Split(' ');
            return splitValues;
        }
    }

    /// <summary>КЛАСС Конвертер (Пример) </summary>
    public class DataGridCellOnErrorConversion : IMultiValueConverter
    {
        private readonly SolidColorBrush DefaultColour = new SolidColorBrush(Colors.White);
        private readonly SolidColorBrush ErrorColour = new SolidColorBrush(Colors.Red);
        private readonly SolidColorBrush AlternatingColour = new SolidColorBrush(Colors.AliceBlue);

        /// <summary>МЕТОД Пример</summary>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return DefaultColour;
            return DefaultColour;

            //DataGridCell dgc = values[0] as DataGridCell;
            //if (dgc == null) return DefaultColour;
            //IList<Point> problems = values[1] as IList<Point>;

            //if (problems == null) return DefaultColour;
            //DataGrid grid = values[2] as DataGrid;
            //if (grid == null) return DefaultColour;
            //int x; int y = -1;
            //ItemCollection itemCollection = grid.Items;
            //for (int i = 0; i < itemCollection.Count; i++)
            //{
            //    if (itemCollection.CurrentItem == itemCollection[i]) y = i;
            //}
            //x = dgc.Column.DisplayIndex;
            //DataRowView currentRowView = null;
            //FieldInfo fi = dgc.GetType().GetField("_owner", BindingFlags.NonPublic | BindingFlags.Instance);
            //try
            //{
            //    if (fi != null)
            //    {
            //        DataGridRow dataGridRow = fi.GetValue(dgc) as DataGridRow;
            //        if (dataGridRow != null) currentRowView = dataGridRow.Item as DataRowView;
            //    }
            //}
            //catch (InvalidCastException) { }
            //if (currentRowView != null)
            //{
            //    for (int i = 0; i < itemCollection.Count; i++)
            //    {
            //        if (currentRowView == itemCollection[i]) y = i;
            //    }
            //}
            //if (problems.Any(problem => System.Convert.ToInt32(problem.X) == x && System.Convert.ToInt32(problem.Y) == y)) { return ErrorColour; }
            //return y % 2 == 0 ? AlternatingColour : DefaultColour;
        }

        /// <summary>МЕТОД Пример</summary>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            string[] splitValues = ((string)value).Split(' ');
            return splitValues;
        }
    }
}
