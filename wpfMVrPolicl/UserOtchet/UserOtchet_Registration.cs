using System.Windows.Controls;
using System.Windows.Documents;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;

namespace wpfMVrPolicl
{
    /// <summary>КЛАСС Регистратура</summary>
    public class UserOtchet_Registration : VirtualOtchet
    {
        ///<summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            // Если нужно формировать отчет
            if (PROP_NewCreate)
            {
                MET_Otchet();                                                   
                PROP_NewCreate = false;
            }
            return this;
        }

        /// <summary>МЕТОД Формируем отчет</summary>
        protected override void MET_Otchet()
        {
            Frame _Frame = new Frame();                                         // фрейм, для  мед. назначений
            UserPage_Registration _Page = new UserPage_Registration();          // создаем лист регистратуры
            _Frame.Navigate(_Page);                                             // помещаем лист мед. назначений на фрейм
            PRO_Paragraph = new Paragraph();
            PRO_Paragraph.Inlines.Add(_Frame);
            Blocks.Clear();
            Blocks.Add(PRO_Paragraph);        
        }   
    }
}
