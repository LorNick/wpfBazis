namespace wpfReestr
{
    /// <summary>КЛАСС Сведения об оформлении направления</summary>
    /// <remarks>Справочник Q018; тип файла C, H, T; путь ZL_LIST/ZAP/Z_SL/SL/NAPR</remarks>
    public class MyNAPR
    {
        /// <summary>Дата создания направления (из json NOM_USL)</summary>
        public string NAPR_DATE { get; set; }

        /// <summary>Код ЛПУ куда направляем (Справочник F003; если себе, то данное поле не указываем; из json NOM_USL)</summary>
        public string NAPR_МО { get; set; }

        /// <summary>Вид направления (Справочник V028; из json NOM_USL)</summary>
        public int NAPR_V { get; set; }

        /// <summary>Метод исследования (Справочник V029; обязательно при NAPR_V = 3; из json NOM_USL)</summary>
        public int MET_ISSL { get; set; }

        /// <summary>Код услуги (Справочник V001; обязательно при наличии MET_ISSL; из json NOM_USL)</summary>
        public string NAPR_USL { get; set; }
    }
}
