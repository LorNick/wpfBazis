using System;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfMVrStac.UserOtchet;
using wpfStatic;

namespace wpfMVrStac
{
    /// <summary>КЛАСС Стартовый модуль Врача Стационара</summary>
    public class UserModul_VrStac : VirtualModul
    {  
        /// <summary>МЕТОД Считываем параметры командной строки</summary>
        public override void MET_ComStr()
        {
            MyGlo.IND = 128212783952037;
            MyGlo.KL = 127503609896201;
            MyGlo.Otd = 1;

            string[] _mArgs = Environment.GetCommandLineArgs();
            for (int x = 0; x < _mArgs.Length; x++)
            {
                // Нулевая строка - путь к программе
                // 1, 2, 3 заполняются в MyGlo
                if (x == 4)
                    MyGlo.KL = Convert.ToDecimal(_mArgs[4]);
                if (x == 5)
                    MyGlo.IND = Convert.ToDecimal(_mArgs[5]);
                if (x == 6)
                    MyGlo.Otd = Convert.ToInt32(_mArgs[6]);
            }            
        }

        /// <summary>МЕТОД Начальные данные</summary>
        public override void MET_NachDan()
        {
            // Заполняем hasAPSTAC
            MyGlo.HashAPSTAC = MySql.MET_QueryHash(MyQuery.APSTAC_Select_1(MyGlo.IND));
            // Код пациента
            MyGlo.KL = (decimal)MyGlo.HashAPSTAC["KL"];
            // Номер стац. карты
            MyGlo.NSTAC = (int)MyGlo.HashAPSTAC["NSTAC"];
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
            // Дата, когда положили пациента в стационар
            MyGlo.DatePriem = MyMet.MET_StrDat(MyGlo.HashAPSTAC["DN"]);
            // Полная строка диагноза
            MyGlo.DiagStac = Convert.ToString(MyGlo.HashAPSTAC["D"]);
            // Обнуляем историю болезни
            MyGlo.HashOtchet.Clear();
        }

        /// <summary>МЕТОД Заголовок программы</summary>
        public override string MET_Title()
        {   
            // Наименование модуля
            string _Title = "wpfBazis -- Врач стационара --";
            // Номер версии
            _Title += " " + MyMet.MET_Ver();
            // Отделение          
            _Title += " (" + MySql.MET_NameSpr(MyGlo.Otd, "s_Department", "Names", "Cod");
            // Пациент
            _Title += "  " + MyGlo.FIO + " " + MyGlo.DR;
            // Показываем имя пользователя
            _Title += " - " + MyGlo.UserName + ")";
            return _Title;
        }

