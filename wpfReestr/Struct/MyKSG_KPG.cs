using System.Collections.Generic;

namespace wpfReestr
{
    /// <summary>КЛАСС Структура сведения о КСГ -> MyONK_SL</summary>
    public class MyKSG_KPG
    {
        /// <summary>Номер КСГ (У,  T(20), V023)</summary>
        public string N_KSG { get; set; }

        /// <summary>Год определения КСГ (О,  N(4), по умолчанию 2018)</summary>
        public int VER_KSG { get; set; } = 2018;

        /// <summary>Признак использования подгруппы КСГ (О,  N(1), 0 - нет подгруппы, 1 - есть подгруппа)</summary>
        public int KSG_PG { get; set; }

        /// <summary>Коэффициент затратоемкости (О,  N(2.5))</summary>
        public double KOEF_Z { get; set; }

        /// <summary>Коэффициент управленчиский (О,  N(2.5), по умолчанию 1)</summary>
        public double KOEF_UP { get; set; } = 1.0;

        /// <summary>Базовая ставка (О,  N(6.2), указывается в рублях)</summary>
        public double BZTSZ { get; set; }

        /// <summary>Коэффициент дифференциации (О,  N(2.5))</summary>
        public double KOEF_D { get; set; }

        /// <summary>Коэффициент уровня/подуровня (О,  N(2.5))</summary>
        public double KOEF_U { get; set; }

        /// <summary>Доп-критерии (УМ,  T(20), схемы лекарств, V024)</summary>
        public List<string> CRIT { get; set; }

        /// <summary>Признак использования КСЛП (О,  N(1), 0 - нет КСЛП, 1 - есть КСЛП)</summary>
        public int SL_K { get; set; }

        /// <summary>Итог КСЛП (У,  N(1.5))</summary>
        public double IT_SL { get; set; }

        /// <summary>Список КСЛП (УМ)</summary>
        public List<MySL_KOEF> SL_KOEF { get; set; }
    }
}
