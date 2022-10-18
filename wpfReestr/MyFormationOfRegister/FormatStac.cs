using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using m = wpfReestr.MyMet;
using wpfStatic;
using e = Microsoft.Office.Interop.Excel;

namespace wpfReestr
{
    /// <summary>КЛАСС Формируем реестры</summary>
    public partial class MyReestr
    {
        /// <summary>МЕТОД 3. Расчет Стационара</summary>
        private void MET_CalcStac()
        {
            // Связываем ReStrahReestr и ReApstac
            DataRow _Apstac = PRI_RowReestr.GetChildRows("ReReestr_Apstac")[0];

            // Если ВМП, то расчитываем и заменяем данные
            bool _FlagVMP = m.MET_PoleStr("MetVMP", _Apstac) != "";
            if (_FlagVMP)
            {
                MySql.MET_DsAdapterFill(MyQuery.ReVMP_Select_1(m.MET_PoleStr("Cod", PRI_RowReestr)), "ReReestr_VMP");

                if (MyGlo.DataSet.Tables["ReReestr_VMP"].Rows.Count == 0) // если не нашли ВМП тариф, то ошибка
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "27";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тариф ВМП";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                }
                else
                {
                    DataRow _VMP = MyGlo.DataSet.Tables["ReReestr_VMP"].Rows[0];
                    _Apstac["PROFIL"] = _VMP["PROFIL"];
                    _Apstac["PROFIL_K"] = _VMP["PROFIL_K"];
                    _Apstac["CODE_USL"] = _VMP["CODE_USL"];
                    _Apstac["VID_VME"] = _VMP["VID_VME"];
                    _Apstac["Tarif"] = _VMP["Tarif"];
                    _Apstac["VID_HMP"] = _VMP["VID_HMP"];
                    _Apstac["METOD_HMP"] = _VMP["METOD_HMP"];
                    _Apstac["TAL_NUM"] = _VMP["TAL_NUM"];
                    _Apstac["TAL_D"] = _VMP["TAL_D"];
                    _Apstac["PRVSs"] = 41;
                    _Apstac["IDMODP"] = _VMP["IDMODP"];
                    _Apstac["HGR"] = _VMP["Hgr"];
                }
            }

            // PLAT -> SMO (Код страховой компании)                                   далее в  MET_CalcAll()
            PRI_StrahReestr.PLAT = _Apstac["ScomEnd"].ToString();

            // LPU_1 (Подразделения МО)
            PRI_StrahReestr.LPU_1 = m.MET_PoleInt("Korpus", _Apstac) == 1 ? 55550900 : 55550901;     // главный/филиал

            // ORDER -> EXTR (Направление на госпитализацию, 1 - плановая, 2 - экстренная (у нас нету), 0 - поликлиника)
            PRI_StrahReestr.ORDER = 1;

            // LPU_ST -> USL_OK (Условия оказания мед. помощи, справочник V006, 1 - кр. стационра, 2 - дн. стационар, 3 - поликлиника)
            PRI_StrahReestr.LPU_ST = m.MET_PoleInt("LPU_ST", _Apstac); ;

            // VIDPOM -> VID_POM (Вид помощи, справочник V008, у нас 13 - поликл, 14 - телемедицина, 31 - стационар, 32 - ВМП)
            PRI_StrahReestr.VIDPOM = PRI_StrahReestr.LPU_ST == 1 ? 31 : 13; // поставил 12.04.2021 (до этого была везде 31) если то это 13 дневной при поликлиники
            if (PRI_StrahReestr.VIDPOM == 13 & m.MET_PoleInt("OtdInPol", _Apstac) == 0)
                PRI_StrahReestr.VIDPOM = 31;    // дневной при стационаре
            if (_FlagVMP) PRI_StrahReestr.VIDPOM = 32;

            // PROFIL (Профиль, справочник V002)
            PRI_StrahReestr.PROFIL = m.MET_PoleDec("PROFIL", _Apstac);
            // Проверка
            if (PRI_StrahReestr.PROFIL == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "12";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден профиль";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
            }            

            // DET (Детский профиль, если ребёнок то 1 иначе 0)            далее в  MET_CalcAll()
            PRI_Age = m.MET_PoleInt("Age", _Apstac);

            // CODE_USL (Код услуги - для дополнительной оплаты и операций Код услуг прописан програмно, справочник услуг)
            PRI_StrahReestr.CODE_USL = m.MET_PoleStr("CODE_USL", _Apstac);

            // PRVS (Специальность врача, справочник V004)
            PRI_StrahReestr.PRVS = m.MET_PoleStr("PRVSs", _Apstac);

            // IDDOKT -> CODE_MD (Код врача, справочник врачей)
            PRI_StrahReestr.IDDOKT = m.MET_PoleStr("IDDOKT", _Apstac);

            // ARR_DATE -> DATE_1 -> DATE_IN (Дата начала)
            PRI_StrahReestr.ARR_DATE = m.MET_PoleDate("DN", _Apstac);

            // EX_DATE ->  DATE_2 -> DATE_OUT (Дата окончания)
            PRI_StrahReestr.EX_DATE = m.MET_PoleDate("DK", _Apstac);

            // PODR (Код отделения)
            if (PRI_StrahReestr.EX_DATE.Value.Year < 2022)
                PRI_StrahReestr.PODR = m.MET_ParseDec(m.MET_PoleStr("PODR", _Apstac) + ((int)PRI_StrahReestr.PROFIL).ToString("D3"));
            else
                PRI_StrahReestr.PODR = m.MET_ParseDec(m.MET_PoleStr("PODR", _Apstac));

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

            // DS2 (Сопутствующий Диагноз - типо Сахарный диабет)
            PRI_StrahReestr.DS2 = "";

            // PACIENTID -> NHISTORY (Номер истории болезни/талона)
            PRI_StrahReestr.PACIENTID = m.MET_PoleStr("Cod", PRI_RowReestr);

            // RES_G -> RSLT (Результат обращения/госпитализации, справочник V009)
            int[] _RES_G = { 0, 1, 4, 5 };
            int _Rezobr = m.MET_PoleInt("FlagOut", _Apstac);                                    // FlagOut 1 - выписан, 2 - переведен, 3 - умер
            // кр. стационар (101-выписан, 104-переведен, 105-умер)
            // дневной стационар (201-выписан, 204-переведен, 205-умер)
            PRI_StrahReestr.RES_G = 100 * PRI_StrahReestr.LPU_ST + _RES_G[_Rezobr];

            // ISHOD (Исход заболевания, справочник V012)
            int _ISXOD = m.MET_PoleInt("ISXOD", _Apstac);
            PRI_StrahReestr.ISHOD = _ISXOD == 6                                                 // если исход равен 6 = умер, то 4
                ? 100 * PRI_StrahReestr.LPU_ST + 4                                              // то исход 104 или 204 ухудшение, для кр. и дн. стац.
                : 100 * PRI_StrahReestr.LPU_ST + _ISXOD;                                        // исход 100... кр. стац и 200 дн. стац.

            // IDSP (Код способа оплаты, справочник V010: 33 - круг. стационар и дневн. страц, 29 или 30 - поликлиника, 32 - ВМП)
            PRI_StrahReestr.IDSP = 33;
            if (_FlagVMP) PRI_StrahReestr.IDSP = 32;

            // KOL_USL (Койко дней)
            PRI_StrahReestr.KOL_USL = m.MET_PoleDec("UET3", _Apstac);

            // TARIF (Тариф)
            PRI_StrahReestr.TARIF = m.MET_PoleDec("Tarif", _Apstac);
            // Проверка
            if (PRI_StrahReestr.TARIF == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "26";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тариф";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
            }

            // VPOLIS (Тип полиса 1 - старый, 2 - временный, 3 - новый)   далее в  MET_CalcAll()
            PRI_NPolis = m.MET_PoleStr("SNEnd", _Apstac);

            // SERIA -> SPOLIS (Серия полиса)
            PRI_StrahReestr.SERIA = m.MET_PoleStr("SSEnd", _Apstac).ToUpper();

            // DayN (Длительность лечения из Тарифов)
            PRI_StrahReestr.DayN = 0; //m.MET_PoleInt("DayN", _Apstac);

            // VID_VME (Вид медицинского вмешательства)
            PRI_StrahReestr.VID_VME = m.MET_PoleStr("VID_VME", _Apstac);

            // VID_HMP (Вид ВМП, типа 01.001.01, справочник V018)
            PRI_StrahReestr.VID_HMP = m.MET_PoleStr("VID_HMP", _Apstac);

            // METOD_HMP (Метод ВМП, число, справочник V019)
            PRI_StrahReestr.METOD_HMP = m.MET_PoleStr("METOD_HMP", _Apstac);

            MET_CalcAll();

            if (PRI_StrahReestr.EX_DATE.Value.Year < 2022)
                MET_CalcKsg2021(_Apstac);
            else
                MET_CalcKsg2022(_Apstac);

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

        /// <summary>МЕТОД 4. Расчет Стационара 2021 (Расчет КСГ)</summary>
        private void MET_CalcKsg2021(DataRow pApstac)
        {
            // ЛПУ направления
            PRI_Sl.NPR_MO = m.MET_PoleInt("NPR_MO", pApstac);

            // Дата направления
            PRI_Sl.NPR_DATE = m.MET_PoleStr("DN", pApstac); //m.MET_PoleStr("NPR_DATE", pApstac);
            //  _Sluch.NPR_DATE = _Sluch.NPR_DATE == "" ? m.MET_PoleStr("DN", pApstac) : _Sluch.NPR_DATE;
            PRI_Sl.NPR_DATE = PRI_Sl.NPR_DATE.Remove(10);

            // Переводы
            PRI_Sl.VB_P = m.MET_PoleInt("OtdIn", pApstac);

            // Профиль койки
            PRI_Sl.PROFIL_K = m.MET_PoleInt("PROFIL_K", pApstac);

            string _jTag = m.MET_PoleStr("jTag", pApstac);

            bool _Zno = PRI_StrahReestr.DS1.Substring(0, 1) == "C" || PRI_StrahReestr.DS1.Substring(0, 2) == "D0";
            if (string.IsNullOrEmpty(_jTag) && !_Zno)
                _jTag = "{ }";

            // Проверка на наличия данных KbolInfo
            if (string.IsNullOrEmpty(_jTag))
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "28";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найдена строка APSTAC в KbolInfo";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            JObject _JsonSL;
            try
            {
                _JsonSL = JObject.Parse(_jTag);
            }
            catch
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "30";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в KbolInfo";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // DS2 (Сопутствующий Диагноз - типо Сахарный диабет)
            PRI_StrahReestr.DS2 = (string)_JsonSL["Kslp_diag"] ?? "";

            //// Сопутствующий диагноз для D70
            //bool _DiagD70 = PRI_StrahReestr.DS1.Substring(0, 3) == "D70";
            //if (_DiagD70)
            //{
            //    PRI_StrahReestr.DS2 = (string)_JsonSL["DiagD70"];

            //    // Обязательно должен быть сопутствующий диагноз С
            //    if(string.IsNullOrEmpty(PRI_StrahReestr.DS2) || PRI_StrahReestr.DS2.Substring(0, 1) != "C")
            //    {
            //        PRI_ErrorToExcel.PROP_ErrorCod = "41";
            //        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Отстутствует или не правильный сопутствующий диагноз для D70 в KbolInfo";
            //        PRI_ErrorToExcel.MET_SaveError();
            //        PRI_ErrorRow = true;
            //        return;
            //    }

            //    _JsonSL["Klin_gr"] = "II";
            //    _JsonSL["DS1_T"] = 6;
            //}


            // Новое 285 приказ
            if (_Zno)
            {
                // УБРАЛ направления 28/07/2020 из за штрафов Капитала по превышению 7го срока и отсутствия точной даты направления
                // Направления на исследования, только для ЗНО
                //if (PRI_StrahReestr.EX_DATE >= new DateTime(2019, 12, 1))
                //{
                //    IList<JToken> _Results = _JsonSL["NAPR"]?.Children().ToList();
                //    if (_Results != null)
                //    {
                //        PRI_Sl.NAPR = new List<MyNAPR>();
                //        foreach (JToken _result in _Results)
                //        {
                //            MyNAPR _Napr = _result.ToObject<MyNAPR>();
                //            _Napr.NAPR_V = 3;
                //            _Napr.NAPR_DATE = PRI_StrahReestr.ARR_DATE.Value.ToString("dd.MM.yyyy");  // !!! Пока берем не из направления, а начальную дату случая
                //            PRI_Sl.NAPR.Add(_Napr);
                //        }
                //    }
                //}

                // Характер заболевания 1 - острое, 2 храническое впервые, 3 - хроническое повторно
                // Только для поликлиники или для стационара с диагнозом С..D09 (таблица Q018)
                PRI_Sl.C_ZAB = m.MET_PoleInt("C_Zab", pApstac);

                // Клиническая группа
                string _klin_gr = (string)_JsonSL["Klin_gr"];

                // Проверка на наличия Клинической группы в KbolInfo
                if (string.IsNullOrEmpty(_klin_gr))
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "29";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тег klin_gr (Клиническая группа) в KbolInfo";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }

                // Убрал Пока подозрение в стационаре
                //// DS_ONK - Если подозрение на ЗНО
                //if (_klin_gr == "Ia" || _klin_gr == "Ib")
                //    PRI_Sluch.DS_ONK = 1;
                //else
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
                    PRI_Sl.ONK_SL.DS1_T = (int?)_JsonSL["DS1_T"] ?? 0;
                    if (PRI_Sl.ONK_SL.DS1_T == 1 || PRI_Sl.ONK_SL.DS1_T == 2)
                    {
                        // MTSTZ - Отдаленные метастазы
                        PRI_Sl.ONK_SL.MTSTZ = 1;
                    }
                    if (PRI_Sl.ONK_SL.DS1_T == 21 || PRI_Sl.ONK_SL.DS1_T == 22)
                    {
                        PRI_Sl.ONK_SL.DS1_T = PRI_Sl.ONK_SL.DS1_T - 20;
                    }

                    // STAD - Стадия (только для DS1_T < 5)
                    if (PRI_Sl.ONK_SL.DS1_T < 5)
                    {
                        string _stadia = (string)_JsonSL["Stadia"];
                        var _N002 = PRI_N002.Where(e => e.Kod1 == _D5).ToList();
                        // Сначала пытаемся найти по полному диагнозу
                        if (!_N002.Any())
                            _N002 = PRI_N002.Where(e => e.Kod1 == _D3).ToList(); // Иначе ищем по 3м сиволам диагноза
                        if (!_N002.Any())
                            _N002 = PRI_N002.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                        int? _stadiaNumber = _N002.FirstOrDefault(e => e.Kod3.Contains($"\"{_stadia}\""))?.Number ??
                                             _N002.First()?.Number;
                        PRI_Sl.ONK_SL.STAD = _stadiaNumber ?? 145;
                    }

                    // TNM (заполняем только для взрослых и первичном лечении)
                    if (PRI_StrahReestr.DET == 0 && PRI_Sl.ONK_SL.DS1_T == 0)
                    {
                        // ONK_T - T
                        string _T = (string)_JsonSL["T"];
                        var _N003 = PRI_N003.Where(e => e.Kod1 == _D5).ToList();
                        // Сначала пытаемся найти по полному диагнозу
                        if (!_N003.Any())
                            _N003 = PRI_N003.Where(e => e.Kod1 == _D3).ToList(); // Иначе ищем по 3м сиволам диагноза
                        if (!_N003.Any())
                            _N003 = PRI_N003.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                        int? _TNumber = _N003.FirstOrDefault(e => e.Kod3.Contains($"\"{_T}\""))?.Number ??
                                        _N003.First()?.Number;
                        PRI_Sl.ONK_SL.ONK_T = _TNumber ?? 182;

                        // ONK_N - N
                        string _N = (string)_JsonSL["N"];
                        var _N004 = PRI_N004.Where(e => e.Kod1 == _D5).ToList();
                        // Сначала пытаемся найти по полному диагнозу
                        if (!_N004.Any())
                            _N004 = PRI_N004.Where(e => e.Kod1 == _D3).ToList(); // Иначе ищем по 3м сиволам диагноза
                        if (!_N004.Any())
                            _N004 = PRI_N004.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                        int? _NNumber = _N004.FirstOrDefault(e => e.Kod3.Contains($"\"{_N}\""))?.Number ??
                                        _N004.First()?.Number;
                        PRI_Sl.ONK_SL.ONK_N = _NNumber ?? 99;

                        // ONK_M - M
                        string _M = (string)_JsonSL["M"];
                        var _N005 = PRI_N005.Where(e => e.Kod1 == _D5).ToList();
                        // Сначала пытаемся найти по полному диагнозу
                        if (!_N005.Any())
                            _N005 = PRI_N005.Where(e => e.Kod1 == _D3).ToList(); // Иначе ищем по 3м сиволам диагноза
                        if (!_N005.Any())
                            _N005 = PRI_N005.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                        int? _MNumber = _N005.FirstOrDefault(e => e.Kod3.Contains($"\"{_M}\""))?.Number ??
                                        _N005.First()?.Number;
                        PRI_Sl.ONK_SL.ONK_M = _MNumber ?? 56;
                    }

                    // B_DIAG - Диагностический блок
                    // Гистология
                    string _Gisto = (string)_JsonSL["resulthistology"] ?? "";
                    if (!string.IsNullOrEmpty(_Gisto))
                    {
                        var _mGisto = _Gisto.Split(';');
                       // PRI_Sl.ONK_SL.B_DIAG = new List<MyB_DIAG>();
                        // Смотрим если такой диагноз в проверочном файле N009
                        var _N009 = PRI_N009.Where(e => e.Kod1.Contains(_D3)).ToList();
                        bool _DiagN009 = _N009.Any();
                        foreach (var _i in _mGisto)
                        {
                            if (int.TryParse(_i, out int j))
                            {
                                var _Daignostic = new MyB_DIAG();
                                _Daignostic.DIAG_DATE = MET_VerifDate((string)_JsonSL["DateDirectHistology"], "DateDirectHistology") ?? PRI_StrahReestr.ARR_DATE.Value.ToString("dd.MM.yyyy");
                                _Daignostic.DIAG_TIP = 1;
                                _Daignostic.DIAG_RSLT = j;
                                _Daignostic.DIAG_CODE = PRI_N008.FirstOrDefault(e => e.Number == j).ID1.Value;
                                if (_DiagN009)
                                {
                                    if (_N009.Any(e => e.ID1 == _Daignostic.DIAG_CODE))
                                    {
                                        if (PRI_Sl.ONK_SL.B_DIAG == null)
                                            PRI_Sl.ONK_SL.B_DIAG = new List<MyB_DIAG>();
                                        PRI_Sl.ONK_SL.B_DIAG.Add(_Daignostic);
                                    }
                                }
                                //else
                                //    PRI_Sl.ONK_SL.B_DIAG.Add(_Daignostic);
                            }
                        }
                    }
                    // ИГХ
                    _Gisto = (string)_JsonSL["markerigh"] ?? "";
                    if (!string.IsNullOrEmpty(_Gisto))
                    {
                        var _mGisto = _Gisto.Split(';');
                        // Смотрим если такой диагноз в проверочном файле N009
                        var _N012 = PRI_N012.Where(e => e.Kod1.Contains(_D3)).ToList();

                        bool _DiagN012 = _N012.Any();
                        foreach (var _i in _mGisto)
                        {
                            if (int.TryParse(_i, out int j))
                            {
                                var _Daignostic = new MyB_DIAG();
                                _Daignostic.DIAG_DATE = MET_VerifDate((string)_JsonSL["DateDirectHistology"], "DateDirectHistology") ?? PRI_StrahReestr.ARR_DATE.Value.ToString("dd.MM.yyyy");
                                _Daignostic.DIAG_TIP = 2;
                                _Daignostic.DIAG_RSLT = j;
                                _Daignostic.DIAG_CODE = PRI_N011.FirstOrDefault(e => e.Number == j).ID1.Value;
                                if (_DiagN012)
                                {
                                    if (_N012.Any(e => e.ID1 == _Daignostic.DIAG_CODE))
                                    {
                                        if (PRI_Sl.ONK_SL.B_DIAG == null)
                                            PRI_Sl.ONK_SL.B_DIAG = new List<MyB_DIAG>();
                                        PRI_Sl.ONK_SL.B_DIAG.Add(_Daignostic);
                                    }
                                }
                                //else
                                //    PRI_Sl.ONK_SL.B_DIAG.Add(_Daignostic);
                            }
                        }
                    }

                    // CONS Блок о проведении консилиума
                    // Связываем ReApstac и ReApstacCons
                    foreach (DataRow _ApstacRowCons in pApstac.GetChildRows("ReApstac_ApstacCons"))
                    {
                        switch (m.MET_PoleInt("PR_CONS", _ApstacRowCons))
                        {
                            case 1:
                                PRI_Sl.Taktika_1 = MET_VerifDate(m.MET_PoleStr("DT_CONS", _ApstacRowCons), $"консилиума", true, false);
                                if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                break;
                            case 2:
                                PRI_Sl.Taktika_2 = MET_VerifDate(m.MET_PoleStr("DT_CONS", _ApstacRowCons), $"консилиума", true, false);
                                if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                break;
                            case 3:
                                PRI_Sl.Taktika_3 = MET_VerifDate(m.MET_PoleStr("DT_CONS", _ApstacRowCons), $"консилиума", true, false);
                                if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                break;
                        }
                    }

                    // B_PROT Блок об имеющихся противопоказаниях и отказах
                    PRI_Sl.PrOt_1 = MET_VerifDate((string)_JsonSL["PrOt_1"], "PrOt_1", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_2 = MET_VerifDate((string)_JsonSL["PrOt_2"], "PrOt_2", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_3 = MET_VerifDate((string)_JsonSL["PrOt_3"], "PrOt_3", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_4 = MET_VerifDate((string)_JsonSL["PrOt_4"], "PrOt_4", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_5 = MET_VerifDate((string)_JsonSL["PrOt_5"], "PrOt_5", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_6 = MET_VerifDate((string)_JsonSL["PrOt_6"], "PrOt_6", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                }
            }

            int _CountUsl = pApstac.GetChildRows("ReApstac_Ksg").Length;

            if (_Zno && PRI_Sl.DS_ONK == 0 && PRI_Sl.ONK_SL.DS1_T < 3)
                PRI_Sl.ONK_SL.ONK_USL = new List<MyONK_USL>();

            PRI_CouUsl = 0;
            // Связываем ReApstac и ReKsg
            foreach (DataRow _KsgRow in pApstac.GetChildRows("ReApstac_Ksg"))
            {
                var _MyUSL = new MyUSL
                {
                    Tip = m.MET_PoleStr("Tip", _KsgRow),
                    Usl = m.MET_PoleStr("Usl", _KsgRow),
                    DopUsl = m.MET_PoleStr("DopUsl", _KsgRow),
                    Frakc = m.MET_PoleInt("FrakcT", _KsgRow),
                    DatN = m.MET_PoleStr("Dat", _KsgRow),
                    Ksg = m.MET_PoleStr("KSG", _KsgRow),
                    Fact = m.MET_PoleRea("Factor", _KsgRow),
                    UprFactor = m.MET_PoleRea("UprFactor", _KsgRow),
                    KUSmo = m.MET_PoleInt("KUSmo", _KsgRow),
                    Day3 = m.MET_PoleInt("Day3", _KsgRow),
                    Dzp = m.MET_PoleRea("Dzp", _KsgRow)
                };

                //// Удаляем все химии, если это не первая услуга!!!!!
                //if (PRI_CouUsl > 0 && ((_MyUSL.Usl.StartsWith("sh") && !PRI_Sl.USL[0].Usl.StartsWith("sh"))
                //        || (_MyUSL.Usl.StartsWith("gem") && !PRI_Sl.USL[0].Usl.StartsWith("gem"))))
                // Поставил в связи с ошибкой на дубль USL_TIP 02.04.2021 (Ни каких бумаг на этот счет нет)

                // Удаляем все НЕ первые услуги 01.03.2021
                if (PRI_CouUsl > 0)
                {
                    _CountUsl--;
                    continue;
                }
                PRI_CouUsl++;

                // Удаляем если пусто
                _MyUSL.DatN = _MyUSL.DatN == "" ? null : _MyUSL.DatN.Remove(10);
                _MyUSL.DatK = MET_VerifDate(_MyUSL.DatN, "DatN", true);
                if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                _MyUSL.DopUsl = _MyUSL.DopUsl == "" ? null : _MyUSL.DopUsl;
                if (PRI_StrahReestr.KOL_USL > 3) _MyUSL.Day3 = null;

                // Для НЕ ЗНО и Радиологии, записываем фракции в CRIT
                if (!_Zno && _MyUSL.Frakc > 0)
                {
                    string _FracCRIT;
                    // Если фракций нет в Группировщике, то сами их рисуем
                    if (_MyUSL.Frakc <= 5) _FracCRIT = "fr01-05";
                    else if (_MyUSL.Frakc <= 7) _FracCRIT = "fr06-07";
                    else if (_MyUSL.Frakc <= 10) _FracCRIT = "fr08-10";
                    else if (_MyUSL.Frakc <= 20) _FracCRIT = "fr11-20";
                    else if (_MyUSL.Frakc <= 29) _FracCRIT = "fr21-29";
                    else if (_MyUSL.Frakc <= 32) _FracCRIT = "fr30-32";
                    else _FracCRIT = "fr33-99";

                    // Добавляем фракции в CRIT
                    if (PRI_Sl.CRIT == null)
                        PRI_Sl.CRIT = new List<string>();
                    PRI_Sl.CRIT.Add(_FracCRIT);

                    string _xInfo = m.MET_PoleStr("xInfo", _KsgRow);

                    _xInfo = string.IsNullOrEmpty(_xInfo) ? "{}" : _xInfo;
                    JObject _JsonUsl;
                    try
                    {
                        _JsonUsl = JObject.Parse(_xInfo);
                    }
                    catch
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "31";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в услугах Oper";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }

                    string[] _mDates = ((string)_JsonUsl["TLuch_Date"]).Split(';');
                    DateTime _DateNU = DateTime.Parse(_mDates[0]);
                    DateTime _DateKU = DateTime.Parse(_mDates[_mDates.Length - 2]);
                    _MyUSL.DatN = _DateNU.ToString("dd.MM.yyyy");
                    _MyUSL.DatK = _DateKU.ToString("dd.MM.yyyy");
                    if (PRI_StrahReestr.ARR_DATE > _DateNU || PRI_StrahReestr.EX_DATE < _DateKU)
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "33";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) В теге радиологии, даты облучения выходят за диапазон стационара";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }
                }

                // Для 285 приказа
                if (_Zno && PRI_Sl.DS_ONK == 0 && PRI_Sl.ONK_SL.DS1_T < 3)
                {
                    Random _Random = new Random();
                    string _xInfo = m.MET_PoleStr("xInfo", _KsgRow);
                    _xInfo = string.IsNullOrEmpty(_xInfo) ? "{}" : _xInfo;
                    JObject _JsonUsl;
                    try
                    {
                        _JsonUsl = JObject.Parse(_xInfo);
                    }
                    catch
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "31";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в услугах Oper";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }

                    MyONK_USL _MyOnkUsl = new MyONK_USL();

                    // Тип услуги по умолчанию
                    _MyOnkUsl.USL_TIP = 5;

                    // Если есть только диагноз
                    // if (_CountUsl == 1 && _MyUSL.Tip == "диаг") убрал 01.06.21
                    if (PRI_CouUsl == 1 && _MyUSL.Tip == "диаг")
                    {
                        // Тип услуги
                        _MyOnkUsl.USL_TIP = 5;
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Биопсия, исследования и др.
                    if (_MyUSL.Usl.StartsWith("A11") || _MyUSL.Usl.StartsWith("B03") || _MyUSL.Usl.StartsWith("A03"))
                    {
                        // Тип услуги
                        _MyOnkUsl.USL_TIP = 5;
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Хирургического лечение
                    if (_MyUSL.Usl.StartsWith("A16"))
                    {
                        // Тип услуги
                        _MyOnkUsl.USL_TIP = 1;
                        // Тип хирургического лечения (ПО УМОЛЧАНИЮ 1)
                        int _Hir = (int?)_JsonUsl["THir"] ?? 0;
                        _MyOnkUsl.HIR_TIP = _Hir > 0 & _Hir < 6 ? _Hir : 1;
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Химия
                    if (_MyUSL.Usl.StartsWith("sh") || _MyUSL.Usl.StartsWith("A25") || _MyUSL.Usl.StartsWith("gem"))
                    {
                        _MyOnkUsl.LEK_PR = new List<MyLEK_PR>();
                        // Тип услуги
                        _MyOnkUsl.USL_TIP = 2;
                        // Тип Линия  (ПО УМОЛЧАНИЮ РАНДОМ ЗАМЕНИТЬ)
                        int _TIP_L = (int?)_JsonUsl["TLek_L"] ?? 0;
                        _MyOnkUsl.LEK_TIP_L = _TIP_L > 0 ? _TIP_L : _Random.Next(1, 4);
                        // Тип Цикл  (ПО УМОЛЧАНИЮ РАНДОМ ЗАМЕНИТЬ)
                        int _TIP_V = (int?)_JsonUsl["TLek_V"] ?? 0;
                        _MyOnkUsl.LEK_TIP_V = _TIP_V > 0 ? _TIP_V : _Random.Next(1, 2);

                        // Тошнота (РАНДОМ ЗАМЕНИТЬ)
                        int _PPTR = (int?)_JsonUsl["PPTR"] ?? 0;
                        if (_PPTR == 1 || _Random.Next(1, 8) == 1)
                            _MyOnkUsl.PPTR = 1;

                        // Полный список всех дат, при сверхкоротком случае (что бы посчетать колличество ВСЕХ дней введения)
                        var _DateList = new List<string>();

                        DateTime _DateN = DateTime.Parse(_MyUSL.DatN);
                        DateTime _DateK = DateTime.Parse(_MyUSL.DatK);

                        // Загружаем препараты
                        for (int i = 1; i < 6; i++)
                        {
                            string _RegNum = (string)_JsonUsl[$"TLek_MNN{i}"] ?? "";
                            if (_RegNum != "")
                            {
                                // Проверяем код МНН по шаблону (6 цифр)
                                Regex _Regex = new Regex(@"\d{6}");
                                if (!_Regex.IsMatch(_RegNum))
                                {
                                    PRI_ErrorToExcel.PROP_ErrorCod = "32";
                                    PRI_ErrorToExcel.PROP_ErrorName = $"(вну) В теге химии стоит неправильный код МНН {_RegNum}";
                                    PRI_ErrorToExcel.MET_SaveError();
                                    PRI_ErrorRow = true;
                                    return;
                                }

                                string _Date = (string)_JsonUsl[$"TLek_Date{i}"] ?? "";
                                if (_Date == "")
                                {
                                    PRI_ErrorToExcel.PROP_ErrorCod = "32";
                                    PRI_ErrorToExcel.PROP_ErrorName = $"(вну) В теге химии, для {i}-го препарата с кодом {_RegNum} отсутствуют даты введения";
                                    PRI_ErrorToExcel.MET_SaveError();
                                    PRI_ErrorRow = true;
                                    return;
                                }

                                var _LekPr = new MyLEK_PR();
                                _LekPr.REGNUM = _RegNum;
                                _LekPr.CODE_SH = _MyUSL.Usl.StartsWith("sh") || _MyUSL.Usl.StartsWith("gem") ? _MyUSL.Usl : "нет";
                                _LekPr.DATE_INJ = new List<string>();

                                string[] _mDates = _Date.Split(';');
                                foreach (var _d in _mDates)
                                {
                                    if (DateTime.TryParse(_d, out DateTime _DateTime))
                                    {
                                        if (_DateN > _DateTime)
                                        {
                                            _DateN = _DateTime;
                                            _MyUSL.DatN = MET_VerifDate(_DateN.ToString("dd.MM.yyyy"), $"химии, для {i}-го препарата с кодом {_RegNum} дата введения", true);
                                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                        }
                                        if (_DateK < _DateTime)
                                        {
                                            _DateK = _DateTime;
                                            _MyUSL.DatK = MET_VerifDate(_DateK.ToString("dd.MM.yyyy"), $"химии, для {i}-го препарата с кодом {_RegNum} дата введения", true);
                                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                        }
                                        _LekPr.DATE_INJ.Add(_DateTime.ToString("dd.MM.yyyy"));
                                    }
                                }
                                _MyOnkUsl.LEK_PR.Add(_LekPr);

                                //// Если сверхкороткий случай, то набираем стек дат, со всех выданных лекарств
                                //if (PRI_StrahReestr.KOL_USL < 4)
                                //{
                                //    _DateList.AddRange(_LekPr.DATE_INJ);
                                //}
                                // Набираем стек дат, со всех выданных лекарств
                                _DateList.AddRange(_LekPr.DATE_INJ);
                            }
                        }

                        // Колличество дней схемы лечения
                        int _DayHim = m.MET_PoleInt("DayHim", _KsgRow);

                        // Если схема не выполненна, то случай прерванный
                        if (_DateList.Distinct().Count() < _DayHim)
                        {
                            PRI_Sl.Prervan = true;
                            // Койко дней менее 4, то считаем случай - сверхкороткий
                            if (PRI_StrahReestr.KOL_USL < 4)
                                _MyUSL.Day3 = 0;
                        }

                        //// Если дней госпитализации менее 4 дней, то смотрим выполнение дней схемы лечения
                        //if (PRI_StrahReestr.KOL_USL < 4)
                        //{
                        //    // Колличество дней схемы лечения
                        //    int _DayHim = m.MET_PoleInt("DayHim", _KsgRow);

                        //    // Если схема не выполненна, то считаем случай - сверхкороткий
                        //    if (_DateList.Distinct().Count() < _DayHim)
                        //        _MyUSL.Day3 = 0;
                        //}

                        // Проверяем, есть ли хоть один препарат
                        if (_MyOnkUsl.LEK_PR.Count == 0)
                        {
                            PRI_ErrorToExcel.PROP_ErrorCod = "36";
                            PRI_ErrorToExcel.PROP_ErrorName = $"(вну) В услуге химии {_MyUSL.Usl} отсутствуют теги с препаратами";
                            PRI_ErrorToExcel.MET_SaveError();
                            PRI_ErrorRow = true;
                            return;
                        }

                        if (_MyUSL.Usl.StartsWith("sh") || _MyUSL.Usl.StartsWith("gem"))
                        {
                            if (PRI_Sl.CRIT == null)
                                PRI_Sl.CRIT = new List<string>();
                            PRI_Sl.CRIT.Add(_MyUSL.Usl);
                        }
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Радиология
                    if (_MyUSL.Usl.StartsWith("A06") || _MyUSL.Usl.StartsWith("A07"))
                    {
                        // Тип услуги
                        _MyOnkUsl.USL_TIP = 3;
                        // Тип Линия  (ПО УМОЛЧАНИЮ РАНДОМ ЗАМЕНИТЬ)
                        int _LUCH_TIP = (int?)_JsonUsl["TLuch"] ?? 0;
                        _MyOnkUsl.LUCH_TIP = _LUCH_TIP > 0 ? _LUCH_TIP : 1;
                        // Проверка на SOD
                        int _SOD = (int?)_JsonSL["Sod"] ?? 0;
                        if (_SOD > 0)
                            PRI_Sl.ONK_SL.SOD = _SOD;
                        else
                            PRI_Sl.ONK_SL.SOD = 20;

                        // Колличество фракций
                        _MyOnkUsl.K_FR = (int?)_JsonUsl["Frakci"] ?? -1;

                        if (_MyOnkUsl.K_FR > 0)
                        {
                            string _FracCRIT = m.MET_PoleStr("FrakcText", _KsgRow);
                            // Если фракций нет в Группировщике, то сами их рисуем
                            if (_FracCRIT == "")
                            {
                                if (_MyOnkUsl.K_FR <= 5) _FracCRIT = "fr01-05";
                                else if (_MyOnkUsl.K_FR <= 7) _FracCRIT = "fr06-07";
                                else if (_MyOnkUsl.K_FR <= 10) _FracCRIT = "fr08-10";
                                else if (_MyOnkUsl.K_FR <= 20) _FracCRIT = "fr11-20";
                                else if (_MyOnkUsl.K_FR <= 29) _FracCRIT = "fr21-29";
                                else if (_MyOnkUsl.K_FR <= 32) _FracCRIT = "fr30-32";
                                else _FracCRIT = "fr33-99";
                            }

                            // Добавляем фракции в CRIT
                            if (PRI_Sl.CRIT == null)
                                PRI_Sl.CRIT = new List<string>();
                            PRI_Sl.CRIT.Add(_FracCRIT);
                        }

                        string[] _mDates = ((string)_JsonUsl["TLuch_Date"]).Split(';');
                        DateTime _DateNU = DateTime.Parse(_mDates[0]);
                        DateTime _DateKU = DateTime.Parse(_mDates[_mDates.Length - 2]);
                        _MyUSL.DatN = _DateNU.ToString("dd.MM.yyyy");
                        _MyUSL.DatK = _DateKU.ToString("dd.MM.yyyy");
                        if (PRI_StrahReestr.ARR_DATE > _DateNU || PRI_StrahReestr.EX_DATE < _DateKU)
                        {
                            PRI_ErrorToExcel.PROP_ErrorCod = "33";
                            PRI_ErrorToExcel.PROP_ErrorName = "(вну) В теге радиологии, даты облучения выходят за диапазон стационара";
                            PRI_ErrorToExcel.MET_SaveError();
                            PRI_ErrorRow = true;
                            return;
                        }

                        if (_MyUSL.DopUsl != null && _MyUSL.DopUsl.StartsWith("mt"))
                        {
                            _MyOnkUsl.LEK_PR = new List<MyLEK_PR>();
                            _MyOnkUsl.USL_TIP = 4;
                            if (PRI_Sl.CRIT == null)
                                PRI_Sl.CRIT = new List<string>();
                            PRI_Sl.CRIT.Add(_MyUSL.DopUsl);

                            string _RegNum = (string)_JsonUsl["TLek_MNN1"] ?? "";
                            if (_RegNum != "")
                            {
                                // Проверяем код МНН по шаблону (6 цифр)
                                Regex _Regex = new Regex(@"\d{6}");
                                if (!_Regex.IsMatch(_RegNum))
                                {
                                    PRI_ErrorToExcel.PROP_ErrorCod = "33";
                                    PRI_ErrorToExcel.PROP_ErrorName =
                                        $"(вну) В теге химии у ЛУЧЕВОЙ терапии стоит неправильный код МНН {_RegNum}";
                                    PRI_ErrorToExcel.MET_SaveError();
                                    PRI_ErrorRow = true;
                                    return;
                                }

                                string _Date = (string)_JsonUsl["TLek_Date1"] ?? "";
                                if (_Date == "")
                                {
                                    PRI_ErrorToExcel.PROP_ErrorCod = "33";
                                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) В теге химии у ЛУЧЕВОЙ терапии отсутствуют даты введения";
                                    PRI_ErrorToExcel.MET_SaveError();
                                    PRI_ErrorRow = true;
                                    return;
                                }

                                var _LekPr = new MyLEK_PR();
                                _LekPr.REGNUM = _RegNum;
                                _LekPr.CODE_SH = _MyUSL.DopUsl;
                                _LekPr.DATE_INJ = new List<string>();

                                DateTime _DateN = DateTime.Parse(_MyUSL.DatN);
                                DateTime _DateK = DateTime.Parse(_MyUSL.DatK);

                                _mDates = _Date.Split(';');
                                foreach (var _d in _mDates)
                                {
                                    if (DateTime.TryParse(_d, out DateTime _DateTime))
                                    {
                                        if (_DateN > _DateTime)
                                        {
                                            _DateN = _DateTime;
                                            _MyUSL.DatN = MET_VerifDate(_DateN.ToString("dd.MM.yyyy"), $"химии у ЛУЧЕВОЙ терапии дата введения", true);
                                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                        }
                                        if (_DateK < _DateTime)
                                        {
                                            _DateK = _DateTime;
                                            _MyUSL.DatK = MET_VerifDate(_DateK.ToString("dd.MM.yyyy"), $"химии у ЛУЧЕВОЙ терапии дата введения", true);
                                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                        }
                                        _LekPr.DATE_INJ.Add(_DateTime.ToString("dd.MM.yyyy"));
                                    }
                                }
                                _MyOnkUsl.LEK_PR.Add(_LekPr);
                            }
                            else
                            {
                                PRI_ErrorToExcel.PROP_ErrorCod = "38";
                                PRI_ErrorToExcel.PROP_ErrorName = $"(вну) В лучевой химии отсутствуют сведенья о препарате {_RegNum}";
                                PRI_ErrorToExcel.MET_SaveError();
                                PRI_ErrorRow = true;
                                return;
                            }

                        }
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Проверяем на наличие онко услуг
                    if (PRI_Sl.ONK_SL.ONK_USL.Count == 0)
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "37";
                        PRI_ErrorToExcel.PROP_ErrorName = $"(вну) Какая то Неведомая услуга {_MyUSL.Usl}";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }
                }
                PRI_Sl.USL.Add(_MyUSL);
            }

            // Проверяем есть химия
            if (PRI_Sl.ONK_SL != null && PRI_Sl.ONK_SL.ONK_USL != null && PRI_Sl.ONK_SL.ONK_USL.Any(p => p.USL_TIP == 2 || p.USL_TIP == 4))
            {
                try
                {
                    // Вес
                    PRI_Sl.ONK_SL.WEI = (double?)_JsonSL["Ves"] ?? new Random().Next(50, 100);
                    if (PRI_Sl.ONK_SL.WEI == 0)
                        PRI_Sl.ONK_SL.WEI = new Random().Next(50, 100);
                    // Рост
                    PRI_Sl.ONK_SL.HEI = (int?)_JsonSL["Rost"] ?? new Random().Next(155, 185);
                    if (PRI_Sl.ONK_SL.HEI == 0)
                        PRI_Sl.ONK_SL.HEI = new Random().Next(155, 185);
                    // Объем тела
                    PRI_Sl.ONK_SL.BSA = Math.Round(Math.Sqrt(PRI_Sl.ONK_SL.WEI * PRI_Sl.ONK_SL.HEI / 3600), 2);
                }
                catch
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "58";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Некорректное значение в тегах Рост или Вес (должно быть число)";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;

                }
            }

            // Если не нашли услуги в группировщике, то выводим ошибку
            if (PRI_Sl.USL.Count == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "57";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найдена ни одна услуга в Групировщике по диагнозу и(или) услуге";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // Исключения по КСГ (приложение 17)
            if (PRI_CouUsl > 1)
            {
                // Если выигрывает диагноз, но есть такая услуга, то берем услугу
                if (PRI_Sl.USL[1].Tip == "опер" && PRI_Sl.USL[0].Tip == "диаг" &&
                    ((PRI_Sl.USL[1].Ksg == "st02.010" && PRI_Sl.USL[0].Ksg == "st02.008") ||
                        (PRI_Sl.USL[1].Ksg == "st02.011" && PRI_Sl.USL[0].Ksg == "st02.008") ||
                        (PRI_Sl.USL[1].Ksg == "st02.010" && PRI_Sl.USL[0].Ksg == "st02.009") ||
                        (PRI_Sl.USL[1].Ksg == "st14.001" && PRI_Sl.USL[0].Ksg == "st04.002") ||
                        (PRI_Sl.USL[1].Ksg == "st14.002" && PRI_Sl.USL[0].Ksg == "st04.002") ||
                        (PRI_Sl.USL[1].Ksg == "st21.001" && PRI_Sl.USL[0].Ksg == "st21.007") ||
                        (PRI_Sl.USL[1].Ksg == "st34.002" && PRI_Sl.USL[0].Ksg == "st34.001") ||
                        (PRI_Sl.USL[1].Ksg == "st34.002" && PRI_Sl.USL[0].Ksg == "st26.001")))
                {
                    PRI_Sl.USL.Remove(PRI_Sl.USL[0]);
                    PRI_CouUsl--;
                }
            }

            // Если дневной стационар или ВМП, то пропускаем КСЛП
            if (PRI_StrahReestr.LPU_ST == 1 && PRI_StrahReestr.METOD_HMP == "")
            {
                // Смотрим КСЛП для круглосуточного стационара

                // Коэффициент дифференциации
                PRI_Sl.KOEF_D = 1.105;

                // Возраст, при поступлении
                DateTime _Dn = PRI_StrahReestr.ARR_DATE.Value;
                DateTime _DR = PRI_StrahReestr.VOZRAST.Value;
                int _Age = _Dn.Year - _DR.Year;
                if (_Dn.Month < _DR.Month || (_Dn.Month == _DR.Month && _Dn.Day < _DR.Day)) _Age--;

                //// КСЛП 13 - дети до года (приложение 17.1)
                //if (_Age < 1)
                //{
                //    PRI_Sl.Sl13 = 0.15; // до года
                //    PRI_Sl.IT_SL += PRI_Sl.Sl13;
                //}

                //// КСЛП 14 - дети от 1 до 4 лет (приложение 17.1)
                //if (_Age < 4 && _Age > 0)
                //{
                //    PRI_Sl.Sl14 = 0.1; // до года
                //    PRI_Sl.IT_SL += PRI_Sl.Sl14;
                //}

                // КСЛП 03 - дети до 4 лет (приложение 19)
                if (_Age < 4 && _Age >= 0)
                {
                    PRI_Sl.Sl03 = 0.2; // до года
                    PRI_Sl.IT_SL += PRI_Sl.Sl03;
                }

                // 10 - Сверх длинный случай (после 30 и 45 дня) 70
                if (PRI_StrahReestr.KOL_USL > 70) // > 30
                {
                    // КСГ, при которых сверхдлительность НЕ учитывается (лучевая терапия, тарифное соглашение)
                    bool _NkdNO = new[]
                    {
                        "st19.075", "st19.076", "st19.077", "st19.078", "st19.079", "st19.080", "st19.081", "st19.082", "st19.083"
                        , "st19.084", "st19.085", "st19.086", "st19.087", "st19.088", "st19.089"
                    }.Contains(PRI_Sl.USL[0].Ksg);

                    if (!_NkdNO)
                    {

                        //// КСГ, при которых сверхдлительность идет, только после 45 дня (приложение 17)
                        //bool _Nkd45 =
                        //    new[] { "st10.001", "st10.002", "st32.006", "st32.007" }.Contains(PRI_Sl.USL[0].Ksg);

                        // Больше 30 дней (по новому больше 70)
                        //if (!_Nkd45)
                        //{
                            PRI_Sl.Sl10 = 0.5;
                            PRI_Sl.IT_SL += PRI_Sl.Sl10;
                       // }

                        //// Больше 45 дней
                        //if (_Nkd45 && PRI_StrahReestr.KOL_USL > 45)
                        //{
                        //    PRI_Sl.Sl10 = Math.Round((Convert.ToDouble(PRI_StrahReestr.KOL_USL) - 45) / 45 * 0.3, 2);
                        //    PRI_Sl.IT_SL += PRI_Sl.Sl10;
                        //}
                    }
                }

                PRI_Sl.IT_SL = Math.Round(PRI_Sl.IT_SL + 1, 2);

                // Коэффицент подуровня стационара (КУСмо)
                //if (PRI_Sl.USL[0].KUSmo == 0) устарел с 2021 - а нифига, в феврале вернули
                //// Если не сказанно, что нужно ингорировать коэффициет подуровня (из dbo.StrahKsg поля KUSmo = 1)
                //{
                    PRI_Sl.KOEF_U = 1.2;

                    // Для ВМП отделений другой коэффициент
                    if (PRI_StrahReestr.EX_DATE >= new DateTime(2021, 1, 1))
                    {
                        if (new[] { 11121060m, 11361060m, 11081060m, 10991060m, 10761076m, 10761097m, 10761060m }.Contains((decimal)PRI_StrahReestr.PODR))
                            PRI_Sl.KOEF_U = 1.40;
                    }
                //}
                //else
                //    PRI_Sl.KOEF_U = 1;

                // Коэфициент затратоёмкости
                PRI_Sl.KOEF_Z = PRI_Sl.USL[0].Fact;

                // Управленчиский коэффицент
                PRI_Sl.KOEF_UP = PRI_Sl.USL[0].UprFactor;

                // Если взрослая химия
                if (PRI_Sl.USL[0]?.Dzp > 0)
                {
                    PRI_Sl.KOEF_Dzp = (double)PRI_Sl.USL[0]?.Dzp;
                    PRI_Sl.SUMV = (double)PRI_StrahReestr.TARIF * PRI_Sl.KOEF_Z *
                                        (1 - PRI_Sl.KOEF_Dzp + PRI_Sl.KOEF_Dzp * PRI_Sl.KOEF_UP *
                                        PRI_Sl.IT_SL * PRI_Sl.KOEF_U * PRI_Sl.KOEF_D);
                }
                else
                {
                    // Основной коэффициет
                    //PRI_Sl.SUMV = (double)PRI_StrahReestr.TARIF * PRI_Sl.KOEF_Z *
                    //                    PRI_Sl.KOEF_UP * PRI_Sl.IT_SL * PRI_Sl.KOEF_U;
                    PRI_Sl.SUMV = 25927.13107024 * PRI_Sl.KOEF_Z *
                            PRI_Sl.KOEF_UP * PRI_Sl.IT_SL * PRI_Sl.KOEF_U;
                }


                if (PRI_Sl.IT_SL == 1)
                    PRI_Sl.IT_SL = 0;

                // Сверх короткий случай
                if (PRI_StrahReestr.KOL_USL < 4 && PRI_Sl.USL[0]?.Day3 != 1)
                {
                    // 0.8 только операциям
                    if (PRI_Sl.USL[0].Tip == "опер" && (PRI_Sl.USL[0].Usl.StartsWith("A16") || PRI_Sl.USL[0].Usl.StartsWith("A11") || PRI_Sl.USL[0].Usl.StartsWith("A03")))
                        PRI_Sl.Short = 0.8;
                    else
                        PRI_Sl.Short = 0.4;

                    PRI_Sl.SUMV *= PRI_Sl.Short;
                }
                else 
                {
                    // если случай прерван (химия)
                    if (PRI_Sl.Prervan)
                    {
                        PRI_Sl.Short = 0.8;
                        PRI_Sl.SUMV *= PRI_Sl.Short;
                    }
                }
            }
            else
            {
                // Если дневной стационар
                if (PRI_StrahReestr.LPU_ST == 2)
                {
                    // Коэфициент затратоёмкости
                    PRI_Sl.KOEF_Z = PRI_Sl.USL[0].Fact;

                    // Управленчиский коэффицент
                    PRI_Sl.KOEF_UP = PRI_Sl.USL[0].UprFactor;

                    // Коэффициент дифференциации
                    PRI_Sl.KOEF_D = 1.105;

                    // КСЛП пока 1
                    //PRI_Sl.IT_SL = 1;

                    // Коэффицент подуровня стационара (КУСмо)
                    //// Если не сказанно, что нужно ингорировать коэффициет подуровня (из dbo.StrahKsg поля KUSmo = 1)
                    //PRI_Sl.KOEF_U = PRI_Sl.USL[0].KUSmo == 0 ? 1.2 : 1;
                    PRI_Sl.KOEF_U = 1.2;

                    // Если взрослая химия
                    if (PRI_Sl.USL[0]?.Dzp > 0)
                    {
                        PRI_Sl.KOEF_Dzp = (double)PRI_Sl.USL[0]?.Dzp;
                        PRI_Sl.SUMV = (double)PRI_StrahReestr.TARIF * PRI_Sl.KOEF_Z *
                                            (1 - PRI_Sl.KOEF_Dzp + PRI_Sl.KOEF_Dzp * PRI_Sl.KOEF_UP *
                                             PRI_Sl.KOEF_U * PRI_Sl.KOEF_D);
                    }
                    else
                    {
                        // Основной коэффициет
                        //PRI_Sl.SUMV = (double)PRI_StrahReestr.TARIF * PRI_Sl.KOEF_Z *
                        //                    PRI_Sl.KOEF_UP * PRI_Sl.KOEF_U *
                        //                    PRI_Sl.KOEF_D;

                        // НФЗ – средний норматив финансовых затрат
                        double _NFZ;
                        if (PRI_StrahReestr.EX_DATE.Value.Year == 2021 && PRI_StrahReestr.EX_DATE.Value.Month < 3 )
                            _NFZ = 14696.53367484; // 24 466.60 * 0,6006774 = 14696.53367484
                        else
                            _NFZ = 14680.09945962; // 24 466.60 * 0,6000057 = 14680.09945962

                        PRI_Sl.SUMV = _NFZ * PRI_Sl.KOEF_Z * PRI_Sl.KOEF_UP * PRI_Sl.KOEF_U;
                    }

                    // Сверх короткий случай (если нет операций (не Ашки), а только диагноз)
                    if (PRI_StrahReestr.KOL_USL < 4 && PRI_Sl.USL[0].Day3 != 1)
                    {
                        // 0.8 только операциям
                        if (PRI_Sl.USL[0].Tip == "опер" && (PRI_Sl.USL[0].Usl.StartsWith("A16") || PRI_Sl.USL[0].Usl.StartsWith("A11") || PRI_Sl.USL[0].Usl.StartsWith("A03")))
                            PRI_Sl.Short = 0.8;
                        else
                            PRI_Sl.Short = 0.4;

                        PRI_Sl.SUMV *= PRI_Sl.Short;
                    }
                    else
                    {
                        // если случай прерван (химия)
                        if (PRI_Sl.Prervan)
                        {
                            PRI_Sl.Short = 0.8;
                            PRI_Sl.SUMV *= PRI_Sl.Short;
                        }
                    }
                }

                // Если ВМП
                if (PRI_StrahReestr.METOD_HMP != "")
                {
                    PRI_Sl.SUMV = (double)PRI_StrahReestr.TARIF;
                    PRI_Sl.USL[0].Ksg = "";

                    // Номер талона ВМП
                    PRI_Sl.TAL_NUM = m.MET_PoleStr("TAL_NUM", pApstac);
                    // Проверка
                    if (PRI_Sl.TAL_NUM.Length < 17)
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "55";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Мало символов в Номере талона ВМП";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                    }

                    // Дата создания талона
                    PRI_Sl.TAL_D = m.MET_PoleStr("TAL_D", pApstac);
                    PRI_Sl.TAL_D = PRI_Sl.TAL_D == "" ? null : PRI_Sl.TAL_D.Remove(10);
                    // Проверка
                    if (PRI_Sl.TAL_D == "")
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "56";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Ошибка Даты талона ВМП";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                    }
                    // Модель пациента V019
                    PRI_Sl.IDMODP = m.MET_PoleStr("IDMODP", pApstac);
                    // Номер группы ВМП V019
                    PRI_Sl.HGR = m.MET_PoleStr("HGR", pApstac);
                }
            }
            PRI_Sl.SUMV = Math.Round(PRI_Sl.SUMV, 2, MidpointRounding.AwayFromZero);
            PRI_StrahReestr.KSG = PRI_Sl.USL[0].Ksg;   // !!!!!!!!!!!!!
            PRI_Sl.KSG = PRI_Sl.USL[0].Ksg;
            PRI_StrahReestr.SUM_LPU = (decimal)PRI_Sl.SUMV;
        }

        /// <summary>МЕТОД 5. Расчет Стационара 2022 (Расчет КСГ)</summary>
        private void MET_CalcKsg2022(DataRow pApstac)
        {

            // ЛПУ направления
            PRI_Sl.NPR_MO = m.MET_PoleInt("NPR_MO", pApstac);

            // Дата направления
            PRI_Sl.NPR_DATE = m.MET_PoleStr("DN", pApstac); //m.MET_PoleStr("NPR_DATE", pApstac);
            //  _Sluch.NPR_DATE = _Sluch.NPR_DATE == "" ? m.MET_PoleStr("DN", pApstac) : _Sluch.NPR_DATE;
            PRI_Sl.NPR_DATE = PRI_Sl.NPR_DATE.Remove(10);

            // Переводы
            PRI_Sl.VB_P = m.MET_PoleInt("OtdIn", pApstac);

            // Профиль койки
            PRI_Sl.PROFIL_K = m.MET_PoleInt("PROFIL_K", pApstac);

            string _jTag = m.MET_PoleStr("jTag", pApstac);

            bool _Zno = PRI_StrahReestr.DS1.Substring(0, 1) == "C" || PRI_StrahReestr.DS1.Substring(0, 2) == "D0";
            if (string.IsNullOrEmpty(_jTag) && !_Zno)
                _jTag = "{ }";

            // Проверка на наличия данных KbolInfo
            if (string.IsNullOrEmpty(_jTag))
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "28";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найдена строка APSTAC в KbolInfo";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            JObject _JsonSL;
            try
            {
                _JsonSL = JObject.Parse(_jTag);
            }
            catch
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "30";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в KbolInfo";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // DS2 (Сопутствующий Диагноз - типо Сахарный диабет)
            PRI_StrahReestr.DS2 = (string)_JsonSL["Kslp_diag"] ?? "";

            //// Сопутствующий диагноз для D70
            //bool _DiagD70 = PRI_StrahReestr.DS1.Substring(0, 3) == "D70";
            //if (_DiagD70)
            //{
            //    PRI_StrahReestr.DS2 = (string)_JsonSL["DiagD70"];

            //    // Обязательно должен быть сопутствующий диагноз С
            //    if(string.IsNullOrEmpty(PRI_StrahReestr.DS2) || PRI_StrahReestr.DS2.Substring(0, 1) != "C")
            //    {
            //        PRI_ErrorToExcel.PROP_ErrorCod = "41";
            //        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Отстутствует или не правильный сопутствующий диагноз для D70 в KbolInfo";
            //        PRI_ErrorToExcel.MET_SaveError();
            //        PRI_ErrorRow = true;
            //        return;
            //    }

            //    _JsonSL["Klin_gr"] = "II";
            //    _JsonSL["DS1_T"] = 6;
            //}


            // Новое 285 приказ
            if (_Zno)
            {
                // УБРАЛ направления 28/07/2020 из за штрафов Капитала по превышению 7го срока и отсутствия точной даты направления
                // Направления на исследования, только для ЗНО
                //if (PRI_StrahReestr.EX_DATE >= new DateTime(2019, 12, 1))
                //{
                //    IList<JToken> _Results = _JsonSL["NAPR"]?.Children().ToList();
                //    if (_Results != null)
                //    {
                //        PRI_Sl.NAPR = new List<MyNAPR>();
                //        foreach (JToken _result in _Results)
                //        {
                //            MyNAPR _Napr = _result.ToObject<MyNAPR>();
                //            _Napr.NAPR_V = 3;
                //            _Napr.NAPR_DATE = PRI_StrahReestr.ARR_DATE.Value.ToString("dd.MM.yyyy");  // !!! Пока берем не из направления, а начальную дату случая
                //            PRI_Sl.NAPR.Add(_Napr);
                //        }
                //    }
                //}

                // Характер заболевания 1 - острое, 2 храническое впервые, 3 - хроническое повторно
                // Только для поликлиники или для стационара с диагнозом С..D09 (таблица Q018)
                PRI_Sl.C_ZAB = m.MET_PoleInt("C_Zab", pApstac);

                // Клиническая группа
                string _klin_gr = (string)_JsonSL["Klin_gr"];

                // Проверка на наличия Клинической группы в KbolInfo
                if (string.IsNullOrEmpty(_klin_gr))
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "29";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тег klin_gr (Клиническая группа) в KbolInfo";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }

                // Убрал Пока подозрение в стационаре
                //// DS_ONK - Если подозрение на ЗНО
                //if (_klin_gr == "Ia" || _klin_gr == "Ib")
                //    PRI_Sluch.DS_ONK = 1;
                //else
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
                    PRI_Sl.ONK_SL.DS1_T = (int?)_JsonSL["DS1_T"] ?? 0;
                    if (PRI_Sl.ONK_SL.DS1_T == 1 || PRI_Sl.ONK_SL.DS1_T == 2)
                    {
                        // MTSTZ - Отдаленные метастазы
                        PRI_Sl.ONK_SL.MTSTZ = 1;
                    }
                    if (PRI_Sl.ONK_SL.DS1_T == 21 || PRI_Sl.ONK_SL.DS1_T == 22)
                    {
                        PRI_Sl.ONK_SL.DS1_T = PRI_Sl.ONK_SL.DS1_T - 20;
                    }

                    // STAD - Стадия (только для DS1_T < 5)
                    if (PRI_Sl.ONK_SL.DS1_T < 5)
                    {
                        string _stadia = (string)_JsonSL["Stadia"];
                        var _N002 = PRI_N002.Where(e => e.Kod1 == _D5).ToList();
                        // Сначала пытаемся найти по полному диагнозу
                        if (!_N002.Any())
                            _N002 = PRI_N002.Where(e => e.Kod1 == _D3).ToList(); // Иначе ищем по 3м сиволам диагноза
                        if (!_N002.Any())
                            _N002 = PRI_N002.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                        int? _stadiaNumber = _N002.FirstOrDefault(e => e.Kod3.Contains($"\"{_stadia}\""))?.Number ??
                                             _N002.First()?.Number;
                        PRI_Sl.ONK_SL.STAD = _stadiaNumber ?? 145;
                    }

                    // TNM (заполняем только для взрослых и первичном лечении)
                    if (PRI_StrahReestr.DET == 0 && PRI_Sl.ONK_SL.DS1_T == 0)
                    {
                        // ONK_T - T
                        string _T = (string)_JsonSL["T"];
                        var _N003 = PRI_N003.Where(e => e.Kod1 == _D5).ToList();
                        // Сначала пытаемся найти по полному диагнозу
                        if (!_N003.Any())
                            _N003 = PRI_N003.Where(e => e.Kod1 == _D3).ToList(); // Иначе ищем по 3м сиволам диагноза
                        if (!_N003.Any())
                            _N003 = PRI_N003.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                        int? _TNumber = _N003.FirstOrDefault(e => e.Kod3.Contains($"\"{_T}\""))?.Number ??
                                        _N003.First()?.Number;
                        PRI_Sl.ONK_SL.ONK_T = _TNumber ?? 182;

                        // ONK_N - N
                        string _N = (string)_JsonSL["N"];
                        var _N004 = PRI_N004.Where(e => e.Kod1 == _D5).ToList();
                        // Сначала пытаемся найти по полному диагнозу
                        if (!_N004.Any())
                            _N004 = PRI_N004.Where(e => e.Kod1 == _D3).ToList(); // Иначе ищем по 3м сиволам диагноза
                        if (!_N004.Any())
                            _N004 = PRI_N004.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                        int? _NNumber = _N004.FirstOrDefault(e => e.Kod3.Contains($"\"{_N}\""))?.Number ??
                                        _N004.First()?.Number;
                        PRI_Sl.ONK_SL.ONK_N = _NNumber ?? 99;

                        // ONK_M - M
                        string _M = (string)_JsonSL["M"];
                        var _N005 = PRI_N005.Where(e => e.Kod1 == _D5).ToList();
                        // Сначала пытаемся найти по полному диагнозу
                        if (!_N005.Any())
                            _N005 = PRI_N005.Where(e => e.Kod1 == _D3).ToList(); // Иначе ищем по 3м сиволам диагноза
                        if (!_N005.Any())
                            _N005 = PRI_N005.Where(e => e.Kod1 == "").ToList(); // Если не нашли, то без диагноза
                        int? _MNumber = _N005.FirstOrDefault(e => e.Kod3.Contains($"\"{_M}\""))?.Number ??
                                        _N005.First()?.Number;
                        PRI_Sl.ONK_SL.ONK_M = _MNumber ?? 56;
                    }

                    // B_DIAG - Диагностический блок
                    // Гистология
                    string _Gisto = (string)_JsonSL["resulthistology"] ?? "";
                    if (!string.IsNullOrEmpty(_Gisto))
                    {
                        var _mGisto = _Gisto.Split(';');
                        // PRI_Sl.ONK_SL.B_DIAG = new List<MyB_DIAG>();
                        // Смотрим если такой диагноз в проверочном файле N009
                        var _N009 = PRI_N009.Where(e => e.Kod1.Contains(_D3)).ToList();
                        bool _DiagN009 = _N009.Any();
                        foreach (var _i in _mGisto)
                        {
                            if (int.TryParse(_i, out int j))
                            {
                                var _Daignostic = new MyB_DIAG();
                                _Daignostic.DIAG_DATE = MET_VerifDate((string)_JsonSL["DateDirectHistology"], "DateDirectHistology") ?? PRI_StrahReestr.ARR_DATE.Value.ToString("dd.MM.yyyy");
                                _Daignostic.DIAG_TIP = 1;
                                _Daignostic.DIAG_RSLT = j;
                                _Daignostic.DIAG_CODE = PRI_N008.FirstOrDefault(e => e.Number == j).ID1.Value;
                                if (_DiagN009)
                                {
                                    if (_N009.Any(e => e.ID1 == _Daignostic.DIAG_CODE))
                                    {
                                        if (PRI_Sl.ONK_SL.B_DIAG == null)
                                            PRI_Sl.ONK_SL.B_DIAG = new List<MyB_DIAG>();
                                        PRI_Sl.ONK_SL.B_DIAG.Add(_Daignostic);
                                    }
                                }
                                //else
                                //    PRI_Sl.ONK_SL.B_DIAG.Add(_Daignostic);
                            }
                        }
                    }
                    // ИГХ
                    _Gisto = (string)_JsonSL["markerigh"] ?? "";
                    if (!string.IsNullOrEmpty(_Gisto))
                    {
                        var _mGisto = _Gisto.Split(';');
                        // Смотрим если такой диагноз в проверочном файле N009
                        var _N012 = PRI_N012.Where(e => e.Kod1.Contains(_D3)).ToList();

                        bool _DiagN012 = _N012.Any();
                        foreach (var _i in _mGisto)
                        {
                            if (int.TryParse(_i, out int j))
                            {
                                var _Daignostic = new MyB_DIAG();
                                _Daignostic.DIAG_DATE = MET_VerifDate((string)_JsonSL["DateDirectHistology"], "DateDirectHistology") ?? PRI_StrahReestr.ARR_DATE.Value.ToString("dd.MM.yyyy");
                                _Daignostic.DIAG_TIP = 2;
                                _Daignostic.DIAG_RSLT = j;
                                _Daignostic.DIAG_CODE = PRI_N011.FirstOrDefault(e => e.Number == j).ID1.Value;
                                if (_DiagN012)
                                {
                                    if (_N012.Any(e => e.ID1 == _Daignostic.DIAG_CODE))
                                    {
                                        if (PRI_Sl.ONK_SL.B_DIAG == null)
                                            PRI_Sl.ONK_SL.B_DIAG = new List<MyB_DIAG>();
                                        PRI_Sl.ONK_SL.B_DIAG.Add(_Daignostic);
                                    }
                                }
                                //else
                                //    PRI_Sl.ONK_SL.B_DIAG.Add(_Daignostic);
                            }
                        }
                    }

                    // CONS Блок о проведении консилиума
                    // Связываем ReApstac и ReApstacCons
                    foreach (DataRow _ApstacRowCons in pApstac.GetChildRows("ReApstac_ApstacCons"))
                    {
                        switch (m.MET_PoleInt("PR_CONS", _ApstacRowCons))
                        {
                            case 1:
                                PRI_Sl.Taktika_1 = MET_VerifDate(m.MET_PoleStr("DT_CONS", _ApstacRowCons), $"консилиума", true, false);
                                if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                break;
                            case 2:
                                PRI_Sl.Taktika_2 = MET_VerifDate(m.MET_PoleStr("DT_CONS", _ApstacRowCons), $"консилиума", true, false);
                                if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                break;
                            case 3:
                                PRI_Sl.Taktika_3 = MET_VerifDate(m.MET_PoleStr("DT_CONS", _ApstacRowCons), $"консилиума", true, false);
                                if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                break;
                        }
                    }

                    // B_PROT Блок об имеющихся противопоказаниях и отказах
                    PRI_Sl.PrOt_1 = MET_VerifDate((string)_JsonSL["PrOt_1"], "PrOt_1", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_2 = MET_VerifDate((string)_JsonSL["PrOt_2"], "PrOt_2", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_3 = MET_VerifDate((string)_JsonSL["PrOt_3"], "PrOt_3", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_4 = MET_VerifDate((string)_JsonSL["PrOt_4"], "PrOt_4", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_5 = MET_VerifDate((string)_JsonSL["PrOt_5"], "PrOt_5", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                    PRI_Sl.PrOt_6 = MET_VerifDate((string)_JsonSL["PrOt_6"], "PrOt_6", true);
                    if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                }
            }

            int _CountUsl = pApstac.GetChildRows("ReApstac_Ksg").Length;

            if (_Zno && PRI_Sl.DS_ONK == 0 && PRI_Sl.ONK_SL.DS1_T < 3)
                PRI_Sl.ONK_SL.ONK_USL = new List<MyONK_USL>();

            PRI_CouUsl = 0;
            // Связываем ReApstac и ReKsg
            foreach (DataRow _KsgRow in pApstac.GetChildRows("ReApstac_Ksg"))
            {
                var _MyUSL = new MyUSL
                {
                    Tip = m.MET_PoleStr("Tip", _KsgRow),
                    Usl = m.MET_PoleStr("Usl", _KsgRow),
                    DopUsl = m.MET_PoleStr("DopUsl", _KsgRow),
                    Frakc = m.MET_PoleInt("FrakcT", _KsgRow),
                    DatN = m.MET_PoleStr("Dat", _KsgRow),
                    Ksg = m.MET_PoleStr("KSG", _KsgRow),
                    Fact = m.MET_PoleRea("Factor", _KsgRow),
                    UprFactor = m.MET_PoleRea("UprFactor", _KsgRow),
                    KUSmo = m.MET_PoleInt("KUSmo", _KsgRow), //устарел ещё с 2021, вернули в фервале 2022
                    Day3 = m.MET_PoleInt("Day3", _KsgRow),
                    Dzp = m.MET_PoleRea("Dzp", _KsgRow)
                };

                //// Удаляем все химии, если это не первая услуга!!!!!
                //if (PRI_CouUsl > 0 && ((_MyUSL.Usl.StartsWith("sh") && !PRI_Sl.USL[0].Usl.StartsWith("sh"))
                //        || (_MyUSL.Usl.StartsWith("gem") && !PRI_Sl.USL[0].Usl.StartsWith("gem"))))
                // Поставил в связи с ошибкой на дубль USL_TIP 02.04.2021 (Ни каких бумаг на этот счет нет)

                // Удаляем все НЕ первые услуги 01.03.2021
                if (PRI_CouUsl > 0)
                {
                    _CountUsl--;
                    continue;
                }
                PRI_CouUsl++;

                // Удаляем если пусто
                _MyUSL.DatN = _MyUSL.DatN == "" ? null : _MyUSL.DatN.Remove(10);
                _MyUSL.DatK = MET_VerifDate(_MyUSL.DatN, "DatN", true);
                if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                _MyUSL.DopUsl = _MyUSL.DopUsl == "" ? null : _MyUSL.DopUsl;
                if (PRI_StrahReestr.KOL_USL > 3) _MyUSL.Day3 = null;

                // Для НЕ ЗНО и Радиологии, записываем фракции в CRIT
                if (!_Zno && _MyUSL.Frakc > 0)
                {
                    string _FracCRIT;
                    // Если фракций нет в Группировщике, то сами их рисуем
                    if (_MyUSL.Frakc <= 5) _FracCRIT = "fr01-05";
                    else if (_MyUSL.Frakc <= 7) _FracCRIT = "fr06-07";
                    else if (_MyUSL.Frakc <= 10) _FracCRIT = "fr08-10";
                    else if (_MyUSL.Frakc <= 20) _FracCRIT = "fr11-20";
                    else if (_MyUSL.Frakc <= 29) _FracCRIT = "fr21-29";
                    else if (_MyUSL.Frakc <= 32) _FracCRIT = "fr30-32";
                    else _FracCRIT = "fr33-99";

                    // Добавляем фракции в CRIT
                    if (PRI_Sl.CRIT == null)
                        PRI_Sl.CRIT = new List<string>();
                    PRI_Sl.CRIT.Add(_FracCRIT);

                    string _xInfo = m.MET_PoleStr("xInfo", _KsgRow);

                    _xInfo = string.IsNullOrEmpty(_xInfo) ? "{}" : _xInfo;
                    JObject _JsonUsl;
                    try
                    {
                        _JsonUsl = JObject.Parse(_xInfo);
                    }
                    catch
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "31";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в услугах Oper";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }

                    string[] _mDates = ((string)_JsonUsl["TLuch_Date"]).Split(';');
                    DateTime _DateNU = DateTime.Parse(_mDates[0]);
                    DateTime _DateKU = DateTime.Parse(_mDates[_mDates.Length - 2]);
                    _MyUSL.DatN = _DateNU.ToString("dd.MM.yyyy");
                    _MyUSL.DatK = _DateKU.ToString("dd.MM.yyyy");
                    if (PRI_StrahReestr.ARR_DATE > _DateNU || PRI_StrahReestr.EX_DATE < _DateKU)
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "33";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) В теге радиологии, даты облучения выходят за диапазон стационара";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }
                }

                // Для 285 приказа
                if (_Zno && PRI_Sl.DS_ONK == 0 && PRI_Sl.ONK_SL.DS1_T < 3)
                {
                    Random _Random = new Random();
                    string _xInfo = m.MET_PoleStr("xInfo", _KsgRow);
                    _xInfo = string.IsNullOrEmpty(_xInfo) ? "{}" : _xInfo;
                    JObject _JsonUsl;
                    try
                    {
                        _JsonUsl = JObject.Parse(_xInfo);
                    }
                    catch
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "31";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в услугах Oper";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }

                    MyONK_USL _MyOnkUsl = new MyONK_USL();

                    // Тип услуги по умолчанию
                    _MyOnkUsl.USL_TIP = 5;

                    // Если есть только диагноз
                    // if (_CountUsl == 1 && _MyUSL.Tip == "диаг") убрал 01.06.21
                    if (PRI_CouUsl == 1 && _MyUSL.Tip == "диаг")
                    {
                        // Тип услуги
                        _MyOnkUsl.USL_TIP = 5;
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Биопсия, исследования и др.
                    if (_MyUSL.Usl.StartsWith("A11") || _MyUSL.Usl.StartsWith("B03") || _MyUSL.Usl.StartsWith("A03"))
                    {
                        // Тип услуги
                        _MyOnkUsl.USL_TIP = 5;
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Хирургического лечение
                    if (_MyUSL.Usl.StartsWith("A16"))
                    {
                        // Тип услуги
                        _MyOnkUsl.USL_TIP = 1;
                        // Тип хирургического лечения (ПО УМОЛЧАНИЮ 1)
                        int _Hir = (int?)_JsonUsl["THir"] ?? 0;
                        _MyOnkUsl.HIR_TIP = _Hir > 0 & _Hir < 6 ? _Hir : 1;
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Химия
                    if (_MyUSL.Usl.StartsWith("sh") || _MyUSL.Usl.StartsWith("A25") || _MyUSL.Usl.StartsWith("gem"))
                    {
                        _MyOnkUsl.LEK_PR = new List<MyLEK_PR>();
                        // Тип услуги
                        _MyOnkUsl.USL_TIP = 2;
                        // Тип Линия  (ПО УМОЛЧАНИЮ РАНДОМ ЗАМЕНИТЬ)
                        int _TIP_L = (int?)_JsonUsl["TLek_L"] ?? 0;
                        _MyOnkUsl.LEK_TIP_L = _TIP_L > 0 ? _TIP_L : _Random.Next(1, 4);
                        // Тип Цикл  (ПО УМОЛЧАНИЮ РАНДОМ ЗАМЕНИТЬ)
                        int _TIP_V = (int?)_JsonUsl["TLek_V"] ?? 0;
                        _MyOnkUsl.LEK_TIP_V = _TIP_V > 0 ? _TIP_V : _Random.Next(1, 2);

                        // Тошнота (РАНДОМ ЗАМЕНИТЬ)
                        int _PPTR = (int?)_JsonUsl["PPTR"] ?? 0;
                        if (_PPTR == 1 || _Random.Next(1, 8) == 1)
                            _MyOnkUsl.PPTR = 1;

                        // Полный список всех дат, при сверхкоротком случае (что бы посчетать колличество ВСЕХ дней введения)
                        var _DateList = new List<string>();

                        DateTime _DateN = DateTime.Parse(_MyUSL.DatN);
                        DateTime _DateK = DateTime.Parse(_MyUSL.DatK);

                        // Загружаем препараты
                        for (int i = 1; i < 6; i++)
                        {
                            string _RegNum = (string)_JsonUsl[$"TLek_MNN{i}"] ?? "";
                            if (_RegNum != "")
                            {
                                // Проверяем код МНН по шаблону (6 цифр)
                                Regex _Regex = new Regex(@"\d{6}");
                                if (!_Regex.IsMatch(_RegNum))
                                {
                                    PRI_ErrorToExcel.PROP_ErrorCod = "32";
                                    PRI_ErrorToExcel.PROP_ErrorName = $"(вну) В теге химии стоит неправильный код МНН {_RegNum}";
                                    PRI_ErrorToExcel.MET_SaveError();
                                    PRI_ErrorRow = true;
                                    return;
                                }

                                string _Date = (string)_JsonUsl[$"TLek_Date{i}"] ?? "";
                                if (_Date == "")
                                {
                                    PRI_ErrorToExcel.PROP_ErrorCod = "32";
                                    PRI_ErrorToExcel.PROP_ErrorName = $"(вну) В теге химии, для {i}-го препарата с кодом {_RegNum} отсутствуют даты введения";
                                    PRI_ErrorToExcel.MET_SaveError();
                                    PRI_ErrorRow = true;
                                    return;
                                }

                                var _LekPr = new MyLEK_PR();
                                _LekPr.REGNUM = _RegNum;
                                _LekPr.CODE_SH = _MyUSL.Usl.StartsWith("sh") || _MyUSL.Usl.StartsWith("gem") ? _MyUSL.Usl : "нет";
                                _LekPr.DATE_INJ = new List<string>();

                                string[] _mDates = _Date.Split(';');
                                foreach (var _d in _mDates)
                                {
                                    if (DateTime.TryParse(_d, out DateTime _DateTime))
                                    {
                                        if (_DateN > _DateTime)
                                        {
                                            _DateN = _DateTime;
                                            _MyUSL.DatN = MET_VerifDate(_DateN.ToString("dd.MM.yyyy"), $"химии, для {i}-го препарата с кодом {_RegNum} дата введения", true);
                                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                        }
                                        if (_DateK < _DateTime)
                                        {
                                            _DateK = _DateTime;
                                            _MyUSL.DatK = MET_VerifDate(_DateK.ToString("dd.MM.yyyy"), $"химии, для {i}-го препарата с кодом {_RegNum} дата введения", true);
                                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                        }
                                        _LekPr.DATE_INJ.Add(_DateTime.ToString("dd.MM.yyyy"));
                                    }
                                }
                                _MyOnkUsl.LEK_PR.Add(_LekPr);

                                //// Если сверхкороткий случай, то набираем стек дат, со всех выданных лекарств
                                //if (PRI_StrahReestr.KOL_USL < 4)
                                //{
                                //    _DateList.AddRange(_LekPr.DATE_INJ);
                                //}
                                // Набираем стек дат, со всех выданных лекарств
                                _DateList.AddRange(_LekPr.DATE_INJ);
                            }
                        }

                        // Колличество дней схемы лечения
                        int _DayHim = m.MET_PoleInt("DayHim", _KsgRow);

                        // Если схема не выполненна, то случай прерванный
                        if (_DateList.Distinct().Count() < _DayHim)
                        {
                            PRI_Sl.Prervan = true;
                            // Койко дней менее 4, то считаем случай - сверхкороткий
                            if (PRI_StrahReestr.KOL_USL < 4)
                                _MyUSL.Day3 = 0;
                        }

                        //// Если дней госпитализации менее 4 дней, то смотрим выполнение дней схемы лечения
                        //if (PRI_StrahReestr.KOL_USL < 4)
                        //{
                        //    // Колличество дней схемы лечения
                        //    int _DayHim = m.MET_PoleInt("DayHim", _KsgRow);

                        //    // Если схема не выполненна, то считаем случай - сверхкороткий
                        //    if (_DateList.Distinct().Count() < _DayHim)
                        //        _MyUSL.Day3 = 0;
                        //}

                        // Проверяем, есть ли хоть один препарат
                        if (_MyOnkUsl.LEK_PR.Count == 0)
                        {
                            PRI_ErrorToExcel.PROP_ErrorCod = "36";
                            PRI_ErrorToExcel.PROP_ErrorName = $"(вну) В услуге химии {_MyUSL.Usl} отсутствуют теги с препаратами";
                            PRI_ErrorToExcel.MET_SaveError();
                            PRI_ErrorRow = true;
                            return;
                        }

                        if (_MyUSL.Usl.StartsWith("sh") || _MyUSL.Usl.StartsWith("gem"))
                        {
                            if (PRI_Sl.CRIT == null)
                                PRI_Sl.CRIT = new List<string>();
                            PRI_Sl.CRIT.Add(_MyUSL.Usl);
                        }
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Радиология
                    if (_MyUSL.Usl.StartsWith("A06") || _MyUSL.Usl.StartsWith("A07"))
                    {
                        // Тип услуги
                        _MyOnkUsl.USL_TIP = 3;
                        // Тип Линия  (ПО УМОЛЧАНИЮ РАНДОМ ЗАМЕНИТЬ)
                        int _LUCH_TIP = (int?)_JsonUsl["TLuch"] ?? 0;
                        _MyOnkUsl.LUCH_TIP = _LUCH_TIP > 0 ? _LUCH_TIP : 1;
                        // Проверка на SOD
                        int _SOD = (int?)_JsonSL["Sod"] ?? 0;
                        if (_SOD > 0)
                            PRI_Sl.ONK_SL.SOD = _SOD;
                        else
                            PRI_Sl.ONK_SL.SOD = 20;

                        // Колличество фракций
                        _MyOnkUsl.K_FR = (int?)_JsonUsl["Frakci"] ?? -1;

                        if (_MyOnkUsl.K_FR > 0)
                        {
                            string _FracCRIT = m.MET_PoleStr("FrakcText", _KsgRow);
                            // Если фракций нет в Группировщике, то сами их рисуем
                            if (_FracCRIT == "")
                            {
                                if (_MyOnkUsl.K_FR <= 5) _FracCRIT = "fr01-05";
                                else if (_MyOnkUsl.K_FR <= 7) _FracCRIT = "fr06-07";
                                else if (_MyOnkUsl.K_FR <= 10) _FracCRIT = "fr08-10";
                                else if (_MyOnkUsl.K_FR <= 20) _FracCRIT = "fr11-20";
                                else if (_MyOnkUsl.K_FR <= 29) _FracCRIT = "fr21-29";
                                else if (_MyOnkUsl.K_FR <= 32) _FracCRIT = "fr30-32";
                                else _FracCRIT = "fr33-99";
                            }

                            // Добавляем фракции в CRIT
                            if (PRI_Sl.CRIT == null)
                                PRI_Sl.CRIT = new List<string>();
                            PRI_Sl.CRIT.Add(_FracCRIT);
                        }

                        string[] _mDates = ((string)_JsonUsl["TLuch_Date"]).Split(';');
                        DateTime _DateNU = DateTime.Parse(_mDates[0]);
                        DateTime _DateKU = DateTime.Parse(_mDates[_mDates.Length - 2]);
                        _MyUSL.DatN = _DateNU.ToString("dd.MM.yyyy");
                        _MyUSL.DatK = _DateKU.ToString("dd.MM.yyyy");
                        if (PRI_StrahReestr.ARR_DATE > _DateNU || PRI_StrahReestr.EX_DATE < _DateKU)
                        {
                            PRI_ErrorToExcel.PROP_ErrorCod = "33";
                            PRI_ErrorToExcel.PROP_ErrorName = "(вну) В теге радиологии, даты облучения выходят за диапазон стационара";
                            PRI_ErrorToExcel.MET_SaveError();
                            PRI_ErrorRow = true;
                            return;
                        }

                        if (_MyUSL.DopUsl != null && _MyUSL.DopUsl.StartsWith("mt"))
                        {
                            _MyOnkUsl.LEK_PR = new List<MyLEK_PR>();
                            _MyOnkUsl.USL_TIP = 4;
                            if (PRI_Sl.CRIT == null)
                                PRI_Sl.CRIT = new List<string>();
                            PRI_Sl.CRIT.Add(_MyUSL.DopUsl);

                            string _RegNum = (string)_JsonUsl["TLek_MNN1"] ?? "";
                            if (_RegNum != "")
                            {
                                // Проверяем код МНН по шаблону (6 цифр)
                                Regex _Regex = new Regex(@"\d{6}");
                                if (!_Regex.IsMatch(_RegNum))
                                {
                                    PRI_ErrorToExcel.PROP_ErrorCod = "33";
                                    PRI_ErrorToExcel.PROP_ErrorName =
                                        $"(вну) В теге химии у ЛУЧЕВОЙ терапии стоит неправильный код МНН {_RegNum}";
                                    PRI_ErrorToExcel.MET_SaveError();
                                    PRI_ErrorRow = true;
                                    return;
                                }

                                string _Date = (string)_JsonUsl["TLek_Date1"] ?? "";
                                if (_Date == "")
                                {
                                    PRI_ErrorToExcel.PROP_ErrorCod = "33";
                                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) В теге химии у ЛУЧЕВОЙ терапии отсутствуют даты введения";
                                    PRI_ErrorToExcel.MET_SaveError();
                                    PRI_ErrorRow = true;
                                    return;
                                }

                                var _LekPr = new MyLEK_PR();
                                _LekPr.REGNUM = _RegNum;
                                _LekPr.CODE_SH = _MyUSL.DopUsl;
                                _LekPr.DATE_INJ = new List<string>();

                                DateTime _DateN = DateTime.Parse(_MyUSL.DatN);
                                DateTime _DateK = DateTime.Parse(_MyUSL.DatK);

                                _mDates = _Date.Split(';');
                                foreach (var _d in _mDates)
                                {
                                    if (DateTime.TryParse(_d, out DateTime _DateTime))
                                    {
                                        if (_DateN > _DateTime)
                                        {
                                            _DateN = _DateTime;
                                            _MyUSL.DatN = MET_VerifDate(_DateN.ToString("dd.MM.yyyy"), $"химии у ЛУЧЕВОЙ терапии дата введения", true);
                                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                        }
                                        if (_DateK < _DateTime)
                                        {
                                            _DateK = _DateTime;
                                            _MyUSL.DatK = MET_VerifDate(_DateK.ToString("dd.MM.yyyy"), $"химии у ЛУЧЕВОЙ терапии дата введения", true);
                                            if (PRI_ErrorRow) return; // Критическая ошибка - выходим
                                        }
                                        _LekPr.DATE_INJ.Add(_DateTime.ToString("dd.MM.yyyy"));
                                    }
                                }
                                _MyOnkUsl.LEK_PR.Add(_LekPr);
                            }
                            else
                            {
                                PRI_ErrorToExcel.PROP_ErrorCod = "38";
                                PRI_ErrorToExcel.PROP_ErrorName = $"(вну) В лучевой химии отсутствуют сведенья о препарате {_RegNum}";
                                PRI_ErrorToExcel.MET_SaveError();
                                PRI_ErrorRow = true;
                                return;
                            }

                        }
                        PRI_Sl.ONK_SL.ONK_USL.Add(_MyOnkUsl);
                    }

                    // Проверяем на наличие онко услуг
                    if (PRI_Sl.ONK_SL.ONK_USL.Count == 0)
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "37";
                        PRI_ErrorToExcel.PROP_ErrorName = $"(вну) Какая то Неведомая услуга {_MyUSL.Usl}";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                        return;
                    }
                }
                PRI_Sl.USL.Add(_MyUSL);
            }

            // Проверяем есть химия
            if (PRI_Sl.ONK_SL != null && PRI_Sl.ONK_SL.ONK_USL != null && PRI_Sl.ONK_SL.ONK_USL.Any(p => p.USL_TIP == 2 || p.USL_TIP == 4))
            {
                try
                {
                    // Вес
                    PRI_Sl.ONK_SL.WEI = (double?)_JsonSL["Ves"] ?? new Random().Next(50, 100);
                    if (PRI_Sl.ONK_SL.WEI == 0)
                        PRI_Sl.ONK_SL.WEI = new Random().Next(50, 100);
                    // Рост
                    PRI_Sl.ONK_SL.HEI = (int?)_JsonSL["Rost"] ?? new Random().Next(155, 185);
                    if (PRI_Sl.ONK_SL.HEI == 0)
                        PRI_Sl.ONK_SL.HEI = new Random().Next(155, 185);
                    // Объем тела
                    PRI_Sl.ONK_SL.BSA = Math.Round(Math.Sqrt(PRI_Sl.ONK_SL.WEI * PRI_Sl.ONK_SL.HEI / 3600), 2);
                }
                catch
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "58";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Некорректное значение в тегах Рост или Вес (должно быть число)";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;

                }
            }

            // Если не нашли услуги в группировщике, то выводим ошибку
            if (PRI_Sl.USL.Count == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "57";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найдена ни одна услуга в Групировщике по диагнозу и(или) услуге";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // Исключения по КСГ (приложение 17)
            if (PRI_CouUsl > 1)
            {
                // Если выигрывает диагноз, но есть такая услуга, то берем услугу
                if (PRI_Sl.USL[1].Tip == "опер" && PRI_Sl.USL[0].Tip == "диаг" &&
                    ((PRI_Sl.USL[1].Ksg == "st02.010" && PRI_Sl.USL[0].Ksg == "st02.008") ||
                        (PRI_Sl.USL[1].Ksg == "st02.011" && PRI_Sl.USL[0].Ksg == "st02.008") ||
                        (PRI_Sl.USL[1].Ksg == "st02.010" && PRI_Sl.USL[0].Ksg == "st02.009") ||
                        (PRI_Sl.USL[1].Ksg == "st14.001" && PRI_Sl.USL[0].Ksg == "st04.002") ||
                        (PRI_Sl.USL[1].Ksg == "st14.002" && PRI_Sl.USL[0].Ksg == "st04.002") ||
                        (PRI_Sl.USL[1].Ksg == "st21.001" && PRI_Sl.USL[0].Ksg == "st21.007") ||
                        (PRI_Sl.USL[1].Ksg == "st34.002" && PRI_Sl.USL[0].Ksg == "st34.001") ||
                        (PRI_Sl.USL[1].Ksg == "st34.002" && PRI_Sl.USL[0].Ksg == "st26.001")))
                {
                    PRI_Sl.USL.Remove(PRI_Sl.USL[0]);
                    PRI_CouUsl--;
                }
            }

            // Если дневной стационар или ВМП, то пропускаем КСЛП
            if (PRI_StrahReestr.LPU_ST == 1 && PRI_StrahReestr.METOD_HMP == "")
            {
                // Смотрим КСЛП для круглосуточного стационара

                // Коэффициент дифференциации
                PRI_Sl.KOEF_D = 1.105;

                // Возраст, при поступлении
                DateTime _Dn = PRI_StrahReestr.ARR_DATE.Value;
                DateTime _DR = PRI_StrahReestr.VOZRAST.Value;
                int _Age = _Dn.Year - _DR.Year;
                if (_Dn.Month < _DR.Month || (_Dn.Month == _DR.Month && _Dn.Day < _DR.Day)) _Age--;

                //// КСЛП 13 - дети до года (приложение 17.1)
                //if (_Age < 1)
                //{
                //    PRI_Sl.Sl13 = 0.15; // до года
                //    PRI_Sl.IT_SL += PRI_Sl.Sl13;
                //}

                //// КСЛП 14 - дети от 1 до 4 лет (приложение 17.1)
                //if (_Age < 4 && _Age > 0)
                //{
                //    PRI_Sl.Sl14 = 0.1; // до года
                //    PRI_Sl.IT_SL += PRI_Sl.Sl14;
                //}

                // КСЛП 01 - дети до 4 лет НЕ ЗНО (доп соглашении 1, 2022 года)
                if (_Age < 4 && _Age >= 0 && !_Zno)
                {
                    PRI_Sl.Sl01 = 0.2;
                    PRI_Sl.IT_SL += PRI_Sl.Sl01;
                }

                // КСЛП 01 - дети до 4 лет ЗНО (доп соглашении 1, 2022 года)
                if (_Age < 4 && _Age >= 0 && _Zno)
                {
                    PRI_Sl.Sl02 = 0.6;
                    PRI_Sl.IT_SL += PRI_Sl.Sl02;
                }

                // 10 - Сверх длинный случай (после 70 дня) (Убрали в доп соглашении 1, 2022 года)
                //if (PRI_StrahReestr.KOL_USL > 70)
                //{
                //    // КСГ, при которых сверхдлительность НЕ учитывается (лучевая терапия, тарифное соглашение)
                //    bool _NkdNO = new[]
                //    {
                //        "st19.075", "st19.076", "st19.077", "st19.078", "st19.079", "st19.080", "st19.081", "st19.082", "st19.083"
                //        , "st19.084", "st19.085", "st19.086", "st19.087", "st19.088", "st19.089"
                //    }.Contains(PRI_Sl.USL[0].Ksg);

                //    if (!_NkdNO)
                //    {
                //        PRI_Sl.Sl10 = 0.5;
                //        PRI_Sl.IT_SL += PRI_Sl.Sl10;
                //    }
                //}

                //PRI_Sl.IT_SL = Math.Round(PRI_Sl.IT_SL + 1, 2);

                // Коэффицент подуровня стационара (КУСмо)
                if (PRI_StrahReestr.EX_DATE >= new DateTime(2022, 8, 1))
                {
                    if (PRI_Sl.USL[0].KUSmo == 0)
                    // Если не сказанно, что нужно ингорировать коэффициет подуровня (из dbo.StrahKsg поля KUSmo = 1)
                    {                   
                        PRI_Sl.KOEF_U = 1.04;
                        // Для ВМП отделений другой коэффициент
                        if (new[] { 55090001m, 55090002m, 55090003m, 55090004m, 55090009m, 55090010m }.Contains((decimal)PRI_StrahReestr.PODR))
                            PRI_Sl.KOEF_U = 1.1;                                        
                    }
                    else
                        PRI_Sl.KOEF_U = 1;
                }
                else
                {
                    if (PRI_Sl.USL[0].KUSmo == 0)
                    // Если не сказанно, что нужно ингорировать коэффициет подуровня (из dbo.StrahKsg поля KUSmo = 1)
                    {
                        PRI_Sl.KOEF_U = 1.17;
                        // Для ВМП отделений другой коэффициент
                        if (new[] { 55090001m, 55090002m, 55090003m, 55090004m, 55090009m, 55090010m }.Contains((decimal)PRI_StrahReestr.PODR))
                            PRI_Sl.KOEF_U = 1.32;
                    }
                    else
                        PRI_Sl.KOEF_U = 1;
                }

                // Коэфициент затратоёмкости
                PRI_Sl.KOEF_Z = PRI_Sl.USL[0].Fact;

                // Коэффициент специфики (бывший Управленчиский коэффицент)
                PRI_Sl.KOEF_UP = PRI_Sl.USL[0].UprFactor;

                // Если есть Доля заработной платы (взрослая химия)
                if (PRI_Sl.USL[0]?.Dzp > 0)
                {
                    PRI_Sl.KOEF_Dzp = (double)PRI_Sl.USL[0]?.Dzp;
                    // ССксг = БС * КЗксг * ((1 - Дзп) + Дзп * КСксг * КУСмо * КД) + БС * КД * КСЛП
                    PRI_Sl.SUMV = (double)PRI_StrahReestr.TARIF * PRI_Sl.KOEF_Z *
                                        ((1 - PRI_Sl.KOEF_Dzp) + PRI_Sl.KOEF_Dzp * PRI_Sl.KOEF_UP *
                                        PRI_Sl.KOEF_U * PRI_Sl.KOEF_D) + ((double)PRI_StrahReestr.TARIF * PRI_Sl.KOEF_D * PRI_Sl.IT_SL);
                }
                else
                {
                    // ССксг = БС * КД * ((КЗксг * КСксг * КУСмо) + КСЛП)
                    PRI_Sl.SUMV = (double)PRI_StrahReestr.TARIF * PRI_Sl.KOEF_D * ((PRI_Sl.KOEF_Z * PRI_Sl.KOEF_UP * PRI_Sl.KOEF_U) + PRI_Sl.IT_SL);
                }

                //if (PRI_Sl.IT_SL == 1)
                //    PRI_Sl.IT_SL = 0;

                // Сверх короткий случай
                if (PRI_StrahReestr.KOL_USL < 4 && PRI_Sl.USL[0]?.Day3 != 1)
                {
                    // 0.8 только операциям
                    if (PRI_Sl.USL[0].Tip == "опер" && (PRI_Sl.USL[0].Usl.StartsWith("A16") || PRI_Sl.USL[0].Usl.StartsWith("A11") || PRI_Sl.USL[0].Usl.StartsWith("A03")))
                        PRI_Sl.Short = 0.8;
                    else
                        PRI_Sl.Short = 0.4;

                    PRI_Sl.SUMV *= PRI_Sl.Short;
                }
                else
                {
                    // если случай прерван (химия)
                    if (PRI_Sl.Prervan)
                    {
                        PRI_Sl.Short = 0.8;
                        PRI_Sl.SUMV *= PRI_Sl.Short;
                    }
                }
            }
            else
            {
                // Если дневной стационар
                if (PRI_StrahReestr.LPU_ST == 2)
                {
                    // Коэфициент затратоёмкости
                    PRI_Sl.KOEF_Z = PRI_Sl.USL[0].Fact;

                    // Управленчиский коэффицент
                    PRI_Sl.KOEF_UP = PRI_Sl.USL[0].UprFactor;

                    // Коэффициент дифференциации
                    PRI_Sl.KOEF_D = 1.105;

                    // КСЛП пока 1
                    //PRI_Sl.IT_SL = 1;

                    // Коэффицент подуровня стационара (КУСмо)
                    if (PRI_StrahReestr.EX_DATE >= new DateTime(2022, 8, 1))
                    {
                        PRI_Sl.KOEF_U = 1.04;
                    }
                    else {
                        PRI_Sl.KOEF_U = 1.09;
                    }
                    // Если взрослая химия
                    if (PRI_Sl.USL[0]?.Dzp > 0)
                    {
                        PRI_Sl.KOEF_Dzp = (double)PRI_Sl.USL[0]?.Dzp;
                        // ССксг = БС * КЗксг * ((1 - Дзп) + Дзп * КСксг * КУСмо * КД) + БС * КД * КСЛП
                        PRI_Sl.SUMV = (double)PRI_StrahReestr.TARIF * PRI_Sl.KOEF_Z *
                                            ((1 - PRI_Sl.KOEF_Dzp) + PRI_Sl.KOEF_Dzp * PRI_Sl.KOEF_UP *
                                            PRI_Sl.KOEF_U * PRI_Sl.KOEF_D); // + ((double)PRI_StrahReestr.TARIF * PRI_Sl.KOEF_D * PRI_Sl.IT_SL); // КСЛП неделаем
                    }
                    else
                    {
                        // ССксг = БС * КД * (КЗксг * КСксг * КУСмо + КСЛП)
                        PRI_Sl.SUMV = (double)PRI_StrahReestr.TARIF * PRI_Sl.KOEF_D * (PRI_Sl.KOEF_Z * PRI_Sl.KOEF_UP * PRI_Sl.KOEF_U); // + PRI_Sl.IT_SL); // КСЛП неделаем
                    }

                    // Сверх короткий случай (если нет операций (не Ашки), а только диагноз)
                    if (PRI_StrahReestr.KOL_USL < 4 && PRI_Sl.USL[0].Day3 != 1)
                    {
                        // 0.8 только операциям
                        if (PRI_Sl.USL[0].Tip == "опер" && (PRI_Sl.USL[0].Usl.StartsWith("A16") || PRI_Sl.USL[0].Usl.StartsWith("A11") || PRI_Sl.USL[0].Usl.StartsWith("A03")))
                            PRI_Sl.Short = 0.8;
                        else
                            PRI_Sl.Short = 0.4;

                        PRI_Sl.SUMV *= PRI_Sl.Short;
                    }
                    else
                    {
                        // если случай прерван (химия)
                        if (PRI_Sl.Prervan)
                        {
                            PRI_Sl.Short = 0.8;
                            PRI_Sl.SUMV *= PRI_Sl.Short;
                        }
                    }
                }

                // Если ВМП
                if (PRI_StrahReestr.METOD_HMP != "")
                {
                    PRI_Sl.SUMV = (double)PRI_StrahReestr.TARIF;
                    PRI_Sl.USL[0].Ksg = "";

                    // Номер талона ВМП
                    PRI_Sl.TAL_NUM = m.MET_PoleStr("TAL_NUM", pApstac);
                    // Проверка
                    if (PRI_Sl.TAL_NUM.Length < 17)
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "55";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Мало символов в Номере талона ВМП";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                    }

                    // Дата создания талона
                    PRI_Sl.TAL_D = m.MET_PoleStr("TAL_D", pApstac);
                    PRI_Sl.TAL_D = PRI_Sl.TAL_D == "" ? null : PRI_Sl.TAL_D.Remove(10);
                    // Проверка
                    if (PRI_Sl.TAL_D == "")
                    {
                        PRI_ErrorToExcel.PROP_ErrorCod = "56";
                        PRI_ErrorToExcel.PROP_ErrorName = "(вну) Ошибка Даты талона ВМП";
                        PRI_ErrorToExcel.MET_SaveError();
                        PRI_ErrorRow = true;
                    }
                    // Модель пациента V019
                    PRI_Sl.IDMODP = m.MET_PoleStr("IDMODP", pApstac);
                    // Номер группы ВМП V019
                    PRI_Sl.HGR = m.MET_PoleStr("HGR", pApstac);
                }
            }
            PRI_Sl.SUMV = Math.Round(PRI_Sl.SUMV, 2, MidpointRounding.AwayFromZero);
            PRI_StrahReestr.KSG = PRI_Sl.USL[0].Ksg;   // !!!!!!!!!!!!!
            PRI_Sl.KSG = PRI_Sl.USL[0].Ksg;
            PRI_StrahReestr.SUM_LPU = (decimal)PRI_Sl.SUMV;
        }
    }
}
