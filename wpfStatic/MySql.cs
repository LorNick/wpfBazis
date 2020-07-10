using System;
using System.Collections;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data;
using System.Xml;
using System.Linq;

namespace wpfStatic
{
    /// <summary>КЛАСС работы с SQL</summary>
    public static class MySql
    {
        /// <summary>Строка подключения к SQL Server`у</summary>
        public static readonly SqlConnection PUB_SqlConct;

        /// <summary>Точка входа подключения к SQL Server`у</summary>
        public static readonly DataContext PUB_Context;

        // КОНСТРУКТОР
        static MySql()
        {
            PUB_SqlConct = new SqlConnection();                                 // создаем подключение к базе
            PUB_SqlConct.ConnectionString = MET_ConSql();                       // строка инициализации подключения к базе
            PUB_Context = new DataContext(PUB_SqlConct);
        }

        /// <summary>Строка подключения к SQL Server`у</summary>
        /// <returns>Возвращаем строку подключения к серверу MS SQL</returns>
        public static string MET_ConSql()
        {
            string _Connect = "packet size=4096;";
            _Connect += "user id=Bazis;";
            switch (MyGlo.Server)
            {
                case 1:                                                         // Локальный
                    _Connect += "password=RhtvyTDF1V9f5#8;";
                    _Connect += "data source=127.0.0.1;";
                    break;
                case 2:                                                         // Удаленное подключение к филиалу
                    _Connect += "password=RhtvyTDF1V9f5#8;";
                    _Connect += "data source=10.30.104.5;";
                    break;
                case 3:                                                         // Главный корпус
                    _Connect += "password=RhtvyTDF1V9f5#8;";
                    _Connect += "data source=192.168.0.8;";
                    break;
                case 5:                                                         // Филиал
                    _Connect += "password=RhtvyTDF1V9f5#8;";
                    _Connect += "data source=192.168.0.8;";
                    break;
                case 6:                                                         // Главный из вне
                    _Connect += "password=RhtvyTDF1V9f5#8;";
                    _Connect += "data source=10.30.103.8;";
                    break; 
            }
            _Connect += "persist security info=False;";
            _Connect += "initial catalog=Bazis;";
            _Connect += "Connect Timeout=90000000;";
            // возвращаем строку подключения к SQL Server`у 
            return _Connect;
            // ~~~~ Строка подключения к SQL Server`у ~~~~
        }

        #region ---- Методы с Подключением к серверу ----
        /// <summary>Заполняем DataSet Базиса (<see cref="MyGlo.DataSet"/>)</summary>
        /// <param name="pStrSQL">Строка SQL кода</param>
        /// <param name="pNameTable">Наименование таблицы</param>
        public static int MET_DsAdapterFill(string pStrSQL, string pNameTable)
        {
            int _CountTimeout = 0;                                              // попытки достучаться до сервера (5 попыток)
            SqlDataAdapter _SqlDa = new SqlDataAdapter(pStrSQL, PUB_SqlConct);  // создаем адаптер
        label1:
            try
            {
                // Предварительно удаляем таблицу
                if (MyGlo.DataSet.Tables[pNameTable] != null)
                    MyGlo.DataSet.Tables[pNameTable].Clear();
                // Загружаем данные
                return _SqlDa.Fill(MyGlo.DataSet, pNameTable);
            }
            catch (SqlException ex)                                             // проблемы со связью
            {
                if (ex.Number == -2 && _CountTimeout++ < 5)
                    goto label1;
                ex.Data["SQL"] = pStrSQL;
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Загрузки данных в DataSet из SQL");
                MyGlo.callbackEvent_sError(ex);
                goto label1;
            }
            catch                                                               // остальные проблемы
            {
                MyGlo.DataSet.Tables.Remove(pNameTable);
                return _SqlDa.Fill(MyGlo.DataSet, pNameTable);
            }
        }

