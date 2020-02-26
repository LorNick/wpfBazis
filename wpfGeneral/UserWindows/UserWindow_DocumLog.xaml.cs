using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using wpfGeneral.UserControls;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Карточка Логов протоколов документа</summary>
    public partial class UserWindow_DocumLog
    {
        #region ---- СВОЙСТВО ----
        /// <summary>СВОЙСТВО PoleHistory родительский элемент протокола</summary>
        public UserPole_History PROP_PoleHistory { get; set; }

        /// <summary>СВОЙСТВО Список логов</summary>
        public List<UserLog> PROP_Logs { get; set; }
        #endregion	

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_DocumLog(UserPole_History pPoleHistory)
        {
            InitializeComponent();
            
            PROP_PoleHistory = pPoleHistory;
            WindowStyle = WindowStyle.ToolWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        /// <summary>СОБЫТИЕ После загрузки окна</summary>
        /// <remarks>Работа с json https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm </remarks>
        private void UserWindows_Loaded(object sender, RoutedEventArgs e)
        {
            PROP_Logs = new List<UserLog>();
           
            // Если протокола нет, то создаем
            if (PROP_PoleHistory.PROP_DocumHistory.PROP_Protokol == null)
                PROP_PoleHistory.PROP_DocumHistory.PROP_Protokol = UserProtokol.MET_FactoryProtokol(PROP_PoleHistory.PROP_Type, (int)PROP_PoleHistory.PROP_Cod);
            
            string _jLog = PROP_PoleHistory.PROP_DocumHistory.PROP_Protokol.PROP_xLog;

            // Если есть логи
            if (!string.IsNullOrEmpty(_jLog))
            {
                JObject _Json = JObject.Parse(_jLog);
                foreach (var i in _Json["Log"].Children())
                {
                    var _Log = new UserLog();
                    _Log.Cod = (int) i["Cod"];
                    _Log.Date = (string) i["Date"];
                    _Log.Tip = (string)i["Tip"];
                    _Log.User = (int)i["User"];
                    _Log.UserName = MyMet.MET_UserName((int)i["User"]);
                    _Log.Ver = (string)i["Ver"];

                    PROP_Logs.Add(_Log);
                }
            }
            PART_ListView.ItemsSource = PROP_Logs;}
    }

    /// <summary>КЛАСС Логи протоколов</summary>
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
    }
}
