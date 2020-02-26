using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using wpfGeneral.UserModul;
using wpfStatic;

namespace wpfGeneral.UserStruct
{
    /// <summary>КЛАСС Шаблоны</summary>
    public class UserShablon : INotifyPropertyChanged
    {
        /// <summary>СВОЙСТВО Код вопроса</summary>
        public int PROP_Cod { get; set; }

        /// <summary>СВОЙСТВО Номер шаблона</summary>
        public int PROP_ID { get; set; }

        private byte PRI_Nomer;
        /// <summary>СВОЙСТВО Порядковый номер вопроса</summary>
        public byte PROP_Nomer
        {
            get { return PRI_Nomer; }
            set { PRI_Nomer = value; PART_RaisePropertyChanged("PROP_Nomer");}
        }

        private int PRI_VarId;
        /// <summary>СВОЙСТВО Уникальный индификатор вопроса</summary>
        public int PROP_VarId
        {
            get { return PRI_VarId; }
            set { PRI_VarId = value; PART_RaisePropertyChanged("PROP_VarId"); }
        }

        private string PRI_Maska;
        /// <summary>СВОЙСТВО Маска вопроса (должен стоять родительский VarID вопроса)</summary>
        public string PROP_Maska
        {
            get { return PRI_Maska; }
            set { PRI_Maska = value; PART_RaisePropertyChanged("PROP_Maska"); }
        }

        private byte PRI_Type;
        /// <summary>СВОЙСТВО Тип вопроса</summary>
        public byte PROP_Type
        {
            get { return PRI_Type; }
            set { PRI_Type = value; PART_RaisePropertyChanged("PROP_Type"); }
        }

        private string PRI_Razdel;
        /// <summary>СВОЙСТВО Раздел (группировка вопросов по теме)</summary>
        public string PROP_Razdel
        {
            get { return PRI_Razdel; }
            set { PRI_Razdel = value; PART_RaisePropertyChanged("PROP_Razdel"); }
        }

        private string PRI_Name;
        /// <summary>СВОЙСТВО Заголовок вопроса</summary>
        public string PROP_Name
        {
            get { return PRI_Name; }
            set { PRI_Name = value; PART_RaisePropertyChanged("PROP_Name"); }
        }

        private string PRI_ValueStart;
        /// <summary>СВОЙСТВО Значение по умолчанию</summary>
        public string PROP_ValueStart
        {
            get { return PRI_ValueStart; }
            set { PRI_ValueStart = value; PART_RaisePropertyChanged("PROP_ValueStart"); }
        }

        private string PRI_OutText;
        /// <summary>СВОЙСТВО Текст подставляющийся ПЕРЕД ответом</summary>
        public string PROP_OutText
        {
            get { return PRI_OutText; }
            set { PRI_OutText = value; PART_RaisePropertyChanged("PROP_OutText"); }
        }

        private string PRI_InText;
        /// <summary>СВОЙСТВО Текст подставляющийся ПОСЛЕ ответа</summary>
        public string PROP_InText
        {
            get { return PRI_InText; }
            set { PRI_InText = value; PART_RaisePropertyChanged("PROP_InText"); }
        }

        private string PRI_xFormat;
        /// <summary>СВОЙСТВО Формат ответа</summary>
        public string PROP_xFormat
        {
            get { return PRI_xFormat; }
            set { PRI_xFormat = value; PART_RaisePropertyChanged("PROP_xFormat"); }
        }

        private string PRI_xLua;
        /// <summary>СВОЙСТВО Код Lua (валидация данных)</summary>
        public string PROP_xLua
        {
            get { return PRI_xLua; }
            set { PRI_xLua = value; PART_RaisePropertyChanged("PROP_xLua"); }
        }

        private string PRI_xInfo;
        /// <summary>СВОЙСТВО Теги (в json)</summary>
        public string PROP_xInfo
        {
            get { return PRI_xInfo; }
            set { PRI_xInfo = value; PART_RaisePropertyChanged("PROP_xInfo"); }
        }


        /// <summary>СВОЙСТВО Тип протокола</summary>
        public MyTipProtokol PROP_TipProtokol { get; set; }

        /// <summary>СВОЙСТВО Флаг редактирования строки шаблона</summary>
        public bool PROP_FlagEdit { get; set; }