        /// <summary>Выполняем запрос (без возврата значений)</summary>
        /// <param name="pStrSQL">Строка SQL кода</param>
        /// <returns>true -  успешное выполнение запроса, false - что то пошло не так</returns>
        public static bool MET_QueryNo(string pStrSQL)
        {
            int _CountTimeout = 0;                                              // попытки достучаться до сервера (5 попыток)
            bool _Result = false;
        label1:
            try
            {
                if (PUB_SqlConct.State == ConnectionState.Closed) PUB_SqlConct.Open();          // открываем базу       
                SqlCommand _SqlCom = new SqlCommand(pStrSQL, PUB_SqlConct);     // создаем команду
                try
                {
                    _SqlCom.ExecuteNonQuery();                                  // выполянем запрос
                    _Result = true;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == -2 && _CountTimeout++ < 5)
                        goto label1;
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (без возврата значения)");
                }
                catch (Exception ex)
                {
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (без возврата значения)");
                }
            }
            catch (Exception ex)
            {
                ex.Data["SQL"] = pStrSQL;
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Запроса SQL (без возврата значения)");
                MyGlo.callbackEvent_sError(ex);
                goto label1;
            }
            PUB_SqlConct.Close();                                               // закрываем подключение
            return _Result;
        }

        /// <summary>Выполняем запрос (возвращаем целое число)</summary>
        /// <param name="pStrSQL">Строка SQL кода</param>
        /// <returns>Возвращаем целое число</returns>
        public static int MET_QueryInt(string pStrSQL)
        {
            int _Value = 0;                                                     // результат
            int _CountTimeout = 0;                                              // попытки достучаться до сервера (5 попыток)
        label1:
            try
            {
                if (PUB_SqlConct.State == ConnectionState.Closed) PUB_SqlConct.Open();          // открываем базу       
                SqlCommand _SqlCom = new SqlCommand(pStrSQL, PUB_SqlConct);     // создаем команду
                try
                {
                    _Value = MyMet.MET_ParseInt(_SqlCom.ExecuteScalar());       // выполянем запрос
                }
                catch (SqlException ex)
                {
                    if (ex.Number == -2 && _CountTimeout++ < 5)
                        goto label1;
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (возвращающего целое число)");
                }
                catch (Exception ex)
                {
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (возвращающего целое число)");
                }
            }
            catch (Exception ex)
            {
                ex.Data["SQL"] = pStrSQL;
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Запроса SQL (возвращающего целое число)");
                MyGlo.callbackEvent_sError(ex);
                goto label1;
            }
            PUB_SqlConct.Close();                                               // закрываем подключение
            return _Value;
        }

        /// <summary>Выполняем запрос (возвращаем реальное число)</summary>
        /// <param name="pStrSQL">Строка SQL кода</param>
        /// <returns>Возвращаем реальное число</returns>
        public static decimal MET_QueryDec(string pStrSQL)
        {
            decimal _Value = 0;                                                 // результат
            int _CountTimeout = 0;                                              // попытки достучаться до сервера (5 попыток)
            SqlConnection _SqlCon = new SqlConnection();                        // создаем подключение к базе
            _SqlCon.ConnectionString = MET_ConSql();                            // строка инициализации подключения к базе
        label1:
            try
            {
                if (PUB_SqlConct.State == ConnectionState.Closed) PUB_SqlConct.Open();          // открываем базу       
                SqlCommand _SqlCom = new SqlCommand(pStrSQL, PUB_SqlConct);     // создаем команду
                try
                {
                    _Value = MyMet.MET_ParseDec(_SqlCom.ExecuteScalar());       // выполянем запрос
                }
                catch (SqlException ex)
                {
                    if (ex.Number == -2 && _CountTimeout++ < 5)
                        goto label1;
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (возвращающего реальное число)");
                }
                catch (Exception ex)
                {
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (возвращающего реальное число)");
                }
            }
            catch (Exception ex)
            {
                ex.Data["SQL"] = pStrSQL;
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Запроса SQL (возвращающего реальное число)");
                MyGlo.callbackEvent_sError(ex);
                goto label1;
            }
            PUB_SqlConct.Close();                                               // закрываем подключение
            return _Value;
        }

