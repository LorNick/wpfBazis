using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Справочник картинок</summary>
    public class UserWindow_Image : VirtualUserWindow
    {
        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Image()
        {
            // Имя таблицы
            PRO_TableName = "s_ListImage";
            // Открываем кнопки редактирования
            PROP_FlagButtonEdit = true;
            // Заголовок
            Title = "Коллекция картинок:";
            // Подсказка в строке поиска
            PART_TextBox.WatermarkContent = "Введите части нормера шаблона и/или комментария";
            //Размеры
            MinWidth = Width;
            // Сортируем по полю Варианты ответа
            PRO_PoleSort = 0;
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
            return MyQuery.s_ListImage_Select_1();
        }

        /// <summary>МЕТОД Меняем Наименование колонок на более читаемые</summary>
        protected override string MET_Header(int pIndex)
        {
            string[] _mName = { "", "Код", "№ Шаблона", "Номер", "Имя", "Рисунок", "Коментарий" };
            return _mName[pIndex];
        }

        /// <summary>МЕТОД Устанавливаем Ширину колонок</summary>
        protected override void MET_WithColumn()
        {
            PART_DataGrid.Columns[4].Width = 140;    // Имя
            PART_DataGrid.Columns[5].Width = 250;    // Рисунок
        }

        /// <summary>МЕТОД Удаляем ненужные столбцы</summary>
        protected override void MET_RemoveAt()
        {
            PART_DataGrid.Columns.RemoveAt(0);
        }

        /// <summary>МЕТОД Сохраняем данные справочника</summary>
        /// <param name="pStrValue">Данные которые сохраняем</param>
        /// <param name="pRow">Редактируемая строка</param>
        /// <param name="pColumn">Редактируемый столбец</param>
        /// <remarks>Не реализованно</remarks>
        protected override bool MET_SqlEdit(DataRow pRow, string pStrValue, DataGridColumn pColumn)
        {           
            return true;
        }

        /// <summary>СОБЫТИЕ Сохраняем файл с рисунком в байтовый массив</summary>
        protected override void PART_ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog _OpenFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "Рисунки",
                DefaultExt = "*.jpg *.png *.ico",
                Filter = "Рисунки (*.jpg *.png *.ico)|*.jpg"
            };
            if (_OpenFileDialog.ShowDialog() == true)
            {
                // Находим запись
                DataRowView _Row = (DataRowView)PART_DataGrid.SelectedItem;
                int _Cod = Convert.ToInt32(_Row["Cod"]);        // код ответа
                int _ID = Convert.ToInt32(_Row["ID"]);          // код шаблона
                int _Nomer = Convert.ToInt32(_Row["Nomer"]);    // номер
                string _NameImage = _OpenFileDialog.SafeFileName;       // имя файла
                string _Comment = Convert.ToString(_Row["Comment"]);        // коментарий
                _Row["NameImage"] = _NameImage;
                byte[] _mPhoto = MET_GetPhoto(_OpenFileDialog.FileName);
                _Row["Image"] = _mPhoto;
                // ---- Update Если есть код ответа, значить строка старая и меняем ответ
                if (_Row["Cod"].ToString() != "")
                {
                    MySql.MET_QueryNoImage(MyQuery.s_ListImage_Update_1(_Cod, _ID, _Nomer, _NameImage, _Comment), "@Image", _mPhoto);
                }
                else
                // ----- Insert Находим максимальный код
                {
                    _Cod = MySql.MET_QueryInt(MyQuery.s_ListImage_Select_2(_ID, _Nomer)) + 1;
                    if (_Cod == 1)
                        _Cod = _ID * 1000 + _Nomer * 100 + 1;     // если это первый ответ в шаблоне то начинаем с нужного номера
                    // ---- Добавляем ответ в базу
                    MySql.MET_QueryNoImage(MyQuery.s_ListImage_Insert_1(_Cod, _ID, _Nomer, _NameImage, _Comment), "@Image", _mPhoto);
                    // Добавляем данные в sqlDs
                    _Row["Cod"] = _Cod;
                    MyGlo.DataSet.Tables[PRO_TableName].Rows.Add(_Row);
                }
            }
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
    }
}
