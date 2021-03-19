using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using wpfStatic;

namespace wpfMVrPolicl
{
    /// <summary>КЛАСС Регистратура</summary>
    public sealed partial class UserPage_Registration
    {
        /// <summary>Oбъявляем DataView для Element</summary>
        private DataView PRI_DataViewElement;
        /// <summary>Oбъявляем DataView для Date</summary>
        private DataView PRI_DataViewDate;
        /// <summary>Oбъявляем DataView для Time</summary>
        private DataView PRI_DataViewTime;
        /// <summary>Oбъявляем DataView для Talon</summary>
        private DataView PRI_DataViewTalon;
       
        /// <summary>Элемент расписания</summary>
        public int PUB_Element;
        /// <summary>Дата расписания</summary>
        public DateTime? PUB_Date = DateTime.Now;
      
        /// <summary>СВОЙСТВО Сервер 1 - главный, 2 - филиал</summary>
        public string PROP_Server
        {
            get
            {
                string _Server = PART_RadioButtonKorpus.PROP_Text == "1" ? "Pol" : "Fil";
                return _Server;
            }
        }

        /// <summary>СВОЙСТВО Подразделение 30 - главная поликлиника, 31 - филиальная поликлиника</summary>
        public int PROP_Depatment
        {
            get
            {
                int _Depatment = PART_RadioButtonKorpus.PROP_Text == "1" ? 30 : 31;
                return _Depatment;
            }
        }

        /// <summary>СВОЙСТВО Выбранный код расписания</summary>
        public int PROP_Cod
        {
            get
            {
                if (PART_DataGridTime.SelectedIndex < 0) return 0;
                // Находим запись
                DataRowView _DataRowView = (DataRowView)PART_DataGridTime.SelectedItem;
                return MyMet.MET_ParseInt(_DataRowView.Row["Cod"]);
            }
        }
              
        /// <summary>КОНСТРУКТОР</summary>
        public UserPage_Registration()
        {
            InitializeComponent();
            // Корпус
            PART_RadioButtonKorpus.PROP_Text = MyGlo.Korpus.ToString();
            // Показываем куда записан пациент
            MET_LoadTalon();
            // Загружаем Элементы
            MET_LoadElement();
            // Загружаем Даты
            MET_LoadDate();
            // Загружаем Время
            MET_LoadTime();
        }   

        /// <summary>СОБЫТИЕ Смена Корпуса</summary>
        private void PART_RadioButtonKorpus_ItemsChanged(object sender, RoutedEventArgs e)
        {
            // Если выбираем чужой корпус
            if (PART_RadioButtonKorpus.PROP_Text != MyGlo.Korpus.ToString())
            {
                // Проверяем доступ сервера друго корпуса по SQL функции Ping (зависит от установленных IP)
                if (!MySql.MET_Ping())
                {
                    // Сервер другого корпуса недоступен
                    PART_LabrlPingError.Content = "(сервер другого корпуса - не доступен)";
                    PART_RadioButtonKorpus.PROP_Text = MyGlo.Korpus.ToString();
                    return;
                }
                PART_LabrlPingError.Content = "";
            }
            // Загружаем Элементы
            MET_LoadElement();
            MyGlo.DataSet.Tables["rnDate"]?.Clear();                             // чистим даты
            MyGlo.DataSet.Tables["rnTime"]?.Clear();                             // чистим время
            // Показываем куда записан пациент
   //         MET_LoadTalon();
        }

        /// <summary>СОБЫТИЕ Выбор Элемента расписания</summary>
        private void PART_DataGridElement_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            MyGlo.DataSet.Tables["rnDate"].Clear();                             // чистим даты
            MyGlo.DataSet.Tables["rnTime"].Clear();                             // чистим время
            if (PART_DataGridElement.SelectedIndex < 0) return;                 // если элемент не выбран - выходим
            
            // Выбранная запись
            DataRowView _DataRowView = (DataRowView)PART_DataGridElement.SelectedItem;
            // Код элемента
            PUB_Element = Convert.ToInt16(_DataRowView.Row["Cod"]);
            // Загружаем Даты
            MET_LoadDate();
        }

        /// <summary>СОБЫТИЕ Выбор Даты расписания</summary>
        private void PART_DataGridDate_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            MyGlo.DataSet.Tables["rnTime"].Clear();                             // чистим время
            if (PART_DataGridDate.SelectedIndex < 0) return;                    // если ничего не выбрали - выходим

