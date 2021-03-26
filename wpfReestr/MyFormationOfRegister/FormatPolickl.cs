using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using m = wpfReestr.MyMet;
using wpfStatic;
using e = Microsoft.Office.Interop.Excel;


namespace wpfReestr
{
    ///// <summary>Модули доступа</summary>
    //public enum eTipUsl
    //{
    //    /// <summary>Круглосуточный стационар</summary>
    //    StacKr = 1,
    //    /// <summary>Дневной стационар</summary>
    //    StacDn = 2,
    //    /// <summary>Поликлиника</summary>
    //    Pol = 3,
    //    /// <summary>Параклиника</summary>
    //    Par = 4,
    //    /// <summary>Гистология</summary>
    //    Gist = 5
    //}

    /// <summary>КЛАСС Формируем реестры</summary>
    public partial class MyReestr
    {
        /// <summary>МЕТОД 5. Расчет Поликлиники</summary>
        private void MET_CalcPol()
        {
            // Связываем ReStrahReestr и ReApac
            DataRow _RowApac = PRI_RowReestr.GetChildRows("ReReestr_Apac")[0];

            // Версия редакции 3.2 (январь 2020)
            // Имя поля в таблице StrahReestr (Описание)
            // PLAT (Код страховой компании)                               далее в  MET_CalcAll()
            PRI_StrahReestr.PLAT = m.MET_PoleStr("Scom", _RowApac);

            // LPU_1 (Подразделения МО)
            int _Podrazd = m.MET_PoleInt("Podrazd", _RowApac);
            PRI_StrahReestr.LPU_1 = _Podrazd == 3 ? 55550900 : 55550901;                // главный/филиал

            // ORDER (Направление на госпитализацию, 1 - плановая, 2 - экстренная (у нас нету), 0 - поликлиника)
            PRI_StrahReestr.ORDER = 0;

            // LPU_ST (Условия оказания мед. помощи, справочник V006, 1 - кр. стационра, 2 - дн. стационар, 3 - поликлиника)
            PRI_StrahReestr.LPU_ST = 3;

            // VIDPOM Вид помощи, справочник V008: 11 - поликл. медсестра, 13 - поликл врач/диагностика/дневной стационар, 14 - телемедицина, 31 - стационар, 32 - ВМП
            PRI_StrahReestr.VIDPOM = m.MET_PoleInt("VIDPOM", _RowApac);

            // PROFIL (Профиль, справочник V002)
            PRI_StrahReestr.PROFIL = m.MET_PoleDec("PROFIL", _RowApac);
            // Проверка
            PRI_ErrorToExcel.MET_VeryfError(PRI_StrahReestr.PROFIL == 0,
                        "12", "(вну) Не найден профиль",
                        ref PRI_ErrorRow);

            // PODR (Код отделения)
            string _Prof = ((int)PRI_StrahReestr.PROFIL).ToString("D3");
            PRI_StrahReestr.PODR = m.MET_ParseDec($"3{_Prof}2{_Prof}");

            // DET (Детский профиль, если ребёнок то 1 иначе 0)   далее в  MET_CalcAll()

            PRI_Age = m.MET_PoleInt("Age", _RowApac);

            // PRVS (Специальность врача, справочник V004)
            PRI_StrahReestr.PRVS = m.MET_PoleStr("PRVS", _RowApac);

            // IDDOKT (Код врача, справочник врачей)
            PRI_StrahReestr.IDDOKT = m.MET_PoleStr("IDDOKT", _RowApac);

            // ARR_DATE -> DATE_1 -> DATE_IN (Дата начала)
            PRI_StrahReestr.ARR_DATE = m.MET_PoleDate("DN", _RowApac);

            // EX_DATE -> DATE_2 -> DATE_OUT (Дата окончания)
            PRI_StrahReestr.EX_DATE = m.MET_PoleDate("DK", _RowApac);

            // DS1 -> DS (Диагноз)
            PRI_StrahReestr.DS1 = m.MET_PoleStr("D", PRI_RowReestr);
            // Проверка
            if (PRI_StrahReestr.DS1.Length < 3)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "42";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Что то не так с диагнозом";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // DS2 (Сопутствующий Диагноз - не заполняем)
            PRI_StrahReestr.DS2 = "";

            // PACIENTID -> NHISTORY (Номер истории болезни/талона)
            PRI_StrahReestr.PACIENTID = m.MET_PoleStr("Cod", PRI_RowReestr);

            // RES_G -> RSLT(Результат обращения/госпитализации, справочник V009)
            int[] _RES_G = { 0, 301, 301, 304, 305, 306, 307, 308, 309, 311, 311, 306 };
            int _Rezobr = m.MET_PoleInt("REZOBR", _RowApac);
            // Если месяц сдачи совпадает с месяцем посещения, то смотрим какой результат проставили врачи
            if (PRI_StrahReestr.EX_DATE.Value.Month == PRI_StrahFile.MONTH)
                PRI_StrahReestr.RES_G = _RES_G[_Rezobr];
            else  // если месяц не совпадает, то ставим результат обращения 302 - прерван по инициативе пациента (ну типо больше не пришел)
                PRI_StrahReestr.RES_G = 302;

            //// Для посещений прошлого месяца, ставим результат обращения (Лечение прервано по инициативе пациента)
            //if (PRI_StrahReestr.EX_DATE.Value.Month < PROP_DateN.Month)
            //    PRI_StrahReestr.RES_G = 302;

            // ISHOD (Исход заболевания, справочник V012)
            int[] _ISHOD = { 0, 301, 303, 302, 304, 304, 304, 304, 304, 304, 304, 304 };
            PRI_StrahReestr.ISHOD = _ISHOD[_Rezobr];

            // KOL_USL (Койко дней)
            PRI_StrahReestr.KOL_USL = m.MET_PoleInt("uet3", _RowApac);

            // IDSP (Код способа оплаты, справочник V010: 29 - разовые посещения, 30 по заболеванию)
            PRI_StrahReestr.IDSP = PRI_StrahReestr.KOL_USL == 1 ? 29 : 30;

            // TARIF (Тариф)
            PRI_StrahReestr.TARIF = m.MET_PoleDec("Tarif", _RowApac);
            // Проверка
            if (PRI_StrahReestr.TARIF == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "26";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тариф";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
            }

            // SUM_LPU (Сумма услуги)
            PRI_StrahReestr.SUM_LPU = PRI_StrahReestr.TARIF;

            // VPOLIS (Тип полиса 1 - старый, 2 - временный, 3 - новый)   далее в  MET_CalcAll()
            PRI_NPolis = m.MET_PoleStr("SN", _RowApac);

            // SERIA -> SPOLIS (Серия полиса)
            PRI_StrahReestr.SERIA = m.MET_PoleStr("SS", _RowApac).ToUpper();

            // DayN (Длительность лечения из Тарифов)
            PRI_StrahReestr.DayN = 1;

            // C_ZAB Характер заболевания 1 - острое, 2 храническое впервые, 3 - хроническое повторно
            // Только для поликлиники (не Z) или для стационара (с диагнозом С..D09)
            if (PRI_StrahReestr.DS1.Substring(0, 1) != "Z")
                PRI_Sl.C_ZAB = m.MET_PoleInt("C_Zab", _RowApac);

            // Цель посещения (дальше будет уточнение)
            PRI_Sl.P_Cel = m.MET_PoleStr("P_Cel", _RowApac);
            // Проверка
            if ((PRI_StrahReestr.IDSP == 30 && PRI_Sl.P_Cel != "3.0") || (PRI_StrahReestr.IDSP != 30 && PRI_Sl.P_Cel == "3.0"))
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "35";
                PRI_ErrorToExcel.PROP_ErrorName = $"(вну) Не совпадает IDSP {PRI_StrahReestr.IDSP} с целью посещения {PRI_Sl.P_Cel}";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
            }

