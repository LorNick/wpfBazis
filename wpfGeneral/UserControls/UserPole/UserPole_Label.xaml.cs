using System.Windows;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для поля Метки (Label)</summary>
    public partial class UserPole_Label : VirtualPole
    {
        /// <summary>СВОЙСТВО Описание вопроса</summary>
        public override string PROP_DefaultText
        {
            get { return PART_TextBox.Text; }
            set { PART_TextBox.Text = value; }
        }

        /// <summary>СВОЙСТВО Цвет текста</summary>
        public override Brush PROP_ForegroundText
        {
            get
            {
                return PART_TextBox.Foreground;
            }
            set
            {
                PART_TextBox.Foreground = value;
            }
        }

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_Label()
        {
            InitializeComponent();

            PART_TextBox.BorderThickness = new Thickness(0);
            PART_TextBox.ContextMenu = MyGlo.ContextMenu;
            PART_TextBox.Tag = this;
        }

        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            // Располагаем текст
            this.HorizontalAlignment = HorizontalAlignment.Left;
            if (PROP_Format.PROP_Value.ContainsKey("ac"))
                this.HorizontalAlignment = HorizontalAlignment.Center;
            if (PROP_Format.PROP_Value.ContainsKey("ar"))
                this.HorizontalAlignment = HorizontalAlignment.Right;
        }
    }
}
