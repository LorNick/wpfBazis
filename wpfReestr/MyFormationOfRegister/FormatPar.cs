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
        /// <summary>МЕТОД 6. Расчет Параклиники/Гистологии</summary>
        private void MET_CalcPar()
        {
            // Проверка на связь двух таблиц
            try
            {
                // Связываем ReStrahReestr и RePar
                DataRow _RowParErr = PRI_RowReestr.GetChildRows("ReReestr_Par")[0];
            }
            catch
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "44";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не могу соединить исследование друг с другом (проверте код Врача в s_UsersDostup)";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // Связываем ReStrahReestr и RePar
            DataRow _RowPar = PRI_RowReestr.GetChildRows("ReReestr_Par")[0];

            // PLAT -> SMO (Код страховой компании)                               далее в  MET_CalcAll()
            PRI_StrahReestr.PLAT = m.MET_PoleStr("Scom", _RowPar);

            // LPU_1 (Подразделения МО)
            PRI_StrahReestr.LPU_1 = 55550900;                               // по умолчанию главный

            // ORDER (Направление на госпитализацию, 1 - плановая, 2 - экстренная (у нас нету), 0 - поликлиника)
            PRI_StrahReestr.ORDER = 0;

            // LPU_ST -> USL_OK (Условия оказания мед. помощи, справочник V006, 1 - кр. стационра, 2 - дн. стационар, 3 - поликлиника, для параклиники пусть будет 4 и 5, при выгрузке в XML поменяем на 3)
            PRI_StrahReestr.LPU_ST = (int)PRI_TipUsl;

            // VIDPOM (Вид помощи, справочник V008, у нас 13 - поликл/параклиники, 14 - телемедицина, 31 - стационар)
            PRI_StrahReestr.VIDPOM = m.MET_PoleInt("VIDPOM", _RowPar);            // основной 13, но для всякой экзотики там другой номер

            // PROFIL (Профиль, справочник V002)
            PRI_StrahReestr.PROFIL = m.MET_PoleDec("PROFIL", _RowPar);
            // Проверка
            PRI_ErrorToExcel.MET_VeryfError(PRI_StrahReestr.PROFIL == 0,
                        "12", "(вну) Не найден профиль",
                        ref PRI_ErrorRow);

            // PODR (Код отделения)
            string _Prof = ((int)PRI_StrahReestr.PROFIL).ToString("D3");
            if (_Prof == "034")
                PRI_StrahReestr.PODR = m.MET_ParseDec($"3{_Prof}2{_Prof}");
            else
                PRI_StrahReestr.PODR = m.MET_ParseDec($"3{_Prof}1{_Prof}");


            // DET (Детский профиль, если ребёнок то 1 иначе 0)      далее в  MET_CalcAll()
            PRI_Age = m.MET_PoleInt("Age", _RowPar);

            // PRVS (Специальность врача, справочник V004)
            PRI_StrahReestr.PRVS = m.MET_PoleStr("PRVS", _RowPar);

            // IDDOKT -> CODE_MD (Код врача, справочник врачей)
            PRI_StrahReestr.IDDOKT = m.MET_PoleStr("IDDOKT", _RowPar);

            // ARR_DATE -> DATE_IN (Дата начала)
            PRI_StrahReestr.ARR_DATE = m.MET_PoleDate("DP", PRI_RowReestr);

            // EX_DATE -> DATE_OUT (Дата окончания)
            PRI_StrahReestr.EX_DATE = PRI_StrahReestr.ARR_DATE;

            // DS1 (Диагноз)
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

            //DS2 (Сопутствующий Диагноз - не заполняем)
            PRI_StrahReestr.DS2 = "";

            // PACIENTID -> NHISTORY (Номер истории Cod из kbolInfo)
            PRI_StrahReestr.PACIENTID = m.MET_PoleStr("Cod", PRI_RowReestr);

            // RES_G -> RSLT (Результат обращения/госпитализации, справочник V009)
            PRI_StrahReestr.RES_G = 301;

            // ISHOD (Исход заболевания, справочник V012)
            PRI_StrahReestr.ISHOD = 304;

            // KOL_USL (Койко дней)
            PRI_StrahReestr.KOL_USL = 1;

            // IDSP (Код способа оплаты, справочник V010: 28 - За медицинскую услугу)
            PRI_StrahReestr.IDSP = 28;

            // TARIF (Тариф)
            PRI_StrahReestr.TARIF = m.MET_PoleDec("Tarif", _RowPar);
            // Проверка
            if (PRI_StrahReestr.TARIF == 0)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "26";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найден тариф";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
            }

            // SUM_LPU -> SUMV_USL (Сумма услуги)
            PRI_StrahReestr.SUM_LPU = PRI_StrahReestr.TARIF;

            // NUMBER -> NPOLIS (Номер полиса)  далее в  MET_CalcAll()
            PRI_NPolis = m.MET_PoleStr("SN", _RowPar);
            if (string.IsNullOrEmpty(PRI_NPolis))
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "61";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Куда номер полиса дели то?";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // SERIA -> SPOLIS (Серия полиса)
            PRI_StrahReestr.SERIA = m.MET_PoleStr("SS", _RowPar).ToUpper();

            // DayN (Длительность лечения из Тарифов)
            PRI_StrahReestr.DayN = 1;

            // Цель посещения
            PRI_Sl.P_Cel = "1.0";

            PRI_CouUsl = 1;

            // Описываем услугу
            MyUSL _Usluga = new MyUSL();
            _Usluga.DatN = PRI_StrahReestr.ARR_DATE.ToString().Remove(10);
            _Usluga.D = PRI_StrahReestr.DS1;
            _Usluga.PRVS_Usl = PRI_StrahReestr.PRVS;

            _Usluga.MD = PRI_StrahReestr.IDDOKT;
            // Проверка на код врача
            if (_Usluga.MD.Length < 16)
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "44";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) Отсутствует код врача";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }
            _Usluga.Code_Usl = m.MET_PoleStr("CODE_USL", _RowPar);
            _Usluga.Usl = m.MET_PoleStr("VID_VME", _RowPar);
            PRI_Sl.USL.Add(_Usluga);

            // ЛПУ направления
            PRI_Sl.NPR_MO = 555509;
            PRI_Sl.NPR_MO = m.MET_PoleInt("NPR_MO", _RowPar);
            // Пациент может быть направлен от нас, только с другого ЛПУ
            if (PRI_Sl.NPR_MO == 555509 && m.MET_ParseInt(PRI_StrahReestr.PLAT) < 100) // Иногородним позволяем направление с нашим ЛПУ
            {
                PRI_ErrorToExcel.PROP_ErrorCod = "46";
                PRI_ErrorToExcel.PROP_ErrorName = "(вну) ЛПУ направления NPR_MO не может быть наша (555509)";
                PRI_ErrorToExcel.MET_SaveError();
                PRI_ErrorRow = true;
                return;
            }

            // Дата направления
            PRI_Sl.NPR_DATE = _Usluga.DatN;

            // DS_ONK - Подозрение на ЗНО
            // PRI_Sl.DS_ONK = 1; -- убрал 30.08.2021 года

            // Если ЗНО
            if (PRI_StrahReestr.DS1.Substring(0, 1) == "C" || PRI_StrahReestr.DS1.Substring(0, 2) == "D0" || PRI_StrahReestr.DS1.Substring(0, 3) == "D70")
            {
                // Характер заболевания 1 - острое, 2 храническое впервые, 3 - хроническое повторно
                PRI_Sl.C_ZAB = 2;

                string _jTag = m.MET_PoleStr("jTag", _RowPar);

                // Проверка на наличия данных KbolInfo
                if (string.IsNullOrEmpty(_jTag))
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "28";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Не найдена строка parObsledov в KbolInfo";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }

                JObject _Json;
                try
                {
                    _Json = JObject.Parse(_jTag);
                }
                catch
                {
                    PRI_ErrorToExcel.PROP_ErrorCod = "30";
                    PRI_ErrorToExcel.PROP_ErrorName = "(вну) Неправильная структура тегов в KbolInfo";
                    PRI_ErrorToExcel.MET_SaveError();
                    PRI_ErrorRow = true;
                    return;
                }

                // Клиническая группа
                string _klin_gr = (string)_Json["Klin_gr"];
                // Проверка на наличия Клинической группы в KbolInfo, если нет, то подозрение
                if (string.IsNullOrEmpty(_klin_gr))
                    _klin_gr = "Ia";

                // DS_ONK - Если подозрение на ЗНО
                if (_klin_gr == "Ia" || _klin_gr == "Ib")
                    PRI_Sl.DS_ONK = 1;
                else
                    PRI_Sl.DS_ONK = 0;

                // ONK_UL - Блок лечения ЗНО
                if (PRI_Sl.DS_ONK == 0)
                {
                    PRI_Sl.ONK_SL = new MyONK_SL();

                    //DS1_T Повод обращения
                    PRI_Sl.ONK_SL.DS1_T = 5;
                }
            }
            PRI_StrahReestr.CODE_USL = _Usluga.Code_Usl;
            PRI_StrahReestr.VID_VME = _Usluga.Usl;

            MET_CalcAll();

            // Формируем Случай MySL в json
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
    }
}
