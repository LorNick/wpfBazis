using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfStatic;
using Label = System.Windows.Controls.Label;
using Orientation = System.Windows.Controls.Orientation;

namespace wpfMViewer.UserOtchet
{
    /// <summary>КЛАСС Отчет Сотрудники (для типа Inform)</summary>
    public class UserOtcher_Staff: VirtualOtchet
    {
        ///<summary>Наша таблица</summary>
        private volatile Table PRI_Table;
        private volatile string PRI_Filtr1;
        private volatile RadWatermarkTextBox PRI_TextBoxFilter1 = new RadWatermarkTextBox();
        private List<string> PRI_FilterList = new List<string>();

        ///<summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            // Ставим Русский язык
            MyMet.MET_Lаng();

            // Если нужно формировать отчет
            if (PROP_NewCreate)
            {
                base.MET_Inizial(pNodes);
                // Формируем отчет
                MET_Otchet();
                // Добавляем последний параграф в блок
                Blocks.Add(PRO_Paragraph);
                // Помечаем, что больше его формировать не надо
                PROP_NewCreate = false;
            }
            return this;
        }

        /// <summary>МЕТОД Формируем отчет Сотрудников</summary>
        protected override void MET_Otchet()
        {
            PRO_Paragraph = new Paragraph();
            StackPanel _Panel = new StackPanel();
            _Panel.Orientation = Orientation.Horizontal;
            var _P = new Paragraph();
            _P.Inlines.Add(_Panel);
            Blocks.Add(_P);

            Label _LabelFiltr1 = new Label();
            _LabelFiltr1.Content = "Поиск: (нажмите на Enter)";
            _LabelFiltr1.VerticalAlignment = VerticalAlignment.Center;
            _LabelFiltr1.Background = Brushes.Gold;
            _Panel.Children.Add(_LabelFiltr1);            
            PRI_TextBoxFilter1.WatermarkContent = "Наберите что нибудь через пробел и нажмите на Enter";
            PRI_TextBoxFilter1.KeyDown += PART_TextBoxFilterKeyDown;
            _Panel.Children.Add(PRI_TextBoxFilter1);


            // Запрос таблицы Сотрудников
            MySql.MET_DsAdapterFill(MyQuery.MET_varStaff_Select_1(), "Staff");

            // Формируем таблицу
            PRI_Table = new Table { CellSpacing = 0, Margin = new Thickness(0) };
            // Колонки
            for (int x = 0; x < 3; x++)
            {
                TableColumn _TableColumn = new TableColumn { Width = new GridLength(1, GridUnitType.Star) };
                PRI_Table.Columns.Add(_TableColumn);
            }
            PRI_Table.Columns[0].Width = new GridLength(1, GridUnitType.Star);
            PRI_Table.Columns[1].Width = new GridLength(4, GridUnitType.Star);
            Blocks.Add(PRI_Table);
            PRI_Table.RowGroups.Add(new TableRowGroup());
            PRI_Table.RowGroups[0].Rows.Add(new TableRow());
            TableCell _Cell = new TableCell(new Paragraph(new Run("ФИО")));
            PRI_Table.RowGroups[0].Rows[0].Cells.Add(_Cell);
            _Cell = new TableCell(new Paragraph(new Run("Описание")));
            PRI_Table.RowGroups[0].Rows[0].Cells.Add(_Cell);
            PRI_Table.RowGroups[0].FontWeight = FontWeights.Bold;
        }

