using System;
using System.Data;
using System.Linq;
using System.Windows;
using wpfStatic;

namespace wpfReestr
{
    /// <summary>КЛАСС Загружаем XML файл справочника Врачей МИАЦ</summary>
    public partial class MyLoadVrachMiacXML : Window
    {
        /// <summary>Имя XML файла</summary>
        private string PRI_FileName;

        /// <summary>КОНСТРУКТОР</summary>
        public MyLoadVrachMiacXML()
        {
            InitializeComponent();

            // Родительская форма (что бы окно не пропадало)
            Owner = Application.Current.MainWindow;
        }

        /// <summary>СОБЫТИЕ Выбор XML файла</summary>
        private void PART_ButtonOpenXML_Click(object sender, RoutedEventArgs e)
        {
            // Находим нужный файл
            var _DialogFileOpen = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = "XML file|spr_medspec.xml"
            };

            if (_DialogFileOpen.ShowDialog() != true) return;           
            PART_RichTextBox.AppendText($"Выбран файл: {_DialogFileOpen.FileName}\n");
            PRI_FileName = _DialogFileOpen.FileName;
            PART_ButtonLoadFile.IsEnabled = true;
        }
                     
        /// <summary>СОБЫТИЕ Нажали на кнопку Загрузить справочник врачей</summary>
        private void PART_ButtonLoadFile_Click(object sender, RoutedEventArgs e)
        {
            PART_ButtonLoadFile.IsEnabled = false;
            PART_ButtonOpenXML.IsEnabled = false;

            // Грузим данные локально
            DataSet dataSet = new DataSet();
            DataTable dataTable;
            try
            {
                // Загружаем XML файл в таблицу 0
                dataSet.ReadXml(PRI_FileName);

                dataTable = dataSet.Tables[0];
                // Удаляем всех НЕ наших врачей
                var query = dataTable.AsEnumerable().Where(r => r.Field<string>("mcod") != "555509");
                foreach (var row in query.ToList())
                    row.Delete();
                dataTable.AcceptChanges();
            }
            catch
            {
                PART_RichTextBox.AppendText("Ошибка загрузки XML файла");
                return;
            }
            // Грузим в Sql
            try
            {
                // Удаляем старые данные
                MySql.MET_QueryNo("delete Bazis.dbo.StrahVrachMIAC");

                // Копируем в Sql
                foreach (DataRow row in dataTable.Rows)                
                    if (!MET_SaveSql(row))
                        throw new Exception();
            }
            catch
            {
                PART_RichTextBox.AppendText("Ошибка копирования файла в SQL - ну т.е. всё плохо");
                return;
            }
            PART_RichTextBox.AppendText($"Загрузил {dataTable.Rows.Count} записей!");
        }

        /// <summary>МЕТОД Сохраняем запись в Sql</summary>
        /// <param name="pRow">Строка таблицы</param>
        /// <returns>true -  успешное выполнение запроса, false - что то пошло не так</returns>
        private bool MET_SaveSql(DataRow pRow)
        {
            string _Str = $@"
                insert into dbo.StrahVrachMIAC
                   (iddokt
                   ,family
                   ,name
                   ,father
                   ,vozrast
                   ,prvs
                   ,n_dok
                   ,data_n
                   ,data_e
                   )
                values
                   ('{pRow["iddokt"]}', '{pRow["family"]}', '{pRow["name"]}', '{pRow["father"]}'    
                    ,convert(date, '{pRow["vozrast"]}', 104), {pRow["prvs"]}, '{pRow["n_dok"]}'
                    ,convert(date, '{pRow["data_n"]}', 104), convert(date, '{pRow["data_e"]}', 104)
                    );";
            return MySql.MET_QueryNo(_Str);
        }
    }
}
