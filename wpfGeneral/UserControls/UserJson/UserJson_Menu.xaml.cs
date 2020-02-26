using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для сборного поля Json (VirtualJson)</summary>
    public partial class UserJson_Menu : VirtualJson
    {
        #region ---- СВОЙСТВО ----               
        /// <summary>СВОЙСТВО Описание вопроса</summary>
        public override string PROP_Tag
        {
            get { return (string)PART_Tag.Content; }
            set { PART_Tag.Content = value; }
        }

        /// <summary>СВОЙСТВО Описание вопроса</summary>
        public override string PROP_TagName
        {
            get { return (string)PART_TagName.Content; }
            set { PART_TagName.Content = value; }
        }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserJson_Menu()
        {
            InitializeComponent();

            //PROP_Type = eVopros.Text;
            //PART_TextBox.ContextMenu = MyGlo.ContextMenu;
            //PART_TextBox.Tag = this;
        }

		/////<summary>МЕТОД Инициализация поля</summary>
		//public void MET_Inicial()
		//{
  //          // Ограничиваем 5000 символами
  // //         PROP_MaxLength = 5000;
		//	//// Располагаем текст
		//	//PROP_TextAlignment = TextAlignment.Left;
		//	//if (PROP_Format.PROP_Value.ContainsKey("fac"))
		//	//	PROP_TextAlignment = TextAlignment.Center;
		//	//if (PROP_Format.PROP_Value.ContainsKey("far"))
		//	//	PROP_TextAlignment = TextAlignment.Right;
		//}

        /// <summary>СОБЫТИЕ при вводе символа в TextBox</summary>
        private void PART_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //// Перекрашиваем шрифт в черный, если был серый
            //if (Equals(PROP_ForegroundText, Brushes.Gray))
            //    PROP_ForegroundText = Brushes.Black;
            //TextBox _TextBox = (TextBox)sender;
            //this.SetValue(DEPR_TextProperty, _TextBox.Text);
            //// Если есть шаблон
            //if (this.PROP_FormShablon?.PROP_Created ?? false)
            //{
            //    // Помечаем, что текст поменялся
            //    PROP_BoolChangeText = true;
            //    // Активируем кнопку "Сохранить"
            //    MyGlo.callbackEvent_sEditShablon?.Invoke(true);
            //}
        }

        ///// <summary>СОБЫТИЕ при вводе символа в TextBox</summary>
        //private void PART_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        //{
        //    // Пропускаем недопустимые символы
        //    switch (e.Text)
        //    {
        //        case "\\":
        //        case "#":
        //        case "'":
        //            e.Handled = true;
        //            break;            
        //    }
        //}

        ///// <summary>СОБЫТИЕ выход из TextBox</summary>
        //private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    // Если есть шаблон
        //    if ((this.PROP_FormShablon?.PROP_Created ?? false) && PROP_BoolChangeText)
        //    {
        //        // Снимаем отметку, об изменении текста
        //        PROP_BoolChangeText = false;
        //        // Запускаем Lua фунцкию, на изменение записи
        //        this.PROP_Lua?.MET_OnChange();
        //    }
        //}
    }
}
