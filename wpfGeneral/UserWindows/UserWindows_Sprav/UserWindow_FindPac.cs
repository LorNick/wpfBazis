using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using wpfGeneral.UserControls;
using wpfStatic;

namespace wpfGeneral.UserWindows
{

    /// <summary>КЛАСС Окно поиска пациента по Kbol, Apac, Apastac (В РАБОТЕ!!!)</summary>
    public class UserWindow_FindPac: VirtualUserWindow
    {
        /// <summary>Начальная дата</summary>
        private DatePicker PRI_DatePicker_1;
        /// <summary>Конечная дата</summary>
        private DatePicker PRI_DatePicker_2;             
        /// <summary>Таймер для задержки перед запросом</summary>
        private DispatcherTimer PRI_Timer;
        /// <summary>Тип базы</summary>
        private UserPole_RadioButton PRI_TipRadioButton;

        private string PRI_FIO;
        private string PRI_Cod;
        private string PRI_Tip;
        private string PRI_D1 = "01/01/2018";
        private string PRI_D2 = "01/01/2018";


        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_FindPac()
        {            
            // Имя таблицы
            PRO_TableName = "FindPac";
            // Если строка ввода ищет через SQL
            PRO_FlagTextSql = true;
            // Заголовок
            Title = "Поиск пациента:";
            // Размеры окна
            Width = 1080;
            MinWidth = Width;
            Height = 700;
            // Подсказка в строке поиска
            PART_TextBox.WatermarkContent = "Введите ФИО и год рождения (например: ИВАНО И НИ 28) или KL, или код посещения, или номер страхового полиса";
            // Сортируем по Фамилии
            PRO_PoleSort = 1;
            // Разрешаем выбирать записи
            PROP_FlagButtonSelect = true;
            // Создаем фильтр
            MET_CreateFiltr();            
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }       

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.MET_varFindPac_Select_1(PRI_FIO, PRI_Cod, PRI_Tip, PRI_D1, PRI_D2);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "KL", "ФИО пациента", "Дата р.", "Поступил", "Выписан", "Диаг.", "Врач", "IND", "От." };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[0].Width = 130;     // KL
            PART_DataGrid.Columns[1].Width = 250;     // ФИО пациента
            PART_DataGrid.Columns[2].Width = 85;      // Дата р.            
            PART_DataGrid.Columns[3].Width = 85;      // Поступил
            PART_DataGrid.Columns[4].Width = 85;      // Выписан
            PART_DataGrid.Columns[6].Width = 130;     // Врач
            PART_DataGrid.Columns[7].Width = 130;     // IND
        }

        /// <summary>МЕТОД Создание фильтров</summary>
        protected void MET_CreateFiltr()
        {
            PART_Grid.RowDefinitions[0].Height = new GridLength(80, GridUnitType.Auto);
            PART_Expander.Visibility = Visibility.Visible;
            PART_Expander.IsExpanded = true;
            Border _Border = new Border { Style = (Style)FindResource("Border_2") };
            PART_Expander.Content = _Border;
            StackPanel _SPanel = new StackPanel();
            _Border.Child = _SPanel;

            // ---- Настраиваем 1й фильтр
            StackPanel _SPanel_1 = new StackPanel();
            _SPanel_1.Orientation = Orientation.Horizontal;
            _SPanel_1.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_1);
            // Тип документа
            PRI_TipRadioButton = new UserPole_RadioButton();
            PRI_TipRadioButton.PROP_Description = "Где ищем:";
            PRI_TipRadioButton.PROP_Items.Add(new ListBoxItem() { Content = "Пациент" });
            PRI_TipRadioButton.PROP_Items.Add(new ListBoxItem() { Content = "Поликлиника" });
            PRI_TipRadioButton.PROP_Items.Add(new ListBoxItem() { Content = "Стационар" });
            PRI_TipRadioButton.PROP_Items.Add(new ListBoxItem() { Content = "Параклиника" });
            PRI_TipRadioButton.ItemsChanged += delegate { PRI_Timer.Start(); };
            _SPanel_1.Children.Add(PRI_TipRadioButton);

            // ---- Настраиваем 2й фильтр
            StackPanel _SPanel_2 = new StackPanel();
            _SPanel_2.Orientation = Orientation.Horizontal;
            _SPanel_2.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_2);
            Label _Label_1 = new Label();
            _Label_1.Content = "Посещал с :";
            _Label_1.Foreground = Brushes.Navy;
            _SPanel_2.Children.Add(_Label_1);
            // Дата с
            PRI_DatePicker_1 = new DatePicker();
            PRI_DatePicker_1.SelectedDateFormat = DatePickerFormat.Short;
            PRI_DatePicker_1.Text = new DateTime(DateTime.Today.Year, 1, 1).ToString();
            PRI_DatePicker_1.DisplayDateEnd = DateTime.Today;
            PRI_DatePicker_1.DisplayDateStart = DateTime.Parse("01/01/2007");
            PRI_DatePicker_1.IsEnabled = true;
            PRI_DatePicker_1.SelectedDateChanged += delegate { PRI_Timer.Start(); };
            _SPanel_2.Children.Add(PRI_DatePicker_1);
            Label _Label_2 = new Label();
            _Label_2.Content = " по :";
            _Label_2.Foreground = Brushes.Navy;
            _SPanel_2.Children.Add(_Label_2);
            // Дата по
            PRI_DatePicker_2 = new DatePicker();
            PRI_DatePicker_2.SelectedDateFormat = DatePickerFormat.Short;
            PRI_DatePicker_2.Text = DateTime.Today.ToString();
            PRI_DatePicker_2.DisplayDateEnd = DateTime.Today;
            PRI_DatePicker_2.DisplayDateStart = DateTime.Parse("01/01/2007");
            PRI_DatePicker_2.IsEnabled = true;
            PRI_DatePicker_2.SelectedDateChanged += delegate { PRI_Timer.Start(); };
            _SPanel_2.Children.Add(PRI_DatePicker_2);

            PRI_Timer = new DispatcherTimer();
            PRI_Timer.Interval = new TimeSpan(0, 0, 1);
            PRI_Timer.Tick += delegate { PRI_Timer.Stop(); MET_SqlFilter(); };
        }

        /// <summary>СОБЫТИЕ Находим данные по строке ввода Фильтра</summary>
        /// <param name="sender">Текущий объект</param>
        /// <param name="e">Генерируемая детализация</param>
        protected override void PART_TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Наше условие фильтра
            PRO_TextFilter = (sender as TextBox)?.Text;

            // Перевод строки фильтра
            PRO_TextFilterTransliter = PRO_Transliter.MET_Replace(PRO_TextFilter);
            // Смотрим есть ли спец символы, кторые бы запортили SQL запрос
            PRO_TextFilter = PRO_TextFilter.Replace("'", "''");

            PRI_Timer.Stop();
            PRI_Timer.Start();
        }

        /// <summary>МЕТОД Создание фильтров для загрузки данных из SQL</summary>
        protected override void MET_SqlFilter()
        {
            PRI_Tip = PRI_TipRadioButton.PROP_Text ?? "0";

            if (PART_DataGrid.Columns.Count > 0)
            {
                switch (PRI_Tip)
                {
                    case "0":   // пациент kbol
                        PART_DataGrid.Columns[3].Header = "Создан";
                        PART_DataGrid.Columns[4].Header = "Умер";
                        PART_DataGrid.Columns[4].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[5].Visibility = Visibility.Collapsed;
                        PART_DataGrid.Columns[6].Visibility = Visibility.Collapsed;
                        PART_DataGrid.Columns[7].Visibility = Visibility.Collapsed;
                        PART_DataGrid.Columns[8].Visibility = Visibility.Collapsed;
                        break;
                    case "1":   // поликлиника APAC
                        PART_DataGrid.Columns[3].Header = "Посещение";
                        PART_DataGrid.Columns[4].Visibility = Visibility.Collapsed;
                        PART_DataGrid.Columns[5].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[6].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[7].Header = "Cod";
                        PART_DataGrid.Columns[7].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[8].Visibility = Visibility.Collapsed;
                        break;
                    case "2":   // стационар APSTAC
                        PART_DataGrid.Columns[3].Header = "Поступил";
                        PART_DataGrid.Columns[4].Header = "Выписан";
                        PART_DataGrid.Columns[4].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[5].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[6].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[7].Header = "IND";
                        PART_DataGrid.Columns[7].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[7].Width = 130;
                        PART_DataGrid.Columns[8].Header = "Отд.";
                        PART_DataGrid.Columns[8].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[8].Width = 80;
                        break;
                    case "3":   // параклиника parObsledov
                        PART_DataGrid.Columns[3].Header = "Посещение";                       
                        PART_DataGrid.Columns[4].Visibility = Visibility.Collapsed;
                        PART_DataGrid.Columns[5].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[6].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[7].Header = "Cod";
                        PART_DataGrid.Columns[7].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[7].Width = 80;     
                        PART_DataGrid.Columns[8].Header = "Протокол";
                        PART_DataGrid.Columns[8].Visibility = Visibility.Visible;
                        PART_DataGrid.Columns[8].Width = 220;
                        break;
                }
            }

            // Если число - то это KL, код посещения, номер полиса
            if (decimal.TryParse(PRO_TextFilter, out decimal result))
            {
                PRI_Cod = PRO_TextFilter;
                PRI_FIO = null;
            }
            else // иначе ФИО либо номер полиса с окончательной буквой типа полиса
            {
                PRI_FIO = PRO_TextFilter;
                PRI_Cod = null;
            }

            PRI_D1 = $"{PRI_DatePicker_1.SelectedDate:MM.dd.yyyy}";
            PRI_D2 = $"{PRI_DatePicker_2.SelectedDate:MM.dd.yyyy}";
           

            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);
        }

        /// <summary>МЕТОД Выбор данных - Находим пациента и открываем окно с ним</summary>
        protected override void MET_Select()
        {
            if (!PROP_FlagButtonSelect)
                return;
            try
            {
                DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
                if (_DataRowView == null)
                    return;
                
                decimal _KL = Convert.ToDecimal(_DataRowView.Row["KL"]);
                decimal _IND = Convert.ToDecimal(_DataRowView.Row["IND"]);

                MyTipProtokol _MyTipProtokol;
                switch (PRI_Tip)
                {
                    case "1":
                        _MyTipProtokol = new MyTipProtokol(eTipDocum.Pol);      // модуль поликлиники
                        break;
                    case "2":
                        _MyTipProtokol = new MyTipProtokol(eTipDocum.Stac);      // модуль стационара
                        break;
                    case "3":
                        _MyTipProtokol = new MyTipProtokol(eTipDocum.Paracl);    // модуль параклиники
                        break;
                    default:
                        _MyTipProtokol = new MyTipProtokol(eTipDocum.Null);     // модуль истории болезни
                        break;
                }

                // У не админов доступ только в историю болезни
                if (!MyGlo.Admin)
                {
                    _MyTipProtokol = new MyTipProtokol(eTipDocum.Null);
                }

                // Пытаемся открыть новую копию программы, для редактирования протоколов
                MyMet.MET_EditWindows(_MyTipProtokol.PROP_TipDocum, _IND, _KL);
                PROP_Return = true;
            }
            catch
            {
            }
            Close();
        }
    }
}
