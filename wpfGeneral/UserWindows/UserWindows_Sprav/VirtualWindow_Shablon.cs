using System;
using System.Data;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Виртуальное окно выбора Шаблонов (любых)</summary>
    public class VirtualWindow_Shablon : VirtualUserWindow
    {   
        /// <summary>Выбранный шаблон</summary>
        public int PUB_Shablon = 0;

        /// <summary>Наименование шаблона</summary>
        public string PUB_Text = "";
       

        /// <summary>КОНСТРУКТОР (пустой)</summary>
        protected VirtualWindow_Shablon()
        {
            Height = 640;
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
            PART_DataGrid.Columns.RemoveAt(0);
            PART_DataGrid.Columns.RemoveAt(0);
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "", "", "", "", "Код", "Наименование" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[4].Width = 25;
            PART_DataGrid.Columns[5].Width = 50;
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
                PUB_Shablon = Convert.ToInt16(_DataRowView.Row["Cod"]);
                PUB_Text = Convert.ToString(_DataRowView.Row["NameKr"]);
                PROP_Return = true;
            }
            catch
            {
            }
            Close();
        }
    }
}
