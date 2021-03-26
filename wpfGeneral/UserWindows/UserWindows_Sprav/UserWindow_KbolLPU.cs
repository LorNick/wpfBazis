using System;
using System.Data;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Таблицы "kbol"  для онколовгов ЛПУ</summary>
    public class UserWindow_KbolLPU : VirtualUserWindow
    {
        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_KbolLPU()
        {
            // Ставим Русский язык
            MyMet.MET_Lаng();
            // Имя таблицы
            PRO_TableName = "kbolLPU";
            // Если строка ввода ищет через SQL
            PRO_FlagTextSql = true;
            // Заголовок
            Title = "Приписные к ЛПУ пациенты по БАЗЕ ЗАСТРАХОВАННЫХ, посещавших БУЗОО КОД (первые 200 человек):";
            // Подсказка в строке поиска
            PART_TextBox.WatermarkContent = "Введите ФИО пациента (например: ИВАНОВ Ф И)";
            // Размеры окна
            MinWidth = Width;
            Height = 660;
            // Сортируем по Фамилии
            PRO_PoleSort = 0;
            // Разрешаем выбирать записи
            PROP_FlagButtonSelect = true;
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.kbol_Select_4(PRO_SqlWhere);
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "ФИО пациента", "Дата рождения" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[1].Width = 400;    // Name
        }

        /// <summary>МЕТОД Фильтруем данные для больших таблиц</summary>
        protected override void MET_SqlFilter()
        {
            PRO_SqlWhere = MyGlo.KL + ")";

            // ФИО пациента
            if (PRO_TextFilter.Length > 0)
                PRO_SqlWhere += $" and (k.FAM like '{PRO_TextFilter}%' or k.FAM like '{PRO_TextFilterTransliter}%')";
            // Запрос
            MySql.MET_DsAdapterFill(MET_SelectQuery(), PRO_TableName);
        }

        /// <summary>МЕТОД Выбор данных</summary>
        protected override void MET_Select()
        {
            if (!PROP_FlagButtonSelect)
                return;
            // Список пациентов
            try
            {
                DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
                MyGlo.KL = Convert.ToDecimal(_DataRowView.Row["KL"]);
                PROP_Return = true;
            }
            catch
            {
            }
            Close();
        }
    }
}
