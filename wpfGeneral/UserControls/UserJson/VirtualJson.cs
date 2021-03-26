using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС Виртуальное поле формы (шаблона)</summary>
    public class VirtualJson : UserControl
    {
        #region ---- Protected Поля ----
        ///// <summary>Максимальное значение</summary>
        //protected double PRO_ValueMax;
        #endregion

        #region ---- СВОЙСТВА ----
        /// <summary>СВОЙСТВО Наименование тега (из таблицы s_Tags, поле Tag)</summary>
        public virtual string PROP_Tag { get; set; }

        /// <summary>СВОЙСТВО Читаемое Наименование тега (из таблицы s_Tags, поле TagName)</summary>
        public virtual string PROP_TagName
        {
            get { return (string)this.GetValue(DEPR_TagNameProperty); }
            set { this.SetValue(DEPR_TagNameProperty, value); }
        }
        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_TagName</summary>
        public static readonly DependencyProperty DEPR_TagNameProperty =
            DependencyProperty.Register("PROP_TagName", typeof(string), typeof(VirtualJson), new PropertyMetadata(""));

        /// <summary>СВОЙСТВО Описние тега (из таблицы s_Tags, поле Discription)</summary>
        public string PROP_Discription { get; set; }

        /// <summary>СВОЙСТВО Значение тега</summary>
        public virtual object PROP_Value
        {
            get { return (string)this.GetValue(DEPR_ValueProperty); }
            set { this.SetValue(DEPR_ValueProperty, value); }
        }
        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_Value</summary>
        public static readonly DependencyProperty DEPR_ValueProperty =
            DependencyProperty.Register("PROP_Value", typeof(object), typeof(VirtualJson), new PropertyMetadata(""));
        #endregion

        ///<summary>КОНСТРУКТОР</summary>
        public VirtualJson()
        {
            ToolTipService.SetShowDuration(this, 30000);        // Устанавливаем Время действия подсказки ToolTip 30 секунд
        }

        ///<summary>КОНСТРУКТОР (статический, пустой)</summary>
        static VirtualJson() { }

        ///<summary>МЕТОД при открытии контекстного меню (пустой)</summary>
        /// <param name="pContextMenu">Контекстного меню</param>
        protected virtual void MET_ContextMenu_Opened(ContextMenu pContextMenu) { }

        ///<summary>МЕТОД Инициализация поля (пустой)</summary>
        public virtual void MET_Inicial() { }

        ///<summary>МЕТОД Действие при сохранении (пустой)</summary>
        public virtual bool MET_Save() { return true; }

        ///<summary>МЕТОД Проверка на допустимость данных и полноте заполнения</summary>
        public virtual bool MET_Verification()
        {
            //// Проверяем на заполнение поля
            //if (PROP_Necessarily && PROP_Text.Length == 0)
            //{
            //    MessageBox.Show("Не заполнено обязательное поле \"" + PROP_Description + "\"", "Внимание!");
            //    // ReSharper disable once ArrangeThisQualifier
            //   this.Focus();
            //    return false;
            //}
            return true;
        }

        /// <summary>МЕТОД Добавляем к полю дочерние элементы, если есть (пустой)</summary>
        /// <param name="pPole">Дочернее поле</param>
        public virtual bool MET_AddElement(VirtualPole pPole) { return true; }
    }
}