        /// <summary>МЕТОД Формируем дерево</summary>
        public override void MET_CreateTree()
        {
            // Заполняем основу дерево (паспорт + история)
            base.MET_CreateTree();
            
            // Загружаем все протоколы Protokol текущего стационара (внутри загружаются и ListShablon и Shablon)
            UserProtokol.MET_FactoryProtokolArray(eTipDocum.Stac, MyGlo.IND);
            
            // ВЕТКА Ошибки Стационара (для реестров)
            MySql.MET_DsAdapterFill(MyQuery.MET_varErrorStac_Select_1(MyGlo.Otd, MyGlo.User), "ErrorStac");
            int _AllError = MyGlo.DataSet.Tables["ErrorStac"].Rows.Count;
            VirtualNodes _Node;
            if (_AllError > 0)
            {
                _Node = new UserNodes_Inform
                {
                    PROP_TipNodes = eTipNodes.Main,
                    Name = "eleTVItem_ErrorStac",
                    PROP_Text = "\"Ошибки\" стационара",
                    PROP_ImageName = "mnDevil",
                    PROP_ParentName = "eleTVItemObSved"
                };
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtcher_ErrorStac { PROP_Docum = _Node.PROP_Docum };
                _Node.MET_Inizial();

                var _UserError = MyGlo.DataSet.Tables["ErrorStac"].Compute("Count(Us)", "Us=1");
                _Node.PROP_TextDown = $"Ваших пациентов: {_UserError} из {_AllError}";

                if (_UserError != null)
                    _Node.IsSelected = Convert.ToInt16(_UserError) > 0;
            }

            // ВЕТКА Текущий стационар
            _Node = new UserNodes_Inform
            {
                PROP_TipNodes = eTipNodes.Main,
                Name = "eleTVItem_TekStac",
                PROP_Text = "Текущий стационар",
                PROP_ParentName = "eleTVItemObSved",
                IsExpanded = true
            };
            // Иконка в зависимости от типа стационара
            if (MySql.MET_QueryInt(MyQuery.s_Department_Select_2(MyGlo.Otd)) == 1)
                _Node.PROP_ImageName = "mnStac";
            else
                _Node.PROP_ImageName = "mnStacDnev";
            _Node.PROP_Docum = new UserDocument(_Node);
            _Node.PROP_Docum.PROP_Otchet = new UserOtchet_Roots { PROP_Docum = _Node.PROP_Docum };
            _Node.MET_Inizial();
            {
                // ВЕТКА Приемное отделение
                _Node = new UserNodes_Inform
                {
                    PROP_TipNodes = eTipNodes.Inform,
                    Name = "eleTVItem_PriemnOtd",
                    PROP_Text = "Приемное отделение",
                    PROP_ImageName = "mnPriemnOtd",
                    PROP_ParentName = "eleTVItem_TekStac"
                };
                // Дата поступления - выписки
                string _DN = Convert.ToString(MyGlo.HashAPSTAC["DN"]);
                string _DK = Convert.ToString(MyGlo.HashAPSTAC["DK"]);
                _DN = _DN == "" ? "" : _DN.Substring(0, 10);
                _DK = _DK == "" ? " - в отделении" : " - " + _DK.Substring(0, 10);
                _Node.PROP_TextDown = _DN + _DK;
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtcher_InformPriem { PROP_Docum = _Node.PROP_Docum };
                _Node.MET_Inizial();
              

                // ВЕТКА Осмотр при поступлении
                _Node = new UserNodes_EditVrStac
                {
                    PROP_TipNodes = eTipNodes.Stac_Edit,
                    Name = "eleTVItem_OneOsmotr",
                    PROP_Text = "Осмотр при поступлении",
                    PROP_TextDefault = "Осмотр при поступлении",
                    PROP_ImageName = "mnOneOsmotr",
                    PROP_ParentName = "eleTVItem_TekStac"
                };
                _Node.PROP_shaNomerShablon = 1;
                _Node.PROP_Docum = new UserDocument(_Node, eTipDocum.Stac);
                _Node.PROP_Docum.PROP_ListShablon = UserListShablon.MET_FactoryListShablon(eTipDocum.Stac, _Node.PROP_shaNomerShablon);
                _Node.PROP_Docum.PROP_Otchet = new UserOtchet_EditVrStac { PROP_Docum = _Node.PROP_Docum };
                _Node.MET_Inizial();
                {
                    // ВЕТКА Гистология
                    _Node = new UserNodes_EditVrStac
                    {
                        PROP_TipNodes = eTipNodes.Stac_Edit,
                        Name = "eleTVItem_Gistol",
                        PROP_Text = "Гистология",
                        PROP_TextDefault = "Гистология",
                        PROP_ImageName = "mnDoc_2",
                        PROP_ParentName = "eleTVItem_OneOsmotr"
                    };
                    _Node.PROP_shaNomerShablon = 9911;
                    _Node.PROP_Docum = new UserDocument(_Node, eTipDocum.Stac);
                    _Node.PROP_Docum.PROP_ListShablon = UserListShablon.MET_FactoryListShablon(eTipDocum.Stac, _Node.PROP_shaNomerShablon);
                    _Node.PROP_Docum.PROP_Otchet = new UserOtchet_EditVrStac { PROP_Docum = _Node.PROP_Docum };
                    _Node.MET_Inizial();  
                }

                // ВЕТКА Назначения мед. препаратов
                _Node = new UserNodes_NaznachMed
                {
                    PROP_TipNodes = eTipNodes.EditDocum,
                    Name = "eleTVItem_NaznachMed",
                    PROP_Text = "Медикаменты",
                    PROP_TextDefault = "Медикаменты",
                    PROP_ImageName = "mnNaznachMed",
                    // PROP_ParentName = "eleTVItem_Naznach"
                    PROP_ParentName = "eleTVItem_TekStac"
                };
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtchet_NaznachMed { PROP_Docum = _Node.PROP_Docum };
                _Node.MET_Inizial();
                _Node.PROP_prnPadding = 2;
                { 

                    // ВЕТКА Бланк листа назначений
                    _Node = new UserNodes_Inform
                    {
                        PROP_TipNodes = eTipNodes.Inform,
                        Name = "eleTVItem_NaznachBlanck",
                        PROP_Text = "Бланк листа назначений",
                        PROP_TextDefault = "Бланк листа назначений",
                        PROP_ImageName = "mnDoc_7",
                        PROP_ParentName = "eleTVItem_NaznachMed"
                        //PROP_ParentName = "eleTVItem_Naznach"
                    };
                    _Node.PROP_Docum = new UserDocument(_Node);
                    _Node.PROP_Docum.PROP_Otchet = new UserOtchet_NaznachBlanck { PROP_Docum = _Node.PROP_Docum };
                    _Node.MET_Inizial();
                    _Node.PROP_prnPadding = 2;
                }

                // ВЕТКА Анестезиолог
                _Node = new UserNodes_RootListVrStac
                {
                    PROP_TipNodes = eTipNodes.Stac_RootsList,
                    Name = "eleTVItem_Anest",
                    PROP_Text = "Анестезиолог",
                    PROP_TextDefault = "Анестезиолог",
                    PROP_ImageName = "mnAnest",
                    PROP_ParentName = "eleTVItem_TekStac"
                };
                ((UserNodes_RootListVrStac)_Node).PROP_shaTipObsled = 130;
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtchet_Roots { PROP_Docum = _Node.PROP_Docum };
                _Node.MET_Inizial();

                // ВЕТКА Лечение
                _Node = new UserNodes_RootListVrStac
                {
                    PROP_TipNodes = eTipNodes.Stac_RootsList,
                    Name = "eleTVItem_Oper",
                    PROP_Text = "Лечение",
                    PROP_TextDefault = "Лечение",
                    PROP_ImageName = "mnOper",
                    PROP_ParentName = "eleTVItem_TekStac"
                };
                ((UserNodes_RootListVrStac)_Node).PROP_shaTipObsled = 104;
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtchet_Roots { PROP_Docum = _Node.PROP_Docum };
                _Node.MET_Inizial();

                // ВЕТКА Обходы, консилиумы
                _Node = new UserNodes_RootListVrStac
                {
                    PROP_TipNodes = eTipNodes.Stac_RootsList,
                    Name = "eleTVItem_Obhod",
                    PROP_Text = "Обходы, консилиумы",
                    PROP_TextDefault = "Обходы, консилиумы",
                    PROP_ImageName = "mnObhod",
                    PROP_ParentName = "eleTVItem_TekStac"
                };
                ((UserNodes_RootListVrStac)_Node).PROP_shaTipObsled = 110;
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtchet_Roots { PROP_Docum = _Node.PROP_Docum };
                _Node.MET_Inizial();

                // ВЕТКА Документы
                _Node = new UserNodes_RootListVrStac
                {
                    PROP_TipNodes = eTipNodes.Stac_RootsList,
                    Name = "eleTVItem_Dokum",
                    PROP_Text = "Документы",
                    PROP_TextDefault = "Документы",
                    PROP_ImageName = "mnDocuments",
                    PROP_ParentName = "eleTVItem_TekStac"
                };
                ((UserNodes_RootListVrStac)_Node).PROP_shaTipObsled = 105;
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtchet_Roots { PROP_Docum = _Node.PROP_Docum };
                _Node.MET_Inizial();

                // ВЕТКА Выписные документы
                var _Node2 = new UserNodes_RootListVrStac
                {
                    PROP_TipNodes = eTipNodes.Stac_RootsList,
                    Name = "eleTVItem_Extact",
                    PROP_Text = "Выписные документы",
                    PROP_TextDefault = "Выписные документы",
                    PROP_ImageName = "mnGoHome",
                    PROP_ParentName = "eleTVItem_TekStac"
                };
                _Node2.PROP_shaTipObsled = 120;
                _Node2.PROP_Docum = new UserDocument(_Node2);
                _Node2.PROP_Docum.PROP_Otchet = new UserOtchet_Roots { PROP_Docum = _Node2.PROP_Docum };
                _Node2.MET_Inizial();
            }   
        }
    }
}
