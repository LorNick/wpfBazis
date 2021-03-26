using System.Windows.Documents;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС Поле отчетов для 13 - Рисунок (для печати)</summary>
    public class UserPoleOtchet_Image : VirtualPoleOtchet
    {
        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            MET_Otvet();
            MET_LoadText();
            // Если поля нету, или признак - не показывать на печать данные то не печатаем его
            if (PROP_Otvet == "" || PROP_Format.MET_If("novis"))
                PROP_Hide = true;
        }

        /// <summary>МЕТОД Выводим в отчет</summary>
        public override TextElement MET_Print()
        {
           // Рзамеры рисунков
            int _Zoom = PROP_Format.MET_If("x2") ? 2 : 1;
            Paragraph _Paragraph = new Paragraph();
            _Paragraph.Inlines.Add(new LineBreak());
            _Paragraph.LineHeight = 5;
            UserPole_Image _Image = new UserPole_Image(PROP_Otvet, PROP_VarID, PROP_Shablon.PROP_ID, _Zoom, PROP_Format);
            _Paragraph.Inlines.Add(_Image);
            return _Paragraph;
        }
    }
}