        /// <summary>МЕТОД Возвращает true, если есть хоть одна строка</summary>
        /// <param name="pStrSQL">Строка SQL кода</param>
        /// <returns>Возвращаем bool</returns>
        public static bool MET_QueryBool(string pStrSQL)
        {
            bool _Value = false;                                                // результат
            int _CountTimeout = 0;                                              // попытки достучаться до сервера (5 попыток)
        label1:
            try
            {
                if (PUB_SqlConct.State == ConnectionState.Closed) PUB_SqlConct.Open();          // открываем базу       
                SqlCommand _SqlCom = new SqlCommand(pStrSQL, PUB_SqlConct);     // создаем команду
                try
                {
                    if(_SqlCom.ExecuteScalar() == null)                         // выполянем запрос
                        _Value = false;                                         // нет строк
                    else
                        _Value = true;                                          // есть строки
                }
                catch (SqlException ex)
                {
                    if (ex.Number == -2 && _CountTimeout++ < 5)
                        goto label1;
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (возвращающего bool)");
                }
                catch (Exception ex)
                {
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (возвращающего bool)");
                }
            }
            catch (Exception ex)
            {
                ex.Data["SQL"] = pStrSQL;
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Запроса SQL (возвращающего bool)");
                MyGlo.callbackEvent_sError(ex);
                goto label1;
            }
            PUB_SqlConct.Close();                                               // закрываем подключение
            return _Value;
        }

        /// <summary>Выполняем запрос (возвращаем строку)</summary>
        /// <param name="pStrSQL">Строка SQL кода</param>
        /// <returns>Возвращаем строку</returns>
        public static string MET_QueryStr(string pStrSQL)
        {
            string _Value = "";                                                 // результат
            int _CountTimeout = 0;                                              // попытки достучаться до сервера (5 попыток)
        label1:
            try
            {
                if (PUB_SqlConct.State == ConnectionState.Closed) PUB_SqlConct.Open();          // открываем базу       
                SqlCommand _SqlCom = new SqlCommand(pStrSQL, PUB_SqlConct);     // создаем команду
                try
                {
                    _Value = (string)_SqlCom.ExecuteScalar();                   // выполянем запрос
                }
                catch (SqlException ex)
                {
                    if (ex.Number == -2 && _CountTimeout++ < 5)
                        goto label1;
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (возвращающего строку)");
                }
                catch (Exception ex)
                {
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (возвращающего строку)");
                }
                PUB_SqlConct.Close();                                           // закрываем подключение
            }
            catch (Exception ex)
            {
                ex.Data["SQL"] = pStrSQL;
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Запроса SQL (возвращающего строку)");
                MyGlo.callbackEvent_sError(ex);
                goto label1;
            }
            return _Value;
        }

        /// <summary>Выполняем запрос (возвращаем дату)</summary>
        /// <param name="pStrSQL">Строка SQL кода</param>
        /// <returns>Возвращаем дату</returns>
        public static DateTime MET_QueryDat(string pStrSQL)
        {
            DateTime _Value = DateTime.Today;                                   // результат
            int _CountTimeout = 0;                                              // попытки достучаться до сервера (5 попыток)
        label1:
            try
            {
                if (PUB_SqlConct.State == ConnectionState.Closed) PUB_SqlConct.Open();          // открываем базу       
                SqlCommand _SqlCom = new SqlCommand(pStrSQL, PUB_SqlConct);     // создаем команду

                try
                {
                    _Value = (DateTime)_SqlCom.ExecuteScalar();                 // выполянем запрос
                }
                catch (SqlException ex)
                {
                    if (ex.Number == -2 && _CountTimeout++ < 5)
                        goto label1;
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (возвращающего дату)");
                }
                catch (Exception ex)
                {
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (возвращающего дату)");
                }
            }
            catch (Exception ex)
            {
                ex.Data["SQL"] = pStrSQL;
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Запроса SQL (возвращающего дату)");
                MyGlo.callbackEvent_sError(ex);
                goto label1;
            }
            PUB_SqlConct.Close();                                               // закрываем подключение
            return _Value;
        }

