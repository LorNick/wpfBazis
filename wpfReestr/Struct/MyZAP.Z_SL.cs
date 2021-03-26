using System;

namespace wpfReestr
{
    /// <summary>КЛАСС Сведенния о законченном случае (в разработке)</summary>
    /// <remarks>Справочник Q018; тип файла C, H, T; путь ZL_LIST/ZAP/Z_SL; только используемые поля</remarks>
    public class MyZ_SL
    {
        /// <summary>Номер записи в реестре законченных случаев (равен IDCASE; из StrahReestr поля NOM_ZAP)</summary>
        public decimal IDCASE { get; set; }

        /// <summary>Условия оказания мед. помощи, (Cправочник V006, 1 - кр. стационра, 2 - дн. стационар, 3 - поликлиника, 4 - параклиника, 5 - гистология; из StrahReestr поля LPU_ST)</summary>
        public int USL_OK { get; set; }

        /// <summary>Вид помощи (Cправочник V008, у нас 13 - поликл/параклиники, 31 - стационар ...; из StrahReestr)</summary>
        public int VIDPOM { get; set; }

        /// <summary>ЛПУ которое направило на лечение (Справочник F003; из json NOM_USL)</summary>
        public int NPR_MO { get; set; }

        /// <summary>Дата направления на лечение (из json NOM_USL)</summary>
        public string NPR_DATE { get; set; }

        /// <summary>Дата начала лечения (из StrahReestr поля ARR_DATE)</summary>
        public DateTime DATE_Z_1 { get; set; }

        /// <summary>Дата окончания лечения (из StrahReestr поля EX_DATE)</summary>
        public DateTime DATE_Z_2 { get; set; }

        /// <summary>Продолжительность госпитализации (только для стационара USL_OK = 1 или 2; из StrahReestr поля KOL_USL)</summary>
        public int KD_Z { get; set; }

        /// <summary>Результат обращения (Справочник V009; из StrahReestr поля RES_G)</summary>
        public decimal RSLT { get; set; }

        /// <summary>Исход заболевания (Справочник V012; из StrahReestr)</summary>
        public decimal ISHOD { get; set; }

        /// <summary>Признак - особый случай (1 - новорожденный, 2 - нет отчества; из StrahReestr)</summary>
        public string OS_SLUCH { get; set; }

        /// <summary>Признак внутрибольничного перевода (1 - при переводе в стационаре; json)</summary>
        public int VB_P { get; set; }

        /// <summary>Блок Сведения о случае (должен быть множественным, но пока еденичный)</summary>
        public MySL1 SL { get; set; }

        /// <summary>Код способа оплаты (Справочник V010; из StrahReestr)</summary>
        public decimal IDSP { get; set; }

        /// <summary>Сумма (из StrahReestr поля SUM_LPU)</summary>
        public decimal SUMV { get; set; }
    }
}
