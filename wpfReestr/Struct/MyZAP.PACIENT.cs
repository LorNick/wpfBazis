namespace wpfReestr
{
    /// <summary>КЛАСС Сведенния о пациенте (в разработке)</summary>
    /// <remarks>Справочник Q018; тип файла C, H, T; путь ZL_LIST/ZAP/PACIENT; только используемые поля</remarks>
    public class MyPACIENT
    {
        /// <summary>Код записи пациента (KL из kbol; из StrahReestr)</summary>
        public decimal ID_PAC { get; set; }

        /// <summary>Вид страхового полиса (Справочник F008: 1 - старый, 2 - временный, 3 - новый; из StrahReestr)</summary>
        public decimal VPOLIS { get; set; }

        /// <summary>Серия страхового полиса (только для старых полисов VPOLIS = 1; из StrahReestr поля SERIA)</summary>
        public string SPOLIS { get; set; }

        /// <summary>Номер страхового полиса (из StrahReestr поля NUMBER)</summary>
        public string NPOLIS { get; set; }

        /// <summary>OKATO Регион страхования (только для старых полисов VPOLIS = 1; из StrahReestr поля SMO_OK)</summary>
        public string ST_OKATO { get; set; }

        /// <summary>Код страховой компании (Справочник F002; из StrahReestr поля PLAT)</summary>
        public string SMO { get; set; }

        /// <summary>ОГРН Страховой компании (из StrahReestr)</summary>
        public string SMO_OGRN { get; set; }

        /// <summary>Наименование страховой компании (из StrahReestr)</summary>
        public string SMO_NAM { get; set; }
    }
}
