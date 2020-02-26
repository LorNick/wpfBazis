using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        /// <summary>Тип страховой компании (0 - все, 1 - Альфа(Аско), 2 - Капитал (РосГос Мед), 3 - Росно, 4 - Иногородние)</summary>
        private int PRI_Strah;
        /// <summary>Тип выгрузки (0 - все, 1 - стационар, 2 - поликлиника, 3 - дневной стационар 4 - параклиника)</summary>
        private int PRI_ExportType;
        /// <summary>Тип реестра (поле VMP) (0 - без ВМП старый, 1- только ВМП действующий, 2 - общий старый, 3 - ЗНО действующий, 4 - без С действующий)</summary>
        private int PRI_TipFile;

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

        /// <summary>Фрагмент условия выборки полкилиники</summary>
        private string PRI_WerePol;
        /// <summary>Фрагмент условия выборки стационара</summary>
        private string PRI_WereStac;
        /// <summary>Фрагмент условия выборки параклиника</summary>
        private string PRI_WerePar;
        /// <summary>Фрагмент выборки ReStrahReestr_Select_1</summary>
        private string PRI_SelectAll;
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
        public int PROP_Parent
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
            PART_DateN.Text = "06.02.2020";
            PART_DateK.Text = "07.02.2020";
#endif
        }

        #region ---- События ----
        /// <summary>СОБЫТИЕ После загрузки окна</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Все страховые компании
            PRI_Strah = 0;
            // Тип выгурзки
            PRI_ExportType = 0;
            // Тип файла
            PRI_TipFile = 3;
            // Свиток - Основной/Исправленный реестр
            MySql.MET_DsAdapterFill(MyQuery.StrahFile_Select_2(), "StrahFile_Select");
            PRI_DataViewStrachComp = new DataView(MyGlo.DataSet.Tables["StrahFile_Select"]);
            PART_ComboBoxParent.ItemsSource = PRI_DataViewStrachComp;
            PART_ComboBoxParent.DisplayMemberPath = "Visual";
            PART_ComboBoxParent.SelectedValuePath = "Cod";
            PART_ComboBoxParent.SelectedValue = 99999;
            DataTable _Table = MyGlo.DataSet.Tables["StrahFile_Select"];
            _Table.PrimaryKey = new[] { _Table.Columns["Cod"] };
            // Загружаем предварительную информацию
            MET_PreInfa();
            //  Разрешаем загружать предварительную инфу (ну что бы по десять раз не загружало, при старте и установке дат)
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Формиуем реестр</summary>
        private void PART_ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            // Проверка стартовых параметров и далее запуск расчета
            MET_StartVerification();
        }

        /// <summary>СОБЫТИЕ Перед открытием списка родительских реестров, делаем фильтр</summary>
        private void PART_ComboBoxParent_DropDownOpened(object sender, EventArgs e)
        {
            if (!PRI_FlagPreInfo) return;
            PRI_FlagPreInfo = false;

            PRI_DataViewStrachComp.RowFilter = $"Cod = 99999 or VMP = {PRI_TipFile}";

            if (PRI_Strah > 0)
                PRI_DataViewStrachComp.RowFilter += $" and StrahComp = {PRI_Strah}";

            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Выбираем, основной файл или выбираем родителя, для исправленного файла</summary>
        private void PART_ComboBoxParent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!PRI_FlagPreInfo || PART_ComboBoxParent.SelectedValue == null) return;

            PRI_FlagPreInfo = false;
            int _Parent = (int)PART_ComboBoxParent.SelectedValue;
            DataRow _RowParent = MyGlo.DataSet.Tables["StrahFile_Select"].Rows.Find(_Parent);
          
            // Если основной файл
            if (_Parent == 99999)
            {
                PROP_DateN = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                PROP_DateK = PROP_DateN.AddMonths(1);
                PROP_DateK = PROP_DateK.AddDays(-1);

                PROP_Parent = 0;
            }
            else // Если исправленный файл
            {
                PROP_DateN = (DateTime)m.MET_PoleDate("DateN", _RowParent);
                PROP_DateK = (DateTime)m.MET_PoleDate("DateK", _RowParent);
                PRI_TipFile = m.MET_PoleInt("VMP", _RowParent);
                switch (PRI_TipFile)
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

                PRI_Strah = m.MET_PoleInt("StrahComp", _RowParent);
                switch (PRI_Strah)
                {
                    case 1:
                        PART_StrahCompRadio_1.IsChecked = true;
                        break;
                    case 2:
                        PART_StrahCompRadio_2.IsChecked = true;
                        break;
                    case 3:
                        PART_StrahCompRadio_3.IsChecked = true;
                        break;
                    case 4:
                        PART_StrahCompRadio_4.IsChecked = true;
                        break;
                }
                PROP_Parent = _Parent;
                PART_ExportType_1.IsChecked = true;               
            }

            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Выбираем какие страховые компании загружать</summary>
        private void RadioButtonStrahComp_Click(object sender, RoutedEventArgs e)
        {
            if (!PRI_FlagPreInfo) return;
            PRI_FlagPreInfo = false;
            PART_ComboBoxParent.SelectedValue = 99999;
            PROP_Parent = 0;
            PRI_Strah = m.MET_ParseInt(((RadioButton)sender).Tag);
            // Загружаем данные
            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Выбираем тип загрузки (Все, Стационар....)</summary>
        private void RadioButtonType_Click(object sender, RoutedEventArgs e)
        {
            if (!PRI_FlagPreInfo) return;
            PRI_FlagPreInfo = false;
            PART_ComboBoxParent.SelectedValue = 99999;
            PROP_Parent = 0;
            PRI_ExportType = m.MET_ParseInt(((RadioButton)sender).Tag);
            // Загружаем данные
            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Тип файла (1- только ВМП, 3 - ЗНО, 4 - без С)</summary>
        private void RadioButtonTipFile_Click(object sender, RoutedEventArgs e)
        {
            if (!PRI_FlagPreInfo) return;
            PRI_FlagPreInfo = false;
            PART_ComboBoxParent.SelectedValue = 99999;
            PROP_Parent = 0;
            PRI_TipFile = m.MET_ParseInt(((RadioButton)sender).Tag);
           
            // Загружаем данные
            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Кнопка - прошлый месяц</summary>
        private void PART_LastButton_Click(object sender, RoutedEventArgs e)
        {
            PRI_FlagPreInfo = false;
            PART_ComboBoxParent.SelectedValue = 99999;
            PROP_Parent = 0;
            PROP_DateN = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, 1);
            PROP_DateK = PROP_DateN.AddMonths(1);
            PROP_DateK = PROP_DateK.AddDays(-1);
            MET_PreInfa();
            PRI_FlagPreInfo = true;

        }

        /// <summary>СОБЫТИЕ Кнопка - текущий месяц</summary>
        private void PART_CarrentButton_Click(object sender, RoutedEventArgs e)
        {
            PRI_FlagPreInfo = false;
            PART_ComboBoxParent.SelectedValue = 99999;
            PROP_Parent = 0;
            PROP_DateN = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            PROP_DateK = PROP_DateN.AddMonths(1);
            PROP_DateK = PROP_DateK.AddDays(-1);
            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Смена начальной даты</summary>
        private void PART_DateN_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!PRI_FlagPreInfo) return;
            PRI_FlagPreInfo = false;
            PART_ComboBoxParent.SelectedValue = 99999;
            PROP_Parent = 0;
            // Пересчитываем начальную инфу
            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }

        /// <summary>СОБЫТИЕ Смена конечной даты</summary>
        private void PART_DateK_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!PRI_FlagPreInfo) return;
            PRI_FlagPreInfo = false;
            PART_ComboBoxParent.SelectedValue = 99999;
            PROP_Parent = 0;
            // Пересчитываем начальную инфу
            MET_PreInfa();
            PRI_FlagPreInfo = true;
        }
        #endregion

        /// <summary>МЕТОД Предварительная информация</summary>
        private void MET_PreInfa()
        {
            // Условия для выборок
            // В зависимости от страховых компаний
            string[] _mStrah = { "in (50, 41, 46)", " = 50", " = 41", " = 46", " > 100" };                  // 0 - все обл., 1 - Альфа, 2 - Капитал, 3 -  ВТБ, 4 - иногородние
            string[] _mPol = { "", "and 1=2", "", "and 1=2", " and 1=2" };                                  // 0 - все, 1 - стац, 2 - поликл, 3 - дн. стац., 4 - пар
            string[] _mStac = { "", "and o.Tip = 1", "and 1=2", "and o.Tip = 3", "and 1=2" };               // 0 - все, 1 - стац, 2 - поликл, 3 - дн. стац., 4 - пар 
            string[] _mPar = { "", "and 1=2", "and 1=2", "and 1=2", "" };                                   // 0 - все, 1 - стац, 2 - поликл, 3 - дн. стац., 4 - пар
            string[] _mTipPol = { "", "and 1=2", "",
                "and (left(a.D, 1) = 'C' or left(a.D, 2) = 'D0')",                                             
                "and (left(a.D, 1) <> 'C' and left(a.D, 2) <> 'D0')" };                                         // 1 - только ВМП, 3 - ЗНО (по умолчанию), 4 - без С
            string[] _mTipStac = { "", "and p.Cod is not null", "",
                "and p.Cod is null and (left(a.D, 1) = 'C' or left(a.D, 2) = 'D0' or left(a.D, 3) = 'D70')",
                "and p.Cod is null and (left(a.D, 1) <> 'C' and left(a.D, 2) <> 'D0' and left(a.D, 3) <> 'D70')" };  // 1 - только ВМП, 3 - ЗНО, 4 - без С
            string[] _mTipPar = { "and 1=2", "and 1=2", "and 1=2", "", "and 1=2" };                             // 1 - только ВМП, 3 - ЗНО (по умолчанию), 4 - без С
            string _Korekt = PROP_Korekt > 0 ? "and a.VZ10 = " + PROP_Korekt : "";                              // по коду корректировки

            // Фрагмент условия выборки поликлиники
            PRI_WerePol = $@"                                                        
                      a.DateCloseTap between '{PROP_DateN:MM.dd.yyyy}' and '{PROP_DateK:MM.dd.yyyy}'  -- по дате посещения
                      and a.Scom {_mStrah[PRI_Strah]}                                       -- по страховой компании                     
                      and a.OMS = 1                                                         -- только ОМС
                      and isnull(a.NumberFirstTap, 0) > 0 
                      and isnull(a.xDelete, 0) = 0                   
                      {_mPol[PRI_ExportType]}                                               -- по типу подразделения
                      {_Korekt}                                                             -- по коду корреткировки
                      {_mTipPol[PRI_TipFile]}                                               -- по типу файла
                ";
            // Фрагмент условия выборки стационара
            PRI_WereStac = $@"                                                                
                where a.DK between '{PROP_DateN:MM.dd.yyyy}' and '{PROP_DateK:MM.dd.yyyy}'  -- по дате посещения
                      and isjson(a.xInfo) > 0
                      and a.ScomEnd {_mStrah[PRI_Strah]}                                    -- по страховой компании                     
                      and a.OMS = 1                                                         -- только ОМС
                      and a.FlagOut > 0                                                     -- флаг выписки     
                      and a.OtdOut = 0                                                      -- отбрасываем переведенных в другие отделения
                      and isnull(a.xDelete, 0) = 0
                      {_mStac[PRI_ExportType]}                                              -- по типу подразделения
                      {_Korekt}                                                             -- по коду корреткировки  
                      {_mTipStac[PRI_TipFile]}                                              -- по типу файла
                ";
            // Фрагмент условия выборки параклиники
            PRI_WerePar = $@"   
                      and i.Oms = 1
                      and p.pDate between '{PROP_DateN:MM.dd.yyyy}' and '{PROP_DateK:MM.dd.yyyy}'  -- по дате посещения
                      and isjson(i.jTag) > 0
                      and k.SCom {_mStrah[PRI_Strah]}                                       -- по страховой компании                     
                      and isnull(p.xDelete, 0) = 0
                      {_mPar[PRI_ExportType]}                                               -- по типу подразделения
                      {(PROP_Korekt > 0 ? "and json_value(i.jTag, '$.Korekt') = " + PROP_Korekt : "")} -- по коду корреткировки
                      {_mTipPar[PRI_TipFile]}                                               -- по типу файла
                ";

            // Фрагмент выборки ReStrahReestr_Select_1
            PRI_SelectAll = @" 
                      ,k.FAM
                      ,k.I
                      ,k.O 
                      ,k.DR
                      ,d.DStrah as D                     
                      ,k.Pol
                      ,k.SNILS
                      ,k.Parents
                      ,do.NewCod as Doc
                      ,k.Bpl as MR
                      ,k.Pasp_Ser
                      ,k.Pasp_Nom
                      ,k.Pasp_Kogda
                      ,k.Pasp_Kem
                      ,k.KL
                      ,isnull(kl.OCATD,'52401000000') as OCATD 
                      ,e.FlagOut as Error                                       -- если 0, то ошибка и не выгружаем
                      ,e.Cod as ErrorCod                                        -- код ошибки
                      ,e.Name as ErrorName                                      -- описание ошибки
                    ";

            // Загружаем заготовку реестра StrahReestr
            MySql.MET_DsAdapterFill(MyQuery.ReStrahReestr_Select_1(PRI_SelectAll, PRI_WerePol, PRI_WereStac, PRI_WerePar), "ReStrahReestr");

            // Максимум ProgressBar
            PART_Info.Content = "Записей: " + MyGlo.DataSet.Tables["ReStrahReestr"].Rows.Count;
        }

        /// <summary>МЕТОД 1. Проверка стартовых параметров и запуск расчета MET_StartCalc</summary>
        private void MET_StartVerification()
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
                    Korekt = PROP_Korekt,
                    StrahComp = (byte?)PRI_Strah,
                    VMP = (byte?)PRI_TipFile,
                    SUMMAV = 0,
                    Tip = 1,
                    YEAR = PROP_DateK.Year,
                    MONTH = PROP_DateK.Month,
                    NSCHET = PROP_Schet,
                    DSCHET = PROP_DateSchet,
                    pHide = 0,
                    pParent = PROP_Parent
                };

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


            // Стартовые номера для поля N_ZAP
            // 0 - Все, 1 - Альфа, 2 - Капитал, 3 - ВТБ, 4 - Иногородние (ЗНО, не С, ВМП)
            int[,] _StartNomer = { { 0, 0, 0 }, { 0, 20000, 29000 }, { 30000, 40000, 49000 }, { 50000, 60000, 69000 }, { 70000, 80000, 89000 } };
            
            PRI_ReeN_Zap = _StartNomer[PRI_Strah, new[] { 0, 2, 0, 0, 1 }[PRI_TipFile]];
            PRI_ReeNom_Zap = PRI_ReeN_Zap;
       
            // Поликлиника 1. Apac (только конечные посещения)
            MySql.MET_DsAdapterFill(MyQuery.ReApac_Select_1(PRI_WerePol), "ReApac");
            // Связываем по колонками
            DataColumn[] _ReStrahReestr = new DataColumn[]
                {   MyGlo.DataSet.Tables["ReStrahReestr"].Columns["Cod"],
                    MyGlo.DataSet.Tables["ReStrahReestr"].Columns["LPU_ST"] };
            DataColumn[] _ReApac = new DataColumn[]
                {   MyGlo.DataSet.Tables["ReApac"].Columns["Cod"],
                    MyGlo.DataSet.Tables["ReApac"].Columns["LPU_ST"] };
            // Связь Реестр - Apac
            DataRelation _DataRelation = new DataRelation("ReReestr_Apac", _ReStrahReestr, _ReApac, false);
            MyGlo.DataSet.Relations.Add(_DataRelation);

            // Поликлиника 2. (промежуточные посещения)
            MySql.MET_DsAdapterFill(MyQuery.ReApac_Select_2(PRI_WerePol), "RePol");
            // Связь Apac - Pol
            _DataRelation = new DataRelation("ReApac_Pol",
                MyGlo.DataSet.Tables["ReApac"].Columns["NumberFirstTap"],
                MyGlo.DataSet.Tables["RePol"].Columns["NumberFirstTap"], false); // Запретим создание ограничений, на "осиротевшие" записи в RePol
            MyGlo.DataSet.Relations.Add(_DataRelation);

            // Поликлиника 3. (консилиумы)
            MySql.MET_DsAdapterFill(MyQuery.ReApac_Select_3(PRI_WerePol), "RePolCons");
            // Связь Apac - PolCons
            _DataRelation = new DataRelation("ReApac_PolCons",
                MyGlo.DataSet.Tables["ReApac"].Columns["Cod"],
                MyGlo.DataSet.Tables["RePolCons"].Columns["Cod"], false); // Запретим создание ограничений, на "осиротевшие" записи в RePolCons
            MyGlo.DataSet.Relations.Add(_DataRelation);

            // Стационар 1. Apstac
            MySql.MET_DsAdapterFill(MyQuery.ReApstac_Select_1(PRI_WereStac), "ReApstac");
            // Связываем по колонками
            DataColumn[] _ReApstac = new DataColumn[]
                {   MyGlo.DataSet.Tables["ReApstac"].Columns["Ind"],
                    MyGlo.DataSet.Tables["ReApstac"].Columns["LPU_ST"] };
            // Связь Реестр - Apstac
            _DataRelation = new DataRelation("ReReestr_Apstac", _ReStrahReestr, _ReApstac, false);
            MyGlo.DataSet.Relations.Add(_DataRelation);
           
           // Стационар 2. Ksg
            MySql.MET_DsAdapterFill(MyQuery.ReApstac_Select_2(PRI_WereStac), "ReKsg");
            // Связь Apstac - Ksg
            _DataRelation = new DataRelation("ReApstac_Ksg",
                MyGlo.DataSet.Tables["ReApstac"].Columns["Ind"],
                MyGlo.DataSet.Tables["ReKsg"].Columns["Ind"], false);
            MyGlo.DataSet.Relations.Add(_DataRelation);

            // Стационар 3. (консилиумы)
            MySql.MET_DsAdapterFill(MyQuery.ReApstac_Select_3(PRI_WereStac), "ReApstacCons");
            // Связь Apstac - ApstacCons
            _DataRelation = new DataRelation("ReApstac_ApstacCons",
                MyGlo.DataSet.Tables["ReApstac"].Columns["Ind"],
                MyGlo.DataSet.Tables["ReApstacCons"].Columns["Ind"], false);
            MyGlo.DataSet.Relations.Add(_DataRelation);

            // Параклиника 1. Par
            MySql.MET_DsAdapterFill(MyQuery.RePar_Select_1(PRI_WerePar), "RePar");
            // Связываем по колонками
            DataColumn[] _RePar = new DataColumn[]
                {   MyGlo.DataSet.Tables["RePar"].Columns["Cod"],
                    MyGlo.DataSet.Tables["RePar"].Columns["LPU_ST"] };
            // Связь Реестр - Apac
            _DataRelation = new DataRelation("ReReestr_Par", _ReStrahReestr, _RePar, false);
            MyGlo.DataSet.Relations.Add(_DataRelation);

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

                // Смотрим что это True - поликлиника, False - стационар
                PRI_TipUsl = (eTipUsl)m.MET_PoleInt("LPU_ST", PRI_RowReestr);

                PRI_ErrorRow = false;                                           // ошибок нет (ну это пока)

                // Заранее готовим информацию для будущей ошибки
                PRI_ErrorToExcel.PROP_StrahReestr = PRI_StrahReestr;
                PRI_ErrorToExcel.PROP_RowReestr = PRI_RowReestr;
                PRI_ErrorToExcel.PROP_Nom++;
                PRI_ErrorToExcel.PROP_Tip = new[] { "", "Кр. Стационар", "Дн. Стационар", "Поликлиника", "Параклиника", "Гистология" } [(int)PRI_TipUsl];
             
                // Ошибки выявленные заранее
                PRI_ErrorToExcel.MET_VeryfError(m.MET_PoleStr("Error", PRI_RowReestr) == "0",
                        m.MET_PoleStr("ErrorCod", PRI_RowReestr),
                        m.MET_PoleStr("ErrorName", PRI_RowReestr),
                        ref PRI_ErrorRow);

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
            if (PROP_Parent > 0)
                MySql.MET_QueryNo(MyQuery.StrahReestr_Update_1(PRI_StrahFile.Cod, PROP_Parent));

            // Пересчитаем поле Сумму
            MySql.MET_QueryNo(MyQuery.StrahFile_Update_1(PRI_StrahFile.Cod));

            // Подчистим связи
            MyGlo.DataSet.Tables["ReApac"].ChildRelations.Clear();
            MyGlo.DataSet.Tables["RePol"].ChildRelations.Clear();
            MyGlo.DataSet.Tables["RePolCons"].ChildRelations.Clear();

            MyGlo.DataSet.Tables["ReApstac"].ChildRelations.Clear();
            MyGlo.DataSet.Tables["ReKsg"].ChildRelations.Clear();
            MyGlo.DataSet.Tables["ReApstacCons"].ChildRelations.Clear();

            MyGlo.DataSet.Tables["RePol"].Clear();
            MyGlo.DataSet.Tables["RePolCons"].Clear();
            MyGlo.DataSet.Tables["ReApac"].Clear();
                        
            MyGlo.DataSet.Tables["ReKsg"].Clear();
            MyGlo.DataSet.Tables["ReApstacCons"].Clear();
            MyGlo.DataSet.Tables["ReApstac"].Clear();

            MyGlo.DataSet.Tables["RePar"].Clear();
            MyGlo.DataSet.Relations.Clear();
            // Усё
            MessageBox.Show(string.Format("Реестр №{0} сформирован!", PRI_StrahFile.Cod));  
            // Показываем ошибки
            PRI_ErrorToExcel.MET_End();
        }        
        
        /// <summary>МЕТОД 3. Расчет Стационара</summary>
        private void MET_CalcStac()
        {
            // Связываем ReStrahReestr и ReApstac
            DataRow _Apstac = PRI_RowReestr.GetChildRows("ReReestr_Apstac")[0];
            
            // Версия редакции 3 (март 2018)
            // Имя поля в таблице StrahReestr (Описание)
            
            // Если ВМП, то расчитываем и заменяем данные
            bool _FlagVMP = m.MET_PoleStr("MetVMP", _Apstac) != "";
            if (_FlagVMP)
            {
                MySql.MET_DsAdapterFill(MyQuery.ReVMP_Select_1(m.MET_PoleStr("Cod", PRI_RowReestr)), "ReReestr_VMP");

                if (MyGlo.DataSet.Tables["ReReestr_VMP"].Rows.Count == 0) // если не нашли ВМП тариф, то ошибка
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "27";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тариф ВМП";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                }
                else
                {
                    DataRow _VMP = MyGlo.DataSet.Tables["ReReestr_VMP"].Rows[0];
                    _Apstac["PROFIL"] = _VMP["PROFIL"];
                    _Apstac["PROFIL_K"] = _VMP["PROFIL_K"];
                    _Apstac["CODE_USL"] = _VMP["CODE_USL"];
                    _Apstac["VID_VME"] = _VMP["VID_VME"];
                    _Apstac["Tarif"] = _VMP["Tarif"];
                    _Apstac["VID_HMP"] = _VMP["VID_HMP"];
                    _Apstac["METOD_HMP"] = _VMP["METOD_HMP"];
                    _Apstac["TAL_NUM"] = _VMP["TAL_NUM"];
                    _Apstac["TAL_D"] = _VMP["TAL_D"];
                }
            }

            // 3. PLAT (Код страховой компании) (28. SMO)                                   далее в  MET_CalcAll() 
            PRI_StrahReestr.PLAT = _Apstac["ScomEnd"].ToString();

            // 7. LPU_1 (Подразделения МО) (44. LPU_1) (39)
            PRI_StrahReestr.LPU_1 = m.MET_PoleInt("Korpus", _Apstac) == 1 ? 55550900 : 55550901;     // главный/филиал

            // 8. ORDER (Направление на госпитализацию, 1 - плановая, 2 - экстренная (у нас нету), 0 - поликлиника) (42. EXTR) (37)
            PRI_StrahReestr.ORDER = 1;

            // 9. LPU_ST (Условия оказания мед. помощи, справочник V006, 1 - кр. стационра, 2 - дн. стационар, 3 - поликлиника) (35 USL_OK) (34)
            int _Depart = m.MET_PoleInt("Depart", _Apstac);           // 1 - кр. стац, 2 -  дн. стац. при стац., 3 - дн. стац. при пол.
            PRI_StrahReestr.LPU_ST = _Depart == 1 ? 1 : 2;

            // 10. VIDPOM (Вид помощи, справочник V008, у нас 13 - поликл, 31 - стационар, 32 - ВМП) (36. VIDPOM) (35)
            PRI_StrahReestr.VIDPOM = _Depart == 3 ? 13 : 31;                                    // если дн. стац. при пол., то 13 иначе 31
            if (_FlagVMP) PRI_StrahReestr.VIDPOM = 32;                                          // если есть код ВМП, то проставляем 32
               

            // 12. PROFIL (Профиль, справочник V002) (46, 80 PROFIL) (41, 73)
            PRI_StrahReestr.PROFIL = m.MET_PoleDec("PROFIL", _Apstac);
            // Проверка
            if (PRI_StrahReestr.PROFIL == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "12";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден профиль";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
            }

            // 11. PODR (Код отделения) (45, 79. PODR) (40, 72)
            PRI_StrahReestr.PODR = m.MET_ParseDec(m.MET_PoleStr("PODR", _Apstac) + ((int)PRI_StrahReestr.PROFIL).ToString("D3"));
            
            // 13. DET (Детский профиль, если ребёнок то 1 иначе 0) (47, 82 DET) (42, 74)   далее в  MET_CalcAll() 
            PRI_Age = m.MET_PoleInt("Age", _Apstac);

            // 14. CODE_USL (Код услуги - для дополнительной оплаты и операций Код услуг прописан програмно, справочник услуг) (86. CODE_USL) (78)
            PRI_StrahReestr.CODE_USL = m.MET_PoleStr("CODE_USL", _Apstac);

            // 15. PRVS (Специальность врача, справочник V004)  (60, 90. PRVS) (53, 82)
            PRI_StrahReestr.PRVS = m.MET_PoleStr("PRVSs", _Apstac);

            // 16. IDDOKT (Код врача, справочник врачей) (61 IDDOKT, 91. CODE_MD) (54, 83)
            PRI_StrahReestr.IDDOKT = m.MET_PoleStr("IDDOKT", _Apstac); 

            // 17. ARR_DATE (Дата начала) (49. DATE_1, 83. DATE_IN) (44, 75)
            PRI_StrahReestr.ARR_DATE = m.MET_PoleDate("DN", _Apstac);

            // 18. EX_DATE (Дата окончания) (50. DATE_2, 84. DATE_OUT) (45, 76)
            PRI_StrahReestr.EX_DATE = m.MET_PoleDate("DK", _Apstac);

            // 19. DS1 (Диагноз) (52. DS1, 85. DS) (47, 77)                                              
            PRI_StrahReestr.DS1 = m.MET_PoleStr("D", PRI_RowReestr);
            // Проверка
            if (PRI_StrahReestr.DS1.Length < 3)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "42";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Что то не так с диагнозом";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // 20. DS2 (Сопутствующий Диагноз - типо Сахарный диабет) (53. DS2) (48)                    
            PRI_StrahReestr.DS2 = "";
          
            // 21. PACIENTID (Номер истории болезни/талона) (48. NHISTORY) (43)        
            PRI_StrahReestr.PACIENTID = m.MET_PoleStr("Cod", PRI_RowReestr);

            // 22. RES_G (Результат обращения/госпитализации, справочник V009), (57. RSLT) (51)
            int[] _RES_G = { 0, 1, 4, 5 };
            int _Rezobr = m.MET_PoleInt("FlagOut", _Apstac);                                    // FlagOut 1 - выписан, 2 - переведен, 3 - умер
            // кр. стационар (101-выписан, 104-переведен, 105-умер)                                                            
            // дневной стационар (201-выписан, 204-переведен, 205-умер)
            PRI_StrahReestr.RES_G = 100 * PRI_StrahReestr.LPU_ST + _RES_G[_Rezobr];

            // 23. ISHOD (Исход заболевания, справочник V012) (59. ISHOD) (52)
            int _ISXOD = m.MET_PoleInt("ISXOD", _Apstac);
            PRI_StrahReestr.ISHOD = _ISXOD == 6                                                 // если исход равен 6 = умер, то 4
                ? 100 * PRI_StrahReestr.LPU_ST + 4                                              // то исход 104 или 204 ухудшение, для кр. и дн. стац.
                : 100 * PRI_StrahReestr.LPU_ST + _ISXOD;                                        // исход 100... кр. стац и 200 дн. стац.

            // 24. IDSP (Код способа оплаты, справочник V010: 33 - круг. стационар и дневн. страц, 29 или 30 - поликлиника, 32 - ВМП) (63. IDSP) (56)
            PRI_StrahReestr.IDSP = 33;
            if (_FlagVMP) PRI_StrahReestr.IDSP = 32;

            // 25. KOL_USL (Койко дней) (87. KOL_USL) (79)
            PRI_StrahReestr.KOL_USL = m.MET_PoleDec("UET3", _Apstac); 

            // 26. TARIF (Тариф) (88. TARIF) (80)
            PRI_StrahReestr.TARIF = m.MET_PoleDec("Tarif", _Apstac);
            // Проверка
            if (PRI_StrahReestr.TARIF == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "26";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тариф";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
            }
           
            // 29. VPOLIS (Тип полиса 1 - старый, 2 - временный, 3 - новый, 4 - электронный, 5 - УЭК) (25. VPOLIS)   далее в  MET_CalcAll() 
            PRI_NPolis = m.MET_PoleStr("SNEnd", _Apstac);

            // 30. SERIA (Серия полиса) (26. SPOLIS)
            PRI_StrahReestr.SERIA = m.MET_PoleStr("SSEnd", _Apstac).ToUpper();

            // 56. DayN (Длительность лечения из Тарифов)
            PRI_StrahReestr.DayN = 0; //m.MET_PoleInt("DayN", _Apstac);

            // 57. VID_VME (Вид медицинского вмешательства) (81. VID_VME)
            PRI_StrahReestr.VID_VME = m.MET_PoleStr("VID_VME", _Apstac);

            // 58. VID_HMP (Вид ВМП, типа 01.001.01, справочник V018) (38. VID_HMP)
            PRI_StrahReestr.VID_HMP = m.MET_PoleStr("VID_HMP", _Apstac);

            // 59. METOD_HMP (Метод ВМП, число, справочник V019) (39. METOD_HMP) 
            PRI_StrahReestr.METOD_HMP = m.MET_PoleStr("METOD_HMP", _Apstac);

            MET_CalcAll();

            MET_CalcKsg(_Apstac);

            try
            {
                PRI_StrahReestr.NOM_USL = JsonConvert.SerializeObject(PRI_Sl,
                                Formatting.Indented,
                                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            }
            catch (Exception)
            {

                PRI_StrahReestr.NOM_USL = "Ошибка json";
            }
        }

        /// <summary>МЕТОД 4. Расчет Стационара (Расчет КСГ)</summary>
        private void MET_CalcKsg(DataRow pApstac)
        {
            // Коэффициент дифференциации
            // double _KdStac = 1.105;
            
            // ЛПУ направления
            PRI_Sl.NPR_MO = m.MET_PoleInt("NPR_MO", pApstac);

            // Дата направления
            PRI_Sl.NPR_DATE = m.MET_PoleStr("DN", pApstac); //m.MET_PoleStr("NPR_DATE", pApstac);
            //  _Sluch.NPR_DATE = _Sluch.NPR_DATE == "" ? m.MET_PoleStr("DN", pApstac) : _Sluch.NPR_DATE;
            PRI_Sl.NPR_DATE = PRI_Sl.NPR_DATE.Remove(10);

            // Переводы
            PRI_Sl.VB_P = m.MET_PoleInt("OtdIn", pApstac);

            // Профиль койки
            PRI_Sl.PROFIL_K = m.MET_PoleInt("PROFIL_K", pApstac);

            string _jTag = m.MET_PoleStr("jTag", pApstac);

            bool _Zno = PRI_StrahReestr.DS1.Substring(0, 1) == "C" || PRI_StrahReestr.DS1.Substring(0, 2) == "D0" || PRI_StrahReestr.DS1.Substring(0, 3) == "D70";
            if (string.IsNullOrEmpty(_jTag) && !_Zno)
                _jTag = "{ }";

            // Проверка на наличия данных KoblInfo
            if (string.IsNullOrEmpty(_jTag))
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "28";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найдена строка APSTAC в KoblInfo";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            JObject _JsonSL;
            try
            {
                _JsonSL = JObject.Parse(_jTag);
            }
            catch
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "30";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в KoblInfo";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // 20. DS2 (Сопутствующий Диагноз - типо Сахарный диабет) (53. DS2) (48)                    
            PRI_StrahReestr.DS2 = (string)_JsonSL["Kslp_diag"] ?? "";

            // Сопутствующий диагноз для D70
            bool _DiagD70 = PRI_StrahReestr.DS1.Substring(0, 3) == "D70";
            if (_DiagD70)
            {
                PRI_StrahReestr.DS2 = (string)_JsonSL["DiagD70"];

                // Обязательно должен быть сопутствующий диагноз С
                if(string.IsNullOrEmpty(PRI_StrahReestr.DS2) || PRI_StrahReestr.DS2.Substring(0, 1) != "C")
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "41";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Отстутствует или не правильный сопутствующий диагноз для D70 в KoblInfo";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }
                
                _JsonSL["Klin_gr"] = "II";
                _JsonSL["DS1_T"] = 6;
            }



            // Новое 285 приказ
            if (_Zno)
            {
                // Направления на исследования, только для ЗНО
                if (PRI_StrahReestr.EX_DATE >= new DateTime(2019, 12, 1))
                {
                    IList<JToken> _Results = _JsonSL["NAPR"]?.Children().ToList();
                    if (_Results != null)
                    {
                        PRI_Sl.NAPR = new List<MyNAPR>();
                        foreach (JToken _result in _Results)
                        {
                            MyNAPR _Napr = _result.ToObject<MyNAPR>();
                            _Napr.NAPR_V = 3;
                            _Napr.NAPR_DATE = PRI_StrahReestr.ARR_DATE.Value.ToString("dd.MM.yyyy");  // !!! Пока берем не из направления, а начальную дату случая
                            PRI_Sl.NAPR.Add(_Napr);
                        }
                    }
                }

                // Характер заболевания 1 - острое, 2 храническое впервые, 3 - хроническое повторно
                // Только для поликлиники или для стационара с диагнозом С..D09
                PRI_Sl.C_ZAB = m.MET_PoleInt("C_Zab", pApstac);

                // Клиническая группа
                string _klin_gr = (string) _JsonSL["Klin_gr"];

                // Проверка на наличия Клинической группы в KoblInfo
                if (string.IsNullOrEmpty(_klin_gr))
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "29";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тег klin_gr (Клиническая группа) в KoblInfo";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }

                // Убрал Пока подозрение в стационаре
                //// DS_ONK - Если подозрение на ЗНО
                //if (_klin_gr == "Ia" || _klin_gr == "Ib")
                //    PRI_Sluch.DS_ONK = 1;
                //else
                    PRI_Sl.DS_ONK = 0;

                // ONK_UL - Блок лечения ЗНО
                if (PRI_Sl.DS_ONK == 0)
                {
                    // Диагноз
                    string _D5 = PRI_StrahReestr.DS1;
                    string _D3 = _D5.Substring(0, 3);

                    PRI_Sl.ONK_SL = new MyONK_SL();

                    // DS1_T - Повод обращения (N018)
                    // 0 - первичное лечение  
                    // 1 - рецедив с метастазами, 2 - прогресирование с метастазами, 21, 22 - то-же но без метастаз)
                    // 3 - динамическое наблюдение
                    // 4 - диспансерное наблюдение
                    // 5 - диагностика
                    // 6 - симптоматическое лечение
                    PRI_Sl.ONK_SL.DS1_T = (int?)_JsonSL["DS1_T"] ?? 0;
                    if (PRI_Sl.ONK_SL.DS1_T == 1 || PRI_Sl.ONK_SL.DS1_T == 2)
                    {
                        // MTSTZ - Отдаленные метастазы
                        PRI_Sl.ONK_SL.MTSTZ = 1;
                    }
                    if (PRI_Sl.ONK_SL.DS1_T == 21 || PRI_Sl.ONK_SL.DS1_T == 22)
                    {
                        PRI_Sl.ONK_SL.DS1_T = PRI_Sl.ONK_SL.DS1_T - 20;
                    }

                    // STAD - Стадия (только для DS1_T < 5)
                    if (PRI_Sl.ONK_SL.DS1_T < 5)
                    {
                        string _stadia = (string) _JsonSL["Stadia"];
                        var _N002 = PRI_N002.Where(e => e.Kod1 == _D5).ToList();
                            // Сначала пытаемся найти по полному диагнозу
                        if (!_N002.Any())
                            _N002 = PRI_N002.Where(e => e.Kod1 == _D3).ToList(); // Иначе ищем по 3м сиволам диагноза
                        if (!_N002.Any())
                            _N002 = PRI_N002.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                        int? _stadiaNumber = _N002.FirstOrDefault(e => e.Kod3.Contains($"\"{_stadia}\""))?.Number ??
                                             _N002.First()?.Number;
                        PRI_Sl.ONK_SL.STAD = _stadiaNumber ?? 145;
                    }

                    // TNM (заполняем только для взрослых и первичном лечении)
                    if (PRI_StrahReestr.DET == 0 && PRI_Sl.ONK_SL.DS1_T == 0)
                    {
                        // ONK_T - T
                        string _T = (string) _JsonSL["T"];
                        var _N003 = PRI_N003.Where(e => e.Kod1 == _D5).ToList();
                            // Сначала пытаемся найти по полному диагнозу
                        if (!_N003.Any())
                            _N003 = PRI_N003.Where(e => e.Kod1 == _D3).ToList(); // Иначе ищем по 3м сиволам диагноза
                        if (!_N003.Any())
                            _N003 = PRI_N003.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                        int? _TNumber = _N003.FirstOrDefault(e => e.Kod3.Contains($"\"{_T}\""))?.Number ??
                                        _N003.First()?.Number;
                        PRI_Sl.ONK_SL.ONK_T = _TNumber ?? 182;

                        // ONK_N - N
                        string _N = (string) _JsonSL["N"];
                        var _N004 = PRI_N004.Where(e => e.Kod1 == _D5).ToList();
                            // Сначала пытаемся найти по полному диагнозу
                        if (!_N004.Any())
                            _N004 = PRI_N004.Where(e => e.Kod1 == _D3).ToList(); // Иначе ищем по 3м сиволам диагноза
                        if (!_N004.Any())
                            _N004 = PRI_N004.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                        int? _NNumber = _N004.FirstOrDefault(e => e.Kod3.Contains($"\"{_N}\""))?.Number ??
                                        _N004.First()?.Number;
                        PRI_Sl.ONK_SL.ONK_N = _NNumber ?? 99;

                        // ONK_M - M
                        string _M = (string) _JsonSL["M"];
                        var _N005 = PRI_N005.Where(e => e.Kod1 == _D5).ToList();
                            // Сначала пытаемся найти по полному диагнозу
                        if (!_N005.Any())
                            _N005 = PRI_N005.Where(e => e.Kod1 == _D3).ToList(); // Иначе ищем по 3м сиволам диагноза
                        if (!_N005.Any())
                            _N005 = PRI_N005.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                        int? _MNumber = _N005.FirstOrDefault(e => e.Kod3.Contains($"\"{_M}\""))?.Number ??
                                        _N005.First()?.Number;
                        PRI_Sl.ONK_SL.ONK_M = _MNumber ?? 56;
                    }

                    // B_DIAG - Диагностический блок 
                    // Гистология
                    string _Gisto = (string) _JsonSL["resulthistology"] ?? "";
                    if (!string.IsNullOrEmpty(_Gisto))
                    {
                        var _mGisto = _Gisto.Split(';');
                        PRI_Sl.ONK_SL.B_DIAG = new List<MyB_DIAG>();
                        // Смотрим если такой диагноз в проверочном файле N009
                        var _N009 = PRI_N009.Where(e => e.Kod1.Contains(_D3)).ToList();
                        bool _DiagN009 = _N009.Any();
                        foreach (var _i in _mGisto)
                        {                           
                            if (int.TryParse(_i, out int j))
                            {
                                var _Daignostic = new MyB_DIAG();
                                _Daignostic.DIAG_DATE = MET_VerifDate((string)_JsonSL["DateDirectHistology"], "DateDirectHistology") ?? PRI_StrahReestr.ARR_DATE.Value.ToString("dd.MM.yyyy");
                                _Daignostic.DIAG_TIP = 1;
                                _Daignostic.DIAG_RSLT = j;
                                _Daignostic.DIAG_CODE = PRI_N008.FirstOrDefault(e => e.Number == j).ID1.Value;
                                if (_DiagN009)
                                {
                                    if (_N009.Any(e => e.ID1 == _Daignostic.DIAG_CODE))
                                        PRI_Sl.ONK_SL.B_DIAG.Add(_Daignostic);
                                }
                                else
                                    PRI_Sl.ONK_SL.B_DIAG.Add(_Daignostic);
                            }
                        }
                    }
                    // ИГХ
                    _Gisto = (string) _JsonSL["markerigh"] ?? "";
                    if (!string.IsNullOrEmpty(_Gisto))
                    {
                        var _mGisto = _Gisto.Split(';');
                        // Смотрим если такой диагноз в проверочном файле N009
                        var _N012 = PRI_N012.Where(e => e.Kod1.Contains(_D3)).ToList();
                        if (PRI_Sl.ONK_SL.B_DIAG == null)
                            PRI_Sl.ONK_SL.B_DIAG = new List<MyB_DIAG>();
                        bool _DiagN012 = _N012.Any();
                        foreach (var _i in _mGisto)
                        {                            
                            if (int.TryParse(_i, out int j))
                            {
                                var _Daignostic = new MyB_DIAG();
                                _Daignostic.DIAG_DATE = MET_VerifDate((string)_JsonSL["DateDirectHistology"], "DateDirectHistology") ?? PRI_StrahReestr.ARR_DATE.Value.ToString("dd.MM.yyyy");
                                _Daignostic.DIAG_TIP = 2;
                                _Daignostic.DIAG_RSLT = j;
                                _Daignostic.DIAG_CODE = PRI_N011.FirstOrDefault(e => e.Number == j).ID1.Value;
                                if (_DiagN012)
                                {
                                    if (_N012.Any(e => e.ID1 == _Daignostic.DIAG_CODE))
                                        PRI_Sl.ONK_SL.B_DIAG.Add(_Daignostic);
                                }
                                else
                                    PRI_Sl.ONK_SL.B_DIAG.Add(_Daignostic);
                            }
                        }
                    }

                    // CONS Блок о проведении консилиума
                    // Связываем ReApstac и ReApstacCons
                    foreach (DataRow _ApstacRowCons in pApstac.GetChildRows("ReApstac_ApstacCons"))
                    {
                        switch (m.MET_PoleInt("PR_CONS", _ApstacRowCons))
                        {
                            case 1:
                                PRI_Sl.Taktika_1 = MET_VerifDate(m.MET_PoleStr("DT_CONS", _ApstacRowCons), $"консилиума", true, false);
                                if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                break;
                            case 2:
                                PRI_Sl.Taktika_2 = MET_VerifDate(m.MET_PoleStr("DT_CONS", _ApstacRowCons), $"консилиума", true, false);
                                if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                break;
                            case 3:
                                PRI_Sl.Taktika_3 = MET_VerifDate(m.MET_PoleStr("DT_CONS", _ApstacRowCons), $"консилиума", true, false);
                                if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                break;
                        }
                    }                 

                    // B_PROT Блок об имеющихся противопоказаниях и отказах
                    PRI_Sl.PrOt_1 = MET_VerifDate((string)_JsonSL["PrOt_1"], "PrOt_1", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_2 = MET_VerifDate((string)_JsonSL["PrOt_2"], "PrOt_2", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_3 = MET_VerifDate((string)_JsonSL["PrOt_3"], "PrOt_3", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_4 = MET_VerifDate((string)_JsonSL["PrOt_4"], "PrOt_4", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_5 = MET_VerifDate((string)_JsonSL["PrOt_5"], "PrOt_5", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_6 = MET_VerifDate((string)_JsonSL["PrOt_6"], "PrOt_6", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                }
            }

            int _CountUsl = pApstac.GetChildRows("ReApstac_Ksg").Length;

            if (_Zno && PRI_Sl.DS_ONK == 0 && PRI_Sl.ONK_SL.DS1_T < 3)
                PRI_Sl.ONK_SL.ONK_USL = new List<MyONK_USL>();

            PRI_CouUsl = 0;
            // Связываем ReApstac и ReKsg
            foreach (DataRow _KsgRow in pApstac.GetChildRows("ReApstac_Ksg"))
            {
                var _MyUSL = new MyUSL
                {
                    Tip = m.MET_PoleStr("Tip", _KsgRow),
                    Usl = m.MET_PoleStr("Usl", _KsgRow),
                    DopUsl = m.MET_PoleStr("DopUsl", _KsgRow),
                    Frakc = m.MET_PoleInt("FrakcT", _KsgRow),
                    DatN = m.MET_PoleStr("Dat", _KsgRow),
                    Ksg = m.MET_PoleStr("KSG", _KsgRow),
                    Fact = m.MET_PoleRea("Factor", _KsgRow),
                    UprFactor = m.MET_PoleRea("UprFactor", _KsgRow),
                    KUSmo = m.MET_PoleInt("KUSmo", _KsgRow),
                    Day3 = m.MET_PoleInt("Day3", _KsgRow)
                };

                // Удаляем все химии, если это не первая услуга!!!!!
                if (PRI_CouUsl > 0 && _MyUSL.Usl.StartsWith("sh") && !PRI_Sl.USL[0].Usl.StartsWith("sh"))
                    continue;

                PRI_CouUsl++;
                               
                // Удаляем если пусто
                _MyUSL.DatN = _MyUSL.DatN == "" ? null : _MyUSL.DatN.Remove(10);
                _MyUSL.DatK = MET_VerifDate(_MyUSL.DatN, "DatN", true);
                if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                _MyUSL.DopUsl = _MyUSL.DopUsl == "" ? null : _MyUSL.DopUsl;
                if (PRI_StrahReestr.KOL_USL > 3) _MyUSL.Day3 = null;

                // Для НЕ ЗНО и Радиологии, записываем фракции в CRIT
                if (!_Zno && _MyUSL.Frakc > 0)
                {
                    string _FracCRIT;
                    // Если фракций нет в Группировщике, то сами их рисуем
                    if (_MyUSL.Frakc <= 5) _FracCRIT = "fr01-05";
                    else if (_MyUSL.Frakc <= 7) _FracCRIT = "fr06-07";
                    else if (_MyUSL.Frakc <= 10) _FracCRIT = "fr08-10";
                    else if (_MyUSL.Frakc <= 20) _FracCRIT = "fr11-20";
                    else if (_MyUSL.Frakc <= 29) _FracCRIT = "fr21-29";
                    else if (_MyUSL.Frakc <= 32) _FracCRIT = "fr30-32";
                    else _FracCRIT = "fr33-99";

                    // Добавляем фракции в CRIT
                    if (PRI_Sl.CRIT == null)
                        PRI_Sl.CRIT = new List<string>();
                    PRI_Sl.CRIT.Add(_FracCRIT);

                    string _xInfo = m.MET_PoleStr("xInfo", _KsgRow);

                    _xInfo = string.IsNullOrEmpty(_xInfo) ? "{}" : _xInfo;
                    JObject _JsonUsl;
                    try
                    {
                        _JsonUsl = JObject.Parse(_xInfo);
                    }
                    catch
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "31";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в услугах Oper";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }

                    string[] _mDates = ((string)_JsonUsl["TLuch_Date"]).Split(';');
                    DateTime _DateNU = DateTime.Parse(_mDates[0]);
                    DateTime _DateKU = DateTime.Parse(_mDates[_mDates.Length - 2]);
                    _MyUSL.DatN = _DateNU.ToString("dd.MM.yyyy");
                    _MyUSL.DatK = _DateKU.ToString("dd.MM.yyyy");
                    if (PRI_StrahReestr.ARR_DATE > _DateNU || PRI_StrahReestr.EX_DATE < _DateKU)
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "33";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) В теге радиологии, даты облучения выходят за диапазон стационара";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }
                }

                // Для 285 приказа
                if (_Zno && PRI_Sl.DS_ONK == 0 && PRI_Sl.ONK_SL.DS1_T < 3)
                {
                    Random _Random = new Random();

                    string _xInfo = m.MET_PoleStr("xInfo", _KsgRow);

                    _xInfo = string.IsNullOrEmpty(_xInfo) ? "{}" : _xInfo;
                    JObject _JsonUsl;
                    try
                    {
                        _JsonUsl = JObject.Parse(_xInfo);
                    }
                    catch
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "31";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в услугах Oper";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }

                    MyONK_USL _MyOnkUsl = new MyONK_USL();
                     

                    // Тип услуги по умолчанию  
                    _MyOnkUsl.USL_TIP = 5;

                    // Если есть только диагноз
                    if (_CountUsl == 1 && _MyUSL.Tip == "диаг")
                    {
                        // Тип услуги  
                        _MyOnkUsl.USL_TIP = 5;
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Биопсия, исследования и др.
                    if (_MyUSL.Usl.StartsWith("A11") || _MyUSL.Usl.StartsWith("B03") || _MyUSL.Usl.StartsWith("A03"))
                    {
                        // Тип услуги  
                        _MyOnkUsl.USL_TIP = 5;
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Хирургического лечение
                    if (_MyUSL.Usl.StartsWith("A16"))
                    {
                        // Тип услуги  
                        _MyOnkUsl.USL_TIP = 1;
                        // Тип хирургического лечения (ПО УМОЛЧАНИЮ 1)
                        int _Hir = (int?)_JsonUsl["THir"] ?? 0;
                        _MyOnkUsl.HIR_TIP = _Hir > 0 & _Hir < 6 ? _Hir : 1;
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Химия
                    if (_MyUSL.Usl.StartsWith("sh") || _MyUSL.Usl.StartsWith("A25"))
                    {
                        _MyOnkUsl.LEK_PR = new List<MyLEK_PR>();
                        // Тип услуги  
                        _MyOnkUsl.USL_TIP = 2;
                        // Тип Линия  (ПО УМОЛЧАНИЮ РАНДОМ ЗАМЕНИТЬ)
                        int _TIP_L = (int?)_JsonUsl["TLek_L"] ?? 0;
                        _MyOnkUsl.LEK_TIP_L = _TIP_L > 0 ? _TIP_L : _Random.Next(1, 4);
                        // Тип Цикл  (ПО УМОЛЧАНИЮ РАНДОМ ЗАМЕНИТЬ)
                        int _TIP_V = (int?)_JsonUsl["TLek_V"] ?? 0;
                        _MyOnkUsl.LEK_TIP_V = _TIP_V > 0 ? _TIP_V : _Random.Next(1, 2);

                        // Тошнота (РАНДОМ ЗАМЕНИТЬ)
                        int _PPTR = (int?)_JsonUsl["PPTR"] ?? 0;
                        if (_PPTR == 1 || _Random.Next(1, 8) == 1)
                            _MyOnkUsl.PPTR = 1;

                        // Полный список всех дат, при сверхкоротком случае (что бы посчетать колличество ВСЕХ дней введения)
                        var _DateList = new List<string>();

                        DateTime _DateN = DateTime.Parse(_MyUSL.DatN);
                        DateTime _DateK = DateTime.Parse(_MyUSL.DatK);

                        // Загружаем препараты
                        for (int i = 1; i < 6; i++)
                        {
                            string _RegNum = (string) _JsonUsl[$"TLek_MNN{i}"] ?? "";
                            if (_RegNum != "")
                            {
                                // Проверяем код МНН по шаблону (6 цифр)
                                Regex _Regex = new Regex(@"\d{6}");
                                if (!_Regex.IsMatch(_RegNum))
                                {
                                    PRI_ErrorToExcel.PROP_ErrorCod = "32";
                                    PRI_ErrorToExcel.PROP_ErrorName = $"(вну) В теге химии стоит неправильный код МНН {_RegNum}";
                                    PRI_ErrorToExcel.MET_SaveError();
                                    PRI_ErrorRow = true;
                                    return;
                                }

                                string _Date = (string) _JsonUsl[$"TLek_Date{i}"] ?? "";
                                if (_Date == "")
                                {
                                    PRI_ErrorToExcel.PROP_ErrorCod = "32";
                                    PRI_ErrorToExcel.PROP_ErrorName = $"(вну) В теге химии, для {i}-го препарата с кодом {_RegNum} отсутствуют даты введения";
                                    PRI_ErrorToExcel.MET_SaveError();
                                    PRI_ErrorRow = true;
                                    return;
                                }

                                var _LekPr = new MyLEK_PR();
                                _LekPr.REGNUM = _RegNum;
                                _LekPr.CODE_SH = _MyUSL.Usl.StartsWith("sh") ? _MyUSL.Usl : "нет";
                                _LekPr.DATE_INJ = new List<string>();
                                
                                string[] _mDates = _Date.Split(';');
                                foreach (var _d in _mDates)
                                {                                    
                                    if (DateTime.TryParse(_d, out DateTime _DateTime))
                                    {
                                        if (_DateN > _DateTime)
                                        {
                                            _DateN = _DateTime;                                           
                                            _MyUSL.DatN = MET_VerifDate(_DateN.ToString("dd.MM.yyyy"), $"химии, для {i}-го препарата с кодом {_RegNum} дата введения", true);
                                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим 
                                        }
                                        if (_DateK < _DateTime)
                                        {
                                            _DateK = _DateTime;
                                            _MyUSL.DatK = MET_VerifDate(_DateK.ToString("dd.MM.yyyy"), $"химии, для {i}-го препарата с кодом {_RegNum} дата введения", true);
                                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                        }
                                        _LekPr.DATE_INJ.Add(_DateTime.ToString("dd.MM.yyyy"));                                       
                                    }
                                }
                                _MyOnkUsl.LEK_PR.Add(_LekPr);

                                // Если сверхкороткий случай, то набираем стек дат, со всех выданных лекарств
                                if (PRI_StrahReestr.KOL_USL < 4)
                                {
                                    _DateList.AddRange(_LekPr.DATE_INJ);
                                }
                            }                            
                        }

                        // Если дней госпитализации менее 4 дней, то смотрим выполнение дней схемы лечения
                        if (PRI_StrahReestr.KOL_USL < 4)
                        {
                            // Колличество дней схемы лечения
                            int _DayHim = m.MET_PoleInt("DayHim", _KsgRow);

                            // Если схема не выполненна, то считаем случай - сверхкороткий
                            if (_DateList.Distinct().Count() < _DayHim)
                                _MyUSL.Day3 = 0;                            
                        }

                        // Проверяем, есть ли хоть один препарат
                        if (_MyOnkUsl.LEK_PR.Count == 0)
                        {
                            PRI_ErrorToExcel.PROP_ErrorCod = "36";
                            PRI_ErrorToExcel.PROP_ErrorName = $"(вну) В услуге химии {_MyUSL.Usl} отсутствуют теги с препаратами";
                            PRI_ErrorToExcel.MET_SaveError();
                            PRI_ErrorRow = true;
                            return;
                        }
                       
                        if (_MyUSL.Usl.StartsWith("sh"))
                        {
                            if (PRI_Sl.CRIT == null)
                                PRI_Sl.CRIT = new List<string>();
                            PRI_Sl.CRIT.Add(_MyUSL.Usl);
                        }

                        //if (_MyUSL.Usl.StartsWith("A25"))
                        //{
                        //    if (PRI_Sl.CRIT == null)
                        //        PRI_Sl.CRIT = new List<string>();
                        //    PRI_Sl.CRIT.Add("нет");
                        //}
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Радиология
                    if (_MyUSL.Usl.StartsWith("A06") || _MyUSL.Usl.StartsWith("A07"))
                    {
                        // Тип услуги  
                        _MyOnkUsl.USL_TIP = 3;
                        // Тип Линия  (ПО УМОЛЧАНИЮ РАНДОМ ЗАМЕНИТЬ)
                        int _LUCH_TIP = (int?)_JsonUsl["TLuch"] ?? 0;
                        _MyOnkUsl.LUCH_TIP = _LUCH_TIP > 0 ? _LUCH_TIP : 1;
                        // Проверка на SOD
                        int _SOD = (int?)_JsonSL["Sod"] ?? 0;
                        if (_SOD > 0)
                            PRI_Sl.ONK_SL.SOD = _SOD;
                        else
                            PRI_Sl.ONK_SL.SOD = 20;

                        // Колличество фракций
                        _MyOnkUsl.K_FR = (int?)_JsonUsl["Frakci"] ?? -1;

                        if (_MyOnkUsl.K_FR > 0)
                        {
                            string _FracCRIT = m.MET_PoleStr("FrakcText", _KsgRow);
                            // Если фракций нет в Группировщике, то сами их рисуем
                            if (_FracCRIT == "")
                            {
                                if (_MyOnkUsl.K_FR <= 5) _FracCRIT = "fr01-05";
                                else if (_MyOnkUsl.K_FR <= 7) _FracCRIT = "fr06-07";
                                else if (_MyOnkUsl.K_FR <= 10) _FracCRIT = "fr08-10";
                                else if (_MyOnkUsl.K_FR <= 20) _FracCRIT = "fr11-20";
                                else if (_MyOnkUsl.K_FR <= 29) _FracCRIT = "fr21-29";
                                else if (_MyOnkUsl.K_FR <= 32) _FracCRIT = "fr30-32";
                                else _FracCRIT = "fr33-99";
                            }

                            // Добавляем фракции в CRIT
                            if (PRI_Sl.CRIT == null)
                                PRI_Sl.CRIT = new List<string>();
                            PRI_Sl.CRIT.Add(_FracCRIT);
                        }

                        string[] _mDates = ((string) _JsonUsl["TLuch_Date"]).Split(';');
                        DateTime _DateNU = DateTime.Parse(_mDates[0]);
                        DateTime _DateKU = DateTime.Parse(_mDates[_mDates.Length - 2]);
                        _MyUSL.DatN = _DateNU.ToString("dd.MM.yyyy");
                        _MyUSL.DatK = _DateKU.ToString("dd.MM.yyyy");
                        if (PRI_StrahReestr.ARR_DATE > _DateNU || PRI_StrahReestr.EX_DATE < _DateKU)
                        {
                            PRI_ErrorToExcel.PROP_ErrorCod = "33";
                            PRI_ErrorToExcel.PROP_ErrorName = "(вну) В теге радиологии, даты облучения выходят за диапазон стационара";
                            PRI_ErrorToExcel.MET_SaveError();
                            PRI_ErrorRow = true;
                            return;
                        }

                        if (_MyUSL.DopUsl != null && _MyUSL.DopUsl.StartsWith("mt"))
                        {
                            _MyOnkUsl.LEK_PR = new List<MyLEK_PR>();
                            _MyOnkUsl.USL_TIP = 4;
                            if (PRI_Sl.CRIT == null)
                                PRI_Sl.CRIT = new List<string>();
                            PRI_Sl.CRIT.Add(_MyUSL.DopUsl);

                            string _RegNum = (string) _JsonUsl["TLek_MNN1"] ?? "";
                            if (_RegNum != "")
                            {
                                // Проверяем код МНН по шаблону (6 цифр)
                                Regex _Regex = new Regex(@"\d{6}");
                                if (!_Regex.IsMatch(_RegNum))
                                {
                                    PRI_ErrorToExcel.PROP_ErrorCod = "33";
                                    PRI_ErrorToExcel.PROP_ErrorName =
                                        $"(вну) В теге химии у ЛУЧЕВОЙ терапии стоит неправильный код МНН {_RegNum}";
                                    PRI_ErrorToExcel.MET_SaveError();
                                    PRI_ErrorRow = true;
                                    return;
                                }

                                string _Date = (string) _JsonUsl["TLek_Date1"] ?? "";
                                if (_Date == "")
                                {
                                    PRI_ErrorToExcel.PROP_ErrorCod = "33";
                                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) В теге химии у ЛУЧЕВОЙ терапии отсутствуют даты введения";
                                    PRI_ErrorToExcel.MET_SaveError();
                                    PRI_ErrorRow = true;
                                    return;
                                }

                                var _LekPr = new MyLEK_PR();
                                _LekPr.REGNUM = _RegNum;
                                _LekPr.CODE_SH = _MyUSL.DopUsl;
                                _LekPr.DATE_INJ = new List<string>();

                                DateTime _DateN = DateTime.Parse(_MyUSL.DatN);
                                DateTime _DateK = DateTime.Parse(_MyUSL.DatK);

                                _mDates = _Date.Split(';');
                                foreach (var _d in _mDates)
                                {                                     
                                    if (DateTime.TryParse(_d, out DateTime _DateTime))
                                    {
                                        if (_DateN > _DateTime)
                                        {
                                            _DateN = _DateTime;
                                            _MyUSL.DatN = MET_VerifDate(_DateN.ToString("dd.MM.yyyy"), $"химии у ЛУЧЕВОЙ терапии дата введения", true);
                                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим                                            
                                        }
                                        if (_DateK < _DateTime)
                                        {
                                            _DateK = _DateTime;
                                            _MyUSL.DatK = MET_VerifDate(_DateK.ToString("dd.MM.yyyy"), $"химии у ЛУЧЕВОЙ терапии дата введения", true);
                                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим                                              
                                        }
                                        _LekPr.DATE_INJ.Add(_DateTime.ToString("dd.MM.yyyy"));
                                    }
                                }
                                _MyOnkUsl.LEK_PR.Add(_LekPr);
                            }
                            else
                            {
                                PRI_ErrorToExcel.PROP_ErrorCod = "38";
                                PRI_ErrorToExcel.PROP_ErrorName = $"(вну) В лучевой химии отсутствуют сведенья о препарате {_RegNum}";
                                PRI_ErrorToExcel.MET_SaveError();
                                PRI_ErrorRow = true;
                                return;
                            }

                        }
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Проверяем на наличие онко услуг
                    if (PRI_Sl.ONK_SL.ONK_USL.Count == 0)
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "37";
                        PRI_ErrorToExcel.PROP_ErrorName = $"(вну) Какая то Неведомая услуга {_MyUSL.Usl}";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }
                }

                PRI_Sl.USL.Add(_MyUSL);
                
            }

            // Проверяем есть химия
            if (PRI_Sl.ONK_SL != null && PRI_Sl.ONK_SL.ONK_USL != null && PRI_Sl.ONK_SL.ONK_USL.Any(p => p.USL_TIP == 2 || p.USL_TIP == 4))
            {
                // Вес
                PRI_Sl.ONK_SL.WEI = (double?) _JsonSL["Ves"] ?? new Random().Next(50, 100);
                if (PRI_Sl.ONK_SL.WEI == 0)
                    PRI_Sl.ONK_SL.WEI = new Random().Next(50, 100);
                // Рост
                PRI_Sl.ONK_SL.HEI = (int?)_JsonSL["Rost"] ?? new Random().Next(155, 185);
                if (PRI_Sl.ONK_SL.HEI == 0)
                    PRI_Sl.ONK_SL.HEI = new Random().Next(155, 185);
                // Объем тела
                PRI_Sl.ONK_SL.BSA = Math.Round(Math.Sqrt(PRI_Sl.ONK_SL.WEI*PRI_Sl.ONK_SL.HEI/3600), 2);
            }

            // Если не нашли услуги в группировщике, то выводим ошибку
            if (PRI_Sl.USL.Count == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "57";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найдена ни одна услуга в Групировщике по диагнозу и(или) услуге";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // Исключения по КСГ (приложение 15)
            if (PRI_CouUsl > 1)
            {
                // Потом убрать
                if (PRI_StrahReestr.EX_DATE >= new DateTime(2020, 1, 1))
                {
                    // Если выигрывает диагноз, но есть такая услуга, то берем услугу
                    if (PRI_Sl.USL[1].Tip == "опер" && PRI_Sl.USL[0].Tip == "диаг" &&
                        ((PRI_Sl.USL[1].Ksg == "st02.010" && PRI_Sl.USL[0].Ksg == "st02.008") ||
                            (PRI_Sl.USL[1].Ksg == "st02.011" && PRI_Sl.USL[0].Ksg == "st02.008") ||
                            (PRI_Sl.USL[1].Ksg == "st02.010" && PRI_Sl.USL[0].Ksg == "st02.009") ||
                            (PRI_Sl.USL[1].Ksg == "st14.001" && PRI_Sl.USL[0].Ksg == "st04.002") ||
                            (PRI_Sl.USL[1].Ksg == "st14.002" && PRI_Sl.USL[0].Ksg == "st04.002") ||
                            (PRI_Sl.USL[1].Ksg == "st21.001" && PRI_Sl.USL[0].Ksg == "st21.007") ||
                            (PRI_Sl.USL[1].Ksg == "st34.002" && PRI_Sl.USL[0].Ksg == "st34.001") ||
                            (PRI_Sl.USL[1].Ksg == "st34.002" && PRI_Sl.USL[0].Ksg == "st26.001")))
                    {
                        PRI_Sl.USL.Remove(PRI_Sl.USL[0]);
                        PRI_CouUsl--;
                    }
                }
                else
                {
                    // Если выигрывает диагноз, но есть такая услуга, то берем услугу
                    if (PRI_Sl.USL[1].Tip == "опер" && PRI_Sl.USL[0].Tip == "диаг" &&
                        ((PRI_Sl.USL[1].Ksg == "st02.010" && PRI_Sl.USL[0].Ksg == "st02.008") ||
                            (PRI_Sl.USL[1].Ksg == "st02.011" && PRI_Sl.USL[0].Ksg == "st02.008") ||
                            (PRI_Sl.USL[1].Ksg == "st02.010" && PRI_Sl.USL[0].Ksg == "st02.009") ||
                            (PRI_Sl.USL[1].Ksg == "st14.001" && PRI_Sl.USL[0].Ksg == "st04.002") ||
                            (PRI_Sl.USL[1].Ksg == "st14.002" && PRI_Sl.USL[0].Ksg == "st04.002") ||
                            (PRI_Sl.USL[1].Ksg == "st21.001" && PRI_Sl.USL[0].Ksg == "st21.007") ||
                            (PRI_Sl.USL[1].Ksg == "st34.002" && PRI_Sl.USL[0].Ksg == "st34.001") ||
                            (PRI_Sl.USL[1].Ksg == "st34.002" && PRI_Sl.USL[0].Ksg == "st26.001") ||
                            (PRI_Sl.USL[1].Ksg == "st30.006" && PRI_Sl.USL[0].Ksg == "st30.003") ||
                            (PRI_Sl.USL[1].Ksg == "st09.001" && PRI_Sl.USL[0].Ksg == "st30.005") ||
                            (PRI_Sl.USL[1].Ksg == "st31.002" && PRI_Sl.USL[0].Ksg == "st31.017")))
                    {
                        PRI_Sl.USL.Remove(PRI_Sl.USL[0]);
                        PRI_CouUsl--;
                    }
                }
            }

            // Если дневной стационар или ВМП, то пропускаем КСЛП 
            if (PRI_StrahReestr.LPU_ST == 1 && PRI_StrahReestr.METOD_HMP == "")
            {
                // Смотрим КСЛП для круглосуточного стационара

                // Возраст, при поступлении
                DateTime _Dn = PRI_StrahReestr.ARR_DATE.Value;
                DateTime _DR = PRI_StrahReestr.VOZRAST.Value;
                int _Age = _Dn.Year - _DR.Year;
                if (_Dn.Month < _DR.Month || (_Dn.Month == _DR.Month && _Dn.Day < _DR.Day)) _Age--;

                // КСЛП 13 - дети до года (приложение 17.1)
                if (_Age < 1)
                {
                    PRI_Sl.Sl13 = 0.15; // до года
                    PRI_Sl.IT_SL += PRI_Sl.Sl13;
                }

                // КСЛП 14 - дети от 1 до 4 лет (приложение 17.1)
                if (_Age < 4 && _Age > 0)
                {
                    PRI_Sl.Sl14 = 0.1; // до года
                    PRI_Sl.IT_SL += PRI_Sl.Sl14;
                }

                // 10 - Сверх длинный случай (после 30 и 45 дня)
                if (PRI_StrahReestr.KOL_USL > 30)
                {

                    // КСГ, при которых сверхдлительность НЕ учитывается (тарифное соглашение)
                    bool _NkdNO = new[]
                    {
                        "st19.039", "st19.040", "st19.041", "st19.042", "st19.043", "st19.044", "st19.045", "st19.046", "st19.047"
                        , "st19.048", "st19.049", "st19.050", "st19.051", "st19.052", "st19.053", "st19.054", "st19.055"
                    }.Contains(PRI_Sl.USL[0].Ksg);

                    if (!_NkdNO)
                    {

                        // КСГ, при которых сверхдлительность идет, только после 45 дня (приложение 17)
                        bool _Nkd45 =
                            new[] {"st10.001", "st10.002", "st32.006", "st32.007"}.Contains(PRI_Sl.USL[0].Ksg);

                        // Больше 30 дней
                        if (!_Nkd45)
                        {
                            PRI_Sl.Sl10 = Math.Round((Convert.ToDouble(PRI_StrahReestr.KOL_USL) - 30) / 30 * 0.3, 2);
                            PRI_Sl.IT_SL += PRI_Sl.Sl10;
                        }

                        // Больше 45 дней
                        if (_Nkd45 && PRI_StrahReestr.KOL_USL > 45)
                        {
                            PRI_Sl.Sl10 = Math.Round((Convert.ToDouble(PRI_StrahReestr.KOL_USL) - 45) / 45 * 0.3, 2);
                            PRI_Sl.IT_SL += PRI_Sl.Sl10;
                        }
                    }
                }

                PRI_Sl.IT_SL = Math.Round(PRI_Sl.IT_SL + 1, 2);

                // Коэффицент подуровня стационара (КУСмо)
                if (PRI_Sl.USL[0].KUSmo == 0)
                // Если не сказанно, что нужно ингорировать коэффициет подуровня (из dbo.StrahKsg поля KUSmo = 1)
                {
                    PRI_Sl.KOEF_U = 1.3;

                    // Для ВМП отделений другой коэффициента
                    if (PRI_StrahReestr.EX_DATE >= new DateTime(2020, 1, 1))
                    {
                        if (new[] { 11121060m, 10991060m, 11361060m, 11081060m }.Contains((decimal)PRI_StrahReestr.PODR))
                            PRI_Sl.KOEF_U = 1.15;
                    }
                }
                else
                    PRI_Sl.KOEF_U = 1;

                // Коэфициент затратоёмкости
                PRI_Sl.KOEF_Z = PRI_Sl.USL[0].Fact;

                // Управленчиский коэффицент
                PRI_Sl.KOEF_UP = PRI_Sl.USL[0].UprFactor;

                // Основной коэффициет
                PRI_Sl.SUMV = (double) PRI_StrahReestr.TARIF*PRI_Sl.USL[0].Fact*
                                    PRI_Sl.USL[0].UprFactor*PRI_Sl.IT_SL*PRI_Sl.KOEF_U;

                if (PRI_Sl.IT_SL == 1)
                    PRI_Sl.IT_SL = 0;

                // Сверх короткий случай
                if (PRI_StrahReestr.KOL_USL < 4 && PRI_Sl.USL[0]?.Day3 != 1)
                {
                    // 0.8 только операциям
                    if (PRI_Sl.USL[0].Tip == "опер" && (PRI_Sl.USL[0].Usl.StartsWith("A16") || PRI_Sl.USL[0].Usl.StartsWith("A11") || PRI_Sl.USL[0].Usl.StartsWith("A03"))) 
                        PRI_Sl.Short = 0.8;
                    else
                        PRI_Sl.Short = 0.4;

                    PRI_Sl.SUMV *= PRI_Sl.Short;
                }
            }
            else
            {
                // Если дневной стационар
                if (PRI_StrahReestr.LPU_ST == 2)
                {
                    // Коэфициент затратоёмкости
                    PRI_Sl.KOEF_Z = PRI_Sl.USL[0].Fact;

                    // Управленчиский коэффицент
                    PRI_Sl.KOEF_UP = PRI_Sl.USL[0].UprFactor;

                    // Коэффицент подуровня
                    PRI_Sl.KOEF_U = 1;

                    PRI_Sl.SUMV = (double) PRI_StrahReestr.TARIF*PRI_Sl.USL[0].Fact;

                    // Сверх короткий случай (если нет операций (не Ашки), а только диагноз)
                    if (PRI_StrahReestr.KOL_USL < 4 && PRI_Sl.USL[0].Day3 != 1)
                    {
                        // 0.8 только операциям
                        if (PRI_Sl.USL[0].Tip == "опер" && (PRI_Sl.USL[0].Usl.StartsWith("A16") || PRI_Sl.USL[0].Usl.StartsWith("A11") || PRI_Sl.USL[0].Usl.StartsWith("A03")))
                            PRI_Sl.Short = 0.8;
                        else
                            PRI_Sl.Short = 0.4;
                        
                        PRI_Sl.SUMV *= PRI_Sl.Short;
                    }
                }

                // Если ВМП
                if (PRI_StrahReestr.METOD_HMP != "")
                {
                    PRI_Sl.SUMV = (double) PRI_StrahReestr.TARIF;
                    PRI_Sl.USL[0].Ksg = "";

                    // Номер талона ВМП
                    PRI_Sl.TAL_NUM = m.MET_PoleStr("TAL_NUM", pApstac);
                    // Проверка
                    if (PRI_Sl.TAL_NUM.Length < 17)
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "55";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Мало символов в Номере талона ВМП";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                    }

                    // Дата создания талона
                    PRI_Sl.TAL_D = m.MET_PoleStr("TAL_D", pApstac);
                    PRI_Sl.TAL_D = PRI_Sl.TAL_D == "" ? null : PRI_Sl.TAL_D.Remove(10);
                    // Проверка
                    if (PRI_Sl.TAL_D == "")
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "56";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Ошибка Даты талона ВМП";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                    }
                }
            }

            PRI_Sl.SUMV = Math.Round(PRI_Sl.SUMV, 2);
            PRI_StrahReestr.Kod_Ksg = 0;   // !!!!!!!!!!!!!
            PRI_Sl.KSG = PRI_Sl.USL[0].Ksg;
            

            PRI_StrahReestr.SUM_LPU = (decimal) PRI_Sl.SUMV;
        }

        /// <summary>МЕТОД 5. Расчет Поликлиники</summary>
        private void MET_CalcPol()
        {  
            // Связываем ReStrahReestr и ReApac
            DataRow _RowApac = PRI_RowReestr.GetChildRows("ReReestr_Apac")[0];
            
            // Версия редакции 3.1 (ноябрь 2018)
            // Имя поля в таблице StrahReestr (Описание)
            
            // 3. PLAT (Код страховой компании) (28. SMO)                               далее в  MET_CalcAll() 
            PRI_StrahReestr.PLAT = m.MET_PoleStr("Scom", _RowApac);
 
            // 7. LPU_1 (Подразделения МО) (44. LPU_1) (39)
            int _Podrazd = m.MET_PoleInt("Podrazd", _RowApac);
            PRI_StrahReestr.LPU_1 = _Podrazd == 3 ? 55550900 : 55550901;                // главный/филиал         

            // 8. ORDER (Направление на госпитализацию, 1 - плановая, 2 - экстренная (у нас нету), 0 - поликлиника) (42. EXTR) (37)
            PRI_StrahReestr.ORDER = 0;

            // 9. LPU_ST (Условия оказания мед. помощи, справочник V006, 1 - кр. стационра, 2 - дн. стационар, 3 - поликлиника) (35 USL_OK) (34)
            PRI_StrahReestr.LPU_ST = 3;

            // 10. VIDPOM (Вид помощи, справочник V008, у нас 13 - поликл, 31 - стационар) (36. VIDPOM) (35)
            PRI_StrahReestr.VIDPOM = m.MET_PoleInt("VIDPOM", _RowApac);            // основной 13, но для всякой экзотики там другой номер

            // 12. PROFIL (Профиль, справочник V002) (46, 80 PROFIL) (41, 73)
            PRI_StrahReestr.PROFIL = m.MET_PoleDec("PROFIL", _RowApac);
            // Проверка
            PRI_ErrorToExcel.MET_VeryfError(PRI_StrahReestr.PROFIL == 0,
                        "12", "(вну) Не найден профиль",
                        ref PRI_ErrorRow);
            
            // 11. PODR (Код отделения) (45, 79. PODR) (40, 72)
            string _Prof = ((int)PRI_StrahReestr.PROFIL).ToString("D3");
            PRI_StrahReestr.PODR = m.MET_ParseDec($"3{_Prof}2{_Prof}");

            // 13. DET (Детский профиль, если ребёнок то 1 иначе 0) (47, 82 DET) (42, 74)   далее в  MET_CalcAll() 
            PRI_Age = m.MET_PoleInt("Age", _RowApac);   

            // 15. PRVS (Специальность врача, справочник V004)  (60, 90. PRVS) (53, 82)
            PRI_StrahReestr.PRVS = m.MET_PoleStr("PRVS", _RowApac);

            // 16. IDDOKT (Код врача, справочник врачей) (61 IDDOKT, 91. CODE_MD) (54, 83)
            PRI_StrahReestr.IDDOKT = m.MET_PoleStr("IDDOKT", _RowApac);

            // 17. ARR_DATE (Дата начала) (49. DATE_1, 83. DATE_IN) (44, 75)
            PRI_StrahReestr.ARR_DATE = m.MET_PoleDate("DN", _RowApac);

            // 18. EX_DATE (Дата окончания) (50. DATE_2, 84. DATE_OUT) (45, 76)
            PRI_StrahReestr.EX_DATE = m.MET_PoleDate("DK", _RowApac);

            // 19. DS1 (Диагноз) (52. DS1, 85. DS) (47, 77)                                             
            PRI_StrahReestr.DS1 = m.MET_PoleStr("D", PRI_RowReestr);
            // Проверка
            if (PRI_StrahReestr.DS1.Length < 3)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "42";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Что то не так с диагнозом";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // 20. DS2 (Сопутствующий Диагноз - не заполняем) (53. DS2) (48)  
            PRI_StrahReestr.DS2 = "";

            // 21. PACIENTID (Номер истории болезни/талона) (48. NHISTORY) (43)        
            PRI_StrahReestr.PACIENTID = m.MET_PoleStr("Cod", PRI_RowReestr);

            // 22. RES_G (Результат обращения/госпитализации, справочник V009), (57. RSLT) (51)
            int[] _RES_G = { 0, 301, 301, 304, 305, 306, 307, 308, 309, 311, 311, 306 };
            int _Rezobr = m.MET_PoleInt("REZOBR", _RowApac);
            // Если месяц сдачи совпадает с месяцем посещения, то смотрим какой результат проставили врачи
            if (PRI_StrahReestr.EX_DATE.Value.Month == PRI_StrahFile.MONTH)
                PRI_StrahReestr.RES_G = _RES_G[_Rezobr];
            else  // если месяц не совпадает, то ставим результат обращения 302 - прерван по инициативе пациента (ну типо больше не пришел)
                PRI_StrahReestr.RES_G = 302;

            //// Для посещений прошлого месяца, ставим результат обращения (Лечение прервано по инициативе пациента)
            //if (PRI_StrahReestr.EX_DATE.Value.Month < PROP_DateN.Month)
            //    PRI_StrahReestr.RES_G = 302;

            // 23. ISHOD (Исход заболевания, справочник V012) (59. ISHOD) (52)
            int[] _ISHOD = { 0, 301, 303, 302, 304, 304, 304, 304, 304, 304, 304, 304 };
            PRI_StrahReestr.ISHOD = _ISHOD[_Rezobr];

            // 25. KOL_USL (Койко дней) (87. KOL_USL) (79)
            PRI_StrahReestr.KOL_USL = m.MET_PoleInt("uet3", _RowApac); 

            // 24. IDSP (Код способа оплаты, справочник V010: 29 - разовые посещения, 30 по заболеванию) (63. IDSP) (56)
            PRI_StrahReestr.IDSP = PRI_StrahReestr.KOL_USL == 1 ? 29 : 30;

            // 26. TARIF (Тариф) (88. TARIF) (80)
            PRI_StrahReestr.TARIF = m.MET_PoleDec("Tarif", _RowApac);
            // Проверка
            if (PRI_StrahReestr.TARIF == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "26";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тариф";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
            }
                    
            // 28. SUM_LPU  (Сумма услуги) (89. SUMV_USL) (81)
            PRI_StrahReestr.SUM_LPU = PRI_StrahReestr.TARIF;

            // 29. VPOLIS (Тип полиса 1 - старый, 2 - временный, 3 - новый, 4 - электронный, 5 -УЭК) (25. VPOLIS)   далее в  MET_CalcAll() 
            PRI_NPolis = m.MET_PoleStr("SN", _RowApac);

            // 30. SERIA (Серия полиса) (26. SPOLIS)
            PRI_StrahReestr.SERIA = m.MET_PoleStr("SS", _RowApac).ToUpper();

            // 56. DayN (Длительность лечения из Тарифов)
            PRI_StrahReestr.DayN = 1;

            // Характер заболевания 1 - острое, 2 храническое впервые, 3 - хроническое повторно
            // Только для поликлиники (не Z) или для стационара (с диагнозом С..D09)
            if (PRI_StrahReestr.DS1.Substring(0, 1) != "Z")
                PRI_Sl.C_ZAB = m.MET_PoleInt("C_Zab", _RowApac);

            // Цель посещения (дальше будет уточнение)
            PRI_Sl.P_Cel = m.MET_PoleStr("P_Cel", _RowApac);
            // Проверка
            if ((PRI_StrahReestr.IDSP == 30 && PRI_Sl.P_Cel != "3.0" ) || (PRI_StrahReestr.IDSP != 30 && PRI_Sl.P_Cel == "3.0"))
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "35";
                PRI_ErrorToExcel.PROP_ErrorName = $"(вну) Не совпадает IDSP {PRI_StrahReestr.IDSP} с целью посещения {PRI_Sl.P_Cel}";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
            }

            PRI_CouUsl = 0;

            // Связываем ReApac и RePol
            foreach (DataRow _PolRow in _RowApac.GetChildRows("ReApac_Pol"))
            {
                PRI_CouUsl++;

                var _Usluga = new MyUSL
                {
                //    Nom = m.MET_PoleInt("Num", _PolRow),
                    DatN = m.MET_PoleStr("DP", _PolRow),
                    D = m.MET_PoleStr("D", _PolRow),
                    PRVS_Usl = m.MET_PoleStr("PRVS_Usl", _PolRow),
                    MD = m.MET_PoleStr("CODE_MD", _PolRow),
                    Code_Usl = "AMB.1.99", // m.MET_PoleStr("CODE_USL", _PolRow),
                    Usl = m.MET_PoleStr("VID_VME", _PolRow)
                };
                _Usluga.DatN = _Usluga.DatN == "" ? null : _Usluga.DatN.Remove(10);
                PRI_Sl.USL.Add(_Usluga);

                // Берем из первого случая
                if (PRI_CouUsl == 1) 
                {
                    // ЛПУ направления
                    PRI_Sl.NPR_MO = m.MET_PoleInt("NPR_MO", _PolRow);

                    // Дата направления
                    PRI_Sl.NPR_DATE = m.MET_PoleStr("NPR_DATE", _PolRow);
                    PRI_Sl.NPR_DATE = PRI_Sl.NPR_DATE == "" ? null : PRI_Sl.NPR_DATE.Remove(10);
                }

                // Направления на исследования, только для ЗНО
                if ((PRI_StrahReestr.DS1.Substring(0, 1) == "C" || PRI_StrahReestr.DS1.Substring(0, 2) == "D0" || PRI_StrahReestr.DS1.Substring(0, 3) == "D70")
                   && PRI_StrahReestr.EX_DATE >= new DateTime(2019, 12, 1))
                {
                    string _jTag = m.MET_PoleStr("jTag", _PolRow);

                    // Проверка на наличия данных KoblInfo
                    if (string.IsNullOrEmpty(_jTag))
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "28";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найдена строка APAC в KoblInfo";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        continue;
                    }

                    JObject _Json;
                    try
                    {
                        _Json = JObject.Parse(_jTag);
                    }
                    catch
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "30";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в KoblInfo";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }
                                     
                    IList<JToken> _Results = _Json["NAPR"]?.Children().ToList();
                    if (_Results != null)
                    { 
                        PRI_Sl.NAPR = new List<MyNAPR>();
                        foreach (JToken _result in _Results)
                        {
                            MyNAPR _Napr = _result.ToObject<MyNAPR>();
                            _Napr.NAPR_V = 3;
                            _Napr.NAPR_DATE = PRI_StrahReestr.ARR_DATE.Value.ToString("dd.MM.yyyy");  // !!! Пока берем не из направления, а начальную дату случая
                            PRI_Sl.NAPR.Add(_Napr);                        
                        }                   
                    }
                }

                // Новое 285 приказ (берем, только из последнего посещения и ЗНО)
                if (PRI_CouUsl == PRI_StrahReestr.KOL_USL && (PRI_StrahReestr.DS1.Substring(0, 1) == "C" || PRI_StrahReestr.DS1.Substring(0, 2) == "D0" || PRI_StrahReestr.DS1.Substring(0, 3) == "D70") 
                    && PRI_StrahReestr.EX_DATE >= new DateTime(2018, 9, 1))
                {
                    string _jTag = m.MET_PoleStr("jTag", _PolRow);

                    // Проверка на наличия данных KoblInfo
                    if (string.IsNullOrEmpty(_jTag))
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "28";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найдена строка APAC в KoblInfo";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        continue;
                    }

                    JObject _Json;
                    try
                    {
                        _Json = JObject.Parse(_jTag);
                    }
                    catch
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "30";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в KoblInfo";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }

                    // Цель посещения 3.0 - обращение, 1.0 - посещение разовое, 1.3 - диспансерное наблюдение (в базисе 4.0)
                    if (PRI_Sl.P_Cel == "1.0")
                    {
                        try
                        {
                            double _Cel = (double) _Json["Cel"];
                            // Цель посещения - диспансерное наблюдение - Состоит
                            if (_Cel == 4.0)
                            {
                                PRI_Sl.P_Cel = "1.3";
                                PRI_Sl.DN = 1;
                            }
                        }
                        catch
                        {
                            PRI_ErrorToExcel.PROP_ErrorCod = "41";
                            PRI_ErrorToExcel.PROP_ErrorName = "(вну) Тег Cel (Цель посещения) в KoblInfo имеет некоректное значение";
                            PRI_ErrorToExcel.MET_SaveError();
                            PRI_ErrorRow = true;
                            return;
                        }
                    }
                   
                    // Клиническая группа
                    string _klin_gr = (string)_Json["Klin_gr"];
                    // Проверка на наличия Клинической группы в KoblInfo
                    if (string.IsNullOrEmpty(_klin_gr))
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "29";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тег Klin_gr (Клиническая группа) в KoblInfo";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }

                    // DS_ONK - Если подозрение на ЗНО
                    if (_klin_gr == "Ia" || _klin_gr == "Ib")
                        PRI_Sl.DS_ONK = 1;
                    else
                        PRI_Sl.DS_ONK = 0;

                    // ONK_UL - Блок лечения ЗНО
                    if (PRI_Sl.DS_ONK == 0)
                    {
                        // Диагноз
                        string _D5 = PRI_StrahReestr.DS1;
                        string _D3 = _D5.Substring(0, 3);

                        PRI_Sl.ONK_SL = new MyONK_SL();

                        // DS1_T - Повод обращения (N018)
                        // 0 - первичное лечение  
                        // 1 - рецедив с метастазами, 2 - прогресирование с метастазами, 21, 22 - то-же но без метастаз)
                        // 3 - динамическое наблюдение
                        // 4 - диспансерное наблюдение
                        // 5 - диагностика
                        // 6 - симптоматическое лечение
                        PRI_Sl.ONK_SL.DS1_T = (int?)_Json["DS1_T"] ?? 0;
                        if (PRI_Sl.ONK_SL.DS1_T == 1 || PRI_Sl.ONK_SL.DS1_T == 2)
                        {
                            // MTSTZ - Отдаленные метастазы
                            PRI_Sl.ONK_SL.MTSTZ = 1;
                        }
                        if (PRI_Sl.ONK_SL.DS1_T == 21 || PRI_Sl.ONK_SL.DS1_T == 22)
                        {
                            PRI_Sl.ONK_SL.DS1_T = PRI_Sl.ONK_SL.DS1_T - 20;
                        }
                        if (PRI_Sl.P_Cel == "1.3") PRI_Sl.ONK_SL.DS1_T = 4;

                        // STAD - Стадия (только для DS1_T < 5)
                        if (PRI_Sl.ONK_SL.DS1_T < 5)
                        {
                            string _stadia = (string) _Json["Stadia"];
                            var _N002 = PRI_N002.Where(e => e.Kod1 == _D5).ToList();
                                // Сначала пытаемся найти по полному диагнозу
                            if (!_N002.Any())
                                _N002 = PRI_N002.Where(e => e.Kod1 == _D3).ToList();
                                    // Иначе ищем по 3м сиволам диагноза
                            if (!_N002.Any())
                                _N002 = PRI_N002.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                            int? _stadiaNumber = _N002.FirstOrDefault(e => e.Kod3.Contains($"\"{_stadia}\""))?.Number ??
                                                 _N002.First()?.Number;
                            PRI_Sl.ONK_SL.STAD = _stadiaNumber ?? 145;
                        }

                        // 13. DET (Детский профиль, если ребёнок то 1 иначе 0) (47, 82 DET) (42, 74)
                        PRI_StrahReestr.DET = PRI_Age < 18 ? 1 : 0;

                        // TNM (заполняем только для взрослых и первичном лечении)
                        if (PRI_StrahReestr.DET == 0 && PRI_Sl.ONK_SL.DS1_T == 0)
                        {
                            // ONK_T - T
                            string _T = (string) _Json["T"];
                            var _N003 = PRI_N003.Where(e => e.Kod1 == _D5).ToList();
                                // Сначала пытаемся найти по полному диагнозу
                            if (!_N003.Any())
                                _N003 = PRI_N003.Where(e => e.Kod1 == _D3).ToList();
                                    // Иначе ищем по 3м сиволам диагноза
                            if (!_N003.Any())
                                _N003 = PRI_N003.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                            int? _TNumber = _N003.FirstOrDefault(e => e.Kod3.Contains($"\"{_T}\""))?.Number ??
                                            _N003.First()?.Number;
                            PRI_Sl.ONK_SL.ONK_T = _TNumber ?? 182;

                            // ONK_N - N
                            string _N = (string) _Json["N"];
                            var _N004 = PRI_N004.Where(e => e.Kod1 == _D5).ToList();
                                // Сначала пытаемся найти по полному диагнозу
                            if (!_N004.Any())
                                _N004 = PRI_N004.Where(e => e.Kod1 == _D3).ToList();
                                    // Иначе ищем по 3м сиволам диагноза
                            if (!_N004.Any())
                                _N004 = PRI_N004.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                            int? _NNumber = _N004.FirstOrDefault(e => e.Kod3.Contains($"\"{_N}\""))?.Number ??
                                            _N004.First()?.Number;
                            PRI_Sl.ONK_SL.ONK_N = _NNumber ?? 99;

                            // ONK_M - M
                            string _M = (string) _Json["M"];
                            var _N005 = PRI_N005.Where(e => e.Kod1 == _D5).ToList();
                                // Сначала пытаемся найти по полному диагнозу
                            if (!_N005.Any())
                                _N005 = PRI_N005.Where(e => e.Kod1 == _D3).ToList();
                                    // Иначе ищем по 3м сиволам диагноза
                            if (!_N005.Any())
                                _N005 = PRI_N005.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                            int? _MNumber = _N005.FirstOrDefault(e => e.Kod3.Contains($"\"{_M}\""))?.Number ??
                                            _N005.First()?.Number;
                            PRI_Sl.ONK_SL.ONK_M = _MNumber ?? 56;
                        }

                        // CONS Блок о проведении консилиума
                        // Связываем ReApac и RePolCons
                        foreach (DataRow _PolRowCons in _RowApac.GetChildRows("ReApac_PolCons"))
                        {
                            switch (m.MET_PoleInt("PR_CONS", _PolRowCons))
                            {
                                case 1:
                                    PRI_Sl.Taktika_1 = m.MET_PoleStr("DT_CONS", _PolRowCons);
                                    break;
                                case 2:
                                    PRI_Sl.Taktika_2 = m.MET_PoleStr("DT_CONS", _PolRowCons);
                                    break;
                                case 3:
                                    PRI_Sl.Taktika_3 = m.MET_PoleStr("DT_CONS", _PolRowCons);
                                    break;
                            }                         
                        }

                        // B_PROT Блок об имеющихся противопоказаниях и отказах
                        PRI_Sl.PrOt_1 = MET_VerifDate((string)_Json["PrOt_1"], "PrOt_1", true);
                        if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                        PRI_Sl.PrOt_2 = MET_VerifDate((string)_Json["PrOt_2"], "PrOt_2", true);
                        if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                        PRI_Sl.PrOt_3 = MET_VerifDate((string)_Json["PrOt_3"], "PrOt_3", true);
                        if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                        PRI_Sl.PrOt_4 = MET_VerifDate((string)_Json["PrOt_4"], "PrOt_4", true);
                        if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                        PRI_Sl.PrOt_5 = MET_VerifDate((string)_Json["PrOt_5"], "PrOt_5", true);
                        if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                        PRI_Sl.PrOt_6 = MET_VerifDate((string)_Json["PrOt_6"], "PrOt_6", true);
                        if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    }
                }
            }

            PRI_StrahReestr.CODE_USL = PRI_Sl.USL[PRI_CouUsl - 1].Code_Usl;
            PRI_StrahReestr.VID_VME = PRI_Sl.USL[PRI_CouUsl - 1].Usl;
            
            MET_CalcAll();

            try
            {
                PRI_StrahReestr.NOM_USL = JsonConvert.SerializeObject(PRI_Sl,
                                Formatting.Indented,
                                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            }
            catch (Exception)
            {

                PRI_StrahReestr.NOM_USL = "Ошибка json";
            }
        }

        /// <summary>МЕТОД 6. Расчет Параклиники/Гистологии</summary>
        private void MET_CalcPar()
        {
            // Проверка на связь двух таблиц
            try
            {
                // Связываем ReStrahReestr и RePar
                DataRow _RowParErr = PRI_RowReestr.GetChildRows("ReReestr_Par")[0];
            }
            catch
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "44";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не могу соединить исследование друг с другом";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // Связываем ReStrahReestr и RePar
            DataRow _RowPar = PRI_RowReestr.GetChildRows("ReReestr_Par")[0];

            // 3. PLAT (Код страховой компании) (28. SMO)                               далее в  MET_CalcAll() 
            PRI_StrahReestr.PLAT = m.MET_PoleStr("Scom", _RowPar);

            // 7. LPU_1 (Подразделения МО) (44. LPU_1) (39)
            PRI_StrahReestr.LPU_1 = 55550900;                               // по умолчанию главный

            // 8. ORDER (Направление на госпитализацию, 1 - плановая, 2 - экстренная (у нас нету), 0 - поликлиника) (42. EXTR) (37)
            PRI_StrahReestr.ORDER = 0;

            // 9. LPU_ST (Условия оказания мед. помощи, справочник V006, 1 - кр. стационра, 2 - дн. стационар, 3 - поликлиника, для параклиники пусть будет 4 и 5, при выгрузке в XML поменяем на 3) (35 USL_OK) (34)
            PRI_StrahReestr.LPU_ST = (int)PRI_TipUsl;

            // 10. VIDPOM (Вид помощи, справочник V008, у нас 13 - поликл/параклиники, 31 - стационар) (36. VIDPOM) (35)
            PRI_StrahReestr.VIDPOM = m.MET_PoleInt("VIDPOM", _RowPar);            // основной 13, но для всякой экзотики там другой номер

            // 12. PROFIL (Профиль, справочник V002) (46, 80 PROFIL) (41, 73)
            PRI_StrahReestr.PROFIL = m.MET_PoleDec("PROFIL", _RowPar);
            // Проверка
            PRI_ErrorToExcel.MET_VeryfError(PRI_StrahReestr.PROFIL == 0,
                        "12", "(вну) Не найден профиль",
                        ref PRI_ErrorRow);

            // 11. PODR (Код отделения) (45, 79. PODR) (40, 72)
            string _Prof = ((int)PRI_StrahReestr.PROFIL).ToString("D3");
            PRI_StrahReestr.PODR = m.MET_ParseDec($"3{_Prof}1{_Prof}");

            // 13. DET (Детский профиль, если ребёнок то 1 иначе 0) (47, 82 DET) (42, 74)   далее в  MET_CalcAll() 
            PRI_Age = m.MET_PoleInt("Age", _RowPar);

            // 15. PRVS (Специальность врача, справочник V004)  (60, 90. PRVS) (53, 82)
            PRI_StrahReestr.PRVS = m.MET_PoleStr("PRVS", _RowPar);

            // 16. IDDOKT (Код врача, справочник врачей) (61 IDDOKT, 91. CODE_MD) (54, 83)
            PRI_StrahReestr.IDDOKT = m.MET_PoleStr("IDDOKT", _RowPar);

            // 17. ARR_DATE (Дата начала) (49. DATE_1, 83. DATE_IN) (44, 75)
            PRI_StrahReestr.ARR_DATE = m.MET_PoleDate("DP", PRI_RowReestr);

            // 18. EX_DATE (Дата окончания) (50. DATE_2, 84. DATE_OUT) (45, 76)
            PRI_StrahReestr.EX_DATE = PRI_StrahReestr.ARR_DATE;

            // 19. DS1 (Диагноз) (52. DS1, 85. DS) (47, 77)                                             
            PRI_StrahReestr.DS1 = m.MET_PoleStr("D", PRI_RowReestr);
            // Проверка
            if (PRI_StrahReestr.DS1.Length < 3)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "42";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Что то не так с диагнозом";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // 20. DS2 (Сопутствующий Диагноз - не заполняем) (53. DS2) (48)  
            PRI_StrahReestr.DS2 = "";

            // 21. PACIENTID (Номер истории Cod из kbolInfo) (48. NHISTORY) (43)        
            PRI_StrahReestr.PACIENTID = m.MET_PoleStr("Cod", PRI_RowReestr);

            // 22. RES_G (Результат обращения/госпитализации, справочник V009), (57. RSLT) (51)
            PRI_StrahReestr.RES_G = 301;

            // 23. ISHOD (Исход заболевания, справочник V012) (59. ISHOD) (52)
            PRI_StrahReestr.ISHOD = 304;

            // 25. KOL_USL (Койко дней) (87. KOL_USL) (79)
            PRI_StrahReestr.KOL_USL = 1;

            // 24. IDSP (Код способа оплаты, справочник V010: 28 - За медицинскую услугу) (63. IDSP) (56)
            PRI_StrahReestr.IDSP = 28;

            // 26. TARIF (Тариф) (88. TARIF) (80)
            PRI_StrahReestr.TARIF = m.MET_PoleDec("Tarif", _RowPar);
            // Проверка
            if (PRI_StrahReestr.TARIF == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "26";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тариф";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
            }

            // 28. SUM_LPU  (Сумма услуги) (89. SUMV_USL) (81)
            PRI_StrahReestr.SUM_LPU = PRI_StrahReestr.TARIF;

            // 29. VPOLIS (Тип полиса 1 - старый, 2 - временный, 3 - новый, 4 - электронный, 5 -УЭК) (25. VPOLIS)   далее в  MET_CalcAll() 
            PRI_NPolis = m.MET_PoleStr("SN", _RowPar);

            // 30. SERIA (Серия полиса) (26. SPOLIS)
            PRI_StrahReestr.SERIA = m.MET_PoleStr("SS", _RowPar).ToUpper();

            // 56. DayN (Длительность лечения из Тарифов)
            PRI_StrahReestr.DayN = 1;
            
            // Цель посещения
            PRI_Sl.P_Cel = "1.0";

            PRI_CouUsl = 1;

            // Описываем услугу
            MyUSL _Usluga = new MyUSL();
            _Usluga.DatN = PRI_StrahReestr.ARR_DATE.ToString().Remove(10);
            _Usluga.D = PRI_StrahReestr.DS1;
            _Usluga.PRVS_Usl = PRI_StrahReestr.PRVS;
            _Usluga.MD = PRI_StrahReestr.IDDOKT;
            _Usluga.Code_Usl = m.MET_PoleStr("CODE_USL", _RowPar);
            _Usluga.Usl = m.MET_PoleStr("VID_VME", _RowPar);
            PRI_Sl.USL.Add(_Usluga);

            // ЛПУ направления
            PRI_Sl.NPR_MO = m.MET_PoleInt("NPR_MO", _RowPar);

            // Дата направления
            PRI_Sl.NPR_DATE = _Usluga.DatN;

            // DS_ONK - Подозрение на ЗНО
            PRI_Sl.DS_ONK = 1;

            // Если ЗНО
            if (PRI_StrahReestr.DS1.Substring(0, 1) == "C" || PRI_StrahReestr.DS1.Substring(0, 2) == "D0" || PRI_StrahReestr.DS1.Substring(0, 3) == "D70")
            {
                // Характер заболевания 1 - острое, 2 храническое впервые, 3 - хроническое повторно
                PRI_Sl.C_ZAB = 2;

                string _jTag = m.MET_PoleStr("jTag", _RowPar);

                // Проверка на наличия данных KoblInfo
                if (string.IsNullOrEmpty(_jTag))
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "28";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найдена строка parObsledov в KoblInfo";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }

                JObject _Json;
                try
                {
                    _Json = JObject.Parse(_jTag);
                }
                catch
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "30";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в KoblInfo";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }

                // Клиническая группа
                string _klin_gr = (string)_Json["Klin_gr"];
                // Проверка на наличия Клинической группы в KoblInfo, если нет, то подозрение
                if (string.IsNullOrEmpty(_klin_gr))
                    _klin_gr = "Ia";

                // DS_ONK - Если подозрение на ЗНО
                if (_klin_gr == "Ia" || _klin_gr == "Ib")
                    PRI_Sl.DS_ONK = 1;
                else
                    PRI_Sl.DS_ONK = 0;

                // ONK_UL - Блок лечения ЗНО
                if (PRI_Sl.DS_ONK == 0)
                {
                    PRI_Sl.ONK_SL = new MyONK_SL();

                    //DS1_T Повод обращения
                    PRI_Sl.ONK_SL.DS1_T = 5;
                }
            }


            PRI_StrahReestr.CODE_USL = _Usluga.Code_Usl;
            PRI_StrahReestr.VID_VME = _Usluga.Usl;

            MET_CalcAll();

            // Формируем Случай MySL в json
            try
            {
                PRI_StrahReestr.NOM_USL = JsonConvert.SerializeObject(PRI_Sl,
                                Formatting.Indented,
                                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            }
            catch (Exception)
            {

                PRI_StrahReestr.NOM_USL = "Ошибка json";
            }
        }

        /// <summary>МЕТОД 7. Расчет общих полей</summary>
        private void MET_CalcAll()
        {
            // Версия редакции 3 (март 2018)
            // Имя поля в таблице StrahReestr (Описание) 
            
            // 2. CodFile (Код файла реестра)
            PRI_StrahReestr.CodFile = PRI_StrahFile.Cod;

            // 3. PLAT (Код страховой компании) (28. SMO)   
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
                case "46":
                case "52":
                    PRI_StrahReestr.PLAT = "55046";
                    break;
            }
            PRI_FlagSMO = PRI_StrahReestr.PLAT != PRI_PLAT;
            // Если страховая компания поменялась
            if (PRI_FlagSMO)
            {
                PRI_PLAT = PRI_StrahReestr.PLAT;
                DataRow _RowStrah = MyGlo.DataSet.Tables["ReStrahComp"].Rows.Find(PRI_PLAT);// Находим строку страховой компании по коду
                PRI_SMO_OGRN = m.MET_PoleStr("OGRN", _RowStrah);                                // ОГРН
                PRI_SMO_OK = m.MET_PoleStr("OKATO", _RowStrah);                                 // ОКАТО
                PRI_SMO_NAM = m.MET_PoleStr("Names", _RowStrah);                                // Наименование страховой компании
            }

            // 4. SMO_OGRN  (ОГРН страховой компании) (29. SMO_OGRN)
            PRI_StrahReestr.SMO_OGRN = PRI_SMO_OGRN;

            // 5. SMO_OK (ОКАТО страховой компании) (30. SMO_OK)  
            PRI_StrahReestr.SMO_OK = PRI_SMO_OK;

            // 6. SMO_NAM (Наименование страховой компании) (31. SMO_NAM)    
            PRI_StrahReestr.SMO_NAM = PRI_SMO_NAM;

            // 9. LPU_ST (Условия оказания мед. помощи, справочник V006, 1 - кр. стационра, 2 - дн. стационар, 3 - поликлиника) (35 USL_OK) (34)
            PRI_StrahReestr.LPU_ST = m.MET_PoleInt("LPU_ST", PRI_RowReestr);

             // 13. DET (Детский профиль, если ребёнок то 1 иначе 0) (47, 82 DET) (42, 74)
            PRI_StrahReestr.DET = PRI_Age < 18 ? 1 : 0;

            // 29. VPOLIS (Тип полиса 1 - старый, 2 - временный, 3 - новый, 4 - электронный, 5 -УЭК) (25. VPOLIS)
            char _VPolis = PRI_NPolis[PRI_NPolis.Length - 1];
            switch (_VPolis)
            {
                case 'с':
                    PRI_StrahReestr.VPOLIS = 1;                                      // старый
                    break;
                case 'в':
                    PRI_StrahReestr.VPOLIS = 2;                                      // временный
                    break;
                case 'н':
                    PRI_StrahReestr.VPOLIS = 3;                                      // новый
                    break;
                case 'э':
                    PRI_StrahReestr.VPOLIS = 4;                                      // электронный
                    break;
                case 'у':
                    PRI_StrahReestr.VPOLIS = 5;                                      // в составе УЭК
                    break;
                default:
                    PRI_StrahReestr.VPOLIS = 1;
                    break;
            }
            if (char.IsLetter(_VPolis)) PRI_NPolis = PRI_NPolis.Remove(PRI_NPolis.Length - 1);

            // 31. NUMBER (Номер полиса) (27. NPOLIS)
            PRI_StrahReestr.NUMBER = PRI_NPolis.ToUpper();

            // 32. FAMILY (Фамилия пациента)  (100. FAM) (93)
            string _FAM = m.MET_PoleStr("FAM", PRI_RowReestr);
            PRI_StrahReestr.FAMILY = _FAM.Substring(0, _FAM.IndexOf(" ")).ToUpper();

            // 33. NAME (Имя пациента)  (101. IM) (94)
            PRI_StrahReestr.NAME = m.MET_PoleStr("I", PRI_RowReestr).ToUpper();

            // 34. FATHER (Отчество пациента) (102. OT) (95)
            string _O = m.MET_PoleStr("O", PRI_RowReestr).ToUpper();
            if (_O != "НЕТ")
                PRI_StrahReestr.FATHER = _O;


            // 35. POL (Пол пациента 1 - муж, 2 - женский)  (103. W) (96)
            PRI_StrahReestr.POL = m.MET_PoleInt("Pol", PRI_RowReestr);

            // 36. VOZRAST (Дата рождения пациента) (104. DR) (97)
            PRI_StrahReestr.VOZRAST = m.MET_PoleDate("DR", PRI_RowReestr);

            // 37. SS (СНИЛС, с разделителями) (116. SNILS)  (107)
            string _SS = m.MET_PoleStr("SNILS", PRI_RowReestr);
            PRI_StrahReestr.SS = _SS.Length == 14 ? _SS.ToUpper() : "";

            // 38. OS_SLUCH (Особый случай, 2 - нет Отчества) (62. OS_SLUCH) (55)
            PRI_StrahReestr.OS_SLUCH = PRI_StrahReestr.FATHER == null ? "2" : "";

            // 39. FAM_P (Фамилия представителя пациента) (106. FAM_P) (98)                  только для новорожденных, а у нас их нету
            PRI_StrahReestr.FAM_P = "";
            // 40. IM_P (Имя представителя пациента) (107 IM_P) (99)
            PRI_StrahReestr.IM_P = "";
            // 41. OT_P (Отчество представителя пациента) (108. OT_P) (100)
            PRI_StrahReestr.OT_P = "";
            // 42. W_P (Пол представителя пациента) (109. W_P) (101)
            PRI_StrahReestr.W_P = 0;
            // 43. DR_P (Дата рождения представителя пациента) (110. DR_P) (102) 
            PRI_StrahReestr.DR_P = null;

            // 44. MR (Место рождения) (112. MR) (103)
            string _MR = m.MET_PoleStr("MR", PRI_RowReestr);
            PRI_StrahReestr.MR = _MR.Length > 0 ? _MR.ToUpper() : "Г. ОМСК";

            // 45. DOCTYPE (Тип документа удостоверяющего личность) (113. DOCTYPE) (104)            
            bool _FlagDocumNo = PRI_StrahReestr.VPOLIS != 3;         // если полис новый, то документ можем не заполнять
            int _Doc = m.MET_PoleInt("Doc", PRI_RowReestr);
            if (_FlagDocumNo && _Doc == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "45";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не указан документ личности";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }
            PRI_StrahReestr.DOCTYPE = _Doc;

            if (_Doc > 0)
            {
                // 46. DOCSER (Серия документа удостоверяющего личность) (114. DOCSER) (105)
                string _Pasp_Ser = m.MET_PoleStr("Pasp_Ser", PRI_RowReestr);
                if (_FlagDocumNo && _Pasp_Ser == "")
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
                string _Pasp_Nom = m.MET_PoleStr("Pasp_Nom", PRI_RowReestr);
                if (_FlagDocumNo && _Pasp_Nom == "")
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "45";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не указан Номер документа личности";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }
                PRI_StrahReestr.DOCNUM = _Pasp_Nom;

                // DOCDATE (Дата выдачи документа удостоверяющего личность)
                DateTime? _Pasp_Kogda = m.MET_PoleDate("Pasp_Kogda", PRI_RowReestr);
                if (_FlagDocumNo && _Pasp_Kogda == null)
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "45";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не указан Дата выдачи документа личности";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }
                PRI_StrahReestr.DOCDATE = _Pasp_Kogda;

                // DOCORG (Кем выдан документ удостоверяющего личность)
                string _Pasp_Kem = m.MET_PoleStr("Pasp_Kem", PRI_RowReestr);
                if (_FlagDocumNo && _Pasp_Kem == "")
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "45";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не указан Кем выдан документ личности";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }
                PRI_StrahReestr.DOCORG = _Pasp_Kem;
            }

            // OKATOG (ОКАТО места жительства) (он же OKATOP)
            // Либо города, либо населенные пункты
            PRI_StrahReestr.OKATOG = m.MET_PoleStr("OCATD", PRI_RowReestr);
            // Для Омских страховых, ОКАТО из других областенй меняем на город Омск
            if (PRI_Strah != 4 && PRI_StrahReestr.OKATOG.Substring(0, 2) != "52")
                PRI_StrahReestr.OKATOG = "52401000000";

            // 49. NOM_ZAP (Номер случая) (34. IDCASE) (33)
            PRI_StrahReestr.NOM_ZAP = PRI_ErrorRow ? 0 : ++PRI_ReeNom_Zap;
            
            // 51. ID_PAC (Код пациента) (24. 99. ID_PAC) (24, 92)
            PRI_StrahReestr.ID_PAC = m.MET_PoleDec("KL", PRI_RowReestr);

            // 52. N_ZAP  (Номер пациента)  (20. N_ZAP) 
            PRI_StrahReestr.N_ZAP = PRI_ErrorRow ? 0 : ++PRI_ReeN_Zap;

            // 53. PR_NOV (21. Признак исправленной записи, 0 - первый раз, 1 - повторно) (21. PR_NOV) (21)
            if (PROP_Parent > 0)
                PRI_StrahReestr.PR_NOV = 1;
            else
                PRI_StrahReestr.PR_NOV = 0;
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
                   ,FAM_P,IM_P,OT_P,W_P
                   ,DR_P,MR,DOCTYPE,DOCSER
                   ,DOCNUM,OKATOG                    
                   ,NOM_ZAP,UKL,NOM_USL,ID_PAC
                   ,N_ZAP,PR_NOV,DayN
                   ,VID_VME,VID_HMP,METOD_HMP, Kod_Ksg
                   ,DOCDATE,DOCORG)                                                
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
                    ,'{PRI_StrahReestr.FAM_P}','{PRI_StrahReestr.IM_P}','{PRI_StrahReestr.OT_P}',{PRI_StrahReestr.W_P.ToString()}
                    ,{_DR_P},'{PRI_StrahReestr.MR}',{PRI_StrahReestr.DOCTYPE.ToString()},'{PRI_StrahReestr.DOCSER}'
                    ,'{PRI_StrahReestr.DOCNUM}','{PRI_StrahReestr.OKATOG}'
                    ,{PRI_StrahReestr.NOM_ZAP.ToString()},'1','{PRI_StrahReestr.NOM_USL}',{PRI_StrahReestr.ID_PAC.ToString()}
                    ,{PRI_StrahReestr.N_ZAP.ToString()},{PRI_StrahReestr.PR_NOV.ToString()},{PRI_StrahReestr.DayN.ToString()}
                    ,'{PRI_StrahReestr.VID_VME}','{PRI_StrahReestr.VID_HMP}','{PRI_StrahReestr.METOD_HMP}', {PRI_StrahReestr.Kod_Ksg ?? 0}
                    ,{_DOCDATE},'{PRI_StrahReestr.DOCORG}');";

           
            MySql.MET_QueryNo(_Str);
        }

        /// <summary>МЕТОД Проверка на формат даты и на диапазон госпитализаци/обращений</summary>
        /// <param name="pStrDate">Строка с датой</param>
        /// <param name="pNameTag">Имя тега</param>
        /// <param name="pFatalError">Флаг фатальной ошибки, в случае некоректной даты (по умолчанию false)</param>
        /// <param name="pIsStart">Проверяем на начло случая (по пумолчанию true)</param>
        /// <param name="pIsEnd">Проверяем на конец случая (по пумолчанию true)</param>
        /// <returns>В случае ошибки возвращаем null иначе туже строку</returns>
        private string MET_VerifDate(string pStrDate, string pNameTag, bool pFatalError = false, bool pIsStart = true, bool pIsEnd = true)
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

            return pStrDate;
        }      
    }
}

