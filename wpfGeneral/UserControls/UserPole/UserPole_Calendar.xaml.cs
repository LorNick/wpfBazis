using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для поля Даты (Data)</summary>
    public partial class UserPole_Calendar
    {
        #region ---- СВОЙСТВО ----
        /// <value>СВОЙСТВО Изменялся ли текст</value>
        public bool PROP_BoolChangeText { get; set; }

        /// <value>СВОЙСТВО Описание вопроса</value>
        public override string PROP_Description
        {
            get { return (string)PART_Label.Content; }
            set
            {
                PART_Label.Content = value;
                // Если описания нету, то убираем пустой отступ
                if (value == "")
                    PART_Label.Padding = new Thickness(0);
                else
                    PART_Label.Padding = new Thickness(5, 0, 5, 0);
            }
        }
       
        /// <summary>СВОЙСТВО Максимальное значение</summary>
        public override object PROP_ValueMax
        {
            get { return PART_Calendar.DisplayDateEnd; }
            set { PART_Calendar.DisplayDateEnd = (DateTime)value; }
        }

        /// <summary>СВОЙСТВО Связанное Максимальное значение</summary>
        public DateTime? PROP_ValueMaxDate
        {
            get { return (DateTime)this.GetValue(DEPR_ValueMaxDateProperty); }
            set { this.SetValue(DEPR_ValueMaxDateProperty, value); }
        }

        /// <summary>СВОЙСТВО Минимальное значение</summary>
        public override object PROP_ValueMin
        {
            get { return PART_Calendar.DisplayDateStart; }
            set { PART_Calendar.DisplayDateStart = (DateTime)value; }
        }

        /// <summary>СВОЙСТВО Связанное Минимальное значение</summary>
        public DateTime? PROP_ValueMinDate
        {
            get { return (DateTime)this.GetValue(DEPR_ValueMinDateProperty); }
            set { this.SetValue(DEPR_ValueMinDateProperty, value); }
        }

        /// <summary>СВОЙСТВО Цвет текста ответа</summary>
        public override Brush PROP_ForegroundText
        {
            get { return PART_TextDate.Foreground; }
            set { PART_TextDate.Foreground = value; }
        }

        /// <summary>СВОЙСТВО Помечаем поле текста</summary>
        public override byte PROP_PometkaText
        {
            get { return PRO_PometkaText; }
            set
            {
                PRO_PometkaText = value;
                if (PRO_PometkaText == 1)
                    PART_TextDate.BorderBrush = Brushes.Red;
                else
                    PART_TextDate.ClearValue(Border.BorderBrushProperty);
            }
        }

        /// <summary>СВОЙСТВО Минимальная Ширина описания</summary>
        public override double PROP_MinWidthDescription
        {
            get { return PART_Label.MinWidth; }
            set { PART_Label.MinWidth = value; }
        }
        #endregion

        #region ---- РЕГИСТРАЦИЯ ----
        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_ValueMaxDate</summary>
        public static readonly DependencyProperty DEPR_ValueMaxDateProperty =
            DependencyProperty.Register("PROP_ValueMaxDate", typeof(DateTime?), typeof(UserPole_Calendar), new UIPropertyMetadata(DateTime.MaxValue));


        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_ValueMinDate</summary>
        public static readonly DependencyProperty DEPR_ValueMinDateProperty =
            DependencyProperty.Register("PROP_ValueMinDate", typeof(DateTime?), typeof(UserPole_Calendar), new UIPropertyMetadata(DateTime.MinValue));
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_Calendar()
        {
            InitializeComponent();

            PART_TextDate.ContextMenu = MyGlo.ContextMenu;
            PART_TextDate.Tag = this;
        }

        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            // Располагаем текст
			this.HorizontalAlignment = HorizontalAlignment.Left;                
            if (PROP_Format.MET_If("ac"))
                this.HorizontalAlignment = HorizontalAlignment.Center;
            if (PROP_Format.MET_If("ar"))
                this.HorizontalAlignment = HorizontalAlignment.Right;

            // Если указанно в формате, что показываем 1, а не 2 месяца
            if (PROP_Format.MET_If("m1"))
            {
                PART_Calendar.Columns = 1;
                PART_Calendar.Width = 250;
            }

            // Помечаем в календаре указанные даты
            foreach (var _Dat in PROP_Text.Split(';'))
            {
                if (DateTime.TryParse(_Dat, out DateTime _DateTime))
                    PART_Calendar.SelectedDates.Add(_DateTime);
            }            
        }
        
        /// <summary>СОБЫТИЕ Нажали на кнопку - скрыть/отобразить календарь</summary>
        private void PART_ButtonVisiblCalendar_Click(object sender, RoutedEventArgs e)
        {
            if (PART_Calendar.Visibility == Visibility.Collapsed)
                PART_Calendar.Visibility = Visibility.Visible;
            else
                PART_Calendar.Visibility = Visibility.Collapsed;
        }

        /// <summary>СОБЫТИЕ Выбор дат в календаре</summary>
        private void PART_Calendar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PROP_Text = "";

            foreach (var _Date in PART_Calendar.SelectedDates.OrderBy(p => p.Date))
            {
                PROP_Text += _Date.Date.ToShortDateString() + "; ";
            }

            PROP_Text += PART_Calendar.SelectedDates.Count > 0
                ? $"(дней {PART_Calendar.SelectedDates.Count})"
                : "";

            if (Equals(PROP_ForegroundText, Brushes.Gray))
                PROP_ForegroundText = Brushes.Black;

            // Если есть шаблон
            if (this.PROP_FormShablon?.PROP_Created ?? false)
            {
                // Активируем кнопку "Сохранить"
                MyGlo.callbackEvent_sEditShablon?.Invoke(true);
                // Запускаем Lua фунцкию, на изменение записи
                this.PROP_Lua?.MET_OnChange();
            }
        }
    }
}
