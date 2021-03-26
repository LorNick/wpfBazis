using System.Collections.Generic;
using Newtonsoft.Json;

namespace wpfReestr
{
    /// <summary>КЛАСС Сведенния о КСГ</summary>
    /// <remarks>Справочник Q018; тип файла C, H; путь ZL_LIST/ZAP/Z_SL/SL/KSG_KPG</remarks>
    public class MyKSG_KPG
    {
        /// <summary>Номер КСГ (Справочник V023; из json NOM_USL)</summary>
        public string N_KSG { get; set; }

        [JsonIgnore]
        /// <summary>Год определения КСГ (в XML ставим текущий год)</summary>
        public int VER_KSG { get; set; }

        [JsonIgnore]
        /// <summary>Признак использования подгруппы КСГ (0 - нет подгруппы, 1 - есть подгруппа; в XML ставим 0)</summary>
        public int KSG_PG { get; set; }

        /// <summary>Коэффициент затратоемкости (из json NOM_USL)</summary>
        public double KOEF_Z { get; set; }

        /// <summary>Коэффициент управленчиский (по умолчанию 1; из json NOM_USL)</summary>
        public double KOEF_UP { get; set; } = 1.0;

        [JsonIgnore]
        /// <summary>Базовая ставка (указывается в рублях; из StrahReestr поля TARIF)</summary>
        public double BZTSZ { get; set; }

        [JsonIgnore]
        /// <summary>Коэффициент дифференциации (в XML ставим 1.10500)</summary>
        public double KOEF_D { get; set; }

        /// <summary>Коэффициент уровня/подуровня (из json NOM_USL)</summary>
        public double KOEF_U { get; set; }

        /// <summary>Доп-критерии (Справочник V024; схемы лекарств; из json NOM_USL)</summary>
        public List<string> CRIT { get; set; }

        [JsonIgnore]
        /// <summary>Признак использования КСЛП (0 - нет КСЛП, 1 - есть КСЛПа; в XML ставим в зависимости от налчия поля IT_SL)</summary>
        public int SL_K { get; set; }

        /// <summary>Итоговый коэффициент КСЛП (из json NOM_USL)</summary>
        public double IT_SL { get; set; }

        /// <summary>Список КСЛП (из json NOM_USL)</summary>
        public List<MySL_KOEF> SL_KOEF { get; set; }
    }
}
