using System.Windows.Documents;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС Поле отчетов для 8 - Текста (для печати)</summary>
    public class UserPoleOtchet_Text : VirtualPoleOtchet
    {
        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            MET_Otvet();
            MET_LoadText();
            // Если поля нету, то не печатаем его (если только принудительно не заставляем печатать вопрос)
            if (PROP_Otvet == "" & !PROP_Format.MET_If("disp"))
                PROP_Hide = true;
            // Если признак - не показывать на печать данные
            if (PROP_Format.MET_If("novis"))
                PROP_Hide = true;
        }

        /// <summary>МЕТОД Выводим в отчет</summary>
        public override TextElement MET_Print()
        {
            // Вопрос
            Run _OutText = new Run();
            if (PROP_OutText.Length > 0)
                _OutText = (PROP_Otvet + PROP_InText).Length > 0 ? new Run(PROP_OutText + "  ") : new Run(PROP_OutText);
            MET_FormatVopros(_OutText);
            // Ответ
            Run _InText = new Run(PROP_Otvet + PROP_InText);
            MET_FormatOtvet(_InText);
            // Вопрос + Ответ
            Span _Span = new Span();
            _Span.Inlines.Add(_OutText);
            _Span.Inlines.Add(_InText);
            TextElement _TextElement = MET_FormatVoprosOtvet(_Span);

            return _TextElement;
        }
    }
}