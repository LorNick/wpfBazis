using System.Collections.Generic;
using System.Windows.Media;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfGeneral.UserNodes
{
    /// <summary>КЛАСС Ветка RootsList (абстрактный)</summary>
    public abstract class VirtualNodes_RootsList : VirtualNodes
    {
        /// <summary>Тип ветки для ребенка</summary>
        protected eTipNodes PRO_TipNodeChild;

        /// <summary>СВОЙСТВО. Дополнительны Типы документов</summary>
        public int PROP_shaTipObsled { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Разрешение редактирования шаблона</summary>
        public override bool PROP_shaButtonEdit { get { return false; } }

        /// <summary>СВОЙСТВО (шаблона) Разрешение создания шаблона</summary>
        public override bool PROP_shaButtonNew { get; set; }

        /// <summary>Количество заполненных подветок</summary>
        private int PRI_CountNodesChild;

        /// <summary>СВОЙСТВО  (шаблона) Количество заполненных подветок</summary>
        public override int PROP_shaCountNodesChild
        {
            get { return PRI_CountNodesChild; }
            set
            {
                PRI_CountNodesChild = value;
                // Если нет заполенных подветок, то красим ветку в серый цвет
                if (PRI_CountNodesChild > 0)
                {
                    this.Foreground = Brushes.Black;
                    if (PROP_Parent != null) PROP_Parent.PROP_shaCountNodesChild++;

                    int _cou = 0;
                    foreach (VirtualNodes _It in Items)
                    {
                        if (!_It.PROP_Delete || MyGlo.ShowDeletedProtokol)
                            _cou++;
                    }
                    PROP_Text = $"{PROP_TextDefault} ({_cou})";
                   // PROP_Text = $"{PROP_TextDefault} ({PRI_CountNodesChild})";
                }
                else
                {
                    this.Foreground = Brushes.Gray;
                    PROP_Text = $"{PROP_TextDefault} (0)";
                }
            }
        }

        ///<summary>МЕТОД Инициализация ветки</summary>
        public override void MET_Inizial()
        {
            // Заполняем свойства базового класса
            base.MET_Inizial();

            // По умолчанию, в данной ветки можно создавать новые протоколы
            PROP_shaButtonNew = true;
            // По умолчанию ветка пустая
            PROP_shaCountNodesChild = 0;
            // Загружаем протоколы данной ветки
            var _Protokol = MET_LoadProtokol();
            foreach (var _pro in _Protokol)
            {
                // Создаем подветку
                UserNodes_Add _Node = MET_TypeNodesAdd();
                _Node.PROP_Docum = new UserDocument(_Node, _pro.PROP_TipProtokol.PROP_TipDocum);
                _Node.PROP_Docum.PROP_ListShablon = UserListShablon.MET_FactoryListShablon(_pro.PROP_TipProtokol.PROP_TipDocum, _pro.PROP_NumShablon);
                _Node.PROP_Docum.PROP_Protokol = _pro;
                _Node.PROP_Docum.PROP_Otchet = MET_OtchetChild(_Node.PROP_Docum);
                _Node.PROP_TipNodes = PRO_TipNodeChild;                             // тип
                _Node.Name = Name + "_Child" + _pro.PROP_pIndex;                    // имя
                _Node.PROP_ParentName = Name;                                       // имя родителя
                _Node.PROP_shaNomerShablon = _pro.PROP_NumShablon;                  // номер шаблона
                _Node.PROP_shaIndex = _pro.PROP_pIndex;                             // индекс
                _Node.PROP_ImageName = _Node.PROP_Docum.PROP_ListShablon.PROP_Icon; // иконка
                _Node.PROP_Text = MET_CreateTextNode(_Node.PROP_Docum);             // описание ветки
                _Node.PROP_TextDefault = _Node.PROP_Text;
                _Node.PROP_shaIND = _pro.PROP_CodApstac;
                _Node.PROP_Data = _pro.PROP_pDate;                                  // дата ветки
                _Node.PROP_shaTipProtokol = _pro.PROP_TipProtokol;                   // тип протокола
                _Node.PROP_TextDown = _pro.PROP_pDate.ToString().Substring(0, 10) + " " + _pro.PROP_UserName;    // нижний текст ветки
                _Node.MET_Inizial();
                // Если протокол удаленый, то помечаем его
                _Node.MET_Remote(_pro.PROP_xDelete == 1);
                _Node.PROP_shaPresenceProtokol = true;
                // Настраиваем дополнительные параметры для подветки
                MET_PropertyNodeAdd(_Node);
            }
        }

        /// <summary>МЕТОД Создание НОВОЙ подветки</summary>
        public virtual VirtualNodes MET_CreateNodesAdd()
        {
            // Создаем подветку
            UserNodes_Add _Node = MET_TypeNodesAdd();
            _Node.PROP_Docum = new UserDocument(_Node, PROP_Docum.PROP_Protokol.PROP_TipProtokol.PROP_TipDocum);
            _Node.PROP_Docum.PROP_ListShablon = PROP_Docum.PROP_ListShablon;
            _Node.PROP_Docum.PROP_FormShablon = PROP_Docum.PROP_FormShablon;
            _Node.PROP_Docum.PROP_Protokol = PROP_Docum.PROP_Protokol;
            _Node.PROP_Docum.PROP_Otchet = MET_OtchetChild(_Node.PROP_Docum);
            _Node.PROP_TipNodes = PRO_TipNodeChild;                                 // тип
            _Node.PROP_shaIndex = PROP_shaIndex;                                    // индекс
            _Node.Name = Name + "_Child" + _Node.PROP_shaIndex;                     // имя
            _Node.PROP_shaNomerShablon = _Node.PROP_Docum.PROP_Protokol.PROP_NumShablon;  // номер шаблона
            _Node.PROP_ImageName = _Node.PROP_Docum.PROP_ListShablon.PROP_Icon;     // иконка
            _Node.PROP_Text = MET_CreateTextNode(_Node.PROP_Docum);                 // описание ветки
            _Node.PROP_TextDefault = _Node.PROP_Text;
            _Node.PROP_shaIND = _Node.PROP_Docum.PROP_Protokol.PROP_CodApstac;
            _Node.PROP_Data = _Node.PROP_Docum.PROP_Protokol.PROP_pDate;            // дата ветки
            _Node.PROP_shaTipProtokol = _Node.PROP_Docum.PROP_Protokol.PROP_TipProtokol;  // тип протокола
            _Node.PROP_TextDown = _Node.PROP_Docum.PROP_Protokol.PROP_pDate.ToString().Substring(0, 10)
                + " " + _Node.PROP_Docum.PROP_Protokol.PROP_UserName;               // нижний текст ветки
            _Node.PROP_ParentName = Name;                                           // имя родителя
            _Node.MET_Inizial();
            // Снимаем выделения редактируемой ветки
            Background = Brushes.White;
            var _Otchet = PROP_Docum.PROP_Otchet;   // запоминаем отчет
            PROP_Docum = new UserDocument { PROP_Nodes = this };
            PROP_Docum.PROP_Otchet = _Otchet;
            PROP_shaNomerShablon = 0;
            // Настраиваем дополнительные параметры для подветки
            MET_PropertyNodeAdd(_Node);
            return _Node;
        }

        /// <summary>МЕТОД Создание Текста подветки (описание ветки)</summary>
        /// <remarks>По умолчанию берется из ListShablon.PROP_NameKr</remarks>
        protected virtual string MET_CreateTextNode(UserDocument userDocument)
        {
            return userDocument.PROP_ListShablon.PROP_NameKr;
        }

        /// <summary>МЕТОД Настраиваем дополнительные параметры для подветки</summary>
        protected virtual void MET_PropertyNodeAdd(VirtualNodes pNodes) { }

        ///<summary>МЕТОД Отчет подветки</summary>
        protected abstract VirtualOtchet MET_OtchetChild(UserDocument pDocument);

        ///<summary>МЕТОД Запрос на поиск подветок</summary>
        protected abstract IEnumerable<UserProtokol> MET_LoadProtokol();

        ///<summary>МЕТОД Тип подветки</summary>
        protected virtual UserNodes_Add MET_TypeNodesAdd()
        {
            return new UserNodes_Add();
        }
    }
}