            PRI_CouUsl = 0;

            // Связываем ReApac и RePol
            foreach (DataRow _PolRow in _RowApac.GetChildRows("ReApac_Pol"))
            {
                PRI_CouUsl++;

                var _Usluga = new MyUSL
                {
                    //    Nom = m.MET_PoleInt("Num", _PolRow),
                    DatN = m.MET_PoleStr("DP", _PolRow),
                    D = m.MET_PoleStr("D", _PolRow),
                    PRVS_Usl = m.MET_PoleStr("PRVS_Usl", _PolRow),
                    MD = m.MET_PoleStr("CODE_MD", _PolRow),
                    Code_Usl = "AMB.1.99", // m.MET_PoleStr("CODE_USL", _PolRow),
                    Usl = m.MET_PoleStr("VID_VME", _PolRow)
                };
                _Usluga.DatN = _Usluga.DatN == "" ? null : _Usluga.DatN.Remove(10);
                PRI_Sl.USL.Add(_Usluga);

                // Берем из первого случая
                if (PRI_CouUsl == 1)
                {
                    // ЛПУ направления
                    PRI_Sl.NPR_MO = m.MET_PoleInt("NPR_MO", _PolRow);

                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим

                    // Дата направления
                    PRI_Sl.NPR_DATE = m.MET_PoleStr("NPR_DATE", _PolRow);
                    // Дата направления должна быть не пустой и меньше = сегодняшнего дня
                    if (string.IsNullOrEmpty(PRI_Sl.NPR_DATE))
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "59";
                        PRI_ErrorToExcel.PROP_ErrorName = $"(вну) Отсутствует дата направления NPR_DATE в поликлинике";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                    }
                    else
                    {
                        MET_VerifDate(PRI_Sl.NPR_DATE, "направления в поликлинику", true, false, false, true);
                    }
                }

