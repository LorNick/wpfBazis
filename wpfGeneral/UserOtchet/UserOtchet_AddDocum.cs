using wpfGeneral.UserNodes;
using wpfGeneral.UserStruct;
using System.Windows.Documents;

namespace wpfGeneral.UserOtchet
{
    /// <summary>КЛАСС Отчет Шаблонов документов для таблиц kdl (для типа Add)</summary>
    public class UserOtchet_AddDocum : VirtualOtchet
    {
        ///<summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            // Если нужно формировать отчет
            if (PROP_NewCreate)
            {
                base.MET_Inizial(pNodes);
                PROP_Prefix = "kdl";
                // Ветка
                PROP_Nodes = pNodes;
                // Чистим блок
                Blocks.Clear();
                // Если есть заполенный протокол
                if (PROP_Nodes.PROP_shaPresenceProtokol)
                    MET_Otchet();                                               // Формируем отчет
                else
                    MET_NoOtchet();                                             // Отчет не заполен
                // Добавляем последний параграф в блок
                Blocks.Add(PRO_Paragraph);
                // Помечаем, что больше его формировать не надо
                PROP_NewCreate = false;
            }
            // Фон
            MET_Background();
            return this;
        }

        /// <summary>МЕТОД Формируем отчет</summary>
        protected override void MET_Otchet()
        {
            PROP_Docum.PROP_Protokol = UserProtokol.MET_FactoryProtokol(PROP_Docum.PROP_Protokol.PROP_TipProtokol.PROP_TipDocum,
                PROP_Docum.PROP_Protokol.PROP_Cod);
            PRO_Paragraph = new Paragraph();
            // Заполняем ответы
            MET_Protokol();
        }

    }
}
