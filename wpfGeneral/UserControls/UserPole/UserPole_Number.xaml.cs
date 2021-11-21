using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для Числового поля (Number)</summary>
    public partial class UserPole_Number : VirtualPole
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
                if (PART_TextBox == null)
                    return 0;
                return PART_TextBox.MaxLength;
            }
            set
            {
                if (PART_TextBox != null)
                    PART_TextBox.MaxLength = value;
            }
        }

        /// <summary>СВОЙСТВО Ширина текста</summary>
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
        public UserPole_Number()
        {
            InitializeComponent();

            PART_TextBox.ContextMenu = MyGlo.ContextMenu;
            PART_TextBox.Tag = this;
        }

        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            // Ограничиваем 20 символами
            PROP_MaxLength = 20;
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
            TextBox _TextBox = (TextBox)sender;
            // Если была ошибка, то при востановлении данных мы просто выходим
            if (_TextBox.Text == PROP_Text) return;
            // Проверим число на правильность
            if (_TextBox.Text != "")
            {
                // Если не парсится в число или мы ставим пробел, то вертаем всё назад
                if (!double.TryParse(_TextBox.Text, out double _Number) || _TextBox.Text.IndexOf(' ') != -1)
                {
                    int _Select = _TextBox.SelectionStart - 1;                  // запоминаем где курсор
                    _TextBox.Text = PROP_Text;                                  // востанавливаем данные
                    _TextBox.SelectionStart = _Select > 0 ? _Select : 0;        // возвращаем курсор на место
                    return;
                }
            }
            // Если есть шаблон
            if (this.PROP_FormShablon?.PROP_Created ?? false)
            {
                // Помечаем, что текст поменялся
                PROP_BoolChangeText = true;
                // Активируем кнопку "Сохранить"
                MyGlo.Event_SaveShablon?.Invoke(true);
            }
            // Перекрашиваем шрифт в черный, если был серый
            if (Equals(PROP_ForegroundText, Brushes.Gray))
                PROP_ForegroundText = Brushes.Black;
            // Сохраним число
            this.SetValue(DEPR_TextProperty, _TextBox.Text);
        }

        /// <summary>СОБЫТИЕ при вводе символа в TextBox</summary>
        private void PART_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Формат десятичного числа
            NumberFormatInfo _NumberFormatInfo = CultureInfo.CurrentCulture.NumberFormat;
            string _DecimalSeparator = _NumberFormatInfo.NumberDecimalSeparator;
            string _GroupSeparator = _NumberFormatInfo.NumberGroupSeparator;
            string _NegativeSign = _NumberFormatInfo.NegativeSign;
            TextBox _Text = (TextBox) sender;
            if (char.IsDigit(e.Text[0]))
            {
               // Это число
            }
            else if (e.Text.Equals(_DecimalSeparator) || e.Text.Equals(_GroupSeparator))
            {
               // Ну может быть и десятичная запятая
            }
            else if (e.Text.Equals(_NegativeSign) & _Text.SelectionStart == 0)
            {
                // Первый символ может быть и минус (ну для придир)
            }
            else
            {
                // Некорректный символ, сваливаем
                e.Handled = true;
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
