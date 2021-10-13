using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
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
                _Value = ((int)(pToDay.Subtract(pDob).Days * 0.99932 / 365)).ToString(); // 0.99932 - убераем високосные дни
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
        /// <remarks>ВНИМАНИЕ!!! В режиме Debug показывает НЕ то что вы указали тут в парамметрах, а то что указано в стартовых параметрах модуля!</remarks>
        public static bool MET_EditWindows(eTipDocum pTipProtokol, decimal pIND, decimal pKL, string pStartFile = "")
        {
            int[] _Modul = { 22, 4, 16, 15, 22 };
            Process _Process = new Process();
            ProcessStartInfo _StartInfo = new ProcessStartInfo();
            _StartInfo.FileName = pStartFile == "" ? MyGlo.PathExe : pStartFile;
            // Отделение
            string _Otd = "";
            if (!(pTipProtokol == eTipDocum.Null || pTipProtokol == eTipDocum.Kdl || pIND == 0))
                _Otd = MySql.MET_QueryInt(MyQuery.MET_varOtd_Select_1(pTipProtokol, pIND)).ToString();
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
                case 5:
                    _ReturnPhone = $"{_Value:#-##-##}";
                    break;
                case 6:
                    _ReturnPhone = $"{_Value:##-##-##}";
                    break;
                case 10:
                    _ReturnPhone = $"8 {_Value:### ###-##-##}";
                    break;
                case 11:
                    _ReturnPhone = $"{_Value:# ### ###-##-##}";
                    break;
                default:
                    _ReturnPhone = "";
                    break;
            }
            return _ReturnPhone;
        }

        /// <summary>МЕТОД Проверяем наличие нужной версии .NET Framework (по умолчанию 4.6.1), начиная с 4.5</summary>
        /// <param name="pVersion">Строка версии .Net</param>
        /// <returns>Если установелнная нужная (или выше) версия, то true</returns>
        public static bool MET_GetVersionNet45(string pVersion = "4.6.1")
        {
            const string _subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
            int _Ver = 0;
            using (var _ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(_subkey))
            {
                if (_ndpKey != null && _ndpKey.GetValue("Release") != null)
                    _Ver = (int)_ndpKey.GetValue("Release");
                else
                    return false;
            }
            int _releaseKey = 0;

            switch (pVersion)
            {
                case "4.8":
                    _releaseKey = 528040;
                    break;
                case "4.7.2":
                    _releaseKey = 461808;
                    break;
                case "4.7.1":
                    _releaseKey = 461308;
                    break;
                case "4.7":
                    _releaseKey = 460798;
                    break;
                case "4.6.2":
                    _releaseKey = 394802;
                    break;
                case "4.6.1":
                    _releaseKey = 394254;
                    break;
                case "4.6":
                    _releaseKey = 393295;
                    break;
                case "4.5.2":
                    _releaseKey = 379893;
                    break;
                case "4.5.1":
                    _releaseKey = 378675;
                    break;
                case "4.5":
                    _releaseKey = 378389;
                    break;
            }
            return _Ver >= _releaseKey;
        }
        #endregion
    }

}
