using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

using m = wpfReestr.MyMet;
using wpfStatic;
using e = Microsoft.Office.Interop.Excel;

namespace wpfReestr
{
    /// <summary>Модули доступа</summary>
    public enum eTipUsl
    {
        /// <summary>Круглосуточный стационар</summary>
        StacKr = 1,
        /// <summary>Дневной стационар</summary>
        StacDn = 2,
        /// <summary>Поликлиника</summary>
        Pol = 3,
        /// <summary>Параклиника</summary>
        Par = 4,
        /// <summary>Гистология</summary>
        Gist = 5
    }

    /// <summary>КЛАСС Формируем реестры</summary>
    public partial class MyReestr
    {
        #region ---- Private ----
        /// <summary>Тип реестра (1 - (T) ВМП, 3 - (C) ЗНО, 4 - (H) Без С)</summary>
        private int PRI_TipReestr;
        /// <summary>Вид реестра: 0 - Основной 1 - Исправленный реестр</summary>
        private int PRI_MainCorrect;
        /// <summary>Наша база</summary>
        private StarahReestrDataContext PRI_Context;
        /// <summary>Список реестров</summary>
        private StrahFile PRI_StrahFile;
        /// <summary>Запись реестра</summary>
        private StrahReestr PRI_StrahReestr;
        /// <summary>Запись Случая реестра</summary>
        private MySL PRI_Sl;
        /// <summary>Колличество услуг</summary>
        private int PRI_CouUsl;
        /// <summary>Номер записи реестра</summary>
        private int PRI_ReeNom_Zap;
        /// <summary>Номер пациента в реестре</summary>
        private int PRI_ReeN_Zap;
        /// <summary>Флаг 1 - кр. стационар, 2 - дн. стационар, 3 - поликлиника, 4 - параклиника, 5 - гистология</summary>
        private eTipUsl PRI_TipUsl;
        /// <summary>Возраст</summary>
        private int PRI_Age;
        /// <summary>Номер страхового полиса</summary>
        private string PRI_NPolis;
        /// <summary>Прошлый Номер страховой компании</summary>
        private string PRI_PLAT;
        /// <summary>Прошлый ОГРН страховой компании</summary>
        private string PRI_SMO_OGRN;
        /// <summary>Прошлый ОКАТО страховой компании</summary>
        private string PRI_SMO_OK;
        /// <summary>Прошлое Наименование страховой компании</summary>
        private string PRI_SMO_NAM;
        /// <summary>Прошлая Страховая компания поменялась</summary>
        private bool PRI_FlagSMO;
        /// <summary>Разрешаем пересчитывать выборку</summary>
        private bool PRI_FlagPreInfo;
        /// <summary>Справочники N002 Стадии</summary>
        private List<StrahSpr> PRI_N002;
        /// <summary>Справочники N003 Т</summary>
        private List<StrahSpr> PRI_N003;
        /// <summary>Справочники N004 N</summary>
        private List<StrahSpr> PRI_N004;
        /// <summary>Справочники N005 M</summary>
        private List<StrahSpr> PRI_N005;
        /// <summary>Справочники N008 Результат гистологии</summary>
        private List<StrahSpr> PRI_N008;
        /// <summary>Справочники N009 Соответствие гистологии диагнозам</summary>
        private List<StrahSpr> PRI_N009;
        /// <summary>Справочники N011 Результат ИГХ</summary>
        private List<StrahSpr> PRI_N011;
        /// <summary>Справочники N012 Соответствие ИГХ диагнозам</summary>
        private List<StrahSpr> PRI_N012;
        /// <summary>Справочник StrahStacSv</summary>
        private List<StrahStacSv> PRI_StrahStacSv;
        /// <summary>Объявление типа делегата для ProgressBar</summary>
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, object value);
        /// <summary>Текущая запись</summary>
        private DataRow PRI_RowReestr;

        private DataView PRI_DataViewStrachComp;

        /// <summary>Выгрузка ошибок в Excel</summary>
        private MyExcel PRI_ErrorToExcel;
        /// <summary>Ошибочная запись</summary>
        private bool PRI_ErrorRow;
        #endregion

        #region ---- Свойства ----
        /// <summary>СВОЙСТВО Код корректировки</summary>
        public int PROP_Korekt
        {
            get { return (int)this.GetValue(DEPR_KorektProperty); }
            set { this.SetValue(DEPR_KorektProperty, value); }
        }

        /// <summary>СВОЙСТВО Номер счета</summary>
        public int PROP_Schet
        {
            get { return (int)this.GetValue(DEPR_SchetProperty); }
            set { this.SetValue(DEPR_SchetProperty, value); }
        }

        /// <summary>СВОЙСТВО Дата счета</summary>
        public DateTime PROP_DateSchet
        {
            get { return (DateTime)this.GetValue(DEPR_DateSchetProperty); }
            set { this.SetValue(DEPR_DateSchetProperty, value); }
        }

        /// <summary>СВОЙСТВО Начальная дата</summary>
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

        /// <summary>СВОЙСТВО Код 1го родительского реестра</summary>
        public int PROP_ParentCod
        {
            get { return (int)this.GetValue(DEPR_ParentProperty); }
            set { this.SetValue(DEPR_ParentProperty, value); }
        }