        /// <summary>Выполняем запрос (возвращаем Hashtable)</summary>
        /// <param name="pStrSQL">Строка SQL кода</param>
        /// <returns>Возвращаем Hashtable (список: ключ/значение)</returns>
        public static Hashtable MET_QueryHash(string pStrSQL)
        {
            Hashtable _Value = new Hashtable();
            int _CountTimeout = 0;                                              // попытки достучаться до сервера (5 попыток)
        label1:
            try
            {
                if (PUB_SqlConct.State == ConnectionState.Closed) PUB_SqlConct.Open();          // открываем базу    
                SqlCommand _SqlCom = new SqlCommand(pStrSQL, PUB_SqlConct);     // создаем команду
                try
                {
                    SqlDataReader _SqlDr = _SqlCom.ExecuteReader();             // выполянем запрос
                    _SqlDr.Read();
                    if (_SqlDr.HasRows) // есть ли записи
                    {
                        for (int i = 0; i <= _SqlDr.FieldCount - 1; i++)
                        {
                            _Value.Add(_SqlDr.GetName(i), _SqlDr.GetValue(i));
                        }
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == -2 && _CountTimeout++ < 5)
                        goto label1;
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (возвращающего Hashtable)");
                }
                catch (Exception ex)
                {
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (возвращающего Hashtable)");
                }
            }
            catch (Exception ex)
            {
                ex.Data["SQL"] = pStrSQL;
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Запроса SQL (возвращающего Hashtable)");
                MyGlo.callbackEvent_sError(ex);
                goto label1;
            }
            PUB_SqlConct.Close();                                               // закрываем подключение
            return _Value;
        }

        /// <summary>Выполняем запрос (возвращаем SqlDataReader)</summary>
        /// <param name="pStrSQL">Строка SQL кода</param>
        /// <returns>Возвращаем SqlDataReader</returns>
        public static SqlDataReader MET_QuerySqlDataReader(string pStrSQL)
        {
            int _CountTimeout = 0;                                              // попытки достучаться до сервера (5 попыток)     
            label1:
            try
            {
                if (PUB_SqlConct.State == ConnectionState.Closed) PUB_SqlConct.Open();          // открываем базу    
                SqlCommand _SqlCom = new SqlCommand(pStrSQL, PUB_SqlConct);     // создаем команду
                try
                {
                    SqlDataReader _SqlDr = _SqlCom.ExecuteReader(CommandBehavior.CloseConnection);  // выполянем запрос  
                    return _SqlDr;
                }
                catch (SqlException ex)
                {
                    if (ex.Number == -2 && _CountTimeout++ < 5)
                        goto label1;
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Fatal(ex, "Ошибка Запроса SQL (возвращающего DataReader)");
                    MyGlo.callbackEvent_sError(ex);
                    goto label1;
                }
                catch (Exception ex)
                {
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Fatal(ex, "Ошибка Запроса SQL (возвращающего DataReader)");
                    MyGlo.callbackEvent_sError(ex);
                    goto label1;
                }
            }
            catch (Exception ex)
            {
                ex.Data["SQL"] = pStrSQL;
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Запроса SQL (возвращающего DataReader)");
                MyGlo.callbackEvent_sError(ex);
                goto label1;
            }          
        }

