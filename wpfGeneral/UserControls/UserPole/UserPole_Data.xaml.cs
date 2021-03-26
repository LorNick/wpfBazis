using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для поля Даты (Data)</summary>
    public partial class UserPole_Data : VirtualPole
    {
        #region ---- СВОЙСТВО ----
        /// <summary>СВОЙСТВО Изменялся ли текст</summary>
        public bool PROP_BoolChangeText { get; set; }

        /// <summary>СВОЙСТВО Связанная дата</summary>
        public virtual DateTime? PROP_Date
        {
            get { return (DateTime?)this.GetValue(DEPR_DateProperty); }
            set { this.SetValue(DEPR_DateProperty, value); }
        }

        /// <summary>СВОЙСТВО Описание вопроса</summary>
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
            get { return PART_DatePicker.DisplayDateEnd; }
            set { PART_DatePicker.DisplayDateEnd = (DateTime)value; }
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
            get { return PART_DatePicker.DisplayDateStart; }
            set { PART_DatePicker.DisplayDateStart = (DateTime)value; }
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
            get { return PART_DatePicker.Foreground; }
            set { PART_DatePicker.Foreground = value; }
        }

        /// <summary>СВОЙСТВО Помечаем поле текста</summary>
        public override byte PROP_PometkaText
        {
            get { return PRO_PometkaText; }
            set
            {
                PRO_PometkaText = value;
                if (PRO_PometkaText == 1)
                    PART_DatePicker.BorderBrush = Brushes.Red;
                else
                    PART_DatePicker.ClearValue(Border.BorderBrushProperty);
            }
        }

        /// <summary>СВОЙСТВО Минимальная Ширина описания</summary>
        public override double PROP_MinWidthDescription
        {
            get { return PART_Label.MinWidth; }
            set { PART_Label.MinWidth = value; }
        }

        /// <summary>СВОЙСТВО Скрываем рамку</summary>
        public override bool PROP_HideBorder
        {
            get
            {
                return PART_DatePicker.BorderThickness.Equals(new Thickness(0));
            }
            set
            {
                if (value)
                    PART_DatePicker.BorderThickness = new Thickness(0);
            }
        }

        /// <summary>СВОЙСТВО Высота текстового поля</summary>
        public override double PROP_HeightText
        {
            get { return PART_DatePicker.Height; }
            set
            {
                PART_DatePicker.Height = value;
            }
        }
        #endregion

        #region ---- РЕГИСТРАЦИЯ ----
        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_Date</summary>
        public static readonly DependencyProperty DEPR_DateProperty =
            DependencyProperty.Register("PROP_Date", typeof(DateTime?), typeof(UserPole_Data), null);

        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_ValueMaxDate</summary>
        public static readonly DependencyProperty DEPR_ValueMaxDateProperty =
            DependencyProperty.Register("PROP_ValueMaxDate", typeof(DateTime?), typeof(UserPole_Data), new UIPropertyMetadata(DateTime.MaxValue));


        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_ValueMinDate</summary>
        public static readonly DependencyProperty DEPR_ValueMinDateProperty =
            DependencyProperty.Register("PROP_ValueMinDate", typeof(DateTime?), typeof(UserPole_Data), new UIPropertyMetadata(DateTime.MinValue));
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_Data()
        {
            InitializeComponent();

            PART_DatePicker.ContextMenu = MyGlo.ContextMenu;
            PART_DatePicker.Tag = this;
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
        }

        /// <summary>СОБЫТИЕ При изменении даты</summary>
        private void PART_DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
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
            MET_Changed?.Invoke(); // Если нужно выловить событие изменение поля
        }

        ///<summary>МЕТОД Проверка на допустимость данных и полноте заполнения</summary>
        public override bool MET_Verification()
        {
            // Проверяем на заполнение поля
            if (PROP_Necessarily && PROP_Date == null)
            {
                MessageBox.Show("Не заполнено обязательное поле \"" + PROP_Description + "\"", "Внимание!");
                this.Focus();
                return false;
            }
            return true;
        }
    }
}
