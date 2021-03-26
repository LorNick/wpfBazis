using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Выбор шаблона исследования КДЛ и других общих документов</summary>
    public class UserWindow_Shablon_Kdl : VirtualWindow_Shablon
    {
        /// <summary>Отображать основные шаблоны = false, дополнительные шаблоны = true</summary>
        private readonly string PRI_xFormat;

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Shablon_Kdl(string pTitle, string pxFormat)
        {
            // Имя таблицы
            PRO_TableName = "kdlListShablon";
            // Заголовок
            Title = pTitle;
            //Размеры
            MinWidth = Width;
            // Профиль шаблонов
            PRI_xFormat = pxFormat;
            // Сортируем по полю Наименование
            PRO_PoleSort = 1;
            // Поле поиска
            PRO_PoleFiltr = "Filter";
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
            return MyQuery.MET_ListShablon_Select_2(PRO_SqlWhere, "kdl", PRI_xFormat);
        }
    }
}
