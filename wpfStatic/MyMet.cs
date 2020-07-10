using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using NLog;

namespace wpfStatic
{
    /// <summary>КЛАСС работы с разными Методами</summary>
    public static class MyMet
    {
        #region ---- Выбор данных из DataRow ----
        /// <summary>МЕТОД Возвращаем строку</summary>
        public static string MET_PoleStr(string pPole, DataRow _Row)
        {
            return Convert.ToString(_Row[pPole]);
        }

        /// <summary>МЕТОД Возвращаем строку c Датой</summary>
        public static string MET_PoleDat(string pPole, DataRow _Row)
        {
            return MET_StrDat(_Row[pPole]).Remove(10);
        }

        /// <summary>МЕТОД Возвращаем строку c Датой</summary>
        public static DateTime? MET_PoleDate(string pPole, DataRow _Row)
        {
            return DateTime.Parse(_Row[pPole].ToString());
        }

        /// <summary>МЕТОД Возвращаем строку c Целым числом</summary>
        public static int MET_PoleInt(string pPole, DataRow _Row)
        {
            return MET_ParseInt(_Row[pPole].ToString());
        }

        /// <summary>МЕТОД Возвращаем строку c Десятичным числом</summary>
        public static decimal MET_PoleDec(string pPole, DataRow _Row)
        {
            return MET_ParseDec(_Row[pPole].ToString());
        }

        /// <summary>МЕТОД Возвращаем строку c Реальным числом</summary>
        public static double MET_PoleRea(string pPole, DataRow _Row)
        {
            return MET_ParseRea(_Row[pPole]);
        }
        #endregion

