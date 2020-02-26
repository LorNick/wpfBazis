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
    public class UserNodes_RootNaprInOtherLpu: VirtualNodes_RootsList
    {
        /// <summary>КОНСТРУКТОР</summary>
        public UserNodes_RootNaprInOtherLpu()
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
            // Запрещаем содаздание документов
            PROP_shaButtonNew = false;

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

            // Если ЛПУ Кардиодиспансер (554505), то смотрим шаблон 9944, иначе химия и шаблон 9924 только для поликлиники
            var _k = _p.Where(p => ((MyGlo.Lpu != 554505 && p.PROP_TipProtokol.PROP_TipDocum == eTipDocum.Pol && p.PROP_NumShablon == 9924 ) 
                    || (MyGlo.Lpu == 554505 && p.PROP_NumShablon == 9944))
                   && p.PROP_KL == MyGlo.KL).ToList()
                .OrderBy(p => p.PROP_pDate).ThenBy(p => p.PROP_Cod);

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
            pNodes.PROP_shaButtonEdit = false;
        }
    }
}
