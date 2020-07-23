using System.Windows.Documents;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС Поле отчетов для 15 - Метки (для печати)</summary>
    /// <remarks>Используем только поле ValueStart</remarks>
    public class UserPoleOtchet_Label : VirtualPoleOtchet
    {
        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            MET_LoadText();
            // Если признак - не показывать на печать данные (что странно для этого поля)
            if (PROP_Format.MET_If("novis"))
                PROP_Hide = true;
        }

        /// <summary>МЕТОД Выводим в отчет</summary>        
        public override TextElement MET_Print()
        {
            // Ответ
            Run _InText = new Run(PROP_Otvet);
            MET_FormatOtvet(_InText);
            Span _Span = new Span();
            _Span.Inlines.Add(_InText);
            TextElement _TextElement = MET_FormatVoprosOtvet(_Span);

            return _TextElement;
        }
    }
}

