using System.Windows.Controls;
using System.Windows.Documents;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;

namespace wpfMVrParacl
{
    /// <summary>КЛАСС Карта параклиники</summary>
    public class UserOtchet_KartParacl : VirtualOtchet
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
            PRO_Paragraph = new Paragraph();
            Frame _Frame = new Frame();
            UserPage_KartParacl _Page = new UserPage_KartParacl();              // создаем карту параклиники
            _Frame.Navigate(_Page);
            PRO_Paragraph.Inlines.Add(_Frame);
            Blocks.Clear();
            Blocks.Add(PRO_Paragraph);        
        }   
    }
}
