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

    /// <summary>КЛАСС Структура случая (в разработке)</summary>
    public class MySL1
    {
        [JsonIgnore]
        /// <summary>Код корпуса (справочник spr_podraz)</summary>
        public string LPU_1 { get; set; }

        [JsonIgnore]
        /// <summary>Код отделения/подразделения (справочник spr_StrucOtdl)</summary>
        public string PODR { get; set; }

        [JsonIgnore]
        // <summary>Профиль мед помощи (справочник V002)</summary>
        public string PROFIL { get; set; }

        [JsonIgnore]
        // <summary>Профиль койки (справочник V020)</summary>
        public string PROFIL_K { get; set; }


        // <summary>Признак, детский профиль (0 - взрослый, 1 - ребенок до 18 лет)</summary>
        public int DET { get; set; }

        /// <summary>Итоговая сумма</summary>
        public double SUMV { get; set; }
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
        /// <summary>Диспансерное наблюдение</summary>
        public int DN { get; set; }
        /// <summary>Дата выдачи талона ВМП</summary>
        public string TAL_D { get; set; }
        /// <summary>Номер талона ВМП</summary>
        public string TAL_NUM { get; set; }
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
        public MySL1()
        {
            USL = new List<MyUSL>();
        }
    }
}
