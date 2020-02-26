using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wpfGeneral.UserControls;
using wpfGeneral.UserNodes;
using wpfMList.UserShablon;
using wpfStatic;

using G = wpfMList.UserModul_List;

namespace wpfMList
{
    /// <summary>КЛАСС Страница Списка протоколов</summary>
    public partial class UserPage_ListOne : Page
    {
        #region  ---- Открытые Поля ----
        /// <summary>Ветка</summary>
        public VirtualNodes PUB_Node;  
        /// <summary> Коллекция всех элементов </summary>
        private ObservableCollection<UserPole_ListProtokol> _collectionSource = new ObservableCollection<UserPole_ListProtokol>();
        /// <summary> Коллекция фильтрованных элементов </summary>
        public List<UserPole_ListProtokol> Collection = new List<UserPole_ListProtokol>();
        #endregion

        #region  ---- Закрытые Поля ----
        /// <summary> Данные таблицы </summary>
        private DataView PRI_DataView;
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserPage_ListOne()
        {
            InitializeComponent();

            // Cоздаем DataView для нашей таблице
            PRI_DataView = new DataView(G.PUB_TableProtokol);
            // Отображаем таблицу
            PART_DataGrid.ItemsSource = PRI_DataView;
            PART_DataGrid.IsReadOnly = false;
            //foreach (DataRowView _Row in PRI_DaViNazn)
            //{
            //    PRI_Row = _Row;
            //    // Начальный день
            //    int _DayN = (MET_PoleDat("DateN") - PROP_DateN).Days;
            //    for (int i = _DayN; (i <= _DayN + ((MET_PoleInt("Kurs") - 1) * MET_PoleInt("Period"))) & i < PRI_Day; i += MET_PoleInt("Period"))
            //    {
            //        _Name = "d" + i;
            //        PRI_Row[_Name] = MET_PoleInt("Amt");
            //    }
            //}

            //MessageBox.Show("Старт");
            //_collectionSource.Clear();
            //Collection.Clear();

            //Stopwatch _Stopwatch = new Stopwatch();
            //_Stopwatch.Start();
            //foreach (DataRow _Row in G.PUB_TableProtokol.Rows)
            //{
            //    G.PUB_RowProtokol = _Row;
            //    UserShablon_ListException _shaShablon = new UserShablon_ListException();
            //    _shaShablon.MET_CreateForm();
            //    UserPole_ListProtokol _ListProtokol = new UserPole_ListProtokol();
            //    _ListProtokol.MET_AddElement(_shaShablon);

            //    Collection.Add(_ListProtokol);
            //}
            //_Stopwatch.Stop();
            //MessageBox.Show(Convert.ToString(_Stopwatch.Elapsed.Milliseconds)); // Время в секундах
            //PART_ListBox.DataContext = Collection;
        }

        /// <summary>СОБЫТИЕ После загрузки окна</summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            MyFormat _Format;

            // Перебор полей
            foreach (DataColumn _Columns in PRI_DataView.Table.Columns)
            {
                Binding _Binding = new Binding(_Columns.ColumnName);
                DataGridTextColumn _ColumnText = new DataGridTextColumn();
               

                // Сначала заполняем константы: FIO
                if (_Columns.ColumnName == "FIO" && G.PUB_ListFormat.MET_If("LFio"))
                {    
                    _ColumnText.Header = "Пациент";
                    _Binding.Mode = BindingMode.OneWay;
                    _ColumnText.Binding = _Binding;
                    _ColumnText.Width = 250;           
                    PART_DataGrid.Columns.Add(_ColumnText);
                    continue;
                }

                // Сначала заполняем константы: DR
                if (_Columns.ColumnName == "DR" && G.PUB_ListFormat.MET_If("LDR"))
                {
                    _ColumnText.Header = "Дата р.";
                    _Binding.Mode = BindingMode.OneWay;
                    _Binding.StringFormat = "dd.MM.yyyy";
                    _ColumnText.Binding = _Binding;
                    _ColumnText.Width = 80;
                    PART_DataGrid.Columns.Add(_ColumnText);
                    continue;
                }

                DataGridTemplateColumn _ColumnTemplate = new DataGridTemplateColumn();
                FrameworkElementFactory _Factory;
                DataTemplate _Template = new DataTemplate();

                // Сначала заполняем константы: pDate
                if (_Columns.ColumnName == "pDate" && G.PUB_ListFormat.MET_If("LpDate"))
                {
                    // Визуальная часть
                    _ColumnTemplate.Header = "Создан";
                    _Binding.Mode = BindingMode.TwoWay;
                    _Binding.StringFormat = "dd.MM.yyyy";
                    _Factory = new FrameworkElementFactory(typeof(TextBlock));
                    _Factory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    _Factory.SetBinding(TextBlock.TextProperty, _Binding); 
                    _Template.VisualTree = _Factory;                       
                    _ColumnTemplate.CellTemplate = _Template;

                    // Редактируемая часть
                    _Factory = new FrameworkElementFactory(typeof(DatePicker));
                    _Factory.SetBinding(DatePicker.SelectedDateProperty, _Binding);
                    _Factory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    _Template = new DataTemplate();
                    _Template.VisualTree = _Factory;
                    _ColumnTemplate.CellEditingTemplate = _Template;
                    // Параметры
                    _ColumnTemplate.Width = 110;
                    PART_DataGrid.Columns.Add(_ColumnTemplate);
                    continue;
                }


                if (_Columns.ColumnName.Length >= 6 && _Columns.ColumnName.Substring(0, 6) == "VarId_")
                {
                    // Формат вопроса
                    _Format = (MyFormat) G.HashFormat["_Columns.ColumnName"];
                }
                else
                {
                    continue;
                }

                // Ищем вопрос
           //     DataRow[] _Row = G.PUB_TableShablon.Select("")
  
                // Визуальная часть

                _ColumnTemplate.Width = 100;
                _ColumnTemplate.Header = _Columns.ColumnName; 
                _Binding.Mode = BindingMode.TwoWay;           
                // Create the TextBlock
                _Factory = new FrameworkElementFactory(typeof(TextBlock));
                _Factory.SetBinding(TextBlock.TextProperty, _Binding);

                
                _Template.VisualTree = _Factory;

                _ColumnTemplate.CellTemplate = _Template;

                // Create the TextBox
                //_Binding.Mode = BindingMode.OneWay;                      
                //FrameworkElementFactory _Factory1 = new FrameworkElementFactory(typeof(TextBox));
                //_Factory1.SetBinding(TextBox.TextProperty, _Binding);
                //DataTemplate textTemplate1 = new DataTemplate();
                //textTemplate1.VisualTree = _Factory1;      
                //_Column.CellEditingTemplate = textTemplate1;


                // Create the DatePicker
                _Binding.Mode = BindingMode.OneWay;

                _Factory = new FrameworkElementFactory(typeof(DatePicker));
                _Binding.StringFormat = "dd.MM.yyyy";
                _Factory.SetBinding(DatePicker.SelectedDateProperty, _Binding);

                _Template = new DataTemplate();
                _Template.VisualTree = _Factory;


                _ColumnTemplate.CellEditingTemplate = _Template;
                
                PART_DataGrid.Columns.Add(_ColumnTemplate);
            }
        }
       
        /// <summary>СОБЫТИЕ Редактируем ячейку</summary>
        private void PART_DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {

        }
    }
}