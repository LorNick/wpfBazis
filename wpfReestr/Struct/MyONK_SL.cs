using System.Collections.Generic;

namespace wpfReestr
{
    /// <summary>КЛАСС Структура сведения о случае лечения ЗНО -> MySL</summary>
    public class MyONK_SL
    {
        /// <summary>Повод обращения (О, N(2), N018)</summary>
        public int DS1_T { get; set; }

        /// <summary>Стадия (У, N(3), N002, при DS1_T = 0, 1, 2, 3, 4)</summary>
        public int STAD { get; set; }

        /// <summary>T (У, N(4), N003, при DS1_T = 0 и DET = 0)</summary>
        public int ONK_T { get; set; }

        /// <summary>N (У, N(4), N004, при DS1_T = 0 и DET = 0)</summary>
        public int ONK_N { get; set; }

        /// <summary>M (У, N(4), N005, при DS1_T = 0 и DET = 0)</summary>
        public int ONK_M { get; set; }

        /// <summary>Отдаленые метастазы (У, N(1), ставим 1 при DS1_T = 1, 2)</summary>
        public int MTSTZ { get; set; }

        /// <summary>Суммарная очагова доза (У, N(4.2), при USL_TIP = 3, 4)</summary>
        public double SOD { get; set; }

        /// <summary>Масса тела (У, N(3.1), кг, при USL_TIP = 2, 4)</summary>
        public double WEI { get; set; }

        /// <summary>Рост (У, N(3), см, при USL_TIP = 2, 4)</summary>
        public int HEI { get; set; }

        /// <summary>Площадь тела (У, N(1.2), м2, при USL_TIP = 2, 4)</summary>
        public double BSA { get; set; }

        /// <summary>Список блоков диагностики (УМ, S)</summary>
        public List<MyB_DIAG> B_DIAG { get; set; }

        /// <summary>Список блоков противопоказаний и отказов (УМ, S)</summary>
        public List<MyB_PROT> B_PROT { get; set; }

        /// <summary>Список блоков онко услуг (УМ, S, обязательно для USL_OK = 1, 2 и DS1_T = 0, 1, 2)</summary>
        public List<MyONK_USL> ONK_USL { get; set; }
    }
}
