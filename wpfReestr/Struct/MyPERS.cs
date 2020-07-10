using System;

namespace wpfReestr
{
    /// <summary>КЛАСС Структура персональных данных пациента (в разработке)</summary>
    /// <remarks>Справочник Q018, тип файла L, путь PERS_LIST/PERS</remarks>
    public class MyPERS
    {
        /// <summary>Код записи пациента (KL из kbol; из StrahReestr)</summary>
        public decimal ID_PAC { get; set; }

        /// <summary>Фамилия пациета (из StrahReestr поля FAMILY)</summary>
        public string FAM { get; set; }

        /// <summary>Имя пациета (из StrahReestr поля NAME)</summary>
        public string IM { get; set; }

        /// <summary>Отчество пациета (Если отчества нет, то в поле DOST = 1; из StrahReestr поля FATHER)</summary>
        public string OT { get; set; }

        /// <summary>Пол пациета (Справочник V005: 1 - муж, 2 - женский; из StrahReestr поля Pol)</summary>
        public int W { get; set; }

        /// <summary>Дата рождения пациета (из StrahReestr поля VOZRAST)</summary>
        public DateTime DR { get; set; }

        /// <summary>Код надежости идентификации пациета (Если отчества нет то ставит 1; из StrahReestr поля OS_SLUCH)</summary>
        public string DOST { get; set; }

        /// <summary>Место рождения пациета из паспорта (из StrahReestr)</summary>        
        public string MR { get; set; }

        /// <summary>Тип документа удостоверяющего личность (Справочник F011; из StrahReestr)</summary>        
        public decimal? DOCTYPE { get; set; }

        /// <summary>Серия документа удостоверяющего личность (из StrahReestr)</summary>        
        public string DOCSER { get; set; }

        /// <summary>Номер документа удостоверяющего личность (из StrahReestr)</summary>        
        public string DOCNUM { get; set; }

        /// <summary>Дата выдачи документа удостоверяющего личность (из StrahReestr)</summary>       
        public DateTime? DOCDATE { get; set; }

        /// <summary>Кем выдан документ удостоверяющего личность (из StrahReestr)</summary>       
        public string DOCORG { get; set; }

        /// <summary>СНИЛС (из StrahReestr поля SS)</summary>
        public string SNILS { get; set; }

        /// <summary>ОКАТО места жительства (из StrahReestr; он же OKATOP - код места пребывания; По умолчанию г. Омск = 52401000000)</summary>
        public string OKATOG { get; set; } = "52401000000";
    }
}
