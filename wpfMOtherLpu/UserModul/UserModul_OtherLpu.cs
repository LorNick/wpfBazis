using System;
using System.Data;
using System.Linq;
using System.Windows;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfGeneral.UserWindows;
using wpfStatic;

namespace wpfMOtherLpu
{
    /// <summary>КЛАСС Модуля Работы со списком протоколов</summary>
    public class UserModul_OtherLpu : VirtualModul
    {
        /// <summary>МЕТОД Считываем параметры командной строки</summary>
        public override void MET_ComStr()
        {
            MyGlo.KL = MyGlo.DataSet.Tables["s_Users"].AsEnumerable()
                .FirstOrDefault(p => p.Field<int>("Cod") == MyGlo.User)?.Field<decimal>("KL") ?? 0;

#if DEBUG
            // Показываем меню - 1 (КОД), скрываем меню и смену пациентов = 0 (онкологи ЛПУ) - пока показываем
            PUB_Menu = 1;
            MyGlo.Otd = 30602060;
            MyGlo.Lpu = 554512; //554505; // 554403; // БСМП2 - 554502, МСЧ 7 - 554403, Кардио - 554505
#endif
            string[] _mArgs = Environment.GetCommandLineArgs();
            for (int x = 0; x < _mArgs.Length; x++)
            {
                // Нулевая строка - путь к программе
                // 1, 2, 3 заполняются в MyGlo
                if (x == 4)
                    MyGlo.Otd = Convert.ToInt32(_mArgs[4]);
                if (x == 5)
                    MyGlo.Lpu = Convert.ToInt32(_mArgs[5]);
            }
        }

        /// <summary>МЕТОД Начальные данные</summary>
        public override void MET_NachDan()
        {
            // Если нет пациента - выходим
            if (MyGlo.KL == 0)
                return;
            // Заполняем hasKBOL
            MyGlo.HashKBOL = MySql.MET_QueryHash(MyQuery.kbol_Select_1(MyGlo.KL));
            // Заполняем HashLastDiag
            MyGlo.HashLastDiag = MySql.MET_QueryHash(MyQuery.MET_kbolInfo_Select_3(MyGlo.KL));
            // ФИО пациента
            MyGlo.FIO = Convert.ToString(MyGlo.HashKBOL["FIO"]);
            // Дата рождения пациента
            MyGlo.DR = MyMet.MET_StrDat(MyGlo.HashKBOL["DR"]);
            // Пол пациента
            MyGlo.Pol = Convert.ToInt16(MyGlo.HashKBOL["POL"]) == 1 ? "Мужской" : "Женский";
            // Обнуляем историю болезни
            MyGlo.HashOtchet.Clear();
        }

        /// <summary>МЕТОД Заголовок программы</summary>
        public override string MET_Title()
        {
            // Наименование модуля
            string _Title = "wpfBazis -- ЛПУ --";
            // Номер версии
            _Title += " " + MyMet.MET_Ver();
            // Пациент
            _Title += MyGlo.KL > 0 ? "  (" + MyGlo.FIO + " " + MyGlo.DR : " ( ";
            // Показываем имя пользователя
            _Title += " - " + MyGlo.UserName + ")";
            return _Title;
        }

