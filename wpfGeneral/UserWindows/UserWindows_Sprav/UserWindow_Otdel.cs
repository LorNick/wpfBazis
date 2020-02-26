using System;
using System.Data;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Справочник Отделений стационара</summary>
    public class UserWindow_Otdel : VirtualUserWindow
    {       
        /// <summary>СВОЙСТВО Код отделения</summary>
        public string PROP_Cod { get; private set; }  
        
        /// <summary>СВОЙСТВО Наименование отделения</summary>
        public string PROP_Text { get; private set; }

        
        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Otdel()
        {
            // Имя таблицы
            PRO_TableName = "s_Otdel";
            // Заголовок
            Title = "Справочник Отделений стационара:";
            //Размеры
            MinWidth = Width;
            // Сортируем по полю Код отделения
            PRO_PoleSort = 0;
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
            return MyQuery.s_Otdel_Select_1();
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "Код", "Наименование" };
            return _mName[pIndex];
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
            }
            Close();
        }
    }
}
