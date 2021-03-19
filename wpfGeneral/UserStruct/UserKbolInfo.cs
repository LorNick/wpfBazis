using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using wpfGeneral.UserModul;
using wpfStatic;

namespace wpfGeneral.UserStruct
{
    /// <summary>КЛАСС KbolInfo</summary>
    public class UserKbolInfo
    {
        /// <summary>СВОЙСТВО Код записи</summary>
        public int PROP_Cod { get; set; }

        /// <summary>СВОЙСТВО Код записи (KL из kbol, IND из АPSTAC, Cod из APAC ...)</summary>
        public decimal PROP_CodZap { get; set; }

        /// <summary>СВОЙСТВО Флаг связанной таблицы (kbol, apaN, ast)</summary>
        public string PROP_Tab { get; set; }

        /// <summary>СВОЙСТВО Код пациента из kbol</summary>
        public decimal PROP_KL { get; set; }

        /// <summary>СВОЙСТВО Строка данных (в json)</summary>
        public string PROP_jTag { get; set; }

        /// <summary>СВОЙСТВО Логи (в json)</summary>
        public string PROP_xLog { get; set; }

        /// <summary>СВОЙСТВО Теги (в json)</summary>
        public string PROP_xInfo { get; set; }

        /// <summary>СВОЙСТВО Подача данных в реестр 0 - не подавать, 1 - подавать в ОМС</summary>
        public int PROP_Oms { get; set; }

        /// <summary>СВОЙСТВО Объект json</summary>
        public JObject PROP_Json { get; set; }

        /// <summary>СВОЙСТВО Флаг - новая запись</summary>
        public bool PROP_FlagNew { get; set; }

        /// <summary>СВОЙСТВО Флаг - меняли ли данные</summary>
        public bool PROP_FlagChange { get; set; }

        ///<summary>МЕТОД Загрузка данных KbolInfo из SQL</summary>
        /// <param name="pTab">Флаг связанной таблицы (kbol, apaN, ast)</param> 
        /// <param name="pCodZap">Код записи (KL из kbol, IND из АPSTAC, Cod из APAC ...)</param> 
        public bool MET_LoadSQL(string pTab, decimal pCodZap)
        {
            bool _Value;
            try
            {
                SqlDataReader _SqlDataReader = MySql.MET_QuerySqlDataReader(MyQuery.MET_kbolInfo_Select_1(pTab, pCodZap));
                if (_SqlDataReader.Read())
                {
                    PROP_Cod = _SqlDataReader.GetInt32(_SqlDataReader.GetOrdinal("Cod"));
                    PROP_CodZap = _SqlDataReader.GetDecimal(_SqlDataReader.GetOrdinal("CodZap"));
                    PROP_Tab = _SqlDataReader.GetString(_SqlDataReader.GetOrdinal("Tab"));
                    PROP_KL = _SqlDataReader.GetDecimal(_SqlDataReader.GetOrdinal("KL"));
                    PROP_jTag = _SqlDataReader.GetString(_SqlDataReader.GetOrdinal("jTag"));
                    PROP_xLog = _SqlDataReader[_SqlDataReader.GetOrdinal("xLog")] as string;
                    PROP_xInfo = _SqlDataReader[_SqlDataReader.GetOrdinal("xInfo")] as string;
                    PROP_Oms = _SqlDataReader.GetInt32(_SqlDataReader.GetOrdinal("Oms"));

                    _Value = true;
                }
                else // не нашли в SQL данных
                {
                    _Value = false;
                }
                _SqlDataReader.Close();
            }
            catch (Exception ex)
            {
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Загрузки данных KbolInfo из SQL");
                MyGlo.callbackEvent_sError(ex);
                _Value = false;
            }

            return _Value;
        }
        
        ///<summary>МЕТОД Сохраняем строку KbolInfo в SQL</summary>
        public bool MET_SaveSQL()
        {
            // Если запись не меняли, выходим
            if (!PROP_FlagChange)
                return false;

            PROP_jTag = PROP_Json.ToString();

            // Если новая запись
            if (PROP_FlagNew)
            {
                PROP_Cod = MySql.MET_GetNextRef(69);
                MySql.MET_QueryNo(MyQuery.MET_kbolInfo_Insert_1(PROP_Cod, PROP_CodZap, PROP_Tab, PROP_KL, PROP_jTag, PROP_Oms));
                PROP_FlagNew = false;
            }
            else
                MySql.MET_QueryNo(MyQuery.MET_kbolInfo_Update_1(PROP_Cod, PROP_jTag, PROP_Oms));

            PROP_FlagChange = false;
            return true;
        }

