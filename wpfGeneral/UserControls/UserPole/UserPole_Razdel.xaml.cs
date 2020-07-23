using System.Windows;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для поля Раздел (Razdel) </summary>
    public partial class UserPole_Razdel : VirtualPole
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
                    PART_Label.Padding = new Thickness(5, 0, 5, 0);
            }
        }

        /// <summary>СВОЙСТВО Ответ</summary>
        public override string PROP_Text
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
        public UserPole_Razdel()
        {
            InitializeComponent();

            PART_Expander.ContextMenu = MyGlo.ContextMenu;
            PART_Expander.Tag = this;
        }

        /// <summary>МЕТОД Добавляем к полю дочерние элементы</summary>
        /// <param name="pPole">Дочернее поле</param>
        public override bool MET_AddElement(VirtualPole pPole)
        {
            PART_StackPanel.Children.Add(pPole);
            return true;
        }

        /// <summary>СОБЫТИЕ Открытие экспандера</summary>
        private void PART_Expander_Collapsed(object sender, RoutedEventArgs e)
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

        /// <summary>СОБЫТИЕ Закрытие экспандера</summary>
        private void PART_Expander_Expanded(object sender, RoutedEventArgs e)
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
