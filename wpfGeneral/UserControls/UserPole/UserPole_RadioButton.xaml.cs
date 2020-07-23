using System.Windows;
using System.Windows.Controls;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для поля Радио-кнопок (RadioButton = 12)</summary>
    public partial class UserPole_RadioButton : VirtualPole
    {
        #region ---- ЗАКРЫТЫЕ ПОЛЯ ----
        ///<summary>Ответ</summary>
        private string PRI_Text;
        #endregion ----

        #region  ---- СОБЫТИЯ ---- 
        ///<summary>Создаем событие</summary>
        public event RoutedEventHandler ItemsChanged
        {
            add { AddHandler(ItemsChangedEvent, value); }
            remove { RemoveHandler(ItemsChangedEvent, value); }
        }            
        #endregion ----

        #region ---- СВОЙСТВО ----
        /// <summary>СВОЙСТВО Коллекция RadioButton</summary>
        /// <remarks>Уникальное</remarks>
        public FreezableCollection<ListBoxItem> PROP_Items
        {
            get { return (FreezableCollection<ListBoxItem>)GetValue(DEPR_ItemsProperty); }
            set 
            { 
                SetValue(DEPR_ItemsProperty, value); 
                PART_ListBox.ItemsSource = PROP_Items;               
            }
        }

        /// <summary>СВОЙСТВО Ориентация</summary>
        public Orientation PROP_Orientation
        {
            get { return (Orientation)this.GetValue(DEPR_Orientation); }
            set { SetValue(DEPR_Orientation, value); }
        }

        /// <summary>СВОЙСТВО Номер выделенного элемента</summary>
        /// <remarks>Уникальное</remarks>
        public int PROP_Selected
        {
            get { return PART_ListBox.SelectedIndex; }
            set { PART_ListBox.SelectedIndex = value;  }
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

        /// <summary>СВОЙСТВО Ответ</summary>
        public override string PROP_Text
        {
            get { return PRI_Text; }
            set
            {              
                try
                {
                    PRI_Text = value;
                    foreach (ListBoxItem _ListBoxItem in PROP_Items)
                    {
                        if (_ListBoxItem.Tag.ToString()  == PRI_Text)
                            PART_ListBox.SelectedItem = _ListBoxItem;
                    }
                }
                catch
                {
                }
            }
        }      
        #endregion ----

        #region ---- РЕГИСТРАЦИЯ ----
        ///<summary>РЕГИСТРАЦИЯ Коллекции RadioButton</summary>
        private static readonly DependencyPropertyKey DEPR_ItemsPropertyKey =
            DependencyProperty.RegisterReadOnly(
              "PROP_Items",
              typeof(FreezableCollection<ListBoxItem>),
              typeof(UserPole_RadioButton),
              new FrameworkPropertyMetadata(new FreezableCollection<ListBoxItem>()));

        ///<summary>РЕГИСТРАЦИЯ Ориентации коллекции</summary>
        public static readonly DependencyProperty DEPR_Orientation =
            DependencyProperty.Register(
              "PROP_Orientation",
              typeof(Orientation), 
              typeof(UserPole_RadioButton), 
              new PropertyMetadata(Orientation.Horizontal));

        /// <summary>РЕГИСТРАЦИЯ RadioButton</summary>
        public static readonly DependencyProperty DEPR_ItemsProperty = DEPR_ItemsPropertyKey.DependencyProperty;

        ///<summary>РЕГИСТРАЦИЯ События</summary>
        public static readonly RoutedEvent ItemsChangedEvent = EventManager.RegisterRoutedEvent(
            "ItemsChanged", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(UserPole_RadioButton));
        #endregion ----

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_RadioButton()
        {
            InitializeComponent();
            // Обнуляем текущую коллекцию
            SetValue(DEPR_ItemsPropertyKey, new FreezableCollection<ListBoxItem>());
            // Подписываемся на событие изменения коллекции
            PROP_Items.Changed += PROP_Items_Changed;  
        }

        ///<summary>Создаем событие</summary>
        /// <param name="e">Информация о событии</param>
        protected virtual void OnItemsChanged(RoutedEventArgs e)
        {
            RaiseEvent(e);
        }

        /// <summary>СОБЫТИЕ При смене элемента</summary>
        private void PART_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PROP_Text = ((sender as ListBox).SelectedItem as ListBoxItem).Tag.ToString();
            // Вызываем событие
            this.RaiseEvent(new RoutedEventArgs(ItemsChangedEvent));
        }

        /// <summary>СОБЫТИЕ При измененнии коллекции PROP_Items</summary>
        private void PROP_Items_Changed(object sender, System.EventArgs e)
        {
            // Изменяемая коллекция
            var _List = sender as FreezableCollection<ListBoxItem>;
            // Если в списке есть кнопки
            if (_List.Count > 0)
            {
                // Искомая радиокнопка
                var _ListBoxItem = _List[_List.Count - 1];
                // Устанавливаем номер радиокнопки (если нужны другие номера, ставим на форме их в ручную в поле Tag
                _ListBoxItem.Tag = _List.Count - 1;
                // Выбираем начальную кнопку
                if (PROP_Text != "" & _ListBoxItem.Tag.ToString() == PROP_Text)
                    PART_ListBox.SelectedItem = _ListBoxItem;
            }
        }       
    }
}