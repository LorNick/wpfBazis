using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfStatic;
using Label = System.Windows.Controls.Label;
using Orientation = System.Windows.Controls.Orientation;


namespace wpfMViewer.UserOtchet
{
    /// <summary>КЛАСС Отчет Истории болезни (для типа Inform)</summary>
    public class UserOtcher_Phone : VirtualOtchet
    {
        ///<summary>Наша таблица</summary>
        private volatile Table PRI_Table;

        private volatile string PRI_Filtr1;
        private volatile string PRI_Filtr2;
        private volatile string PRO_FiltrOtd = "";
        private DispatcherTimer PRI_Timer;
     
        private volatile TextBox PRI_TextBoxFilter1 = new TextBox();
        private volatile TextBox PRI_TextBoxFilter2 = new TextBox();
    
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

        /// <summary>МЕТОД Формируем отчет Телефонов</summary>
        protected override void MET_Otchet()
        {
            PRO_Paragraph = new Paragraph();
            
            // Добавляем подразделение
            StackPanel _Panel = new StackPanel();
            _Panel.Orientation = Orientation.Horizontal;
            var _P = new Paragraph();
            _P.Inlines.Add(_Panel);
            Blocks.Add(_P);

            Label _LabelOtd = new Label();
            _LabelOtd.Content = "Подразделения:";
            _LabelOtd.Foreground = Brushes.Navy;
            _Panel.Children.Add(_LabelOtd);
            // Отделения
            ComboBox _ComboBoxOtd = new ComboBox();
            _ComboBoxOtd.Width = 280;
            MySql.MET_DsAdapterFill(MyQuery.MET_varPhone_Select_2(), "Divisions");
            _ComboBoxOtd.ItemsSource = new DataView(MyGlo.DataSet.Tables["Divisions"]);
            _ComboBoxOtd.DisplayMemberPath = "Name";
            _ComboBoxOtd.SelectedValuePath = "Name";
          //  _ComboBoxOtd.IsEnabled = false;
          //  _ComboBoxOtd.SelectedValue = '2018 Аппарат управления";
            _ComboBoxOtd.SelectionChanged += delegate { PRO_FiltrOtd = (string)_ComboBoxOtd.SelectedValue ?? ""; PRI_Timer.Start(); };
            _Panel.Children.Add(_ComboBoxOtd);
            // Check Отделения
            CheckBox _CheckBoxOtd = new CheckBox();
            _CheckBoxOtd.Margin = new Thickness(10, 0, 0, 0);
            _CheckBoxOtd.Content = "учитывать подразделение";
            _CheckBoxOtd.VerticalAlignment = VerticalAlignment.Center;
            _CheckBoxOtd.Foreground = Brushes.Navy;
            _CheckBoxOtd.IsChecked = true;
            _CheckBoxOtd.Click += delegate 
                {
                    if (_CheckBoxOtd.IsChecked == false)
                    {
                        _ComboBoxOtd.IsEnabled = false;
                        PRO_FiltrOtd = "";
                    }
                    else
                    {
                        _ComboBoxOtd.IsEnabled = true;
                        PRO_FiltrOtd = (string)_ComboBoxOtd.SelectedValue ?? "";
                    }                    
                    PRI_Timer.Start();
                };
            _Panel.Children.Add(_CheckBoxOtd);

            // Добавляем строки поиска
            _Panel = new StackPanel();
            _Panel.Orientation = Orientation.Horizontal;
            _P = new Paragraph();
            _P.Inlines.Add(_Panel);
            Blocks.Add(_P);

            Label _LabelFiltr1 = new Label();
            _LabelFiltr1.Content = "Поиск по (любой текст):";
            _LabelFiltr1.VerticalAlignment = VerticalAlignment.Center;
            _LabelFiltr1.Background = Brushes.Gold;
            _Panel.Children.Add(_LabelFiltr1);

            PRI_TextBoxFilter1.MinWidth = 120;
            PRI_TextBoxFilter1.SelectionChanged += PART_TextBoxSelectionChanged;
            _Panel.Children.Add(PRI_TextBoxFilter1);

            Label _LabelFiltr2 = new Label();
            _LabelFiltr2.Content = "дополнительный поиск:";
            _LabelFiltr2.VerticalAlignment = VerticalAlignment.Center;
            _LabelFiltr2.Background = Brushes.LightCoral;
            _LabelFiltr2.Margin = new Thickness(10, 0, 0, 0);
            _Panel.Children.Add(_LabelFiltr2);

            PRI_TextBoxFilter2.MinWidth = 120;
            PRI_TextBoxFilter2.SelectionChanged += PART_TextBoxSelectionChanged;
            _Panel.Children.Add(PRI_TextBoxFilter2);


            PRI_Timer = new DispatcherTimer();
            PRI_Timer.Interval = new TimeSpan(0, 0, 1);
            PRI_Timer.Tick += PART_TimerOnTick; 
            
            // Запрос таблицы Телефонов 
            MySql.MET_DsAdapterFill(MyQuery.MET_varPhone_Select_1(), "Phone");

            // Формируем таблицу
            PRI_Table = new Table { CellSpacing = 0, Margin = new Thickness(0) };
            
            // Колонки
            for (int x = 0; x < 7; x++)
            {
                TableColumn _TableColumn = new TableColumn { Width = new GridLength(1, GridUnitType.Star) };

                PRI_Table.Columns.Add(_TableColumn);
            }
            PRI_Table.Columns[0].Width = new GridLength(2, GridUnitType.Star);
            PRI_Table.Columns[1].Width = new GridLength(1.8, GridUnitType.Star);
            PRI_Table.Columns[2].Width = new GridLength(1.8, GridUnitType.Star);
            PRI_Table.Columns[3].Width = new GridLength(0.6, GridUnitType.Star);
            Blocks.Add(PRI_Table);

            PRI_Table.RowGroups.Add(new TableRowGroup());

            PRI_Table.RowGroups[0].Rows.Add(new TableRow());

            TableCell _Cell = new TableCell(new Paragraph(new Run("ФИО")));
            PRI_Table.RowGroups[0].Rows[0].Cells.Add(_Cell);
            _Cell = new TableCell(new Paragraph(new Run("Должность")));
            PRI_Table.RowGroups[0].Rows[0].Cells.Add(_Cell);
            _Cell = new TableCell(new Paragraph(new Run("Место")));
            PRI_Table.RowGroups[0].Rows[0].Cells.Add(_Cell);
            _Cell = new TableCell(new Paragraph(new Run("Мест")));
            PRI_Table.RowGroups[0].Rows[0].Cells.Add(_Cell);
            _Cell = new TableCell(new Paragraph(new Run("Город")));
            PRI_Table.RowGroups[0].Rows[0].Cells.Add(_Cell);
            _Cell = new TableCell(new Paragraph(new Run("IP")));
            PRI_Table.RowGroups[0].Rows[0].Cells.Add(_Cell);
            _Cell = new TableCell(new Paragraph(new Run("Сотовый")));
            PRI_Table.RowGroups[0].Rows[0].Cells.Add(_Cell);
            PRI_Table.RowGroups[0].FontWeight = FontWeights.Bold;
        }

