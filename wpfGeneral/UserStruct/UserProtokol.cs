using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using wpfGeneral.UserModul;
using wpfStatic;

namespace wpfGeneral.UserStruct
{
    /// <summary>КЛАСС Протоколы</summary>
    public class UserProtokol
    {
        /// <summary>СВОЙСТВО Код протокола</summary>
        public int PROP_Cod { get; set; }

        /// <summary>СВОЙСТВО Код карты, к которой привязан протокол</summary>
        public decimal PROP_CodApstac { get; set; }

        /// <summary>СВОЙСТВО Номер шаблона</summary>
        public int PROP_NumShablon { get; set; }

        /// <summary>СВОЙСТВО Данные протокола</summary>
        public string PROP_Protokol { get; set; }

        /// <summary>СВОЙСТВО Код пациента</summary>
        public decimal PROP_KL { get; set; }

        /// <summary>СВОЙСТВО Дата создания/изменения протокола</summary>
        public DateTime PROP_xDateUp { get; set; }

        /// <summary>СВОЙСТВО Польователь создавший/изменивший протокол</summary>
        public int PROP_xUserUp { get; set; }

        /// <summary>СВОЙСТВО Удаленный протокол (1 - удален, 0 - не удален)</summary>
        public int PROP_xDelete { get; set; }

        /// <summary>СВОЙСТВО Индекс протокола (для одинаковых шаблонов)</summary>
        public int PROP_pIndex { get; set; }

        /// <summary>СВОЙСТВО Дата события (осмотр/выписка/операция и др.)</summary>
        public DateTime PROP_pDate { get; set; }

        /// <summary>СВОЙСТВО Логи (в json)</summary>
        public string PROP_xLog { get; set; }

        /// <summary>СВОЙСТВО Теги (в json)</summary>
        public string PROP_xInfo { get; set; }


        /// <summary>СВОЙСТВО Тип протокола</summary>
        public MyTipProtokol PROP_TipProtokol { get; set; }

        /// <summary>СВОЙСТВО Имя пользователя создавшего/изменившего протокол</summary>
        public string PROP_UserName { get; set; }

        /// <summary>Тип делегата Удаление протокола</summary>
        public delegate void callbackEvent();

        /// <summary>Переменная делегата Удаление протокола</summary>
        public callbackEvent OnDelete;

        /// <summary>Переменная делегата Отмена Удаления протокола</summary>
        public callbackEvent OnRestore;

        /// <summary>КОНСТРУКТОР</summary>       
        public UserProtokol()
        {
            OnDelete = new callbackEvent(MET_Delete);
            OnRestore = new callbackEvent(MET_Restore);
        }

        ///<summary>МЕТОД Конвертация данных Protokol из DataReader</summary>
        /// <param name="pDataReader">Поток данных из SQL</param> 
        private void MET_LoadDataReader(IDataRecord pDataReader)
        {
            try
            {
                PROP_Cod = pDataReader.GetInt32(pDataReader.GetOrdinal("Cod"));
                PROP_CodApstac = pDataReader.GetDecimal(pDataReader.GetOrdinal("CodApstac"));
                PROP_NumShablon = pDataReader.GetInt32(pDataReader.GetOrdinal("NumShablon"));
                PROP_Protokol = pDataReader.GetString(pDataReader.GetOrdinal("Protokol"));
                PROP_KL = pDataReader.GetDecimal(pDataReader.GetOrdinal("KL"));
                PROP_xDateUp = pDataReader.GetDateTime(pDataReader.GetOrdinal("xDateUp"));
                PROP_xUserUp = pDataReader.GetInt32(pDataReader.GetOrdinal("xUserUp"));
                PROP_xDelete = pDataReader.GetByte(pDataReader.GetOrdinal("xDelete"));
                PROP_pIndex = pDataReader.GetInt32(pDataReader.GetOrdinal("pIndex"));
                PROP_pDate = pDataReader.GetDateTime(pDataReader.GetOrdinal("pDate"));
                PROP_xLog = pDataReader[pDataReader.GetOrdinal("xLog")] as string;
                PROP_xInfo = pDataReader[pDataReader.GetOrdinal("xInfo")] as string;
                PROP_UserName = pDataReader[pDataReader.GetOrdinal("UserName")] as string;
            }
            catch (Exception ex)
            {
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Конвертация данных Protokol из DataReader");
                MyGlo.callbackEvent_sError(ex);
            }
        }

