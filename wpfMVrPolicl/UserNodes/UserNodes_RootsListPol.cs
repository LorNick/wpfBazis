using System;
using System.Collections.Generic;
using System.Linq;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfMVrPolicl
{
    /// <summary>КЛАСС Ветка для Добавочных веток поликлиники</summary>
    /// <remarks>Начальные данные ветки, только для присвоения данных</remarks>
    public class UserNodes_RootsListPol : VirtualNodes_RootsList
    {
        /// <summary>СВОЙСТВО (шаблона) Разрешение создания шаблона</summary>
        ///<remarks>Только для модуля поликлиники</remarks>
        public override bool PROP_shaButtonNew { get { return MyGlo.TypeModul == eModul.VrPolicl; } }

         /// <summary>КОНСТРУКТОР</summary>
        public UserNodes_RootsListPol()
        {
            PRO_TipNodeChild = eTipNodes.Pol_Add;
        }

        ///<summary>МЕТОД Инициализация ветки</summary>
        public override void MET_Inizial()
        {
            //  Тип протоколов
            PROP_shaTipProtokol = new MyTipProtokol(eTipDocum.Pol);
            // Заполняем свойства базового класса
            base.MET_Inizial();
            // Нижний текст вкладок (дата посещения протокола и ФИО врача)
            PROP_TextDown = Convert.ToString(MyGlo.HashAPAC["DP"]).Substring(0, 10) + " "
                               + MySql.MET_NameSpr(Convert.ToInt16(MyGlo.HashAPAC["KV"]), "s_VrachPol");
        }

        ///<summary>МЕТОД Отчет подветки</summary>
        protected override VirtualOtchet MET_OtchetChild(UserDocument pDocument)
        {
            return new UserOtchet_AddPol { PROP_Docum = pDocument };
        }

        ///<summary>МЕТОД Запрос на поиск подветок</summary>
        protected override IEnumerable<UserProtokol> MET_LoadProtokol()
        {
            return ((VirtualModul)MyGlo.Modul).PUB_Protokol.Where(p => p.PROP_TipProtokol.PROP_TipDocum == eTipDocum.Pol && p.PROP_CodApstac == MyGlo.IND).OrderBy(p => p.PROP_Cod);
        }

        ///<summary>МЕТОД Создание подветки</summary>
        protected override UserNodes_Add MET_TypeNodesAdd()
        {
            return new UserNodes_Add();
        }
    }
}