        /// <summary>МЕТОД Срабатывание таймера и начало построения таблицы</summary>
        private void PART_TimerOnTick(object sender, EventArgs e)
        {
            PRI_Timer.Stop();
            
            TableRowGroup _Group = new TableRowGroup();

            PRI_Filtr1 = PRI_TextBoxFilter1.Text.Trim();
            PRI_Filtr2 = PRI_TextBoxFilter2.Text.Trim();
            string _Korpus = null;
            string _Division = null;
            int _i = 0;

            foreach (DataRow _Row in MyGlo.DataSet.Tables["Phone"].Rows)
            {
                if (PRO_FiltrOtd.Length == 0 && PRI_TextBoxFilter1.Text.Length < 2)
                    return;
                _Group.Rows.Add(new TableRow());

                PRO_RowShablon = _Row;
                string _Find = MET_PoleStr("Find");
                string _FindDivision = MET_PoleStr("Division");


                if ((_FindDivision == PRO_FiltrOtd  && PRI_Filtr1.Length == 0) ||
                    (_Find.ToLower().IndexOf(PRI_Filtr1.ToLower()) >= 0 &&
                        (PRI_Filtr2.Length == 0 || _Find.ToLower().IndexOf(PRI_Filtr2.ToLower()) >= 0) &&
                        (PRO_FiltrOtd.Length == 0 || _FindDivision == PRO_FiltrOtd))
                        )

                {
                    string _KorpusRow = MET_PoleStr("Korpus");
                    string _DivisionRow = MET_PoleStr("Division");
                    TableCell _Cell;

                    // Оформляем заголовки (Корпус и Отделение)
                    if (!(_Korpus == _KorpusRow && _Division == _DivisionRow))
                    {
                        _Korpus = _KorpusRow;
                        _Division = _DivisionRow;

                        _Group.Rows.Add(new TableRow());

                        _Cell = new TableCell(MET_FindText("Korpus"));
                        _Group.Rows[_i].Cells.Add(_Cell);
                        _Cell.ColumnSpan = 2;
                        _Cell.Background = Brushes.LightBlue;
                        _Cell.TextAlignment = TextAlignment.Left;
                        _Cell = new TableCell(MET_FindText("Division"));
                        _Group.Rows[_i].Cells.Add(_Cell);
                        _Cell.ColumnSpan = 5;
                        _Cell.Background = Brushes.LightBlue;
                        _Cell.TextAlignment = TextAlignment.Left;

                        _i++;
                    }
                    _Group.Rows.Add(new TableRow());

                    _Cell = new TableCell(MET_FindText("FIO"));
                    _Group.Rows[_i].Cells.Add(_Cell);
                    _Cell.TextAlignment = TextAlignment.Left;
                    _Cell = new TableCell(MET_FindText("Post"));
                    _Group.Rows[_i].Cells.Add(_Cell);
                    _Cell.TextAlignment = TextAlignment.Left;
                    _Cell = new TableCell(MET_FindText("Cabinet"));
                    _Group.Rows[_i].Cells.Add(_Cell);
                    _Cell.TextAlignment = TextAlignment.Left;
                    _Cell = new TableCell(MET_FindText("LocalPhone"));
                    _Group.Rows[_i].Cells.Add(_Cell);
                    _Cell = new TableCell(MET_FindText("CityPhone"));
                    _Group.Rows[_i].Cells.Add(_Cell);
                    _Cell = new TableCell(MET_FindText("IPPhone"));
                    _Group.Rows[_i].Cells.Add(_Cell);
                    _Cell = new TableCell(MET_FindText("MobilPhone"));
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
        private Paragraph MET_FindText(string pPole)
        {
            Paragraph _Paragraph = new Paragraph();
            string _Text = MET_PoleStr(pPole);
            string _Fil1 = PRI_Filtr1.ToLower();
            string _Fil2 = PRI_Filtr2.ToLower();

            // Первый фильтр
            if (_Text.ToLower().IndexOf(_Fil1) >= 0)
            {
                int _Index = _Text.ToLower().IndexOf(_Fil1);
                MET_LastFindText(_Paragraph, _Text.Substring(0, _Index));
            
                Run _ChangeRun = new Run(_Text.Substring(_Index, _Fil1.Length));
                _ChangeRun.Background = Brushes.Gold;
                _Paragraph.Inlines.Add(_ChangeRun);

                MET_LastFindText(_Paragraph, _Text.Remove(0, _Index + _Fil1.Length));
            }
            // Второй фильтр
            else if (_Fil2.Length > 0 && _Text.ToLower().IndexOf(_Fil2) >= 0)
            {
                MET_LastFindText(_Paragraph, _Text);
            }
            // Не найдено совпадений с фильтрами
            else
            {
                _Paragraph.Inlines.Add(new Run(_Text));
            }
            return _Paragraph;
        }
        
        /// <summary>МЕТОД Ищем значение 2го фильтра</summary>
        /// <param name="pParagraph">Возвращаемый параграф</param>
        /// <param name="pText">Текст, в котором ищем 2й фильтр</param>
        private void MET_LastFindText(Paragraph pParagraph, string pText)
        {
            string _Fil2 = PRI_Filtr2.ToLower();
            if (_Fil2.Length > 0 && pText.ToLower().IndexOf(_Fil2) >= 0)
            {
                int _Index = pText.ToLower().IndexOf(_Fil2);
                Run _FistRun = new Run(pText.Substring(0, _Index));
                pParagraph.Inlines.Add(_FistRun);

                Run _ChangeRun = new Run(pText.Substring(_Index, _Fil2.Length));
                _ChangeRun.Background = Brushes.LightCoral;
                pParagraph.Inlines.Add(_ChangeRun);

                Run _LastRun = new Run(pText.Remove(0, _Index + _Fil2.Length));
                pParagraph.Inlines.Add(_LastRun);
            }
            // Не найдено совпадений с фильтрами
            else
            {
                pParagraph.Inlines.Add(new Run(pText));
            }
        }
        
        /// <summary>СОБЫТИЕ Поиск по фильтру при наборе символов</summary>
        private void PART_TextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            // Если ничего не менялось, то просто выходим (например при смене вкладок и возврате обратно на отчет)
            if (PRI_Filtr1 == PRI_TextBoxFilter1.Text.Trim() & PRI_Filtr2 == PRI_TextBoxFilter2.Text.Trim())
                return;

            if (PRI_TextBoxFilter1.Text.Length < 2)
            {
                // Удаляем строки если есть
                if (PRI_Table.RowGroups.Count > 1)
                    PRI_Table.RowGroups.RemoveAt(1);
                return;
            }

            PRI_Timer.Start();
        }
    }
}