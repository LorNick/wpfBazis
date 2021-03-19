using System.Collections;
using System.Windows.Controls;
using System.Windows.Media;
using wpfGeneral.UserNodes;
using wpfGeneral.UserControls;
using wpfGeneral.UserStruct;
using wpfStatic;
using System.Windows.Input;
using System.Windows;

namespace wpfGeneral.UserFormShablon
{
    /// <summary>КЛАСС виртуальная форма шаблона </summary>
    /// <remarks> Описание работы, создания шаблона
    /// Три типа документа:
    /// 1. Новый документ, единственный в ветке
    /// 2. Редактируем любой документ
    /// 3. Создаем новый документ в подветке
    /// 
    /// - Помечаем ветку, зеленым цветом, сообщая, что мы её редактируем;
    /// - Создаем вкладку;
    /// - Привязываем вкладку к закладке;
    /// - Если (тип документа 3), то создаем копию документа;
    /// - Если у документа есть созданный шаблон, то привязываем его к закладке;
    /// - Если у документа нет шаблона, то создаем его;
    /// - Привязываем шаблон к закладке шаблона;
    /// - Отображаем и переключаемся на закладку шаблона;
    /// - При сохранении если (тип документа 3), то:
    ///    - убираем пометку, что редактируем ветку,
    ///    - создаем новую ветку, 
    ///    - переключаемся на неё, 
    ///    - помечаем, что её редактируем,
    ///    - привязываем её к документу;
    /// - При сохранении, редактируем данные вкладки; 
    /// - При закрытии спрашиваем о сохранении и убираем пометку, что редактируем ветку
    ///</remarks>
    public abstract class VirtualFormShablon : Grid 
    {
        /// <summary>Список полей</summary>
        public SortedList PUB_HashPole;

        /// <summary>Ветка</summary>
        public VirtualNodes PUB_VirtualNodes;

        /// <summary>Наименование шаблона</summary>
        public string PUB_Text;

        /// <summary>Строка протокола</summary>
        protected string PRO_StrProtokol;

        /// <summary>Редактировали ли шаблон? (true - да, false -нет)</summary>
        protected bool PRO_EditShablon;

        /// <summary>Наименование таблицы шаблона</summary>
        protected string PRO_TabShablon;

        /// <summary>Наименование таблицы протокола</summary>
        protected string PRO_TabProtokol;

        /// <summary>Номер шаблона</summary>
        protected int PRO_NomerShablon;


        #region ---- Свойства ----
        /// <summary>СВОЙСТВО Новый или старый шаблон</summary>
        public bool PROP_Now { get; set; }

        /// <summary>СВОЙСТВО Процесс оздания шаблона (true - создан, false - в процессе создания)</summary>
        public bool PROP_Created { get; set; }

        /// <summary>СВОЙСТВО Состояние шаблона</summary>
        public eStadNodes PROP_StadNodes { get; set; }

        /// <summary>СВОЙСТВО Редактировали ли шаблон?</summary>
        /// <value>Работает с переменной PRO_EditShablon</value>
        /// <remarks>Отображает, либо скрываем кнопку "Сохранить"</remarks>
        public bool PROP_EditShablon
        {
            get { return PRO_EditShablon; }
            set
            {
                PRO_EditShablon = value;
                // Отображаем, либо скрываем кнопку "Сохранить"
                MyGlo.callbackEvent_sEditShablon(PRO_EditShablon);
            }
        }

        /// <summary>СВОЙСТВО Код протокола</summary>
        public int PROP_Cod { get; set; }

        /// <summary>СВОЙСТВО Форматирование шаблона</summary>
        /// <remarks>Берем из поля xFormat таблицы ListShablon</remarks>
        public MyFormat PROP_Format { get; set; }

        /// <summary>СВОЙСТВО Сылка на родительский документ</summary>
        public UserDocument PROP_Docum { get; set; }

        /// <summary>СВОЙСТВО Тип протокола</summary>
        public MyTipProtokol PROP_TipProtokol { get; set; }  
        #endregion


        /// <summary>КОНСТРУКТОР (пустой)</summary>
        protected VirtualFormShablon() { }

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pDocument">Документ</param>
        protected VirtualFormShablon(UserDocument pDocument)
        {
            PROP_Docum = pDocument;
            PROP_Docum.PROP_FormShablon = this;
        }

        /// <summary>МЕТОД Инициализация Шаблона</summary>
        /// <param name="pNodes">Ветка</param>
        /// <param name="pNew">ture - Новый шаблон, false - Старый шаблон</param>
        /// <param name="pShablon">Номер шаблона, по умолчанию 0</param>
        /// <param name="pText">Наименование шаблона (по умолчанию pMyNodes.svoText)</param>
        public virtual VirtualFormShablon MET_Inizial(VirtualNodes pNodes, bool pNew, int pShablon = 0, string pText = "")
        {
            PUB_VirtualNodes = pNodes;                                          // ветка
            PUB_VirtualNodes.Background = Brushes.LightGreen;                   // показываем, что данную ветку редактируем
            PROP_Now = pNew;                                                    // новый или старый шаблон
            PROP_EditShablon = false;                                           // вначале сохранять нечего
            PRO_NomerShablon = pShablon != 0 ? pShablon : PUB_VirtualNodes.PROP_shaNomerShablon;
            PUB_Text = pText != "" ? pText : PUB_VirtualNodes.PROP_Text;
            return this;
        }

