namespace wpfReestr
{
    /// <summary>КЛАСС Коэффициенты сложности лечения в КСГ -> MyKSG_KPG</summary>
    public class MySL_KOEF
    {
        /// <summary>Код КСЛП (О, N(4))</summary>
        public int IDSL { get; set; }

        /// <summary>Значение КСЛП (О, N(1.5))</summary>
        public double Z_SL { get; set; }
    }
}