        ///<summary>МЕТОД Обновляем строку KbolInfo в SQL, только если она уже была ранее создана</summary>
        /// <remarks>Данные берем не из PROP_Json, а из текстовой строки PROP_jTag, предварительно их проверив</remarks>
        public bool MET_UpdateSQL()
        {
            // Если запись не меняли, выходим
            if (!PROP_FlagChange)
                return false;

            // Если новая запись, тоже выходим
            if (PROP_FlagNew)
                return false;

            // Проверяем json строку  на правильность
            try
            {
                PROP_Json = JObject.Parse(PROP_jTag);               
            }
            catch (JsonReaderException)
            {
                MessageBox.Show($"Не корректная json строка!", "Ошибка");
                return false;
            }

            MySql.MET_QueryNo(MyQuery.MET_kbolInfo_Update_1(PROP_Cod, PROP_jTag, PROP_Oms));

            PROP_FlagChange = false;
            MessageBox.Show($"Успешно сохранено!");
            return true;
        }

        ///<summary>МЕТОД Меняем данные KbolInfo (строка)</summary>
        /// <param name="pTag">Имя тега</param> 
        /// <param name="pValue">Значение</param> 
        public bool MET_Change(string pTag, dynamic pValue)
        {
            // Находим старое значение тега, если этот тег есть (не null)
            dynamic _Json = (string)PROP_Json[pTag] == null ? "" : PROP_Json[pTag].ToObject<dynamic>();
            // Преобразовываем тип int в long, так как Json, не использует int, а сразу long
            Type _Type = pValue.GetType().Name == "Int32" ? typeof(long) : pValue.GetType();
            // Если тип double, то сокращаем до 2х знаков
            if (_Type == typeof(double))
            {
                pValue = Math.Round(pValue, 2);
            }
            // Если у существующего тега разные типы или разные значения, то записываем изменения
            if (_Json.GetType() != _Type || _Json != pValue)
            {
                PROP_Json[pTag] = pValue;       // меняем значение или создаем новый тег с этим значением
                PROP_FlagChange = true;         // пометка на изменение
                return true;
            }
            return false;
        }

        ///<summary>МЕТОД Удаляем данный тег KbolInfo (строка)</summary>
        /// <param name="pTag">Имя тега</param> 
        public bool MET_Delete(string pTag)
        {
            // Если данный тег в наличии, то удаляем его
            if ((string) PROP_Json[pTag] != null)
            {
                PROP_Json.Property(pTag).Remove();
                PROP_FlagChange = true;
                return true;
            }
            return false;
        }

        ///<summary>МЕТОД Меняем поле Oms в KbolInfo</summary>
        /// <param name="pOms">Признак подачи в реестр ОМС</param> 
        public void MET_ChangeOms(int pOms)
        {
            // Если данный тег в наличии, то удаляем его
            if (PROP_Oms != pOms)
            {
                PROP_Oms = pOms;
                PROP_FlagChange = true;               
            }           
        }

        ///<summary>МЕТОД Cохраняем объекты KbolInfo в SQL</summary>
        public static void MET_SaveKbolInfo()
        {
            // Коллекция KbolInfo которые не зафиксированы в SQL
            List<UserKbolInfo> _KbolInfo = ((VirtualModul)MyGlo.Modul).PUB_KbolInfo.FindAll(p => p.PROP_FlagChange);
            foreach (var _Value in _KbolInfo)
            {
                _Value.MET_SaveSQL();
            }
        }
        
        ///<summary>МЕТОД Фабрика объекта KbolInfo</summary>
        /// <param name="pTab">Флаг связанной таблицы (apaN, ast, par, kdl)</param> 
        /// <param name="pCodZap">Код записи (IND из АPSTAC, Cod из APAC ...)</param> 
        /// <param name="pKL">Код пациента</param>
        public static UserKbolInfo MET_FactoryKbolInfo(string pTab, decimal pCodZap, decimal pKL)
        {
            // Коллекция KbolInfo
            List<UserKbolInfo> _KbolInfo = ((VirtualModul)MyGlo.Modul).PUB_KbolInfo;
            //  Ищем в коллекции KbolInfo по типу, коду Apstac/Apac.., номеру шаблона и индексу
            UserKbolInfo _Value = _KbolInfo.FirstOrDefault(p => p.PROP_Tab == pTab && p.PROP_CodZap == pCodZap);
            // Если не нашли, то пытаемся KbolInfo создать 
            if (_Value == null)
            {
                _Value = new UserKbolInfo();
                // Загружаем данные из SQL
                if (_Value.MET_LoadSQL(pTab, pCodZap))
                    _KbolInfo.Add(_Value);
                else // не нашли данных ни в коллекции, ни в SQL - будем пытаться создать пустышку
                {
                    _Value.PROP_CodZap = pCodZap;
                    _Value.PROP_Tab = pTab;
                    _Value.PROP_KL = pKL;
                    _Value.PROP_FlagNew = true;      // помечаем, что новая запись
                    _KbolInfo.Add(_Value);
                }
                _Value.PROP_Json = JObject.Parse(_Value.PROP_jTag ?? "{ }");
            }
            return _Value;
        }
    }
}
