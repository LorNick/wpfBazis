using System;
using System.Linq;
using System.Reflection;
using wpfGeneral.UserStruct;
using wpfStatic;
using Neo.IronLua;

namespace wpfGeneral.UserVariable
{
    /// <summary>КЛАСС Для переменных (для шаблона или отчетов)</summary>
    public class UserVariable_Standart
    {
        /// <summary>СВОЙСТВО Tекст</summary>
        public string PROP_Text { get; set; }

        /// <summary>СВОЙСТВО Номер шаблона, если есть</summary>
        public int PROP_NomShabl { get; set; }

        /// <summary>СВОЙСТВО Префикс таблиц (apaN, ast, par)</summary>
        public string PROP_Prefix { get; set; }

        /// <summary>СВОЙСТВО Номер VarId вопроса, если есть</summary>
        public int PROP_VarId { get; set; }

        /// <summary>СВОЙСТВО Строка протокола, если есть</summary>
        public UserProtokol PROP_Protokol { get; set; }

        /// <summary>Текст переменной, вместе со скобками [FIO] (для текущей переменной)</summary>
        private string PRI_TextPr;

        /// <summary>Тело переменной, если есть [ShabRazdel|620|Диагноз] ->  620|Диагноз (для текущей переменной)</summary>
        private string PRI_TextBody;

        /// <summary>Начало переменной, символ [ (для текущей переменной)</summary>
        private int PRI_N;

        /// <summary>Конец переменной, символ ] (для текущей переменной)</summary>
        private int PRI_K;

        /// <summary>МЕТОД Подстановка ключевых полей в строку</summary>
        /// <param name="pString">Строка с текстом поля значения по умолчанию ValueStart</param>
        /// <param name="pNomShabl">Номер шаблона</param>
        /// <param name="pVarId">VarID вопроса</param>
        /// <param name="pPrefix">Приставка к таблице (ast....)</param>
        public string MET_ReplacePole(string pString, int pNomShabl, string pPrefix = "ast", int pVarId = 0)
        {
            PROP_Text = pString;
            // Есть ли вообще ключевые поля в строке
            if (pString.IndexOf('[') > -1)
            {
                PROP_NomShabl = pNomShabl;
                PROP_VarId = pVarId;
                PROP_Prefix = pPrefix;
                // Записываем переменные в список
                PRI_K = 0;                                                      // конец переменной, символ ]
                PRI_N = pString.IndexOf('[', PRI_K);                            // начало переменной, символ [
                do
                {
                    PRI_K = pString.IndexOf(']', PRI_N);
                    int _Len = PRI_K - PRI_N + 1;                               // длинна имени переменной
                    int _Kp = pString.IndexOf('|', PRI_N, _Len);                // окончание имени переменной, если есть, символ |
                    PRI_TextBody = _Kp > -1 ? pString.Substring(_Kp + 1, PRI_K - _Kp - 1) : "";
                    PRI_TextPr = pString.Substring(PRI_N, PRI_K - PRI_N + 1);
                    _Kp = _Kp > -1 ? _Kp : PRI_K;
                    _Len = _Kp - PRI_N - 1;
                    string _Name = pString.Substring(PRI_N + 1, _Len);          // имя переменной
                    PRI_N = pString.IndexOf('[', PRI_K);
                    try
                    {
                        MethodInfo _Method = typeof(UserVariable_Standart).GetMethod(_Name, BindingFlags.NonPublic | BindingFlags.Instance);
                        _Method?.Invoke(this, null);
                    }
                    catch
                    {
                        PROP_Text = "";
                    }
                } while (PRI_N > -1);
            }
            return PROP_Text;
        }

        /// <summary>МЕТОД Расчитывает код шаблона в зависимости от отделения</summary>
        /// <param name="pNomer">Номер шаблона</param>
        private int MET_NomShablon(int pNomer)
        {
            // Если дневной стационар (код больше 50) круглосуточных отделений, то используем шаблоны этих круглосуточных отделений
            int _Otdel = MyGlo.Otd > 50 ? MyGlo.Otd - 50 : MyGlo.Otd;
            return _Otdel * 100 + pNomer;
        }

        #region ---- Общие методы ----
        /// <summary>Новая строка</summary>
        private void n()
        {
            PROP_Text = PROP_Text.Replace("[n]", "\n");
        }

        /// <summary>Полное ФИО пациента</summary>
        private void FIO()
        {
            PROP_Text = PROP_Text.Replace("[FIO]", MyGlo.FIO);
        }

        /// <summary>Дата рождения</summary>
        private void DR()
        {
            PROP_Text = PROP_Text.Replace("[DR]", MyGlo.DR);
        }

