namespace wpfReestr
{
    /// <summary>КЛАСС Структура услуги -> MySL</summary>
    public class MyUSL
    {
        public string Tip { get; set; }
        public string Usl { get; set; }
        public string DatN { get; set; }
        public string DatK { get; set; }
        public string Ksg { get; set; }
        public double Fact { get; set; }
        public double UprFactor { get; set; }
        public int KUSmo { get; set; }
        public string D { get; set; }
        public string PRVS_Usl { get; set; }
        public string Code_Usl { get; set; }
        public string MD { get; set; }

        /// <summary>Дополнительная услуга (препарат для лучевойхимии mt001)</summary>
        public string DopUsl { get; set; }

        /// <summary>Диапазон фракции (дней облучения)</summary>
        public int Frakc { get; set; }

        /// <summary>Услуга оплачиваемая полностью (если меньше 4й дней)</summary>
        public int? Day3 { get; set; }

        ///// <summary>План дней лечения химии</summary>
        //public int? Day3 { get; set; }
    }
}