        /// <summary>Массовая вставка данных в SQL из DataTable</summary>
        /// <param name="pDataTable">Строка SQL кода</param>
        /// <returns>Возвращаем bool</returns>
        public static bool MET_SqlBulkCopy(DataTable pDataTable)
        {
            bool _Result;             
            try
            {
                if (PUB_SqlConct.State == ConnectionState.Closed) PUB_SqlConct.Open();          // открываем базу 

                // Создаем объект SqlBulkCopy, указываем таблицу назначения и загружаем.
                using (var loader = new SqlBulkCopy(PUB_SqlConct))
                {
                    loader.DestinationTableName = "StrahZero";
                    loader.WriteToServer(pDataTable);
                }
                _Result = false;
            }
            catch (Exception ex)
            {               
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка массовой вставки SqlBulkCopy");
                MyGlo.callbackEvent_sError(ex);
                _Result = false;
            }
            return _Result;
        }

        /// <summary>МЕТОД Работа с изображениями</summary>
        /// <param name="pStrSQL">Строка SQL кода</param>
        /// <param name="pImage">Наименование поля картинки</param>
        /// <param name="photo">Байтовый массив изображения</param>
        public static void MET_QueryNoImage(string pStrSQL, string pImage, byte[] photo)
        {  
            int _CountTimeout = 0;                                              // попытки достучаться до сервера (5 попыток)
        label1:
            try
            {
                SqlCommand _SqlCom = new SqlCommand(pStrSQL, PUB_SqlConct);     // создаем команду
                _SqlCom.Parameters.Add("@Image", SqlDbType.Image, photo.Length).Value = photo;
                if (PUB_SqlConct.State == ConnectionState.Closed) PUB_SqlConct.Open();          // открываем базу
                try
                {
                    _SqlCom.ExecuteNonQuery();                                  // выполянем запрос
                }
                catch (SqlException ex)
                {
                    if (ex.Number == -2 && _CountTimeout++ < 5)
                        goto label1;
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (для работы с изображениями)");
                }
                catch (Exception ex)
                {
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (для работы с изображениями)");
                }
            }
            catch (Exception ex)
            {
                ex.Data["SQL"] = pStrSQL;
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Запроса SQL (для работы с изображениями)");
                MyGlo.callbackEvent_sError(ex);
                goto label1;
            }
            PUB_SqlConct.Close();                                               // закрываем подключение
        }

        /// <summary>Выполняем XML запрос (без возврата значений)</summary>
        /// <param name="pStrSQL">Строка SQL кода</param>
        /// <param name="pFile">Имя файла</param>
        public static void MET_QueryXML(string pStrSQL, string pFile)
        {
            int _CountTimeout = 0;                                              // попытки достучаться до сервера (5 попыток)

            XmlDocument _Doc = new XmlDocument();
            XmlDeclaration _XmlDecl = _Doc.CreateXmlDeclaration("1.0", "windows-1251", null);        
        label1:
            try
            {
                if (PUB_SqlConct.State == ConnectionState.Closed) PUB_SqlConct.Open();          // открываем базу       
                SqlCommand _SqlCom = new SqlCommand(pStrSQL, PUB_SqlConct);     // создаем команду
                _SqlCom.CommandTimeout = 90000000;

                try
                {
                    using (XmlReader _Read = _SqlCom.ExecuteXmlReader())        // выполянем запрос
                    {
                        _Doc.Load(_Read);

                        _Read.Close();
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == -2 && _CountTimeout++ < 5)
                        goto label1;
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (формирование XML файла)");
                }
                catch (Exception ex)
                {
                    ex.Data["SQL"] = pStrSQL;
                    MyGlo.PUB_Logger.Error(ex, "Ошибка Запроса SQL (формирование XML файла)");
                }
            }
            catch (Exception ex)
            {
                ex.Data["SQL"] = pStrSQL;
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Запроса SQL (формирование XML файла)");
                MyGlo.callbackEvent_sError(ex);
                goto label1;
            }
            // Добавляем ветку рут
            XmlElement _Root = _Doc.DocumentElement;
            _Doc.InsertBefore(_XmlDecl, _Root);

            PUB_SqlConct.Close();                                               // закрываем подключение
                       
            // Имя файла
            string _PathToXml = @"c:\1Reestrs\" + pFile + ".xml";
            // Сохроняем xml в файл
            _Doc.Save(_PathToXml);
        }
        #endregion

