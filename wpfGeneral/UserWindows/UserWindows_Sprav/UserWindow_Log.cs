using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using wpfGeneral.UserControls;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Логирование</summary>
    public class UserWindow_Log : VirtualUserWindow
    {
        // Элементы фильтра
        private UserPole_Text PRI_FiltrUserCod;
        private UserPole_Text PRI_FiltrUserName;
        private UserPole_Text PRI_FiltrCompName;
        private UserPole_Text PRI_FiltrProcess;
        private UserPole_Data PRI_FiltrDateN;
        private UserPole_Data PRI_FiltrDateK;
        private UserPole_ComboBox PRI_FiltrLevel;
        private Button PRI_ButtonReplace;
        private Button PRI_ButtonDelete;
        private UserPole_ComboBox PRI_Server;
        private Button PRI_GotoHistory;

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Log()
        {
            // Имя таблицы
            PRO_TableName = "log_wpfBazis";
            // Заголовок
            Title = "Логирование wpfBazis:";
            // Размеры окна
            MinWidth = 1680;
            Width = 1680;
            Height = 900;
            PRO_FlagTextSql = true;
            // Сортируем по полю Наименование
            PRO_PoleSort = 0;
            // Показываем в подсказке (по полю Message)
            PRO_PoleBarPanel = 11;
            // Создаем фильтр
            MET_CreateFiltr();
            // Запрещаем редактировать записи
            PROP_FlagButtonEdit = false;
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
            // Убираем строку добавления
            PART_DataGrid.CanUserAddRows = false;
            // Скрываем панель поиска
            PART_FindePanel.Visibility = Visibility.Collapsed;
            // Отображаем детализацию строк
            PART_DataGrid.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.log_wpfBazis_Select_1(PRO_SqlWhere, PRI_Server.PART_ComboBox.SelectedValue.ToString());
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
            PART_DataGrid.Columns.RemoveAt(0);
            PART_DataGrid.Columns.RemoveAt(0);
        }
       
        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "", "",  "Cod", "User", "Name", "Comp", "Ver", "Process", "Date", "Level", "Message", "Exception", "StackTrace", "D" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[3].Width = 60;
            PART_DataGrid.Columns[5].Width = 130;
            PART_DataGrid.Columns[6].Width = 130;
            PART_DataGrid.Columns[7].Width = 40;
            PART_DataGrid.Columns[9].Width = 80;
            PART_DataGrid.Columns[10].Width = 50; // Level
            PART_DataGrid.Columns[11].Width = 350; // Message
            PART_DataGrid.Columns[12].Width = 300; // Exception
            PART_DataGrid.Columns[13].Width = 350; // StackTrace
        }

        /// <summary>МЕТОД Создание фильтров</summary>
        private void MET_CreateFiltr()
        {
            PART_Grid.RowDefinitions[0].Height = new GridLength(80, GridUnitType.Auto);
            PART_Expander.Visibility = Visibility.Visible;
            Border _Border = new Border();
            _Border.Style = (Style)FindResource("Border_2");
            PART_Expander.Content = _Border;
            PART_Expander.IsExpanded = true;
            StackPanel _SPanel = new StackPanel();
            _Border.Child = _SPanel;
            // ---- Настраиваем фильтр
            StackPanel _SPanel_1 = new StackPanel();
            _SPanel_1.Orientation = Orientation.Horizontal;
            _SPanel_1.Margin = new Thickness(0, 2, 0, 2);
            _SPanel.Children.Add(_SPanel_1);
            // Server
            PRI_Server = new UserPole_ComboBox();
            PRI_Server.PART_ComboBox.ItemsSource = new[] { "Pol", "Fil" };
            PRI_Server.PART_ComboBox.SelectedValue = "Pol";
            PRI_Server.PROP_WidthText = 60;
            PRI_Server.PROP_Description = "Server";
            _SPanel_1.Children.Add(PRI_Server);
            // UserCod
            PRI_FiltrUserCod = new UserPole_Text();
            PRI_FiltrUserCod.PROP_MinWidthDescription = 90;
            PRI_FiltrUserCod.PROP_WidthText = 100;
            PRI_FiltrUserCod.PROP_Description = "UserCod";
            _SPanel_1.Children.Add(PRI_FiltrUserCod);
            // UserName
            PRI_FiltrUserName = new UserPole_Text();
            PRI_FiltrUserName.PROP_MinWidthDescription = 90;
            PRI_FiltrUserName.PROP_WidthText = 100;
            PRI_FiltrUserName.PROP_Description = "UserName";
            _SPanel_1.Children.Add(PRI_FiltrUserName);
            // CompName
            PRI_FiltrCompName = new UserPole_Text();
            PRI_FiltrCompName.PROP_WidthText = 100;
            PRI_FiltrCompName.PROP_Description = "CompName";
            _SPanel_1.Children.Add(PRI_FiltrCompName);
            // Process
            PRI_FiltrProcess = new UserPole_Text();
            PRI_FiltrProcess.PROP_WidthText = 100;
            PRI_FiltrProcess.PROP_Description = "Process";
            _SPanel_1.Children.Add(PRI_FiltrProcess);
            // DateN
            PRI_FiltrDateN = new UserPole_Data();
            PRI_FiltrDateN.PROP_WidthText = 100;
            PRI_FiltrDateN.PROP_Date = DateTime.Today;
            PRI_FiltrDateN.PROP_Description = "DateN";
            _SPanel_1.Children.Add(PRI_FiltrDateN);
            // DateK
            PRI_FiltrDateK = new UserPole_Data();
            PRI_FiltrDateK.PROP_WidthText = 100;
            PRI_FiltrDateK.PROP_Date = DateTime.Today;
            PRI_FiltrDateK.PROP_Description = "DateK";
            _SPanel_1.Children.Add(PRI_FiltrDateK);
            // Level
            PRI_FiltrLevel = new UserPole_ComboBox();
            PRI_FiltrLevel.PART_ComboBox.ItemsSource = new[] { "All", "Trace", "Debug", "Info", "Warn", "Error", "Fatal" };
            PRI_FiltrLevel.PART_ComboBox.SelectedValue = "All";
            PRI_FiltrLevel.PROP_WidthText = 100;
            PRI_FiltrLevel.PROP_Description = "Level";
            _SPanel_1.Children.Add(PRI_FiltrLevel);
            // ---- Кнопка обновления фильтров
            PRI_ButtonReplace = new Button();
            PRI_ButtonReplace.Content = "Обновить";
            PRI_ButtonReplace.Margin = new Thickness(20, 0, 0, 0);
            PRI_ButtonReplace.Click += delegate{ MET_SqlFilter(); };
            _SPanel_1.Children.Add(PRI_ButtonReplace);
            // ---- Кнопка удаления отфильтрованных записей
            PRI_ButtonDelete = new Button();
            PRI_ButtonDelete.Content = "Удалить";
            PRI_ButtonDelete.Margin = new Thickness(20, 0, 0, 0);
            PRI_ButtonDelete.Click += delegate
            {
                // Спросим на всякий случай
                if (MessageBox.Show("Вы точно хотите Удалить все эти записи?", "Внимание!", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    return;
                // Удаляем
                MySql.MET_QueryNo(MyQuery.log_wpfBazis_Delete_1(PRO_SqlWhere, PRI_Server.PART_ComboBox.SelectedValue.ToString()));
                MET_SqlFilter();
            };
            _SPanel_1.Children.Add(PRI_ButtonDelete);
            // ---- Кнопка перехода в историю с ошибкой
            PRI_GotoHistory = new Button();
            PRI_GotoHistory.Content = "Переход";
            PRI_GotoHistory.Margin = new Thickness(20, 0, 0, 0);
            PRI_GotoHistory.Click += delegate
            {
                try
                {
                    DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
                    if (_DataRowView == null)
                        return;                   
                    decimal _KL = Convert.ToDecimal(_DataRowView.Row["KL"]);
                    decimal _IND = Convert.ToDecimal(_DataRowView.Row["CodApstac"]);
                    MySql.MET_DsAdapterFill(MyQuery.log_wpfBazis_Select_2(_KL, _IND), "TableRezult");
                    DataTable _Table = MyGlo.DataSet.Tables["TableRezult"];

                    if (_Table.Rows.Count == 0)
                    {
                        MessageBox.Show("Данной записи на текущем сервере нет, возможно эта запись сегодня создана на другом сервере", "Внимание!");
                        return;
                    }
                    DataRow _Row = _Table.Rows[0];
                    string _Tip = (string)_Row["Tip"];

                    MyTipProtokol _MyTipProtokol;
                    switch (_Tip)
                    {
                        case "стац":
                            _MyTipProtokol = new MyTipProtokol(eTipDocum.Stac);     // модуль стационара
                            break;
                        case "пол":
                            _MyTipProtokol = new MyTipProtokol(eTipDocum.Pol);      // модуль поликлиники
                            break;
                        case "пар":
                            _MyTipProtokol = new MyTipProtokol(eTipDocum.Paracl);   // модуль параклиники
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
            };
            _SPanel_1.Children.Add(PRI_GotoHistory);
        }

        /// <summary>МЕТОД Открытие детализации строки</summary>
        /// <param name="sender">Текущий объект</param>
        /// <param name="e">Генерируемая детализация</param>
        protected override void MET_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            string _Text = (e.Row.Item as DataRowView).Row[0].ToString();
            // Если нет данных, то выходим
            if (string.IsNullOrWhiteSpace(_Text))
                return;

            TextBox _TextBox = new TextBox();
            _TextBox.MaxWidth = 1000;
            _TextBox.MaxHeight = 500;
            _TextBox.TextWrapping = TextWrapping.Wrap;
            _TextBox.Text = _Text;
            _TextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;

            var _StackPanel = e.DetailsElement.FindName("PART_DetailsPanel") as StackPanel;           
            _StackPanel?.Children.Add(_TextBox);
        }
        
        /// <summary>МЕТОД Фильтруем данные для больших таблиц</summary>
        protected override void MET_SqlFilter()
        {
            PRO_SqlWhere = "where 1 = 1";

            // Фильтр по UserCod
            if (PRI_FiltrUserCod.PROP_Text != "")
                PRO_SqlWhere += $" and UserCod like '{PRI_FiltrUserCod.PROP_Text}'";

            // Фильтр по UserName
            if (PRI_FiltrUserName.PROP_Text != "")
                PRO_SqlWhere += $" and UserName like '%{PRI_FiltrUserName.PROP_Text}%'";

            // Фильтр по CompName
            if (PRI_FiltrCompName.PROP_Text != "")
                PRO_SqlWhere += $" and CompName like '%{PRI_FiltrCompName.PROP_Text}%'";

            // Фильтр по Level
            if (PRI_FiltrLevel.PART_ComboBox.SelectedIndex != 0)
                PRO_SqlWhere += $" and LogLevel = '{PRI_FiltrLevel.PART_ComboBox.SelectedValue}'";

            // Фильтр по Process
            if (PRI_FiltrProcess.PROP_Text != "")
                PRO_SqlWhere += $" and Process like '{PRI_FiltrProcess.PROP_Text}'";

            // Фильтр по Date
            if (PRI_FiltrDateN.PROP_Text != "" && PRI_FiltrDateK.PROP_Text != "")
                    PRO_SqlWhere += $" and cast(LogDate as date) between '{PRI_FiltrDateN.PROP_Date:MM.dd.yyyy}' and '{PRI_FiltrDateK.PROP_Date:MM.dd.yyyy}'";
            
            // При старте, ниже указанные фильтры не учитываем
            string _Sort = "";
            if (PRO_DataView != null)
                _Sort = PRO_DataView.Sort;

            // Запрос
            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);
        }

        /// <summary>МЕТОД Проверяем доступность данного окна текущему пользователю</summary>        
        public static new bool MET_Access()
        {
            if (!MyGlo.Admin)
            {
                MessageBox.Show("У вас нет доступа к данной информации.");
                return false;
            }
            return true;
        }
    }
}
