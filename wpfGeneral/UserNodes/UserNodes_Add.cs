using wpfStatic;

namespace wpfGeneral.UserNodes
{
    /// <summary>КЛАСС Виртуальная ветка для Добавочных веток</summary>
    public class UserNodes_Add : VirtualNodes
    {
        /// <summary>Индекс ветки (только для eTipNodes.Add)</summary>
        private int PRI_Index;

        /// <summary>СВОЙСТВО (шаблона) Разрешение редактирования шаблона</summary>
        public override bool PROP_shaButtonEdit { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Разрешение создания шаблона</summary>
        public override bool PROP_shaButtonNew => false;

        /// <summary>СВОЙСТВО (шаблона) Индекс протокола</summary>
        public override int PROP_shaIndex
        {
            get { return PRI_Index; }
            set
            {
                PRI_Index = value;
                // Запоминает максимальный индекс
                if (PRI_Index > MyGlo.MaxImdex)
                    MyGlo.MaxImdex = PRI_Index;
            }
        }

        ///<summary>МЕТОД Инициализация ветки</summary>
        public override void MET_Inizial()
        {
            // Заполняем свойства базового класса
            base.MET_Inizial();

            // По умолчанию ветку можно редактировать
            PROP_shaButtonEdit = true;
            // Наличие протокола
            PROP_shaPresenceProtokol = true;
        }
    }
}
