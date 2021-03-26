using System.Windows.Documents;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfMVrStac.UserOtchet
{
    /// <summary>КЛАСС Отчет Шаблонов Стационара (для типа EditStac)</summary>
    public class UserOtchet_EditVrStac : VirtualOtchet_Edit
    {
        ///<summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            // Если НЕ нужно формировать отчет
            if (!PROP_NewCreate) return this;
            base.MET_Inizial(pNodes);
            PROP_Prefix = "ast";
            // Ветка
            PROP_Nodes = pNodes;
            // Чистим блок
            Blocks.Clear();
            // Если есть заполенный протокол
            if (PROP_Nodes.PROP_shaPresenceProtokol)
                MET_Otchet();      // Формируем отчет
            else
                MET_NoOtchet();
            // Добавляем последний параграф в блок
            Blocks.Add(PRO_Paragraph);
            // Помечаем, что больше его формировать не надо
            PROP_NewCreate = false;
            return this;
        }

        /// <summary>МЕТОД Формируем отчет</summary>
        protected override void MET_Otchet()
        {
            // Протокол Шаблона
            PROP_Docum.PROP_Protokol = UserProtokol.MET_FactoryProtokol(PROP_Docum.PROP_TipDocum,
                MyGlo.IND, PROP_Docum.PROP_ListShablon.PROP_Cod, PROP_Nodes.PROP_shaIndex);
            PRO_Paragraph = new Paragraph();
            // Заполняем ответы
            MET_Protokol();
        }
    }
}