        /// <summary>СОБЫТИЕ Поиск по фильтру при нажатии на Enter</summary>
        private void PART_TextBoxFilterKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;
            // Если ничего не менялось, то просто выходим
            if (PRI_Filtr1 == PRI_TextBoxFilter1.Text.Trim())
                return;
            PRI_Filtr1 = PRI_TextBoxFilter1.Text.Trim();
            MET_StartFind();
        }

        /// <summary>МЕТОД Стартуем поиск и формирование найденых данных</summary>
        private void MET_StartFind()        {          
            TableRowGroup _Group = new TableRowGroup();
            string _text = Regex.Replace(PRI_TextBoxFilter1.Text.ToLower().Trim(), "[ ]+", " ");  // удаляем 2е пробелы
            PRI_FilterList = _text.Split(' ').Distinct().ToList();           
            int _i = 0;
            foreach (DataRow _Row in MyGlo.DataSet.Tables["Staff"].Rows)
            {
                _Group.Rows.Add(new TableRow());
                PRO_RowShablon = _Row;
                string _Find = (MET_PoleStr("FIO") + '|' + MET_PoleStr("Tegs") + '|' + MET_PoleStr("Tips") + '|' + MET_PoleStr("isWork")).ToLower();
                if (PRI_FilterList.All(x => _Find.Contains(x)))
                {
                    Brush _Brush = Brushes.White;
                    switch (MET_PoleStr("Tips"))
                    {
                        case "2 - пользователи":
                            _Brush = Brushes.Beige;
                            break;
                        case "3 - врач поликлиники":
                            _Brush = Brushes.Azure;
                            break;
                        case "4 - врач стационара":
                            _Brush = Brushes.Linen;
                            break;
                        case "5 - медсёстры":
                            _Brush = Brushes.LightCyan;
                            break;
                    }
                    TableCell _Cell;
                    // ФИО
                    Paragraph _Paragraph = MET_ColorFindText("FIO");
                    if (MET_PoleStr("isWork") == "удален")
                        _Paragraph.TextDecorations = TextDecorations.Strikethrough;
                    _Cell = new TableCell(_Paragraph);
                    _Paragraph = MET_ColorFindText("Tips");  // тип записи
                    _Paragraph.FontSize = 12;
                    _Cell.Blocks.Add(_Paragraph);
                    _Cell.Background = _Brush;
                    _Group.Rows[_i].Cells.Add(_Cell);
                    // Теги
                    _Cell.TextAlignment = TextAlignment.Left;
                    _Cell = new TableCell(MET_ColorFindText("Tegs"));
                    _Cell.TextAlignment = TextAlignment.Left;
                    _Cell.FontSize = 14;
                    _Cell.Background = _Brush;
                    _Group.Rows[_i].Cells.Add(_Cell);
                    _i++;
                }
            }
            if (PRI_Table.RowGroups.Count > 1)
                PRI_Table.RowGroups.RemoveAt(1);
            PRI_Table.RowGroups.Add(_Group);
        }

        /// <summary>МЕТОД Разукрашиваем найденое поле</summary>
        /// <param name="pPole">Имя поля</param>
        private Paragraph MET_ColorFindText(string pPole)
        {
            Paragraph _Paragraph = new Paragraph();
            string _Text = MET_PoleStr(pPole);
            if (PRI_FilterList.Any(x => _Text.ToLower().Contains(x)))
            {
                // Находим все слова поиска, которые есть в текущем поле и записываем их в SortedList
                // если есть слова которые начинаются с одной и той же буквы, то они объединяются
                var keys = new SortedList<int, int>();
                foreach (string _strFilter in PRI_FilterList) 
                {
                    int x = 0;
                    while (_Text.ToLower().IndexOf(_strFilter, x) >= 0)
                    {                        
                        int _Index = _Text.ToLower().IndexOf(_strFilter, x);
                        x = _Index + _strFilter.Length;
                        if (keys.ContainsKey(_Index))
                        {
                            keys[_Index] = keys[_Index] > x ? keys[_Index] : x;
                        } else
                        {
                            keys.Add(_Index, x);
                        }
                    }
                }

                // Если слова пересекаются, то они тоже объединяются
                var keyArr = keys.Keys.ToArray();
                var valueArr = keys.Values.ToArray();               
                for (int i = 0; i < keyArr.Length - 1; i++)
                {
                    if (keyArr[i] > -1)
                    {
                        for (int j = i + 1; j < keyArr.Length; j++)
                        {
                            if (keyArr[j] > keyArr[i] && keyArr[j] <= valueArr[i])
                            {
                                valueArr[i] = valueArr[i] > valueArr[j] ? valueArr[i] : valueArr[j];
                                keyArr[j] = -1;
                            }
                        }
                    }
                }
               
                // Формируем поле
                int x1 = 0;
                for (int i = 0; i < keyArr.Length; i++)
                {
                    if (keyArr[i] > -1)
                    {
                        // До найденого текста
                        if (x1 < keyArr[i])
                        {
                            Run _FistRun = new Run(_Text.Substring(x1, keyArr[i] - x1));
                            _Paragraph.Inlines.Add(_FistRun);
                        }
                        // Найденый текст, который красим
                        Run _ChangeRun = new Run(_Text.Substring(keyArr[i], valueArr[i] - keyArr[i]));
                        _ChangeRun.Background = Brushes.Gold;
                        _Paragraph.Inlines.Add(_ChangeRun);
                        x1 = valueArr[i];
                    }
                }
                // После найденого текста и до конца
                if (x1 < _Text.Length)
                {
                    Run _FistRun = new Run(_Text.Substring(x1, _Text.Length - x1));
                    _Paragraph.Inlines.Add(_FistRun);
                }
            }
            // Не найдено совпадений с фильтрами
            else
            {
                _Paragraph.Inlines.Add(new Run(_Text));
            }
            return _Paragraph;
        }
    }
}