﻿using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using wpfGeneral.UserControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using wpfGeneral.UserNodes;
using wpfGeneral.UserStruct;
using wpfGeneral.UserVariable;
using wpfStatic;
using System.Data;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для вывода Json формы</summary>
    public class UserJson_Form : Grid
    {
        /// <summary>Панель, куда добавляем поля</summary>
        private readonly ScrollViewer PRI_ScrollViewer = new ScrollViewer();

        /// <summary>Панель, куда добавляем поля</summary>
        private readonly StackPanel PRI_StackPanel = new StackPanel();

        #region ---- СВОЙСТВО ----
        /// <summary>СВОЙСТВО Ориентация панели (вертикальная, горизонтальная)</summary>
        public Orientation PROP_Orientation
        {
            get { return PRI_StackPanel.Orientation; }
            set { PRI_StackPanel.Orientation = value; }
        }

        /// <summary>СВОЙСТВО Описание формы</summary>
        public string PROP_Description { get; set; }

        /// <summary>СВОЙСТВО Текст Json</summary>
        public string PROP_TextJson { get; set; }

        /// <summary>СВОЙСТВО Объект Json</summary>
        public JObject PROP_Json { get; set; }

        /// <summary>СВОЙСТВО Имя используемой таблицы (поле Tables)</summary>
        public string PROP_Tables { get; set; }

        /// <summary>СВОЙСТВО Дополнительный фильтр (поле xInfo, тег Dop_Filter)</summary>
        public string PROP_Dop_Filter { get; set; }

        private EnumerableRowCollection<DataRow> PRI_Rows { get; set; }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserJson_Form()
        {
            PROP_Tables = "kbolInfo";
            PROP_Dop_Filter = "pol";
            PROP_Orientation = Orientation.Vertical;
            Children.Add(PRI_ScrollViewer);
            PRI_ScrollViewer.Content = PRI_StackPanel;
            PROP_TextJson = @"{
                  ""DS1_T"": 3,
                  ""Stadia"": ""Ic"",
                  ""T"": ""1c"",
                  ""N"": ""0"",
                  ""M"": ""0"",
                  ""Klin_gr"": ""II"",
                  ""Cel"": 3.0
                }";
        }



        /// <summary>МЕТОД Инициализация Шаблона</summary>
        public UserJson_Form MET_Inizial()
        {
            MET_CreateTegs();

            // Проверяем json строку на правильность
            if (string.IsNullOrEmpty(PROP_TextJson))
            {
                //
            }
            else
            {
                MET_CreateForm();
            }
            return this;
        }

        /// <summary>МЕТОД Формируем конструктор тегов</summary>
        public void MET_CreateTegs()
        {
            // Загружаем наши теги
            MySql.MET_DsAdapterFill(MyQuery.MET_s_Tags_Select_1(PROP_Tables, PROP_Dop_Filter), "s_Tegs");
            DataTable _TableSQL = MyGlo.DataSet.Tables["s_Tegs"];
            PRI_Rows = MyGlo.DataSet.Tables["s_Tegs"].AsEnumerable();
            foreach (var _Row in PRI_Rows)
            {
                var _PoleTeg = new UserJson_Menu();
                _PoleTeg.PROP_Tag = Convert.ToString(_Row["Tag"]);
                _PoleTeg.PROP_TagName = Convert.ToString(_Row["TagName"]);
                _PoleTeg.ToolTip = Convert.ToString(_Row["Discription"]);

               // PRI_StackPanel.Children.Add(_PoleTeg);
            }

        }

        /// <summary>МЕТОД Формируем форму Шаблона</summary>
        public void MET_CreateForm()
        {
            JObject _JsonSL = JObject.Parse(PROP_TextJson);

            foreach (var _Teg in _JsonSL)
            {
                var _PoleTeg = new UserJson_Text();

                _PoleTeg.PROP_Tag = _Teg.Key;
                _PoleTeg.PROP_TagName = _Teg.Key;
                _PoleTeg.PROP_Value = Convert.ToString(_Teg.Value);

                // Находим тег в таблице s_Tags
                var _Tag = PRI_Rows.Where(myRow => myRow.Field<string>("Tag") == _PoleTeg.PROP_Tag);
                PRI_StackPanel.Children.Add(_PoleTeg);
            }

            //var _p1 = new UserJson_Text();
            //_p1.PROP_Description = "asdf";
            //PRI_StackPanel.Children.Add(_p1);
            //var _p2 = new UserJson_Text();
            //PRI_StackPanel.Children.Add(_p2);
            //var _p3 = new UserJson_Text();
            //PRI_StackPanel.Children.Add(_p3);
            //var _p4 = new UserJson_Text();
            //PRI_StackPanel.Children.Add(_p4);

            //Children.Add(PRI_StackPanel);


            //if (PROP_Docum.PROP_ListShablon == null)
            //    PROP_Docum.PROP_ListShablon = UserListShablon.MET_FactoryListShablon(PROP_TipProtokol.PROP_TipDocum, PRO_NomerShablon);

            //// Исключение при выполнении запроса к базе
            //try
            //{
            //    PUB_HashPole = new SortedList();                                // список полей
            //    Background = Brushes.GhostWhite;
            //    // Если открываем заполненый протокол
            //    if (!PROP_Now)
            //    {
            //        PRO_StrProtokol = PROP_Docum.PROP_Protokol.PROP_Protokol;
            //        PROP_Cod = PROP_Docum.PROP_Protokol.PROP_Cod;
            //        // Старый шаблон
            //        PROP_StadNodes = eStadNodes.Old;
            //    }
            //    else // Пытаемся найти последний протокол по KL и коду шаблона
            //    {
            //        if (MySql.MET_DsAdapterFill(MyQuery.MET_Protokol_Select_8(MyGlo.KL, PRO_NomerShablon, PROP_TipProtokol.PROP_Prefix), PROP_TipProtokol.PROP_Protokol) > 0)
            //        {
            //            // Новый шаблон, с предыдушими данными
            //            PROP_StadNodes = eStadNodes.NewPerw;
            //            PRO_StrProtokol = MyGlo.DataSet.Tables[PROP_TipProtokol.PROP_Protokol].Rows[0]["Protokol"].ToString();
            //        }
            //        else
            //            // Новый шаблон
            //            PROP_StadNodes = eStadNodes.New;
            //    }
            //    // ---- Дата осмотра
            //    PROP_Format = new MyFormat(PROP_Docum.PROP_ListShablon.PROP_xFormat);
            //    VirtualPole _Date = MET_CreateUserPole(3);
            //    _Date.PROP_Necessarily = true;
            //    _Date.PROP_Description = PROP_Format.MET_If("pDateT") ? PROP_Format.PROP_Value["pDateT"].ToString() : "Дата";
            //    // Если старый шаблон
            //    if (!PROP_Now)
            //        _Date.PROP_Text = PROP_Docum.PROP_Protokol.PROP_pDate.ToString();
            //    else
            //    {
            //        // Если новый шаблон
            //        if (PROP_Format.MET_If("pDateD"))
            //        {
            //            switch (PROP_Format.PROP_Value["pDateD"].ToString())
            //            {
            //                case "pol":                                                         // если посещение поликлиники
            //                    _Date.PROP_Text = MyMet.MET_ParseDat(MyGlo.HashAPAC["DP"]).ToString();
            //                    break;
            //                default:
            //                    _Date.PROP_Text = DateTime.Today.ToString();                    // текущий день
            //                    break;
            //            }
            //        }
            //        else
            //            _Date.PROP_Text = DateTime.Today.ToString();                            // текущий день
            //    }
            //    _Date.PROP_DefaultText = DateTime.Today.ToString();             // начальное значение
            //    _Date.PROP_ValueMin = Convert.ToDateTime("01/01/2011");         // минимальное число - 2011 год
            //    _Date.Name = "DateOsmotr";                                      // имя поля
            //    _Date.PROP_FormShablon = this;                                  // ссылка на форму
            //    PUB_HashPole.Add(_Date.Name, _Date);                            // записываем элемент в очередь
            //    PRI_StackPanel.Children.Add(_Date);                             // добавляем элемент на форму
            //    // Перебераем все вопросы
            //    MET_LoadPole();
            //    // Деактивируем кнопку "Сохранить"
            //    PROP_EditShablon = false;
            //    // Всё шаблон создан!
            //    PROP_Created = true;
            //}
            //catch
            //{
            //    // ignored
            //}
        }

        ///// <summary>МЕТОД Создаем объект Pole</summary>
        ///// <param name="pTypePole">Номер типа поля eTypePole</param>
        //protected VirtualPole MET_CreateUserPole(int pTypePole)
        //{
            //VirtualPole _Pole;
            //switch (pTypePole)
            //{
            //    case 1:
            //        _Pole = new UserPole_Number();
            //        break;
            //    case 2:
            //        _Pole = new UserPole_Text();
            //        break;
            //    case 3:
            //        _Pole = new UserPole_Data();
            //        break;
            //    case 4:
            //        _Pole = new UserPole_Text();
            //        break;
            //    case 5:
            //        _Pole = new UserPole_ComboBox(); // список
            //        break;
            //    case 6:
            //        _Pole = new UserPole_Text();
            //        break;
            //    case 7:
            //        _Pole = new UserPole_MultyList(); // пока только операции
            //        break;
            //    case 8:
            //        _Pole = new UserPole_Text();
            //        break;
            //    case 9:
            //        _Pole = new UserPole_Razdel();
            //        break;
            //    case 10:
            //        _Pole = new UserPole_Text();
            //        break;
            //    case 11:
            //        _Pole = new UserPole_Text();
            //        break;
            //    case 12:
            //        _Pole = new UserPole_RadioButton();
            //        break;
            //    case 13:
            //        _Pole = new UserPole_Image();
            //        break;
            //    case 14:
            //        _Pole = new UserPole_Grid();
            //        break;
            //    case 15:
            //        _Pole = new UserPole_Label();
            //        break;
            //    case 16:
            //        _Pole = new UserPole_Sprav();       // справочники
            //        break;
            //    case 17:
            //        _Pole = new UserPole_Calendar();
            //        break;
            //    default:
            //        _Pole = new UserPole_Text();
            //        break;
            //}
            //// Проставляем тип поля
            //_Pole.PROP_Type = (eVopros)pTypePole;
            //_Pole.PROP_Docum = PROP_Docum;
            //return _Pole;
        //}

        /// <summary>МЕТОД Заполняем Поля</summary>
        protected void MET_LoadPole()
        {
            //// Пробегаемся по вопросам шаблона
            //foreach (UserShablon _Shablon in PROP_Docum.PROP_Shablon)
            //{
            //    try
            //    {
            //        MyFormat _Format = new MyFormat(_Shablon.PROP_xFormat); // формат
            //        // Если поле нужно проппустить, то следующее поле
            //        if (_Format.MET_If("hide") || _Format.MET_If("old"))
            //            continue;
            //        VirtualPole _Pole = MET_CreateUserPole(_Shablon.PROP_Type); // создаем поле, соответствующего типа
            //        _Pole.PROP_FormShablon = this; // ссылка на форму
            //        _Pole.PROP_Shablon = _Shablon.PROP_ID; // номер шаблона, как минимум нужен для Картинок
            //        _Pole.PROP_Description = _Shablon.PROP_Name; // вопрос
            //        _Pole.PROP_VarId = _Shablon.PROP_VarId; // номер индификатора VarId
            //        _Pole.PROP_Format = _Format; // формат поля
            //        _Pole.PROP_Necessarily = _Format.MET_If("nec"); // обязательное поле
            //        _Pole.IsEnabled = !_Format.MET_If("disab"); // запрет на редактирование
            //        _Pole.PROP_DefaultText = MET_ReplaceProp(_Shablon.PROP_ValueStart, PRO_NomerShablon,
            //            _Pole.PROP_VarId); // Значение ответа по умолчанию
            //        // Ответ
            //        if (PROP_StadNodes == eStadNodes.New ||
            //            (PROP_StadNodes == eStadNodes.NewPerw && _Format.MET_If("nprev")))
            //            // Значения по умолчанию
            //            _Pole.PROP_Text = _Pole.PROP_DefaultText;
            //        else
            //        // Значения из протокола (либо eStadNodes.Old, либо eStadNodes.NewPerw)
            //            _Pole.PROP_Text = MyMet.MET_GetPole(_Pole.PROP_VarId, PRO_StrProtokol);
            //        // Если новый шаблон, то текст серый (значение по умолчанию)
            //        if (PROP_Now)
            //            _Pole.PROP_ForegroundText = Brushes.Gray;
            //        _Pole.Name = "elePoleShabl_" + _Pole.PROP_VarId; // имя поля
            //        // Lua
            //        if (!String.IsNullOrEmpty(_Shablon.PROP_xLua))
            //        {
            //            _Pole.PROP_Lua = new UserLua_Standart(_Pole)
            //            {
            //                PROP_ChankText = _Shablon.PROP_xLua
            //            };
            //            _Pole.PROP_Lua.MET_StartLua();
            //        }
            //        PUB_HashPole.Add(_Pole.Name, _Pole); // записываем элемент в очередь
            //        // Указатель на принадлежность к разделу
            //        string _Maska = _Shablon.PROP_Maska;
            //        // Добавляем элемент в ...
            //        if (_Maska == "")
            //            PRI_StackPanel.Children.Add(_Pole); // добавляем элемент на форму
            //        else
            //            ((VirtualPole) PUB_HashPole["elePoleShabl_" + _Maska]).MET_AddElement(_Pole);
            //                // добавляем элемент в родительское поле
            //        _Pole.MET_Inicial(); // инициализация поля (если есть)
            //    }
            //    catch
            //    {
            //        // ignored
            //    }
            //}
            //Children.Add(PRI_StackPanel);
            //// Ещё раз пробегаем по полям и запускаем Lua код
            //// ReSharper disable once LoopCanBePartlyConvertedToQuery
            //foreach (DictionaryEntry _DiEnt in PUB_HashPole)
            //{
            //    var _PoleLua = (VirtualPole) _DiEnt.Value;
            //    _PoleLua?.PROP_Lua?.MET_OnCreat();
            //}
        }

        /// <summary>МЕТОД Список переменных</summary>
        /// <param name="pString">Строка с текстом поля значения по умолчанию ValueStart</param>
        /// <param name="pNomShabl">Номер шаблона</param>
        /// <param name="pVarID">Номер конкретной ссылки на вопрос (по умолчанию не используем)</param>
        protected string MET_ReplaceProp(string pString, int pNomShabl, int pVarID = 0)
        {
            //// Есть ли вообще ключевые поля в строке
            //if (pString.IndexOf('[') > -1)
            //{
            //    UserVariable_Standart _Var = new UserVariable_Standart { PROP_Protokol = PROP_Docum.PROP_Protokol };
            //    pString = _Var.MET_ReplacePole(pString, pNomShabl, PROP_TipProtokol.PROP_Prefix, pVarID);
            //}
            return pString;
        }

        /// <summary>МЕТОД Сохраняем данные</summary>
        public bool MET_Save()
        {
   //         // Срабатываю фокус поля, что бы выполнилась Lua функция OnChange
   //         UIElement _ElementWithFocus = Keyboard.FocusedElement as UIElement;                      // элемент фокуса
   //         if (_ElementWithFocus?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)) ?? false)      // сдвигаю фокус на следующее поле
   //             _ElementWithFocus.Focus();
   //         // возвращаю фокус на место
   //         string _Rezult = "";                                                // результат ответа
         //   VirtualPole _Pole;                                                  // элементы поля
            //DateTime _Date = DateTime.Today;                                    // дата осмотра
            //// Формируем строку ответов
            //foreach (DictionaryEntry _DiEnt in PUB_HashPole)
            //{
            //    _Pole = (VirtualPole)_DiEnt.Value;                              // наше поле с ответом
            //    if (_Pole.PROP_Type == eVopros.Label) continue;                 // не берем поля с меткой
   //             if (_Pole.PROP_VarId > 0 && !string.IsNullOrWhiteSpace(_Pole.PROP_Text))          // если есть ответ, то вставляем его в строку ответов
            //        _Rezult += "\\" + _Pole.PROP_VarId + "#" + _Pole.PROP_Text;
            //    if (_Pole.Name == "DateOsmotr")
            //        _Date = Convert.ToDateTime(_Pole.PROP_Text);
   //             // Запускаем Lua функцию, перед сохранием, если есть.
   //             if(!(_Pole.PROP_Lua?.MET_OnBeforeSave() ?? true))
   //             {
   //                 return false;
   //             }
   //         }
            //// Сохраняем ответы
            //if (_Rezult.Length > 0)
            //{
            //    // Если данные уже сохроняли, то просто обновляем, иначе добавляем
            //    string _StrSql;                                                     // строка SQL запроса
   //             int _User = MyGlo.User;
            //    string _UserName = MyGlo.UserName;
   //             if (MySql.MET_QueryBool(MyQuery.MET_Protokol_Select_7(MyGlo.KL, MyGlo.IND, PRO_NomerShablon, PUB_VirtualNodes.PROP_shaIndex, PROP_TipProtokol.PROP_Prefix)))
            //    {
   //                 // Если установлен админ - то обновляем протокол с пользователем, создавшем данный протокол
            //        if (MyGlo.Admin || MyGlo.FlagEdit)
            //        {
            //            _User = PROP_Docum.PROP_Protokol.PROP_xUserUp;
   //                     _UserName = MyMet.MET_UserName(_User);
   //                 }
   //                 // Обновим логи
   //                 PROP_Docum.PROP_Protokol.MET_ChangeLogs(MyGlo.User, "Изменён");

   //                 _StrSql = MyQuery.MET_Protokol_Update_1(_Rezult, DateTime.Today, _User, MyGlo.KL, MyGlo.IND, PRO_NomerShablon,
   //                     PUB_VirtualNodes.PROP_shaIndex, _Date, PROP_TipProtokol.PROP_Prefix, PROP_Docum.PROP_Protokol.PROP_xLog);
   //                // Обновим протокол в SQL
   //                MySql.MET_QueryNo(_StrSql);
   //                 // Обновим Protokol
   //                 PROP_Docum.PROP_Protokol.MET_Save(_Rezult, DateTime.Today, _User, _Date);
            //    }
            //    else
   //             {
   //                 string _xLog = UserProtokol.MET_CreateLogs(MyGlo.User, "Создан");
   //                 PROP_Cod = MySql.MET_GetNextRef(PROP_TipProtokol.PROP_NextRef);
            //        _StrSql = MyQuery.MET_Protokol_Insert_1(PROP_Cod, MyGlo.IND, PRO_NomerShablon, _Rezult,
   //                     MyGlo.KL, DateTime.Today, _User, PUB_VirtualNodes.PROP_shaIndex, _Date, PROP_TipProtokol.PROP_Prefix, _xLog);
   //                 // Сохраним протокол в SQL
   //                 MySql.MET_QueryNo(_StrSql);
   //                 // Создадим Protokol
   //                 PROP_Docum.PROP_Protokol = UserProtokol.MET_FactoryProtokol(PUB_VirtualNodes.PROP_shaTipProtokol.PROP_TipDocum, PROP_Cod);
            //    }
            //    if (!PUB_VirtualNodes.PROP_shaPresenceProtokol)
            //        PUB_VirtualNodes.PROP_shaPresenceProtokol = true;           // пометили, что сохранили шаблон
            //    // Дата протокола
            //    PUB_VirtualNodes.PROP_Data = _Date;
            //    // Нижний текст вкладок
            //    PUB_VirtualNodes.PROP_TextDown = _Date.ToString().Substring(0, 10) + " " + _UserName;
            //    // Только для добавляемых веток
            //    PUB_VirtualNodes.PROP_Text = PUB_VirtualNodes.PROP_TextDefault;
            //    PROP_EditShablon = false;                                       // сохранили шаблон
            //    PUB_VirtualNodes.PROP_Docum.PROP_Otchet.PROP_NewCreate = true;  // отчет необходимо переформировать
            //    // Выполняем сохранение определенных полей
            //    foreach (DictionaryEntry _DiEnt in PUB_HashPole)
            //    {
            //        _Pole = (VirtualPole)_DiEnt.Value;                          // наше поле с ответом

   //                 // Запускаем Lua функцию, после сохрания, если есть.
   //                 _Pole?.PROP_Lua?.MET_OnSave();
   //                 // Запускаем сохранение полей
   //                 if (!_Pole.MET_Save())
            //        {
            //            MessageBox.Show("Ошибка сохранения");
            //            return false;
            //        }
            //    }
            //    UserKbolInfo.MET_SaveKbolInfo();

            //    return true;
            //}
            return false;
        }
    }
}
