using System;
using System.Collections;
using System.Windows;
using wpfGeneral.UserNodes;
using System.Net;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;

namespace wpfGeneral.UserPage
{
    /// <summary>КЛАСС Страница Листа назначений</summary>
    public sealed partial class UserPage_ShablonPDF
    {
        /// <summary>Список полей</summary>
        public SortedList PUB_HashPole;
        /// <summary>Ветка</summary>
        public VirtualNodes PUB_Node;

        /// <summary>Веб клиент</summary>
        private WebClient PRI_WebClient;

        /// <summary>КОНСТРУКТОР</summary>
        public UserPage_ShablonPDF()
        {
            InitializeComponent();
            MET_SavePdfFileOnServer();
        }

        /// <summary>СОБЫТИЕ После загрузки окна</summary>
        private void UserPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>МЕТОД Сохраняем PDF файл на сервер</summary>
        private bool MET_SavePdfFileOnServer()
        {
            OpenFileDialog _OpenFileDialog = new OpenFileDialog();
            _OpenFileDialog.Filter = "pdf files (*.pdf)|*.pdf";
            _OpenFileDialog.ShowDialog();
            if (string.IsNullOrWhiteSpace(_OpenFileDialog.FileName))
                return false;
            // Алгоритм нахождения MD5
            // Просто отображает Хэш на экране
            FileStream stream = File.OpenRead(_OpenFileDialog.FileName);
            MD5 _MD5 = new MD5CryptoServiceProvider();
            byte[] _Hashenc = _MD5.ComputeHash(stream);
            string _Result = "";
            foreach (var _b in _Hashenc)
            {
                _Result += _b.ToString("x2");
            }
            PRI_WebClient = new WebClient();
            // Завершение загрузки
            PRI_WebClient.UploadFileCompleted += new UploadFileCompletedEventHandler(MET_Completed);
            // Прогресс загрузки
            PRI_WebClient.UploadProgressChanged += new UploadProgressChangedEventHandler(MET_ProgressChanged);
            // URI сервера и api
            var uri = new Uri("http://192.168.0.6:81/api/Storage/UploadFile");
            try
            {
                // Заголовок с токеном для аутентификации в функции загрузки
                PRI_WebClient.Headers.Add("auth-key", "wpfBazisDownloadAndUploadFileWebApi20201014");
                // Асинхронная загрузка файла
                PRI_WebClient.UploadFileAsync(uri, _OpenFileDialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        /// <summary>МЕТОД Завершение сохранения PDF файла на сервер</summary>
        private void MET_Completed(object sender, UploadFileCompletedEventArgs e)
        {
            if (e?.Error is WebException)
            {
                var _Response = (HttpWebResponse)((WebException)e.Error).Response;
                switch (_Response.StatusCode)
                {
                    case HttpStatusCode.Conflict:
                        MessageBox.Show($"Данный файл уже был загружен!", "Ошибка 409");
                        break;
                }
                return;
            }
            MessageBox.Show("Файл Загружен!");
        }

        /// <summary>МЕТОД Прогесс сохранения PDF файла на сервер</summary>
        private void MET_ProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            //  this.progressBar.Value = e.ProgressPercentage;
        }

    }

}
