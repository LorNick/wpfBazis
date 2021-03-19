using System;
using System.Data;
using System.Globalization;

namespace wpfStatic
{
    /// <summary>КЛАСС работы с Полями (столбцы) таблиц DataView</summary>
    public class MyColumn
    {
        #region ---- Свойства ----
        /// <summary>Тип поля (столбца)</summary>
        public string PROP_Type { get; private set; }
        /// <summary>Имя поля</summary>
        public string RPOP_Name { get; private set; }
        /// <summary>Имя поля с квадратными скобками [ ]</summary>
        public string RPOP_NameK { get; private set; }
        /// <summary>Виртуальная таблица DataView</summary>
        public DataView PROP_DataView { get; private set; }
        /// <summary>Есть ошибка - true, нету ошибки - false</summary>
        public bool PROP_Error { get; private set; }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pNamePole">Имя поля</param>
        /// <param name="pDataView">Виртуальная таблица</param>
        public MyColumn(string pNamePole, DataView pDataView)
        {
            PROP_DataView = pDataView;                                          // виртуальная таблица
            RPOP_Name = pNamePole;                                              // имя колонки
            RPOP_NameK = RPOP_Name;                                             // имя колонки с квадратными скобками
            if (RPOP_Name.Contains("["))
                RPOP_Name = RPOP_Name.Substring(1, RPOP_Name.Length - 2);       // тут убираем скобки
            else
                RPOP_NameK = "[" + RPOP_Name + "]";                             // а тут их ставим   
            try
            {
                // Находим тип колонки
                PROP_Type = pDataView.Table.Columns[RPOP_Name].DataType.Name;
                PROP_Error = false;
            }
            catch
            {
                // Ошибка! Не нашли тип
                PROP_Error = true;
            }
        }

        /// <summary>МЕТОД Возвращаем точное условие фильтра (только равно)</summary>
        /// <param name="pWhere">Текст условия</param>    
        public string MET_Filtr(string pWhere)
        {
            if (PROP_Error) return "";
            string _Where;                                                      // строка условия
            switch (PROP_Type)
            {
                case "Decimal":
                    decimal _Dec = Convert.ToDecimal(pWhere);
                    _Where = string.Format(CultureInfo.InvariantCulture, "{0} = {1}", RPOP_NameK, _Dec);
                    break;
                case "Byte":
                case "Int32":                                                   // для чисел
                    _Where = RPOP_NameK + " = " + pWhere;
                    break;
                default:                                                        // для текста и даты
                    _Where = RPOP_NameK + " = '" + pWhere + "'";
                    break;
            }
            return _Where;
        }

        /// <summary>МЕТОД Возвращаем примерное условие фильтра (больше равно, либо Like)</summary>
        /// <param name="pWhere">Текст условия</param>    
        public string MET_FiltrPr(string pWhere)
        {
            if (PROP_Error) return "";
            string _Where;                                                      // строка условия
            switch (PROP_Type)
            {
                case "Decimal":
                    decimal _Dec = Convert.ToDecimal(pWhere);
                    _Where = string.Format(CultureInfo.InvariantCulture, "{0} >= {1}", RPOP_NameK, _Dec);
                    break;
                case "Byte":
                case "Int32":                                                   // для чисел
                    _Where = RPOP_NameK + " >= " + pWhere;
                    break;
                case "DateTime":                                                // для дат
                    _Where = RPOP_NameK + " >= '" + pWhere + "'";
                    break;
                default:                                                        // для текста
                    _Where = RPOP_NameK + " like '" + pWhere + "%'";
                    break;
            }
            return _Where;
        }
    }
}