        /// <summary>МЕТОД Сохраняем протокол</summary>
        /// <param name="pProtokol">Текст протокола</param> 
        /// <param name="pxDateUp">Дата изменения протокола</param> 
        /// <param name="pxUserUp">Кто изменил протокол</param> 
        /// <param name="pDate">Дата события протокола</param> 
        /// <param name="pxDelete">Признак удаления протокола</param> 
        public void MET_Save(string pProtokol, DateTime pxDateUp, int pxUserUp, DateTime pDate, int pxDelete = 0)
        {                           
            PROP_Protokol = pProtokol;  
            PROP_xDateUp = pxDateUp;
            PROP_xUserUp = pxUserUp;    
            PROP_pDate = pDate;
            PROP_xDelete = pxDelete;
        }

        /// <summary>МЕТОД Удаляем протокол</summary>        
        public void MET_Delete()
        {
            if (PROP_xDelete == 0)
            {
                PROP_xDelete = 1;                            

                // Обновим логи
                MET_ChangeLogs(MyGlo.User, "Удалён");

                string _StrSql = MyQuery.MET_Protokol_Update_2(PROP_Cod, PROP_xDelete, PROP_xDateUp, PROP_xUserUp,
                        PROP_TipProtokol.PROP_Prefix, PROP_xLog);

                // Обновим протокол в SQL
                MySql.MET_QueryNo(_StrSql);              
            }
        }

        /// <summary>МЕТОД Отмена Удаления протокола</summary>        
        public void MET_Restore()
        {
            if (PROP_xDelete == 1)
            {
                PROP_xDelete = 0;
                PROP_xDateUp = DateTime.Today;
                PROP_xUserUp = MyGlo.User;
                // Обновим логи
                MET_ChangeLogs(MyGlo.User, "Изменён");

                string _StrSql = MyQuery.MET_Protokol_Update_2(PROP_Cod, PROP_xDelete, PROP_xDateUp, PROP_xUserUp,
                        PROP_TipProtokol.PROP_Prefix, PROP_xLog);

                // Обновим протокол в SQL
                MySql.MET_QueryNo(_StrSql);
            }
        }

        /// <summary>МЕТОД Изменяем логи xLog</summary>
        /// <param name="pUser">Кто создал/изменил протокол</param> 
        /// <param name="pTipLog">Тип лога Создан, Изменён, Удалён</param> 
        public void MET_ChangeLogs1(int pUser, string pTipLog)
        {
            string _jLog = PROP_xLog;
            var _ListLogs = new List<UserLog>();
            UserLog _Log;
            // Если нет логов и это НЕ создание протокола
            if (string.IsNullOrEmpty(_jLog) && pTipLog != "Создан")
            {
                // Если изменили протокол (вытаскиваем информацию из xDateUp, pDate)
                _Log = new UserLog
                {
                    Cod = 1,
                    Date = PROP_pDate < PROP_xDateUp ? PROP_pDate.ToString("dd.MM.yyyy H:mm") : PROP_xDateUp.ToString("dd.MM.yyyy H:mm"),
                    Tip = "Создан",
                    User = PROP_xUserUp,
                    Ver = ""
                };
                _ListLogs.Add(_Log);

                // Если ВОЗМОЖНО менялся протокол
                if (PROP_xDateUp > PROP_pDate)
                {
                    _Log = new UserLog
                    {
                        Cod = 2,
                        Date = PROP_xDateUp.ToString("dd.MM.yyyy H:mm"),
                        Tip = "Изменён",
                        User = PROP_xUserUp,
                        Ver = ""
                    };
                    _ListLogs.Add(_Log);
                }
            }
            else
            {
                // Заполняем старые логи
                JObject _Json = JObject.Parse(_jLog);
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
            }

            // Берем последний лог и смотрим тип, кто и когда его менял
            _Log = _ListLogs.LastOrDefault();
            if (_Log != null && _Log.Tip == pTipLog && _Log.User == pUser && _Log.Ver == MyMet.MET_Ver()
                && (DateTime.Now - DateTime.ParseExact(_Log.Date, "dd.MM.yyyy H:mm", CultureInfo.InvariantCulture)).Hours < 6)
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
                    Date = DateTime.Now.ToString("dd.MM.yyyy H:mm"),
                    Tip = pTipLog,
                    User = pUser,
                    Ver = MyMet.MET_Ver()
                };
                _ListLogs.Add(_Log);
            }

