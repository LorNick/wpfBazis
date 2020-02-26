using System.Collections.Generic;
using System.Linq;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfMOtherLpu
{
    /// <summary>КЛАСС Ветка для Добавочных веток документов из таблиц kdl</summary>
    /// <remarks>Начальные данные ветки, только для присвоения данных</remarks>
    public class UserNodes_OtherLpu: VirtualNodes_RootsList
    {
        /// <summary>КОНСТРУКТОР</summary>
        public UserNodes_OtherLpu()
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

            // Если ЛПУ Кардиодиспансер (554505), то смотрим шаблон 3, иначе химия и шаблон 2
            var _k = _p.Where(p => p.PROP_TipProtokol.PROP_TipDocum == eTipDocum.Kdl 
                    && ((MyGlo.Lpu != 554505 && p.PROP_NumShablon == 2) 
                        || (MyGlo.Lpu == 554505 && p.PROP_NumShablon == 3)) 
                    && p.PROP_KL == MyGlo.KL).ToList()
                .OrderBy(p => p.PROP_pDate).ThenBy(p => p.PROP_Cod);

            return _k;
        }

        ///<summary>МЕТОД Создание подветки</summary>
        protected override UserNodes_Add MET_TypeNodesAdd()
        {
            return new UserNodes_Add(); 
        }
    }
}
