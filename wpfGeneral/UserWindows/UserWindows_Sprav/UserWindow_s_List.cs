using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Таблица универнсальных ответов шаблона s_List</summary>
    public class UserWindow_s_List : VirtualUserWindow
    {
        /// <summary>Номер шаблона</summary>
        private readonly int PRI_NomerShablon;
        /// <summary>Номер вопроса</summary>
        private readonly int PRI_VarID;

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_s_List(string pTable, int pShablon, int pVarID)
        {
            // Имя таблицы
            PRO_TableName = pTable;
            // Номер шаблона
            PRI_NomerShablon = pShablon;
            // Номер вопроса
            PRI_VarID = pVarID;
            // Заголовок
            Title = "Универсальные варианты ответов из справочника s_List:";
            //Размеры
            MinWidth = Width;
            // Сортируем по полю Варианты ответа
            PRO_PoleSort = 0;
            // Показываем в подсказке
            PRO_PoleBarPanel = 1;
            // Открываем кнопки редактирования
            PROP_FlagButtonEdit = true;
            // Открываем таблицу
            MET_OpenForm();
            // Ставим фокус на сторку поиска
            PART_TextBox.Focus();
        }

        /// <summary>МЕТОД Формирование Запроса</summary>
        protected override string MET_SelectQuery()
        {
            return MyQuery.MET_List_Select_1(PRO_TableName, PRI_NomerShablon, PRI_VarID);
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "Варианты ответов" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Разрешаем (false), Запрещаяем (true) редактировать столбцы</summary>
        protected override bool MET_ReadOnly(int pIndex)
        {
            // строка с метода metHeader
            //string[] _Name = { "", "Варианты ответов" };
            bool[] _mReadOnly = { true, false };
            return _mReadOnly[pIndex];
        }

        /// <summary>МЕТОД Удаляем данные справочника</summary>
        /// <param name="pRow">Удаляемая строка</param>
        protected override bool MET_SqlDelete(DataRow pRow)
        {
            // Если нету кода, то выходим
            if (pRow["Cod"].ToString() == "")
                return false;
            if (MessageBox.Show("Вы точно хотите удалить этот ответ? \n А может он кому то нужен!", "Удалить?",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return false;
            // Код ответа
            int _Cod = Convert.ToInt32(pRow["Cod"]);
            // Удаляем
            MySql.MET_QueryNo(MyQuery.MET_List_Delete_1(PRO_TableName, _Cod));
            // Записываем в логи
            MyGlo.PUB_Logger.Info($"Удалили ответ в поле с VarId {PRI_VarID}," +
                                          $" шаблона {PRI_NomerShablon}," +
                                          $"\n таблицы {PRO_TableName}:" +
                                          $"\n {pRow["Value"]}");
            return true;
        }

        /// <summary>МЕТОД Сохраняем данные справочника</summary>
        /// <param name="pStrValue">Данные которые сохраняем</param>
        /// <param name="pRow">Редактируемая строка</param>
        /// <param name="pColumn">Редактируемый столбец</param>
        protected override bool MET_SqlEdit(DataRow pRow, string pStrValue, DataGridColumn pColumn)
        {
            int _Cod;                                                           // код ответа
            // Проверяем на наличие повторов
            // Если есть повтор, то не сохраняем
            if (MySql.MET_QueryBool(MyQuery.MET_List_Select_3(PRO_TableName, PRI_NomerShablon, PRI_VarID, pStrValue)))
            {
                MessageBox.Show("Ошибка! Данный ответ уже есть в списке ответов");
                return false;
            }
            // Update Если есть код ответа, значить строка старая и меняем ответ
            if (pRow["Cod"].ToString() != "")
            {
                _Cod = Convert.ToInt32(pRow["Cod"]);        // код ответа
                MySql.MET_QueryNo(MyQuery.MET_List_Update_1(PRO_TableName, _Cod, pStrValue));
                // Записываем в логи
                MyGlo.PUB_Logger.Info($"Изменили ответ в поле с VarId {PRI_VarID}," +
                                              $" шаблона {PRI_NomerShablon}," +
                                              $"\n таблицы {PRO_TableName}:" +
                                              $"\n {pRow["Value", DataRowVersion.Original]}");
                return true;
            }
            // Insert Находим максимальный код
            _Cod = MySql.MET_QueryInt(MyQuery.MET_List_MaxCod_Select_2(PRO_TableName, PRI_NomerShablon)) + 1;
            if (_Cod == 1)
                _Cod = PRI_NomerShablon * 1000 + 1;                             // если это первый ответ в шаблоне то начинаем с нужного номера
            // Добавляем ответ в базу
            MySql.MET_QueryNo(MyQuery.MET_List_Insert_1(PRO_TableName, _Cod, PRI_NomerShablon, PRI_VarID, pStrValue));
            // Добавляем данные в sqlDs
            pRow["Cod"] = _Cod;
            pRow["Value"] = pStrValue;
            MyGlo.DataSet.Tables[PRO_TableName].Rows.Add(pRow);
            return true;
        }
    }
}