                // Только для ЗНО
                if ((PRI_StrahReestr.DS1.Substring(0, 1) == "C" || PRI_StrahReestr.DS1.Substring(0, 2) == "D0")
                   && PRI_StrahReestr.EX_DATE >= new DateTime(2019, 12, 1))
                {
                    string _jTag = m.MET_PoleStr("jTag", _PolRow);

                    // Проверка на наличия данных KoblInfo
                    if (string.IsNullOrEmpty(_jTag))
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "28";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найдена строка APAC в KoblInfo";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        continue;
                    }

                    JObject _Json;
                    try
                    {
                        _Json = JObject.Parse(_jTag);
                    }
                    catch
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "30";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в KoblInfo";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }

                    // УБРАЛ направления 28/07/2020 из за штрафов Капитала по превышению 7го срока и отсутствия точной даты направления
                    //// Направления на исследования
                    //IList<JToken> _Results = _Json["NAPR"]?.Children().ToList();
                    //if (_Results != null)
                    //{
                    //    PRI_Sl.NAPR = new List<MyNAPR>();
                    //    foreach (JToken _result in _Results)
                    //    {
                    //        MyNAPR _Napr = _result.ToObject<MyNAPR>();
                    //        _Napr.NAPR_V = 3;
                    //        _Napr.NAPR_DATE = PRI_StrahReestr.ARR_DATE.Value.ToString("dd.MM.yyyy");  // !!! Пока берем не из направления, а начальную дату случая
                    //        PRI_Sl.NAPR.Add(_Napr);
                    //    }
                    //}

