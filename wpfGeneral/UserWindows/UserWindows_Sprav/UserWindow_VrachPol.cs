using System;
using System.Data;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Справочник врачей поликлиники</summary>
    public class UserWindow_VrachPol : VirtualUserWindow
    {
        /// <summary>Код Врача</summary>
        public int PUB_Cod;

        /// <summary>ФИО Врача</summary>
        public string PUB_Name = "";

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_VrachPol()
        {
            // Имя таблицы
            PRO_TableName = "s_VrachPol";
            // Заголовок
            Title = "Справочник Врачей поликлиники:";
            //Размеры
            MinWidth = Width;
            // Сортируем по полю ФИО
            PRO_PoleSort = 1;
            // Показываем в подсказке
            PRO_PoleBarPanel = 2;
            // Поле поиска
            PRO_PoleFiltr = "Filter";
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.s_VrachPol_Select_1();
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "Код", "ФИО врача", "Профиль", "Специализация", "Подразделение" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[1].Width = 60;        // ФИО
            PART_DataGrid.Columns[2].Width = 200;       // Дата р.
        }

        /// <summary>МЕТОД Выбор данных</summary>
        protected override void MET_Select()
        {
            if (!PROP_FlagButtonSelect)
                return;
            try
            {
                DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
                PUB_Cod = Convert.ToInt16(_DataRowView.Row["KOD"]);
                PUB_Name = Convert.ToString(_DataRowView.Row["TKOD"]);
                PROP_Return = true;
            }
            catch
            {
            }
            Close();
        }
    }
}
