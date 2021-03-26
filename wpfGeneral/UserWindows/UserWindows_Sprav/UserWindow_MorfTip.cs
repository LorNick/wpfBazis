using System;
using System.Data;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Справочник Морфологических типов</summary>
    public class UserWindow_MorfTip : VirtualUserWindow
    {
        /// <summary>СВОЙСТВО Наименование морфологического типа</summary>
        public string PROP_Text { get; private set; }

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_MorfTip()
        {
            // Имя таблицы
            PRO_TableName = "s_MorfTip";
            // Заголовок
            Title = "Справочник Морфологических типов:";
            //Размеры
            MinWidth = Width;
            // Сортируем по полю Варианты ответа
            PRO_PoleSort = 0;
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.s_MorfTip_Select_1();
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "Наименование" };
            return _mName[pIndex];
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
                PROP_Text = Convert.ToString(_DataRowView.Row["Dec"]);
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