                    // Только из последнего посещения
                    if (PRI_CouUsl == PRI_StrahReestr.KOL_USL)
                    {
                        // Цель посещения 3.0 - обращение, 1.0 - посещение разовое, 1.3 - диспансерное наблюдение (в базисе 4.0)
                        if (PRI_Sl.P_Cel == "1.0")
                        {
                            try
                            {
                                double _Cel = (double)_Json["Cel"];
                                // Цель посещения - диспансерное наблюдение - Состоит
                                if (_Cel == 4.0)
                                {
                                    PRI_Sl.P_Cel = "1.3";
                                    PRI_Sl.DN = 1;
                                }
                            }
                            catch
                            {
                                PRI_ErrorToExcel.PROP_ErrorCod = "41";
                                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Тег Cel (Цель посещения) в KoblInfo имеет некоректное значение";
                                PRI_ErrorToExcel.MET_SaveError();
                                PRI_ErrorRow = true;
                                return;
                            }
                        }

                        // Клиническая группа
                        string _klin_gr = (string)_Json["Klin_gr"];
                        // Проверка на наличия Клинической группы в KoblInfo
                        if (string.IsNullOrEmpty(_klin_gr))
                        {
                            PRI_ErrorToExcel.PROP_ErrorCod = "29";
                            PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тег Klin_gr (Клиническая группа) в KoblInfo";
                            PRI_ErrorToExcel.MET_SaveError();
                            PRI_ErrorRow = true;
                            return;
                        }

                        // DS_ONK - Если подозрение на ЗНО
                        if (_klin_gr == "Ia" || _klin_gr == "Ib")
                            PRI_Sl.DS_ONK = 1;
                        else
                            PRI_Sl.DS_ONK = 0;

                        // ONK_UL - Блок лечения ЗНО
                        if (PRI_Sl.DS_ONK == 0)
                        {
                            // Диагноз
                            string _D5 = PRI_StrahReestr.DS1;
                            string _D3 = _D5.Substring(0, 3);

                            PRI_Sl.ONK_SL = new MyONK_SL();

                            // DS1_T - Повод обращения (N018)
                            // 0 - первичное лечение
                            // 1 - рецедив с метастазами, 2 - прогресирование с метастазами, 21, 22 - то-же но без метастаз)
                            // 3 - динамическое наблюдение
                            // 4 - диспансерное наблюдение
                            // 5 - диагностика
                            // 6 - симптоматическое лечение
                            PRI_Sl.ONK_SL.DS1_T = (int?)_Json["DS1_T"] ?? 0;
                            if (PRI_Sl.ONK_SL.DS1_T == 1 || PRI_Sl.ONK_SL.DS1_T == 2)
                            {
                                // MTSTZ - Отдаленные метастазы
                                PRI_Sl.ONK_SL.MTSTZ = 1;
                            }
                            if (PRI_Sl.ONK_SL.DS1_T == 21 || PRI_Sl.ONK_SL.DS1_T == 22)
                            {
                                PRI_Sl.ONK_SL.DS1_T = PRI_Sl.ONK_SL.DS1_T - 20;
                            }
                            if (PRI_Sl.P_Cel == "1.3") PRI_Sl.ONK_SL.DS1_T = 4;

                            // STAD - Стадия (только для DS1_T < 5)
                            if (PRI_Sl.ONK_SL.DS1_T < 5)
                            {
                                string _stadia = (string)_Json["Stadia"];
                                var _N002 = PRI_N002.Where(e => e.Kod1 == _D5).ToList();
                                // Сначала пытаемся найти по полному диагнозу
                                if (!_N002.Any())
                                    _N002 = PRI_N002.Where(e => e.Kod1 == _D3).ToList();
                                // Иначе ищем по 3м сиволам диагноза
                                if (!_N002.Any())
                                    _N002 = PRI_N002.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                                int? _stadiaNumber = _N002.FirstOrDefault(e => e.Kod3.Contains($"\"{_stadia}\""))?.Number ??
                                                     _N002.First()?.Number;
                                PRI_Sl.ONK_SL.STAD = _stadiaNumber ?? 145;
                            }

                            // DET (Детский профиль, если ребёнок то 1 иначе 0)
                            PRI_StrahReestr.DET = PRI_Age < 18 ? 1 : 0;

                            // TNM (заполняем только для взрослых и первичном лечении)
                            if (PRI_StrahReestr.DET == 0 && PRI_Sl.ONK_SL.DS1_T == 0)
                            {
                                // ONK_T - T
                                string _T = (string)_Json["T"];
                                var _N003 = PRI_N003.Where(e => e.Kod1 == _D5).ToList();
                                // Сначала пытаемся найти по полному диагнозу
                                if (!_N003.Any())
                                    _N003 = PRI_N003.Where(e => e.Kod1 == _D3).ToList();
                                // Иначе ищем по 3м сиволам диагноза
                                if (!_N003.Any())
                                    _N003 = PRI_N003.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                                int? _TNumber = _N003.FirstOrDefault(e => e.Kod3.Contains($"\"{_T}\""))?.Number ??
                                                _N003.First()?.Number;
                                PRI_Sl.ONK_SL.ONK_T = _TNumber ?? 182;

                                // ONK_N - N
                                string _N = (string)_Json["N"];
                                var _N004 = PRI_N004.Where(e => e.Kod1 == _D5).ToList();
                                // Сначала пытаемся найти по полному диагнозу
                                if (!_N004.Any())
                                    _N004 = PRI_N004.Where(e => e.Kod1 == _D3).ToList();
                                // Иначе ищем по 3м сиволам диагноза
                                if (!_N004.Any())
                                    _N004 = PRI_N004.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                                int? _NNumber = _N004.FirstOrDefault(e => e.Kod3.Contains($"\"{_N}\""))?.Number ??
                                                _N004.First()?.Number;
                                PRI_Sl.ONK_SL.ONK_N = _NNumber ?? 99;

                                // ONK_M - M
                                string _M = (string)_Json["M"];
                                var _N005 = PRI_N005.Where(e => e.Kod1 == _D5).ToList();
                                // Сначала пытаемся найти по полному диагнозу
                                if (!_N005.Any())
                                    _N005 = PRI_N005.Where(e => e.Kod1 == _D3).ToList();
                                // Иначе ищем по 3м сиволам диагноза
                                if (!_N005.Any())
                                    _N005 = PRI_N005.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                                int? _MNumber = _N005.FirstOrDefault(e => e.Kod3.Contains($"\"{_M}\""))?.Number ??
                                                _N005.First()?.Number;
                                PRI_Sl.ONK_SL.ONK_M = _MNumber ?? 56;
                            }

                            // CONS Блок о проведении консилиума
                            // Связываем ReApac и RePolCons
                            foreach (DataRow _PolRowCons in _RowApac.GetChildRows("ReApac_PolCons"))
                            {
                                switch (m.MET_PoleInt("PR_CONS", _PolRowCons))
                                {
                                    case 1:
                                        PRI_Sl.Taktika_1 = MET_VerifDate(m.MET_PoleStr("DT_CONS", _PolRowCons), $"консилиума", true, false);
                                        if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                        break;
                                    case 2:
                                        PRI_Sl.Taktika_2 = MET_VerifDate(m.MET_PoleStr("DT_CONS", _PolRowCons), $"консилиума", true, false);
                                        if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                        break;
                                    case 3:
                                        PRI_Sl.Taktika_3 = MET_VerifDate(m.MET_PoleStr("DT_CONS", _PolRowCons), $"консилиума", true, false);
                                        if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                        break;
                                }
                            }

                            // B_PROT Блок об имеющихся противопоказаниях и отказах
                            PRI_Sl.PrOt_1 = MET_VerifDate((string)_Json["PrOt_1"], "PrOt_1", true);
                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                            PRI_Sl.PrOt_2 = MET_VerifDate((string)_Json["PrOt_2"], "PrOt_2", true);
                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                            PRI_Sl.PrOt_3 = MET_VerifDate((string)_Json["PrOt_3"], "PrOt_3", true);
                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                            PRI_Sl.PrOt_4 = MET_VerifDate((string)_Json["PrOt_4"], "PrOt_4", true);
                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                            PRI_Sl.PrOt_5 = MET_VerifDate((string)_Json["PrOt_5"], "PrOt_5", true);
                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                            PRI_Sl.PrOt_6 = MET_VerifDate((string)_Json["PrOt_6"], "PrOt_6", true);
                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                        }
                    }