        /// <summary>Событие на изменение свойств</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        ///<summary>МЕТОД Вызываем изменение свойств</summary>
        ///<param name="pPropertyName">Имя свойства</param>
        public void PART_RaisePropertyChanged(string pPropertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(pPropertyName));
            // Ставим флаг редактирования
            PROP_FlagEdit = true;
        }

        ///<summary>МЕТОД Сохраняем ВЕСЬ шаблон данного элемента в SQL</summary>
        public void MET_SaveSQL()
        {
            // Коллекция Shablon
            List<UserShablon> _Shablon = ((VirtualModul)MyGlo.Modul).PUB_Shablon;
            //  Ищем в коллекции <Shablon> по типу и номеру шаблона все вопросы данного шаблона, которые были изменены
            List<UserShablon> _Value = _Shablon.Where(p => p.PROP_ID == PROP_ID && p.PROP_TipProtokol == PROP_TipProtokol && p.PROP_FlagEdit).ToList();
            // Если не нашли, то пытаемся <Shablon> создать 
            foreach (var _i in _Value)
            {
                MySql.MET_QueryNo(MyQuery.MET_Shablon_Update_1(_i.PROP_TipProtokol.PROP_Shablon, _i.PROP_Cod, _i.PROP_ID, _i.PROP_Nomer, _i.PRI_VarId, _i.PROP_Maska,
                                                                _i.PROP_Type, _i.PROP_Razdel, _i.PROP_Name, _i.PROP_ValueStart, _i.PROP_OutText, _i.PROP_InText,
                                                                _i.PROP_xFormat, _i.PROP_xLua, _i.PROP_xInfo));
                
                // Сбрасываем флаг редактирования
                _i.PROP_FlagEdit = false;
            }
            MessageBox.Show($"Сохранено {_Value.Count} вопросов");
        }

        ///<summary>МЕТОД Замена шаблона в SQL (при импорте из Excel)</summary>
        public static void MET_SaveExcelToSQL(eTipDocum pTip, int pCodShablon)
        {
            // Коллекция Shablon
            List<UserShablon> _Shablon = ((VirtualModul)MyGlo.Modul).PUB_Shablon;
            //  Ищем в коллекции <Shablon> по типу и номеру шаблона все вопросы данного шаблона
            List<UserShablon> _Value = _Shablon.Where(p => p.PROP_ID == pCodShablon && p.PROP_TipProtokol.PROP_TipDocum == pTip).ToList();
            // Удаляем шаблон из SQL
            MyTipProtokol _Tip = new MyTipProtokol(pTip);
            MySql.MET_QueryNo(MyQuery.MET_Shablon_Delete_1(pCodShablon, _Tip.PROP_Prefix));

            // Добавляем элементы шаблона в SQL 
            foreach (var _i in _Value)
            {
                MySql.MET_QueryNo(MyQuery.MET_Shablon_Insert_1(_i.PROP_TipProtokol.PROP_Shablon, _i.PROP_Cod, _i.PROP_ID, _i.PROP_Nomer, _i.PRI_VarId, _i.PROP_Maska,
                                                                _i.PROP_Type, _i.PROP_Razdel, _i.PROP_Name, _i.PROP_ValueStart, _i.PROP_OutText, _i.PROP_InText,
                                                                _i.PROP_xFormat, _i.PROP_xLua, _i.PROP_xInfo));

                // Сбрасываем флаг редактирования
                _i.PROP_FlagEdit = false;
            }
        }

        ///<summary>МЕТОД Конвертация данных Shablon из DataReader</summary>
        /// <param name="pDataReader">Поток данных из SQL</param> 
        private void MET_LoadDataReader(IDataRecord pDataReader)
        {
            try
            {
                PROP_Cod = pDataReader.GetInt32(pDataReader.GetOrdinal("Cod"));
                PROP_ID = pDataReader.GetInt32(pDataReader.GetOrdinal("ID"));
                PROP_Nomer = pDataReader.GetByte(pDataReader.GetOrdinal("Nomer"));
                PROP_VarId = pDataReader.GetInt32(pDataReader.GetOrdinal("VarId"));
                PROP_Maska = pDataReader.GetString(pDataReader.GetOrdinal("Maska"));
                PROP_Type = pDataReader.GetByte(pDataReader.GetOrdinal("Type"));
                PROP_Razdel = pDataReader.GetString(pDataReader.GetOrdinal("Razdel"));
                PROP_Name = pDataReader.GetString(pDataReader.GetOrdinal("Name"));
                PROP_ValueStart = pDataReader.GetString(pDataReader.GetOrdinal("ValueStart"));
                PROP_OutText = pDataReader.GetString(pDataReader.GetOrdinal("OutText"));
                PROP_InText = pDataReader.GetString(pDataReader.GetOrdinal("InText"));
                PROP_xFormat = pDataReader.GetString(pDataReader.GetOrdinal("xFormat"));
                PROP_xLua = pDataReader.GetString(pDataReader.GetOrdinal("xLua"));
                PROP_xInfo = pDataReader.GetString(pDataReader.GetOrdinal("xInfo"));
            }
            catch (Exception ex)
            {
                MyGlo.PUB_Logger.Fatal(ex, "Ошибка Конвертация данных Shablon из DataReader");
                MyGlo.callbackEvent_sError(ex);
            }
        }

