using System;
using System.Collections.Generic;

namespace wpfReestr
{
    /// <summary>КЛАСС Структура Химио-Лекарственных препаратов -> MyONK_USL</summary>
    public class MyLEK_PR
    {
        /// <summary>Код МНН (О,  Т(6), N020)</summary>
        public string REGNUM { get; set; }

        /// <summary>Схема химии (О,  Т(10), N024)</summary>
        public string CODE_SH { get; set; }

        /// <summary>Список дат введения ЛС (ОМ,  D)</summary>
        public List<string> DATE_INJ { get; set; }
    }
}
