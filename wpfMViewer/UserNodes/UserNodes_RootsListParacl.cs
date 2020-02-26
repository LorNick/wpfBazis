//using wpfGeneral.UserNodes;
//using wpfGeneral.UserOtchet;
//using wpfGeneral.UserStruct;
//using wpfStatic;

//namespace wpfMViewer
//{
//    /// <summary>КЛАСС Ветка для Добавочных веток просмоторщика</summary>
//    /// <remarks>Начальные данные ветки, только для присвоения данных</remarks>
//    public class UserNodes_RootsListParacl : VirtualNodes_RootsList
//    {
//        /// <summary>КОНСТРУКТОР</summary>
//        public UserNodes_RootsListParacl()
//        {
//            PRO_TipNodeChild = eTipNodes.Para_Add;
//        }
//        ///<summary>МЕТОД Инициализация ветки</summary>
//        public override void MET_Inizial()
//        {
//            //  Тип протоколов
//            PROP_shaTipProtokol = new MyTipProtokol(eTipDocum.Paracl);
//            // Заполняем свойства базового класса
//            base.MET_Inizial();
//            // Запрещаем содаздание документов
//            PROP_shaButtonNew = false;
//        }

//    }
//}
