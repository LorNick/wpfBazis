using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using wpfGeneral.UserModul;
using wpfStatic;

namespace wpfGeneral.UserStruct
{
    /// <summary>КЛАСС Информация о Шаблоне</summary>
    public class UserListShablon 
    {
        /// <summary>СВОЙСТВО Код шаблона</summary>
        public int PROP_Cod { get; set; }

        /// <summary>СВОЙСТВО Тип шаблона</summary>
        public int PROP_TipObsled { get; set; }

        /// <summary>СВОЙСТВО Полное наименование шаблона</summary>
        public string PROP_Name { get; set; }

        /// <summary>СВОЙСТВО Профиль врача/Отделение стационара</summary>
        public int PROP_ProfilVr { get; set; }

        /// <summary>СВОЙСТВО Наименование иконки</summary>
        public string PROP_Icon { get; set; }

        /// <summary>СВОЙСТВО Краткое наименование шаблона (для дерева)</summary>
        public string PROP_NameKr { get; set; }
                                   
        /// <summary>СВОЙСТВО Формат шаблонов</summary>
        public string PROP_xFormat { get; set; }

        /// <summary>СВОЙСТВО Теги (в json)</summary>
        public string PROP_xInfo { get; set; }

        /// <summary>СВОЙСТВО Тип протокола</summary>
        public MyTipProtokol PROP_TipProtokol { get; set; }

        ///<summary>МЕТОД Конвертация данных ListShablon из DataReader</summary>
        /// <param name="pDataReader">Поток данных из SQL</param> 
        private void MET_LoadDataReader(IDataRecord pDataReader)
        {
            try
            {
                PROP_Cod = pDataReader.GetInt32(pDataReader.GetOrdinal("Cod"));
                PROP_TipObsled = pDataReader.GetInt32(pDataReader.GetOrdinal("TipObsled"));
                PROP_Name = pDataReader.GetString(pDataReader.GetOrdinal("Name"));
                PROP_Icon = pDataReader.GetString(pDataReader.GetOrdinal("Icon"));
                PROP_NameKr = pDataReader.GetString(pDataReader.GetOrdinal("NameKr"));
                PROP_xFormat = pDataReader.GetString(pDataReader.GetOrdinal("xFormat"));
                PROP_xInfo = pDataReader[pDataReader.GetOrdinal("xInfo")] as string;
            }
            catch (Exception ex)
            {
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Конвертация данных ListShablon из DataReader");
                MyGlo.callbackEvent_sError(ex);
            }
        }
        
        ///<summary>МЕТОД Фабрика объекта ListShablon</summary>
        /// <param name="pTip">Тип протокола</param> 
        /// <param name="pCodShablon">Код шаблона</param>
        public static UserListShablon MET_FactoryListShablon(eTipDocum pTip, int pCodShablon)
        {
            // Коллекция ListShablons
            List<UserListShablon>_ListShablons = ((VirtualModul)MyGlo.Modul).PUB_ListShablons;
            //  Ищем в коллекции ListShablon по типу и номеру шаблона
            UserListShablon _Value = _ListShablons.FirstOrDefault(p => p.PROP_Cod == pCodShablon && p.PROP_TipProtokol.PROP_TipDocum == pTip);
            // Если не нашли, то пытаемся ListShablon создать 
            if (_Value == null)
            {
                _Value = new UserListShablon();
                // Загружаем данные из SQL
                try
                {
                    var _TipProtokol = new MyTipProtokol(pTip);
                    SqlDataReader _SqlDataReader = MySql.MET_QuerySqlDataReader(MyQuery.MET_ListShablon_Select_3(pCodShablon, _TipProtokol.PROP_Prefix));
                    _SqlDataReader.Read();
                    _Value.MET_LoadDataReader(_SqlDataReader);
                    _Value.PROP_TipProtokol = _TipProtokol;
                    _SqlDataReader.Close();
                    _ListShablons.Add(_Value);
                }
                catch (Exception ex)
                {
                    MyGlo.PUB_Logger.Fatal(ex, "Ошибка Загрузки данных ListShablon из SQL");
                    MyGlo.callbackEvent_sError(ex);
                    _Value = null;
                }
            }
            return _Value;
        }

        /// <summary>МЕТОД Фабрика Массовая загрузка всех объектов ListShablon для загруженной коллекции Protokol</summary>
        /// <param name="pTip">Тип протокола</param> 
        /// <remarks>Делается как правило разово из MET_FactoryProtokolArray</remarks>
        public static bool MET_FactoryListShablonArray(eTipDocum pTip)
        {
            // Коллекция ListShablons
            List<UserListShablon> _ListShablons = ((VirtualModul)MyGlo.Modul).PUB_ListShablons;
            // Находим список кодов шаблонов (пример: 101, 110, 120, 0), в конце добавляем ноль, что бы перекрыть последнюю запятую или в пустом списке был просто ноль
            string _NomShablons = ((VirtualModul)MyGlo.Modul).PUB_Protokol.Select(i => i.PROP_NumShablon).Distinct().Aggregate("", (s, i) => s + i + ", ") + "0";
            try
            {
                var _TipProtokol = new MyTipProtokol(pTip);
                SqlDataReader _SqlDataReader = MySql.MET_QuerySqlDataReader(MyQuery.MET_ListShablon_Select_4(_TipProtokol.PROP_Prefix, _NomShablons));
                // Перебираем весь поток и дабавляем все строки ListShablon
                while (_SqlDataReader.Read())
                {
                    UserListShablon _Value = new UserListShablon();
                    _Value.MET_LoadDataReader(_SqlDataReader);
                    _Value.PROP_TipProtokol = _TipProtokol;
                    _ListShablons.Add(_Value);
                }
                _SqlDataReader.Close();
                return true;
            }
            catch (Exception ex)
            {
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Массовой Загрузки данных ListShablon из SQL");
                MyGlo.callbackEvent_sError(ex);
                return false;
            }
        }
    }
}

