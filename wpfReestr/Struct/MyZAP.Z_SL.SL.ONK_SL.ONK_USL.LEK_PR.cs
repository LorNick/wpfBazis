using System.Collections.Generic;

namespace wpfReestr
{
    /// <summary>КЛАСС Структура Химио-Лекарственных препаратов</summary>
    /// <remarks>Справочник Q018; тип файла C, T; путь ZL_LIST/ZAP/Z_SL/SL/ONK_SL/LEK_PR</remarks>
    public class MyLEK_PR
    {
        /// <summary>Код МНН (Справочник N020; из json NOM_USL)</summary>
        public string REGNUM { get; set; }

        /// <summary>Схема химии (Справочник N024; из json NOM_USL)</summary>
        public string CODE_SH { get; set; }

        /// <summary>Список дат введения ЛС (из json NOM_USL)</summary>
        public List<string> DATE_INJ { get; set; }
    }
}