                    // Телемедицина. Берем из первого случая.
                    if (PRI_CouUsl == 1)
                    {
                        // Есть ли тег телемедицины
                        int? _TMK = (int?)_Json["TMK"];
                        if (_TMK != null)
                        {
                            // Телемедицина в режими отложенной консультации
                            if (_TMK == 2)
                            {
                                PRI_Sl.USL[PRI_CouUsl - 1].Code_Usl = "TM.2";
                            }
                            // Телемедицина консилиум
                            else if (_TMK == 3)
                            {
                                PRI_Sl.USL[PRI_CouUsl - 1].Code_Usl = "TM.3";
                            }
                            else
                            {
                                PRI_ErrorToExcel.PROP_ErrorCod = "47";
                                PRI_ErrorToExcel.PROP_ErrorName = $"(вну) Неизвестный код телемедицины TMK:{_TMK}";
                                PRI_ErrorToExcel.MET_SaveError();
                                PRI_ErrorRow = true;
                                return;
                            }

                            var _Sv = PRI_StrahStacSv.FirstOrDefault(e => e.Flag == 12 && e.CODE_USL == PRI_Sl.USL[PRI_CouUsl - 1].Code_Usl
                                    && PRI_StrahReestr.ARR_DATE >= e.DateN && PRI_StrahReestr.ARR_DATE <= e.DateK);

                            // Тарифы
                            PRI_StrahReestr.SUM_LPU = _Sv.Tarif;
                            PRI_StrahReestr.TARIF = PRI_StrahReestr.SUM_LPU;

                            // Вид помощи 14
                            PRI_StrahReestr.VIDPOM = _Sv.VidPom;

                            // Цель посещения для телемедицины (Справочник V025: 2.6 - Посещение по другим обстоятельствам)
                            PRI_Sl.P_Cel = "2.6";

                            // Проверка на нахождения тарифа
                            if (PRI_StrahReestr.TARIF == null)
                            {
                                PRI_ErrorToExcel.PROP_ErrorCod = "47";
                                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тариф";
                                PRI_ErrorToExcel.MET_SaveError();
                                PRI_ErrorRow = true;
                                return;
                            }

                            // Пациент НЕ может быть направлен от нас, только с другого ЛПУ
                            if (PRI_Sl.NPR_MO == 555509)
                            {
                                PRI_ErrorToExcel.PROP_ErrorCod = "46";
                                PRI_ErrorToExcel.PROP_ErrorName = "(вну) В телемедицине ЛПУ направления NPR_MO не может быть наша (555509)";
                                PRI_ErrorToExcel.MET_SaveError();
                                PRI_ErrorRow = true;
                                return;
                            }
                        }
                    }
                }

                PRI_StrahReestr.CODE_USL = PRI_Sl.USL[PRI_CouUsl - 1].Code_Usl;
                PRI_StrahReestr.VID_VME = PRI_Sl.USL[PRI_CouUsl - 1].Usl;

                try
                {
                    PRI_StrahReestr.NOM_USL = JsonConvert.SerializeObject(PRI_Sl,
                                    Formatting.Indented,
                                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                }
                catch (Exception)
                {

                    PRI_StrahReestr.NOM_USL = "Ошибка json";
                }
            }
            MET_CalcAll();
        }
    }
}
