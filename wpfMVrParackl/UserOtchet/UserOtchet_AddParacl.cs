using System.Windows.Documents;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfMVrParacl
{
    /// <summary>КЛАСС Отчет Шаблонов Параклиники (для типа Add)</summary>
    public class UserOtchet_AddParacl : VirtualOtchet
    {
        ///<summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            // Если нужно формировать отчет
            if (PROP_NewCreate)
            {
                base.MET_Inizial(pNodes);
                // Ветка
                PROP_Nodes = pNodes;
                // Чистим блок
                Blocks.Clear();
                // Если есть заполенный протокол
                if (PROP_Nodes.PROP_shaPresenceProtokol)
                    MET_Otchet();        // Формируем отчет               
                else
                    MET_NoOtchet();      // Отчет не заполен

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
            PROP_Docum.PROP_Protokol = UserProtokol.MET_FactoryProtokol(PROP_Docum.PROP_ListShablon.PROP_TipProtokol.PROP_TipDocum,
                MyGlo.IND, PROP_Docum.PROP_ListShablon.PROP_Cod, PROP_Nodes.PROP_shaIndex);

            PRO_Paragraph = new Paragraph();
            // Заполняем ответы           
            MET_Protokol();
        }
    }
}