        /// <summary>Пол</summary>
        private void Pol()
        {
            PROP_Text = PROP_Text.Replace("[Pol]", MyGlo.Pol);
        }

        /// <summary>Время текущее</summary>
        private void Time()
        {
            PROP_Text = PROP_Text.Replace("[Time]", DateTime.Now.ToString("t"));
        }

        /// <summary>Паспортные данные</summary>
        private void Pasport()
        {
            try
            {
                string _Pasport = "серия " + Convert.ToString(MyGlo.HashKBOL["Pasp_Ser"]) + " № " +
                                  Convert.ToString(MyGlo.HashKBOL["Pasp_Nom"])
                                  + " выдан " + Convert.ToString(MyGlo.HashKBOL["Pasp_Kogda"]).Substring(0, 10) + "г. " +
                                  Convert.ToString(MyGlo.HashKBOL["Pasp_Kem"]);
                PROP_Text = PROP_Text.Replace("[Pasport]", _Pasport);
            }
            catch
            {
                PROP_Text = PROP_Text.Replace("[Pasport]", "");
            }
        }

        /// <summary>СНИЛС</summary>
        private void SNILS()
        {
            PROP_Text = PROP_Text.Replace("[SNILS]", Convert.ToString(MyGlo.HashKBOL["SNILS"]));
        }

        /// <summary>Полис (из паспортных данных kbol)</summary>
        private void Polis()
        {
            // Наименование страховой компании
            string _Comp = MySql.MET_NameSpr((int)(MyGlo.HashKBOL["SCom"]), "s_StrahComp");
            // Серия
            string _Seria = Convert.ToString(MyGlo.HashKBOL["SS"]);
            _Seria = _Seria == "" ? "" : " серия " + _Seria;
            // Номер
            string _Nomer = Convert.ToString(MyGlo.HashKBOL["SN"]);
            _Nomer = _Nomer == "" ? "" : " №" + _Nomer;
            // Выводим
            PROP_Text = PROP_Text.Replace("[Polis]", _Comp + _Seria + _Nomer);
        }

        /// <summary>Индекс проживания</summary>
        private void Post()
        {
            PROP_Text = PROP_Text.Replace("[Post]", Convert.ToString(MyGlo.HashKBOL["Post"]));
        }

        /// <summary>Адрес</summary>
        private void Adr()
        {
            PROP_Text = PROP_Text.Replace("[Adr]", Convert.ToString(MyGlo.HashKBOL["Adres"]));
        }

        /// <summary>Телефон домашний</summary>
        private void Tel()
        {
            string _Tel = Convert.ToString(MyGlo.HashKBOL["TelDom"]);
            _Tel = _Tel == "" ? "" : ", тел.:" + _Tel;
            PROP_Text = PROP_Text.Replace("[Tel]", _Tel);
        }

        /// <summary>Место работы и профессия</summary>
        private void Work()
        {
            PROP_Text = PROP_Text.Replace("[Work]", Convert.ToString(MyGlo.HashKBOL["MRab"]) + ' ' + Convert.ToString(MyGlo.HashKBOL["Professia"]));
        }

        /// <summary>Социальный статус</summary>
        private void kboSocStat()
        {
            string _Cod = Convert.ToString(MyGlo.HashKBOL["SP"]);
            PROP_Text = PROP_Text.Replace("[kboSocStat]", MySql.MET_NameSpr(_Cod, "s_SocPol"));
        }

        /// <summary>Инвалидность</summary>
        private void kboInv()
        {
            string[] _mInval = { "I группа", "II группа", "III группа", "ребенок-инвалид", "", "", "снята" };
            int _Cod = Convert.ToInt16(MyGlo.HashKBOL["Inv"]);
            PROP_Text = PROP_Text.Replace("[kboInv]", _Cod > 0 ? _mInval[_Cod - 1] : "");
        }

        /// <summary>Врач вернее пользователь</summary>
        private void Vrach()
        {
            PROP_Text = PROP_Text.Replace("[Vrach]", MyGlo.UserName);
        }

        /// <summary>Пользователь создавший протокол</summary>
        /// <remarks>Только для 15 типа (метки), так как в начале протокола ещё нет</remarks>
        private void User()
        {
            string _User = "";
            if (PROP_Protokol != null)
                _User = MyMet.MET_UserName(PROP_Protokol.PROP_xUserUp);
            PROP_Text = PROP_Text.Replace("[User]", _User);
        }

        /// <summary>Дата протокола</summary>
        private void pDate()
        {
            string _Date = "";
            if (PROP_Protokol != null)
                _Date = PROP_Protokol.PROP_pDate.ToShortDateString() + "г.";
            PROP_Text = PROP_Text.Replace("[pDate]", _Date);
        }

