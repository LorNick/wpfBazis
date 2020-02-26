using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Data;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfStatic;

namespace wpfMVrStac
{
    /// <summary>КЛАСС Отчет Назначений ЛС</summary>
    public class UserOtchet_NaznachMed : VirtualOtchet
    {   
        /// <summary>Текущая строка</summary>
        private TableRow PRI_Row;
        /// <summary>Номер Текущей строки</summary>
        private int PRI_RowNum;
        /// <summary>Таблица</summary>
        private Table PRI_Table;
        

        ///<summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            // Если НЕ нужно формировать отчет
            if (!PROP_NewCreate) return this;
            base.MET_Inizial(pNodes);
            // Чистим блок
            Blocks.Clear();
            // Формируем отчет 
            MET_Otchet();
            // Обнуляем параграф
            PRO_Paragraph = null;
            // Помечаем, что больше его формировать не надо
            PROP_NewCreate = false;
            return this;
        }

        /// <summary>МЕТОД Формируем отчет </summary>
        protected override void MET_Otchet()
        {     
            Paragraph _Parag;
            const string _TabName = "lnzVrachLS_Otchet";
            if (MyGlo.DataSet.Tables[_TabName] != null)
            {
                MyGlo.DataSet.Tables[_TabName].Reset();
            }
            DataTable _TableSQL;
            // Запрос
            MySql.MET_DsAdapterFill(MyQuery.lnzVrachLS_Select_1(MyGlo.IND), _TabName);
            _TableSQL = MyGlo.DataSet.Tables[_TabName];
            if (_TableSQL.Rows.Count == 0)
            {
                MET_NoOtchet();      // Отчет не заполен
                return;
            }
            // Находим максимальный день
            DateTime _DateN = (DateTime)MyGlo.HashAPSTAC["DN"];
            DateTime _DateK = (DateTime)_TableSQL.Compute("max(DateK)", "");            
            int _CountDay = (_DateK - _DateN).Days + 1;
            int _Ost;
            int _CountPage = Math.DivRem(_CountDay, 16, out _Ost);
            _CountPage = _Ost > 0 ? ++_CountPage : _CountPage ;

            // Находим аллергию из листа Осмотра при поступлении (если он есть)
            string _Allergia = MySql.MET_QueryStr(MyQuery.lnzVrachLS_Select_4(MyGlo.IND));

            // Рисуем страницы по 1му
            for (int _List = 1; _List <= _CountPage; _List++)
            {
                // Начинае новую страницу
                if (_List > 1)
                {
                    xPage = true;
                    MET_Print();
                }
                // Номер карты стационара, ФИО, Год рождения, Номер палаты
                xOtvet = "№ карты " + MyGlo.NSTAC.ToString() + "/________  " + MyGlo.FIO + ", " + MyGlo.DR.Substring(6) + "р.,  палата № ";
                string _Pal = MyGlo.HashAPSTAC["Number"].ToString();
                if (_Pal == "0")
                    xOtvet += "________";
                else
                    xOtvet += _Pal;
                xParagraph = true; xAligment = 2;
                MET_Print();
                // Лист врачебных назначений
                xOtvet = "ЛИСТ ВРАЧЕБНЫХ НАЗНАЧЕНИЙ № " + _List;
                xParagraph = true; xAligment = 2;
                MET_Print();
               
                // Находим аллергию из листа Осмотра при поступлении (если он есть)                
                if (_Allergia != "")
                {
                    xOtvet = _Allergia;
                    xVopr = "Аллергия:";
                    xParagraph = true;
                    MET_Print();
                }
                                
                xParagraph = true;
                MET_Print();

                PRI_Table = new Table();
                PRI_Table.CellSpacing = 0;
                PRI_Table.BorderBrush = Brushes.Black;
                PRI_Table.BorderThickness = new Thickness(0.4);

                // Создаем 20 столбцов 
                for (int x = 0; x < 20; x++)
                {
                    PRI_Table.Columns.Add(new TableColumn());
                }

                // Create and add an empty TableRowGroup to hold the table's Rows.
                PRI_Table.RowGroups.Add(new TableRowGroup());                 

                // Add the second (header) row.   
                PRI_Table.RowGroups[0].Rows.Add(new TableRow());  // 1 строка
                PRI_Table.RowGroups[0].Rows.Add(new TableRow());  // 2 строка - месяцы
                PRI_Table.RowGroups[0].Rows.Add(new TableRow());  // 3 строка - дни
                PRI_Table.RowGroups[0].Rows.Add(new TableRow());  // 4 строка - режим
                PRI_Table.RowGroups[0].Rows.Add(new TableRow());  // 5 строка - диета

                // ---- 1я строка
                PRI_RowNum = -1;
                MET_RowAdd();

                MET_CellsAdd("Назначение");
                PRI_Table.Columns[0].Width = new GridLength(150);
                PRI_Row.Cells[0].RowSpan = 3;
                MET_CellsAdd("Исполнитель");
                PRI_Table.Columns[1].Width = new GridLength(50);
                PRI_Row.Cells[1].RowSpan = 3;
                MET_CellsAdd("Отметки о назначении и выполнении");  
                PRI_Row.Cells[2].ColumnSpan = 17;

                // ---- 2 строка - месяц
                MET_RowAdd();

                MET_CellsAdd("дата");
                PRI_Row.Cells[0].RowSpan = 2;                 
                PRI_Table.Columns[2].Width = new GridLength(20);

                // Вставляем месяц
                TimeSpan _TS = new TimeSpan((_List - 1) * 16, 0, 0, 0);             // отступ от начальной даты
                DateTime _DN = _DateN + _TS;                                        // начальная дата
                int _EndDayMonth = DateTime.DaysInMonth(_DN.Year, _DN.Month);       // последний день месяца
                int _DNCount = _DN.Day + 15 > _EndDayMonth ? _EndDayMonth - _DN.Day + 1 : 16; // колличество дней в этом месяце
                string _MonthStr = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_DN.Month);
                if (_DNCount > 3) 
                    MET_CellsAdd(_MonthStr + " " + _DN.Year); 
                else
                    PRI_Row.Cells.Add(new TableCell()); 
                PRI_Row.Cells[1].ColumnSpan = _DNCount;
                // Если есть переход на новый месяц
                if (_DNCount < 16)
                {
                    DateTime _DK = _DN + new TimeSpan(_DNCount, 0, 0, 0);
                    _MonthStr = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_DK.Month);
                    if (16 - _DNCount > 3)
                        MET_CellsAdd(_MonthStr + " " + _DK.Year);
                    else
                        PRI_Row.Cells.Add(new TableCell()); 
                    PRI_Row.Cells[2].ColumnSpan = 16 - _DNCount;
                }   
                
