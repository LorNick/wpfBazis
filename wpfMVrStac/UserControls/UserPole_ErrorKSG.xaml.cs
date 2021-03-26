using System.Data;
using System.Windows;
using System.Windows.Media;
using wpfStatic;

namespace wpfMVrStac.UserControls
{
    /// <summary>КЛАСС для поля Ошибок по КСГ</summary>
    public partial class UserPole_ErrorKSG
    {
        #region ---- СВОЙСТВО ----
        /// <summary>СВОЙСТВО Пациент</summary>
        public string PROP_FIO { get; set; }

        /// <summary>СВОЙСТВО Дата рождения</summary>
        public string PROP_DR { get; set; }

        /// <summary>СВОЙСТВО Код диагноза</summary>
        public string PROP_Diag { get; set; }

        /// <summary>СВОЙСТВО Дата поступления</summary>
        public string PROP_DN { get; set; }

        /// <summary>СВОЙСТВО Дата выписки</summary>
        public string PROP_DK { get; set; }

        /// <summary>СВОЙСТВО Койко дни</summary>
        public int PROP_Uet3 { get; set; }

        /// <summary>СВОЙСТВО Код врача</summary>
        public int PROP_VrachCod { get; set; }

        /// <summary>СВОЙСТВО Имя врача</summary>
        public string PROP_VrachName { get; set; }

        /// <summary>СВОЙСТВО Код KL пациента</summary>
        public decimal PROP_KL { get; set; }

        /// <summary>СВОЙСТВО Код APSTAC (IND карточки)</summary>
        public decimal PROP_CodApstac { get; set; }

        /// <summary>СВОЙСТВО Описание ошибки</summary>
        public string PROP_Desk { get; set; }

        /// <summary>СВОЙСТВО 1 - текущий пользователь/врач, 0 - остальные врачи этого отделения</summary>
        public int PROP_User { get; set; }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserPole_ErrorKSG(DataRow pRow )
        {
            PROP_FIO = MyMet.MET_PoleStr("FIO", pRow);
            PROP_DR = MyMet.MET_PoleDat("DR", pRow) + " г.р.";
            PROP_Diag = MyMet.MET_PoleStr("D", pRow);
            PROP_DN = "с " + MyMet.MET_PoleDat("DN", pRow) + " по";
            PROP_DK = MyMet.MET_PoleDat("DK", pRow);
            if (PROP_DK == "01.01.0001")
                PROP_DK = ". . .";
            PROP_Uet3 = MyMet.MET_PoleInt("Uet3", pRow);
            PROP_VrachCod = MyMet.MET_PoleInt("KV", pRow);
            PROP_VrachName = MyMet.MET_PoleStr("TKOD", pRow);
            PROP_KL = MyMet.MET_PoleDec("KL", pRow);
            PROP_CodApstac = MyMet.MET_PoleDec("IND", pRow);
            PROP_User = MyMet.MET_PoleInt("Us", pRow);
            PROP_Desk = MyMet.MET_PoleStr("Desk", pRow);

            InitializeComponent();

            // Помечаем личных пациетов этого врача
            if (PROP_User == 1)
            {
               PART_Border.Background = new SolidColorBrush(Colors.AntiqueWhite);
                PART_Border.BorderBrush = new SolidColorBrush(Colors.Red);
            }
        }

        /// <summary>СОБЫТИЕ Открытие контекстного меню Редактировать</summary>
        private void PART_MenuItem_Edit_Click(object sender, RoutedEventArgs e)
        {
            MyTipProtokol _MyTipProtokol = new MyTipProtokol(eTipDocum.Stac);
            // Пытаемся открыть новую копию программы, для редактирования протоколов
            MyMet.MET_EditWindows(_MyTipProtokol.PROP_TipDocum, PROP_CodApstac, PROP_KL);
        }
    }
}
