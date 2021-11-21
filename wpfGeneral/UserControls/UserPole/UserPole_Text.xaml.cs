using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для Текстового поля (Text)</summary>
    public partial class UserPole_Text
    {
        #region ---- СВОЙСТВО ----
        /// <summary>СВОЙСТВО Изменялся ли текст</summary>
        public bool PROP_BoolChangeText { get; set; }

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

        /// <summary>СВОЙСТВО Цвет текста ответа</summary>
        public override Brush PROP_ForegroundText
        {
            get
            {
                return PART_TextBox?.Foreground;
            }
            set
            {
                if (PART_TextBox != null)
                    PART_TextBox.Foreground = value;
            }
        }

        /// <summary>СВОЙСТВО Количество символов в ответе</summary>
        public override int PROP_MaxLength
        {
            get
            {
                return PART_TextBox?.MaxLength ?? 0;
            }
            set
            {
                if (PART_TextBox != null)
                    PART_TextBox.MaxLength = value;
            }
        }

        /// <summary>СВОЙСТВО Ввод заглавных букв</summary>
        public override CharacterCasing PROP_CharacterCasing
        {
            get { return PART_TextBox.CharacterCasing ; }
            set { PART_TextBox.CharacterCasing = value; }
        }

        /// <summary>СВОЙСТВО Ширина текстового поля</summary>
        public override double PROP_WidthText
        {
            get { return PART_TextBox.Width; }
            set
            {
                PART_TextBox.Width = value;
                if (double.IsNaN(PART_TextBox.Width))
                    PART_TextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                else
                    PART_TextBox.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }

        /// <summary>РЕГИСТРАЦИЯ Метода События при вводе текста</summary>
        public event TextChangedEventHandler TextChanged;

        /// <summary>СВОЙСТВО Высота текстового поля</summary>
        public override double PROP_HeightText
        {
            get { return PART_TextBox.Height; }
            set
            {
                PART_TextBox.Height = value;
            }
        }

        /// <summary>СВОЙСТВО Минимальная Ширина описания</summary>
        public override double PROP_MinWidthDescription
        {
            get { return PART_Label.MinWidth; }
            set { PART_Label.MinWidth = value; }
        }

        /// <summary>СВОЙСТВО Помечаем поле текста</summary>
        public override byte PROP_PometkaText
        {
            get { return PRO_PometkaText; }
            set
            {
                PRO_PometkaText = value;
                if (PRO_PometkaText == 1)
                    PART_TextBox.BorderBrush = Brushes.Red;
                else
                    PART_TextBox.ClearValue(Border.BorderBrushProperty);
            }
        }

        /// <summary>СВОЙСТВО Скрываем рамку</summary>
        public override bool PROP_HideBorder
        {
            get
            {
                return PART_TextBox.BorderThickness.Equals(new Thickness(0));
            }
            set
            {
                if (value)
                    PART_TextBox.BorderThickness = new Thickness(0);
            }
        }

        /// <summary>СВОЙСТВО Располагаем текст (слева, по центру, справа)</summary>
        public override TextAlignment PROP_TextAlignment
        {
            get
            {
                return PART_TextBox.TextAlignment;
            }
            set
            {
                PART_TextBox.TextAlignment = value;
            }
        }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_Text()
        {
            InitializeComponent();

            PROP_Type = eVopros.Text;
            PART_TextBox.ContextMenu = MyGlo.ContextMenu;
            PART_TextBox.Tag = this;
        }

        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            // Ограничиваем 5000 символами
            PROP_MaxLength = 5000;
            // Располагаем текст
            PROP_TextAlignment = TextAlignment.Left;
            if (PROP_Format.PROP_Value.ContainsKey("fac"))
                PROP_TextAlignment = TextAlignment.Center;
            if (PROP_Format.PROP_Value.ContainsKey("far"))
                PROP_TextAlignment = TextAlignment.Right;
        }

        /// <summary>СОБЫТИЕ при вводе символа в TextBox</summary>
        private void PART_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Перекрашиваем шрифт в черный, если был серый
            if (Equals(PROP_ForegroundText, Brushes.Gray))
                PROP_ForegroundText = Brushes.Black;
            TextBox _TextBox = (TextBox)sender;
            this.SetValue(DEPR_TextProperty, _TextBox.Text);
            // Если есть шаблон
            if (this.PROP_FormShablon?.PROP_Created ?? false)
            {
                // Помечаем, что текст поменялся
                PROP_BoolChangeText = true;
                // Активируем кнопку "Сохранить"
                MyGlo.Event_SaveShablon?.Invoke(true);
            }
            // Вызываем TextChanged для новых форм (если есть, такое событие)
            TextChanged?.Invoke(this, e);
        }

        /// <summary>СОБЫТИЕ при вводе символа в TextBox</summary>
        private void PART_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Пропускаем недопустимые символы
            switch (e.Text)
            {
                case "\\":
                case "#":
                case "'":
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>СОБЫТИЕ выход из TextBox</summary>
        private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            // Если есть шаблон
            if ((this.PROP_FormShablon?.PROP_Created ?? false) && PROP_BoolChangeText)
            {
                // Снимаем отметку, об изменении текста
                PROP_BoolChangeText = false;
                // Запускаем Lua фунцкию, на изменение записи
                this.PROP_Lua?.MET_OnChange();
            }
        }
    }
}