            PROP_xLog = JsonConvert.SerializeObject(_ListLogs,
                            Formatting.None,
                            new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
            PROP_xLog = "{ \"Log\":" + PROP_xLog + "}";
        }

        /// <summary>МЕТОД Изменяем логи xLog</summary>
        /// <param name="pUser">Кто создал/изменил протокол</param> 
        /// <param name="pTipLog">Тип лога Создан, Изменён, Удалён</param> 
        public void MET_ChangeLogs(int pUser, string pTipLog)
        {
            string _jLog = PROP_xLog;
          
            // Если нет логов и это НЕ создание протокола
            if (string.IsNullOrEmpty(PROP_xLog) && pTipLog != "Создан")
            {
                // СОЗДАЕМ первый лог (если изменили протокол вытаскиваем информацию из xDateUp, pDate)
                PROP_xLog = UserLog.MET_LogAdd("", PROP_pDate < PROP_xDateUp ? PROP_pDate : PROP_xDateUp, "Создан", PROP_xUserUp, "-");
                
                // Если возможно ИЗМЕНЯЛСЯ протокол, добавляем изменение
                if (PROP_xDateUp > PROP_pDate)
                    PROP_xLog = UserLog.MET_LogAdd("", PROP_xDateUp, "Изменён", PROP_xUserUp, "-");               
            }
            else
                PROP_xLog = UserLog.MET_LogAdd(PROP_xLog, DateTime.Now, pTipLog);         
        }

        ///<summary>МЕТОД Фабрика объекта Protokol</summary>
        /// <param name="pTip">Тип протокола</param> 
        /// <param name="pCodApstac">Код посещения</param> 
        /// <param name="pCodShablon">Код шаблона</param>
        /// <param name="pIndex">Индекс протокола</param> 
        public static UserProtokol MET_FactoryProtokol(eTipDocum pTip, decimal pCodApstac, int pCodShablon, int pIndex)
        {
            // Коллекция Protokol
            List<UserProtokol> _Protokol = ((VirtualModul)MyGlo.Modul).PUB_Protokol;
            //  Ищем в коллекции Protokol по типу, коду Apstac, номеру шаблона и индексу
            UserProtokol _Value = _Protokol.FirstOrDefault(p => p.PROP_TipProtokol.PROP_TipDocum == pTip
                && p.PROP_CodApstac == pCodApstac && p.PROP_NumShablon == pCodShablon && p.PROP_pIndex == pIndex);
            // Если не нашли, то пытаемся Protokol создать 
            if (_Value == null)
            {
                _Value = new UserProtokol();
                // Загружаем данные из SQL
                try
                {
                    var _TipProtokol = new MyTipProtokol(pTip);
                    SqlDataReader _SqlDataReader = MySql.MET_QuerySqlDataReader(MyQuery.MET_Protokol_Select_5(pCodApstac, pCodShablon, pIndex, _TipProtokol.PROP_Prefix));
                    _SqlDataReader.Read();
                    _Value.MET_LoadDataReader(_SqlDataReader);
                    _Value.PROP_TipProtokol = _TipProtokol;
                    _SqlDataReader.Close();
                    _Protokol.Add(_Value);
                }
                catch (Exception ex)
                {
                    MyGlo.PUB_Logger.Fatal(ex, "Ошибка Загрузки данных Protokol из SQL");
                    MyGlo.callbackEvent_sError(ex);
                    _Value = null;
                }
            }
            return _Value;
        }

