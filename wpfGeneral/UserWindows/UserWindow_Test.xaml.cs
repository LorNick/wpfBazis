using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using wpfGeneral.UserControls;
using wpfGeneral.UserModul;
using System.Windows.Threading;
using wpfStatic;
using WebSocket4Net;
using SuperSocket.ClientEngine;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Виртуальной формы справочников</summary>
    public partial class UserWindow_Test 
    {           
        //#region ---- СВОЙСТВО ----
        /////// <summary>СВОЙСТВО Дата создания DateUp</summary>
        ////public DateTime? PROP_DateUp
        ////{
        ////    get { return PART_DateUp.PROP_Date; }
        ////    set { PART_DateUp.PROP_Date = value; }
        ////}
        //#endregion	

		/// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Test()
        {
            InitializeComponent();
        }              

        string ip = "ws://localhost:33999";
        /// <summary>WebSocket</summary>
        public WebSocket websocket;

        private void UserWindows_Loaded(object sender, RoutedEventArgs e)
        {
            //PART_JsonForm.MET_Inizial();

            //bool _Ver = MyMet.MET_GetVersionNet45();
            // MessageBox.Show(_Ver.ToString(), "версия");
            //  Class1.MET_Access();
            websocket = new WebSocket(ip);
            websocket.Opened += new EventHandler(websocket_Opened);
            websocket.Error += new EventHandler<ErrorEventArgs>(websocket_Error);
            websocket.Closed += new EventHandler(websocket_Closed);
            websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
            websocket.DataReceived += new EventHandler<DataReceivedEventArgs>(websocket_MessageReceived);

            websocket.Open();
        }

        private void websocket_Opened(object sender, EventArgs e)
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

            websocket.Send(_M);        
        }

        private void websocket_MessageReceived(object sender, EventArgs e)
        {
            var d = (MessageReceivedEventArgs)e;
             MET_LogAdd("Ответ", d.Message);
        }

        private void websocket_Closed(object sender, EventArgs e)
        {
            MET_LogAdd("Закрытие", e.ToString());
        }

        private void websocket_Error(object sender, ErrorEventArgs e)
        {           
            MET_LogAdd("Ошибка", e.ToString());
        }

        /// <summary>МЕТОД Проверяем доступность данного окна текущему пользователю</summary>        
        public static bool MET_Access()
        {
            if (!MyGlo.Admin)
            {
                MessageBox.Show("У вас нет доступа.");
                return false;
            }
            return true;
        }

        private void PART_Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            string _M = @"{
                  ""MessageType"": 7,
                }
                ";

            websocket.Send(_M);
            websocket.Close();
        }

        /// <summary>МЕТОД Добавляем сообщение Лога</summary>
        /// <param name="pType">Тип сообщения</param>
        /// <param name="pText">Текст сообщения</param>
        private void MET_LogAdd(string pType, string pText)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate ()
            {
                PART_Log.Text = $"{DateTime.Now.ToLongTimeString()} {pType}:  {pText}\n{PART_Log.Text}";

                if ((pText.StartsWith("{") && pText.EndsWith("}")) || (pText.StartsWith("[") && pText.EndsWith("]")))
                {
                    try
                    {
                        JObject _Json = JObject.Parse(pText);

                        string _Text = "";

                        switch ((int)_Json["MessageType"])
                        {
                            case 16:
                                _Text = (string)_Json["Data"];
                                if (_Text == "¶")
                                    _Text = "\n";
                                if (PART_Text.PROP_Text.Length > 0)
                                    PART_Text.PROP_Text += " ";
                                PART_Text.PROP_Text += _Text;
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