            // Выбранная запись
            DataRowView _DataRowView = (DataRowView)PART_DataGridDate.SelectedItem;
            // Наша дата
            PUB_Date = MyMet.MET_ParseDat(_DataRowView.Row["Dat"]);
            // Загружаем Время
            MET_LoadTime();
        }

        /// <summary>СОБЫТИЕ Записываем пациента на прием</summary>
        private void PART_ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            // Наш код расписания
            int _CodSetka = PROP_Cod;
            // Если не выбрали код - выходим
            if (_CodSetka == 0)
            {
                MessageBox.Show("Нужно выбрать время!", "Сосредоточтесь");
                return;
            }

            // Проверяем, есть ли свободная запись (повторно, а вдруг уже заняли)
            if (!MySql.MET_QueryBool(MyQuery.MET_RnSetka_Time_Select_2(_CodSetka, PROP_Server)))
            {
                MessageBox.Show("Пока вы думали, это время заняли!", "Задумались?");
                return;
            }

            //  Проверяем не записан ли повторно пациент в этот кабинет
            if (MySql.MET_QueryBool(MyQuery.MET_RnSetka_Time_Select_3(MyGlo.KL, PUB_Element, PROP_Server)))
            {
                MessageBox.Show("Пациент уже записан в этот кабинет!", "Зачем?");
                return;
            }
            
            // Заполняем RsTalon
            int _Cod = MySql.MET_GetNextRef(67);
            string _FIO = (string)MyGlo.HashKBOL["FAM"];                        // ФИО
            DateTime? _DR = MyMet.MET_ParseDat(MyGlo.HashKBOL["DR"]);           // ДР
            string _Rai = MyGlo.HashKBOL["KRName"].ToString();                  // Район
            if (_Rai == "") _Rai = (string)MyGlo.HashKBOL["NasPName"];          // Населенный пункт
            string _xLog = $@"{{""CrUser"": ""{MyGlo.UserName}"",""CrLPU"": ""БУЗОО КОД"",""CrProg"": ""wpfBazis"",""CrDate"": ""{DateTime.Now:yyyy-MM-dd hh:mm}""}}";

            // Сохраняем
            if (!MySql.MET_QueryNo(MyQuery.MET_RnTalon_Insert_1(_Cod, _CodSetka, MyGlo.KL, _FIO, _DR, _Rai, _xLog, PROP_Server)))
            {
                MessageBox.Show("Записать не удалось!", "Ошибка");
                return;
            }

            // Обновляем время (убрав, только что занятую позицию)
            MET_LoadTime();
            // Показываем куда записан пациент
            MET_LoadTalon();
        }                                                                                                

        /// <summary>МЕТОД Показываем куда записан пациент</summary>
        private void MET_LoadTalon()
        {
            // Запрос
            MySql.MET_DsAdapterFill(MyQuery.MET_RnSetka_Time_Select_4(MyGlo.KL, PROP_Server), "rnTalon");
            // Cоздаем DataView для нашей таблицы
            PRI_DataViewTalon = new DataView(MyGlo.DataSet.Tables["rnTalon"]);
            // Отображаем таблицу
            PART_DataGridTalon.ItemsSource = PRI_DataViewTalon;
        }

        /// <summary>МЕТОД Загружаем Элементы</summary>
        private void MET_LoadElement()
        {
            // Запрос
            MySql.MET_DsAdapterFill(MyQuery.MET_RnElement_Select_1(PROP_Server, PROP_Depatment), "rnElement");
            // Cоздаем DataView для нашей таблице
            PRI_DataViewElement = new DataView(MyGlo.DataSet.Tables["rnElement"]);
            // Отображаем таблицу
            PART_DataGridElement.ItemsSource = PRI_DataViewElement;
        }

        /// <summary>МЕТОД Загружаем Даты</summary>
        private void MET_LoadDate()
        {
            // Запрос
            MySql.MET_DsAdapterFill(MyQuery.MET_RnSetka_Date_Select_1(PUB_Element, PROP_Server), "rnDate");
            // Cоздаем DataView для нашей таблице
            PRI_DataViewDate = new DataView(MyGlo.DataSet.Tables["rnDate"]);
            // Отображаем таблицу
            PART_DataGridDate.ItemsSource = PRI_DataViewDate;
        }

        /// <summary>МЕТОД Загружаем Время</summary>
        private void MET_LoadTime()
        {
            // Запрос
            MySql.MET_DsAdapterFill(MyQuery.MET_RnSetka_Time_Select_1(PUB_Element, PUB_Date, PROP_Server), "rnTime");
            // Cоздаем DataView для нашей таблице
            PRI_DataViewTime = new DataView(MyGlo.DataSet.Tables["rnTime"]);
            // Отображаем таблицу
            PART_DataGridTime.ItemsSource = PRI_DataViewTime;
        }      
    }
}