        /// <summary>СВОЙСТВО Информация процесса</summary>
        public string PROP_ProgressLabel
        {
            get { return (string)this.GetValue(DEPR_ProgressLabelProperty); }
            set { this.SetValue(DEPR_ProgressLabelProperty, value); }
        }
        #endregion

        #region ---- РЕГИСТРАЦИЯ ----
        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_Cod</summary>
        public static readonly DependencyProperty DEPR_KorektProperty =
            DependencyProperty.Register("PROP_Korekt", typeof(int), typeof(MyReestr), new PropertyMetadata(0));

        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_Schet</summary>
        public static readonly DependencyProperty DEPR_SchetProperty =
            DependencyProperty.Register("PROP_Schet", typeof(int), typeof(MyReestr), new PropertyMetadata(0));

        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_DateSchet</summary>
        public static readonly DependencyProperty DEPR_DateSchetProperty =
            DependencyProperty.Register("PROP_DateSchet", typeof(DateTime), typeof(MyReestr), new PropertyMetadata(DateTime.Today));

        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_DateN</summary>
        public static readonly DependencyProperty DEPR_DateNProperty =
            DependencyProperty.Register("PROP_DateN", typeof(DateTime), typeof(MyReestr), new PropertyMetadata(DateTime.Today));

        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_DateK</summary>
        public static readonly DependencyProperty DEPR_DateKProperty =
            DependencyProperty.Register("PROP_DateK", typeof(DateTime), typeof(MyReestr), new PropertyMetadata(DateTime.Today));

        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_Parent</summary>
        public static readonly DependencyProperty DEPR_ParentProperty =
            DependencyProperty.Register("PROP_Parent", typeof(int), typeof(MyReestr), new PropertyMetadata(0));

        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_ProgressLabel</summary>
        public static readonly DependencyProperty DEPR_ProgressLabelProperty =
            DependencyProperty.Register("PROP_ProgressLabel", typeof(string), typeof(MyReestr));
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public MyReestr()
        {
            InitializeComponent();

            // Родительская форма
            Owner = Application.Current.MainWindow;

            // Начальные значения
#if DEBUG
            PART_DateN.Text = "01.03.2021";
            PART_DateK.Text = "31.03.2021";
#endif
        }

