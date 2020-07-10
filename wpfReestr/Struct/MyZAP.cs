using System;

namespace wpfReestr
{
    /// <summary>КЛАСС Записи о законченных случаях оказания мед. помощи (в разработке)</summary>
    /// <remarks>Справочник Q018, тип файла C, H, T, путь ZL_LIST/ZAP</remarks>
    public class MyZAP
    {
        /// <summary>Номер записи (из StrahReestr)</summary>
        public decimal N_ZAP { get; set; }

        /// <summary>Признак исправленной записи (0 - передаются впревые, 1 - повторно после исправления; из StrahReestr)</summary>        
        public byte PR_NOV { get; set; } = 0;        

        /// <summary>Сведения о пациенте</summary>        
        public MyPACIENT PACIENT { get; set; }

        /// <summary>Сведения о законченном случае</summary>        
        public MyZ_SL Z_SL { get; set; }
    }
}
