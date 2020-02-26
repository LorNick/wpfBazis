using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfStatic;

namespace wpfMVrStac
{
    /// <summary>КЛАСС Отчет Назначений ЛС</summary>
    public class UserOtchet_NaznachBlanck : VirtualOtchet
    {
        #region  ---- Закрытые Поля ----
        /// <summary>Текущая строка</summary>
        private TableRow PRI_Row;
        /// <summary>Номер Текущей строки</summary>
        private int PRI_RowNum;
        /// <summary>Таблица</summary>
        private Table PRI_Table;
        #endregion

        ///<summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            // Если нужно формировать отчет
            if (PROP_NewCreate)
            {
                base.MET_Inizial(pNodes);
                // Чистим блок
                Blocks.Clear();
                // Формируем отчет 
                MET_Otchet();                       
                // Добавляем последний параграф в блок
                Blocks.Add(PRO_Paragraph);
                // Помечаем, что больше его формировать не надо
                PROP_NewCreate = false;
            }
            return this;
        }

        /// <summary>МЕТОД Формируем отчет </summary>
        protected override void MET_Otchet()
        {
            // Находим максимальный день
            DateTime _DateN = DateTime.Today;
            // Номер карты стационара, ФИО, Год рождения, Номер палаты
            xOtvet = "№ карты " + MyGlo.NSTAC + "/________  " + MyGlo.FIO + ", " + MyGlo.DR.Substring(6) + "р.,  палата № ";
            string _Pal = MyGlo.HashAPSTAC["Number"].ToString();
            if (_Pal == "0")
                xOtvet += "________";
            else
                xOtvet += _Pal;
            xParagraph = true; xAligment = 2;
            MET_Print();
            // Лист врачебных назначений
            xOtvet = "ЛИСТ ВРАЧЕБНЫХ НАЗНАЧЕНИЙ № _____ ";
            xParagraph = true; xAligment = 2;
            MET_Print();

            // Находим аллергию из листа Осмотра при поступлении (если он есть)
            string _Allergia = MySql.MET_QueryStr(MyQuery.lnzVrachLS_Select_4(MyGlo.IND));
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
            DateTime _DN = _DateN;                                                              // начальная дата
            int _EndDayMonth = DateTime.DaysInMonth(_DN.Year, _DN.Month);                       // последний день месяца
            int _DNCount = _DN.Day + 15 > _EndDayMonth ? _EndDayMonth - _DN.Day + 1 : 16;       // колличество дней в этом месяце
            string _MonthStr = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_DN.Month);
            if (_DNCount > 3) 
                MET_CellsAdd(_MonthStr + " " + _DN.Year.ToString()); 
            else
                PRI_Row.Cells.Add(new TableCell()); 
            PRI_Row.Cells[1].ColumnSpan = _DNCount;
            // Если есть переход на новый месяц
            if (_DNCount < 16)
            {
                DateTime _DK = _DN + new TimeSpan(_DNCount, 0, 0, 0);
                _MonthStr = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(_DK.Month);
                if (16 - _DNCount > 3)
                    MET_CellsAdd(_MonthStr + " " + _DK.Year.ToString());
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
            int n = 0;
            for (; n < 20; n++)
            {
                // Строки - подписи
                PRI_Table.RowGroups[0].Rows.Add(new TableRow());
                PRI_Table.RowGroups[0].Rows.Add(new TableRow());
                MET_RowAdd();
                if (n % 2 == 0)
                    PRI_Row.Background = Brushes.Beige;
                PRI_Row.Cells.Add(new TableCell()); 
                PRI_Row.Cells[0].RowSpan = 2;
                MET_CellsAdd("врач");
                for (int i = 1; i < 17; i++)
                    PRI_Row.Cells.Add(new TableCell());
                PRI_Row.Cells[1].ColumnSpan = 2;
                MET_RowAdd();
                if (n % 2 == 0)
                    PRI_Row.Background = Brushes.Beige;
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

