using Newtonsoft.Json;

namespace wpfReestr
{
    /// <summary>КЛАСС Диагностический блок</summary>
    /// <remarks>Справочник Q018; тип файла C, T; путь ZL_LIST/ZAP/Z_SL/SL/ONK_SL/B_DIAG</remarks> 
    public class MyB_DIAG
    {
        /// <summary>Дата взятия материала (из json NOM_USL)</summary>
        public string DIAG_DATE { get; set; }

        /// <summary>Тип диагностики (Справочник N014, 1-гистология, 2-ИГХ; из json NOM_USL)</summary>
        public int DIAG_TIP { get; set; }

        /// <summary>Код показателя. (при DIAG_TIP=1 берем из N007, при  DIAG_TIP=2 берем из N010; из json NOM_USL)</summary>
        public int DIAG_CODE { get; set; }

        /// <summary>Код результата. (при DIAG_TIP=1 берем из N008, при  DIAG_TIP=2 берем из N011; из json NOM_USL)</summary>
        public int DIAG_RSLT { get; set; }

        [JsonIgnore]
        /// <summary>Признак результата (в XML ставим 1)</summary>
        public int REC_RSLT { get; set; } = 1;
    }
}
