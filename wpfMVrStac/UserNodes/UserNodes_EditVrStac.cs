using System.Linq;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfMVrStac
{
    /// <summary>КЛАСС Ветка для Редактируемых веток Врача Стационара</summary>
    public class UserNodes_EditVrStac : VirtualNodes
    {
        /// <summary>Номер шаблона</summary>
        private int PRI_NomerShablon;

        /// <summary>СВОЙСТВО (шаблона) Разрешение редактирования шаблона</summary>
        public override bool PROP_shaButtonEdit { get { return PROP_shaPresenceProtokol; } }

        /// <summary>СВОЙСТВО (шаблона) Разрешение создания шаблона</summary>
        public override bool PROP_shaButtonNew { get { return !PROP_shaPresenceProtokol; } }

        /// <summary>СВОЙСТВО (шаблона) Номер шаблона</summary>
        public override int PROP_shaNomerShablon
        {
            get { return PRI_NomerShablon; }
            set
            {
                // Если последняя часть шаблона (иначе весь код шаблона)
                if (value < 99)
                {
                    PRI_NomerShablon = (MyGlo.Otd > 50 ? MyGlo.Otd - 50 : MyGlo.Otd) * 100 + value;
                    // Временно только для Химиотерапии 12
                    if (MyGlo.Otd == 12)
                        PRI_NomerShablon = 3 * 100 + value;
                }
                else
                    PRI_NomerShablon = value;
                // Проверяем наличие протокола
                PROP_shaPresenceProtokol = ((VirtualModul)MyGlo.Modul).PUB_Protokol.Any(p => p.PROP_TipProtokol.PROP_TipDocum == eTipDocum.Stac
                        && p.PROP_CodApstac == MyGlo.IND && p.PROP_NumShablon == PRI_NomerShablon);
            }
        }

        ///<summary>МЕТОД Инициализация ветки</summary>
        public override void MET_Inizial()
        {
            // Заполняем свойства базового класса
            base.MET_Inizial();
            //  Тип протоколов
            PROP_shaTipProtokol = new MyTipProtokol(eTipDocum.Stac);
            // Если есть протокол в наличии
            if (PROP_shaPresenceProtokol)
            {
                // Находим протокол
                PROP_Docum.PROP_Protokol = UserProtokol.MET_FactoryProtokol(PROP_shaTipProtokol.PROP_TipDocum, MyGlo.IND, PROP_shaNomerShablon, PROP_shaIndex);
                PROP_Data = PROP_Docum.PROP_Protokol.PROP_pDate;
                PROP_TextDown = PROP_Data.ToString().Substring(0, 10);
            }
        }
    }
}
