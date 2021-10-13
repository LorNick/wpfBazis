using System;
using System.Data;
using System.Windows;
using System.Windows.Media.Imaging;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Редактор протоколов (показывает списко последних протоколов, выбранного шаблона)</summary>
    public class UserWindow_EditProtokol: VirtualUserWindow
    {
        ///// <summary>Выбор Типа шаблона</summary>
        //private ComboBox PRI_ComboBox_1;
        ///// <summary>Кнопка выгрузки шаблона в Excel</summary>
        //private Tel.RadButton PRI_ButtonToExcel_1;
        ///// <summary>Кнопка загрузки шаблона из Excel</summary>
        //private Tel.RadButton PRI_ButtonFromExcel_1;
        ///// <summary>Загружать шаблон сразу в SQL</summary>
        //private CheckBox PRI_CheckBox_1;
        ///// <summary>Кнопка загрузки шаблона на SQL филиала</summary>
        //private Tel.RadButton PRI_ButtonFromToFilialSQL_1

        /// <summary>Тип шаблона</summary>
        private readonly MyTipProtokol PRI_Tip;
        /// <summary>Номер шаблона</summary>
        private readonly int PRI_NumSha;

        /// <summary>Начальная дата протокола</summary>
        private readonly DateTime PRI_DN;
        /// <summary>Конечная дата протокола</summary>
        private readonly DateTime PRI_DK;
        /// <summary>Код пользователя (если есть)</summary>
        private readonly string PRI_UserCod;

        //[DllImport("User32.dll")]
        //private static extern IntPtr SetForegroundWindow(IntPtr hWnd);          // Активирует окно процесса
        //[DllImport("User32.dll")]
        //private static extern bool ShowWindow(IntPtr handle, int cmdShow);      // Отображает данное окно впереди, даже если было свернуто
        //[DllImport("user32.dll", SetLastError = true)]
        //static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_EditProtokol(MyTipProtokol pTipSha, int pNumSha, string pNameSha, string pImageSha, DateTime pDN, DateTime pDK, string pUserCod)
        {
            // Имя таблицы
            PRO_TableName = "EditProtokol";
            // Заголовок
            Title = $"Протоколы: {pNumSha} - {pNameSha}";
            // Иконка
            Icon = (BitmapImage)Application.Current.FindResource(pImageSha);
            // Тип шаблонов
            PRI_Tip = pTipSha;
            // Номер шаблона
            PRI_NumSha = pNumSha;
            // Начальная и конечная дата протоколов
            PRI_DN = pDN;
            PRI_DK = pDK;
            // Код пользователя (если есть)
            PRI_UserCod = pUserCod;
            // Размеры окна
            Width = 800;
            MinWidth = Width;
            Height = 700;
            // Сортируем по ФИО
            PRO_PoleSort = 3;
            // Разрешаем выбирать записи
            PROP_FlagButtonSelect = true;
            // Создаем фильтр
           // MET_CreateFiltr();
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.MET_Protokol_Select_4(PRI_Tip.PROP_Prefix, PRI_NumSha, PRI_DN, PRI_DK, PRI_UserCod);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "", "", "Пациент", "Дата рождения", "Дата протокола", "Код", "Пользователь" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[2].Width = 24;    // Иконка - удаление протокола
            PART_DataGrid.Columns[3].Width = 300;   // Пациент
            PART_DataGrid.Columns[6].Width = 60;    // Код Пользователя
            PART_DataGrid.Columns[7].Width = 150;   // Пользователь
        }

        /// <summary>МЕТОД Проверяем доступность данного окна текущему пользователю</summary>
        public static new bool MET_Access()
        {
            if (!MyGlo.Admin)
            {
                MessageBox.Show("У вас нет доступа.");
                return false;
            }
            return true;
        }

        /// <summary>МЕТОД Выбор данных</summary>
        protected override void MET_Select()
        {
            if (!PROP_FlagButtonSelect)
                return;
            try
            {
                DataRowView _DataRowView = (DataRowView)PART_DataGrid.SelectedItem;
                if (_DataRowView == null)
                    return;
                decimal _KL = Convert.ToDecimal(_DataRowView.Row["KL"]);
                decimal _IND = Convert.ToDecimal(_DataRowView.Row["CodApstac"]);
                // Пытаемся открыть новую копию программы, для редактирования протоколов
                MyMet.MET_EditWindows(PRI_Tip.PROP_TipDocum, _IND, _KL);
                PROP_Return = true;
            }
            catch
            {
            }
            Close();
        }
    }
}
