using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using wpfGeneral.UserFormShablon;
using wpfGeneral.UserStruct;
using wpfGeneral.UserTab;
using wpfStatic;

namespace wpfGeneral.UserNodes
{
    /// <summary>КЛАСС Виртуальная ветка</summary>
    public abstract class VirtualNodes : TreeViewItem
    {
        /// <summary>Наличие протокола</summary>
        private bool PRI_PresenceProtokol;

        /// <summary>Текст ветки</summary>
        private string PRI_Text;

        /// <summary>Нижний Текст ветки</summary>
        private string PRI_TextDown;

        /// <summary>Номер сортировки ветки</summary>
        public int PRI_Sort = 0;

        #region ---- Свойства ----
        /// <summary>СВОЙСТВО Документ этой ветки</summary>
        public UserDocument PROP_Docum { get; set; }

        /// <summary>СВОЙСТВО Тип ветки eTipNodes (нужно избавиться)</summary>
        public eTipNodes PROP_TipNodes { get; set; }

        /// <summary>СВОЙСТВО Текст ветки по умолчанию</summary>
        public string PROP_TextDefault { get; set; }

        /// <summary>СВОЙСТВО Текст ветки (заголовок)</summary>
        public virtual string PROP_Text
        {
            get { return PRI_Text; }
            set
            {
                PRI_Text = value;
                // Если вкладка уже создана, то меняем у неё текст
                if (Header != null)
                {
                    (Header as UserTabNod).PROP_Text = PROP_Text;
                }
            }
        }

        /// <summary>СВОЙСТВО Нижний Текст ветки (заголовок)</summary>
        public virtual string PROP_TextDown
        {
            get { return PRI_TextDown; }
            set
            {
                PRI_TextDown = value;
                // Если вкладка уже создана, то меняем у неё текст
                if (Header != null)
                {
                    (Header as UserTabNod).PROP_TextDown = PROP_TextDown;
                }
            }
        }

        /// <summary>СВОЙСТВО Иконка ветки</summary>
        public ImageSource PROP_ImageSource { get; set; }

        /// <summary>СВОЙСТВО Наименование Иконки ветки</summary>
        public string PROP_ImageName { get; set; }

        /// <summary>СВОЙСТВО Родительская ветка</summary>
        public VirtualNodes PROP_Parent { get; set; }

        /// <summary>СВОЙСТВО Имя родительской ветки (если нету, то родитель - корень)</summary>
        public string PROP_ParentName
        {
            get
            {
                // Если есть имя родительской ветки, то показываем
                return PROP_Parent.Name ?? "";
            }
            set
            {
                // Добавляем ветку
                if (value == "")
                    MyGlo.TreeView.Items.Add(this);                             // добавляем в дерево
                else
                {
                    // Находим родительскую ветку
                    PROP_Parent = (VirtualNodes)MyGlo.TreeView.FindName(value);
                    PROP_Parent.Items.Add(this);                             // добавляем в родительскую ветку
                }
            }
        }

        /// <summary>СВОЙСТВО (ветки) Дата ветки</summary>
        public DateTime PROP_Data { get; set; }

        /// <summary>СВОЙСТВО (ветки) Удаленная ветка true - удалена, false - не удалена</summary>
        public bool PROP_Delete { get; set; } = false;

