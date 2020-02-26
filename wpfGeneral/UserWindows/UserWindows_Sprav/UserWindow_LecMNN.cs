using System;
using System.Data;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Справочник Методов ВМП</summary>
    public class UserWindow_LecMNN : VirtualUserWindow
    {
        /// <summary>СВОЙСТВО Наименование МНН препарата</summary>
        public string PROP_Value { get; private set; }


        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_LecMNN()
        {
            // Ставим Русский язык
            MyMet.MET_Lаng();

            // Имя таблицы
            PRO_TableName = "LecMNN";
            // Заголовок
            Title = "Справочник МНН Препаратов:";
            // Размеры окна
            MinWidth = Width;
            Height = 600;
            // Сортируем по полю Препарату
            PRO_PoleSort = 0;
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.s_ListDocum_Select_1("s_ListDocum", 4, 1);
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "Препарат"  };
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
                PROP_Value = Convert.ToString(_DataRowView.Row["Value"]);
                PROP_Return = true;
            }
            catch
            {
            }
            Close();
        }
    }
}
