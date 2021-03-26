using System.Collections.Generic;
using System.Linq;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfMVrParacl
{
    /// <summary>КЛАСС Ветка для Добавочных веток параклиники</summary>
    /// <remarks>Начальные данные ветки, только для присвоения данных</remarks>
    public class UserNodes_RootsListParacl : VirtualNodes_RootsList
    {
        /// <summary>СВОЙСТВО (шаблона) Разрешение создания шаблона</summary>
        ///<remarks>Только для модуля параклиники</remarks>
        public override bool PROP_shaButtonNew { get { return MyGlo.TypeModul == eModul.VrPara; } }

        /// <summary>КОНСТРУКТОР</summary>
        public UserNodes_RootsListParacl()
        {
            PRO_TipNodeChild = eTipNodes.Para_Add;
        }
        ///<summary>МЕТОД Инициализация ветки</summary>
        public override void MET_Inizial()
        {
            //  Тип протоколов
            PROP_shaTipProtokol = new MyTipProtokol(eTipDocum.Paracl);
            // Заполняем свойства базового класса
            base.MET_Inizial();
        }

        ///<summary>МЕТОД Отчет подветки</summary>
        protected override VirtualOtchet MET_OtchetChild(UserDocument pDocument)
        {
            return new UserOtchet_AddParacl { PROP_Docum = pDocument };
        }

        ///<summary>МЕТОД Запрос на поиск подветок</summary>
        protected override IEnumerable<UserProtokol> MET_LoadProtokol()
        {
            return ((VirtualModul)MyGlo.Modul).PUB_Protokol.Where(p => p.PROP_TipProtokol.PROP_TipDocum == eTipDocum.Paracl 
                            && p.PROP_CodApstac == MyGlo.IND).OrderBy(p => p.PROP_Cod);
        }

        ///<summary>МЕТОД Создание подветки</summary>
        protected override UserNodes_Add MET_TypeNodesAdd()
        {
            return new UserNodes_Add();
        }
    }
}