        /// <summary>Номер Амбулаторной карты</summary>
        private void kboNomAK()
        {
            PROP_Text = PROP_Text.Replace("[kboNomAK]", Convert.ToString(MyGlo.HashKBOL["NomAK"]));
        }

        /// <summary>Тройная переменная - по шаблону и разделу (Razdel) [ShabRazdel|620|Диагноз]</summary>
        private void ShabRazdel()
        {
            try
            {
                int _Start = PROP_Text.IndexOf("[ShabRazdel");
                int _End = PROP_Text.IndexOf("]", _Start) - _Start + 1;
                string[] _mArr = PROP_Text.Substring(_Start + 1, _End - 2).Split('|');
                // Если номер шаблона меньше 100, то это номер для всех отделениий
                int _Num = Convert.ToInt32(_mArr[1]);
                _Num = _Num < 100 ? MET_NomShablon(_Num) : _Num;
                PROP_Text = PROP_Text.Replace(PROP_Text.Substring(_Start, _End), MET_StrRazdel(_Num, _mArr[2]));
            }
            catch
            {
            }
        }

        /// <summary>Тройная переменная - по шаблону и имени вопроса (Name) [ShabRazdel|101|Гистология]</summary>
        private void ShabName()
        {
            try
            {
                int _Start = PROP_Text.IndexOf("[ShabName");
                int _End = PROP_Text.IndexOf("]", _Start) - _Start + 1;
                string[] _mArr = PROP_Text.Substring(_Start + 1, _End - 2).Split('|');
                // Если номер шаблона меньше 100, то это номер для всех отделениий
                int _Num = Convert.ToInt32(_mArr[1]);
                _Num = _Num < 100 ? MET_NomShablon(_Num) : _Num;
                PROP_Text = PROP_Text.Replace(PROP_Text.Substring(_Start, _End), MET_StrRazdel(_Num, "", 0, _mArr[2]));
            }
            catch
            {
            }
        }

        /// <summary>Тройная переменная по шаблону и номеру вопроса (VarId) [ShabVarId|620|50]</summary>
        private void ShabVarId()
        {
            try
            {
                int _Start = PROP_Text.IndexOf("[ShabVarId");
                int _End = PROP_Text.IndexOf("]", _Start) - _Start + 1;
                string[] _mArr = PROP_Text.Substring(_Start + 1, _End - 2).Split('|');
                // Если номер шаблона меньше 100, то это номер для всех отделениий
                int _Num = Convert.ToInt32(_mArr[1]);
                _Num = _Num < 100 ? MET_NomShablon(_Num) : _Num;
                PROP_Text = PROP_Text.Replace(PROP_Text.Substring(_Start, _End), MET_StrRazdel(_Num, "", Convert.ToInt32(_mArr[2]), "", true));
            }
            catch
            {
            }
        }

