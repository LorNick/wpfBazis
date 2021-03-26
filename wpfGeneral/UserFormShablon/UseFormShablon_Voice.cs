using System;
using wpfGeneral.UserStruct;
using WebSocket4Net;
using SuperSocket.ClientEngine;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Windows.Threading;
using wpfGeneral.UserControls;

namespace wpfGeneral.UserFormShablon
{
    /// <summary>КЛАСС работы шаблона с помощью микрофона и программы Voice2Med</summary>
    class UseFormShablon_Voice
    {
        #region ---- Свойства ----
        /// <summary>СВОЙСТВО Сылка на документ</summary>
        public UserDocument PROP_Docum { get; set; }
        #endregion

        private const string _ip = "ws://localhost:33999";
        /// <summary>Соединение с Vice2Med</summary>
        private WebSocket PRI_WebSocket;

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pDocument">Документ</param>
        public UseFormShablon_Voice(UserDocument pDocument)
        {
            PROP_Docum = pDocument;
            if (!PROP_Docum.PROP_ListShablon.PROP_MyFormat.MET_If("Voice"))
                return;

            PRI_WebSocket = new WebSocket(_ip);
            PRI_WebSocket.Opened += new EventHandler(MET_WebSocket_Opened);
            PRI_WebSocket.Error += new EventHandler<ErrorEventArgs>(MET_WebSocket_Error);
            PRI_WebSocket.Closed += new EventHandler(MET_WebSockett_Closed);
            PRI_WebSocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(MET_WebSocket_MessageReceived);
            PRI_WebSocket.DataReceived += new EventHandler<DataReceivedEventArgs>(MET_WebSocket_MessageReceived);

            PRI_WebSocket.Open();
        }

        /// <summary>МЕТОД Открываем соединение</summary>
        private void MET_WebSocket_Opened(object sender, EventArgs e)
        {
            string _M = @"{
                  ""MessageType"": 6,
                  ""Data"": {
                                ""Key"": ""a0bc1744-010c-4414-84e6-acabeff7b517"",
                    ""Name"": ""Форма №1"",
                    ""Childs"": [
                      {
                        ""DateTimeFormat"": ""dd.MM.yyyy"",
                        ""Type"": 4,
                        ""Synonyms"": [
                          ""Плановая дата следующего осмотра"",
                          ""Cледующий осмотр"",
                          ""Дата осмотра"",
                          ""Дата следующего осмотра""
                        ],
                        ""Key"": ""53""
                      }
                    ],
                    ""CultureCode"": ""ru-RU""
                  }
                }
                ";

            PRI_WebSocket.Send(_M);
        }

        /// <summary>МЕТОД Получаем ответ</summary>
        private void MET_WebSocket_MessageReceived(object sender, EventArgs e)
        {
            var d = (MessageReceivedEventArgs)e;
            MET_LogAdd("Ответ", d.Message);
        }

        /// <summary>МЕТОД Закрываем соединение</summary>
        private void MET_WebSockett_Closed(object sender, EventArgs e)
        {
            MET_LogAdd("Закрытие", e.ToString());
        }

        /// <summary>МЕТОД Вернули ошибку</summary>
        private void MET_WebSocket_Error(object sender, ErrorEventArgs e)
        {
            MET_LogAdd("Ошибка", e.ToString());
        }

        //private void PART_Button_Delete_Click(object sender, RoutedEventArgs e)
        //{
        //    string _M = @"{
        //          ""MessageType"": 7,
        //        }
        //        ";

        //    websocket.Send(_M);
        //    websocket.Close();
        //}

        /// <summary>МЕТОД Добавляем сообщение Лога</summary>
        /// <param name="pType">Тип сообщения</param>
        /// <param name="pText">Текст сообщения</param>
        private void MET_LogAdd(string pType, string pText)
        {
            //(Action)(() =>    //
            PROP_Docum.PROP_FormShablon.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                //MessageBox.Show(pText);
                if ((pText.StartsWith("{") && pText.EndsWith("}")) || (pText.StartsWith("[") && pText.EndsWith("]")))
                {
                    try
                    {
                        JObject _Json = JObject.Parse(pText);

                        string _Text = "";
                        VirtualPole _Pole;

                        _Pole =  PROP_Docum.PROP_FormShablon.GetFocusedPole();
                        if (_Pole == null)
                            return;

                        switch ((int)_Json["MessageType"])
                        {
                            case 16:
                                _Text = (string)_Json["Data"];
                                if (_Text == "¶")
                                    _Text = "\n";
                                if (_Pole.PROP_Text.Length > 0)
                                    _Pole.PROP_Text += " ";
                                _Pole.PROP_Text += _Text;
                                break;
                        }
                    }
                    catch
                    {
                        //
                    }
                }
        });
        }
    }
}

