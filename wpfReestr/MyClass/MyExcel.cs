using System;
using System.Data;
using m = wpfReestr.MyMet;
using e = Microsoft.Office.Interop.Excel;
using wpfStatic;

namespace wpfReestr
{
    /// <summary>КЛАСС Формируем Excel файл с ошибками</summary>
    public class MyExcel
    {
        #region ---- Свойства ----
        /// <summary>СВОЙСТВО Строка реестра</summary>
        public StrahReestr PROP_StrahReestr { get; set; }

        /// <summary>СВОЙСТВО Строка предварительного реестра</summary>
        public DataRow PROP_RowReestr { get; set; }

        /// <summary>СВОЙСТВО Описание ошибки</summary>
        public int PROP_Nom { get; set; }

        /// <summary>СВОЙСТВО Тип ошибки (поликлиника/стационар)</summary>
        public string PROP_Tip { get; set; }

        /// <summary>СВОЙСТВО Код ошибки</summary>
        public string PROP_ErrorCod { get; set; }

        /// <summary>СВОЙСТВО Описание ошибки</summary>
        public string PROP_ErrorName { get; set; }
        #endregion

        private readonly StrahFile PRI_StrahFile;
        private readonly e.Application PRI_ExcelApp;
        private readonly e.Worksheet PRI_Worksheet;
        private int PRI_Row = 3;

        /// <summary>КОНСТРУКТОР </summary>
        /// <param name="pStrahFile">Строка файла реестра</param>
        public MyExcel(StrahFile pStrahFile)
        {
            // Используя NuGet пакет LinqToExcel (https://code.google.com/p/linqtoexcel/) подключаемся к книге Excel
            // Работа с Excel http://wladm.narod.ru/C_Sharp/comexcel.html
            PRI_StrahFile = pStrahFile;
            // Наш объект Excel
            PRI_ExcelApp = new e.Application();
            // Одна страница в книге
            PRI_ExcelApp.SheetsInNewWorkbook = 1;
            // Добавляем книгу
            PRI_ExcelApp.Workbooks.Add(Type.Missing);
            // Колллекция страниц
            e.Sheets _Sheets = PRI_ExcelApp.Workbooks.Item[1].Worksheets;
            // Рабочая страница
            PRI_Worksheet = (e.Worksheet)_Sheets.Item[1];
            // Заголовок
            string _Value = "Реестр №" + PRI_StrahFile.Cod;
            MET_Print("A1", _Value);
            // Шапка
            // 1
            MET_Print("A3", "Номер");
            MET_ColumnWidth("A", 6);
            // 2
            MET_Print("B3", "Тип");
            MET_ColumnWidth("B", 13);
            // 3
            MET_Print("C3", "Поступил");
            MET_ColumnWidth("C", 10);
            // 4
            MET_Print("D3", "ФИО");
            MET_ColumnWidth("D", 22);
            // 5
            MET_Print("E3", "Д.Р.");
            MET_ColumnWidth("E", 10);
            // 6
            MET_Print("F3", "Код ошибки");
            // 7
            MET_Print("G3", "Описание ошибки");
            MET_ColumnWidth("G", 80);
            // 8
            MET_Print("H3", "Код");
            MET_ColumnWidth("H", 16);
        }

        /// <summary>МЕТОД Проверка и запись ошибки реестра в Excel</summary>
        /// <param name="pVerif">Условие на наличие ошибки</param>
        /// <param name="pCod">Код ошибки</param>
        /// <param name="pName">Описание ошибки</param>
        /// <param name="pErrorRow">Сообщаем, что строка с ошибкой</param>
        /// <returns>true - есть ошибка, false - нет ошибки</returns>
        public bool MET_VeryfError(bool pVerif, string pCod, string pName, ref bool pErrorRow)
        {
            // Если условие ошибки true, то ошибка
            if (pVerif)
            {
                PROP_ErrorCod = pCod;
                PROP_ErrorName = pName;
                MET_SaveError();
                pErrorRow = true;
                return true;
            }
            // Ошибки нет
            return false;
        }

            /// <summary>МЕТОД Записываем ошибки реестра в Excel</summary>
            public void MET_SaveError()
        {
            PRI_Row++;
            MET_PrintR("A", PROP_Nom.ToString());
            MET_PrintR("B", PROP_Tip);
            var _DP = m.MET_PoleDate("DP", PROP_RowReestr);
            MET_PrintR("C", _DP?.ToShortDateString() ?? "");
            string _FAM = m.MET_PoleStr("FAM", PROP_RowReestr);
            MET_PrintR("D", _FAM);
            var _DR = m.MET_PoleDate("DR", PROP_RowReestr);
            MET_PrintR("E", _DR?.ToShortDateString() ?? "");
            MET_PrintR("F", PROP_ErrorCod);
            MET_PrintR("G", PROP_ErrorName);
            string _Cod = m.MET_PoleStr("Cod", PROP_RowReestr);
            MET_PrintR("H", _Cod);
        }

        /// <summary>МЕТОД Закругляемся</summary>
        public void MET_End()
        {
            // Если нет ошибок, то даже не открываем
            if (PRI_Row == 3)
            {
                PRI_ExcelApp.Workbooks[1].Close(false);
                PRI_ExcelApp.Quit();
                return;
            }
            // Заголовок
            string _Value = "Время создания: " + MySql.MET_QueryDat("select pDate from dbo.StrahFile where Cod =" + PRI_StrahFile.Cod);
            MET_Print("C1", _Value);
            PRI_ExcelApp.Visible = true;
        }

        /// <summary>МЕТОД Записи в Excel </summary>
        /// <param name="pCells">Кордината ячейки А1</param>
        /// <param name="pValue">Значение</param>
        private void MET_Print(string pCells, string pValue)
        {
            try
            {
                PRI_Worksheet.Range[pCells, Type.Missing].Formula = pValue;
            }
            catch (Exception)
            {
                //
            }
        }

        /// <summary>МЕТОД Записи в Excel для строки с номером PRI_Row</summary>
        /// <param name="pCells">Кордината ячейки А + PRI_Row</param>
        /// <param name="pValue">Значение</param>
        private void MET_PrintR(string pCells, string pValue)
        {
            try
            {
                PRI_Worksheet.Range[pCells + PRI_Row, Type.Missing].Formula = pValue;
            }
            catch (Exception)
            {
                //
            }
        }

        /// <summary>МЕТОД Ширина колонки</summary>
        /// <param name="pColumn">Кордината колонки А (одна буква)</param>
        /// <param name="pWidth">Ширина конолнки 1..255</param>
        private void MET_ColumnWidth(string pColumn, int pWidth)
        {
            if (pWidth > 255 || pColumn.Length > 1) return;
            try
            {
                ((e.Range)PRI_Worksheet.Columns[pColumn]).EntireColumn.ColumnWidth = pWidth;
            }
            catch (Exception)
            {
                //
            }
        }
    }
}
