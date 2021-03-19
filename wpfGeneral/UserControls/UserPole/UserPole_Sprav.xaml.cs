using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using wpfGeneral.UserWindows;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для поля Диагноз </summary>
    public partial class UserPole_Sprav
    {
        /// <summary>СВОЙСТВО Код</summary>
        public string PROP_Cod { get; set; }

        /// <summary>СВОЙСТВО Наименование</summary>
        public string PROP_TextDiag { get; set; }

        /// <summary>СВОЙСТВО Начальный фильтр для справочника (указывается через второй символ | в поле Name)</summary>
        public string PROP_FilterNach { get; set; }

        /// <summary>СВОЙСТВО Помечаем поле текста</summary>
        public override byte PROP_PometkaText
        {
            get { return PRO_PometkaText; }
            set
            {
                PRO_PometkaText = value;
                if (PRO_PometkaText == 1)
                    PART_TextDiag.BorderBrush = Brushes.Red;
                else
                    PART_TextDiag.ClearValue(Border.BorderBrushProperty);
            }
        }
       

        /// <summary>ДЕЛЕГАТ простой </summary>
        public delegate void DelegVoid();
        /// <summary>ДЕЛЕГАТ с bool</summary>
        public delegate bool DelegBool();

        /// <summary>МЕТОД Делегата Инициализации</summary>
        private DelegVoid PRI_DelegInicial;
        /// <summary>МЕТОД Делегата Нажатие на кнопку открытия справочника</summary>
        private DelegVoid PRI_DelegButtonSelect;
        /// <summary>МЕТОД Делегата Сохранение шаблона</summary>
        private DelegBool PRI_DelegSave;  

        /// <summary>Меняли ли код</summary>
        private bool PRI_FlagChange;
        // <summary>Создаем контекстное меню</summary>       
        private readonly ContextMenu PRI_ContextMenu = new ContextMenu();

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_Sprav()
        {
            InitializeComponent();

            // Инициализация конекстного меню
            PART_TextDiag.ContextMenu = MyGlo.ContextMenu;
            PART_TextDiag.Tag = this;
        }

        ///<summary>МЕТОД Инициализация поля</summary>
        ///<remarks>В поле шаблона "Name" прописываем тип справочника, а через символ '|' можем прописать подпись в шаблоне</remarks>
        public override void MET_Inicial()
        {
            // Разбиваем на тип справочника и подпись в шаблолне, через символ '|'
            var _Description = PROP_Description.Split('|');
            switch (_Description[0])
            {
                case "@apaDiag":
                    PART_Label.Content = _Description.Length > 1 ? _Description[1] : "Диагноз";
                    PROP_Description = _Description.Length > 1 ? _Description[1] : "Диагноз";
                    PRI_DelegInicial = MET_Inicial_ApaDiag;
                    PRI_DelegButtonSelect = MET_ButtonSelect_Diag;
                    PRI_DelegSave = MET_Save_ApaDiag;
                    break;
                case "@astDiag":
                    PART_Label.Content = _Description.Length > 1 ? _Description[1] : "Диагноз МКБ-10";
                    PROP_Description = _Description.Length > 1 ? _Description[1] : "Диагноз МКБ-10";
                    PRI_DelegInicial = MET_Inicial_AstDiag;
                    PRI_DelegButtonSelect = MET_ButtonSelect_Diag;
                    PRI_DelegSave = MET_Save_AstDiag;
                    break;
                case "@Diag":
                    PART_Label.Content = _Description.Length > 1 ? _Description[1] : "Диагноз";
                    PROP_Description = _Description.Length > 1 ? _Description[1] : "Диагноз";
                    PROP_FilterNach = _Description.Length > 2 ? _Description[2] : "";
                    PRI_DelegInicial = delegate { };
                    PRI_DelegButtonSelect = MET_ButtonSelect_Diag;
                    PRI_DelegSave = () => true;
                    break;
                case "@Otdel":
                    PART_Label.Content = _Description.Length > 1 ? _Description[1] : "Отделение стационара";
                    PROP_Description = _Description.Length > 1 ? _Description[1] : "Отделение стационара";
                    PRI_DelegInicial = delegate { };
                    PRI_DelegButtonSelect = MET_ButtonSelect_Otdel;
                    PRI_DelegSave = () => true;
                    break;
                case "@Department":
                    PART_Label.Content = _Description.Length > 1 ? _Description[1] : "Подразделение";
                    PROP_Description = _Description.Length > 1 ? _Description[1] : "Подразделение";
                    PRI_DelegInicial = delegate { };
                    PRI_DelegButtonSelect = MET_ButtonSelect_Department;
                    PRI_DelegSave = () => true;
                    break;
                case "@MetodVMP":
                    PART_Label.Content = _Description.Length > 1 ? _Description[1] : "Метод ВМП";
                    PROP_Description = _Description.Length > 1 ? _Description[1] : "Метод ВМП";
                    PRI_DelegInicial = delegate { };
                    PRI_DelegButtonSelect = MET_ButtonSelect_MetodVMP;
                    PRI_DelegSave = () => true;
                    break;
                case "@Oper":
                    PART_Label.Content = _Description.Length > 1 ? _Description[1] : "Услуги (операции)";
                    PROP_Description = _Description.Length > 1 ? _Description[1] : "Услуги (операции)";
                    PRI_DelegInicial = delegate { };
                    PRI_DelegButtonSelect = MET_ButtonSelect_Oper;
                    PRI_DelegSave = () => true;
                    break;
                case "@LecMNN":
                    PART_Label.Content = _Description.Length > 1 ? _Description[1] : "Наименование";
                    PROP_Description = _Description.Length > 1 ? _Description[1] : "Наименование";
                    PRI_DelegInicial = delegate { };
                    PRI_DelegButtonSelect = MET_ButtonSelect_LecMNN;
                    PRI_DelegSave = () => true;
                    break;
                case "@MorfTip":
                    PART_Label.Content = _Description.Length > 1 ? _Description[1] : "Морфологический код";
                    PROP_Description = _Description.Length > 1 ? _Description[1] : "Морфологический код";
                    PRI_DelegInicial = delegate { };
                    PRI_DelegButtonSelect = MET_ButtonSelect_MorfTip;
                    PRI_DelegSave = () => true;
                    break; 
            }
            PRI_DelegInicial();
        }

        /// <summary>СОБЫТИЕ Открыть, для выбора, таблицу справочников</summary>
        private void PART_ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            PRI_DelegButtonSelect();
        }
        
        ///<summary>МЕТОД Действие при сохранении</summary>
        public override bool MET_Save()
        {
            return PRI_DelegSave();
        }     

        #region ---- Диагноз  -----
        ///<summary>МЕТОД Инициализация поля Диагноза поликлиники</summary>
        private void MET_Inicial_ApaDiag()
        {
            PROP_Cod = Convert.ToString(MyGlo.HashAPAC["D"]);
            PROP_DefaultText = PROP_Cod == "" ? "" : PROP_Cod + " - " + MySql.MET_NameSpr(PROP_Cod, "s_Diag");
            if (PROP_Text == "")
            {
                PROP_Text = PROP_DefaultText;
            }  
        }

        ///<summary>МЕТОД Инициализация поля Диагноза стационара</summary>
        private void MET_Inicial_AstDiag()
        {
            PROP_Cod = Convert.ToString(MyGlo.HashAPSTAC["D"]);
            PROP_DefaultText = PROP_Cod == "" ? "" : PROP_Cod + " - " + MySql.MET_NameSpr(PROP_Cod, "s_Diag");
            if (PROP_Text == "")
            {
                PROP_Text = PROP_DefaultText;
            }
        }

        ///<summary>МЕТОД Нажали на кнопку выбора Диагноза</summary>
        private void MET_ButtonSelect_Diag()
        {
            // Справочник Диагнозов                   
            UserWindow_Diag _WinSpr = new UserWindow_Diag(PROP_FilterNach)
            {
                WindowStyle = WindowStyle.ToolWindow,
                PROP_Modal = true,
                // Разрешаем выбирать записи
                PROP_FlagButtonSelect = true,
            };
            _WinSpr.ShowDialog();
            if (_WinSpr.PROP_Return)
            {
                // Текст
                PROP_Text = _WinSpr.PROP_Cod + " - " + _WinSpr.PROP_Text;
                // Код диагноза
                PROP_Cod = _WinSpr.PROP_Cod;
                //  Поменяли диагноз
                PRI_FlagChange = true;
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

        ///<summary>МЕТОД Действие при сохранении Диагноза Поликлиники</summary>
        private bool MET_Save_ApaDiag()
        {
            // Если не меняли диагноз, то выходим 
            if (!PRI_FlagChange)
                return true;
            // Если меняли диагноз,  то сохраняем его в базу посещений/стац карту
            MySql.MET_QueryNo(MyQuery.MET_varString_Update_1("APAC", "D", PROP_Cod, "Cod", MyGlo.IND));
            MyGlo.HashAPAC["D"] = PROP_Cod;
            return true;
        }

        ///<summary>МЕТОД Действие при сохранении Диагноза виписки Стационара</summary>
        private bool MET_Save_AstDiag()
        {
            // Если не меняли диагноз, то выходим 
            if (!PRI_FlagChange)
                return true;
            // Если меняли диагноз,  то сохраняем его в базу стац карты
            MySql.MET_QueryNo(MyQuery.MET_varString_Update_1("APSTAC", "D", PROP_Cod, "IND", MyGlo.IND));
            MyGlo.HashAPSTAC["D"] = PROP_Cod;
            return true;
        }
        #endregion ----

        #region ---- Отделения стационара  -----
        ///<summary>МЕТОД Нажали на кнопку выбора Отделения стационара</summary>
        private void MET_ButtonSelect_Otdel()
        {
            // Справочник Отделений                   
            UserWindow_Department _WinSpr = new UserWindow_Department(" and Tip in (1, 2)")
            {
                WindowStyle = WindowStyle.ToolWindow,
                PROP_Modal = true,
                // Разрешаем выбирать записи
                PROP_FlagButtonSelect = true
            };
            _WinSpr.ShowDialog();
            if (_WinSpr.PROP_Return)
            {
                // Текст
                PROP_Text = _WinSpr.PROP_Cod + ". " + _WinSpr.PROP_Text;
                // Код отделения
                PROP_Cod = _WinSpr.PROP_Cod;
                //  Поменяли поле
                PRI_FlagChange = true;
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
        #endregion ----  

        #region ---- Подразделения  -----
        ///<summary>МЕТОД Нажали на кнопку выбора Подразделения</summary>
        private void MET_ButtonSelect_Department()
        {
            // Справочник Подразделений                   
            UserWindow_Department _WinSpr = new UserWindow_Department
            {
                WindowStyle = WindowStyle.ToolWindow,
                PROP_Modal = true,
                // Разрешаем выбирать записи
                PROP_FlagButtonSelect = true
            };
            _WinSpr.ShowDialog();
            if (_WinSpr.PROP_Return)
            {
                // Текст
                PROP_Text = _WinSpr.PROP_Cod + ". " + _WinSpr.PROP_Text;
                // Код отделения
                PROP_Cod = _WinSpr.PROP_Cod;
                //  Поменяли поле
                PRI_FlagChange = true;
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
        #endregion ----

        #region ---- Метод ВМП  -----
        ///<summary>МЕТОД Нажали на кнопку выбора Метод ВМП</summary>
        private void MET_ButtonSelect_MetodVMP()
        {
            string _Diag;                                                       // диагноз
            try
            {
                string _Name = PROP_Format.PROP_Value["VarId"].ToString();     // имя элемента
                _Diag = ((VirtualPole)this.PROP_Docum.PROP_FormShablon.PUB_HashPole["elePoleShabl_" + _Name]).PROP_Text;
            }
            catch
            {
                _Diag = "";
            }
            
            // Если диагноза нет, то выходим
            if (_Diag == "")
            {
                MessageBox.Show("Предварительно нужно выбрать диагноз!", "Ошибка!");
                return;
            }

            // Справочник Метода ВМП                  
            UserWindow_MetodVMP _WinSpr = new UserWindow_MetodVMP(_Diag)
            {
                WindowStyle = WindowStyle.ToolWindow,
                PROP_Modal = true,
                // Разрешаем выбирать записи
                PROP_FlagButtonSelect = true
            };
            _WinSpr.ShowDialog();
            if (_WinSpr.PROP_Return)
            {
                // Текст
                PROP_Text = _WinSpr.PROP_IDHM + ". " + _WinSpr.PROP_HMName;
                // Код метода ВМП
                PROP_Cod = _WinSpr.PROP_IDHM.ToString();
                //  Поменяли поле
                PRI_FlagChange = true;
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
        #endregion ----

        #region ---- Услуги (операции)  -----
        ///<summary>МЕТОД Нажали на кнопку выбора Услуги (операции)</summary>
        private void MET_ButtonSelect_Oper()
        {
            // Находим дату создания протокола из первого поля шаблона с pDate
            DateTime PRI_Date = DateTime.Parse(PROP_FormShablon.GetPole("DateOsmotr").PROP_Text);
            // Справочник Отделений                   
            UserWindow_Oper _WinSpr = new UserWindow_Oper(PRI_Date, PROP_Shablon)
            {
                WindowStyle = WindowStyle.ToolWindow,
                PROP_Modal = true,
                // Разрешаем выбирать записи
                PROP_FlagButtonSelect = true
            };
            _WinSpr.ShowDialog();
            if (_WinSpr.PROP_Return)
            {
                // Текст
                PROP_Text = _WinSpr.PROP_Cod + ". " + _WinSpr.PROP_Text;
                // Код услуги
                PROP_Cod = _WinSpr.PROP_Cod;
                //  Поменяли поле
                PRI_FlagChange = true;
                // Если есть шаблон
                if ((bool) this.PROP_FormShablon?.PROP_Created)
                {
                    // Активируем кнопку "Сохранить"
                    MyGlo.callbackEvent_sEditShablon?.Invoke(true);
                    // Запускаем Lua фунцкию, на изменение записи
                    this.PROP_Lua?.MET_OnChange();
                }
            }
        }
        #endregion ---- 

        #region ---- МНН Лекарственных препаратов  -----
        ///<summary>МЕТОД Нажали на кнопку выбора МНН Лекарственных препаратов</summary>
        private void MET_ButtonSelect_LecMNN()
        {
            // Справочник МНН препаратов                   
            UserWindow_LecMNN _WinSpr = new UserWindow_LecMNN
            {
                WindowStyle = WindowStyle.ToolWindow,
                PROP_Modal = true,
                // Разрешаем выбирать записи
                PROP_FlagButtonSelect = true
            };
            _WinSpr.ShowDialog();
            if (_WinSpr.PROP_Return)
            {
                // Текст
                PROP_Text = _WinSpr.PROP_Value;
               
                //  Поменяли поле
                PRI_FlagChange = true;
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
        #endregion ----

        #region ---- Морфологический тип  -----
        ///<summary>МЕТОД Нажали на кнопку выбора Морфологический тип</summary>
        private void MET_ButtonSelect_MorfTip()
        {
            // Справочник Морфологический тип                   
            UserWindow_MorfTip _WinSpr = new UserWindow_MorfTip
            {
                WindowStyle = WindowStyle.ToolWindow,
                PROP_Modal = true,
                // Разрешаем выбирать записи
                PROP_FlagButtonSelect = true
            };
            _WinSpr.ShowDialog();
            if (_WinSpr.PROP_Return)
            {
                // Текст
                PROP_Text = _WinSpr.PROP_Text;

                //  Поменяли поле
                PRI_FlagChange = true;
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
        #endregion ----
    }
}
