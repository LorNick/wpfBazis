using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using wpfGeneral.UserNodes;
using wpfGeneral.UserStruct;
using wpfGeneral.UserWindows;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для поля Истории болезни (History)</summary>
    public partial class UserPole_History : UserControl
    {
        /// <summary>Тип делегата</summary>
        public delegate void callbackEvent(UserPole_History pMyPole);

        /// <summary>Переменная делегата (открытие вкладки в первый раз)</summary>
        public callbackEvent callbackOpenNew;

        /// <summary>Очередь элементов истории (если есть подэлементы)</summary>     
        public readonly Queue PROP_PoleHistory = new Queue();

        /// <summary>Очередь элементов истории (если есть подэлементы)</summary>
        public RichTextBox PUB_RichTextBox;


        #region ---- СВОЙСТВО ----
        /// <summary>СВОЙСТВО Описание вопроса</summary>
        public string PROP_Description
        {
            get { return PART_Description.Text; }
            set { PART_Description.Text = value; }
        }

        /// <summary>СВОЙСТВО Дата посещения/стационара/документа</summary>
        public string PROP_Date
        {
            get { return PART_Date.Text; }
            set { PART_Date.Text = value; }
        }

        /// <summary>СВОЙСТВО Тип документа/отделение/профиль врача/наименование документа</summary>
        public string PROP_Document
        {
            get { return PART_Document.Text; }
            set { PART_Document.Text = value; }
        }

        /// <summary>СВОЙСТВО Инконка</summary>
        public BitmapImage PROP_BitmapImage
        {
            get { return (BitmapImage)PART_Image.Source; }
            set { PART_Image.Source = value; }
        }

        /// <summary>СВОЙСТВО Дополнительная Инконка (та что справа)</summary>
        public BitmapImage PROP_BitmapImageInform
        {
            get { return (BitmapImage)PART_ImageInform.Source; }
            set { PART_ImageInform.Source = value; }
        }

        /// <summary>СВОЙСТВО Дополнительная Инконка (та что справа)</summary>
        public string PROP_ImageInformToolTip
        {
            get { return (string)PART_ImageInform.ToolTip; }
            set { PART_ImageInform.ToolTip = value; }
        }

        /// <summary>СВОЙСТВО Тип вкладки 1 - поликлиника, 2 - стационар, 3 - параклиника, 4 - КДЛ</summary>
        public eTipDocum PROP_Type { get; set; }

        /// <summary>СВОЙСТВО Код документа (протокола)</summary>
        public decimal PROP_Cod { get; set; }

        /// <summary>СВОЙСТВО Код APSTAC (карточки)</summary>
        public decimal PROP_CodApstac { get; set; }

        /// <summary>СВОЙСТВО Код родительского документа</summary>
        public decimal PROP_ParentCod { get; set; }

        /// <summary>СВОЙСТВО Номер шаблона</summary>
        public int PROP_NumerShablon { get; set; }

        /// <summary>СВОЙСТВО Цвет фона PART_Expander</summary>
        public Brush PROP_Background
        {
            get
            {
                return PART_DockPanel?.Background;
            }
            set
            {
                if (PART_DockPanel != null)
                    PART_DockPanel.Background = value;
            }
        }

        /// <summary>СВОЙСТВО ФИО Пользователя (Врача)</summary>
        public string PROP_Vrach
        {
            get { return PART_Vrach.Text; }
            set { PART_Vrach.Text = value; }
        }

        /// <summary>СВОЙСТВО Код Пользователя (Врача)</summary>
        public int PROP_UserCod { get; set; }

        /// <summary>СВОЙСТВО Дата посещения</summary>
        public string PROP_Dp { get; set; }

        /// <summary>СВОЙСТВО Код диагноза</summary>
        public string PROP_Diag
        {
            get { return PART_Diag.Text; }
            set { PART_Diag.Text = value; }
        }

        /// <summary>СВОЙСТВО Метка</summary>
        public string PROP_Metca
        {
            get { return PART_Metca.Text; }
            set { PART_Metca.Text = value; }
        }

        /// <summary>СВОЙСТВО Профиль врача</summary>
        public string PROP_Profil { get; set; }

        /// <summary>СВОЙСТВО true - вкладка содержит текст дкумента, false - содержит другие вкладки</summary>
        public bool PROP_IsTexted { get; set; }

        /// <summary>СВОЙСТВО Документ раскрываемый этим компонентом</summary>
        public UserDocument PROP_DocumHistory { get; set; }

        /// <summary>СВОЙСТВО Ветка если есть</summary>
        public VirtualNodes PROP_Nodes { get; set; }

        /// <summary>СВОЙСТВО Открывает/закрывает экспандер</summary>
        public bool PROP_IsExpanded
        {
            get { return PART_Expander.IsEnabled; }
            set { PART_Expander.IsExpanded = value; }
        }

        /// <summary>СВОЙСТВО КДЛ</summary>
        public string PROP_Kdl { get; set; }

        /// <summary>Признак удаления true - удален, false - не удален</summary>
        private bool PRI_IsDelete;

        /// <summary>СВОЙСТВО Признак удаления true - удален (1), false - не удален (0)</summary>
        public bool PROP_IsDelete
        {
            get { return PRI_IsDelete; }
            set
            {
                PRI_IsDelete = value;

                if (PRI_IsDelete)
                {
                    PART_Description.TextDecorations = TextDecorations.Strikethrough;
                    PART_Date.TextDecorations = TextDecorations.Strikethrough;
                    PART_Document.TextDecorations = TextDecorations.Strikethrough;
                    PART_Vrach.TextDecorations = TextDecorations.Strikethrough;
                    PART_Diag.TextDecorations = TextDecorations.Strikethrough;
                    PART_Metca.TextDecorations = TextDecorations.Strikethrough;
                }
                else
                {
                    PART_Description.TextDecorations = null;
                    PART_Date.TextDecorations = null;
                    PART_Document.TextDecorations = null;
                    PART_Vrach.TextDecorations = null;
                    PART_Diag.TextDecorations = null;
                    PART_Metca.TextDecorations = null;
                }

                // Показываем поле или скрываем, в зависимости от флага
                if (!PRI_IsDelete || MyGlo.ShowDeletedProtokol)
                    Visibility = Visibility.Visible;
                else
                    Visibility = Visibility.Collapsed;

                PART_MenuItem_Delete.Visibility = PRI_IsDelete ? Visibility.Collapsed : Visibility.Visible;
                PART_MenuItem_Restore.Visibility = PRI_IsDelete ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>СВОЙСТВО xLog (пока только для поликлиники и стационара)</summary>
        public string PROP_xLog { get; set; }
        #endregion


        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_History()
        {
            InitializeComponent();
            // По умолчанию вкладка содержит другие элементы, а не текст документа
            PROP_IsTexted = false;

            // Обнуляем содержание от служебных слов
            PROP_Description = "";
            PROP_Date = "";
            PROP_Document = "";
            PROP_Vrach = "";
            PROP_Diag = "";
            PROP_Metca = "";
        }

        ///<summary>МЕТОД Инициализация поля</summary>
        public void MET_Inicial()
        {
            if (PROP_DocumHistory?.PROP_Protokol != null)
            {
                PROP_DocumHistory.PROP_Protokol.OnDelete += delegate { PROP_IsDelete = true; };
                PROP_DocumHistory.PROP_Protokol.OnRestore += delegate { PROP_IsDelete = false; };
            }
        }

        /// <summary>МЕТОД Добавляем иконку</summary>
        /// <param name="pIconName">Наименование иконки</param>
        public void MET_LoadIcon(string pIconName)
        {
            try
            {
                PROP_BitmapImage = (BitmapImage)FindResource(pIconName);     // настраиваем картинку
            }
            catch (Exception e)
            {
                PROP_BitmapImage = (BitmapImage)FindResource("mnDoc_7");    // не нашел иконку, ставим стандартную
                MyGlo.PUB_Logger.Error(e, $"Для поля UserPole_History типа: {PROP_Type}, не найдена иконка: {pIconName}.");
            }
        }

        /// <summary>МЕТОД Добавляем дополнительную иконку (ту что справа)</summary>
        /// <param name="pIconName">Наименование иконки</param>
        /// <param name="pToolTip">Подсказка</param>
        public void MET_LoadIconInform(string pIconName, string pToolTip = "")
        {
            try
            {
                PROP_BitmapImageInform = (BitmapImage)FindResource(pIconName);     // настраиваем картинку
                PROP_ImageInformToolTip = pToolTip;
            }
            catch (Exception e)
            {
                // Иконку не ставим, раз не нашли
                MyGlo.PUB_Logger.Error(e, $"Для поля UserPole_History типа: {PROP_Type}, не найдена дополнительная иконка: {pIconName}.");
            }
        }

        /// <summary>МЕТОД Добавляем к полю дочерние элементы</summary>
        /// <param name="pElement">Дочернее поле</param>
        public bool MET_AddEle(UIElement pElement)
        {
            if (pElement is UserPole_History)
                PROP_PoleHistory.Enqueue(pElement);
            PART_StackPanel.Children.Add(pElement);

            return true;
        }

        /// <summary>МЕТОД Вывод на печать</summary>
        /// <param name="pFlowDocument">Поток текста для отображения печати</param>
        public void MET_StacPanel(FlowDocument pFlowDocument)
        {
            // Если экспандер закрыт - выходим
            if (!PART_Expander.IsExpanded) return;
            // Если есть подвкладки
            if (!PROP_IsTexted)
            {
                // Перебераем все подвкладки
                foreach (UserPole_History _Pole in PROP_PoleHistory)
                {
                    _Pole.MET_StacPanel(pFlowDocument);
                }
            }
            else // Выводим на печать текста
            {
                // Создаем временный поток для копирования
                using (MemoryStream _Stream = new MemoryStream())
                {
                    // Выделяем область которую хотим копировать
                    TextRange _TextRange = new TextRange(PUB_RichTextBox.Document.ContentStart, PUB_RichTextBox.Document.ContentEnd);
                    // Копируем текст в поток
                    _TextRange.Save(_Stream, DataFormats.Xaml);
                    _Stream.Position = 0;
                    // Новый параграф (что бы был отступ между документами)
                    pFlowDocument.Blocks.Add(new Paragraph());
                    // Выделяем коновку, куда хотим вставить текст
                    _TextRange = new TextRange(pFlowDocument.ContentEnd, pFlowDocument.ContentEnd);
                    // Вставляем текст в поток просмотра печати
                    _TextRange.Load(_Stream, DataFormats.Xaml);
                }
            }
        }

        /// <summary>СОБЫТИЕ Открытие контекстного меню</summary>
        private void UserPole_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            PART_MenuItem_CardAdmin.IsEnabled = MyGlo.Admin;
            PART_MenuItem_Edit.IsEnabled = MyGlo.Admin || MyGlo.FlagEdit;

            if (PROP_IsTexted)
            {
                PART_MenuItem_Delete.IsEnabled = PROP_DocumHistory?.PROP_IsUserDeleted == true;
                PART_MenuItem_Delete.Visibility = PRI_IsDelete ? Visibility.Collapsed : Visibility.Visible;
                PART_MenuItem_Restore.Visibility = PRI_IsDelete ? Visibility.Visible : Visibility.Collapsed;
                //PART_MenuItem_Log.IsEnabled = PROP_DocumHistory?.PROP_TipDocum != eTipDocum.Null && PROP_IsTexted;
            }
            else
            {
                PART_MenuItem_Delete.Visibility = Visibility.Collapsed;
                PART_MenuItem_Restore.Visibility = Visibility.Collapsed;
                //PART_MenuItem_Log.Visibility = Visibility.Collapsed; 
            }
           
        }
        
        /// <summary>СОБЫТИЕ Открытие Expander</summary>
        private void PART_Expander_Expanded(object sender, RoutedEventArgs e)
        {
            callbackOpenNew?.Invoke(this);
        }

        #region ---- СОБЫТИЯ Контекстного меню----
        /// <summary>СОБЫТИЕ Выбор контекстного меню Карточка Администратора</summary>
        private void PART_MenuItem_CardAdmin_Click(object sender, RoutedEventArgs e)
        {
            // Открываем Форму Карточка Администратора
            UserWindow_CardAdmin _WinCardAdmin = new UserWindow_CardAdmin
            {
                PROP_PoleHistory = this,      
                WindowStyle = WindowStyle.ToolWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            _WinCardAdmin.Show();
        }

        /// <summary>СОБЫТИЕ Выбор контекстного меню Редактировать</summary>     
        private void PART_MenuItem_Edit_Click(object sender, RoutedEventArgs e)
        {
            MyTipProtokol _MyTipProtokol;
            decimal _IND;
            if (PROP_Nodes != null) // протоколы основной карточки
            {
                _MyTipProtokol = PROP_Nodes.PROP_shaTipProtokol;
                _IND = PROP_Nodes.PROP_shaIND;
                if (_IND == 0) return;
            }
            else // из истории болезни
            {
                _MyTipProtokol = new MyTipProtokol(PROP_Type);
                _IND = PROP_CodApstac;
            }

            // Если документы Kdl, то выходим
            if (_MyTipProtokol.PROP_TipDocum == eTipDocum.Kdl)
            {
                MessageBox.Show("И что вы хотите?", "Опомнитесь!");
                return;
            }

            // Пытаемся открыть новую копию программы, для редактирования протоколов
            MyMet.MET_EditWindows(_MyTipProtokol.PROP_TipDocum, _IND, MyGlo.KL);
        }

        /// <summary>СОБЫТИЕ Выбор контекстного меню Открытие окна логов документа</summary>     
        private void PART_MenuItem_Log_Click(object sender, RoutedEventArgs e)
        {
            string _jLog;

            // Для Apac и Apstac
            if (!string.IsNullOrEmpty(PROP_xLog))
                _jLog = PROP_xLog;
            else
            {
                // Если протокола нет, то создаем
                if (PROP_DocumHistory.PROP_Protokol == null)
                    PROP_DocumHistory.PROP_Protokol = UserProtokol.MET_FactoryProtokol(PROP_Type, (int)PROP_Cod);

                _jLog = PROP_DocumHistory.PROP_Protokol.PROP_xLog;
            }

            if (!string.IsNullOrEmpty(_jLog))
            {
                // Открываем Форму Карточка Log
                UserWindow_DocumLog _WinLog = new UserWindow_DocumLog(_jLog);
                _WinLog.Show();
            }
            else
                MessageBox.Show("А логов то и нет!", "Ошибочка вышла");            
        }

        /// <summary>СОБЫТИЕ Выбор контекстного меню Удалить протокол</summary>     
        private void PART_MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите удалить этот протокол?", "Удалить", MessageBoxButton.YesNo) ==
                MessageBoxResult.Yes)
            {                
                PROP_DocumHistory.PROP_Protokol.OnDelete?.Invoke();
            }                                
        }

        /// <summary>СОБЫТИЕ Выбор контекстного меню Восстаовить протокол</summary>     
        private void PART_MenuItem_Restore_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы точно хотите восстановить этот протокол?", "Восстановить", MessageBoxButton.YesNo) ==
                MessageBoxResult.Yes)
            {
                PROP_DocumHistory.PROP_Protokol.OnRestore?.Invoke();               
            }           
        }
        #endregion       
    }
}
