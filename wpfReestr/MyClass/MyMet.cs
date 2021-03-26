using System;
using System.Data;
using System.Globalization;
using e = Microsoft.Office.Interop.Excel;

namespace wpfReestr
{
    /// <summary>КЛАСС работы с разными Методами</summary>
    public static class MyMet
    {
        #region ---- Выбор данных из DataRow ----
        /// <summary>МЕТОД Возвращаем строку</summary>
        public static string MET_PoleStr(string pPole, DataRow _Row)
        {
            return Convert.ToString(_Row[pPole]);
        }

        /// <summary>МЕТОД Возвращаем строку c Датой (01.01.2015 г.)</summary>
        public static string MET_PoleDat(string pPole, DataRow _Row)
        {
            return MET_StrDat(_Row[pPole]);
        }

        /// <summary>МЕТОД Возвращаем строку c Датой</summary>
        public static DateTime? MET_PoleDate(string pPole, DataRow _Row)
        {
            DateTime.TryParse(_Row[pPole].ToString(), out DateTime _Result);
            if (_Result == DateTime.MinValue)
                return null;
            else
                return _Result;
        }

        /// <summary>МЕТОД Возвращаем строку c Целым числом</summary>
        public static int MET_PoleInt(string pPole, DataRow _Row)
        {
            return MET_ParseInt(_Row[pPole].ToString());
        }

        /// <summary>МЕТОД Возвращаем строку c Десятичным числом</summary>
        public static decimal MET_PoleDec(string pPole, DataRow _Row)
        {
            return MET_ParseDec(_Row[pPole].ToString());
        }

        /// <summary>МЕТОД Возвращаем строку c Реальным числом</summary>
        public static double MET_PoleRea(string pPole, DataRow _Row)
        {
            return MET_ParseRea(_Row[pPole]);
        }
        #endregion

