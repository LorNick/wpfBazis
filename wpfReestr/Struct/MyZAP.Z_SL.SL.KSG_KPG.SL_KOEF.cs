namespace wpfReestr
{
    /// <summary>КЛАСС Коэффициенты сложности лечения в КСГ</summary>
    /// <remarks>Справочник Q018; тип файла C, H; путь ZL_LIST/ZAP/Z_SL/SL/KSG_KPG/SL_KOEF</remarks>    
    public class MySL_KOEF
    {
        /// <summary>Код КСЛП (Справочник SprKslp; из json NOM_USL)</summary>
        public int IDSL { get; set; }

        /// <summary>Значение КСЛП (из json NOM_USL)</summary>
        public double Z_SL { get; set; }
    }
}
