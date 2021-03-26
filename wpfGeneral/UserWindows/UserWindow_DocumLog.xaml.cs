using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json.Linq;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Карточка Логов протоколов документа</summary>
    public partial class UserWindow_DocumLog
    {
        #region ---- СВОЙСТВО ----
        /// <summary>Строка логов</summary>
        private string PRI_jLog { get; set; }

        /// <summary>СВОЙСТВО Список логов</summary>
        public List<UserLog> PROP_Logs { get; set; }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pLog">Текст лога</param>
        public UserWindow_DocumLog(string pLog)
        {
            InitializeComponent();

            PRI_jLog = pLog;
            WindowStyle = WindowStyle.ToolWindow;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        /// <summary>СОБЫТИЕ После загрузки окна</summary>
        /// <remarks>Работа с json https://www.newtonsoft.com/json/help/html/QueryingLINQtoJSON.htm </remarks>
        private void UserWindows_Loaded(object sender, RoutedEventArgs e)
        {
            PROP_Logs = new List<UserLog>();
            // Если есть логи
            if (!string.IsNullOrEmpty(PRI_jLog))
            {
                JObject _Json = JObject.Parse(PRI_jLog);
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
            PART_ListView.ItemsSource = PROP_Logs;
        }
    }
}
