using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using wpfGeneral.UserWindows;
using Newtonsoft.Json.Linq;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для поля с выбором из справочника (MultyList)</summary>
    public partial class UserPole_MultyList
    {
        #region ---- ЗАКРЫТЫЕ ПОЛЯ ----
        /// <summary>Создаем контекстное меню</summary>
        private readonly ContextMenu PRI_ContextMenu = new ContextMenu();
        /// <summary>Коды операций (максимум 11 операций)</summary>
        private readonly string[] PRI_mCodOper = new string[10];
        /// <summary>Дата операции</summary>
        private DateTime? PRI_Date;
        /// <summary>Количество операций</summary>
        private int PRI_Count;
        /// <summary>Добавляли ли операции</summary>
        private bool PRI_Insert;
        /// <summary>Если false - то не добавляем операции в таблицу операций (т.е. есть старые операции, в этом стационаре, не привязанные к протоколам)</summary>
        private bool PRI_FlagNew = true;
        #endregion

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

        /// <summary>СВОЙСТВО Цвет текста ответа</summary>
        public override Brush PROP_ForegroundText
        {
            get
            {
                return PART_TextBox?.Foreground;
            }
            set
            {
                if (PART_TextBox != null)
                    PART_TextBox.Foreground = value;
            }
        }

        /// <summary>СВОЙСТВО Количество символов в ответе</summary>
        public override int PROP_MaxLength
        {
            get
            {
                return PART_TextBox?.MaxLength ?? 0;
            }
            set
            {
                if (PART_TextBox != null)
                    PART_TextBox.MaxLength = value;
            }
        }

        /// <summary>СВОЙСТВО Ввод заглавных букв</summary>
        public override CharacterCasing PROP_CharacterCasing
        {
            get { return PART_TextBox.CharacterCasing ; }
            set { PART_TextBox.CharacterCasing = value; }
        }

        /// <summary>СВОЙСТВО Ширина текста</summary>
        public override Double PROP_WidthText
        {
            get { return PART_TextBox.Width; }
            set
            {
                PART_TextBox.Width = value;
                if (double.IsNaN(PART_TextBox.Width))
                    PART_TextBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                else
                    PART_TextBox.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }

        /// <summary>СВОЙСТВО Минимальная Ширина описания</summary>
        public override Double PROP_MinWidthDescription
        {
            get { return PART_Label.MinWidth; }
            set { PART_Label.MinWidth = value; }
        }

        /// <summary>СВОЙСТВО Помечаем поле текста</summary>
        public override byte PROP_PometkaText
        {
            get { return PRO_PometkaText; }
            set
            {

                PRO_PometkaText = value;
                if (PRO_PometkaText == 1)
                    PART_TextBox.BorderBrush = Brushes.Red;
                else
                    PART_TextBox.ClearValue(Border.BorderBrushProperty);
            }
        }

        /// <summary>СВОЙСТВО Теги (в json)</summary>
        public string PROP_xInfo { get; set; }

        /// <summary>СВОЙСТВО Объект json</summary>
        public JObject PROP_Json { get; set; }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_MultyList()
        {
            InitializeComponent();

            // Инициализация конекстного меню
            PART_TextBox.ContextMenu = PRI_ContextMenu;
            PRI_ContextMenu.Opened += MET_ContextMenu_Opened;
            PART_TextBox.Tag = this;
        }

        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            // Помечаем поле, как обязательное
            PROP_PometkaText = 1;
            // Если есть операции, заполенные не через протокол, то коды в таблицу операций не добавляем
            if (MySql.MET_QueryBool(MyQuery.Oper_Select_1(MyGlo.IND)))
            {
                PRI_FlagNew = false;
                return;
            }
            // Дата операции
            PRI_Date = PROP_FormShablon.PUB_VirtualNodes.PROP_Data;
            // Если старый протокол
            if (!PROP_FormShablon.PROP_Now)
            {
                // Если старый протокол, то находим заполенные операции
                MySql.MET_DsAdapterFill(MyQuery.Oper_Select_2(PROP_FormShablon.PROP_Cod), "Oper");
                // Перебераем все операции и запоминаем их
                foreach (DataRow _DataRow in MyGlo.DataSet.Tables["Oper"].Rows)
                {
                    PRI_mCodOper[PRI_Count] = _DataRow["OPER"].ToString();
                    PRI_Count++;
                }
                PROP_xInfo = MyGlo.DataSet.Tables["xInfo"]?.Rows[0].ToString();
                PROP_Json = JObject.Parse(PROP_xInfo ?? "{ }");
            }
            else
            {
                PROP_Text = "";
                PROP_Json = JObject.Parse("{ }");

                // Новый протокол
                try
                {
                    // Если установлен код услуги/операции по умолчанию
                    if (PROP_DefaultText.Length == 10 || PROP_DefaultText.Length == 14)
                    {
                        // Пытаемся найти её в справочнике услуг s_VidOper
                        MySql.MET_DsAdapterFill(MyQuery.s_VidOper_Select_1($" and KOP = '{PROP_DefaultText}'"), "Oper");
                        if (MyGlo.DataSet.Tables["Oper"].Rows.Count > 0)
                        {
                            DataRow _DataRow = MyGlo.DataSet.Tables["Oper"].Rows[0];
                            // Текст операции
                            PROP_Text = _DataRow["KOP"] + " - " + _DataRow["NAME"];
                            // Код операции
                            PRI_mCodOper[PRI_Count] = _DataRow["KOP"].ToString();
                            // Количество операций
                            PRI_Count++;
                            // Флаг новой операции
                            PRI_Insert = true;
                        }
                    }
                }
                catch
                {
                    // ignore
                }
            }
        }

        /// <summary>СОБЫТИЕ при вводе символа в TextBox</summary>
        private void PART_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Активируем кнопку "Сохранить"
            MyGlo.callbackEvent_sEditShablon?.Invoke(true);
            // Перекрашиваем шрифт в черный, если был серый
            if (Equals(PROP_ForegroundText, Brushes.Gray))
                PROP_ForegroundText = Brushes.Black;
            TextBox _TextBox = (TextBox)sender;
            this.SetValue(DEPR_TextProperty, _TextBox.Text);
        }

        /// <summary>СОБЫТИЕ при открытии контекстного меню</summary>
        private void MET_ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu _ConMen = (ContextMenu)sender;                          // контекстное меню
            _ConMen.FontSize = 13;
            _ConMen.Items.Clear();                                              // чистим список контекстного меню
            // Создаем пункт меню "Добавить"
            MenuItem _MenuItem = new MenuItem();
            _MenuItem.Header = "Выбрать КОД из справочника";
            _MenuItem.Click += MET_MenuItem_Click;
            _ConMen.Items.Add(_MenuItem);
            // Создаем пункт меню "Изменить"
            _MenuItem = new MenuItem();
            _MenuItem.Header = "Удалить операции";
            _MenuItem.Click += MET_MenuItem_Click;
            _ConMen.Items.Add(_MenuItem);
            // Разделитель
            Separator _Separator = new Separator();
            _ConMen.Items.Add(_Separator);
            // Создаем пункт меню "Копировать"
            _MenuItem = new MenuItem();
            _MenuItem.Command = ApplicationCommands.Copy;
            _ConMen.Items.Add(_MenuItem);
        }

        /// <summary>СОБЫТИЕ Вставляем выбранное значение из контектсного меню</summary>
        private void MET_MenuItem_Click(object sender, EventArgs e)
        {
            // Выбранный пункт меню
            string _Text = (sender as MenuItem)?.Header.ToString();
            switch (_Text)
            {
                case "Выбрать КОД из справочника":
                    // Находим дату создания протокола из первого поля шаблона с pDate
                    PRI_Date = DateTime.Parse(PROP_FormShablon.GetPole("DateOsmotr").PROP_Text);
                    // Справочник Операций
                    UserWindow_Oper _WinSpr = new UserWindow_Oper((DateTime)PRI_Date, PROP_Shablon)
                    {
                        WindowStyle = WindowStyle.ToolWindow,
                        PROP_Modal = true,
                        // Разрешаем выбирать записи
                        PROP_FlagButtonSelect = true
                    };
                    _WinSpr.ShowDialog();
                    if (_WinSpr.PROP_Return)
                    {
                        // Текст
                        if (PROP_Text.Length > 0)
                            PROP_Text += "\n" +_WinSpr.PROP_Cod + " - " + _WinSpr.PROP_Text;
                        else
                            PROP_Text = _WinSpr.PROP_Cod + " - " + _WinSpr.PROP_Text;
                        // Код операции
                        PRI_mCodOper[PRI_Count] = _WinSpr.PROP_Cod;
                        // Количество операций
                        PRI_Count++;
                        // Флаг новой операции
                        PRI_Insert = true;
                    }
                    break;
                case "Удалить операции":
                    if (MessageBox.Show("Вы точно хотите удалить все операции?", "Внимание!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        // Чистим текст
                        PROP_Text = "";
                        // Обнуляем количество операций
                        PRI_Count = 0;
                    }
                    break;
            }
        }

        ///<summary>МЕТОД Действие при сохранении</summary>
        public override bool MET_Save()
        {
            // Если операции добавляли или сменили дату операции или меняем данные xInfo
            if ((PRI_Insert | PROP_FormShablon.PUB_VirtualNodes.PROP_Data != PRI_Date | PROP_xInfo != PROP_Json?.ToString()) & PRI_FlagNew)
            {
                // Удаляем старые кода операций, если они не были синхронизированны
                MySql.MET_QueryNo(MyQuery.Oper_Delete_1(PROP_FormShablon.PROP_Cod));
                // Помечаем для удаления старые кода, которые были синхронизированы
                MySql.MET_QueryNo(MyQuery.Oper_Update_1(PROP_FormShablon.PROP_Cod));
                // Создаем строку тегов
                PROP_xInfo = PROP_Json?.ToString();
                // Добавляем новые кода операций
                for (int i = 0; i < PRI_Count; i++)
                {
                    MySql.MET_QueryNo(MyQuery.Oper_Insert_1(MySql.MET_GetNextRef(5), MyGlo.KL, MyGlo.IND, PROP_FormShablon.PUB_VirtualNodes.PROP_Data,
                                                           MyGlo.Otd, PRI_mCodOper[i], PROP_FormShablon.PROP_Cod, PROP_xInfo));
                }
                PRI_Insert = false;
            }
            return true;
        }

        ///<summary>МЕТОД Меняем данные xInfo</summary>
        /// <param name="pTag">Имя тега</param>
        /// <param name="pValue">Значение</param>
        public bool MET_ChangeInfo(string pTag, dynamic pValue)
        {
            // Находим старое значение тега, если этот тег есть (не null)
            dynamic _Json = (string) PROP_Json[pTag] == null ? "" : PROP_Json[pTag].ToObject<dynamic>();
            // Преобразовываем тип int в long, так как Json, не использует int, а сразу long
            Type _Type = pValue.GetType().Name == "Int32" ? typeof (long) : pValue.GetType();
            // Если у существующего тега разные типы или разные значения, то записываем изменения
            if (_Json.GetType() != _Type || _Json != pValue)
            {
                PROP_Json[pTag] = pValue; // меняем значение или создаем новый тег с этим значением
                PRI_Insert = true; // пометка на изменение
                return true;
            }
            return false;
        }

        ///<summary>МЕТОД Удаляем тег из xInfo</summary>
        /// <param name="pTag">Имя тега</param>
        /// <returns>Возвращаем успех или не успех удаления тега</returns>
        public bool MET_DeleteInfo(string pTag)
        {
            return PROP_Json.Remove(pTag);
        }

        ///<summary>МЕТОД Проверка на допустимость данных и полноте заполнения</summary>
        public override bool MET_Verification()
        {
            if (PROP_Text.Length == 0)
            {
                MessageBox.Show("Не заполнено поле Код операции/лечения!", "Внимание!");
                this.Focus();
                return false;
            }
            return true;
        }
    }
}