                // ---- 3я строка - дни
                MET_RowAdd();
                for (int i = _DN.Day; i <= _DN.Day + _DNCount - 1; i++)
                {
                    TableCell _Cell = new TableCell(new Paragraph(new Run(i.ToString())));
                    PRI_Row.Cells.Add(_Cell);
                    int _DayOfWeek = (int)(_DN + new TimeSpan(i - _DN.Day, 0, 0, 0, 0)).DayOfWeek;
                    if (_DayOfWeek == 0 | _DayOfWeek == 6)
                        _Cell.Background = Brushes.LightCoral;
                }
                // Если есть переход на новый месяц
                if (_DNCount < 16)
                {
                    for (int i = 1; i <= 16 - _DNCount; i++)
                    {
                        TableCell _Cell = new TableCell(new Paragraph(new Run(i.ToString())));
                        PRI_Row.Cells.Add(_Cell);
                        int _DayOfWeek = (int)(_DN + new TimeSpan(_DNCount + i - 1, 0, 0, 0, 0)).DayOfWeek;
                        if (_DayOfWeek == 0 | _DayOfWeek == 6)
                            _Cell.Background = Brushes.LightCoral;
                    }
                }

                // ---- 4я строка- Режим
                MET_RowAdd();
                MET_CellsAdd("Режим");
                for (int i = 0; i < 17; i++)
                    PRI_Row.Cells.Add(new TableCell()); 
                PRI_Row.Cells[1].ColumnSpan = 2;
                
                // ---- 5 строка - Диета
                MET_RowAdd();
                MET_CellsAdd("Диета");
                for (int i = 0; i < 17; i++)
                    PRI_Row.Cells.Add(new TableCell());
                PRI_Row.Cells[1].ColumnSpan = 2;

                // Строки назначения
                string _Str = String.Format("'{0}' >= DateN and '{1}' <= DateK", _DN + new TimeSpan(15, 0, 0, 0), _DN);
                DataRow[] _mRow = _TableSQL.Select(_Str);

