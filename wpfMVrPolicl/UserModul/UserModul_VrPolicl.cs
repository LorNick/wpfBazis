using System;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfMVrPolicl
{
    /// <summary>КЛАСС Стартовый модуль Врача Поликлиники</summary>
    public sealed class UserModul_VrPolicl : VirtualModul
    {  
        /// <summary>МЕТОД Считываем параметры командной строки</summary>
        public override void MET_ComStr()
        {
            MyGlo.KL = 116730666826365; //114510866161757;
            MyGlo.Otd = 11;  // Профиль врача ProfilVr
            MyGlo.IND = 1704377; //631942;

            String[] _mArgs = Environment.GetCommandLineArgs();
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
            // Заполняем HashAPAC
            MyGlo.HashAPAC = MySql.MET_QueryHash(MyQuery.APAC_Select_1(MyGlo.IND));
            // Заполняем HashKBOL
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
            string _Title = "wpfBazis -- Врач поликлиники --";
            // Номер версии
            _Title += " " + MyMet.MET_Ver();
            // Пациент
            _Title += "  (" + MyGlo.FIO + " " + MyGlo.DR;
            // Показываем имя пользователя
            _Title += " - " + MyGlo.UserName + ")";
            return _Title;
        }

        /// <summary>МЕТОД Формируем дерево</summary>
        public override void MET_CreateTree()
        {
            // Заполняем основу дерево (паспорт + история)
            base.MET_CreateTree();
            
            // Загружаем все протоколы Protokol текущей поликлиники (внутри загружаются и ListShablon и Shablon)
            UserProtokol.MET_FactoryProtokolArray(eTipDocum.Pol, MyGlo.IND);

            VirtualNodes _Node;

            // Временно скрыта для врачей по приказу начальника, из за конфликта Тарасевич-Плахотенко (сентябрь 2019)
            // Пока открыл (июль 2020)
            //if (MyGlo.Admin)
            //{
                // ВЕТКА Запись в регистратуру
                _Node = new UserNodes_Inform
                {
                    PROP_TipNodes = eTipNodes.EditDocum,
                    Name = "eleTVItem_Reg",
                    PROP_Text = "Регистратура",
                    PROP_ImageName = "mnReg",
                    PROP_ParentName = ""
                };
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtchet_Registration { PROP_Docum = _Node.PROP_Docum };
                _Node.MET_Inizial();
            //}

            // ВЕТКА Поликлиника
            _Node = new UserNodes_RootsListPol
            {
                PROP_TipNodes = eTipNodes.Pol_RootsList,
                Name = "eleTVItem_Pol",
                PROP_Text = "Поликлиника",
                PROP_TextDefault = "Поликлиника",
                PROP_ImageName = "mnPosPolikl",
                PROP_ParentName = "",
                IsExpanded = true
            };
            _Node.PROP_Docum = new UserDocument(_Node);
            _Node.PROP_Docum.PROP_Otchet = new UserOtchet_Roots { PROP_Docum = _Node.PROP_Docum };
            _Node.MET_Inizial();
            _Node.IsSelected = true;
        }
    }
}


