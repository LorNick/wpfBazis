using System;
using System.Collections.Generic;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfMViewer.UserOtchet;
using wpfStatic;

namespace wpfGeneral.UserModul
{
    /// <summary>КЛАСС Виртуальноый стартовый модуль</summary>
    public abstract class VirtualModul
    {
        /// <summary>СПИСОК KbolInfo</summary>
        public List<UserKbolInfo> PUB_KbolInfo = new List<UserKbolInfo>();

        /// <summary>СПИСОК ListShablon</summary>
        public List<UserListShablon> PUB_ListShablons = new List<UserListShablon>();

        /// <summary>СПИСОК Shablon</summary>
        public List<UserShablon> PUB_Shablon = new List<UserShablon>();

        /// <summary>СПИСОК Protokol</summary>
        public List<UserProtokol> PUB_Protokol = new List<UserProtokol>();

        ///<summary>МЕТОД Начальные данные (пустой)</summary>
        public virtual void MET_NachDan() { }

        /// <summary>МЕТОД Считываем параметры командной строки (пустой)</summary>
        public virtual void MET_ComStr() { }

        /// <summary>МЕТОД Заголовок программы (пустой)</summary>
        public virtual string MET_Title() { return ""; }

        /// <summary>Показываем меню - 1, скрываем меню и смену пациентов = 0</summary>
        public int PUB_Menu = 1;

        /// <summary>МЕТОД Формируем дерево</summary>
        public virtual void MET_CreateTree()
        {
            // Преварительно чистим  дерево
            MyGlo.TreeView.Items.Clear();

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
                {
                    _Node.PROP_ImageName = "mnAngel";
                    _Node.PROP_TextDown = "   Пациент Умер";
                }
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtcher_InformPasport { PROP_Docum = _Node.PROP_Docum };
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
                _Node.PROP_Docum.PROP_Otchet = new UserOtcher_History { PROP_Docum = _Node.PROP_Docum };
                ((UserOtcher_History)_Node.PROP_Docum.PROP_Otchet).PUB_Ban = PUB_Menu == 0;
                _Node.MET_Inizial();

                // ВЕТКА Канцер регистра
                if (MySql.MET_QueryBool(MyQuery.MET_varIfRakReg_Select_1(MyGlo.KL)))                // только если есть пациент в Канцер-Регистре
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
                    _Node.PROP_Docum.PROP_Otchet = new UserOtcher_InformKancerRegistr { PROP_Docum = _Node.PROP_Docum };

                    _Node.MET_Inizial();
                }

                // ВЕТКА Общие документы
                _Node = new UserNodes_RootObDoсum
                {
                    PROP_TipNodes = eTipNodes.Kdl_RootsList,
                    Name = "eleTVItem_ObDocum",
                    PROP_Text = "Общие документы",
                    PROP_TextDefault = "Общие документы",
                    PROP_ImageName = "mnObDocum",
                    PROP_ParentName = "eleTVItemObSved"
                };
                ((UserNodes_RootObDoсum)_Node).PROP_shaTipObsled = 140;
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtchet_Roots { PROP_Docum = _Node.PROP_Docum };
                _Node.MET_Inizial();

                // Тест. Пока только для админов
                if (MyGlo.Admin)
                {
                    // ВЕТКА Pdf документы
                    _Node = new UserNodes_RootPdf
                    {
                        PROP_TipNodes = eTipNodes.Kdl_RootsPdf,
                        Name = "eleTVItem_Pdf",
                        PROP_Text = "PDF документы",
                        PROP_TextDefault = "PDF документы",
                        PROP_ImageName = "mnPdfMain",
                        PROP_ParentName = "eleTVItemObSved"
                    };
                    ((UserNodes_RootPdf)_Node).PROP_shaTipObsled = 2;
                    _Node.PROP_Docum = new UserDocument(_Node);
                    _Node.PROP_Docum.PROP_Otchet = new UserOtchet_RootsPdf { PROP_Docum = _Node.PROP_Docum };
                    _Node.MET_Inizial();
                }

                // ВЕТКА Телефоны
                _Node = new UserNodes_Inform
                {
                    Name = "elePhone",
                    PROP_Text = "Телефоны",
                    PROP_TextDefault = "Телефоны",
                    PROP_ImageName = "mnPhone",
                    PROP_ParentName = "",
                };
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtcher_Phone { PROP_Docum = _Node.PROP_Docum };
                _Node.MET_Inizial();

                // ВЕТКА Сотрудники (Только для админов)
                if (MyGlo.Admin)
                {
                    _Node = new UserNodes_Inform
                    {
                        Name = "eleStaff",
                        PROP_Text = "Сотрудники",
                        PROP_TextDefault = "Сотрудники",
                        PROP_ImageName = "mnMen",
                        PROP_ParentName = "",
                    };
                    _Node.PROP_Docum = new UserDocument(_Node);
                    _Node.PROP_Docum.PROP_Otchet = new UserOtcher_Staff {PROP_Docum = _Node.PROP_Docum};
                    _Node.MET_Inizial();
                }
            }
        }
    }
}
