using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для поля Картинок (Image)</summary>
    public sealed partial class UserPole_Image : VirtualPole
    {
        private readonly int PRI_Height;
        private readonly int PRI_Width;
        private readonly double PRI_Opacity;
        private string PRI_Text;

        #region ---- СВОЙСТВО ----
        /// <summary>СВОЙСТВО Описание вопроса</summary>
        public override string PROP_Description
        {
            get { return (string)PART_Label.Content; }
            set
            {
                PART_Label.Content = value;
                // Если описания нету, то убираем пустой отступ
                if (value == "")
                    PART_Label.Padding = new Thickness(0);
                else
                    PART_Label.Padding = new Thickness(5, 0, 5 ,0);
            }
        }

        /// <summary>СВОЙСТВО Ответ</summary>
        public override string PROP_Text
        {
            get { return PRI_Text; }
            set
            {
                if (PRI_Text == null)
                {
                    PRI_Text = value;
                    MET_StartPole();
                }
                else
                    PRI_Text = value;
            }
        }
        #endregion

        /// <summary>КОНСТРУКТОР для формы</summary>
        public UserPole_Image()
        {
            InitializeComponent();

            // Загружаем картинки в DataSet
            PRI_Height = 100;
            PRI_Width = 140;
            PRI_Opacity = 1;
            PART_Border.Background = Brushes.White;
        }

        /// <summary>КОНСТРУКТОР для отчета</summary>
        public UserPole_Image(string pText, int pVarID, int Shablon, int pZoom, MyFormat pFormat)
        {
            InitializeComponent();
            PROP_VarId = pVarID;
            PROP_Shablon = Shablon;
            MET_LoadDataSet();
            PART_Label.Visibility = Visibility.Collapsed;
            PROP_Format = pFormat;
            if (PROP_Format.MET_If("xH"))
                PRI_Height = (int)(MyMet.MET_ParseInt(PROP_Format.PROP_Value["xH"]) * 3.4);
            if (PROP_Format.MET_If("xW"))
                PRI_Width = (int)(MyMet.MET_ParseInt(PROP_Format.PROP_Value["xW"]) * 3.4);
            PRI_Height = PRI_Height == 0 ? 230 * pZoom : PRI_Height;
            PRI_Width = PRI_Width == 0 ? 320 * pZoom : PRI_Width;
            PRI_Opacity = 0.6;
            PROP_Text = pText;
        }

        /// <summary>МЕТОД Загружаем картинки в DataSet</summary>
        private void MET_LoadDataSet()
        {
            // Строка запроса
            string _StrSQL = MyQuery.s_ListImage_Select_1(PROP_Shablon, PROP_VarId);
            // Заполняем DataSet
            MySql.MET_DsAdapterFill(_StrSQL, "s_ListImage");
            MyGlo.DataSet.Tables["s_ListImage"].PrimaryKey = new DataColumn[] { MyGlo.DataSet.Tables["s_ListImage"].Columns["Cod"] };
        }

        /// <summary>МЕТОД Загружаем картинку по коду</summary>
        /// <param name="pCod">Код картинки</param>
        /// <returns>Возвращаем полученную карту</returns>
        private BitmapImage MET_LoadImag(int pCod)
        {
            // Карта
            BitmapImage _BitmapImage = new BitmapImage();
            // Искомая строка с картой
            DataRow _DataRow;
            try
            {
                _DataRow = MyGlo.DataSet.Tables["s_ListImage"].Rows.Find(pCod);
            }
            catch
            {
                MET_LoadDataSet();
                _DataRow = MyGlo.DataSet.Tables["s_ListImage"].Rows.Find(pCod);
            }
            if (_DataRow != null)
            {
                // Загружаем в поток рисунок
                MemoryStream _Stream = new MemoryStream((byte[])_DataRow["Image"]);
                // Загружаем в карту рисунка байтовый поток рисунка
                _BitmapImage.BeginInit();
                _BitmapImage.StreamSource = _Stream;
                _BitmapImage.EndInit();
            }
            return _BitmapImage;
        }

        /// <summary>МЕТОД Показываем все картинки</summary>
        private void MET_StartPole()
        {
            if (PRI_Text.Length > 0)
            {
                // Скрываем надпись - подсказку
                PART_Change.Visibility = Visibility.Collapsed;
                foreach (string s in PRI_Text.Split(new[] { ',' }))
                {
                    Image _NewImage = new Image();
                    _NewImage.Height = PRI_Height;
                    _NewImage.Width = PRI_Width;
                    _NewImage.Opacity = PRI_Opacity;
                    _NewImage.Source = MET_LoadImag(Convert.ToInt32(s));
                    PART_WrapPanel.Children.Add(_NewImage);
                }
            }
        }

        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            // Загружаем картинки в DataSet
            MET_LoadDataSet();
        }

        /// <summary>СОБЫТИЕ При открытии контекстного меню</summary>
        private void PART_ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            // Если уже заполненно, то выходим (иначе заполняем)
            if (PART_ContextMenu.Items.Count > 0) return;
            // Создаем пункт меню "Удалить все"
            MenuItem _MenuItem = new MenuItem();
            _MenuItem.Header = "Удалить все";
            _MenuItem.Click += PART_MenuItem_Click;
            PART_ContextMenu.Items.Add(_MenuItem);
            // Разделитель
            Separator _Separator = new Separator();
            PART_ContextMenu.Items.Add(_Separator);
            // Всего рисунков
            int _Count = MyGlo.DataSet.Tables["s_ListImage"].Rows.Count;
            // Массив строк
            DataRow[] _mDataRow = new DataRow[_Count];
            // Заполняем все варианты ответов
            for (int i = 0; i < _Count; i++)
            {
                // Строка с данными
                _mDataRow[i] = MyGlo.DataSet.Tables["s_ListImage"].Rows[i];
                Image _Image = new Image();
                _Image.Height = 100;
                _Image.Width = 140;
                _Image.Source = MET_LoadImag((int)_mDataRow[i]["Cod"]);
                // Код рисунка
                _Image.Tag = _mDataRow[i]["Cod"].ToString();
                _Image.MouseDown += PART_Image_Click;
                PART_ContextMenu.Items.Add(_Image);
            }
        }

        /// <summary>СОБЫТИЕ Вставляем выбранное значение из контектсного меню</summary>
        private void PART_Image_Click(object sender, EventArgs e)
        {
            // Скрываем надпись - подсказку
            PART_Change.Visibility = Visibility.Collapsed;
            // Загружаем рисунок в шаблон
            Image _Image = sender as Image;
            Image _NewImage = new Image();
            _NewImage.Height = _Image.Height;
            _NewImage.Width = _Image.Width;
            _NewImage.Tag = _Image.Tag;
            _NewImage.Source = (sender as Image).Source;
            PART_WrapPanel.Children.Add(_NewImage);
            // Если уже есть в ответе рисунки, то добавляем перед ними запятую
            if (PROP_Text.Length > 0)
                PROP_Text += ",";
            // Добавляем код рисунка в строку текста
            PROP_Text += _NewImage.Tag.ToString();
            // Активируем кнопку "Сохранить"
            MyGlo.callbackEvent_sEditShablon(true);
        }

        /// <summary>СОБЫТИЕ Удаляем все рисунки</summary>
        private void PART_MenuItem_Click(object sender, EventArgs e)
        {
            // Удаляем все рисунки
            PART_WrapPanel.Children.Clear();
            // Возвращаем и показываем подсказку
            PART_WrapPanel.Children.Add(PART_Change);
            PART_Change.Visibility = Visibility.Visible;
            // Отчищаем строку ответа
            PROP_Text = "";
            // Активируем кнопку "Сохранить"
            MyGlo.callbackEvent_sEditShablon(true);
        }
    }
}
