using System.Collections.Generic;
using System.Linq;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfMVrStac
{
    /// <summary>КЛАСС Ветка для Добавочных веток стационара</summary>
    /// <remarks>Начальные данные ветки, только для присвоения данных</remarks>
    public class UserNodes_RootListVrStac : VirtualNodes_RootsList
    {
        /// <summary>КОНСТРУКТОР</summary>
        public UserNodes_RootListVrStac()
        {
            PRO_TipNodeChild = eTipNodes.Stac_Add;
        }

        ///<summary>МЕТОД Инициализация ветки</summary>
        public override void MET_Inizial()
        {
            //  Тип протоколов
            PROP_shaTipProtokol = new MyTipProtokol(eTipDocum.Stac);
            // Заполняем свойства базового класса
            base.MET_Inizial();
        }

        ///<summary>МЕТОД Отчет подветки</summary>
        protected override VirtualOtchet MET_OtchetChild(UserDocument pDocument)
        {
            return new UserOtchet_AddVrStac { PROP_Docum = pDocument };
        }

        ///<summary>МЕТОД Запрос на поиск подветок</summary>
        protected override IEnumerable<UserProtokol> MET_LoadProtokol()
        {
            int _Tip_1 = PROP_shaTipObsled;
            int _Tip_2 = PROP_shaTipObsled;

            switch (PROP_shaTipObsled)
            {
                case 104:                                                       // Шаблоны лечения
                    _Tip_1 = 102;
                    _Tip_2 = 109;
                    break;
                case 130:                                                       // Шаблоны Анестезиолога
                    _Tip_2 = 139;
                    break;
                case 110:                                                       // Обходы и консилиумы
                    _Tip_2 = 119;
                    break;
                case 105:                                                       // Документы
                    _Tip_1 = 140;
                    _Tip_2 = 160;
                    break;
            }
            var _p = ((VirtualModul) MyGlo.Modul).PUB_Protokol;
            var _l = ((VirtualModul) MyGlo.Modul).PUB_ListShablons;

            var _k =
                _p.Join(_l.Where(k => k.PROP_TipObsled >= _Tip_1 && k.PROP_TipObsled <= _Tip_2 && k.PROP_TipProtokol.PROP_TipDocum == eTipDocum.Stac), a => a.PROP_NumShablon,
                    b => b.PROP_Cod, (a, b) => a).Where(p => p.PROP_TipProtokol.PROP_TipDocum == eTipDocum.Stac
                                                             && p.PROP_CodApstac == MyGlo.IND).ToList().OrderBy(p => p.PROP_pDate).ThenBy(p => p.PROP_Cod);
            return _k;
        }

        ///<summary>МЕТОД Создание подветки</summary>
        protected override UserNodes_Add MET_TypeNodesAdd()
        {
            return new UserNodes_Add();
        }
    }
}
