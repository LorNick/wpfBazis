using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Windows;
using Microsoft.Win32;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Fixed;
using Newtonsoft.Json.Linq;

namespace wpfStatic
{
    /// <summary>Команды REST API Pdf сервера</summary>
    public enum eRestPdf
    {
        /// <summary>Сохраняем файл</summary>
        UploadFile,
        /// <summary>Удаляем файл (не используем пока)</summary>
        Delete,
        /// <summary>Загружаем файл</summary>
        Download,
        /// <summary>Наличие файла (не используем пока)</summary>
        GetFileInfo,
        /// <summary>Переименование файла (используем для пометки на удаление)</summary>
        FileRename,
        /// <summary>Отсутствие файла</summary>
        IsNotExistFile
    }

    /// <summary>Откуда загружаем данные, с сервера или локально</summary>
    public enum eServerOrLocal
    {
        /// <summary>Данные загружаем с сервера</summary>
        Server,
        /// <summary>Данные загружаем с локального диска</summary>
        Local
    }

    /// <summary>Доступы к PDF файлам</summary>
    public enum eAccessPdf
    {
        /// <summary>Нет доступа, даже просмотра</summary>
        None,
        /// <summary>Только смотреть</summary>
        View,
        /// <summary>Может создавать, а так же редактировать и удалять свои протоколы</summary>
        Edit,
        /// <summary>Админ</summary>
        Admin
    }

    /// <summary>КЛАСС для Работы с PDF файлами</summary>
    public static class MyPdf
    {
        /// <summary>Доступы к PDF файлам</summary>
        public static eAccessPdf PROP_AccessPdf;

        /// <summary>Строка пути к API сервера PDF</summary>
        private static readonly string PRI_Uri = "http://10.30.103.197:81/api/Storage/";

        /// <summary>Строка аутентификации</summary>
        private static readonly string PRI_Authenticate = "wpfBazisDownloadAndUploadFileWebApi20201014";

        /// <summary>Установить доступ работы с Pdf файлами</summary>
        /// <remarks>Для модулей wpfMVrPolicl и wpfMVrStac ставим доступ Edit
        /// для компов с админским доступом - Admin
        /// для остальных View
        /// так же планируется доступ с помощью accessWpfPdf тега из s_Users поля xInfo
        /// </remarks>
        public static void MET_SetAccessPdf()
        {
            // По умолчанию
            PROP_AccessPdf = eAccessPdf.View;

            // Модули врачей
            if (MyGlo.TypeModul == eModul.VrPolicl || MyGlo.TypeModul == eModul.VrStac)
                PROP_AccessPdf = eAccessPdf.Edit;

            // Находим тег доступа, если есть
            string _xInfo = MyGlo.DataSet.Tables["s_Users"].AsEnumerable()
                .FirstOrDefault(p => p.Field<int>("Cod") == MyGlo.User)?.Field<string>("xInfo");
            if (!string.IsNullOrEmpty(_xInfo))
            {
                JObject _json;
                try
                {
                    _json = JObject.Parse(_xInfo);
                    switch ((string)_json["accessWpfPdf"] ?? "")
                    {
                        case "None":
                            PROP_AccessPdf = eAccessPdf.None;
                            break;
                        case "View":
                            PROP_AccessPdf = eAccessPdf.View;
                            break;
                        case "Edit":
                            PROP_AccessPdf = eAccessPdf.Edit;
                            break;
                    }
                }
                catch
                {
                    // ну не получилось
                }
            }
            // Админские права
            if (MyGlo.PROP_Admin || MyGlo.FlagEdit)
                PROP_AccessPdf = eAccessPdf.Admin;
        }

        /// <summary>Uri в зависимости от типа Rest запроса</summary>
        /// <param name="restPdf">Команда REST API Pdf сервака</param>
        /// <param name="file">Файл, если есть</param>
        /// <returns>Возвращаем uri</returns>
        private static Uri MET_GetUrl(eRestPdf restPdf, string file = "")
        {            
            return new Uri(PRI_Uri + restPdf.ToString() + file);            
        }

        /// <summary>Формируем WebClient</summary>        
        private static WebClient MET_NewWebClient()
        {
            WebClient _WebClient = new WebClient();
            _WebClient.Headers.Add("auth-key", PRI_Authenticate);
            return _WebClient;
        }

        /// <summary>МЕТОД Открываем диалог для выбора PDF файла для дальнейшей загрузки</summary>       
        /// <returns>Полный путь к имени файла</returns>
        public static string MET_OpenFileDialog()
        {
            OpenFileDialog _OpenFileDialog = new OpenFileDialog();
            _OpenFileDialog.Filter = "pdf files (*.pdf)|*.pdf";
            _OpenFileDialog.ShowDialog();
            return _OpenFileDialog.FileName;           
        }