        /// <summary>СВОЙСТВО (шаблона) Номер шаблона</summary>
        public virtual int PROP_shaNomerShablon { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Наличие шаблона</summary>
        public bool PROP_shaPresenceShablon { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Количество заполненных подветок</summary>
        public virtual int PROP_shaCountNodesChild { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Наличие протокола (Данных)</summary>
        public bool PROP_shaPresenceProtokol
        {
            get { return PRI_PresenceProtokol; }
            set
            {
                PRI_PresenceProtokol = value;
                if (value)
                {
                    this.Foreground = Brushes.Black;
                    if (PROP_Parent != null) PROP_Parent.PROP_shaCountNodesChild++;
                }
                else
                {
                    this.Foreground = Brushes.Gray;
                    if (PROP_Parent != null && PROP_Parent.PROP_shaCountNodesChild > 0) PROP_Parent.PROP_shaCountNodesChild--;
                }
            }
        }

        /// <summary>СВОЙСТВО (шаблона) Разрешение редактирования шаблона</summary>
        public virtual bool PROP_shaButtonEdit { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Разрешение создания шаблона</summary>
        public virtual bool PROP_shaButtonNew { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Кнопка сохранения шаблона</summary>
        public virtual Visibility PROP_shaButtonSvaveSha { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Кнопка очистить шаблон</summary>
        public virtual Visibility PROP_shaButtonClearSha { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Индекс протокола</summary>
        public virtual int PROP_shaIndex { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Номер посещения/стационара/обследования</summary>
        public decimal PROP_shaIND { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Тип протокола</summary>
        public MyTipProtokol PROP_shaTipProtokol { get; set; }

        /// <summary>СВОЙСТВО (печати) Отступы при печати (0 - нету,1 - стандартные, 2 - минимальные )</summary>
        public byte PROP_prnPadding { get; set; }
        #endregion

        ///<summary>МЕТОД Инициализация ветки</summary>
        public virtual void MET_Inizial()
        {
            // Данные ветки
            try
            {
                PROP_ImageSource = (BitmapImage)FindResource(PROP_ImageName);   // настраиваем картинку
            }
            catch (Exception)
            {
                PROP_ImageSource = (BitmapImage)FindResource("mnDoc_7");        // не нашел иконку, ставим стандартную
            }

            PROP_shaButtonNew = false;
            PROP_shaButtonEdit = false;
            PROP_shaButtonSvaveSha = Visibility.Visible;
            PROP_shaButtonClearSha = Visibility.Visible;
            PROP_prnPadding = 1;
            // Рисуем ветку
            Header = new UserTabNod(PROP_ImageSource, PROP_Text, PROP_TextDown);
            // Регистрируем имя (ну что бы потом по нему можно было искать)
            try
            {
                if(this.FindName(this.Name) != null)
                    this.UnregisterName(this.Name);
                this.RegisterName(this.Name, this);
            }
            catch
            {
                this.UnregisterName(this.Name);
                this.RegisterName(this.Name, this);
            }
            // Регистрируем события на удаление и восстановление протоколов
            if (PROP_Docum != null && PROP_Docum.PROP_Protokol != null)
            {
                PROP_Docum.PROP_Protokol.OnDelete += delegate { MET_Delete(true); };
                PROP_Docum.PROP_Protokol.OnRestore += delegate { MET_Delete(false); };
            }
        }

        /// <summary>МЕТОД Показываем ветку (закладку)</summary>
        /// <param name="pParent">Вкладка - родитель закладки</param>
        /// <param name="pVkladki">Тип вкладки</param>
        /// <param name="pClose">Кнопка закрытия вкладки</param>
        public virtual UserTabVrladka MET_Header(object pParent, eVkladki pVkladki, bool pClose = false)
        {
            UserTabVrladka _UserTabNods = new UserTabVrladka(PROP_ImageSource, PROP_Text, pParent, pVkladki, pClose, PROP_TextDown);
            return _UserTabNods;
        }

        /// <summary>МЕТОД Отображение шаблона</summary>
        /// <param name="pGrid">Сюда добавляем шабллон</param>
        /// <param name="pNewShablon">ture - Новый шаблон, false - Старый шаблон</param>
        /// <param name="pNomerShablon">Номер шаблона</param>
        /// <param name="pText">Наименование шаблона (по умолчанию pMyNodes.svoText)</param>
        public virtual bool MET_ShowShablon(Grid pGrid, bool pNewShablon, int pNomerShablon = 0, string pText = "")
        {
            if (pNewShablon)
            {
                // Создаем шаблон
                PROP_Docum.PROP_FormShablon = new UserFormShablon_Standart(PROP_Docum);
                PROP_Docum.PROP_FormShablon.MET_Inizial(this, true, pNomerShablon, pText);
            }
            else if (PROP_Docum.PROP_FormShablon == null)
            {
                PROP_Docum.PROP_FormShablon = new UserFormShablon_Standart(PROP_Docum);
                PROP_Docum.PROP_FormShablon.MET_Inizial(this, false, pNomerShablon, pText);
            }

            // Создаем контейнер и добавляем в него шаблон
            ScrollViewer _ScrollViewer = new ScrollViewer
            {
                Margin = new Thickness(0, 30, 0, 0),
                Content = PROP_Docum.PROP_FormShablon
            };
            // Добавляем, предварительно почистив, в таблицу контейнер с шаблоном
            if (pGrid.Children.Count > 1)
                pGrid.Children.RemoveAt(1);
            pGrid.Children.Add(_ScrollViewer);
            return true;
        }

        /// <summary>МЕТОД Признак удаления ветки (зачеркивание текста)</summary>
        /// <param name="pDelete">true - удалено, false - не удалено</param>
        public void MET_Delete(bool pDelete)
        {
            PROP_Delete = pDelete;
            // Если протокол удаленый, то помечаем его
            (Header as UserTabNod).MET_Delete(PROP_Delete);
            // Показываем ветку или скрываем, в зависимости от флага
            if (!PROP_Delete || MyGlo.ShowDeletedProtokol)
                Visibility = Visibility.Visible;
            else
                Visibility = Visibility.Collapsed;
        }
    }
}
