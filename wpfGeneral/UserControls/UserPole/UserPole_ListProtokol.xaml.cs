using System.Windows;
using System.Windows.Controls;
using wpfGeneral;
using wpfGeneral.UserFormShablon;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для поля Раздел (Razdel) </summary>
    public partial class UserPole_ListProtokol : UserControl
    {
        #region ---- СВОЙСТВО ----
        /// <summary>СВОЙСТВО Ответ</summary>
        public string PROP_Text
        {
            get
            {
                // Если Expander раскрыт то пишем "Открыт", иначе "Закрыт"
                if (PART_Expander.IsExpanded)
                    return "Открыт";
                return "Закрыт";
            }
            set
            {
                // Если открыт, то расскрываем Expander, иначе закрываем его
                if (value == "Открыт")
                    PART_Expander.IsExpanded = true;
                else

                    PART_Expander.IsExpanded = false;
            }
        }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_ListProtokol()
        {
            InitializeComponent();
        }

        /// <summary>МЕТОД Добавляем к полю дочерние элементы</summary>
        /// <param name="_Shablon">Дочернее поле</param>
        public bool MET_AddElement(VirtualFormShablon _Shablon)
        {
            PART_Expander.Header = _Shablon;
           // PART_StackPanelUp.Children.Add(pPole);
            return true;
        }

        /// <summary>СОБЫТИЕ Открытие экспандера</summary>
        private void PART_Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            //// Активируем кнопку "Сохранить"
            //if (MyGlo.callbackEvent_sEditShablon != null)
            //    MyGlo.callbackEvent_sEditShablon(true);
        }

        /// <summary>СОБЫТИЕ Закрытие экспандера</summary>
        private void PART_Expander_Expanded(object sender, RoutedEventArgs e)
        {
            // Активируем кнопку "Сохранить"
            //if (MyGlo.callbackEvent_sEditShablon != null)
            //    MyGlo.callbackEvent_sEditShablon(true);
        }
    }
}