        ///<summary>МЕТОД Фабрика объектов Shablon</summary>
        /// <param name="pTip">Тип протокола</param> 
        /// <param name="pCodShablon">Код шаблона</param>
        public static List<UserShablon> MET_FactoryListShablon(eTipDocum pTip, int pCodShablon)
        {
            // Коллекция Shablon
            List<UserShablon> _Shablons = ((VirtualModul)MyGlo.Modul).PUB_Shablon;
            //  Ищем в коллекции <Shablon> по типу и номеру шаблона
            List<UserShablon> _Value = _Shablons.Where(p => p.PROP_ID == pCodShablon && p.PROP_TipProtokol.PROP_TipDocum == pTip).ToList();
            // Если не нашли, то пытаемся <Shablon> загрузить из SQL 
            if (!_Value.Any())
            {
                try
                {
                    MyTipProtokol _Tip = new MyTipProtokol(pTip);
                    SqlDataReader _SqlDataReader = MySql.MET_QuerySqlDataReader(MyQuery.MET_Shablon_Select_3(pCodShablon, _Tip.PROP_Prefix));

                    while (_SqlDataReader.Read())
                    {
                        UserShablon _Shablon = new UserShablon();
                        _Shablon.MET_LoadDataReader(_SqlDataReader);
                        _Shablon.PROP_TipProtokol = _Tip;
                        // После загрузки, сбрасываем флаг редактирования
                        _Shablon.PROP_FlagEdit = false;
                        _Value.Add(_Shablon);
                    }
                    _SqlDataReader.Close();
                }
                catch (Exception ex)
                {
                    MyGlo.PUB_Logger.Fatal(ex, "Ошибка Загрузки данных Shablon из SQL");
                    MyGlo.callbackEvent_sError(ex);
                    _Value = null;
                }
                // Добавляем наши вопросы в библиотеку
                if (_Value != null) _Shablons?.AddRange(_Value);
            }
            return _Value;
        }

        /// <summary>МЕТОД Фабрика Массовая загрузка всех объектов Shablon для загруженной коллекции Protokol</summary>
        /// <param name="pTip">Тип протокола</param> 
        /// <remarks>Делается как из MET_FactoryProtokolArray</remarks>
        public static bool MET_FactoryShablonArray(eTipDocum pTip)
        {
            // Коллекция ListShablons
            List<UserShablon> _Shablons = ((VirtualModul)MyGlo.Modul).PUB_Shablon;
            // Находим список кодов шаблонов (пример: 101, 110, 120, 0), в конце добавляем ноль, что бы перекрыть последнюю запятую или в пустом списке был просто ноль
            string _NomShablons = ((VirtualModul)MyGlo.Modul).PUB_Protokol.Select(i => i.PROP_NumShablon).Distinct().Aggregate("", (s, i) => s + i + ", ") + "0";
            try
            {
                var _TipProtokol = new MyTipProtokol(pTip);
                SqlDataReader _SqlDataReader = MySql.MET_QuerySqlDataReader(MyQuery.MET_Shablon_Select_4(_TipProtokol.PROP_Prefix, _NomShablons));
                // Перебираем весь поток и дабавляем все шаблоны
                while (_SqlDataReader.Read())
                {
                    UserShablon _Value = new UserShablon();
                    _Value.MET_LoadDataReader(_SqlDataReader);
                    _Value.PROP_TipProtokol = _TipProtokol;
                    _Value.PROP_FlagEdit = false;
                    _Shablons.Add(_Value);
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
