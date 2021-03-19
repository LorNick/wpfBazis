using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Cправочник врачей из МИАЦ (для проверки действующих сертефикатов, только наше ЛПУ 555509)</summary>
    public class UserWindow_StrahVrachMIAC : VirtualUserWindow
    {   
        /// <summary>КОНСТРУКТОР</summary>      
        public UserWindow_StrahVrachMIAC() 
        {            
            // Имя таблицы
            PRO_TableName = "StrahVrachMIAC";
            //Размеры
            MinWidth = Width;
            // Заголовок
            Title = "Cправочник врачей из МИАЦ:";
            // Подсказка в строке поиска
            PART_TextBox.WatermarkContent = "Введите код врача IDDOKT, ФИО врача, год рождения (например: АЛ Ю ДМ 82)";
            // Размеры окна
            Height = 660;
            Width = 800;
            MinWidth = Width;
            // Сортируем по полю Варианты ответа
            PRO_PoleSort = 1;
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
            return MyQuery.MET_StrahVrachMIAC_Select_1();
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "IDDOKT", "Врач", "День р.", "PRVS", "Начало", "Конец" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[1].Width = 135;
            PART_DataGrid.Columns[2].Width = 300;
            PART_DataGrid.Columns[5].Width = 85;
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
        }      
    }
}