        /// <summary>Переменная по поиску нахождению и выводу данных из всех протоколов указанного номера шаблона/типа протоколов</summary>
        /// <remarks>
        /// <para>Вид переменной: [Docum|Tip|Nomer|X1|X2|...|Xn]</para>
        /// <list type="table">
        /// <listheader><term>Константа</term><description>Описание</description></listheader>
        /// <item><term>Docum</term><description>константа (имя переменной)</description></item>
        /// <item><term>Tip</term><description>3 константы</description></item>
        /// <item><term>- Tip</term><description>номер типа шаблона, для посещения</description> </item>
        /// <item><term>- TipAll</term><description>номер типа шаблона, для пациента</description> </item>
        /// <item><term>- Sha</term><description>номер шаблона (если меньше 100, то умножаем на 100 и номер отдела, иначе и есть сам номер шаблона), для посещения</description> </item>
        /// <item><term>Nomer</term><description>номер со значением из Tip</description> </item>
        /// <item><term>Х1, X2, X3, ... Xn</term><description>переменные</description></item>
        /// <item><term>- pDate</term><description>дата текущего протокола</description> </item>
        /// <item><term>- 100</term><description>число, номер VarId вопроса</description> </item>
        /// <item><term>- Диагноз</term><description>раздел</description> </item>
        /// <item><term>- fE</term><description>новая строка</description> </item>
        /// <item><term>- fEy</term><description>новая строка в разделе между вопросами (по умолчанию)</description> </item>
        /// <item><term>- fEn</term><description>без новой строки в разделе между вопросами</description> </item>
        /// </list>
        /// </remarks>
        private void Docum()
        {
            try
            {
                string _Str = "";
                // В разделе, вопросы отображаем с новой строки, fEy - по умолчанию
                bool _Enter = true;
                // Находим 1й и последний символ
                int _Start = PROP_Text.IndexOf("[Docum");
                int _End = PROP_Text.IndexOf("]", _Start) - _Start + 1;
                // Разбиваем строку на слова
                string[] _mArr = PROP_Text.Substring(_Start + 1, _End - 2).Split('|');
                // Тип или номер шаблона true - тип, false - номер шаблона
                string _Type = _mArr[1];
                // Номер шаблона или номер типа
                int _Num = Convert.ToInt32(_mArr[2]);
                // Если номер шаблона меньше 100, то это номер для всех отделениий
                if (_Type == "Sha")
                    _Num = _Num < 100 ? MyGlo.Otd * 100 + _Num : _Num;
                // Загружаем протоколы
                switch (_Type)
                {
                    // По типу обследования, для текущего посещения
                    case "Tip":
                        MySql.MET_DsAdapterFill(MyQuery.MET_Protokol_Select_1(MyGlo.KL, MyGlo.IND, _Num, PROP_Prefix), "ProtokolRazdel");
                        break;
                    // По типу обследования, для всех протоколов
                    case "TipAll":
                        MySql.MET_DsAdapterFill(MyQuery.MET_Protokol_Select_3(MyGlo.KL, _Num, PROP_Prefix), "ProtokolRazdel");
                        break;
                    // По коду шаблона
                    case "Sha":
                        // Если номер шаблона меньше 100, то это номер для всех отделениий
                        _Num = _Num < 100 ? MyGlo.Otd * 100 + _Num : _Num;
                        MySql.MET_DsAdapterFill(MyQuery.MET_Protokol_Select_2(MyGlo.KL, MyGlo.IND, _Num, PROP_Prefix), "ProtokolRazdel");
                        break;
                }
                // Количество протоколов данного типа
                int _Count = MyGlo.DataSet.Tables["ProtokolRazdel"].Rows.Count;
                // Перебираем все протоколы
                for (int p = 0; p < _Count; p++)
                {
                    string _Protokol = MyGlo.DataSet.Tables["ProtokolRazdel"].Rows[p]["Protokol"].ToString();
                    int _NomProtokol = Convert.ToInt32(MyGlo.DataSet.Tables["ProtokolRazdel"].Rows[p]["NumShablon"]);
                    if (p > 0) _Str += ",\n";
                    int _NumVopr = 0;                                           // номер вопроса/раздела
                    // Перебераем все параметры
                    for (int i = 3; i < _mArr.Length; i++)
                    {
                        // Ставим в разделе между ответами знак Энтер
                        if (_mArr[i] == "fEy")
                        {
                            _Enter = true;
                            continue;
                        }
                        // НЕ ставим в разделе между ответами знак Энтер
                        if (_mArr[i] == "fEn")
                        {
                            _Enter = false;
                            continue;
                        }
                        // Вставляем дату проведения манипуляции
                        if (_mArr[i] == "pDate")
                        {
                            _Str += MyGlo.DataSet.Tables["ProtokolRazdel"].Rows[p]["pDate"].ToString().Substring(0, 10) + " г. ";
                            continue;
                        }
                        // Знак перехода на новую строку fE
                        if (_Str.Length > 1 && _mArr[i] == "fE")
                        {
                            _Str += "\n";
                            continue;
                        }
                        try
                        {
                            // Увеличиваем VarID или раздел
                            _NumVopr++;
                            // Если VarID или раздел не первый, то печатаем OutText, в первом вопросе
                            bool _OneOut = _NumVopr != 1;
                            if (int.TryParse(_mArr[i], out int _VarID))
                            {
                                // VarID
                                _Str += MET_LoadStr(_Protokol, _NomProtokol, "", _VarID, "", true, _OneOut) + " ";
                            }
                            else
                            {
                                // Раздел
                                _Str += MET_LoadStr(_Protokol, _NomProtokol, _mArr[i], 0, "", _Enter, _OneOut) + " ";
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                PROP_Text = PROP_Text.Replace(PROP_Text.Substring(_Start, _End), _Str).TrimEnd();
            }
            catch
            {
            }
        }

        /// <summary>Переменная по поиску нахождению и выводу данных из всех протоколов указанного номера шаблона/типа протоколов</summary>
        /// <remarks>
        /// VarId17xd - (xd) только для полей с ЧИСЛОВЫМИ данными,
        /// VarId17nc - (nc) только для ТЕКСТОВЫХ полей
        /// <para>Вид переменной: [fFormula||VarId17xd|+78.51*power(10,2) - 17 - |VarId35xd|]</para>
        /// </remarks>
        [Obsolete("Устарела, будет удалена")]
        private void fFormula()
        {
            try
            {
                string _Str = PRI_TextBody;
                _Str = _Str.Replace("|VarId", " cast(replace(dbo.GetPole(");
                _Str = _Str.Replace("xd|", ",'" + PROP_Protokol.PROP_Protokol + "'), ',','.') as float) ");   // replace на всякий случай для расчетов меняем все запятые на точки
                _Str = _Str.Replace("nc|", ",'" + PROP_Protokol.PROP_Protokol + "'), '','') as nvarchar) ");  // для обычного текста
                _Str = $"select cast({_Str} as nvarchar)";
                string _Value = MySql.MET_QueryStr(_Str);
                PROP_Text = PROP_Text.Replace(PRI_TextPr, _Value);
            }
            catch
            {
                PROP_Text = PROP_Text.Replace(PRI_TextPr, "");
            }
        }
        #endregion

        #region ---- Только для стационара ----
        /// <summary>Дата поступления в стационар</summary>
        private void DatePriem()
        {
            PROP_Text = PROP_Text.Replace("[DatePriem]", MyGlo.DatePriem);
        }

        /// <summary>Дата выписки из стационара</summary>
        private void DateVip()
        {
            if (Convert.ToString(MyGlo.HashAPSTAC["DK"]) == "")
                PROP_Text = PROP_Text.Replace("[DateVip]", " . .");
            else
                PROP_Text = PROP_Text.Replace("[DateVip]", Convert.ToString(MyGlo.HashAPSTAC["DK"]).Substring(0, 10) + " г.");
        }

        /// <summary>Номер операции</summary>
        private void NomerOper()
        {
            PROP_Text = PROP_Text.Replace("NomerOper", MySql.MET_GetNextRef(34).ToString());
        }

        /// <summary>Полная строка диагноза (из приемного отделения)</summary>
        private void DiagPriem()
        {
            PROP_Text = PROP_Text.Replace("[DiagPriem]", MyGlo.DiagStac + " " + MySql.MET_NameSpr(MyGlo.DiagStac, "s_Diag"));
        }

        /// <summary>Код диагноза стационара (из приемного отделения)</summary>
        private void DiagMKB()
        {
            PROP_Text = PROP_Text.Replace("[DiagMKB]", MyGlo.DiagStac);
        }

        /// <summary>Полная строка диагноза (из осмотра при поступлении)</summary>
        private void DiagPrOsmotr()
        {
            PROP_Text = PROP_Text.Replace("[DiagPrOsmotr]", MET_StrRazdel(MET_NomShablon(1), "Диагноз"));
        }

        /// <summary>Наименование текущего Отделения</summary>
        private void NameOtd()
        {
            PROP_Text = PROP_Text.Replace("[NameOtd]", MyMet.MET_NameOtd());
        }

        /// <summary>Наименование текущего Отделения с номером</summary>
        private void astOtd()
        {
            PROP_Text = PROP_Text.Replace("[astOtd]", $"{MyGlo.Otd}. {MyMet.MET_NameOtd()}");
        }

        /// <summary>Номер истории болезни</summary>
        private void NStac()
        {
            PROP_Text = PROP_Text.Replace("[NStac]", MyGlo.NSTAC.ToString());
        }

        /// <summary>Адрес родственников и номер телефона</summary>
        private void astRelative()
        {
            PROP_Text = PROP_Text.Replace("[astRelative]", Convert.ToString(MyGlo.HashAPSTAC["Relative"]));
        }

        /// <summary>Вес пациента</summary>
        private void astWeight()
        {
            PROP_Text = PROP_Text.Replace("[astWeight]", Convert.ToString(MyGlo.HashAPSTAC["Weight"]));
        }

        /// <summary>Рост пациента</summary>
        private void astHeight()
        {
            PROP_Text = PROP_Text.Replace("[astHeight]", Convert.ToString(MyGlo.HashAPSTAC["Height"]));
        }

        /// <summary>Площадь пациента</summary>
        private void astSquare()
        {
            PROP_Text = PROP_Text.Replace("[astSquare]", Convert.ToString(MyGlo.HashAPSTAC["Square"]));
        }

        /// <summary>Вид оплаты (ОМС, платное)</summary>
        private void astOMS()
        {
            string[] _mOMS = { "ОМС", "ДМС", "Платное", "Бюджетное" };
            int _Cod = Convert.ToInt16(MyGlo.HashAPSTAC["OMS"]);
            PROP_Text = PROP_Text.Replace("[astOMS]", _mOMS[_Cod - 1]);
        }

        /// <summary>Сколько лет (относительно посещения стационара)</summary>
        private void astAge()
        {
            try
            {
                DateTime _DN = Convert.ToDateTime(MyGlo.HashAPSTAC["DN"]);
                DateTime _DR = Convert.ToDateTime(MyGlo.DR);
                int _Age = (int)(_DN.Subtract(_DR).Days * 0.99932 / 365); // 0.99932 - убераем високосные дни
                PROP_Text = PROP_Text.Replace("[astAge]", _Age.ToString());
            }
            catch
            {
                PROP_Text = PROP_Text.Replace("[astAge]", "");
            }
        }

        /// <summary>Полис (из стационара APSTAC)</summary>
        private void astPolis()
        {
            // Наименование страховой компании
            string _Comp = MySql.MET_NameSpr((int)(MyGlo.HashAPSTAC["ScomEnd"]), "s_StrahComp");
            // Серия
            string _Seria = Convert.ToString(MyGlo.HashAPSTAC["SSEnd"]);
            _Seria = _Seria == "" ? "" : " серия " + _Seria;
            // Номер
            string _Nomer = Convert.ToString(MyGlo.HashAPSTAC["SNEnd"]);
            _Nomer = _Nomer == "" ? "" : " № " + _Nomer;
            // Выводим
            PROP_Text = PROP_Text.Replace("[astPolis]", _Comp + _Seria + _Nomer);
        }

        /// <summary>Шапка Шаблона выписки из стационара</summary>
        private void astShapkaExtact()
        {
            string _Str = "Бюджетное учреждение здравоохранения Омской области \"Клинический онкологический диспансер\"\n\n";
            // Адрес медицинской организации
            if (MyGlo.Server == 3)
                _Str += "г. Омск, ул. Завертяева 9/1\n";
            else
                _Str += "г. Омск, ул. Учебная 205\n";
            // Находим код отделения
            int _CodOtd = MySql.MET_QueryInt(MyQuery.APSTAC_Select_5(PROP_Protokol.PROP_CodApstac));
            // Наименование Отделения
            _Str += MySql.MET_QueryStr(MyQuery.MET_s_ListDocum_Select_2("2", _CodOtd)) + "\n";
            // Линия
            _Str += "_________________________________________________________________________________________";
            PROP_Text = PROP_Text.Replace("[astShapkaExtact]", _Str);
        }

        /// <summary>Находим значение с прошлого документа, этого же типа</summary>
        private void LastDocum()
        {
            PROP_Text = PROP_Text.Replace("[LastDocum]", MET_StrRazdel(PROP_NomShabl, "", PROP_VarId, "", true));
        }

        /// <summary>Оплата по тарифу Стационара</summary>
        private void astSumTarif()
        {
            string _SumTarif;
            try
            {
                _SumTarif = MySql.MET_QueryStr(MyQuery.MET_varSumTarifStac_Select_1(MyGlo.IND));
            }
            catch
            {
                _SumTarif = "";
            }
            if (_SumTarif == "0") _SumTarif = "";
            PROP_Text = PROP_Text.Replace("[astSumTarif]", _SumTarif);
        }

        /// <summary>Вставляем текст в зависимости от кода отделения  Otd</summary>
        /// <remarks>
        /// <para>Вид переменной: [astIfOtd|Условие|Отд1,Отд2, ... |Текст]</para>
        /// <list type="table">
        /// <listheader><term>Константа</term><description>Описание</description></listheader>
        /// <item><term>astIfOtd</term><description>константа (имя переменной)</description></item>
        /// <item><term>Условие</term><description>3 спец символа</description></item>
        /// <item><term>- =</term><description>отделение  равно указанным отделениям</description> </item>
        /// <item><term>- !</term><description>отделение  не равно указанным отделениям</description> </item>
        /// <item><term>- ></term><description>отделеие  больше указанной госпитализации</description> </item>
        /// <item><term>Отд1,Отд2, ... </term><description>перечень отделений</description> </item>
        /// <item><term>Текст</term><description>отображаемый текст, в случае правильного отделения</description></item>
        /// </list>
        /// </remarks>
        private void astIfOtd()
        {
            try
            {
                int _Start = PROP_Text.IndexOf("[astIfOtd");
                int _End = PROP_Text.IndexOf("]", _Start) - _Start + 1;
                // 0 - условие, 1 -  перечень отделений, через запятую, 2 - текст который вставляем
                string[] _mArr = PROP_Text.Substring(_Start + 1, _End - 2).Split('|');
                // Подстановочный текст
                string _Text = _mArr[3];
                try
                {
                    // Отделение госпитализации
                    int _Otd = (int)MyGlo.HashAPSTAC["otd"];
                    if (_Otd == 0)
                    {
                        _Text = "";
                        goto exit;
                    }
                    // Перечень отделений
                    int[] _mOtd = _mArr[2].Split(',').Select(int.Parse).ToArray();
                    // Условия
                    switch (_mArr[1])
                    {
                        case "=":
                            if (Array.IndexOf(_mOtd, _Otd) == -1)
                                _Text = "";
                            break;
                        case "!":
                            if (Array.IndexOf(_mOtd, _Otd) != -1)
                                _Text = "";
                            break;
                        case ">":
                            if (_Otd <= _mOtd[0])
                                _Text = "";
                            break;
                        default:
                            _Text = "";
                            break;
                    }
                exit:
                    ;
                }
                catch
                {
                    _Text = "";
                }
                PROP_Text = PROP_Text.Replace(PROP_Text.Substring(_Start, _End), _Text);
            }
            catch
            {
            }
        }
        #endregion

        #region ---- Только для поликлиники ----
        /// <summary>Возраст относительно посещения поликлиники</summary>
        private void apaAge()
        {
            try
            {
                PROP_Text = PROP_Text.Replace("[apaAge]", Convert.ToString(MyGlo.HashAPAC["Age"]));
                if (PROP_Text == "11" || PROP_Text == "12" || PROP_Text == "13" || PROP_Text == "14")

                    PROP_Text += " лет";
                else
                    switch (PROP_Text.Last())
                    {
                        case '1':
                            PROP_Text += " год";
                            break;
                        case '2':
                        case '3':
                        case '4':
                            PROP_Text += " года";
                            break;
                        default:
                            PROP_Text += " лет";
                            break;
                    }
            }
            catch
            {
                PROP_Text = PROP_Text.Replace("[apaAge]", "");
            }
        }

        /// <summary>Код диагноза посещения поликлиники</summary>
        private void apaMkb10()
        {
            PROP_Text = PROP_Text.Replace("[apaMkb10]", Convert.ToString(MyGlo.HashAPAC["D"]));
        }

        /// <summary>Полный диагноз посещения поликлиники</summary>
        private void apaDiag()
        {
            string _Diag;
            try
            {
                _Diag = Convert.ToInt16(MyGlo.HashAPAC["EndMKB"]) == 1
                    ? "(окончательный) "
                    : "(предварительный) ";
                string _D = (string)MyGlo.HashAPAC["D"];
                _Diag += _D + " " + MySql.MET_NameSpr(_D, "s_Diag");
            }
            catch
            {
                _Diag = "";
            }
            PROP_Text = PROP_Text.Replace("[apaDiag]", _Diag);
        }

        /// <summary>Оплата по тарифу Поликлиники</summary>
        private void apaSumTarif()
        {
            string _SumTarif;
            try
            {
                _SumTarif = MySql.MET_QueryStr(MyQuery.MET_varSumTarifPol_Select_1(MyGlo.IND));
            }
            catch
            {
                _SumTarif = "";
            }
            if (_SumTarif == "0") _SumTarif = "";
            PROP_Text = PROP_Text.Replace("[apaSumTarif]", _SumTarif);
        }
        #endregion

        #region ---- Только для параклиники ----
        /// <summary>Кто направил на обследование</summary>
        private void ParSend()
        {
            PROP_Text = PROP_Text.Replace("[ParSend]", MySql.MET_NameSpr((int)MyGlo.IND, "parObsledov", "SendName", "Cod"));
        }

        /// <summary>Аппарат</summary>
        private void Apparat()
        {
            PROP_Text = PROP_Text.Replace("[Apparat]", MySql.MET_QueryStr(MyQuery.parObsledovt_Select_2((int)MyGlo.IND)));
        }
        #endregion

        /// <summary>МЕТОД Формируем строку по данным указанного шаблона, указанного раздела у соответствуюещго шаблона</summary>
        /// <param name="pNomProtokol">Номер протокола</param>
        /// <param name="pRazdel">Наименование раздела (Razdel)</param>
        /// <param name="pVarID">Код вопроса (VarID)</param>
        /// <param name="pName">Имя вопроса (Name)</param>
        /// <param name="pLastDocum">true - в любой госпитализации, false - в этой же госпитализации</param>
        private string MET_StrRazdel(int pNomProtokol, string pRazdel, int pVarID = 0, string pName = "", bool pLastDocum = false)
        {
            string _Str = "";
            if (PROP_Prefix == null)
                PROP_Prefix = PROP_Protokol.PROP_TipProtokol.PROP_Prefix;
            // Загружаем необходимый протокол
            if (!pLastDocum)
                MySql.MET_DsAdapterFill(MyQuery.MET_Protokol_Select_6(MyGlo.IND, pNomProtokol, PROP_Prefix), "ProtokolRazdel"); // в этой же госпитализации
            else
                MySql.MET_DsAdapterFill(MyQuery.MET_Protokol_Select_8(MyGlo.KL, pNomProtokol, PROP_Prefix), "ProtokolRazdel");  // в любой из госпитализаций
            // Количество протоколов данного типа
            int _Count = MyGlo.DataSet.Tables["ProtokolRazdel"].Rows.Count;
            // Смотрим, есть ли такой протокол
            if (_Count > 0)
            {
                string _Protokol = MyGlo.DataSet.Tables["ProtokolRazdel"].Rows[_Count - 1]["Protokol"].ToString();
                //  Находим данные
                _Str = MET_LoadStr(_Protokol, pNomProtokol, pRazdel, pVarID, pName);
            }
            return _Str;
        }

        /// <summary>МЕТОД Загрузка данных из протокола для переменной</summary>
        /// <param name="pText">Текст</param>
        /// <param name="pNomProtokol">Номер протокола</param>
        /// <param name="pRazdel">Наименование раздела (Razdel)</param>
        /// <param name="pVarID">Код вопроса (VarID)</param>
        /// <param name="pName">Имя вопроса (Name)</param>
        /// <param name="pEnter">Ставим ли между вопросами раздела знак энтер</param>
        /// <param name="pOneOut">Печатать первый заголовок в разделе OutText</param>
        private string MET_LoadStr(string pText, int pNomProtokol, string pRazdel, int pVarID = 0, string pName = "", bool pEnter = true, bool pOneOut = false)
        {
            string _Str = "";
            if (PROP_Prefix == null)
                PROP_Prefix = PROP_Protokol.PROP_TipProtokol.PROP_Prefix;
            // Строка условия
            string _Where;
            if (pRazdel != "")
                _Where = string.Format("Razdel = '{0}'", pRazdel);
            else if (pVarID != 0)
                _Where = string.Format("VarId = {0}", pVarID);
            else if (pName != "")
                _Where = string.Format("Name = '{0}'", pName);
            else
                return "";
            // Загружаем шаблон
            MySql.MET_DsAdapterFill( MyQuery.MET_Shablon_Select_2(pNomProtokol, _Where, PROP_Prefix + "Shablon"), "SablonRazdel");
            for (int i = 0; i < MyGlo.DataSet.Tables["SablonRazdel"].Rows.Count; i++)
            {
                int _VarId = Convert.ToInt16(MyGlo.DataSet.Tables["SablonRazdel"].Rows[i]["VarId"]);
                int _N = pText.IndexOf("\\" + _VarId + "#");                    // номер первого символа ответа
                if (_N >= 0)                                                    // печатаем, если раздел или есть ответ на вопрос
                {
                    int _K = pText.IndexOf("\\", _N + 2);                       // находим номер последнего символа ответа
                    if (_K == -1) _K = pText.Length;                            // если последний вопрос, то номер последнего символа равен длинне ответов
                    _N += _VarId.ToString().Length + 2;                         // увеличиваем номер первого символа на количество служебных символов \45#
                    _K = _K - _N;                                               // высчитываем длинну ответа
                    // Если есть ответ и pEnter, ставим переход на новую строку
                    if (_Str.Length > 0 && pEnter)
                    {
                        if (MyGlo.DataSet.Tables["SablonRazdel"].Rows[i]["xFormat"].ToString().IndexOf("\\n") > -1)
                        {
                            _Str = _Str + "\n";
                        }
                    }
                    // Если есть подстановка вопроса, то печатаем её (пропускаем первый заголовок, если pOneOut = false)
                    if ((i != 0 || pOneOut) && MyGlo.DataSet.Tables["SablonRazdel"].Rows[i]["OutText"].ToString() != "")
                        _Str = _Str + " " + MyGlo.DataSet.Tables["SablonRazdel"].Rows[i]["OutText"];
                    // Не печатаем ответ разделов
                    if (MyGlo.DataSet.Tables["SablonRazdel"].Rows[i]["Type"].ToString() != "9")
                        if (pVarID == 0)
                            _Str = _Str + " " + pText.Substring(_N, _K) + MyGlo.DataSet.Tables["SablonRazdel"].Rows[i]["InText"];
                        else
                            _Str = pText.Substring(_N, _K); // для еденичного VarId печатаем только ответ, без подстановок OutTex и InText
                }
            }
            return _Str;
        }
    }
}
