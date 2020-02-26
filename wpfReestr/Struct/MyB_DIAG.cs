namespace wpfReestr
{
    /// <summary>КЛАСС Структура диагностический блок -> MyONK_SL</summary>
    public class MyB_DIAG
    {
        /// <summary>Дата взятия материала (О, D)</summary>
        public string DIAG_DATE { get; set; }

        /// <summary>Тип диагностики (О, N(1), N014, заполняется 1-гистология, 2-ИГХ)</summary>
        public int DIAG_TIP { get; set; }

        /// <summary>Код показателя. (О, N(3), при DIAG_TIP=1 берем из N007, при  DIAG_TIP=2 берем из N010)</summary>
        public int DIAG_CODE { get; set; }

        /// <summary>Код результата. (У, N(3), при DIAG_TIP=1 берем из N008, при  DIAG_TIP=2 берем из N011)</summary>
        public int DIAG_RSLT { get; set; }

        /// <summary>Признак результата (У, N(1), ставим 1)</summary>
        public int REC_RSLT { get; set; } = 1;
    }
}
