namespace wpfReestr
{
    /// <summary>КЛАСС Структура направление на исследования-> MySL</summary>
    public class MyNAPR
    {
        /// <summary>Дата направления (О, D)</summary>
        public string NAPR_DATE { get; set; }

        /// <summary>Код МО (куда направляем, если в другое ЛПУ). Справочник F003. (У, Т(6))</summary>
        public string NAPR_МО { get; set; }

        /// <summary>Вид направления. Справочник V028. (О, N(2))</summary>
        public int NAPR_V { get; set; }

        /// <summary>Метод исследования (обязательно при NAPR_V = 3). Справочник V029. (У, N(2))</summary>
        public int MET_ISSL { get; set; }

        /// <summary>Код услуги. Справочник V001. (У, Т(15))</summary>
        public string NAPR_USL { get; set; }
    }
}
