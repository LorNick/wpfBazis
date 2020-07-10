using System.Collections.Generic;
using Newtonsoft.Json;

namespace wpfReestr
{
    /// <summary>КЛАСС Cведения об услугах лечения ЗНО</summary>
    /// <remarks>Справочник Q018; тип файла C, T; путь ZL_LIST/ZAP/Z_SL/SL/ONK_SL/ONK_USL</remarks>
    public class MyONK_USL
    {
        /// <summary>Тип услуги (Справочник N013; из json NOM_USL)</summary>
        public int USL_TIP { get; set; }

        /// <summary>Тип хирургического лечения (Справочник, N014, при USL_TIP = 1; из json NOM_USL)</summary>
        public int HIR_TIP { get; set; }

        /// <summary>Линия лекарственной терапии (Справочник N015, при USL_TIP = 2; из json NOM_USL)</summary>
        public int LEK_TIP_L { get; set; }

        /// <summary>Цикл лекарственной терапии (Справочник N016, при USL_TIP = 2; из json NOM_USL)</summary>
        public int LEK_TIP_V { get; set; }

        /// <summary>Блок лекарственных препаратов (при USL_TIP = (2, 4); из json NOM_USL)</summary>
        public List<MyLEK_PR> LEK_PR { get; set; }

        /// <summary>Признак проведения профилактики тошноты (указываем 1 если есть, при химии USL_TIP = (2, 4); из json NOM_USL)</summary>
        public int PPTR { get; set; }

        /// <summary>Тип лучевой терапии (Справочник N017; при USL_TIP = (3, 4); из json NOM_USL)</summary>
        public int LUCH_TIP { get; set; }
                
        /// <summary>Количество фракций проведения лучевой терапии (при USL_TIP = (3, 4))</summary>
        /// <remarks>В онко услугу не пишется, пишется в случай и в KOL_USL</remarks>
        public int K_FR { get; set; }
    }
}
