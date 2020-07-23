using System.Windows.Documents;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС Поле отчетов для 9 - Раздел (для печати)</summary>
    public class UserPoleOtchet_Razdel : VirtualPoleOtchet
    {
        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            MET_Otvet();
            MET_LoadText();
            // Если поля нету или нету вопроса, или признак - не показывать на печать данные то не печатаем его
            if (PROP_Otvet != "Открыт" || PROP_OutText.Length == 0 || PROP_Format.MET_If("novis"))
            {
                PROP_Hide = true;
                PROP_HideChild = true;
            }
        }

        /// <summary>МЕТОД Выводим в отчет</summary>        
        public override TextElement MET_Print()
        {
            // В разделе печатаем только Вопрос
            Run _OutText = new Run(PROP_OutText);
            MET_FormatVopros(_OutText);

            Span _Span = new Span();
            _Span.Inlines.Add(_OutText);
            TextElement _TextElement = MET_FormatVoprosOtvet(_Span);

            return _TextElement;
        }
    }
}

