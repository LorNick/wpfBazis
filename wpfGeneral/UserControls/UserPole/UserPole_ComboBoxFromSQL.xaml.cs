using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls;
using wpfStatic;

namespace wpfGeneral.UserControls
{
    /// <summary>
    /// КЛАСС для Текстового поля (Text)
    /// </summary>
    public partial class UserPole_ComboBoxFromSQL : VirtualPole
    {
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
                    PART_Label.Padding = new Thickness(5, 0, 5, 0);
            }
        }

        /// <summary>СВОЙСТВО Ответ</summary>
        public override string PROP_Text
        {
            get { return PROP_ComboBox.SelectedValue?.ToString() ?? ""; }
            set { PROP_ComboBox.SelectedValue = value; }
        }

        /// <summary>СВОЙСТВО Цвет текста ответа</summary>
        public override Brush PROP_ForegroundText
        {
            get
            {
                return PART_ComboBox?.Foreground;
            }
            set
            {
                if (PART_ComboBox != null)
                    PART_ComboBox.Foreground = value;
            }
        }

        /// <summary>СВОЙСТВО Минимальная Ширина описания</summary>
        public override double PROP_MinWidthDescription
        {
            get { return PART_Label.MinWidth; }
            set { PART_Label.MinWidth = value; }
        }

        /// <summary>СВОЙСТВО Ширина текста</summary>
        public override double PROP_WidthText
        {
            get { return PART_ComboBox.Width; }
            set
            {
                PART_ComboBox.Width = value;
                if (double.IsNaN(PART_ComboBox.Width))
                    PART_ComboBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                else
                    PART_ComboBox.HorizontalAlignment = HorizontalAlignment.Left;
            }
        }

        /// <summary>СВОЙСТВО Помечаем поле текста</summary>
        public override byte PROP_PometkaText
        {
            get { return PRO_PometkaText; }
            set
            {
                PRO_PometkaText = value;
                if (PRO_PometkaText == 1)
                {
                    PART_Border.BorderBrush = Brushes.Red;
                }
                else
                    PART_Border.BorderBrush = Brushes.Gray;
            }
        }

        /// <summary>СВОЙСТВО ComboBox</summary>
        public RadComboBox PROP_ComboBox => PART_ComboBox;

        /// <summary>СВОЙСТВО Сортировка списка (false - по алфавиту, true - по порядку Cod)</summary>
        public bool PROP_SortList { get; set; }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_ComboBoxFromSQL()
        {
            InitializeComponent();

            PART_ComboBox.ContextMenu = MyGlo.ContextMenu;
            PART_ComboBox.Tag = this;
        }

        ///<summary>МЕТОД Инициализация поля</summary>
        public override void MET_Inicial()
        {
            // Находим дату создания протокола из первого поля шаблона с pDate
            DateTime PRI_Date = DateTime.Parse(PROP_FormShablon.GetPole("DateOsmotr").PROP_Text);

            // Располагаем текст
            PROP_TextAlignment = TextAlignment.Left;
            if (PROP_Format.MET_If("fac"))
                PROP_TextAlignment = TextAlignment.Center;
            if (PROP_Format.MET_If("far"))
                PROP_TextAlignment = TextAlignment.Right;
            // Сортировка нисподающего списка (по умолчанию по алфавиту, иначе по порядку List)
            PROP_SortList = PROP_Format.MET_If("sortlist");
            // Загружаем варианты ответов
            // Разбиваем на тип справочника и подпись в шаблолне, через символ '|'
            var _Description = PROP_DescriptionOriginal.Split('|');
            PART_Label.Content = _Description.Length > 1 ? _Description[1] : PART_Label.Content;
            PROP_Description = _Description.Length > 1 ? _Description[1] : PROP_Description;
            // Строка запроса
            string sql = "";
            List<string> _ValueList;
            // Проверим запрос это или нет
            if (_Description[0].StartsWith("@{") && _Description[0].EndsWith("}"))
            {
                sql = _Description[0].Substring(2, _Description[0].Length - 3);
                // Разберем переменные в запросе SQL
                // Дата события {pDate}
                sql = sql.Replace("{pDate}", PRI_Date.ToString("yyyy-MM-dd"));
                // Код ЛПУ {Lpu}
                sql = sql.Replace("{Lpu}", MyGlo.Lpu.ToString());
                // Код пациента {KL}
                sql = sql.Replace("{KL}", MyGlo.KL.ToString());
                // Стационар
                // Стационар - Номер отделения (otd) {Otd}
                sql = sql.Replace("{Otd}", MyGlo.Otd.ToString());
                // Стационар - Номер отделения (otd), поликлиника - код специальности (SPRS), параклиника - код элемента кабинета (parEL:Cod) (нужно отсюда убрать) {IND}
                sql = sql.Replace("{IND}", MyGlo.IND.ToString());
                //// Дата госпитализации в стационар {DN}
                //sql = sql.Replace("{DN}", Convert.ToString(MyGlo.HashAPSTAC["DN"]));
                //// Дата выписки из стационара {DK}
                //sql = sql.Replace("{DK}", Convert.ToString(MyGlo.HashAPSTAC["DK"]));
                //// Диагноз из стационара {D}
                //sql = sql.Replace("{D}", Convert.ToString(MyGlo.HashAPSTAC["D"]));
                //// Поликлиника
                //// Дата посещения поликлиники {DP}
                //sql = sql.Replace("{DP}", Convert.ToString(MyGlo.HashAPAC["DP"]));
                //// Диагноз посещения поликлиники {DP}
                //sql = sql.Replace("{D}", Convert.ToString(MyGlo.HashAPAC["D"]));

                _ValueList = (from IDataRecord r in MySql.MET_QuerySqlDataReader(sql) select r.GetValue(0).ToString()).ToList();
            }
            else
            {
                // Загружаем варианты ответов
                MySql.MET_DsAdapterFill(MyQuery.MET_List_Select_4(PROP_FormShablon.PROP_TipProtokol.PROP_List, PROP_Shablon, PROP_VarId, PROP_SortList), Name);
                _ValueList = (from DataRow _Row in MyGlo.DataSet.Tables[Name].Rows select _Row["Value"].ToString()).ToList();
            }

            PROP_ComboBox.ItemsSource = _ValueList;

            // Начальное значение
            PROP_ComboBox.SelectedValue = PROP_Text;
        }

        /// <summary>СОБЫТИЕ При изменении поля</summary>
        private void PART_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Если есть шаблон
            if (this.PROP_FormShablon?.PROP_Created ?? false)
            {
                // Активируем кнопку "Сохранить"
                MyGlo.Event_SaveShablon?.Invoke(true);
                // Запускаем Lua фунцкию, на изменение записи
                this.PROP_Lua?.MET_OnChange();
            }
        }
    }
}
