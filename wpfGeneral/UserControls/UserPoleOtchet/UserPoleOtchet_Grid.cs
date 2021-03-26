using System;
using System.Windows;
using System.Windows.Documents;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС Поле отчетов для 14 - Grid (для печати)</summary>
    public class UserPoleOtchet_Grid : VirtualPoleOtchet
    {
        ///<summary>Наша таблица</summary>
        private Table PRI_Table;

        ///<summary>Скрываем отображение сетки</summary>
        private bool PRI_HideThick;

        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            // Может ли содержать внутри другие объекты или эта таблица нужна только для группировки формы
            PROP_Nested = PROP_Format.MET_If("gsr");
            // Если поля нету, или признак - не показывать на печать данные то не печатаем его
            if (!PROP_Nested || PROP_Format.MET_If("novis"))
                PROP_Hide = true;
            else
            {
                // Находим текст шапочки
                MET_LoadText();
                MET_LoadTextValueStart();
            }
        }

        /// <summary>МЕТОД Добавляем к полю дочерние элементы, если есть</summary>
        public override TextElement MET_Print()
        {
            // Выходная секция (заголовок + таблица)
            Section _Section = new Section();
            // Формируем таблицу
            int _Row;
            int _Column;
            // Строк
            try { _Row = Convert.ToInt16(PROP_Format.PROP_Value["gR"]); }
            catch { _Row = 1; }
            // Колонок
            try { _Column = Convert.ToInt16(PROP_Format.PROP_Value["gC"]); }
            catch { _Column = 1; }
            //  Нужно ли скрывать отображение сетки
            PRI_HideThick = PROP_Format.MET_If("hdt");
            // Формируем таблицу
            PRI_Table = new Table { CellSpacing = 0, Margin = new Thickness(0) };
            PRI_Table.RowGroups.Add(new TableRowGroup());
            // Колонки
            for (int x = 0; x < _Column; x++)
            {
                TableColumn _TableColumn = new TableColumn { Width = new GridLength(1, GridUnitType.Star) };
                PRI_Table.Columns.Add(_TableColumn);
            }
            // Строки
            for (int i = 0; i < _Row; i++)
            {
                PRI_Table.RowGroups[0].Rows.Add(new TableRow());
            }
            // Формируем верхнюю подпись таблци
            if (PROP_OutText.Length > 0 | PROP_Otvet.Length > 0 | PROP_InText.Length > 0)
            {
                // Вопрос
                Run _OutText = new Run();
                if (PROP_OutText.Length > 0)
                {
                    _OutText = (PROP_Otvet + PROP_InText).Length > 0 ? new Run(PROP_OutText + "  ") : new Run(PROP_OutText);
                }
                MET_FormatVopros(_OutText);
                // Ответ
                Run _InText = new Run(PROP_Otvet + PROP_InText);
                MET_FormatOtvet(_InText);
                // Вопрос + Ответ
                Span _Span = new Span();
                _Span.Inlines.Add(_OutText);
                _Span.Inlines.Add(_InText);
                TextElement _TextElement = MET_FormatVoprosOtvet(_Span);
                // Создаем параграф и вставляем текст
                Paragraph _Paragraph = new Paragraph();
                if (_TextElement is Paragraph)
                    _Paragraph = (Paragraph)_TextElement;
                else
                    _Paragraph.Inlines.Add((Inline)_TextElement);
                _Section.Blocks.Add(_Paragraph);
            }
            // Добавляем в секцию таблицу
            _Section.Blocks.Add(PRI_Table);
            return _Section;
        }

        /// <summary>МЕТОД Добавляем к полю дочерние элементы, если есть</summary>
        /// <param name="pPole">Дочернее поле</param>
        public override bool MET_AddElement(VirtualPoleOtchet pPole)
        {
            int _Row;
            int _Column;
            int _RowSpan;
            int _ColumnSpan;
            double _With;
            TableCell _Cell;

            // Строка
            try { _Row = Convert.ToInt16(pPole.PROP_Format.PROP_Value["gRo"]); }
            catch { _Row = 0; }
            // Колонка
            try { _Column = Convert.ToInt16(pPole.PROP_Format.PROP_Value["gCo"]); }
            catch { _Column = 0; }
            // Ширина колонки
            try { _With = Convert.ToDouble(pPole.PROP_Format.PROP_Value["gw"]); }
            catch { _With = 0; }
            // Создаем ячейку
            TextElement _Element = pPole.MET_Print();
            if (_Element is Block)
            {
                _Cell = new TableCell((Block)_Element);
            }
            else
            {
                _Cell = new TableCell(new Paragraph((Inline)_Element));
            }
            // Скрываем отображение сетки, если нужно - hdt (hide thickness)
            if (PRI_HideThick)
                _Cell.BorderThickness = new Thickness(0);
            _Cell.TextAlignment = TextAlignment.Left;
            if (pPole.PROP_Format.MET_If("ac"))
                _Cell.TextAlignment = TextAlignment.Center;
            if (pPole.PROP_Format.MET_If("ar"))
                _Cell.TextAlignment = TextAlignment.Right;
            // Добавляем ячейку
            PRI_Table.RowGroups[0].Rows[_Row].Cells.Add(_Cell);
            // Объединение по Строкам
            try { _RowSpan = Convert.ToUInt16(pPole.PROP_Format.PROP_Value["gRs"]); }
            catch { _RowSpan = 0; }
            if (_RowSpan > 0)
                _Cell.RowSpan = _RowSpan;
            // Объединение по Колонокам
            try { _ColumnSpan = Convert.ToUInt16(pPole.PROP_Format.PROP_Value["gCs"]); }
            catch { _ColumnSpan = 0; }
            if (_ColumnSpan > 0)
                _Cell.ColumnSpan = _ColumnSpan;
            // Если надо поменять ширину колонки
            if (_With > 0)
                PRI_Table.Columns[_Column].Width = new GridLength(_With, GridUnitType.Star);

            return true;
        }
    }
}

