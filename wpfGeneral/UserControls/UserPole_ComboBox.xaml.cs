using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для Текстового поля (Text)</summary>
    public partial class UserPole_ComboBox : VirtualPole
    {
        #region ---- СВОЙСТВО ----
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
                    PART_Label.Padding = new Thickness(5, 0, 5 ,0);
            }
        }

        /// <summary>СВОЙСТВО Ответ</summary>
        public override string PROP_Text
        {
            get { return PROP_ComboBox.SelectedValue?.ToString() ?? ""; }
            set { PROP_ComboBox.SelectedValue = value; }
        }

        /// <summary>СВОЙСТВО Цвет текста ответа</summary>
        public override Brush PROP_ForegroundText
        {
            get
            {
                return PART_ComboBox?.Foreground;
            }
            set
            {
                if (PART_ComboBox != null)
                    PART_ComboBox.Foreground = value;
            }
        }

         /// <summary>СВОЙСТВО Минимальная Ширина описания</summary>
        public override double PROP_MinWidthDescription
        {
            get { return PART_Label.MinWidth; }
            set { PART_Label.MinWidth = value; }
        }

        /// <summary>СВОЙСТВО Ширина текста</summary>
        public override double PROP_WidthText
        {
            get { return PART_ComboBox.Width; }
            set
            {
                PART_ComboBox.Width = value;
                if (double.IsNaN(PART_ComboBox.Width))
                    PART_ComboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                else
                    PART_ComboBox.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }

        /// <summary>СВОЙСТВО Помечаем поле текста</summary>
        public override byte PROP_PometkaText
        {
            get { return PRO_PometkaText; }
            set
            {
                PRO_PometkaText = value;
                if (PRO_PometkaText == 1)
                {
                    PART_Border.BorderBrush = Brushes.Red;
                }
                else
                    PART_Border.BorderBrush = Brushes.Gray;
            }
        }
        
        /// <summary>СВОЙСТВО ComboBox</summary>
        public RadComboBox PROP_ComboBox => PART_ComboBox;

        /// <summary>СВОЙСТВО Сортировка списка (false - по алфавиту, true - по порядку Cod)</summary>
        public bool PROP_SortList { get; set; }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_ComboBox()
        {
            InitializeComponent();

            PART_ComboBox.ContextMenu = MyGlo.ContextMenu;
            PART_ComboBox.Tag = this;
        }        

        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            // Располагаем текст
			PROP_TextAlignment = TextAlignment.Left;
            if (PROP_Format.MET_If("fac"))
                PROP_TextAlignment = TextAlignment.Center;
            if (PROP_Format.MET_If("far"))
                PROP_TextAlignment = TextAlignment.Right;

            // Сортировка нисподающего списка (по умолчанию по алфавиту, иначе по порядку List)
            PROP_SortList = PROP_Format.MET_If("sortlist");
          
            // Загружаем варианты ответов
            string _List = PROP_FormShablon.PROP_TipProtokol.PROP_List;
            MySql.MET_DsAdapterFill(MyQuery.s_ListDocum_Select_4(_List, PROP_Shablon, PROP_VarId, PROP_SortList), Name);                             
            List<string> _ValueList = (from DataRow _Row in MyGlo.DataSet.Tables[Name].Rows select _Row["Value"].ToString()).ToList();
            PROP_ComboBox.ItemsSource = _ValueList;

            // Начальное значение
            PROP_ComboBox.SelectedValue = PROP_Text;
        }

        /// <summary>СОБЫТИЕ При изменении поля</summary>
        private void PART_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
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