        #region ---- События ----
        /// <summary>СОБЫТИЕ После загрузки окна</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Тип файла, по умолчанию (T) ВМП реестр
            PRI_TipReestr = 1;
            // Основной реестр
            PRI_MainCorrect = 0;
            // Свиток - Выбор месяца и родительского реестра
            MySql.MET_DsAdapterFill(MyQuery.StrahFile_Select_2(), "StrahFile_Select");
            PRI_DataViewStrachComp = new DataView(MyGlo.DataSet.Tables["StrahFile_Select"]);
            PART_ComboBoxMainMonth.ItemsSource = PRI_DataViewStrachComp;
            PART_ComboBoxMainMonth.DisplayMemberPath = "Visual";
            PART_ComboBoxMainMonth.SelectedValuePath = "ParentCod";
            DataTable _Table = MyGlo.DataSet.Tables["StrahFile_Select"];
            _Table.PrimaryKey = new[] { _Table.Columns["ParentCod"] };
            //  Разрешаем загружать предварительную инфу (ну что бы по десять раз не загружало, при старте и установке дат)
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Выбираем основной месяц</summary>
        private void PART_ComboBoxMainMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PART_ComboBoxMainMonth.SelectedItem == null) return;
            PRI_FlagPreInfo = false;
            int _Parent = (int)PART_ComboBoxMainMonth.SelectedValue;
            DataRow _RowParent = MyGlo.DataSet.Tables["StrahFile_Select"].Rows.Find(_Parent);
            PROP_DateN = (DateTime)m.MET_PoleDate("DateNNew", _RowParent);
            PROP_DateK = (DateTime)m.MET_PoleDate("DateKNew", _RowParent);
            PRI_TipReestr = m.MET_PoleInt("TipReestr", _RowParent);
            PROP_ParentCod = m.MET_PoleInt("ParentCod", _RowParent);
            switch (PRI_TipReestr)
            {
                case 1:
                    PART_VMPRadio_1.IsChecked = true;
                    break;
                case 3:
                    PART_VMPRadio_3.IsChecked = true;
                    break;
                case 4:
                    PART_VMPRadio_4.IsChecked = true;
                    break;
            }
            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Выбираем 0 - основной, 1 - исправленный реестр</summary>
        private void RadioButtonMainCorrection_Click(object sender, RoutedEventArgs e)
        {
            if (!PRI_FlagPreInfo) return;
            PRI_FlagPreInfo = false;
            PART_ComboBoxMainMonth.SelectedValue = null;
            PRI_MainCorrect = m.MET_ParseInt(((RadioButton)sender).Tag);
            if (PRI_MainCorrect == 0)
            {
                PROP_ParentCod = 0;
                MET_PreInfa();
            }
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Тип реестра (1- (T) ВМП, 3 - (C) ЗНО, 4 - (H) Без С)  Выбор только для Тестовых реестров</summary>
        private void RadioButtonTipReestr_Click(object sender, RoutedEventArgs e)
        {
            if (!PRI_FlagPreInfo) return;
            PRI_FlagPreInfo = false;           ;
            PRI_TipReestr = m.MET_ParseInt(((RadioButton)sender).Tag);
            // Загружаем данные
            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Кнопка - прошлый месяц</summary>
        private void PART_LastButton_Click(object sender, RoutedEventArgs e)
        {
            PRI_FlagPreInfo = false;
            PROP_DateN = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, 1);
            PROP_DateK = PROP_DateN.AddMonths(1);
            PROP_DateK = PROP_DateK.AddDays(-1);
            // Загружаем данные
            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Кнопка - текущий месяц</summary>
        private void PART_CarrentButton_Click(object sender, RoutedEventArgs e)
        {
            PRI_FlagPreInfo = false;
            PROP_DateN = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            PROP_DateK = PROP_DateN.AddMonths(1);
            PROP_DateK = PROP_DateK.AddDays(-1);
            // Загружаем данные
            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Смена начальной даты</summary>
        private void PART_DateN_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!PRI_FlagPreInfo) return;
            PRI_FlagPreInfo = false;
            // Пересчитываем начальную инфу
            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Смена конечной даты</summary>
        private void PART_DateK_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!PRI_FlagPreInfo) return;
            PRI_FlagPreInfo = false;
            // Пересчитываем начальную инфу
            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }
        #endregion

        /// <summary>МЕТОД Предварительная информация</summary>
        private void MET_PreInfa()
        {
            // Загружаем заготовку реестра StrahReestr
            MySql.MET_DsAdapterFill(MyQuery.ReStrahReestr_Select_2(PRI_TipReestr, PRI_MainCorrect, PROP_DateN, PROP_DateK), "ReStrahReestr");
            // Максимум ProgressBar
            PART_Info.Content = "Записей: " + MyGlo.DataSet.Tables["ReStrahReestr"].Rows.Count;
        }

        /// <summary>СОБЫТИЕ 1. Формиуем реестр</summary>
        private void PART_ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем на даты
            if (PROP_DateN > PROP_DateK)
            {
                MessageBox.Show("Начальная дата не может быть больше конечной!", "Ошибка");
                return;
            }
            // Проверяем на наличие записей
            if (MyGlo.DataSet.Tables["ReStrahReestr"].Rows.Count == 0)
            {
                MessageBox.Show("Так нечего же выгружать!", "Ошибка");
                return;
            }
            // Запуск расчета
            MET_StartCalc();
        }

        /// <summary>МЕТОД 2. Запуск расчета</summary>
        private void MET_StartCalc()
        {
            // Сохраняем значение ProgressBar
            double _BarValue = 0;
            // Создание делегата и привязка к изменению свойства ProgressBar
            UpdateProgressBarDelegate _Progress = PART_ProgressBar.SetValue;
            // Ну начинаем
            PROP_ProgressLabel = "Подгружаем данные с сервера";
            // Всё больше форму НЕ трогаем, можем только закрыть
            PART_UserPage.IsEnabled = false;
            // Даем поток, для обновления формы
            Dispatcher.Invoke(_Progress, DispatcherPriority.Background, RangeBase.ValueProperty, _BarValue);
            // Связь с SQL, в принципе не очень то и нужен, только для разового сохраниения в  StrahFile
            try
            {
                PRI_Context = new StarahReestrDataContext(MySql.MET_ConSql());
                // Запись файла  StrahFile
                PRI_StrahFile = new StrahFile
                {
                    Cod = PRI_Context.StrahFile.Max(e => e.Cod + 1),
                    DateN = PROP_DateN,
                    DateK = PROP_DateK,
                    Korekt = 0,
                    StrahComp = 4,
                    VMP = (byte?)PRI_TipReestr,
                    SUMMAV = 0,
                    Tip = 1,
                    YEAR = PROP_DateK.Year,
                    MONTH = PROP_DateK.Month,
                    NSCHET = PROP_Schet,
                    DSCHET = PROP_DateSchet,
                    pPaket = PRI_MainCorrect == 1 ? (byte?)124 : (byte?)123,
                    pHide = 0,
                    pParent = PROP_ParentCod
                };
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            // Стартовые номера для поля N_ZAP
            int.TryParse($"{PRI_TipReestr}{PROP_DateK.Month:00}00000", out PRI_ReeN_Zap);
            PRI_ReeNom_Zap = PRI_ReeN_Zap;

            DataRelation _DataRelation;
            DataColumn[] _ReStrahReestr = new DataColumn[]
                    {   MyGlo.DataSet.Tables["ReStrahReestr"].Columns["Cod"],
                    MyGlo.DataSet.Tables["ReStrahReestr"].Columns["LPU_ST"] };

            if (PRI_TipReestr == 3 || PRI_TipReestr == 4)
            {
                // Поликлиника 1. Apac (только конечные посещения)
                MySql.MET_DsAdapterFill(MyQuery.ReApac_Select_4(PRI_TipReestr, PRI_MainCorrect, PROP_DateN, PROP_DateK), "ReApac");
                // Связываем по колонками
                DataColumn[] _ReApac = new DataColumn[]
                    {   MyGlo.DataSet.Tables["ReApac"].Columns["Cod"],
                    MyGlo.DataSet.Tables["ReApac"].Columns["LPU_ST"] };
                // Связь Реестр - Apac
                _DataRelation = new DataRelation("ReReestr_Apac", _ReStrahReestr, _ReApac, false);
                MyGlo.DataSet.Relations.Add(_DataRelation);

                // Поликлиника 2. (промежуточные посещения)
                MySql.MET_DsAdapterFill(MyQuery.ReApac_Select_5(PRI_TipReestr, PRI_MainCorrect, PROP_DateN, PROP_DateK), "RePol");
                // Связь Apac - Pol
                _DataRelation = new DataRelation("ReApac_Pol",
                    MyGlo.DataSet.Tables["ReApac"].Columns["NumberFirstTap"],
                    MyGlo.DataSet.Tables["RePol"].Columns["NumberFirstTap"], false); // Запретим создание ограничений, на "осиротевшие" записи в RePol
                MyGlo.DataSet.Relations.Add(_DataRelation);

                // Поликлиника 3. (консилиумы)
                MySql.MET_DsAdapterFill(MyQuery.ReApac_Select_6(PRI_TipReestr, PRI_MainCorrect, PROP_DateN, PROP_DateK), "RePolCons");
                // Связь Apac - PolCons
                _DataRelation = new DataRelation("ReApac_PolCons",
                    MyGlo.DataSet.Tables["ReApac"].Columns["Cod"],
                    MyGlo.DataSet.Tables["RePolCons"].Columns["Cod"], false); // Запретим создание ограничений, на "осиротевшие" записи в RePolCons
                MyGlo.DataSet.Relations.Add(_DataRelation);
            }

            // Стационар 1. Apstac
            MySql.MET_DsAdapterFill(MyQuery.ReApstac_Select_4(PRI_TipReestr, PRI_MainCorrect, PROP_DateN, PROP_DateK), "ReApstac");
            // Связываем по колонками
            DataColumn[] _ReApstac = new DataColumn[]
                {   MyGlo.DataSet.Tables["ReApstac"].Columns["Ind"],
                    MyGlo.DataSet.Tables["ReApstac"].Columns["LPU_ST"] };
            // Связь Реестр - Apstac
            _DataRelation = new DataRelation("ReReestr_Apstac", _ReStrahReestr, _ReApstac, false);
            MyGlo.DataSet.Relations.Add(_DataRelation);

           // Стационар 2. Ksg
            MySql.MET_DsAdapterFill(MyQuery.ReApstac_Select_5(PRI_TipReestr, PRI_MainCorrect, PROP_DateN, PROP_DateK), "ReKsg");
            // Связь Apstac - Ksg
            _DataRelation = new DataRelation("ReApstac_Ksg",
                MyGlo.DataSet.Tables["ReApstac"].Columns["Ind"],
                MyGlo.DataSet.Tables["ReKsg"].Columns["Ind"], false);
            MyGlo.DataSet.Relations.Add(_DataRelation);

            // Стационар 3. (консилиумы)
            MySql.MET_DsAdapterFill(MyQuery.ReApstac_Select_6(PRI_TipReestr, PRI_MainCorrect, PROP_DateN, PROP_DateK), "ReApstacCons");
            // Связь Apstac - ApstacCons
            _DataRelation = new DataRelation("ReApstac_ApstacCons",
                MyGlo.DataSet.Tables["ReApstac"].Columns["Ind"],
                MyGlo.DataSet.Tables["ReApstacCons"].Columns["Ind"], false);
            MyGlo.DataSet.Relations.Add(_DataRelation);

            if (PRI_TipReestr == 4)
            {
                // Параклиника 1. Par
                MySql.MET_DsAdapterFill(MyQuery.RePar_Select_2(PRI_TipReestr, PRI_MainCorrect, PROP_DateN, PROP_DateK), "RePar");
                // Связываем по колонками
                DataColumn[] _RePar = new DataColumn[]
                    {   MyGlo.DataSet.Tables["RePar"].Columns["Cod"],
                    MyGlo.DataSet.Tables["RePar"].Columns["LPU_ST"] };
                // Связь Реестр - RePar
                _DataRelation = new DataRelation("ReReestr_Par", _ReStrahReestr, _RePar, false);
                MyGlo.DataSet.Relations.Add(_DataRelation);
            }

            // Страховые компании. StrahComp
            if (MyGlo.DataSet.Tables["ReStrahComp"] == null)                // если ещё не загруженна
            {
                MySql.MET_DsAdapterFill(MyQuery.ReStrahComp_Select_1(), "ReStrahComp");
                DataTable _Table = MyGlo.DataSet.Tables["ReStrahComp"];
                _Table.PrimaryKey = new[] {_Table.Columns["Cod"]};
            }

            // Загружаем справочники StrahSpr
            PRI_N002 = new List<StrahSpr>(PRI_Context.StrahSpr.Where(e => e.Number > 0 && e.TableID == "N002"));
            PRI_N003 = new List<StrahSpr>(PRI_Context.StrahSpr.Where(e => e.Number > 0 && e.TableID == "N003"));
            PRI_N004 = new List<StrahSpr>(PRI_Context.StrahSpr.Where(e => e.Number > 0 && e.TableID == "N004"));
            PRI_N005 = new List<StrahSpr>(PRI_Context.StrahSpr.Where(e => e.Number > 0 && e.TableID == "N005"));
            PRI_N008 = new List<StrahSpr>(PRI_Context.StrahSpr.Where(e => e.Number > 0 && e.TableID == "N008"));
            PRI_N009 = new List<StrahSpr>(PRI_Context.StrahSpr.Where(e => e.Number > 0 && e.TableID == "N009"));
            PRI_N011 = new List<StrahSpr>(PRI_Context.StrahSpr.Where(e => e.Number > 0 && e.TableID == "N011"));
            PRI_N012 = new List<StrahSpr>(PRI_Context.StrahSpr.Where(e => e.Number > 0 && e.TableID == "N012"));

            PRI_StrahStacSv = new List<StrahStacSv>(PRI_Context.StrahStacSv);

            // Максимум ProgressBar
            PART_ProgressBar.Maximum = MyGlo.DataSet.Tables["ReStrahReestr"].Rows.Count;

            // Сохраним запись в StrahFile
            PRI_Context.StrahFile.InsertOnSubmit(PRI_StrahFile);
            PRI_Context.SubmitChanges();

            // Создаем файл с ошибками (шапачку там)
            PRI_ErrorToExcel = new MyExcel(PRI_StrahFile);

            // Перебераем все записи
            foreach (DataRow _Reestr in MyGlo.DataSet.Tables["ReStrahReestr"].Rows)
            {
                PRI_RowReestr = _Reestr;
                PRI_StrahReestr = new StrahReestr();
                PRI_Sl  = new MySL();

                // Тип услуги
                PRI_TipUsl = (eTipUsl)m.MET_PoleInt("LPU_ST", PRI_RowReestr);

                PRI_ErrorRow = false;                                           // ошибок нет (ну это пока)

                // Заранее готовим информацию для будущей ошибки
                PRI_ErrorToExcel.PROP_StrahReestr = PRI_StrahReestr;
                PRI_ErrorToExcel.PROP_RowReestr = PRI_RowReestr;
                PRI_ErrorToExcel.PROP_Nom++;
                PRI_ErrorToExcel.PROP_Tip = new[] { "", "Кр. Стационар", "Дн. Стационар", "Поликлиника", "Параклиника", "Гистология" } [(int)PRI_TipUsl];

                switch (PRI_TipUsl)
                {
                    case eTipUsl.StacKr:
                    case eTipUsl.StacDn:
                        MET_CalcStac();   // считаем стационар
                        break;
                    case eTipUsl.Pol:
                        MET_CalcPol();    // считаем поликлинику
                        break;
                    case eTipUsl.Par:
                    case eTipUsl.Gist:
                        MET_CalcPar();    // считаем параклинику
                        break;
                }

                // Изменения прогресса
                PROP_ProgressLabel = $"Загружено: {++_BarValue} из {PART_ProgressBar.Maximum}";
                Dispatcher.Invoke(_Progress, DispatcherPriority.Background, RangeBase.ValueProperty, _BarValue);

                // Сохраним строку в StrahReestr (если ошибок нет)
                if (!PRI_ErrorRow) MET_SaveSql();
            }

            // Для корректрирующих реестров пересчитаем нумерацию
            if (PROP_ParentCod > 0)
                MySql.MET_QueryNo(MyQuery.StrahReestr_Update_2(PRI_StrahFile.Cod, PROP_ParentCod));

            // Пересчитаем поле Сумму
            MySql.MET_QueryNo(MyQuery.StrahFile_Update_1(PRI_StrahFile.Cod));

            // Подчистим связи
            if (PRI_TipReestr == 3 || PRI_TipReestr == 4)
            {
                MyGlo.DataSet.Tables["ReApac"].ChildRelations.Clear();
                MyGlo.DataSet.Tables["RePol"].ChildRelations.Clear();
                MyGlo.DataSet.Tables["RePolCons"].ChildRelations.Clear();

                MyGlo.DataSet.Tables["RePol"].Clear();
                MyGlo.DataSet.Tables["RePolCons"].Clear();
                MyGlo.DataSet.Tables["ReApac"].Clear();
            }

            MyGlo.DataSet.Tables["ReApstac"].ChildRelations.Clear();
            MyGlo.DataSet.Tables["ReKsg"].ChildRelations.Clear();
            MyGlo.DataSet.Tables["ReApstacCons"].ChildRelations.Clear();

            MyGlo.DataSet.Tables["ReKsg"].Clear();
            MyGlo.DataSet.Tables["ReApstacCons"].Clear();
            MyGlo.DataSet.Tables["ReApstac"].Clear();

            if (PRI_TipReestr == 4)
            {
                MyGlo.DataSet.Tables["RePar"].Clear();
            }
            MyGlo.DataSet.Relations.Clear();
            // Усё
            MessageBox.Show(string.Format("Реестр №{0} сформирован!", PRI_StrahFile.Cod));
            // Показываем ошибки
            PRI_ErrorToExcel.MET_End();
        }


        

        /// <summary>МЕТОД 7. Расчет общих полей</summary>
        private void MET_CalcAll()
        {
            // Версия редакции 3 (март 2018)

            // CodFile (Код файла реестра)
            PRI_StrahReestr.CodFile = PRI_StrahFile.Cod;

            // PLAT (Код страховой компании)
            // Перекодируем местные страховые компании
            switch (PRI_StrahReestr.PLAT)
            {
                case "50":
                    PRI_StrahReestr.PLAT = "55050";
                    break;
                case "41":
                    PRI_StrahReestr.PLAT = "55041";
                    break;
                case "42":
                case "44":
                case "46":
                case "52":
                    PRI_StrahReestr.PLAT = "55044";
                    break;
            }
            PRI_FlagSMO = PRI_StrahReestr.PLAT != PRI_PLAT;
            // Если страховая компания поменялась
            if (PRI_FlagSMO)
            {
                PRI_PLAT = PRI_StrahReestr.PLAT;
                DataRow _RowStrah = MyGlo.DataSet.Tables["ReStrahComp"].Rows.Find(PRI_PLAT);    // Находим строку страховой компании по коду
                if (_RowStrah == null)
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "58";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найдена страховая компания";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }
                PRI_SMO_OGRN = m.MET_PoleStr("OGRN", _RowStrah);                                // ОГРН
                PRI_SMO_OK = m.MET_PoleStr("OKATO", _RowStrah);                                 // ОКАТО
                PRI_SMO_NAM = m.MET_PoleStr("Names", _RowStrah);                                // Наименование страховой компании
            }

            // SMO_OGRN  (ОГРН страховой компании)
            PRI_StrahReestr.SMO_OGRN = PRI_SMO_OGRN;

            // SMO_OK (ОКАТО страховой компании) -> ST_OKATO (Для старых полисов)
            PRI_StrahReestr.SMO_OK = PRI_SMO_OK;

            // SMO_NAM (Наименование страховой компании)
            PRI_StrahReestr.SMO_NAM = PRI_SMO_NAM;

            // LPU_ST -> USL_OK (Условия оказания мед. помощи, справочник V006, 1 - кр. стационра, 2 - дн. стационар, 3 - поликлиника, 4 - параклиника, 5 - гистология)
            PRI_StrahReestr.LPU_ST = m.MET_PoleInt("LPU_ST", PRI_RowReestr);

             // DET (Детский профиль, если ребёнок то 1 иначе 0)
            PRI_StrahReestr.DET = PRI_Age < 18 ? 1 : 0;

            // VPOLIS (Тип полиса 1 - старый, 2 - временный, 3 - новый)
            if (PRI_NPolis.Length == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "60";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Отсутствует номер страхового полиса";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }
            char _VPolis = PRI_NPolis[PRI_NPolis.Length - 1];
            switch (_VPolis)
            {
                case 'с':
                    PRI_StrahReestr.VPOLIS = 1;                                      // старый
                    if (PRI_StrahReestr.SERIA.Length == 0)
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "61";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильный тип страхового полиса";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }
                    break;
                case 'в':
                    PRI_StrahReestr.VPOLIS = 2;                                      // временный
                    if (PRI_NPolis.Length != 10)
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "61";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильный тип страхового полиса";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }
                    break;
                case 'н':
                    PRI_StrahReestr.VPOLIS = 3;                                      // новый
                    if (PRI_NPolis.Length != 17)
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "61";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильный тип страхового полиса";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }
                    break;
                default:
                    PRI_StrahReestr.VPOLIS = 1;
                    break;
            }
            if (char.IsLetter(_VPolis)) PRI_NPolis = PRI_NPolis.Remove(PRI_NPolis.Length - 1);

            // NUMBER -> NPOLIS (Номер полиса)
            PRI_StrahReestr.NUMBER = PRI_NPolis.ToUpper();

            // FAMILY (Фамилия пациента)
            string _FAM = m.MET_PoleStr("FAM", PRI_RowReestr);
            PRI_StrahReestr.FAMILY = _FAM.Substring(0, _FAM.IndexOf(" ")).ToUpper();

            // NAME (Имя пациента)
            PRI_StrahReestr.NAME = m.MET_PoleStr("I", PRI_RowReestr).ToUpper();

            // FATHER (Отчество пациента)
            string _O = m.MET_PoleStr("O", PRI_RowReestr).ToUpper();
            if (_O != "НЕТ")
                PRI_StrahReestr.FATHER = _O;

            // POL (Пол пациента 1 - муж, 2 - женский)
            PRI_StrahReestr.POL = m.MET_PoleInt("Pol", PRI_RowReestr);

            // VOZRAST (Дата рождения пациента)
            PRI_StrahReestr.VOZRAST = m.MET_PoleDate("DR", PRI_RowReestr);

            // SS (СНИЛС, с разделителями)
            string _SS = m.MET_PoleStr("SNILS", PRI_RowReestr);
            PRI_StrahReestr.SS = _SS.Length == 14 ? _SS.ToUpper() : "";

            // OS_SLUCH (Особый случай, 2 - нет Отчества)
            PRI_StrahReestr.OS_SLUCH = PRI_StrahReestr.FATHER == null ? "2" : "";

            // MR (Место рождения)
            string _MR = m.MET_PoleStr("MR", PRI_RowReestr);
            PRI_StrahReestr.MR = _MR.Length > 0 ? _MR.ToUpper() : "Г. ОМСК";

            // DOCTYPE (Тип документа удостоверяющего личность)
            PRI_StrahReestr.DOCTYPE = 0;
            if (PRI_StrahReestr.VPOLIS != 3)                    // если полис новый, то документ можем не заполнять
            {
                PRI_StrahReestr.DOCTYPE = m.MET_PoleInt("Doc", PRI_RowReestr);
                if (PRI_StrahReestr.DOCTYPE == 0)
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "45";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не указан документ личности";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }

                // DOCSER (Серия документа удостоверяющего личность)
                string _Pasp_Ser = m.MET_PoleStr("Pasp_Ser", PRI_RowReestr);
                if (_Pasp_Ser == "")
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "45";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не указана Серия документа личности";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }
                PRI_StrahReestr.DOCSER = PRI_StrahReestr.DOCTYPE == 14 && _Pasp_Ser.Length > 3
                    ? _Pasp_Ser.Substring(0, 2) + " " + _Pasp_Ser.Substring(2, 2)               // паспорт
                    : _Pasp_Ser;                                                                // другие

                // DOCNUM (Номер документа удостоверяющего личность)
                PRI_StrahReestr.DOCNUM = m.MET_PoleStr("Pasp_Nom", PRI_RowReestr);
                if (PRI_StrahReestr.DOCNUM == "")
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "45";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не указан Номер документа личности";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }

                // DOCDATE (Дата выдачи документа удостоверяющего личность)
                PRI_StrahReestr.DOCDATE = m.MET_PoleDate("Pasp_Kogda", PRI_RowReestr);
                if (PRI_StrahReestr.DOCDATE == null)
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "45";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не указан Дата выдачи документа личности";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }

                // DOCORG (Кем выдан документ удостоверяющего личность)
                PRI_StrahReestr.DOCORG = m.MET_PoleStr("Pasp_Kem", PRI_RowReestr);
                if (PRI_StrahReestr.DOCORG == "")
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "45";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не указан Кем выдан документ личности";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }
            }

            // OKATOG -> OKATOP (ОКАТО места жительства)
            // Либо города, либо населенные пункты
            PRI_StrahReestr.OKATOG = m.MET_PoleStr("OCATD", PRI_RowReestr);
            // Для Омских страховых, ОКАТО из других областенй меняем на город Омск
            if (PRI_StrahReestr.PLAT.StartsWith("55") && PRI_StrahReestr.OKATOG.Substring(0, 2) != "52")
                PRI_StrahReestr.OKATOG = "52401000000";

            // PR_NOV (Признак исправленной записи, 0 - первый раз, 1 - повторно)
            PRI_StrahReestr.PR_NOV = (byte)m.MET_PoleInt("Korekt", PRI_RowReestr);

            // NOM_ZAP -> IDCASE (Номер случая)
            PRI_StrahReestr.NOM_ZAP = PRI_ErrorRow || PRI_StrahReestr.PR_NOV  == 1 ? 0 : ++PRI_ReeNom_Zap;

            // ID_PAC (Код пациента)
            PRI_StrahReestr.ID_PAC = m.MET_PoleDec("KL", PRI_RowReestr);

            // N_ZAP  (Номер пациента)
            PRI_StrahReestr.N_ZAP = PRI_ErrorRow || PRI_StrahReestr.PR_NOV == 1 ? 0 : ++PRI_ReeN_Zap;
        }

        /// <summary>МЕТОД 8. Сохраняем полученную запись в Sql</summary>
        private void MET_SaveSql()
        {
            string _ARR_DATE = PRI_StrahReestr.ARR_DATE != null ? "'" + PRI_StrahReestr.ARR_DATE.Value.ToString("MM.dd.yyyy") + "'" : "null";
            string _EX_DATE = PRI_StrahReestr.EX_DATE != null ? "'" + PRI_StrahReestr.EX_DATE.Value.ToString("MM.dd.yyyy") + "'" : "null";
            string _VOZRAST = PRI_StrahReestr.VOZRAST != null ? "'" + PRI_StrahReestr.VOZRAST.Value.ToString("MM.dd.yyyy") + "'" : "null";
            string _DR_P = PRI_StrahReestr.DR_P != null ? "'" + PRI_StrahReestr.DR_P.Value.ToString("MM.dd.yyyy") + "'" : "null";
            string _DOCDATE = PRI_StrahReestr.DOCDATE != null ? "'" + PRI_StrahReestr.DOCDATE.Value.ToString("MM.dd.yyyy") + "'" : "null";

            NumberFormatInfo _nfi = new CultureInfo("en-US", false).NumberFormat;

            // Основной файл
            string _Str = $@"
                insert into dbo.StrahReestr
                   (CodFile
                   ,PLAT,SMO_OGRN,SMO_OK,SMO_NAM
                   ,LPU_1,[ORDER],LPU_ST
                   ,VIDPOM,PODR,PROFIL
                   ,DET,CODE_USL,PRVS,IDDOKT
                   ,ARR_DATE,EX_DATE,DS1,DS2,PACIENTID
                   ,RES_G,ISHOD,IDSP
                   ,KOL_USL,TARIF,SUM_LPU
                   ,VPOLIS,SERIA,NUMBER
                   ,FAMILY,NAME,FATHER,POL
                   ,VOZRAST,SS,OS_SLUCH
                   ,MR,DOCTYPE,DOCSER
                   ,DOCNUM,OKATOG
                   ,NOM_ZAP,UKL,NOM_USL,ID_PAC
                   ,N_ZAP,PR_NOV,DayN
                   ,VID_VME,VID_HMP,METOD_HMP
                   ,DOCDATE,DOCORG, KSG)
                values
                   ({PRI_StrahReestr.CodFile.ToString()}
                    ,'{PRI_StrahReestr.PLAT}','{PRI_StrahReestr.SMO_OGRN}','{PRI_StrahReestr.SMO_OK}','{PRI_StrahReestr.SMO_NAM}'
                    ,{PRI_StrahReestr.LPU_1.ToString()},{PRI_StrahReestr.ORDER.ToString()},{PRI_StrahReestr.LPU_ST.ToString()}
                    ,{PRI_StrahReestr.VIDPOM.ToString()},{PRI_StrahReestr.PODR.ToString()},{PRI_StrahReestr.PROFIL.ToString()}
                    ,{PRI_StrahReestr.DET.ToString()},'{PRI_StrahReestr.CODE_USL}','{PRI_StrahReestr.PRVS}','{PRI_StrahReestr.IDDOKT}'
                    ,{_ARR_DATE},{_EX_DATE},'{PRI_StrahReestr.DS1}','{PRI_StrahReestr.DS2}','{PRI_StrahReestr.PACIENTID}'
                    ,{PRI_StrahReestr.RES_G.ToString()},{PRI_StrahReestr.ISHOD.ToString()},{PRI_StrahReestr.IDSP.ToString()}
                    ,{PRI_StrahReestr.KOL_USL.ToString()},{((double)PRI_StrahReestr.TARIF).ToString("F2", _nfi)},{((double)PRI_StrahReestr.SUM_LPU).ToString("F2", _nfi)}
                    ,{PRI_StrahReestr.VPOLIS.ToString()},'{PRI_StrahReestr.SERIA}','{PRI_StrahReestr.NUMBER}'
                    ,'{PRI_StrahReestr.FAMILY}','{PRI_StrahReestr.NAME}','{PRI_StrahReestr.FATHER}',{PRI_StrahReestr.POL.ToString()}
                    ,{_VOZRAST},'{PRI_StrahReestr.SS}','{PRI_StrahReestr.OS_SLUCH}'
                    ,'{PRI_StrahReestr.MR}',{PRI_StrahReestr.DOCTYPE.ToString()},'{PRI_StrahReestr.DOCSER}'
                    ,'{PRI_StrahReestr.DOCNUM}','{PRI_StrahReestr.OKATOG}'
                    ,{PRI_StrahReestr.NOM_ZAP.ToString()},'1','{PRI_StrahReestr.NOM_USL}',{PRI_StrahReestr.ID_PAC.ToString()}
                    ,{PRI_StrahReestr.N_ZAP.ToString()},{PRI_StrahReestr.PR_NOV.ToString()},{PRI_StrahReestr.DayN.ToString()}
                    ,'{PRI_StrahReestr.VID_VME}','{PRI_StrahReestr.VID_HMP}','{PRI_StrahReestr.METOD_HMP}'
                    ,{_DOCDATE},'{PRI_StrahReestr.DOCORG}', '{PRI_StrahReestr.KSG}');";
            MySql.MET_QueryNo(_Str);
        }

        /// <summary>МЕТОД Проверка на формат даты и на диапазон госпитализаци/обращений</summary>
        /// <param name="pStrDate">Строка с датой</param>
        /// <param name="pNameTag">Имя тега</param>
        /// <param name="pFatalError">Флаг фатальной ошибки, в случае некоректной даты (по умолчанию false)</param>
        /// <param name="pIsStart">Проверяем на начло случая (по пумолчанию true)</param>
        /// <param name="pIsEnd">Проверяем на конец случая (по пумолчанию true)</param>
        /// <param name="pIsNapr">Дата должна быть меньше или равно началу случая (дата направления) (по пумолчанию false)</param>
        /// <returns>В случае ошибки возвращаем null иначе туже строку</returns>
        private string MET_VerifDate(string pStrDate, string pNameTag, bool pFatalError = false, bool pIsStart = true, bool pIsEnd = true, bool pIsNapr = false)
        {
            // Наличие даты
            if (string.IsNullOrEmpty(pStrDate))
                return null;

            // Проверка на формат даты
            if (!DateTime.TryParse(pStrDate, out DateTime _DateTime))
            {
                // Если ошибка критическая
                if (pFatalError)
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "39";
                    PRI_ErrorToExcel.PROP_ErrorName = $"(вну) Не могу преобразовать тег {pNameTag} = {pStrDate} в дату";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                }
                return null;
            }

            // Проверка на начало случая
            if (pIsStart && PRI_StrahReestr.ARR_DATE > _DateTime)
            {
                // Если ошибка критическая
                if (pFatalError)
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "40";
                    PRI_ErrorToExcel.PROP_ErrorName = $"(вну) Дата тега {pNameTag} = {pStrDate} меньше даты поступления госпитализации или даты первого посещения";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                }
                return null;
            }

            // Проверка на конец случая
            if (pIsEnd && PRI_StrahReestr.EX_DATE < _DateTime)
            {
                // Если ошибка критическая
                if (pFatalError)
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "43";
                    PRI_ErrorToExcel.PROP_ErrorName = $"(вну) Дата тега {pNameTag} = {pStrDate} больше даты выписки госпитализации или даты последнего посещения";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                }
                return null;
            }

            // Дата должна быть раньше или равна началу случая
            if (pIsNapr && (PRI_StrahReestr.ARR_DATE < _DateTime || _DateTime.Year < 2000))
            {
                // Если ошибка критическая
                if (pFatalError)
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "59";
                    PRI_ErrorToExcel.PROP_ErrorName = $"(вну) Дата тега {pNameTag} = {pStrDate} далжна быть раньше или равна началу случая {PRI_StrahReestr.ARR_DATE}";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                }
                return null;
            }

            return pStrDate;
        }
    }
}