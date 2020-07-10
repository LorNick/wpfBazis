using System.Collections.Generic;

namespace wpfReestr
{
    /// <summary>КЛАСС Сведения о случае лечения ЗНО</summary>
    /// <remarks>Справочник Q018; тип файла C, T; путь ZL_LIST/ZAP/Z_SL/SL/ONK_SL</remarks>
    public class MyONK_SL
    {
        /// <summary>Повод обращения (Справочник N018; из json NOM_USL)</summary>
        public int DS1_T { get; set; }

        /// <summary>Стадия (Справочник N002; при DS1_T = (0, 1, 2, 3, 4); из json NOM_USL)</summary>
        public int STAD { get; set; }

        /// <summary>T (Справочник N003, при DS1_T = 0 и DET = 0; из json NOM_USL)</summary>
        public int ONK_T { get; set; }

        /// <summary>N (Справочник N004, при DS1_T = 0 и DET = 0; из json NOM_USL)</summary>
        public int ONK_N { get; set; }

        /// <summary>M (Справочник N005, при DS1_T = 0 и DET = 0; из json NOM_USL)</summary>
        public int ONK_M { get; set; }

        /// <summary>Отдаленые метастазы (ставим 1 при DS1_T = (1, 2); из json NOM_USL)</summary>
        public int MTSTZ { get; set; }

        /// <summary>Суммарная очагова доза (при USL_TIP = (3, 4); из json NOM_USL)</summary>
        public double SOD { get; set; }

        /// <summary>Список блоков диагностики (из json NOM_USL)</summary>
        public List<MyB_DIAG> B_DIAG { get; set; }

        /// <summary>Список блоков противопоказаний и отказов (из json NOM_USL)</summary>
        public List<MyB_PROT> B_PROT { get; set; }

        /// <summary>Список блоков онко-услуг (обязательно для USL_OK = (1, 2) и DS1_T = (0, 1, 2); из json NOM_USL)</summary>
        public List<MyONK_USL> ONK_USL { get; set; }

        /// <summary>Количество фракций проведения лучевой терапии (при USL_TIP = (3, 4) в блоке ONK_USL; из json NOM_USL)</summary>       
        public int K_FR { get; set; }

        /// <summary>Масса тела (кг, при USL_TIP = (2, 4); из json NOM_USL)</summary>
        public double WEI { get; set; }

        /// <summary>Рост (см, при USL_TIP = (2, 4); из json NOM_USL)</summary>
        public int HEI { get; set; }

        /// <summary>Площадь тела (м2, при USL_TIP = (2, 4); из json NOM_USL)</summary>
        public double BSA { get; set; }
    }
}
