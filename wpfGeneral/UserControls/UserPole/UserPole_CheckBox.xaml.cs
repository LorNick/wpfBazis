using System.Windows;
using System.Windows.Controls;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для (CheckBox)</summary>
    public partial class UserPole_CheckBox : VirtualPole
    {
        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_Checked</summary>
        public static readonly DependencyProperty PROP_CheckedProperty =
            DependencyProperty.Register("PROP_Checked", typeof(bool), typeof(UserPole_CheckBox), null);

        /// <summary>СВОЙСТВО Поставить галочку</summary>
        public bool PROP_Checked
        {
            get { return (bool)this.GetValue(PROP_CheckedProperty); }
            set { this.SetValue(PROP_CheckedProperty, value); }
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

        /// <summary>РЕГИСТРАЦИЯ События при нажатии на CheckBox</summary>
        public static readonly RoutedEvent CheckedEvent;

        /// <summary>РЕГИСТРАЦИЯ Метода События при нажатии на CheckBox</summary>
        public event RoutedPropertyChangedEventHandler<bool> IsChecked
        {
            add { AddHandler(CheckedEvent, value); }
            remove { RemoveHandler(CheckedEvent, value); }
        }

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_CheckBox()
        {
            InitializeComponent();
        }

        /// <summary>Статический КОНСТРУКТОР</summary>
        static UserPole_CheckBox()
        {
            CheckedEvent = EventManager.RegisterRoutedEvent("IsChecked", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<bool>), typeof(UserPole_CheckBox));
        }

        /// <summary>СОБЫТИЕ Нажали на CheckBox</summary>
        private void PART_Checked_Click(object sender, RoutedEventArgs e)
        {
            CheckBox _CheckBox = (CheckBox)sender;
            // Активируем кнопку "Сохранить"
            MyGlo.callbackEvent_sEditShablon?.Invoke(true);
            RoutedPropertyChangedEventArgs<bool> _Args = new RoutedPropertyChangedEventArgs<bool>(false, true);
            _Args.RoutedEvent = CheckedEvent;
            _CheckBox.RaiseEvent(_Args);
        }
    }
}
