using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace wpfReestr
{
    /// <summary>КЛАСС Структура случая</summary>
    public class MySL
    {
        /// <summary>КСГ (временная)</summary>
        public string KSG { get; set; }

        /// <summary>Итоговая сумма</summary>
        public double SUMV { get; set; }
        /// <summary>Дети до 4 года (с 2021)</summary>
        public double Sl03 { get; set; }
        /// <summary>Сверхдлительность</summary>
        public double Sl10 { get; set; }
        /// <summary>Дети до 1 года</summary>
        public double Sl13 { get; set; }
        /// <summary>Дети 1 - 4 года</summary>
        public double Sl14 { get; set; }
        /// <summary>Дополнительные диагнозы (типо E11)</summary>
        public double Sl15 { get; set; }
        /// <summary>Коэффициент затратоемкости</summary>
        public double KOEF_Z { get; set; }
        /// <summary>Управленчиский коэффициент</summary>
        public double KOEF_UP { get; set; }
        /// <summary>Коэффициент подуровня</summary>
        public double KOEF_U { get; set; }
        /// <summary>Коэффициент Дифференциации</summary>
        public double KOEF_D { get; set; }
        /// <summary>Коэффициент Доля заработной платы</summary>
        public double KOEF_Dzp { get; set; }
        /// <summary>Короткий случай</summary>
        public double Short { get; set; }
        /// <summary>Итоговый коэффициент КСЛП</summary>
        public double IT_SL { get; set; }
        /// <summary>Код перевода (указывает на код строки куда переводят)</summary>
        public int VB_P { get; set; }
        /// <summary>Цель посещения</summary>
        public string P_Cel { get; set; }
        /// <summary>Код МО направления</summary>
        public int NPR_MO { get; set; }
        /// <summary>Дата направления</summary>
        public string NPR_DATE { get; set; }
        /// <summary>Профиль койки</summary>
        public int PROFIL_K { get; set; }
        /// <summary>Диспансерное наблюдение</summary>
        public int DN { get; set; }
        /// <summary>Дата выдачи талона ВМП</summary>
        public string TAL_D { get; set; }
        /// <summary>Номер талона ВМП</summary>
        public string TAL_NUM { get; set; }
        /// <summary>Модель пациента ВМП</summary>
        public string IDMODP { get; set; }
        /// <summary>Номер группы ВМП</summary>
        public string HGR{ get; set; }
        /// <summary>Характер заболевания (1 - острое, 2 - хроническое впервые до года, 3 - хроническое повторно в прошлом году и ранее)</summary>
        public int C_ZAB { get; set; }

        /// <summary>(У) Подозрение на ЗНО (клиническая группа 1a и 1b)</summary>
        public int DS_ONK { get; set; }
        /// <summary>ONK_SL (У) Противопоказания и отказы (дата регистрации противопоказания к хирургическому лечению)</summary>
        public string PrOt_1 { get; set; }
        /// <summary>ONK_SL (У) Противопоказания и отказы (дата регистрации противопоказания к химии)</summary>
        public string PrOt_2 { get; set; }
        /// <summary>ONK_SL (У) Противопоказания и отказы (дата регистрации противопоказания к лучеваой терапии)</summary>
        public string PrOt_3 { get; set; }
        /// <summary>ONK_SL (У) Противопоказания и отказы (дата регистрации отказа к хирургическому лечению)</summary>
        public string PrOt_4 { get; set; }
        /// <summary>ONK_SL (У) Противопоказания и отказы (дата регистрации отказа к химии)</summary>
        public string PrOt_5 { get; set; }
        /// <summary>ONK_SL (У) Противопоказания и отказы (дата регистрации отказа к к лучеваой терапии)</summary>
        public string PrOt_6 { get; set; }

        /// <summary>ONK_SL (У) Консилиум (Определена тактика обследования)</summary>
        public string Taktika_1 { get; set; }
        /// <summary>ONK_SL (У) Консилиум (Определена тактика лечения)</summary>
        public string Taktika_2 { get; set; }
        /// <summary>ONK_SL (У) Консилиум (Изменена тактика лечения)</summary>
        public string Taktika_3 { get; set; }

        /// <summary>Список Блоков направлений (УМ, S)</summary>
        public List<MyNAPR> NAPR { get; set; }

        /// <summary>Список Блоков консилиумов (УМ, S)</summary>
        public List<MyCONS> CONS { get; set; }

        /// <summary>Блок Онко-случай (У, S)</summary>
        public MyONK_SL ONK_SL { get; set; }

        /// <summary>Блок КСГ (У, S)</summary>
        public MyKSG_KPG KSG_KPG { get; set; }

        /// <summary>Блок услуг</summary>
        public List<MyUSL> USL { get; set; }

        /// <summary>Доп-критерии (УМ,  T(20), схемы лекарств, V024) УБРАТЬ ПОТОМ В БЛОК KSG_KPG</summary>
        public List<string> CRIT { get; set; }

        /// <summary>КОНСТРУКТОР</summary>
        public MySL()
        {
            USL = new List<MyUSL>();
        }
    }

    /// <summary>КЛАСС Сведенния о случае (в разработке)</summary>
    /// <remarks>Справочник Q018; тип файла C, H, T; путь ZL_LIST/ZAP/Z_SL/SL; только используемые поля</remarks>
    public class MySL1
    {
        [JsonIgnore]
        /// <summary>Идентификатор (просто порядковый номер случая = 1; автоматом в XML)</summary>
        public int SL_ID { get; set; }

        [JsonIgnore]
        /// <summary>Код корпуса (Cправочник spr_podraz; из StrahReestr)</summary>
        public string LPU_1 { get; set; }

        [JsonIgnore]
        /// <summary>Код отделения/подразделения (Справочник spr_StrucOtdl; из StrahReestr)</summary>
        public string PODR { get; set; }

        [JsonIgnore]
        /// <summary>Профиль мед помощи (Справочник V002; из StrahReestr)</summary>
        public string PROFIL { get; set; }

        /// <summary>Профиль койки (Справочник V020; обязательный для стационара; из json NOM_USL)</summary>
        public string PROFIL_K { get; set; }

        [JsonIgnore]
        /// <summary>Признак, детский профиль (0 - взрослый, 1 - ребенок до 18 лет; из StrahReestr)</summary>
        public int DET { get; set; }

        /// <summary>Цель посещения (Справочник V025; обязательный для поликлиники USL_OK = 3; из json NOM_USL)</summary>
        public string P_CEL { get; set; }

        [JsonIgnore]
        /// <summary>Номер стационара/посещения/диагностики (0 - взрослый, 1 - ребенок до 18 лет; из StrahReestr поля PACIENTID)</summary>
        public string NHISTORY { get; set; }

        [JsonIgnore]
        /// <summary>Признак поступления/перевода (только для стационара USL_OK = 1 или 2; 1 - самостоятельно, по умолчанию, 4 - перевод; в XML ставим 1)</summary>
        public int P_PER { get; set; }

        [JsonIgnore]
        /// <summary>Дата начала лечения (из StrahReestr поля ARR_DATE)</summary>
        public DateTime DATE_1 { get; set; }

        [JsonIgnore]
        /// <summary>Дата окончания лечения (из StrahReestr поля EX_DATE)</summary>
        public DateTime DATE_2 { get; set; }

        [JsonIgnore]
        /// <summary>Продолжительность госпитализации (только для стационара USL_OK = 1 или 2; из StrahReestr поля KOL_USL)</summary>
        public int KD { get; set; }

        [JsonIgnore]
        /// <summary>Диагноз основной (из StrahReestr)</summary>
        public string DS1 { get; set; }

        [JsonIgnore]
        /// <summary>Диагноз сопутствующий (из StrahReestr)</summary>
        public string DS2 { get; set; }

        /// <summary>Характер заболевания (Справочник V027; обязательно для ЗНО; из json NOM_USL)</summary>
        /// <remarks>1 - острое, 2 - хроническое впервые до года, 3 - хроническое повторно в прошлом году и ранее</remarks>
        public int C_ZAB { get; set; }

        /// <summary>Подозрение на ЗНО (0 - ЗНО, 1 - подозрение ЗНО; из json NOM_USL)</summary>
        /// <remarks>клиническая группа 1a и 1b</remarks>
        public int DS_ONK { get; set; }

        /// <summary>Диспансерное наблюдение (заполняем при P_CEL = 1.3; из json NOM_USL)</summary>
        public int DN { get; set; }

        /// <summary>Список Блоков направлений (из json NOM_USL)</summary>
        /// <remarks>если есть направления</remarks>
        public List<MyNAPR> NAPR { get; set; }

        /// <summary>Список Блоков консилиумов (Обязательно для ЗНО; из json NOM_USL)</summary>
        public List<MyCONS> CONS { get; set; }

        /// <summary>Блок Онко-случай (Обязательно при DS_ONK = 0; из json NOM_USL)</summary>
        public MyONK_SL ONK_SL { get; set; }

        /// <summary>Блок КСГ (Обязательно для стационара USL_OK = 1 или 2; из json NOM_USL)</summary>
        public MyKSG_KPG KSG_KPG { get; set; }

        [JsonIgnore]
        /// <summary>Специальность врача (Справочник V021 поле IDSPEC; из StrahReestr)</summary>
        public int PRVS { get; set; }

        [JsonIgnore]
        /// <summary>Наименование справочника Специальность врача (в XML ставим "V021")</summary>
        public string VERS_SPEC { get; set; } = "V021";

        [JsonIgnore]
        /// <summary>Код врача (Справочник spr_medspec (StrahVrachMIAC); из StrahReestr)</summary>
        public string IDDOKT { get; set; }

        [JsonIgnore]
        /// <summary>Количество единиц оплаты (в XML ставим 1)</summary>
        public int ED_COL { get; set; }

        [JsonIgnore]
        /// <summary>Тариф (из StrahReestr)</summary>
        public decimal TARIF { get; set; }

        [JsonIgnore]
        /// <summary>Сумма случая (из StrahReestr поля SUM_LPU)</summary>
        public decimal SUM_M { get; set; }

        /// <summary>Блок услуг</summary>
        public List<MyUSL> USL { get; set; }

        /// <summary>КОНСТРУКТОР</summary>
        public MySL1()
        {
            USL = new List<MyUSL>();
        }
    }
}