        #region ---- Методы преобразования данных ----
        /// <summary>МЕТОД Парсим строку в целое число</summary>
        /// <param name="pStr">Строка с числом</param>
        /// <returns>В случае ошибки возвращаем 0</returns>
        public static int MET_ParseInt(string pStr)
        {            
            int.TryParse(pStr, out int _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим объект в целое число</summary>
        /// <param name="pObject">Строка с числом</param>
        /// <returns>В случае ошибки возвращаем 0</returns>
        public static int MET_ParseInt(object pObject)
        {            
            int.TryParse(pObject.ToString(), out int _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим строку в десятичное число</summary>
        /// <param name="pStr">Строка с числом</param>
        /// <returns>В случае ошибки возвращаем 0</returns>
        public static decimal MET_ParseDec(string pStr)
        {            
            decimal.TryParse(pStr, out decimal _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим объект в десятичное число</summary>
        /// <param name="pObject">Строка с числом</param>
        /// <returns>В случае ошибки возвращаем 0</returns>
        public static decimal MET_ParseDec(object pObject)
        {           
            decimal.TryParse(pObject.ToString(), out decimal _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим строку в дату</summary>
        /// <param name="pStr">Строка с датой</param>
        /// <returns>В случае ошибки возвращаем Null</returns>
        public static DateTime? MET_ParseDat(string pStr)
        {            
            DateTime.TryParse(pStr, out DateTime _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим объект в дату</summary>
        /// <param name="pObject">Объект строки с датой</param>
        /// <returns>В случае ошибки возвращаем Null</returns>
        public static DateTime? MET_ParseDat(object pObject)
        {
            if (pObject.ToString() == "") return null;            
            DateTime.TryParse(pObject.ToString(), out DateTime _Result);
            return _Result;
        }

        /// <summary>МЕТОД Парсим объект даты в строку с датой</summary>
        /// <param name="pObject">Объект с датой</param>
        /// <returns>В случае ошибки возвращаем пустую строку</returns>
        public static string MET_StrDat(object pObject)
        {
            var _DateTime = MET_ParseDat(pObject);
            var _Result = _DateTime == null ? "" : _DateTime.Value.ToShortDateString() + " г.";
            return _Result;
        } 

        /// <summary>МЕТОД Парсим объект строки в реальное число</summary>
        /// <param name="pObject">Строка с числом</param>
        /// <returns>В случае ошибки возвращаем 0</returns>
        public static double MET_ParseRea(object pObject)
        {            
            double.TryParse(pObject.ToString(), out double _Result);
            return _Result;
        }
        #endregion

        #region ---- Разные ----
        /// <summary>МЕТОД Версия программы</summary>
        public static string MET_Ver()
        {
            string _Ver = Assembly.GetEntryAssembly().GetName().Version.ToString().Substring(4);
            return _Ver;                                                          
        }

        /// <summary>МЕТОД Логирование</summary>
        public static void MET_Log()
        {
            // Поля логирования
            LogManager.Configuration.Variables["UserCod"] = MyGlo.User.ToString();
            LogManager.Configuration.Variables["UserName"] = MyGlo.UserName;
            LogManager.Configuration.Variables["СonnectionStringSQL"] = MySql.MET_ConSql();
            LogManager.Configuration.Variables["Ver"] = MET_Ver();
            LogManager.Configuration.Variables["CodApstac"] = MyGlo.IND.ToString();
            LogManager.Configuration.Variables["KL"] = MyGlo.KL.ToString();
        }

        /// <summary>МЕТОД Выборка ответа из строки протокола по VarId</summary>
        /// <param name="pVarId">Номер индификатора VarId</param>
        /// <param name="pProtokol">Строка протокола</param>
        public static string MET_GetPole(int pVarId, string pProtokol)
        {
            int _N = pProtokol.IndexOf("\\" + pVarId + "#");                    // номер первого символа ответа
            if (_N >= 0)                                                        // есть ответ на вопрос
            {
                int _K = pProtokol.IndexOf("\\", _N + 2);                       // находим номер последнего символа ответа
                if (_K == -1) _K = pProtokol.Length;                            // если последний вопрос, то номер последнего символа равен длинне ответов
                _N += pVarId.ToString().Length + 2;                             // увеличиваем номер первого символа на количество служебных символов (\45#)
                _K = _K - _N;                                                   // высчитываем длинну ответа
                return pProtokol.Substring(_N, _K);
            }
            return "";                                                          // если ответа нету
        }

        /// <summary>МЕТОД Имя пользователя по коду из s_Users</summary>
        /// <param name="pCod">Код пользователя</param>
        /// <returns>В случае ошибки возвращаем пустое поле</returns>
        public static string MET_UserName(int pCod)
        {
            string _UserName = MyGlo.DataSet.Tables["s_Users"].AsEnumerable()
                .FirstOrDefault(p => p.Field<int>("Cod") == pCod)?.Field<string>("FIO");
            return _UserName;
        }

        /// <summary>МЕТОД Сколько лет</summary>
        /// <param name="pDob">Дата рождения</param>
        /// <param name="pToDay">Дата на которую расчитываем возраст</param>
        /// <returns>Возвращаем число лет + слово год. В случае ошибки возвращаем пустую строку</returns>
        public static string MET_Age(DateTime pDob, DateTime pToDay )
        {
            string _Value;
            try
            {   
                _Value = ((int)((pToDay.Subtract(pDob).Days * 0.99932) / 365)).ToString(); // 0.99932 - убераем високосные дни
                if (_Value == "11" || _Value == "12" || _Value == "13" || _Value == "14")
                    _Value += " лет";
                else
                    switch (_Value.Last())
                    {
                        case '1':
                            _Value += " год";
                            break;
                        case '2':
                        case '3':
                        case '4':
                            _Value += " года";
                            break;
                        default:
                            _Value += " лет";
                            break;
                    }
            }
            catch
            {
                _Value = "";
            }
            return _Value;
        }

        /// <summary>Наименование отделения из s_Department по коду</summary>
        /// <param name="pCod">Числовой код отделения</param>
        /// <returns>Возвращаем наименование отделения</returns>
        public static string MET_NameOtd(int pCod)
        {
            string _NameOtd = MyGlo.PUB_Context.s_Department.FirstOrDefault(e => e.Cod == pCod).Names;
            return _NameOtd;
        }

        /// <summary>Наименование отделения из s_Department текущего отделения по MyGlo.Otd</summary>        
        /// <returns>Возвращаем наименование текущего отделения</returns>
        public static string MET_NameOtd()
        {
            string _NameOtd = MyGlo.PUB_Context.s_Department.FirstOrDefault(e => e.Cod == MyGlo.Otd).Names;
            return _NameOtd;
        }

        /// <summary>МЕТОД Устанавливаем язык RU или US по умолчанию русский</summary>
        /// <param name="pTeg">Какой язык нужен RU или US</param>
        public static void MET_Lаng(string pTeg = "RU")
        {
            if(pTeg == "RU")
                // Если язык Английский, то ставим Русский
                if (InputLanguageManager.Current.CurrentInputLanguage.Equals(CultureInfo.CreateSpecificCulture("en-US")))
                    InputLanguageManager.Current.CurrentInputLanguage = CultureInfo.CreateSpecificCulture("ru-RU");

            if (pTeg == "US")
                // Если язык Русский, то ставим Английский
                if (InputLanguageManager.Current.CurrentInputLanguage.Equals(CultureInfo.CreateSpecificCulture("ru-RU")))
                    InputLanguageManager.Current.CurrentInputLanguage = CultureInfo.CreateSpecificCulture("en-US");
        }

        [DllImport("User32.dll")]
        private static extern IntPtr SetForegroundWindow(IntPtr hWnd);          // Активирует окно процесса
        [DllImport("User32.dll")]
        private static extern bool ShowWindow(IntPtr handle, int cmdShow);      // Отображает данное окно на перед, даже елси было свернуто

        /// <summary>МЕТОД Пытаемся найти подобный процесс по строке заголовка главного окна</summary>
        /// <param name="pTitle">Строка нового окна</param>
        /// <param name="pCountWindows">Сколько открыто окон wpfBazis</param>
        /// <returns>true - такое окно уже открыто - выходим, false - окно уникальное</returns>
        public static bool MET_DobleWindows(string pTitle, out int pCountWindows)
        {
            // Все процессы
            Process[] _Processes = Process.GetProcesses();
            // Выбираем только наши
            IEnumerable<Process> _ProcsBazis = _Processes.Where(p => p.ProcessName == "wpfBazis" && pTitle == p.MainWindowTitle);

            pCountWindows = _Processes.Count(p => p.ProcessName == "wpfBazis");
            // Если нашли подобный просесс, то true
            foreach (var _Process in _ProcsBazis)
            {
                MessageBox.Show("Такое окно уже открыто!");
                // Активирует окно процесса
                SetForegroundWindow(_Process.MainWindowHandle);
                // Отображает данное окно на перед, даже если было свернуто
                ShowWindow(_Process.MainWindowHandle, 1);
                return true;
            }
            return false;
        }

        /// <summary>МЕТОД Открытие  новой копии программы, для редактирования протоколов</summary>
        /// <param name="pTipProtokol">Тип протокола</param>
        /// <param name="pIND">Код посещения</param>
        /// <param name="pKL">Код пациента</param>
        /// <param name="pStartFile">Путь к wpfBazis, по умолчанию пусто и берется из MyGlo.PathExe</param>
        /// <returns>true - открыли новое окно, false - несмогли</returns>
        public static bool MET_EditWindows(eTipDocum pTipProtokol, decimal pIND, decimal pKL, string pStartFile = "")
        {            
            int[] _Modul = { 22, 4, 16, 15 };
            Process _Process = new Process();
            ProcessStartInfo _StartInfo = new ProcessStartInfo();
            _StartInfo.FileName = pStartFile == "" ? MyGlo.PathExe : pStartFile;
            // Отделение
            string _Otd = "";
            if (!(pTipProtokol == eTipDocum.Null || pIND == 0))
                _Otd = MySql.MET_QueryInt(MyQuery.varOtd_Select_1(pTipProtokol, pIND)).ToString();           
            _StartInfo.Arguments = $"{MyGlo.Server} {MyGlo.User} {_Modul[(int)pTipProtokol]} {pKL} {pIND} {_Otd}";
            _Process.StartInfo = _StartInfo;
            _Process.Start(); 
            return true;
        }

        /// <summary>МЕТОД Парсим телефон в нормальный вид</summary>
        /// <param name="pPhone">Строка нового окна</param>        
        /// <returns>Возвращаем номер телефона в нормальном виде, иначе пустую строку</returns>
        public static string MET_TryPhon(string pPhone)
        {
            string _ReturnPhone = "";

            decimal.TryParse(string.Join("", pPhone.Where(c => char.IsDigit(c))), out decimal _Value);

            _ReturnPhone = _Value.ToString();

            switch(_ReturnPhone.Length)
            {
                case (5):
                    _ReturnPhone = $"{_Value:#-##-##}";
                    break;
                case (6):
                    _ReturnPhone = $"{_Value:##-##-##}";
                    break;
                case (10):
                    _ReturnPhone = $"8 {_Value:### ###-##-##}";
                    break;
                case (11):                    
                    _ReturnPhone = $"{_Value:# ### ###-##-##}";
                    break;
                default:
                    _ReturnPhone = "";
                    break;
            }

            return _ReturnPhone;
        }
        #endregion
    }
   
    // ==============================================================================

    /// <summary>КЛАСС Тип протокола</summary>
    public class MyTipProtokol
    {
        #region ---- Свойства ----
        /// <summary>СВОЙСТВО Тип документа</summary>
        public eTipDocum PROP_TipDocum { get; private set; }

        /// <summary>СВОЙСТВО Префикс протокола (ast, apaN, par, kdl)</summary>
        public string PROP_Prefix { get; }

        /// <summary>СВОЙСТВО Таблица - список шаблонов</summary>
        public string PROP_ListShablon => PROP_Prefix + "ListShablon";

        /// <summary>СВОЙСТВО Таблица -  список вопросов шаблона</summary>
        public string PROP_Shablon => PROP_Prefix + "Shablon";

        /// <summary>СВОЙСТВО Таблица - протоколы</summary>
        public string PROP_Protokol => PROP_Prefix + "Protokol";

        /// <summary>СВОЙСТВО Таблица - список ответов на вопрос</summary>
        public string PROP_List => PROP_Prefix + "List";

        /// <summary>СВОЙСТВО Код индексатора в таблице NextRef</summary>
        public int PROP_NextRef { get; private set; }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>  
        /// <param name="pTipProtokol">Тип протокола</param>         
        public MyTipProtokol(eTipDocum pTipProtokol)
        {
            // Тип
            PROP_TipDocum = pTipProtokol;

            // Префикс
            switch (pTipProtokol)
            {
                case eTipDocum.Stac:
                    PROP_Prefix = "ast";
                    PROP_NextRef = 24;
                    break;
                case eTipDocum.Pol:
                    PROP_Prefix = "apaN";
                    PROP_NextRef = 21;
                    break;
                case eTipDocum.Paracl:
                    PROP_Prefix = "par";
                    PROP_NextRef = 29;
                    break;
                case eTipDocum.Kdl:
                    PROP_Prefix = "kdl";
                    PROP_NextRef = 41;
                    break;
                default:
                    PROP_Prefix = "";
                    PROP_NextRef = 0;
                    break;
            }
        }
    }

    // ==============================================================================

    /// <summary>КЛАСС Смены кодировки для поиска</summary>
    public class MyTransliter
    {
        /// <summary>Словарь</summary>
        private readonly Dictionary<string, string> PRI_Words = new Dictionary<string, string>();

        /// <summary>КОНСТРУКТОР</summary>
        public MyTransliter()
        {
            // Перевод Кирилица в Латиницу
            PRI_Words.Add("Ё", "`");
            PRI_Words.Add("Й", "Й");
            PRI_Words.Add("Ц", "W");
            PRI_Words.Add("У", "E");
            PRI_Words.Add("К", "R");
            PRI_Words.Add("Е", "T");
            PRI_Words.Add("Н", "Y");
            PRI_Words.Add("Г", "U");
            PRI_Words.Add("Ш", "I");
            PRI_Words.Add("Щ", "O");
            PRI_Words.Add("З", "P");
            //   PRI_Words.Add("Х", "[");
            //   PRI_Words.Add("Ъ", "]");
            PRI_Words.Add("Ф", "A");
            PRI_Words.Add("Ы", "S");
            PRI_Words.Add("В", "D");
            PRI_Words.Add("А", "F");
            PRI_Words.Add("П", "G");
            PRI_Words.Add("Р", "H");
            PRI_Words.Add("О", "J");
            PRI_Words.Add("Л", "K");
            PRI_Words.Add("Д", "L");
            PRI_Words.Add("Ж", ";");
            PRI_Words.Add("Э", "''");
            PRI_Words.Add("Я", "Z");
            PRI_Words.Add("Ч", "X");
            PRI_Words.Add("С", "C");
            PRI_Words.Add("М", "V");
            PRI_Words.Add("И", "B");
            PRI_Words.Add("Т", "N");
            PRI_Words.Add("Ь", "M");
            PRI_Words.Add("Б", ",");
            PRI_Words.Add("Ю", ".");
            // Перевод Латиницу в Кирилица 
            PRI_Words.Add("`", "Ё");
            PRI_Words.Add("Q", "Й");
            PRI_Words.Add("W", "Ц");
            PRI_Words.Add("E", "У");
            PRI_Words.Add("R", "К");
            PRI_Words.Add("T", "Е");
            PRI_Words.Add("Y", "Н");
            PRI_Words.Add("U", "Г");
            PRI_Words.Add("I", "Ш");
            PRI_Words.Add("O", "Щ");
            PRI_Words.Add("P", "З");
            PRI_Words.Add("[", "Х");
            PRI_Words.Add("]", "Ъ");
            PRI_Words.Add("A", "Ф");
            PRI_Words.Add("S", "Ы");
            PRI_Words.Add("D", "В");
            PRI_Words.Add("F", "А");
            PRI_Words.Add("G", "П");
            PRI_Words.Add("H", "Р");
            PRI_Words.Add("J", "О");
            PRI_Words.Add("K", "Л");
            PRI_Words.Add("L", "Д");
            PRI_Words.Add(";", "Ж");
            PRI_Words.Add("'", "Э");
            PRI_Words.Add("Z", "Я");
            PRI_Words.Add("X", "Ч");
            PRI_Words.Add("C", "С");
            PRI_Words.Add("V", "М");
            PRI_Words.Add("B", "И");
            PRI_Words.Add("N", "Т");
            PRI_Words.Add("M", "Ь");
            PRI_Words.Add(",", "Б");
            PRI_Words.Add(".", "Ю");
        }

        /// <summary>МЕТОД Возвращаем текст в другой кодировке</summary>
        /// <param name="pText">Текст условия</param>    
        public string MET_Replace(string pText)
        {   
            string source = "";
            foreach (char _Char in pText)
            {
                string _Value;
                if (PRI_Words.TryGetValue(_Char.ToString(), out _Value))
                    source += _Value;
                else
                    source += _Char.ToString();
            }
            return source;
        }
    }

    // ==============================================================================

    /// <summary>КЛАСС работы с Полями (столбцы) таблиц DataView</summary>
    public class MyColumn
    {
        #region ---- Свойства ----
        /// <summary>Тип поля (столбца)</summary>
        public string PROP_Type { get; private set; }
        /// <summary>Имя поля</summary>
        public string RPOP_Name { get; private set; }
        /// <summary>Имя поля с квадратными скобками [ ]</summary>
        public string RPOP_NameK { get; private set; }
        /// <summary>Виртуальная таблица DataView</summary>
        public DataView PROP_DataView { get; private set; }
        /// <summary>Есть ошибка - true, нету ошибки - false</summary>
        public bool PROP_Error { get; private set; }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pNamePole">Имя поля</param>
        /// <param name="pDataView">Виртуальная таблица</param>
        public MyColumn(string pNamePole, DataView pDataView)
        {
            PROP_DataView = pDataView;                                          // виртуальная таблица
            RPOP_Name = pNamePole;                                              // имя колонки
            RPOP_NameK = RPOP_Name;                                             // имя колонки с квадратными скобками
            if (RPOP_Name.Contains("["))
                RPOP_Name = RPOP_Name.Substring(1, RPOP_Name.Length - 2);       // тут убираем скобки
            else
                RPOP_NameK = "[" + RPOP_Name + "]";                             // а тут их ставим   
            try
            {
                // Находим тип колонки
                PROP_Type = pDataView.Table.Columns[RPOP_Name].DataType.Name;
                PROP_Error = false;
            }
            catch
            {
                // Ошибка! Не нашли тип
                PROP_Error = true;
            }
        }

        /// <summary>МЕТОД Возвращаем точное условие фильтра (только равно)</summary>
        /// <param name="pWhere">Текст условия</param>    
        public string MET_Filtr(string pWhere)
        {
            if (PROP_Error) return "";
            string _Where;                                                      // строка условия
            switch (PROP_Type)
            {
                case "Decimal":
                    decimal _Dec = Convert.ToDecimal(pWhere);
                    _Where = String.Format(CultureInfo.InvariantCulture, "{0} = {1}", RPOP_NameK, _Dec);
                    break;
                case "Byte":
                case "Int32":                                                   // для чисел
                    _Where = RPOP_NameK + " = " + pWhere;
                    break;
                default:                                                        // для текста и даты
                    _Where = RPOP_NameK + " = '" + pWhere + "'";
                    break;
            }
            return _Where;
        }

        /// <summary>МЕТОД Возвращаем примерное условие фильтра (больше равно, либо Like)</summary>
        /// <param name="pWhere">Текст условия</param>    
        public string MET_FiltrPr(string pWhere)
        {
            if (PROP_Error) return "";
            string _Where;                                                      // строка условия
            switch (PROP_Type)
            {
                case "Decimal":
                    decimal _Dec = Convert.ToDecimal(pWhere);
                    _Where = String.Format(CultureInfo.InvariantCulture, "{0} >= {1}", RPOP_NameK, _Dec);
                    break;
                case "Byte":
                case "Int32":                                                   // для чисел
                    _Where = RPOP_NameK + " >= " + pWhere;
                    break;
                case "DateTime":                                                // для дат
                    _Where = RPOP_NameK + " >= '" + pWhere + "'";
                    break;
                default:                                                        // для текста
                    _Where = RPOP_NameK + " like '" + pWhere + "%'";
                    break;
            }
            return _Where;
        }
    }

    // ==============================================================================

    /// <summary>КЛАСС Форматирование xFormat</summary>
    public class MyFormat
    {   
        /// <summary>Набор параметров</summary>
        public Hashtable PROP_Value { get; private set; }
        
        /// <summary>КОНСТРУКТОР</summary>          
        public MyFormat()
        {
            MET_Initial("");
        }

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pFormat">Строка формата</param>       
        public MyFormat(string pFormat)
        {
            MET_Initial(pFormat);
        }

        /// <summary>МЕТОД Инициализация</summary>
        /// <param name="pFormat">Строка формата</param>
        private void MET_Initial(string pFormat)
        {
            PROP_Value = new Hashtable();
            if (pFormat == "") return;
            string[] _mSplit = pFormat.Split('\\');
            foreach (string _Format in _mSplit)
            {
                int i = 0;
                string _Key = "";
                string _Value = "";
                string[] _mSplit2 = _Format.Split(' ');
                foreach (string _Format2 in _mSplit2)
                {
                    i++;
                    if (i == 1 && _Format2 == "") continue;
                    if (i == 1) _Key = _Format2;
                    if (i == 2) _Value = _Format2;
                    if (i > 2) _Value += ' ' + _Format2;
                }
                if (_Key != "")
                {
                    if (PROP_Value.Contains(_Key))
                    {
                    }
                    else
                    {
                        PROP_Value.Add(_Key, _Value);
                    }
                }
            }
        } 

        /// <summary>МЕТОД Есть ли такой параметр?</summary>
        /// <param name="pParamentr">Сам параметр</param>
        public bool MET_If(string pParamentr)
        {
            return PROP_Value.ContainsKey(pParamentr);
        }

        /// <summary>МЕТОД Добавляет параметр или если есть, меняет значение параметра</summary>
        /// <param name="pParamentr">Сам параметр</param>
        /// <param name="pValue">Значение параметра (по умолчанию, пусто)</param>
        public void MET_Add(string pParamentr, string pValue = "")
        {
            PROP_Value[pParamentr] = pValue;
        }
    }
}
