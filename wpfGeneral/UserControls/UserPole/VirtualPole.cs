using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfGeneral.UserFormShablon;
using wpfGeneral.UserLua;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС Виртуальное поле формы (шаблона)</summary>
    public class VirtualPole : UserControl
    {
        #region ---- Protected Поля ----
        /// <summary>Максимальное значение</summary>
        protected double PRO_ValueMax;

        /// <summary>Минимальное значение</summary>
        protected double PRO_ValueMin;

        /// <summary>Подстановка данных, при вставке - false или замена данных - true</summary>
        protected bool PRO_Insert;

        /// <summary>Подстановка данных Value - false или подставляем код ValueCod - true</summary>
        protected bool PRO_ValueCod;

        /// <summary>Номер документа Дополнительный (для заимствования свойств другого поля)</summary>
        protected int PRO_DopID;

        /// <summary>Номер индификатора VarId Дополнительный (для заимствования свойств другого поля)</summary>
        protected int PRO_DopVarId;

        /// <summary>Помечаем поле текста</summary>
        protected byte PRO_PometkaText;

        /// <summary>Помечаем поле текста</summary>
        protected bool PRO_Necessarily;
        #endregion
       
        #region ---- СВОЙСТВА ----
        /// <summary>СВОЙСТВО Документ этой ветки</summary>
        public UserDocument PROP_Docum { get; set; }

        /// <summary>СВОЙСТВО Lua</summary>
        public UserLua_Standart PROP_Lua { get; set; }

        /// <summary>СВОЙСТВО Описание вопроса</summary> 
        public virtual string PROP_Description { get; set; }

        /// <summary>СВОЙСТВО Ответ</summary>
        public virtual string PROP_Text
        {   
            get { return (string)this.GetValue(DEPR_TextProperty); }
            set { this.SetValue(DEPR_TextProperty, value); }
        }
        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_Text</summary>
        public static readonly DependencyProperty DEPR_TextProperty =
            DependencyProperty.Register("PROP_Text", typeof(string), typeof(VirtualPole), new PropertyMetadata(""));
       
        /// <summary>СВОЙСТВО Заначение ответа по умолчанию</summary>
        public virtual string PROP_DefaultText { get; set; }

        /// <summary>СВОЙСТВО Максимальное значение</summary>
        public virtual object PROP_ValueMax
        {
            get { return PRO_ValueMax; }
            set { PRO_ValueMax = (int)value; }
        }

        /// <summary>СВОЙСТВО Минимальное значение</summary>
        public virtual object PROP_ValueMin
        {
            get { return PRO_ValueMin; }
            set { PRO_ValueMin = (int)value; }
        }    

        /// <summary>СВОЙСТВО Количество символов в ответе</summary>
        public virtual int PROP_MaxLength { get ; set; }

        /// <summary>СВОЙСТВО Номер шаблона</summary>
        public virtual int PROP_Shablon { get; set; }

        /// <summary>СВОЙСТВО Номер индификатора VarId</summary>
        public virtual int PROP_VarId
        {   
            get { return (int)this.GetValue(DEPR_VarIdProperty); }
            set { this.SetValue(DEPR_VarIdProperty, value); }
        } 
        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_VarId</summary>
        public static readonly DependencyProperty DEPR_VarIdProperty =
            DependencyProperty.Register("PROP_VarId", typeof(int), typeof(VirtualPole), new PropertyMetadata(0));     
       
        /// <summary>СВОЙСТВО Подстановка данных - false или замена данных - true</summary>
        public virtual bool PROP_Insert
        {
            get { return PRO_Insert; }
            set { PRO_Insert = value; }
        }

        /// <summary>СВОЙСТВО Подстановка данных Value - false или подставляем код ValueCod - true</summary>
        public virtual bool PROP_ValueCod
        {
            get { return PRO_ValueCod; }
            set { PRO_ValueCod = value; }
        }

        /// <summary>СВОЙСТВО Цвет текста ответа</summary>
        public virtual Brush PROP_ForegroundText { get; set; }

        /// <summary>СВОЙСТВО Ввод заглавных букв</summary>
        public virtual CharacterCasing PROP_CharacterCasing { get; set; }

        /// <summary>СВОЙСТВО Ширина текста</summary>
        public virtual double PROP_WidthText { get; set; }

        /// <summary>СВОЙСТВО Высота текста</summary>
        public virtual double PROP_HeightText { get; set; }

        /// <summary>СВОЙСТВО Минимальная Ширина описания</summary>
        public virtual double PROP_MinWidthDescription { get; set; }

        /// <summary>СВОЙСТВО Помечаем поле текста</summary>
        public virtual byte PROP_PometkaText
        {
            get { return PRO_PometkaText; }
            set { PRO_PometkaText = value; }
        }

        /// <summary>СВОЙСТВО Скрываем рамку</summary>
        public virtual bool PROP_HideBorder { get; set; }

        /// <summary>СВОЙСТВО Располагаем текст (слева, по центру, справа)</summary>
        public virtual TextAlignment PROP_TextAlignment { get; set; }

        /// <summary>СВОЙСТВО Шаблон-форма, владелец данного поля</summary>
        public VirtualFormShablon PROP_FormShablon { get; set; }

        /// <summary>СВОЙСТВО Тип вопроса</summary>
        public eVopros PROP_Type { get; set; }

        /// <summary>СВОЙСТВО Формат поля</summary>
        public virtual MyFormat PROP_Format { get; set; }

        /// <summary>СВОЙСТВО Обязательное для заполнения поле</summary>
        public bool PROP_Necessarily
        {
            get { return PRO_Necessarily; }
            set
            {
                PRO_Necessarily = value;
                // Помечаем красной рамкой обязательное поле
                if (PRO_Necessarily)
                    PROP_PometkaText = 1;
                else
                    PROP_PometkaText = 0;
            }
        }
        #endregion 


        ///<summary>КОНСТРУКТОР</summary>
        public VirtualPole()
        {
            ToolTipService.SetShowDuration(this, 30000);        // Устанавливаем Время действия подсказки ToolTip 30 секунд
            PROP_Format = new MyFormat();
        }

        ///<summary>КОНСТРУКТОР (статический, пустой)</summary>
        static VirtualPole() { }

        ///<summary>МЕТОД при открытии контекстного меню (пустой)</summary>
        /// <param name="pContextMenu">Контекстного меню</param>
        protected virtual void MET_ContextMenu_Opened(ContextMenu pContextMenu) { }

        ///<summary>МЕТОД Инициализация поля (пустой)</summary>
        public virtual void MET_Inicial() { }

        /// <summary>МЕТОД Изменение поля (пустой)</summary>
        public delegate void callbackChanged();
        ///<summary>МЕТОД Инициализация поля (пустой)</summary>
        public callbackChanged MET_Changed;

        ///<summary>МЕТОД Действие при сохранении (пустой)</summary>
        public virtual bool MET_Save() { return true; }

        ///<summary>МЕТОД Проверка на допустимость данных и полноте заполнения</summary>
        public virtual bool MET_Verification()
        {
            // Проверяем на заполнение поля
            if (PROP_Necessarily && PROP_Text.Length == 0)
            {
                MessageBox.Show("Не заполнено обязательное поле \"" + PROP_Description + "\"", "Внимание!");
                // ReSharper disable once ArrangeThisQualifier
               this.Focus();
                return false;
            }
            return true;
        }

        /// <summary>МЕТОД Добавляем к полю дочерние элементы, если есть (пустой)</summary>
        /// <param name="pPole">Дочернее поле</param>
        public virtual bool MET_AddElement(VirtualPole pPole) { return true; }
    }
}
