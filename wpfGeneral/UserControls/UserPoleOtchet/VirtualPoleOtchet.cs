using System;
using System.Windows;
using System.Windows.Documents;
using wpfGeneral.UserStruct;
using wpfGeneral.UserVariable;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС Виртуальное поле отчетов (для печати)></summary>
    public class VirtualPoleOtchet
    {
        #region ---- СВОЙСТВО ----
        /// <summary>СВОЙСТВО Имя</summary>
        public string PROP_Name { get; set; }

        /// <summary>СВОЙСТВО Запись шаблона</summary>
        public UserShablon PROP_Shablon { get; set; }

        /// <summary>СВОЙСТВО Запись протокола</summary>
        public UserProtokol PROP_Protokol { get; set; }

        /// <summary>СВОЙСТВО Строка протокола</summary>
        public string PROP_StrProtokol { get; set; }

        /// <summary>СВОЙСТВО Ответ из протокола</summary>
        public string PROP_Otvet { get; set; }

        /// <summary>СВОЙСТВО OutText</summary>
        public string PROP_OutText { get; set; }

        /// <summary>СВОЙСТВО InText</summary>
        public string PROP_InText { get; set; }

        /// <summary>СВОЙСТВО Префикс таблицы (ast..)</summary>
        public string PROP_Prefix { get; set; }

        /// <summary>СВОЙСТВО Формат поля</summary>
        public MyFormat PROP_Format { get; set; }

        /// <summary>СВОЙСТВО Тип вопроса</summary>
        public eVopros PROP_Type { get; set; }

        /// <summary>СВОЙСТВО VarID вопроса</summary>
        public int PROP_VarID { get; set; }

        /// <summary>СВОЙСТВО Маска вопроса</summary>
        public int PROP_Maska { get; set; }

        /// <summary>СВОЙСТВО Элемент скрытый не печатаем</summary>
        public bool PROP_Hide { get; set; }

        /// <summary>СВОЙСТВО Запрет детям печататься</summary>
        public bool PROP_HideChild { get; set; }

        /// <summary>СВОЙСТВО Может содержать вложенные поля</summary>
        public bool PROP_Nested { get; set; }

        /// <summary>СВОЙСТВО Родитель</summary>
        public VirtualPoleOtchet PROP_Parent { get; set; }
        #endregion

        ///<summary>КОНСТРУКТОР</summary>
        public VirtualPoleOtchet()
        {
            PROP_Format = new MyFormat();
        }

        /// <summary>МЕТОД Добавляем к полю дочерние элементы, если есть (пустой)</summary>
        /// <param name="pPole">Дочернее поле</param>
        public virtual bool MET_AddElement(VirtualPoleOtchet pPole) { return true; }

        /// <summary>МЕТОД Находим ответ из строки протокола</summary>
        public string MET_Otvet()
        {
            PROP_Otvet = MyMet.MET_GetPole(PROP_VarID, PROP_StrProtokol);
            return PROP_Otvet;
        }

        /// <summary>МЕТОД Выводим в отчет</summary>
        public virtual TextElement MET_Print() { return new Run(""); }

        ///<summary>МЕТОД Инициализация поля (пустой)</summary>
        public virtual void MET_Inicial() { }

        ///<summary>МЕТОД Формируем выходной текст</summary>
        protected void MET_LoadText()
        {
            UserVariable_Standart _Var = new UserVariable_Standart { PROP_Protokol = this.PROP_Protokol };
            PROP_OutText = _Var.MET_ReplacePole(PROP_Shablon.PROP_OutText, PROP_Shablon.PROP_ID, PROP_Prefix);
            PROP_InText = _Var.MET_ReplacePole(PROP_Shablon.PROP_InText, PROP_Shablon.PROP_ID, PROP_Prefix);
            if (PROP_Format.MET_If("hide") || PROP_Type == eVopros.Label)
                MET_LoadTextValueStart();
        }

        ///<summary>МЕТОД Формируем выходной текст поля ValueStart</summary>
        protected void MET_LoadTextValueStart()
        {
            UserVariable_Standart _Var = new UserVariable_Standart { PROP_Protokol = this.PROP_Protokol };
            PROP_Otvet = _Var.MET_ReplacePole(PROP_Shablon.PROP_ValueStart, PROP_Shablon.PROP_ID, PROP_Prefix);
        }

        ///<summary>МЕТОД Форматируем данные</summary>
        /// <param name="pInline">Элемент, который форматируем</param>
        protected TextElement MET_FormatVoprosOtvet(Inline pInline)
        {
            TextElement _TextElement = pInline;

            // Табуляция
            if (PROP_Format.MET_If("t"))
            {
                try
                {
                    int x = MyMet.MET_ParseInt(PROP_Format.PROP_Value["t"]);
                    do
                    {
                        (pInline as Span).Inlines.InsertBefore((pInline as Span).Inlines.FirstInline, new Run("\t"));
                        x--;
                    } while (x > 0);
                }
                catch
                {
                    (pInline as Span).Inlines.InsertBefore((pInline as Span).Inlines.FirstInline, new Run("\t"));
                }
            }

            // Пробел
            if (PROP_Format.MET_If("s"))
            {
                try
                {
                    int x = MyMet.MET_ParseInt(PROP_Format.PROP_Value["s"]);
                    do
                    {
                        (pInline as Span).Inlines.InsertBefore((pInline as Span).Inlines.FirstInline, new Run(" "));
                        x--;
                    } while (x > 0);
                }
                catch
                {
                    (pInline as Span).Inlines.InsertBefore((pInline as Span).Inlines.FirstInline, new Run(" "));
                }
            }

            bool _NewPage = PROP_Format.MET_If("f");
            bool _NewParagraf = PROP_Format.MET_If("p");
            bool _NewLine = PROP_Format.MET_If("n");

            if (_NewPage || _NewParagraf || _NewLine)
            {
                Paragraph _Paragraph = new Paragraph(pInline) {LineHeight = 1};
                // Новая страница
                if (_NewPage)
                    _Paragraph.BreakPageBefore = true;
                // Новый параграф
                if (_NewParagraf)
                {
                    Thickness _Thickness = new Thickness {Top = 5};
                    if ((string)PROP_Format.PROP_Value["p"] != "")
                    {
                        try
                        {
                            int x = Convert.ToInt16(PROP_Format.PROP_Value["p"]);
                            _Thickness.Top += x < 2 ? 0 : (x - 1) * 15;
                        }
                        catch { }
                    }
                    _Paragraph.Margin = _Thickness;
                }
                // Новая строка
                if (_NewLine)
                {
                    if ((string)PROP_Format.PROP_Value["n"] != "")
                    {
                        try
                        {
                            Thickness _Thickness = new Thickness();
                            int x = Convert.ToInt16(PROP_Format.PROP_Value["n"]);
                            _Thickness.Top = x < 2 ? 0 : (x - 1) * 12;
                            _Paragraph.Margin = _Thickness;
                        }
                        catch { }
                    }
                }
                if (PROP_Format.MET_If("ac"))
                    _Paragraph.TextAlignment = TextAlignment.Center;
                if (PROP_Format.MET_If("ar"))
                    _Paragraph.TextAlignment = TextAlignment.Right;
                _TextElement = _Paragraph;
            }
            return _TextElement;
        }

        ///<summary>МЕТОД Форматируем данные Вопроса (OutText)</summary>
        /// <param name="pInline">Элемент, который форматируем</param>
        protected void MET_FormatVopros(Inline pInline)
        {
            // Форматирование Персонально для вопроса
            bool _FontWeightBold = PROP_Format.MET_If("vsb");
            bool _FontStyleItalic = PROP_Format.MET_If("vsi");
            bool _FontStyleNormal = !_FontStyleItalic;
            bool _Underline = PROP_Format.MET_If("vsu");
            // Размер шрифта (15 - это 11 размер)
            int _FontSize = 15;
            if (PROP_Format.MET_If("vfx"))
            {
                try { _FontSize = Convert.ToInt16(PROP_Format.PROP_Value["vfx"]) + 4; }
                catch { }
            }

            // Общее форматирование
            _FontWeightBold = PROP_Format.MET_If("sb") | _FontWeightBold;
            _FontStyleItalic = PROP_Format.MET_If("si") | _FontStyleItalic;
            _FontStyleNormal = PROP_Format.MET_If("sn") | _FontStyleNormal;
            _Underline = PROP_Format.MET_If("su") | _Underline;
            // Размер шрифта
            if (PROP_Format.MET_If("fx"))
            {
                try { _FontSize = Convert.ToInt16(PROP_Format.PROP_Value["fx"]) + 4; }
                catch { }
            }
            // Применяем форматирование
            pInline.FontSize = _FontSize;
            if (_FontWeightBold)
                pInline.FontWeight = FontWeights.Bold;
            if (_FontStyleItalic)
                pInline.FontStyle = FontStyles.Italic;
            if (_FontStyleNormal)
                pInline.FontStyle = FontStyles.Normal;
            if (_Underline)
                pInline.TextDecorations = TextDecorations.Underline;
        }

        ///<summary>МЕТОД Форматируем данные Ответа (Ответ+InText)</summary>
        /// <param name="pInline">Элемент, который форматируем</param>
        protected void MET_FormatOtvet(Inline pInline)
        {
            // Форматирование Персонально для ответа
            bool _FontWeightBold = PROP_Format.MET_If("osb");
            bool _FontStyleNormal = PROP_Format.MET_If("osn");
            bool _FontStyleItalic = _FontStyleNormal;   //!_FontStyleNormal;
            bool _Underline = PROP_Format.MET_If("osu");
            // Размер шрифта (16 - это 12 размер)
            int _FontSize = 16;
            if (PROP_Format.PROP_Value.ContainsKey("ofx"))
            {
                try { _FontSize = Convert.ToInt16(PROP_Format.PROP_Value["ofx"]) + 4; }
                catch { }
            }
            // Общее форматирование
            _FontWeightBold = PROP_Format.MET_If("sb") | _FontWeightBold;
            _FontStyleItalic = PROP_Format.MET_If("si") | _FontStyleItalic;
            _FontStyleNormal = PROP_Format.MET_If("sn") | _FontStyleNormal;
            _Underline = PROP_Format.MET_If("su") | _Underline;
            // Размер шрифта
            if (PROP_Format.MET_If("fx"))
            {
                try { _FontSize = Convert.ToInt16(PROP_Format.PROP_Value["fx"]) + 4; }
                catch { }
            }
            // Применяем форматирование
            pInline.FontSize = _FontSize;
            if (_FontWeightBold)
                pInline.FontWeight = FontWeights.Bold;
            if (_FontStyleItalic)
                pInline.FontStyle = FontStyles.Italic;
            if (_FontStyleNormal)
                pInline.FontStyle = FontStyles.Normal;
            if (_Underline)
                pInline.TextDecorations = TextDecorations.Underline;
        }
    }
}

