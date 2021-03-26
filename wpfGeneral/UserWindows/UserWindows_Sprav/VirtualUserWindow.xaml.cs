using System;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Виртуальной формы справочников</summary>
    public partial class VirtualUserWindow
    {
        /// <summary>Если false - не пересчитываем размер колонок (ещё рано), true - с этого момента начинаем пересчет размера колонок</summary>
        private bool PRI_FlageResizeColumn;

        #region ---- Поля Protected ----
        /// <summary>Наименование справочника</summary>
        protected string PRO_TableName;

        /// <summary>Строка SQL запроса</summary>
        protected string PRO_SqlQuery = "";

        /// <summary>Строка SQL условия фильтра</summary>
        protected string PRO_SqlWhere = "";

        /// <summary>Строка фильтра уже загруженных данных</summary>
        protected string PRO_Where = "";

        /// <summary>Строка SQL условия фильтра</summary>
        protected string PRO_TextFilter = "";

        /// <summary>Поле, по которому происходит первичная сортировка</summary>
        protected int PRO_PoleSort;

        /// <summary>Выбранное Поле, которое показываем в нижней панели StatusBar</summary>
        /// <remarks>Если не выбранно, то используется поле сортировки PRO_PoleSort</remarks>
        protected int? PRO_PoleBarPanel;

        /// <summary>Поле, по которому происходит фильтровка записей (может быть составное из нескольких полей, иначе смысла нет)</summary>
        protected string PRO_PoleFiltr = "";

        /// <summary>Если строка ввода ищет через SQL</summary>
        protected bool PRO_FlagTextSql = false;

        /// <summary>Условия</summary>
        protected object[] PRO_mUslov = new object[10];

        /// <summary>Oбъявляем DataView</summary>
        protected DataView PRO_DataView;

        /// <summary>Объявляем класс транслита для фильтрации</summary>
        protected MyTransliter PRO_Transliter = new MyTransliter();

        /// <summary>Строка SQL условия фильтра с переводом</summary>
        protected string PRO_TextFilterTransliter = "";
        #endregion

        #region ---- СВОЙСТВА ----
        /// <summary>СВОЙСТВО Закрытие: true - с выбором, false - без выбора</summary>
        public bool PROP_Return { get; protected set; }

        /// <summary>СВОЙСТВО Скрывает, открывает кнопки редактирования</summary>
        public bool PROP_FlagButtonEdit { get; set; }
        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_FlagButtonEdit</summary>
        public static readonly DependencyProperty DEPR_FlagButtonEditProperty =
            DependencyProperty.Register("PROP_FlagButtonEdit", typeof(bool), typeof(VirtualUserWindow), new PropertyMetadata(false));

        /// <summary>СВОЙСТВО Скрывает, открывает кнопку выбора</summary>
        public bool PROP_FlagButtonSelect
        {
            get { return (bool)GetValue(DEPR_FlagButtonSelectProperty); }
            set { SetValue(DEPR_FlagButtonSelectProperty, value); }
        }
        /// <summary>РЕГИСТРАЦИЯ Свойства PROP_FlagButtonSelect</summary>
        public static readonly DependencyProperty DEPR_FlagButtonSelectProperty =
            DependencyProperty.Register("PROP_FlagButtonSelect", typeof(bool), typeof(VirtualUserWindow), new PropertyMetadata(false));

        /// <summary>СВОЙСТВО Модальное окно (бредовое свойство)</summary>
        public bool PROP_Modal { get; set; }
        #endregion

        #region ---- МЕТОДЫ Protected ----
        /// <summary>КОНСТРУКТОР</summary>
        protected VirtualUserWindow()
        {
            InitializeComponent();

            // Ставим Русский язык
            MyMet.MET_Lаng();
        }

        /// <summary>МЕТОД Открытие формы</summary>
        /// <param name="pLoadSqlFlag">Нужно ли загружать SQL данные = true (по умолчанию), или они уже были ранее загружены = false</param>
        /// <remarks>Должен распологаться после предварительной фильтрации данных (если эта фильтрация есть pLoadSqlFlag = false)</remarks>
        protected void MET_OpenForm(bool pLoadSqlFlag = true)
        {
            if (Owner == null)
                Owner = Application.Current.MainWindow;
            // Определяем условия и загружаем данные
            if (pLoadSqlFlag)  MET_SqlFilter();
            // Cоздаем DataView для нашей таблице
            PRO_DataView = new DataView(MyGlo.DataSet.Tables[PRO_TableName]);
            // Отображаем таблицу
            PART_DataGrid.ItemsSource = PRO_DataView;
            // Открываем кнопки редактирования
            if (PROP_FlagButtonEdit)
            {
                PART_DataGrid.IsReadOnly = false;                               // разрешаем редактировать справочник
                PART_Button_Add.IsEnabled = true;
                PART_Button_Edit.IsEnabled = true;
                PART_Button_Delete.IsEnabled = true;
            }
            MET_SizeChanged(PART_DataGrid);
        }

        /// <summary>МЕТОД Формирование Запроса (пустой)</summary>
        protected virtual string MET_SelectQuery() { return ""; }

        /// <summary>МЕТОД Создание фильтров для загрузки данных из SQL</summary>
        protected virtual void MET_SqlFilter()
        {
            // Запрос
            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);

        }

        /// <summary>МЕТОД Создание фильтров для уже загруженных данных</summary>
        protected virtual void MET_Filter()
        {
            if (PRO_TextFilter.Length > 0)
            {
                // Разбиваем строку поиска на отдельные слова
                string[] _mFilter = PRO_TextFilter.Split(' ');
                // Если есть спец поле, то используем его, иначе выбранный столбик
                string _PoleSort = PRO_PoleFiltr != "" ? PRO_PoleFiltr : PRO_DataView.Sort;
                // Если после сортировки так и не нашли то выходим (как правило, это когда нет данных)
                if (_PoleSort == "")
                    return;
                string _And = PRO_Where.Length > 0 ? " and " : "";
                try
                {
                    string _Filtr = $"{PRO_Where}{_And}";

                    foreach (var _f in _mFilter)
                    {
                        _Filtr += $"({_PoleSort} like '%{_f}%' or {_PoleSort} like '%{PRO_Transliter.MET_Replace(_f)}%') and ";
                    }
                    _Filtr += "1 = 1";

                    PRO_DataView.RowFilter = _Filtr;
                }
                catch
                {
                    try
                    {
                        PRO_DataView.RowFilter = PRO_Where + _And + PRO_DataView.Sort + " >= " + PRO_TextFilter;
                    }
                    catch
                    {
                        try
                        {
                            PRO_DataView.RowFilter = PRO_Where;
                        }
                        catch
                        {
                            PRO_DataView.RowFilter = "";
                        }
                    }
                }
            }
            else
                try
                {
                    PRO_DataView.RowFilter = PRO_Where;
                }
                catch
                {
                    PRO_DataView.RowFilter = "";
                }
            // Выделяем первую строку
            if (PRO_DataView != null && PRO_DataView.Table.Rows.Count != -1)
                PART_DataGrid.SelectedIndex = 0;
        }

        /// <summary>МЕТОД Удяляем данные справочника (пустой)</summary>
        /// <param name="pRow">Удаляемая строка</param>
        protected virtual bool MET_SqlDelete(DataRow pRow) { return true; }

        /// <summary>МЕТОД Сохраняем данные справочника (пустой)</summary>
        /// <param name="pStrValue">Данные которые сохраняем</param>
        /// <param name="pRow">Редактируемая строка</param>
        /// <param name="pColumn">Редактируемый столбец</param>
        protected virtual bool MET_SqlEdit(DataRow pRow, string pStrValue, DataGridColumn pColumn) { return true; }

        /// <summary>МЕТОД Выбора данных</summary>
        protected virtual void MET_Select()
        {
            Close();
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы (пустой)</summary>
        protected virtual void MET_RemoveAt() { }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые (пустой)</summary>
        protected virtual string MET_Header(int pIndex) { return ""; }

        /// <summary>МЕТОД Устанавливаем Ширину колонок (если надо) (пустой)</summary>
        protected virtual void MET_WithColumn() { }

        /// <summary>МЕТОД Разрешаем (false), Запрещаяем (true) редактировать столбцы (пустой)</summary>
        protected virtual bool MET_ReadOnly(int pIndex) { return true; }

        /// <summary>МЕТОД Генерация столбцов - если нужны какие то особенные столбцы (пустой)</summary>
        /// <param name="sender">Текущий объект</param>
        /// <param name="e">Генерируемая колонка</param>
        /// <returns>true - прошла генерация (дальше генерировать не надо), генерации не было</returns>
        protected virtual bool MET_GeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e) { return false; }

        /// <summary>СОБЫТИЕ Сохраняем файл с рисунком в байтовый массив (пустой)</summary>
        protected virtual void PART_ButtonOpen_Click(object sender, RoutedEventArgs e) { }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Добавить запись"</summary>
        protected virtual void PART_Button_Add_Click(object sender, RoutedEventArgs e) { }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Изменить запись"</summary>
        protected virtual void PART_Button_Edit_Click(object sender, RoutedEventArgs e) { }

        /// <summary>МЕТОД Открытие детализации строки (пустой)</summary>
        /// <param name="sender">Текущий объект</param>
        /// <param name="e">Генерируемая детализация</param>
        protected virtual void MET_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e) { }

        /// <summary>СОБЫТИЕ Находим данные по строке ввода Фильтра</summary>
        /// <param name="sender">Текущий объект</param>
        /// <param name="e">Генерируемая детализация</param>
        protected virtual void PART_TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Наше условие фильтра
            PRO_TextFilter = (sender as TextBox)?.Text;

            // Если строка ввода ищет через SQL (большая таблица загружанная по кусочкам), то вызываем отдельную фильтрацию для данной таблицы
            if (PRO_FlagTextSql)
            {
                // Перевод строки фильтра
                PRO_TextFilterTransliter = PRO_Transliter.MET_Replace(PRO_TextFilter);
                // Смотрим есть ли спец символы, кторые бы запортили SQL запрос
                PRO_TextFilter = PRO_TextFilter.Replace("'", "''");
                MET_SqlFilter();
                PRO_DataView.RowFilter = "";
            }
            // Вызываем дополнительный фильтр, куда добавляем фильтр по загруженным данным
            else
                MET_DopFilter();
        }

        /// <summary>МЕТОД Дополнительный фильтр по загруженным данным</summary>
        protected virtual void MET_DopFilter()
        {
            MET_Filter();
        }

        /// <summary>МЕТОД Меняем строку статусной панели StatusBar</summary>
        protected virtual void MET_BarText()
        {
            // Количество строк
            PART_BarText_2.Text = $"Строк: {PRO_DataView.Count}";
            // Показываем выбранное значение
            DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
            if (_DataRowView != null)
                PART_BarText_1.Text = Convert.ToString(_DataRowView.Row[PRO_PoleBarPanel ?? PRO_PoleSort]);
        }
        #endregion

        #region ---- МЕТОДЫ Private ----
        /// <summary>МЕТОД Убираем пустое пространство в последнюю колонку</summary>
        private void MET_SizeChanged(DataGrid pDataGrid)
        {
            if (!PRI_FlageResizeColumn)
                return;
            // Выставляем авторазмер
            pDataGrid.ColumnWidth = DataGridLength.Auto;
            if (pDataGrid.RenderSize.Width > 0)
            {
                // Ширина всех колонок
                double _AllSize = 0.0;
                // Находим ширину всех колонок
                foreach (DataGridColumn _Column in pDataGrid.Columns)
                {
                    if (Equals(_Column, pDataGrid.Columns[pDataGrid.Columns.Count - 1]))
                    {
                        double _Space = (pDataGrid.RenderSize.Width - 40) - _AllSize;
                        _Column.Width = new DataGridLength(_Space);
                    }
                    else
                    {
                        // Если уже проставлен MaxWidth, то не меняем, если бесконечность, то проставлям новый MaxWidth
                        _Column.MaxWidth = _Column.MaxWidth == double.PositiveInfinity ? _Column.Width.DesiredValue : _Column.MaxWidth;
                    }
                    _AllSize += _Column.ActualWidth;
                }
            }
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Удалить запись"</summary>
        private void PART_Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            // Находим запись
            DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
            // Если успешно удалили в базе, то и удаляем в таблице
            if (_DataRowView != null && MET_SqlDelete(_DataRowView.Row))
                MyGlo.DataSet.Tables[PRO_TableName].Rows.Remove(_DataRowView.Row);
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Выбрать"</summary>
        private void PART_Button_Check_Click(object sender, RoutedEventArgs e)
        {
            MET_Select();
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку "Закрыть"</summary>
        private void PART_Button_Undo_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>СОБЫТИЕ После загрузки окна</summary>
        private void UserWindows_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Cортировка по номеру поля (потом можно поменять на индивидуальную)
                if (PART_DataGrid.Columns.Count >= PRO_PoleSort && PRO_DataView.Table.Rows.Count > 0)
                    PRO_DataView.Sort = PART_DataGrid.Columns[PRO_PoleSort].SortMemberPath;
            }
            catch
            {
                // ignored
            }

            if (PART_DataGrid.Columns.Count > 0)
            {


                // Выделяем первую строку
                if (PRO_DataView.Table.Rows.Count != -1)
                    PART_DataGrid.SelectedIndex = 0;
            }
        }

        /// <summary>СОБЫТИЕ Меняются размеры DataGrid</summary>
        private void PART_DataGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Убираем пустое пространство в последнюю колонку
            MET_SizeChanged((DataGrid)sender);
        }

        /// <summary>СОБЫТИЕ Сохроняем измения в ячейке</summary>
        private void PART_DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Находим запись
            DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
            // Меняем (добавляем запись), если строка не пустая
            string _Str = ((TextBox)e.EditingElement).Text;
            if (_Str == "" || !MET_SqlEdit(_DataRowView.Row, _Str, e.Column))
                e.Cancel = true;                                                // в случае ошибки не даем зафиксировать данные
        }

        /// <summary>СОБЫТИЕ Сохроняем измения в строке</summary>
        private void PART_DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e) { }

        /// <summary>СОБЫТИЕ Выбор данных двойным нажатием мышки</summary>
        private void PART_DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Выбор, если токо разрешён
            if (PROP_FlagButtonSelect)
                MET_Select();
        }

        /// <summary>СОБЫТИЕ Генерация столбцов</summary>
        private void PART_DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // Номер столбца
            int _Index = (sender as DataGrid).Columns.Count;
            // Запускаем свою генерацию, если не прошла, то пытаемся делать общую генерацию
            if (!MET_GeneratingColumn(sender, e))
            {
                // Отображаем картинки из базы
                if (e.PropertyName == "Image")
                {
                    DataGridTemplateColumn _TemplateColumn = new DataGridTemplateColumn
                    {
                        Header = "Image",
                        CellTemplate = (DataTemplate) Resources["dueDateCellTemplate"],
                        CellEditingTemplate = (DataTemplate) Resources["dueDateCellEditingTemplate"]
                    };
                    e.Column = _TemplateColumn;
                }
                // Отображаем иконки из словаря
                if (e.PropertyName == "Icon")
                {
                    DataGridTemplateColumn _TemplateColumn = new DataGridTemplateColumn
                    {
                        Header = "Icon",
                        CellTemplate = (DataTemplate)Resources["icoDateCellTemplate"]
                    };
                    e.Column = _TemplateColumn;
                }
                // Отображаем иконки из словаря (как правило для удаления)
                if (e.PropertyName == "IconDel")
                {
                    DataGridTemplateColumn _TemplateColumn = new DataGridTemplateColumn
                    {
                        Header = "IconDel",
                        CellTemplate = (DataTemplate)Resources["icoDelDateCellTemplate"]
                    };
                    e.Column = _TemplateColumn;
                }
                // Отображаем Дату в нормальном виде
                if (e.PropertyType == typeof(DateTime))
                {
                    if (e.Column is DataGridTextColumn _DateText)
                        _DateText.Binding.StringFormat = "dd.MM.yyyy";
                }
                // Отображаем текст (для просмотра)
                if (e.PropertyType == typeof(string))
                {
                    // Настраиваем перенос строки, в ячейке
                    if (e.Column is DataGridTextColumn _DateText)
                        _DateText.ElementStyle = this.Resources["WordWrapStyle"] as Style;
                }
                //// Отображаем Дату в нормальном виде
                //if (e.PropertyName == "NameLS")
                //{
                //    DataGridTemplateColumn _TemplateColumn = new DataGridTemplateColumn();
                //    _TemplateColumn.CellTemplate = (DataTemplate)Resources["dueDateCellTemplateSpavEdit"];
                //    _TemplateColumn.CellEditingTemplate = (DataTemplate)Resources["dueDateCellTemplateSpavEdit"];
                //    e.Column = _TemplateColumn;
                //}
            }
            // Переименовываем столбцы
            e.Column.Header = MET_Header((sender as DataGrid).Columns.Count);
            // Разрешаем редактировать столбцы
            e.Column.IsReadOnly = MET_ReadOnly(_Index);
        }

        /// <summary>СОБЫТИЕ После генерации всех столбцов</summary>
        private void PART_DataGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            // Устанавливаем ширину колонок (если надо)
            MET_WithColumn();
            // Удаляем ненужный столбец, если есть колонки
            MET_RemoveAt();
        }

        /// <summary>МЕТОД Сохраняем файл с рисунком в байтовый массив</summary>
        public static byte[] MET_GetPhoto(string pFilePath)
        {
            FileStream _Stream = new FileStream(pFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader _Reader = new BinaryReader(_Stream);
            byte[] _mPhoto = _Reader.ReadBytes((int)_Stream.Length);
            _Reader.Close();
            _Stream.Close();
            return _mPhoto;
        }

        /// <summary>СОБЫТИЕ Изменения статуса окна, если оно свернуто - сворачивает главное оконо</summary>
        private void UserWindows_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                if (Owner != null)
                {
                    Owner.WindowState = WindowState.Minimized;
                }
            }
        }

        /// <summary>СОБЫТИЕ Инициализация новой строки</summary>
        private void PART_DataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e){ }

        /// <summary>СОБЫТИЕ Нажали на Кнопку клавиатуры в PART_DataGrid (пока токо Удаление)</summary>
        private void PART_DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Нажали на Delete (если разрешено редактировать, то удаляем запись)
            if (e.Key == Key.Delete & PROP_FlagButtonEdit)
            {
                // Находим запись
                DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
                // Если успешно удалили в базе, то и удаляем в таблице
                if (MET_SqlDelete(_DataRowView.Row))
                {
                    MyGlo.DataSet.Tables[PRO_TableName].Rows.Remove(_DataRowView.Row);
                }
                else
                    e.Handled = true;
            }
        }

        /// <summary>СОБЫТИЕ При закрытии окна</summary>
        private void UserWindows_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Owner?.Activate();
        }

        /// <summary>СОБЫТИЕ Генерация детализации строки</summary>
        private void PART_DataGrid_LoadingRowDetails(object sender, DataGridRowDetailsEventArgs e)
        {
            MET_LoadingRowDetails(sender, e);
        }

        /// <summary>СОБЫТИЕ После создания DataGrid</summary>
        private void PART_DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            PRI_FlageResizeColumn = true;
            MET_SizeChanged(PART_DataGrid);
        }

        /// <summary>СОБЫТИЕ При выборе новой строки в DataGrid</summary>
        private void PART_DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Меняем строку статусной панели StatusBar
            MET_BarText();
        }
        #endregion

        /// <summary>МЕТОД Проверяем доступность данного окна текущему пользователю</summary>
        public static bool MET_Access()
        {
            return true;
        }
    }

    ///<summary>КЛАСС Конвертор Возвращаем иконку из словаря по имени</summary>
    class ConvertTextToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString() == "") return null;
            try
            {
                return (BitmapImage)Application.Current.FindResource(value.ToString());
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