                int n = 0;
                foreach(DataRow row in _mRow)
                {                    
                    PRI_Table.RowGroups[0].Rows.Add(new TableRow());
                    PRI_Table.RowGroups[0].Rows.Add(new TableRow());
                    MET_RowAdd();
                    if (n % 2 == 0)
                        PRI_Row.Background = Brushes.GhostWhite;
                    // Наименование Назначения
                    _Parag = new Paragraph(new Run((string)row["NameLS"]));
                    // Способ приема и Примечание
                    string _PrimStr = (string)row["Route"] != "" ? ", " + (string)row["Route"] : "";
                    _PrimStr = (string)row["Note"] != "" ? _PrimStr + ", " + (string)row["Note"] : _PrimStr;      // примечание
                    _PrimStr = (bool)row["FlagPac"] ? _PrimStr + ", преп. пациента" : _PrimStr;                   // препарат пациента
                    if (_PrimStr != "")
                    {
                        Run _Prim = new Run(_PrimStr);
                        // Устанавливаем стиль для ответа
                        _Prim.FontStyle = FontStyles.Italic;
                        // Размер шрифта
                        _Prim.FontSize = 14;
                        _Parag.Inlines.Add(_Prim);
                    }
                    _Parag.TextAlignment = TextAlignment.Left;
                    PRI_Row.Cells.Add(new TableCell(_Parag));
                    PRI_Row.Cells[0].RowSpan = 2;
                    MET_CellsAdd("врач");  
                  
                    // Заполняем данные врача
                    // Находим разницу между начальной датой и датой назначения
                    int _ListDay = (_List - 1) * 16;
                    int _Dr1 = ((DateTime)row["DateN"] - _DN).Days;
                    int _Dr = _Dr1 + _ListDay;
                    int _Per = Convert.ToInt16(row["Period"]);
                    int _Kurs = Convert.ToInt16(row["Kurs"]) - 1;

                    for (int i = _ListDay; i < 16 + _ListDay; i++)
                    {
                        int _In = Math.DivRem(i - _Dr, _Per, out int _Os);
                        if (_Os == 0 && _In >= 0 && _In * _Per <= _Per * _Kurs) 
                            MET_CellsAdd(row["Amt"].ToString());
                        else
                            PRI_Row.Cells.Add(new TableCell());
                    }

                    PRI_Row.Cells[1].ColumnSpan = 2;
                    MET_RowAdd();
                    if (n % 2 == 0)
                        PRI_Row.Background = Brushes.GhostWhite; //Beige;
                    MET_CellsAdd("сестра");
                    for (int i = 0; i < 16; i++)
                        PRI_Row.Cells.Add(new TableCell()); 
                    PRI_Row.Cells[0].ColumnSpan = 2;
                 //   double _Height = PRI_Row.Cells[0].ElementStart.GetCharacterRect(LogicalDirection.Forward).Height;
                    n++;
                }

                for (; n < 16; n++)
                {
                    // Строки - подписи
                    PRI_Table.RowGroups[0].Rows.Add(new TableRow());
                    PRI_Table.RowGroups[0].Rows.Add(new TableRow());
                    MET_RowAdd();
                    if (n % 2 == 0)
                        PRI_Row.Background = Brushes.GhostWhite;
                    PRI_Row.Cells.Add(new TableCell()); 
                    PRI_Row.Cells[0].RowSpan = 2;
                    MET_CellsAdd("врач");
                    for (int i = 1; i < 17; i++)
                        PRI_Row.Cells.Add(new TableCell());
                    PRI_Row.Cells[1].ColumnSpan = 2;
                    MET_RowAdd();
                    if (n % 2 == 0)
                        PRI_Row.Background = Brushes.GhostWhite;
                    MET_CellsAdd("сестра");
                    for (int i = 1; i < 17; i++)
                        PRI_Row.Cells.Add(new TableCell()); 
                    PRI_Row.Cells[0].ColumnSpan = 2;
                }

                // Строки - подписи
                PRI_Table.RowGroups[0].Rows.Add(new TableRow());
                PRI_Table.RowGroups[0].Rows.Add(new TableRow());
                MET_RowAdd();
                MET_CellsAdd("Подписи");
                PRI_Row.Cells[0].RowSpan = 2;
                MET_CellsAdd("врач");
                for (int i = 1; i < 17; i++)
                    PRI_Row.Cells.Add(new TableCell()); 
                PRI_Row.Cells[1].ColumnSpan = 2;
                MET_RowAdd();
                MET_CellsAdd("сестра");                
                for (int i = 1; i < 17; i++)
                    PRI_Row.Cells.Add(new TableCell()); 
                PRI_Row.Cells[0].ColumnSpan = 2;
                Blocks.Add(PRI_Table);
            }
        }

        /// <summary>МЕТОД Добавляем в строку новую ячейку с текстом</summary>
        private void MET_CellsAdd(string pText)
        {
            PRI_Row.Cells.Add(new TableCell(new Paragraph(new Run(pText))));
        }

        /// <summary>МЕТОД Добавляем новую строку в таблицу</summary>
        private void MET_RowAdd()
        {
            PRI_Row = PRI_Table.RowGroups[0].Rows[++PRI_RowNum];
        }

        /// <summary>МЕТОД Показываем эмблему (вернее не показываем)</summary>
        protected override void MET_ImageEmblema(FlowDocument pFlowDocument) { }
    }
}
