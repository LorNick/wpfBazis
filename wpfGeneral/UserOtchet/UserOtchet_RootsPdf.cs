using System.Windows.Media;
using wpfGeneral.UserControls;
using wpfStatic;

namespace wpfGeneral.UserOtchet
{
    /// <summary>КЛАСС Отчет для типа Roots</summary>
    public class UserOtchet_RootsPdf : UserOtchet_Roots
    {
        /// <summary>МЕТОД Настраиваем дополнительные параметры для подветки</summary>
        protected override void MET_PropertyPoleAdd(UserPole_History pPole)
        {
            pPole.PROP_Background = Brushes.Linen;
        }

        /// <summary>МЕТОД Открываем PDF файл при открытии экспандера</summary>
        /// <param name="pPole">Наше поле</param>
        public override void MET_OpenOtch(UserPole_History pPole)
        {
            MyGlo.Event_OpenPdfNode?.Invoke(pPole.PROP_Nodes);
            pPole.PART_Expander.IsExpanded = false; // и тут же закрываем, что бы можно было опять открыть
        }
    }
}