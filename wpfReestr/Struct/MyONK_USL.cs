using System.Collections.Generic;

namespace wpfReestr
{
    /// <summary>КЛАСС Структура сведения об услугах лечения ЗНО -> MyONK_SL</summary>
    public class MyONK_USL
    {
        /// <summary>Тип услуги (О, N(1), N013)</summary>
        public int USL_TIP { get; set; }

        /// <summary>Тип хирургического лечения (У, N(1), N014, заполняется при USL_TIP = 1)</summary>
        public int HIR_TIP { get; set; }

        /// <summary>Линия лекарственной терапии (У, N(1), N015, заполняется при USL_TIP = 2)</summary>
        public int LEK_TIP_L { get; set; }

        /// <summary>Цикл лекарственной терапии (У, N(1), N016, заполняется при USL_TIP = 2)</summary>
        public int LEK_TIP_V { get; set; }

        /// <summary>Блок лекарственных препаратов (УМ, S, заполняется при USL_TIP = 2, 4)</summary>
        public List<MyLEK_PR> LEK_PR { get; set; }

        /// <summary>Признак проведения профилактики тошноты (У, N(1), указываем 1 если есть, заполняется при USL_TIP = 2, 4)</summary>
        public int PPTR { get; set; }

        /// <summary>Тип лучевой терапии (У, N(1), N017, заполняется при USL_TIP = 3, 4)</summary>
        public int LUCH_TIP { get; set; }

        /// <summary>Количество фракций проведения лучевой терапии (У, N(2), при USL_TIP = 3, 4) В онко услугу не пишеться, пишется в случай и в KOL_USL</summary>
        public int K_FR { get; set; }
    }
}