        /// <summary>Находим Хэш файла и на его основе создаем его Хеш имя</summary>
        /// <param name="fullNameFile">Полный путь к имени</param>
        /// <returns>Возвращаем имя хэш файла</returns>
        public static string MET_FileNameHash(string fullNameFile)
        {
            // Алгоритм нахождения MD5         
            FileStream stream = File.OpenRead(fullNameFile);
            MD5 _MD5 = new MD5CryptoServiceProvider();
            byte[] _Hashenc = _MD5.ComputeHash(stream);
            string _hashName = "";
            foreach (var _b in _Hashenc)
            {
                _hashName += _b.ToString("x2");
            }
            return _hashName + ".pdf";
        }

        /// <summary>МЕТОД API Проверяем Отсутствие файла на сервере</summary>
        /// <param name="fullFileName">Имя файла</param>
        /// <param name="serverOrLocal">Полный путь к локальному имени файла (по умолчанию) или хэш имя с сервера</param>
        /// <returns>true - нет такого файла, false - есть такой файл на сервере</returns>
        public static bool MET_isNotExistPdfFileOnServer(string fullFileName, eServerOrLocal serverOrLocal = eServerOrLocal.Local)
        {
            try
            {
                string _file = serverOrLocal == eServerOrLocal.Local ? MET_FileNameHash(fullFileName) : fullFileName;
                string _strRequest = MET_GetUrl(eRestPdf.IsNotExistFile) + "/" + _file;                
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_strRequest);
                request.Headers.Add("auth-key", PRI_Authenticate);              
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                response.Close();
            }
            catch (WebException e)
            {
                // Не могу подключится к серверу
                if (e.Response == null)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
                HttpWebResponse _Response = (HttpWebResponse)e.Response;
                switch (_Response.StatusCode)
                {
                    case (HttpStatusCode)418:
                        MessageBox.Show($"Данный файл уже загружен!", "Ошибка 418");
                        break;
                    default:
                        MessageBox.Show(e.Message);
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }

        /// <summary>МЕТОД API Загрузка PDF файла с сервера или локально</summary>
        /// <param name="fullFileName">Полный путь к имени файла</param>
        /// <param name="pdfViewer">RadPdfViewer куда загружаем данные</param>
        /// <param name="serverOrLocal">Загружаем данные с сервера (по умолчанию) или с локального диска</param>
        public static void MET_LoadPdfFile(string fullFileName, RadPdfViewer pdfViewer, eServerOrLocal serverOrLocal = eServerOrLocal.Server)
        {
            try
            {
                Uri _uri;
                if (serverOrLocal == eServerOrLocal.Server)
                    _uri = MET_GetUrl(eRestPdf.Download, "/" + fullFileName);
                else
                    _uri = new Uri(fullFileName);

                var _clietn = MET_NewWebClient();
                // Событие по завершению загрузки
                _clietn.DownloadDataCompleted += MET_DownloadDataCompleted;
                // Асинхронная загрузка файла
                _clietn.DownloadDataAsync(_uri, pdfViewer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>МЕТОД Завершение скачивание PDF файла и отображем его в просмоторщике</summary>
        private static void MET_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {                
                MemoryStream _Stream = new MemoryStream(e.Result);
                (e.UserState as RadPdfViewer).DocumentSource = new PdfDocumentSource(_Stream);
            }
            catch (Exception)
            {
                MessageBox.Show($"Не смог загрузить PDF файл", "Ошибка загрузки файла");
            }
        }

        /// <summary>МЕТОД API Сохраняем PDF файл на сервер</summary>
        /// <param name="fullFileName">Полный путь к имени файла</param>
        /// <returns>false - удачное сохранение, false - неудачное сохранение</returns>
        public static bool MET_SavePdfFileOnServer(string fullFileName)
        {
            try
            {
                Uri _uri = MET_GetUrl(eRestPdf.UploadFile);
                var _clietn = MET_NewWebClient();
                _clietn.UploadFile(_uri, fullFileName);                
            }
            catch (WebException e)
            {
                // Не могу подключится к серверу
                if (e.Response == null)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
                HttpWebResponse _Response = (HttpWebResponse)e.Response;
                switch (_Response.StatusCode)
                {
                    case HttpStatusCode.Conflict:
                        MessageBox.Show($"Данный файл уже был загружен!", "Ошибка 409");
                        break;
                    default:
                        MessageBox.Show(e.Message);
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            //MessageBox.Show("Файл Загружен!");
            return true;
        }

        /// <summary>МЕТОД API Помечаем "Удаление" протокола, переименовывая PDF файл на сервер</summary>
        /// <param name="FileName">Текущее имя файла</param>
        /// <param name="FileNameDelete">Имя файла после удаления (прибавляем Cod протокола и приставку Del т.е. _123456Del</param>
        /// <returns>false - удачное удаление, false - неудачное удаление</returns>
        public static bool MET_DeleteProtokol(string FileName, string FileNameDelete)
        {
            try
            {
                Uri _uri = MET_GetUrl(eRestPdf.FileRename, "/" + FileName + "/" + FileNameDelete);
                var _clietn = MET_NewWebClient();
                string _d =_clietn.UploadString(_uri, "PUT", "");
            }
            catch (WebException e)
            {
                // Не могу подключится к серверу
                if (e.Response == null)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
                HttpWebResponse _Response = (HttpWebResponse)e.Response;
                MessageBox.Show(e.Message, "Ошибка " + _Response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }
    }
}