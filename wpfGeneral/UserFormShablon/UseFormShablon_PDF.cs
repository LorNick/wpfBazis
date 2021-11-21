using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using wpfGeneral.UserControls;
using wpfGeneral.UserNodes;
using wpfGeneral.UserPage;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfGeneral.UserFormShablon
{
    /// <summary>КЛАСС работы шаблона PDF</summary>
    class UseFormShablon_PDF : VirtualFormShablon
    {
        private UserPage_ShablonPDF PRI_Page = new UserPage_ShablonPDF();

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pDocument">Документ</param>
        public UseFormShablon_PDF(UserDocument pDocument) : base(pDocument) { }

        /// <summary>МЕТОД Инициализация Шаблона</summary>
        /// <param name="pNodes">Ветка</param>
        /// <param name="pNewProtokol">ture - Новый протокол, false - Старый протокол</param>
        /// <param name="pShablon">Номер шаблона, по умолчанию 0</param>
        /// <param name="pText">Наименование шаблона (по умолчанию pMyNodes.svoText)</param>
        public override VirtualFormShablon MET_Inizial(VirtualNodes pNodes, bool pNewProtokol, int pShablon = 0, string pText = "")
        {
            pNodes.PROP_shaButtonClearSha = Visibility.Collapsed;
            pNodes.PROP_shaButtonPrintSha = Visibility.Collapsed;
            base.MET_Inizial(pNodes, pNewProtokol, pShablon, pText);
            PROP_TipProtokol = new MyTipProtokol(PUB_VirtualNodes.PROP_shaTipProtokol.PROP_TipDocum);
            MET_CreateForm();
            return this;
        }

        /// <summary>МЕТОД Формируем форму Шаблона</summary>
        public override void MET_CreateForm()
        {
            if (PROP_Docum.PROP_ListShablon == null)
                PROP_Docum.PROP_ListShablon = UserListShablon.MET_FactoryListShablon(PROP_TipProtokol.PROP_TipDocum, PRO_NomerShablon);
            PUB_HashPole = new SortedList();                                // список полей             

            // Если открываем заполненый протокол
            if (!PROP_NewProtokol)
            {
                PRO_StrProtokol = PROP_Docum.PROP_Protokol.PROP_Protokol;
                PROP_Cod = PROP_Docum.PROP_Protokol.PROP_Cod;
                // Старый шаблон
                PROP_StadNodes = eStadNodes.Old;
            }
            else // Пытаемся найти последний протокол по KL и коду шаблона
            {
                 // Новый шаблон
                 PROP_StadNodes = eStadNodes.New;
            }
            // Создаем поля
            MET_LoadPole();           
            
            Frame _Frame = new Frame();
            PRI_Page.MET_Inizial(PUB_VirtualNodes, this, PROP_NewProtokol);            
            _Frame.Navigate(PRI_Page);
         
            this.Children.Add(_Frame);

            // Деактивируем кнопку "Сохранить"
            PROP_EditShablon = false;
            // Всё шаблон создан!
            PROP_Created = true;
        }

        /// <summary>МЕТОД Заполняем Поля</summary>
        protected override void MET_LoadPole()
        {
            // Наименование документа
            VirtualPole _Pole = MET_CreateUserPole(8);
            _Pole.PROP_FormShablon = this;
            _Pole.PROP_VarId = 1;
            // Ответ
            if (PROP_StadNodes == eStadNodes.Old)
                 _Pole.PROP_Text = MyMet.MET_GetPole(_Pole.PROP_VarId, PRO_StrProtokol);
            _Pole.Name = "elePoleShabl_" + _Pole.PROP_VarId; // имя поля
            PUB_HashPole.Add(_Pole.Name, _Pole); // записываем элемент в очередь

            // Имя файла
            _Pole = MET_CreateUserPole(8);
            _Pole.PROP_FormShablon = this;
            _Pole.PROP_VarId = 2;
            // Ответ
            if (PROP_StadNodes == eStadNodes.Old)
                _Pole.PROP_Text = MyMet.MET_GetPole(_Pole.PROP_VarId, PRO_StrProtokol);
            _Pole.Name = "elePoleShabl_" + _Pole.PROP_VarId; // имя поля
            PUB_HashPole.Add(_Pole.Name, _Pole); // записываем элемент в очередь       
        }

        /// <summary>МЕТОД Сохраняем данные</summary>
        public override bool MET_Save()
        { 
            // Сохраняем Pdf файл на сервер
            if (!PRI_Page.MET_Save())
                return false;

            // возвращаю фокус на место
            string _Rezult = "";                                                // результат ответа
            VirtualPole _Pole;                                                  // элементы поля
            DateTime _Date = PROP_NewProtokol ? DateTime.Today : PROP_Docum.PROP_Protokol.PROP_pDate; // дата создания протокола
            string _textDocum = "";
            // Формируем строку ответов
            foreach (DictionaryEntry _DiEnt in PUB_HashPole)
            {                
                _Pole = (VirtualPole)_DiEnt.Value;                              // наше поле с ответом
                if (_Pole.PROP_VarId > 0 && !string.IsNullOrWhiteSpace(_Pole.PROP_Text))          // если есть ответ, то вставляем его в строку ответов
                        _Rezult += "\\" + _Pole.PROP_VarId + "#" + _Pole.PROP_Text;

                if (_Pole.PROP_VarId == 1)
                    _textDocum = _Pole.PROP_Text; ;
            }

            // Если данные уже сохроняли, то просто обновляем, иначе добавляем
            string _StrSql;                                                     // строка SQL запроса
            int _User = MyGlo.User;
            string _UserName = MyGlo.UserName;
            if (PROP_Cod > 0)
            {
                // Если установлен админ - то обновляем протокол с пользователем, создавшем данный протокол
                if (MyGlo.Admin || MyGlo.FlagEdit)
                {
                    _User = PROP_Docum.PROP_Protokol.PROP_xUserUp;
                    _UserName = MyMet.MET_UserName(_User);
                }
                // Обновим логи
                PROP_Docum.PROP_Protokol.MET_ChangeLogs(MyGlo.User, "Изменён");
                _StrSql = MyQuery.MET_Protokol_Update_1(PROP_Cod, _Rezult, DateTime.Today, _User, _Date,
                    PROP_TipProtokol.PROP_Prefix, PROP_Docum.PROP_Protokol.PROP_xLog);
                // Обновим протокол в SQL
                MySql.MET_QueryNo(_StrSql);
                // Обновим Protokol
                PROP_Docum.PROP_Protokol.MET_Save(_Rezult, DateTime.Today, _User, _Date);                
            }
            else
            {  // Новый протокол
                string _xLog = UserLog.MET_CreateLogs();
                PROP_Cod = MySql.MET_GetNextRef(PROP_TipProtokol.PROP_NextRef);
                _StrSql = MyQuery.MET_Protokol_Insert_1(PROP_Cod, MyGlo.IND, PRO_NomerShablon, _Rezult,
                    MyGlo.KL, DateTime.Today, _User, PUB_VirtualNodes.PROP_shaIndex, _Date, PROP_TipProtokol.PROP_Prefix, _xLog);
                // Сохраним протокол в SQL
                MySql.MET_QueryNo(_StrSql);
                // Создадим Protokol
                PROP_Docum.PROP_Protokol = UserProtokol.MET_FactoryProtokol(PUB_VirtualNodes.PROP_shaTipProtokol.PROP_TipDocum, PROP_Cod);
            }
            if (!PUB_VirtualNodes.PROP_shaPresenceProtokol)
                PUB_VirtualNodes.PROP_shaPresenceProtokol = true;           // пометили, что сохранили шаблон
                                                                            // Дата протокола
            PUB_VirtualNodes.PROP_Data = _Date;
            // Нижний текст вкладок
            PUB_VirtualNodes.PROP_TextDown = _Date.ToString().Substring(0, 10) + " " + _UserName;
            // Наименование ветки
            PUB_VirtualNodes.PROP_Text = _textDocum;
            PUB_VirtualNodes.PROP_TextDefault = _textDocum;

            PROP_EditShablon = false;                                       // сохранили шаблон
            PUB_VirtualNodes.PROP_Docum.PROP_Otchet.PROP_NewCreate = true;  // отчет необходимо переформировать
            return true;
        }
    }
}