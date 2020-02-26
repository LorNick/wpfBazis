using System.Collections.Generic;
using System.Linq;
using wpfGeneral.UserModul;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfGeneral.UserNodes
{
    /// <summary>КЛАСС Ветка для Добавочных веток документов из таблиц kdl</summary>
    /// <remarks>Начальные данные ветки, только для присвоения данных</remarks>
    public class UserNodes_RootObDoсum : VirtualNodes_RootsList
    {
        /// <summary>КОНСТРУКТОР</summary>
        public UserNodes_RootObDoсum()
        {
            PRO_TipNodeChild = eTipNodes.Kdl_Add;
        }

        ///<summary>МЕТОД Инициализация ветки</summary>
        public override void MET_Inizial()
        {
            //  Тип протоколов
            PROP_shaTipProtokol = new MyTipProtokol(eTipDocum.Kdl);
            // Заполняем свойства базового класса
            base.MET_Inizial();
        }

        ///<summary>МЕТОД Отчет подветки</summary>
        protected override VirtualOtchet MET_OtchetChild(UserDocument pDocument)
        {
            return new UserOtchet_AddDocum { PROP_Docum = pDocument };
        }

        ///<summary>МЕТОД Запрос на поиск подветок</summary>
        protected override IEnumerable<UserProtokol> MET_LoadProtokol()
        {
            var _p = ((VirtualModul) MyGlo.Modul).PUB_Protokol;
            var _l = ((VirtualModul) MyGlo.Modul).PUB_ListShablons;

            var _k =
                _p.Join(_l.Where(k => k.PROP_TipObsled == 140 || k.PROP_TipObsled == 1), a => a.PROP_NumShablon,
                    b => b.PROP_Cod, (a, b) => a).Where(p => p.PROP_TipProtokol.PROP_TipDocum == eTipDocum.Kdl
                                                             && p.PROP_KL == MyGlo.KL).ToList().OrderBy(p => p.PROP_pDate).ThenBy(p => p.PROP_Cod);

            return _k;
        }

        ///<summary>МЕТОД Создание подветки</summary>
        protected override UserNodes_Add MET_TypeNodesAdd()
        {
            return new UserNodes_Add(); 
        }

        /// <summary>МЕТОД Настраиваем дополнительные параметры для НОВОЙ подветки</summary>
        protected override void MET_PropertyNodeAdd(VirtualNodes pNodes)
        {
            // Если это гистология запрещаем редактировать
            pNodes.PROP_shaButtonEdit = pNodes.PROP_Docum.PROP_ListShablon.PROP_TipObsled != 140;
        }
    }
}