        /// <summary>МЕТОД Фабрика объекта Protokol</summary>
        /// <param name="pTip">Тип протокола</param> 
        /// <param name="pCod">Код протокола</param> 
        public static UserProtokol MET_FactoryProtokol(eTipDocum pTip, int pCod)
        {
            // Коллекция Protokol
            List<UserProtokol> _Protokol = ((VirtualModul)MyGlo.Modul).PUB_Protokol;
            //  Ищем в коллекции Protokol по типу и коду протокола
            UserProtokol _Value = _Protokol.FirstOrDefault(p => p.PROP_TipProtokol.PROP_TipDocum == pTip && p.PROP_Cod == pCod);
            // Если не нашли, то пытаемся Protokol создать 
            if (_Value == null)
            {
                _Value = new UserProtokol();
                // Загружаем данные из SQL
                try
                {
                    var _TipProtokol = new MyTipProtokol(pTip);
                    SqlDataReader _SqlDataReader = MySql.MET_QuerySqlDataReader(MyQuery.MET_Protokol_Select_9(pCod, _TipProtokol.PROP_Prefix));
                    _SqlDataReader.Read();
                    _Value.MET_LoadDataReader(_SqlDataReader);
                    _Value.PROP_TipProtokol = _TipProtokol;
                    _SqlDataReader.Close();
                    _Protokol.Add(_Value);
                }
                catch (Exception ex)
                {
                    MyGlo.PUB_Logger.Fatal(ex, "Ошибка Загрузки данных Protokol из SQL");
                    MyGlo.callbackEvent_sError(ex);
                    _Value = null;
                }
            }
            return _Value;
        }

        /// <summary>МЕТОД Фабрика Массовая загрузка всех объектов Protokol одного посещения/стационара</summary>
        /// <param name="pTip">Тип протокола</param> 
        /// <param name="pCodApstacKL">Код посещения/пациента</param> 
        /// <param name="pTipFind">Ищем по CodApstac или по KL</param>
        public static bool MET_FactoryProtokolArray(eTipDocum pTip, decimal pCodApstacKL, string pTipFind = "CodApstac")
        {
            // Коллекция Protokol
            List<UserProtokol> _Protokol = ((VirtualModul)MyGlo.Modul).PUB_Protokol;
            try
            {
                var _TipProtokol = new MyTipProtokol(pTip);
                SqlDataReader _SqlDataReader = MySql.MET_QuerySqlDataReader(MyQuery.MET_Protokol_Select_12(pCodApstacKL, _TipProtokol.PROP_Prefix, pTipFind));
                // Перебираем весь поток и дабавляем все Protokolы
                while (_SqlDataReader.Read())
                {
                    UserProtokol _Value = new UserProtokol();
                    _Value.MET_LoadDataReader(_SqlDataReader);
                    _Value.PROP_TipProtokol = _TipProtokol;
                    _Protokol.Add(_Value);
                }
                _SqlDataReader.Close();

                // Загружаем все заголовки шаблонов ListShablon по загруженным протоколам
                UserListShablon.MET_FactoryListShablonArray(pTip);

                // Загружаем все заголовки шаблонов Shablon по загруженным протоколам
                UserShablon.MET_FactoryShablonArray(pTip);

                return true;
            }
            catch (Exception ex)
            {
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Массовой Загрузки данных Protokol из SQL");
                MyGlo.callbackEvent_sError(ex);
                return false;
            }
        }

        /// <summary>МЕТОД Чистим Протоколы и сопутствующие таблицы, при смене пациента</summary>
        /// <remarks>Делается в самом начале</remarks>
        public static void MET_ClearProtokol()
        {
            // Чистим Protokol
            ((VirtualModul)MyGlo.Modul).PUB_Protokol.Clear();
            // Чистим ListShablon
            ((VirtualModul)MyGlo.Modul).PUB_ListShablons.Clear();
            // Чистим Shablon
            ((VirtualModul)MyGlo.Modul).PUB_Shablon.Clear();
        }
    }
}
