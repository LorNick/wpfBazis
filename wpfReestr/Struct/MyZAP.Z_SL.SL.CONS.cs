namespace wpfReestr
{
    /// <summary>КЛАСС Сведенния о проведения консилиумов (в разработке)</summary>
    /// <remarks>Справочник Q018; тип файла C, H, T; путь ZL_LIST/ZAP/Z_SL/SL/CONS</remarks>
    public class MyCONS
    {
        /// <summary>Цель проведения консилиума (Справочник N019; из json NOM_USL)</summary>
        public int PR_CONS { get; set; }

        /// <summary>Дата проведения консилиума</summary>
        /// <remarks>Обязательно для PR_CONS=(1,2,3)</remarks>
        public string DT_CONS { get; set; }
    }
}
