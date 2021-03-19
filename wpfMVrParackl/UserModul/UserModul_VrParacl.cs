using System;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfMVrParacl
{
    /// <summary>КЛАСС Модуля Врача Параклиники</summary>
    public class UserModul_VrParacl : VirtualModul
    {   
        /// <summary>МЕТОД Считываем параметры командной строки</summary>
        public override void MET_ComStr()
        {
            MyGlo.KL = 125069331097215;
            MyGlo.IND = 480177;
            MyGlo.Otd = 2;

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
            string _Title = "wpfBazis -- Врач параклиники --";
            // Номер версии
            _Title += " " + MyMet.MET_Ver();
            // Элемент расписания 
            _Title += " (" + MySql.MET_NameSpr(MyGlo.Otd, "parElement", "FIO", "Cod") + " ";
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
            
            // Загружаем все протоколы Protokol текущей параклиники (внутри загружаются и ListShablon и Shablon)
            UserProtokol.MET_FactoryProtokolArray(eTipDocum.Paracl, MyGlo.IND);

            VirtualNodes _Node;
            
            // ВЕТКА Параклинические исследования
            _Node = new UserNodes_RootsListParacl
            {
                PROP_TipNodes = eTipNodes.Para_RootsList,
                Name = "eleTVItem_ParaIss",
                PROP_Text = "Исследования",
                PROP_TextDefault = "Исследования",
                PROP_ImageName = "mnParacl",
                PROP_ParentName = "",
                IsExpanded = true
            };
            _Node.PROP_Docum = new UserDocument(_Node);
            _Node.PROP_Docum.PROP_Otchet = new UserOtchet_Roots { PROP_Docum = _Node.PROP_Docum };
            _Node.MET_Inizial();
        }
    }
}
