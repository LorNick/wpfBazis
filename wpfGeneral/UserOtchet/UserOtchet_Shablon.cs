using System.Windows.Documents;

namespace wpfGeneral.UserOtchet
{
    /// <summary>КЛАСС Отчет Шаблона</summary>
    public class UserOtchet_Shablon : VirtualOtchet
    {
        ///<summary>МЕТОД Инициализация отчета</summary>
        public override VirtualOtchet MET_Inizial()
        {
            // Если нужно формировать отчет
            if (PROP_NewCreate)
            {
                base.MET_Inizial();
                // Чистим
                Blocks.Clear();
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
            PRO_Paragraph = new Paragraph();
            // Заполняем ответы
            MET_Protokol();
        }
    }
}
