using System;
using System.Data;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Справочник Диагнозов МКБ-10</summary>
    public class UserWindow_Diag : VirtualUserWindow
    {
        /// <summary>СВОЙСТВО Код диагноза</summary>
        public string PROP_Cod { get; private set; }

        /// <summary>СВОЙСТВО Наименование диагноза</summary>
        public string PROP_Text { get; private set; }

        /// <summary>Начальный фильтр SQL</summary>
        private readonly string PRI_JsonFiltr = "";

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pSqlOneWhere">Фильтр из тега (необязательный)</param>
        public UserWindow_Diag(string pSqlOneWhere = "")
        {
            // Фильтр для диагнозов, к примеру (устаревший) показать диагнозы только для kslp: "dbo.jsonIf(xInfo, 'kslp') = 1"
            if (!string.IsNullOrWhiteSpace(pSqlOneWhere))
                PRI_JsonFiltr = $" and dbo.jsonIf(xInfo, '{pSqlOneWhere}') = 1";
            // Имя таблицы
            PRO_TableName = "s_Diag";
            //Размеры
            MinWidth = Width;
            // Заголовок
            Title = "Справочник Диагнозов МКБ-10:";
            // Подсказка в строке поиска
            PART_TextBox.WatermarkContent = "Введите части слов через пробел (например: С5 ШЕ МАТ Ч)";
            // Сортируем по полю Варианты ответа
            PRO_PoleSort = 0;
            // Спец поле (составное) по которому производится поиск
            PRO_PoleFiltr = "Filter";
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.s_Diag_Select_1(PRI_JsonFiltr);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "Код", "Наименование" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[1].Width = 60;
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Выбор данных</summary>
        protected override void MET_Select()
        {
            if (!PROP_FlagButtonSelect)
                return;
            // Список выбора
            try
            {
                DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
                PROP_Cod = Convert.ToString(_DataRowView.Row["KOD"]);
                PROP_Text = Convert.ToString(_DataRowView.Row["TKOD"]);
                PROP_Return = true;
            }
            catch
            {
                // ignored
            }
            Close();
        }
    }
}