        #region ---- Методы преобразования данных ----
        /// <summary>МЕТОД Парсим строку в целое число</summary>
        /// <param name="pStr">Строка с числом</param>
        /// <returns>В случае ошибки возвращаем 0</returns>
        public static int MET_ParseInt(string pStr)
        {
            int.TryParse(pStr, out int _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим объект строки в целое число</summary>
        /// <param name="pObject">Строка с числом</param>
        /// <returns>В случае ошибки возвращаем 0</returns>
        public static int MET_ParseInt(object pObject)
        {
            int.TryParse(pObject.ToString(), out int _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим строку в десятичное число</summary>
        /// <param name="pStr">Строка с числом</param>
        /// <returns>В случае ошибки возвращаем 0</returns>
        public static decimal MET_ParseDec(string pStr)
        {
            decimal.TryParse(pStr, out decimal _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим объект строки в десятичное число</summary>
        /// <param name="pObject">Строка с числом</param>
        /// <returns>В случае ошибки возвращаем 0</returns>
        public static decimal MET_ParseDec(object pObject)
        {
            decimal.TryParse(pObject.ToString(), out decimal _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим строку в дату</summary>
        /// <param name="pStr">Строка с датой</param>
        /// <returns>В случае ошибки возвращаем Null</returns>
        public static DateTime? MET_ParseDat(string pStr)
        {
            DateTime.TryParse(pStr, out DateTime _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим объект строки в реальное число</summary>
        /// <param name="pObject">Строка с числом</param>
        /// <returns>В случае ошибки возвращаем 0</returns>
        public static double MET_ParseRea(object pObject)
        {
            double.TryParse(pObject.ToString(), out double _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим объект строки в дату</summary>
        /// <param name="pObject">Объект строки с датой</param>
        /// <returns>В случае ошибки возвращаем Null</returns>
        public static DateTime? MET_ParseDat(object pObject)
        {
            if (pObject.ToString() == "") return null;
            DateTime.TryParse(pObject.ToString(), out DateTime _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим объект даты в строку</summary>
        /// <param name="pObject">Объект с датой</param>
        /// <returns>В случае ошибки возвращаем пустую строку</returns>
        public static string MET_StrDat(object pObject)
        {
            string _Result;
            var _DateTime = MET_ParseDat(pObject);
            _Result = _DateTime == null ? "" : _DateTime.Value.ToShortDateString() + " г.";
            return _Result;
        }
        #endregion
    }

    //===============================================================================

    /// <summary>КЛАСС Формируем Excel файл с ошибками</summary>
    public class MyExcel1
    {
        /// <summary>Exel</summary>
        public readonly e.Application PUB_ExcelApp;

        /// <summary>Наша активная страница</summary>
        public readonly e.Worksheet PUB_Worksheet;

        /// <summary>Текущая строка в Excel</summary>
        public int PUB_Row = 3;

        /// <summary>КОНСТРУКТОР </summary>
        /// <param name="pSheet">Страниц в книге, по умолчанию 1</param>
        public MyExcel1(int pSheet = 1)
        {
            // Используя NuGet пакет LinqToExcel (https://code.google.com/p/linqtoexcel/) подключаемся к книге Excel
            // Работа с Excel http://wladm.narod.ru/C_Sharp/comexcel.html
            // Наш объект Excel
            PUB_ExcelApp = new e.Application();
            // Одна страница в книге
            PUB_ExcelApp.SheetsInNewWorkbook = pSheet;
            // Добавляем книгу
            PUB_ExcelApp.Workbooks.Add(Type.Missing);
            // Коллекция страниц
            e.Sheets _Sheets = PUB_ExcelApp.Workbooks.Item[1].Worksheets;
            // Рабочая страница
            PUB_Worksheet = (e.Worksheet)_Sheets.Item[1];
        }

        /// <summary>МЕТОД Закругляемся</summary>
        public bool MET_End()
        {
            // Если нет ошибок, то даже не открываем
            if (PUB_Row == 3)
            {
                PUB_ExcelApp.Workbooks[1].Close(false);
                PUB_ExcelApp.Quit();
                return false;
            }
            PUB_ExcelApp.Visible = true;
            return true;
        }

        /// <summary>МЕТОД Записи в Excel </summary>
        /// <param name="pCells">Кордината ячейки А1</param>
        /// <param name="pValue">Значение</param>
        public void MET_Print(string pCells, string pValue)
        {
            try
            {
                PUB_Worksheet.Range[pCells, Type.Missing].Formula = pValue;
            }
            catch (Exception)
            {
                //
            }
        }

        /// <summary>МЕТОД Записи в Excel для строки с номером PRI_Row</summary>
        /// <param name="pCells">Кордината ячейки А + PRI_Row</param>
        /// <param name="pValue">Значение</param>
        public void MET_PrintR(string pCells, string pValue)
        {
            try
            {
                PUB_Worksheet.Range[pCells + PUB_Row, Type.Missing].Formula = pValue;
            }
            catch (Exception)
            {
                //
            }
        }

        /// <summary>МЕТОД Ширина колонки</summary>
        /// <param name="pColumn">Кордината колонки А (одна буква)</param>
        /// <param name="pWidth">Ширина конолнки 1..255</param>
        public void MET_ColumnWidth(string pColumn, int pWidth)
        {
            if (pWidth > 255 || pColumn.Length > 1) return;
            try
            {
                ((e.Range)PUB_Worksheet.Columns[pColumn]).EntireColumn.ColumnWidth = pWidth;
            }
            catch (Exception)
            {
                //
            }
        }
    }

    //===============================================================================

    /// <summary>КЛАСС работы с Полями (столбцы)</summary>
    public class MyPole
    {
        #region ---- Поля ----
        /// <summary>Тип поля</summary>
        public string PUB_Type;
        /// <summary>Имя поля</summary>
        public string PUB_Name;
        /// <summary>Имя поля с квадратными скобками [ ]</summary>
        public string PUB_NameK;
        /// <summary>DataView</summary>
        public DataView PRU_DataView;
        /// <summary>Ошибка</summary>
        public bool PUB_Error;
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pNamePole">Имя поля</param>
        /// <param name="pDataView">Вьювер</param>
        public MyPole(string pNamePole, DataView pDataView)
        {
            PRU_DataView = pDataView;
            PUB_Name = pNamePole;                                               // имя колонки
            PUB_NameK = PUB_Name;                                               // имя колонки с квадратными скобками
            if (PUB_Name.IndexOf("[") > -1)
                PUB_Name = PUB_Name.Substring(1, PUB_Name.Length - 2);          // тут убираем скобки
            else
                PUB_NameK = "[" + PUB_Name + "]";                               // а тут их ставим
            try
            {
                // Находим тип колонки
                PUB_Type = pDataView.Table.Columns[PUB_Name].DataType.Name;
                PUB_Error = false;
            }
            catch
            {
                PUB_Error = true;
            }
        }

        /// <summary>МЕТОД Возвращаем точное условие фильтра (только равно)</summary>
        /// <param name="pWhere">Текст условия</param>
        public string MET_Filtr(string pWhere)
        {
            if (PUB_Error) return "";
            string _Where;                                                      // строка условия
            switch (PUB_Type)
            {
                case "Decimal":
                    decimal _Dec = Convert.ToDecimal(pWhere);
                    _Where = string.Format(CultureInfo.InvariantCulture, "{0} = {1}", PUB_NameK, _Dec);
                    break;
                case "Byte":
                case "Int32":                                                   // для чисел
                    _Where = PUB_NameK + " = " + pWhere;
                    break;
                default:                                                        // для текста и даты
                    _Where = PUB_NameK + " = '" + pWhere + "'";
                    break;
            }
            return _Where;
        }

        /// <summary>МЕТОД Возвращаем примерное условие фильтра (больше равно, либо Like)</summary>
        /// <param name="pWhere">Текст условия</param>
        public string MET_FiltrPr(string pWhere)
        {
            if (PUB_Error) return "";
            string _Where;                                                      // строка условия
            switch (PUB_Type)
            {
                case "Decimal":
                    decimal _Dec = Convert.ToDecimal(pWhere);
                    _Where = String.Format(CultureInfo.InvariantCulture, "{0} >= {1}", PUB_NameK, _Dec);
                    break;
                case "Byte":
                case "Int32":                                                   // для чисел
                    _Where = PUB_NameK + " >= " + pWhere;
                    break;
                case "DateTime":                                                // для дат
                    _Where = PUB_NameK + " >= '" + pWhere + "'";
                    break;
                default:                                                        // для текста
                    _Where = PUB_NameK + " like '" + pWhere + "%'";
                    break;
            }
            return _Where;
        }
    }
}
