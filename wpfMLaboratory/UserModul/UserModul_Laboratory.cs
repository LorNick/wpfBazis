using System;
using System.Windows;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfGeneral.UserWindows;
using wpfStatic;

namespace wpfMLaboratory
{
    /// <summary>КЛАСС Стартовый модуль Лаборатория</summary>
    public class UserModul_Laboratory : VirtualModul
    {
        /// <summary>МЕТОД Считываем параметры командной строки</summary>
        public override void MET_ComStr()
        {
#if DEBUG
            // Показываем меню - 1 (КОД), скрываем меню и смену пациентов = 0 (онкологи ЛПУ) - пока показываем

            //PUB_Menu = 1;
            //MyGlo.Otd = 55450501;
            //MyGlo.Lpu = 554505; // 554403; // БСМП2 - 554502, МСЧ 7 - 554403, Кардио - 554505
            MyGlo.IND = 480177;
#endif
            string[] _mArgs = Environment.GetCommandLineArgs();
            for (int x = 0; x < _mArgs.Length; x++)
            {
                // Нулевая строка - путь к программе
                // 1, 2, 3 заполняются в MyGlo
                if (x == 4)
                    MyGlo.Otd = Convert.ToInt32(_mArgs[4]);
                if (x == 5)
                    MyGlo.KL = Convert.ToDecimal(_mArgs[4]);
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
             string _Title = "wpfBazis -- Лаборатория --";
            // Номер версии
            _Title += " " + MyMet.MET_Ver();
            // Показываем имя пользователя
            _Title += " (" + MyGlo.UserName + ")";
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
                _Node.PROP_Docum.PROP_Otchet = new UserOtcher_InformCreate { PROP_Docum = _Node.PROP_Docum };
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
                    _Node.PROP_Docum.PROP_Otchet = new UserOtcher_InformPasport { PROP_Docum = _Node.PROP_Docum };
                    _Node.MET_Inizial();
                    _Node.IsSelected = true;

                    // ВЕТКА Для лабораторных исследований (протоколы с 1000 по 1999)
                    _Node = new UserNodes_RootLaboratory
                    {
                        PROP_TipNodes = eTipNodes.Kdl_RootsList,
                        Name = "eleTVItem_Laboratory",
                        PROP_Text = "Исследования",
                        PROP_TextDefault = "Исследования",
                        PROP_ImageName = "mnKdl_green",
                        PROP_ParentName = "eleTVItemObSved",
                        IsExpanded = true
                    };
                    _Node.PROP_Docum = new UserDocument(_Node);
                    _Node.PROP_Docum.PROP_Otchet = new UserOtchet_Roots { PROP_Docum = _Node.PROP_Docum };
                    _Node.MET_Inizial();
                }
            }
            else
            {
                VirtualUserWindow _WinSpr = new UserWindow_Laboratory();
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
