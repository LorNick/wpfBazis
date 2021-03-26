using wpfGeneral.UserNodes;

namespace wpfGeneral.UserOtchet
{
    /// <summary>КЛАСС Отчет в разработке (для типа Inform)</summary>
    public class UserOtcher_InformCreate : VirtualOtchet
    {
        ///<summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            if (PROP_NewCreate)
            {
                base.MET_Inizial(pNodes);
                // Формируем отчет
                MET_Otchet();
                // Добавляем последний параграф в блок
                Blocks.Add(PRO_Paragraph);
                // Помечаем, что больше его формировать не надо
                PROP_NewCreate = false;
            }
            return this;
        }

        /// <summary>МЕТОД Формируем отчет</summary>
        protected override void MET_Otchet()
        {
            xVopr = "Отчет в разработке";
            xAligment = 2; xParagraph = true;
            MET_Print();
        }
    }
}
