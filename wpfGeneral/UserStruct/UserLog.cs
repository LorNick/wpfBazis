using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using wpfStatic;

namespace wpfGeneral.UserStruct
{

    /// <summary>КЛАСС Логирования записей в поле xLog</summary>
    /// <remarks>Пример json лога: { "Log":[{ "Cod":1, "Tip":"Создан", "User":160, "Date":"01.01.2018 12:30", "Ver":"1.274" },
    ///                                     { "Cod":2, "Tip":"Изменён", "User":215, "Date":"02.01.2018 12:30", "Ver":"1.274" }
    ///                                     { "Cod":3, "Tip":"Удален", "User":300, "Date":"03.01.20181 11:30", "Ver":"1.274" }] }</remarks>
    public class UserLog
    {
        /// <summary>СВОЙСТВО Порядковый номер лога Cod</summary>
        public int Cod { get; set; }

        /// <summary>СВОЙСТВО Дата-время создания/изменения протокола Date</summary>
        public string Date { get; set; }

        /// <summary>СВОЙСТВО Тип лога (create, update, delete) Tip (c, u, d)</summary>
        public string Tip { get; set; }

        /// <summary>СВОЙСТВО Код пользователя User</summary>
        public int User { get; set; }

        /// <summary>СВОЙСТВО ФИО пользователя</summary>
        public string UserName { get; set; }

        /// <summary>СВОЙСТВО Версия программы</summary>
        public string Ver { get; set; }

        /// <summary>МЕТОД Создаем логи xLog (Создан)</summary>
        /// <param name="pUser">Кто создал документ (по умолчанию MyGlo.User)</param>

        /// <param name="pDate">Дата лога (по умолчанию текущее время)</param>
        /// <param name="pVer">Версия программы (по умолчанию текущее версия)</param>
        /// <remarks>Работа с json https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm </remarks>
        public static string MET_CreateLogs(int pUser = 0, string pDate = "", string pVer = "")
        {
            if (pUser == 0) pUser = MyGlo.User;
            if (pDate == "") pDate = DateTime.Now.ToString("dd.MM.yyyy H:mm");
            if (pVer == "") pVer = MyMet.MET_Ver();
            var _ListLogs = new List<UserLog>();
            UserLog _Log = new UserLog
            {
                Cod = 1,
                Date = pDate,
                Tip = "Создан",
                User = pUser,
                Ver = pVer
            };
            _ListLogs.Add(_Log);
            string _xLog = JsonConvert.SerializeObject(_ListLogs,
                            Formatting.None,
                            new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            _xLog = "{ \"Log\":" + _xLog + "}";
            return _xLog;
        }

        /// <summary>МЕТОД Изменяем логи xLog (Изменён, Удалён)</summary>
        /// <param name="pLog">Строка текущего лога</param>
        /// <param name="pDate">Дата лога</param>
        /// <param name="pTip">Тип лога Создан, Изменён, Удалён</param>
        /// <param name="pUser">Кто изменил/удалил документ</param>
        /// <param name="pVer">Версия программы (по умолчанию текущее версия)</param>
        /// <remarks>Работа с json https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm </remarks>
        public static string MET_LogAdd(string pLog, DateTime pDate, string pTip = "Изменён", int pUser = 0, string pVer = "")
        {
            if (pUser == 0) pUser = MyGlo.User;
            if (pVer == "") pVer = MyMet.MET_Ver();

            // Если Создан лог
            if (string.IsNullOrEmpty(pLog))
                return MET_CreateLogs(pUser, DateTime.Now.ToString("dd.MM.yyyy H:mm"), pVer);
            var _ListLogs = new List<UserLog>();
            UserLog _Log;
            JObject _Json = JObject.Parse(pLog);
            foreach (var i in _Json["Log"].Children())
            {
                _Log = new UserLog
                {
                    Cod = (int)i["Cod"],
                    Date = (string)i["Date"],
                    Tip = (string)i["Tip"],
                    User = (int)i["User"],
                    Ver = (string)i["Ver"]
                };
                _ListLogs.Add(_Log);
            }
            // Берем последний лог и смотрим тип, кто и когда его менял
            _Log = _ListLogs.LastOrDefault();
            if (_Log != null && _Log.Tip == pTip && _Log.User == pUser && _Log.Ver == MyMet.MET_Ver()
                && (pDate - DateTime.ParseExact(_Log.Date, "dd.MM.yyyy H:mm", CultureInfo.InvariantCulture)).Hours < 6)
            {
                // Если прошло менее 6 часов просто меняем время редактирования у последнего лога
                _Log.Date = DateTime.Now.ToString("dd.MM.yyyy H:mm");
            }
            else
            {
                // Иначе добавляем новый логи
                _Log = new UserLog
                {
                    Cod = _ListLogs.Count + 1,
                    Date = pDate.ToString("dd.MM.yyyy H:mm"),
                    Tip = pTip,
                    User = pUser,
                    Ver = MyMet.MET_Ver()
                };
                _ListLogs.Add(_Log);
            }
            string _xLog = JsonConvert.SerializeObject(_ListLogs,
                            Formatting.None,
                            new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            _xLog = "{ \"Log\":" + _xLog + "}";
            return _xLog;
        }
    }
}