        #region ---- Методы без Подключения к серверу ----
        /// <summary>Находим у таблицы Cod из NextRef</summary>
        /// <param name="pNomTabl">Код нужной табицы из NextRef</param>
        /// <returns>Возвращаем Cod</returns>
        public static int MET_GetNextRef(int pNomTabl)
        {
            // Находим новый номер
            int _Cod = MET_QueryInt(MyQuery.NextRef_Select_1(pNomTabl));
            // Увеличиваем счетчик
            MET_QueryNo(MyQuery.NextRef_Update_1(pNomTabl));
            return _Cod;
        }
        
        /// <summary>Наименование (поле TKOD) справочников (с указанием текстового кода)</summary>
        /// <param name="pValCod">Текстовый код</param>
        /// <param name="pTable">Имя таблицы</param>
        /// <returns>Возвращаем наименование</returns>
        public static string MET_NameSpr(string pValCod, string pTable)
        {
            string _StrSql = $@"select TKOD
                               from dbo.{pTable}    
                               where KOD = '{pValCod}'";
            return MET_QueryStr(_StrSql);
        }

        /// <summary>Наименование (поле TKOD) справочников (с указанием числового кода)</summary>
        /// <param name="pValCod">Числовой код</param>
        /// <param name="pTable">Имя таблицы</param>
        /// <returns>Возвращаем наименование</returns>
        public static string MET_NameSpr(int pValCod, string pTable)
        {
            string _StrSql = $@"select TKOD
                            from dbo.{pTable}    
                            where KOD = {pValCod}";
            return MET_QueryStr(_StrSql);
        }

        /// <summary>Наименование справочников (с указанием 4х параметров)</summary>
        /// <param name="pValCod">Числовой код</param>
        /// <param name="pTable">Имя таблицы</param>
        /// <param name="pPoleSelect">Возвращаемое поле</param>
        /// <param name="pPoleWhere">Условие выборки</param> 
        /// <returns>Возвращаем наименование</returns>
        public static string MET_NameSpr(int pValCod, string pTable, string pPoleSelect, string pPoleWhere)
        {
            string _StrSql = $@"select {pPoleSelect}   
                            from dbo.{pTable}    
                            where {pPoleWhere} = {pValCod}";
            return MET_QueryStr(_StrSql);
        }       

        /// <summary>Находим максимальный Cod + 1 (целое число)</summary>
        /// <param name="pTable">Имя таблицы</param>
        /// <param name="pCod">Имя поля, по умолчанию Cod</param>
        /// <returns>Возвращаем наименование</returns>
        public static int MET_MaxCod(string pTable, string pCod = "Cod")
        {
            string _StrSql = $@"
                select max({pCod}) + 1
                from dbo.{pTable}";
            return MET_QueryInt(_StrSql);
        }

        /// <summary>МЕТОД Update отдельных ячеек</summary>
        /// <param name="pTable">Имя таблицы</param>
        /// <param name="pSet">Подставляем к SET к примеру "DateN = '01.01.2012'"</param>
        /// <param name="pCod">Код строки (Cod)</param>
        /// <returns>Возвращаем bool</returns>
        public static bool MET_UpdateCell(string pTable, string pSet, int pCod)
        {
            string _StrSql = $@"
                update dbo.{pTable}
                set {pSet}
                where Cod = {pCod}";
            return MET_QueryNo(_StrSql);
        }

        /// <summary>Пингуем сервер ДРУГОГО корпуса по IP (Pol, Fil)</summary>
        /// <returns>true - есть пинг</returns>
        public static bool MET_Ping()
        {   
            return MET_QueryInt("select dbo.Ping()") == 0;
        }
        #endregion
    }
}