        /// <summary>МЕТОД Формируем форму Шаблона (пустой)</summary>
        public virtual void MET_CreateForm() { }

        /// <summary>МЕТОД Заполняем Поля (пустое)</summary>
        protected virtual void MET_LoadPole() { }

        /// <summary>МЕТОД Создаем объект Pole</summary>
        /// <param name="pTypePole">Номер типа поля eTypePole</param>
        protected VirtualPole MET_CreateUserPole(int pTypePole)
        {
            VirtualPole _Pole;
            switch (pTypePole)
            {
                case 1:
                    _Pole = new UserPole_Number();
                    break;
                case 2:
                    _Pole = new UserPole_Text();
                    break;
                case 3:
                    _Pole = new UserPole_Data();
                    break;
                case 4:
                    _Pole = new UserPole_Text();
                    break;
                case 5:
                    _Pole = new UserPole_ComboBox(); // список                   
                    break;
                case 6:
                    _Pole = new UserPole_Text();
                    break;
                case 7:
                    _Pole = new UserPole_MultyList(); // пока только операции
                    break;
                case 8:
                    _Pole = new UserPole_Text();
                    break;
                case 9:
                    _Pole = new UserPole_Razdel();
                    break;
                case 10:
                    _Pole = new UserPole_Text();
                    break;
                case 11:
                    _Pole = new UserPole_Text();
                    break;
                case 12:
                    _Pole = new UserPole_RadioButton();
                    break;
                case 13:
                    _Pole = new UserPole_Image();
                    break;
                case 14:
                    _Pole = new UserPole_Grid();
                    break;
                case 15:
                    _Pole = new UserPole_Label();
                    break;
                case 16:
                    _Pole = new UserPole_Sprav();       // справочники
                    break;
                case 17:
                    _Pole = new UserPole_Calendar();
                    break;
                default:
                    _Pole = new UserPole_Text();
                    break;
            }
            // Проставляем тип поля
            _Pole.PROP_Type = (eVopros) pTypePole;
            _Pole.PROP_Docum = PROP_Docum;
            return _Pole;
        }

        /// <summary>МЕТОД Список переменных (пустой)</summary>
        /// <param name="pString">Строка с текстом поля значения по умолчанию ValueStart</param>
        /// <param name="pNomShabl">Номер шаблона</param>
        /// <param name="pVarID">Номер конкретной ссылки на вопрос (по умолчанию не используем)</param>
        protected virtual string MET_ReplaceProp(string pString, int pNomShabl, int pVarID = 0)  { return pString; }

        /// <summary>МЕТОД Проверка перед сохранением</summary>
        public virtual bool MET_Verification()
        {
            // Проверяем ответы на правильность заполнений
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (DictionaryEntry _DiEnt in PUB_HashPole)
            {
                VirtualPole _Pole = (VirtualPole) _DiEnt.Value;                 // наше поле с ответом  
                if (!_Pole.MET_Verification())                                  // сообщение об ошибки выдает само поле
                    return false;
            }
            return true;
        }

        /// <summary>МЕТОД Сохраняем данные (пустой)</summary>
        public virtual bool MET_Save()  { return true; }

        /// <summary>МЕТОД Отчищаем шаблон</summary>
        public virtual void MET_Clear()
        {
            foreach (DictionaryEntry _DictEn in PUB_HashPole)
            {
                try
                {
                    VirtualPole _Pole = (VirtualPole) _DictEn.Value;            // элемент поля                                                       
                    _Pole.PROP_Text = _Pole.PROP_DefaultText;                   // Значение по умолчанию                                         
                    _Pole.PROP_ForegroundText = Brushes.Gray;                   // Окрашиваем текст в серый цвет (текст по умолчанию)
                }
                catch
                {
                }
            }
        }

        /// <summary>МЕТОД Возвращает поле по коду VarId</summary>
        /// <param name="pVarId">Код VarId данного поля</param>
        public VirtualPole GetPole(int pVarId)
        {
            string _NamePole = "elePoleShabl_" + pVarId;
            return GetPole(_NamePole);
        }

        /// <summary>МЕТОД Возвращает поле по имени поля</summary>
        /// <param name="pNamePole">Имя поля (например DateOsmotr)</param>
        public VirtualPole GetPole(string pNamePole)
        {
            if (PUB_HashPole.ContainsKey(pNamePole))
            {
                return (VirtualPole)PUB_HashPole[pNamePole];
            }
            return null;
        }

        /// <summary>МЕТОД Возвращаем поле на котором стоит фокус</summary>
        public VirtualPole GetFocusedPole()
        {
            IInputElement _focusedElement = Keyboard.FocusedElement as UIElement;
            if (((FrameworkElement)_focusedElement).Tag is VirtualPole)
                return (VirtualPole)((FrameworkElement)_focusedElement).Tag;
            else
                return null;
        }
    }
}