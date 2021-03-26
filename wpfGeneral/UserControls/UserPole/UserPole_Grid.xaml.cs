using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>КЛАСС для поля статической Таблицы (Grid)</summary>
    /// <remarks>
    /// <para>Сокращения Formatа таблицы</para>
    /// <para>Все параметры не проверяются на ошибки и должны контролироваться вручную</para>
    /// <para>Поле таблицы (14 тип):</para>
    /// <list type="table">
    /// <listheader><term>Имя</term><description>Описание</description></listheader>
    /// <item> <term>\gR 3</term><description>сколько строк (3 - количество строк)</description> </item>
    /// <item> <term>\gC 3</term><description>сколько столбцов (3 - количество столбцов)</description> </item>
    /// <item> <term>\gsr</term><description>отображать границы между ячейками, если установлен то это таблица иначе просто группировка полей</description> </item>
    /// </list>
    /// <para>Поля ячеек:</para>
    /// <list type="table">
    /// <listheader><term>Имя</term><description>Описание</description></listheader>
    /// <item> <term>\gRo 3</term><description>номер строки (3 - сам номер, начинается с нуля, в данном случае это 4я строка), если нет параметра, то устанавливается в 0</description> </item>
    /// <item> <term>\gCo 3</term><description>номер колонки (3 - сам номер, начинается с нуля, в данном случае это 4я колонка), если нет параметра, то устанавливается в 0</description> </item>
    /// <item> <term>\gRs 2</term><description>объеденение строки (2 - объеденяем ячейки 2х строк)</description> </item>
    /// <item> <term>\gCs 2</term><description>объеденение колонки (2 - объеденяем ячейки 2х колонок)</description> </item>
    /// <item> <term>\gw 2,5</term><description>ширина колонки в долях (в 2,5 шире чем обычная колонка), ставить желательно в ячейке первой строке</description> </item>
    /// </list>
    /// </remarks>
    public partial class UserPole_Grid : VirtualPole
    {
        #region ---- СВОЙСТВО ----
        /// <summary>СВОЙСТВО Описание вопроса</summary>
        public override string PROP_Description
        {
            get { return (string)PART_Label.Content; }
            set
            {
                PART_Label.Content = value;
                // Если описания нету, то убираем описание
                if (value == "")
                    PART_Label.Visibility = Visibility.Collapsed;
                else
                    PART_Label.Padding = new Thickness(5, 0, 5, 0);
            }
        }
        /// <summary>СВОЙСТВО Отображаем внутренние границы, между ячейками (по умолчанию они скрыты)</summary>
        public bool PROP_ShowRectangle { get; private set; }
        #endregion ----

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_Grid()
        {
            InitializeComponent();

            PART_Grid.ContextMenu = MyGlo.ContextMenu;
            PART_Grid.Tag = this;
        }

        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            int _Row;
            int _Column;

            // Строк
            try { _Row = Convert.ToInt16(PROP_Format.PROP_Value["gR"]); }
            catch { _Row = 1; }
            // Колонок
            try { _Column = Convert.ToInt16(PROP_Format.PROP_Value["gC"]); }
            catch { _Column = 1; }
            // Границы между ячейками
            PROP_ShowRectangle = PROP_Format.PROP_Value.ContainsKey("gsr");
            // Если это настоящая таблица, меняем цвет фона    и обрамляем рамочкой
            if (PROP_ShowRectangle)
            {
                PART_Border.Background = Brushes.Beige;
                PART_Border.BorderThickness = new Thickness(0.5);
            }
            // Высота таблицы
            PART_Grid.MinHeight = 27 * _Row;
            // Добавляем строки и колонки
            MET_AddRow(_Row);
            MET_AddColumn(_Column);
        }

        /// <summary>МЕТОД Добавляем строки</summary>
        /// <param name="pCountRow">Количество строк</param>
        private void MET_AddRow(int pCountRow)
        {
            PART_Grid.RowDefinitions.Clear();
            RowDefinition _Row;
            for (int i = 0; i < pCountRow; i++)
            {
                _Row = new RowDefinition();
                _Row.MinHeight = 27;
                PART_Grid.RowDefinitions.Add(_Row);
            }
        }

        /// <summary>МЕТОД Добавляем Столбцы</summary>
        /// <param name="pCountColumn">Количество столбцов</param>
        private void MET_AddColumn(int pCountColumn)
        {
            PART_Grid.ColumnDefinitions.Clear();
            ColumnDefinition _Column;
            for (int i = 0; i < pCountColumn; i++)
            {
                _Column = new ColumnDefinition();
                _Column.Width = new GridLength(1, GridUnitType.Star);
                PART_Grid.ColumnDefinitions.Add(_Column);
            }
        }

        /// <summary>МЕТОД Добавляем к полю дочерние элементы</summary>
        /// <param name="pPole">Дочернее поле</param>
        public override bool MET_AddElement(VirtualPole pPole)
        {
            int _Row;
            int _Column;
            int _RowSpan;
            int _ColumnSpan;
            double _With;

            // Строка
            try { _Row = Convert.ToUInt16(pPole.PROP_Format.PROP_Value["gRo"]); }
            catch { _Row = 0; }
            Grid.SetRow(pPole, _Row);
            // Колонка
            try { _Column = Convert.ToUInt16(pPole.PROP_Format.PROP_Value["gCo"]); }
            catch { _Column = 0; }

            Grid.SetColumn(pPole, _Column);
            // Объединение по Строкам
            try { _RowSpan = Convert.ToUInt16(pPole.PROP_Format.PROP_Value["gRs"]); }
            catch { _RowSpan = 0; }

            if (_RowSpan > 0) Grid.SetRowSpan(pPole, _RowSpan);
            // Объединение по Колонокам
            try { _ColumnSpan = Convert.ToUInt16(pPole.PROP_Format.PROP_Value["gCs"]); }
            catch { _ColumnSpan = 0; }
            if (_ColumnSpan > 0) Grid.SetColumnSpan(pPole, _ColumnSpan);
            // Ширина колонки
            try { _With = Convert.ToDouble(pPole.PROP_Format.PROP_Value["gw"]); }
            catch { _With = 0; }
            if (_With > 0)
                PART_Grid.ColumnDefinitions[_Column].Width = new GridLength(_With, GridUnitType.Star);
            // Добавляем поле в ячейку
            PART_Grid.Children.Add(pPole);
            // Если нужно рисуем границу между ячейками
            if (PROP_ShowRectangle)
            {
                Rectangle _Rectangle = new Rectangle();
                Grid.SetRow(_Rectangle, _Row);
                Grid.SetColumn(_Rectangle, _Column);
                if (_RowSpan > 0) Grid.SetRowSpan(_Rectangle, _RowSpan);
                if (_ColumnSpan > 0) Grid.SetColumnSpan(_Rectangle, _ColumnSpan);
                _Rectangle.Stroke = Brushes.Navy;
                _Rectangle.StrokeThickness = 0.5;
                PART_Grid.Children.Add(_Rectangle);
                // Если ресуем границу между ячейками, то убираем границу у элемента
                pPole.PROP_HideBorder = true;
            }
            return true;
        }
    }
}