        /// <summary>МЕТОД Формируем дерево</summary>
        public override void MET_CreateTree()
        {
            // Преварительно чистим  дерево
            MyGlo.TreeView.Items.Clear();
            if (MyGlo.KL > 0)
            {
                // Чистим структуру протоколов
                UserProtokol.MET_ClearProtokol();
                // Загружаем все протоколы Protokol таблицы kdl, по KL пациента
                UserProtokol.MET_FactoryProtokolArray(eTipDocum.Kdl, MyGlo.KL, "KL");
                // Загружаем все протоколы Protokol таблицы Pol, по KL пациента
                UserProtokol.MET_FactoryProtokolArray(eTipDocum.Pol, MyGlo.KL, "KL");
                // Загружаем все протоколы Protokol таблицы Stac, по KL пациента
                UserProtokol.MET_FactoryProtokolArray(eTipDocum.Stac, MyGlo.KL, "KL");

                // ВЕТКА Общие сведенья
                VirtualNodes _Node = new UserNodes_Inform
                {
                    PROP_TipNodes = eTipNodes.Main,
                    Name = "eleTVItemObSved",
                    PROP_Text = "Общие сведения",
                    PROP_ImageName = "mnObSved",
                    PROP_ParentName = "",
                    IsExpanded = true
                };
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtcher_InformCreate {PROP_Docum = _Node.PROP_Docum};
                _Node.MET_Inizial();
                {
                    // ВЕТКА Паспортная часть (Сразу ставим на нем фокус)
                    _Node = new UserNodes_Inform
                    {
                        PROP_TipNodes = eTipNodes.Inform,
                        Name = "eleTVItemPasp",
                        PROP_Text = "Паспортная часть",
                        PROP_ImageName = "mnPasp",
                        PROP_ParentName = "eleTVItemObSved"
                    };
                    // Если пациент умер, то пишем сообщение
                    if (Convert.ToString(MyGlo.HashKBOL["DSmerti"]) != "")
                        _Node.PROP_TextDown = "   Пациент Умер";
                    _Node.PROP_Docum = new UserDocument(_Node);
                    _Node.PROP_Docum.PROP_Otchet = new UserOtcher_InformPasport {PROP_Docum = _Node.PROP_Docum};
                    _Node.MET_Inizial();
                    _Node.IsSelected = true;

                    // ВЕТКА История болезни
                    _Node = new UserNodes_Inform
                    {
                        PROP_TipNodes = eTipNodes.Inform,
                        Name = "eleTVItem_History",
                        PROP_Text = "История болезни",
                        PROP_ImageName = "mnHistory",
                        PROP_ParentName = "eleTVItemObSved"
                    };
                    _Node.PROP_Docum = new UserDocument(_Node);
                    _Node.PROP_Docum.PROP_Otchet = new UserOtcher_History {PROP_Docum = _Node.PROP_Docum};
                    ((UserOtcher_History) _Node.PROP_Docum.PROP_Otchet).PUB_Ban = PUB_Menu == 0;
                    _Node.MET_Inizial();

                    // ВЕТКА Канцер регистра
                    if (MySql.MET_QueryBool(MyQuery.MET_varIfRakReg_Select_1(MyGlo.KL)))
                        // только если есть пациент в Канцер-Регистре
                    {
                        _Node = new UserNodes_Inform
                        {
                            PROP_TipNodes = eTipNodes.Inform,
                            Name = "eleTVItem_KancerRegistr",
                            PROP_Text = "Канцер-Регистр",
                            PROP_ImageName = "mnRakReg",
                            PROP_ParentName = "eleTVItemObSved"
                        };
                        _Node.PROP_Docum = new UserDocument(_Node);
                        _Node.PROP_Docum.PROP_Otchet = new UserOtcher_InformKancerRegistr
                        {
                            PROP_Docum = _Node.PROP_Docum
                        };
                        _Node.MET_Inizial();
                    }

                    // ВЕТКА Для направлений
                    _Node = new UserNodes_RootNaprInOtherLpu
                    {
                        PROP_TipNodes = eTipNodes.Pol_RootsList,
                        Name = "eleTVItem_NaprOtherLpu",
                        PROP_Text = "Направления",
                        PROP_TextDefault = "Направления",
                        PROP_ImageName = "mnSelectPac",
                        PROP_ParentName = "eleTVItemObSved",
                        IsExpanded = true
                    };
                    _Node.PROP_Docum = new UserDocument(_Node);
                    _Node.PROP_Docum.PROP_Otchet = new UserOtchet_Roots { PROP_Docum = _Node.PROP_Docum };
                    _Node.MET_Inizial();

                    // ВЕТКА Для документов - внешних ЛПУ
                    _Node = new UserNodes_OtherLpu
                    {
                        PROP_TipNodes = eTipNodes.Kdl_RootsList,
                        Name = "eleTVItem_OtherLpu",
                        PROP_Text = "Документы",
                        PROP_TextDefault = "Документы",
                        PROP_ImageName = "mnObDocum",
                        PROP_ParentName = "eleTVItemObSved",
                        IsExpanded = true
                    };
                    _Node.PROP_Docum = new UserDocument(_Node);
                    _Node.PROP_Docum.PROP_Otchet = new UserOtchet_Roots {PROP_Docum = _Node.PROP_Docum};
                    _Node.MET_Inizial();
                }
            }
            else
            {
                VirtualUserWindow _WinSpr = new UserWindow_OtherLpu();
                _WinSpr.PROP_Modal = true;
                _WinSpr.WindowStyle = WindowStyle.ToolWindow;
                _WinSpr.ShowDialog();
                if (_WinSpr.PROP_Return)
                {
                    // Запуск программы MET_Window_Loaded()
                    MyGlo.callbackEvent_sReloadWindows?.Invoke(true);
                }
            }
        }
    }
}
