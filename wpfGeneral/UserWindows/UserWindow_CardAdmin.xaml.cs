using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using wpfGeneral.UserControls;
using wpfGeneral.UserModul;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Виртуальной формы справочников</summary>
    internal partial class UserWindow_CardAdmin
    {           
        #region ---- СВОЙСТВО ----
        /// <summary>СВОЙСТВО KL kbol</summary>
        public decimal PROP_KLkbol
        {
            get { return MyMet.MET_ParseDec(PART_KLkbol.PROP_Text); }
            set { PART_KLkbol.PROP_Text = value.ToString(); }
        }

        /// <summary>СВОЙСТВО Kод посещения/стационара/обследования</summary>
        public decimal PROP_IND
        {
            get { return MyMet.MET_ParseDec(PART_IND.PROP_Text); }
            set { PART_IND.PROP_Text = value.ToString(); }
        }

        /// <summary>СВОЙСТВО ФИО пациента карточки</summary>
        public string PROP_FIOkbol
        {
            get { return PART_FIOkbol.PROP_Text; }
            set { PART_FIOkbol.PROP_Text = value; }
        }

        /// <summary>СВОЙСТВО Тип Протокол/Карта</summary>
        public string PROP_Tip
        {
            get { return PART_Tip.PROP_Text; }
            set { PART_Tip.PROP_Text = value; }
        }

        /// <summary>СВОЙСТВО PoleHistory</summary>
        public UserPole_History PROP_PoleHistory { get ; set; }

        /// <summary>СВОЙСТВО xImport</summary>
        public int PROP_Import
        {
            get { return MyMet.MET_ParseInt(PART_Import.PROP_Text); }
            set { PART_Import.PROP_Text = value.ToString(); }
        }

        /// <summary>СВОЙСТВО Индекс</summary>
        public int PROP_Index
        {
            get { return MyMet.MET_ParseInt(PART_Index.PROP_Text); }
            set { PART_Index.PROP_Text = value.ToString(); }
        }

        /// <summary>СВОЙСТВО Код протокола</summary>
        public int PROP_CodProtokol
        {
            get { return MyMet.MET_ParseInt(PART_CodProtokol.PROP_Text); }
            set { PART_CodProtokol.PROP_Text = value.ToString(); }
        }

        /// <summary>СВОЙСТВО Номер шаблона</summary>
        public int PROP_NumShablon
        {
            get { return MyMet.MET_ParseInt(PART_NumShablon.PROP_Text); }
            set { PART_NumShablon.PROP_Text = value.ToString(); }
        }

        /// <summary>СВОЙСТВО Кто создал (код)</summary>
        public int PROP_UserUp
        {
            get { return MyMet.MET_ParseInt(PART_UserUp.PROP_Text); }
            set { PART_UserUp.PROP_Text = value.ToString(); }
        }

        /// <summary>СВОЙСТВО Кто создал (имя)</summary>
        public string PROP_UserUpName
        {
            get { return PART_UserUpName.PROP_Text; }
            set { PART_UserUpName.PROP_Text = value; }
        }
       
        // Тип протокола
        private MyTipProtokol PRI_MyTipProtokol;

        /// <summary>СВОЙСТВО Тип протокола</summary>
        public MyTipProtokol PROP_MyTipProtokol
        {
            get { return PRI_MyTipProtokol; }
            set
            {
                PRI_MyTipProtokol = value;
                PART_TipProtokol.PROP_Text = PRI_MyTipProtokol.PROP_TipDocum.ToString();     // сразу отображаем на экране
            }
        }

        /// <summary>СВОЙСТВО Дата pDate</summary>
        public DateTime? PROP_Date
        {
            get { return PART_pDate.PROP_Date; }
            set { PART_pDate.PROP_Date = value; }
        }

        /// <summary>СВОЙСТВО Дата создания DateUp</summary>
        public DateTime? PROP_DateUp
        {
            get { return PART_DateUp.PROP_Date; }
            set { PART_DateUp.PROP_Date = value; }
        }

        /// <summary>СВОЙСТВО Удаленный протокол</summary>
        public int PROP_xDelete
        {
            get { return MyMet.MET_ParseInt(PART_xDelete.PROP_Text); }
            set { PART_xDelete.PROP_Text = value.ToString(); }
        }

        // Строка KbolInfo
        private UserKbolInfo PRI_KbolInfo;

        /// <summary>СВОЙСТВО jTag (KbolInfo)</summary>
        public string PROP_jTag
        {
            get { return PART_jTag.PROP_Text; }
            set
            {               
                PART_jTag.PROP_Text = value;
                PRI_KbolInfo.PROP_jTag = value;
                PRI_KbolInfo.PROP_FlagChange = true;
                PART_ButtonKbolInfo.IsEnabled = true;
            }
        }

        /// <summary>СВОЙСТВО Oms (KbolInfo)</summary>
        public bool PROP_Oms
        {
            get { return PART_CheckBoxOms.PROP_Checked; }
            set
            {               
                PART_CheckBoxOms.PROP_Checked = value;   // сразу отображаем на экране
                PRI_KbolInfo.PROP_Oms = value ? 1 : 0;
                PRI_KbolInfo.PROP_FlagChange = true;
                PART_ButtonKbolInfo.IsEnabled = true;
            }
        }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        public UserWindow_CardAdmin()
        {
            InitializeComponent();

            // Меню работы с реестром
            using (var _Key = Registry.CurrentUser.OpenSubKey("Software\\wpfBazis", true))
            {

                if ((string)_Key?.GetValue("Edit") == "true")
                    PART_MenuItem_1.Header = "_Убрать доступ на Редактирование (в реестре)";
                else
                    PART_MenuItem_1.Header = "_Поставить доступ на Редактирование (в реестре)";

                if ((string)_Key?.GetValue("Admin") == "true")
                    PART_MenuItem_2.Header = "_Убрать доступ Администратора (в реестре)";
                else
                    PART_MenuItem_2.Header = "_Поставить доступ Администратора (в реестре)";
            }
        }
        
        /// <summary>СОБЫТИЕ После загрузки окна</summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            PROP_FIOkbol = MyGlo.FIO;
            UserProtokol _Protokol = null;
            if (PROP_PoleHistory.PROP_Nodes != null)                            // протоколы основной карточки
            {
                _Protokol = PROP_PoleHistory.PROP_DocumHistory?.PROP_Protokol;
            }
            else                                                                // из истории болезни
            {
                PROP_MyTipProtokol = new MyTipProtokol(PROP_PoleHistory.PROP_Type);
                if (PROP_PoleHistory.PROP_IsTexted)                               // ... если протокол
                {
                    PROP_CodProtokol = (int)PROP_PoleHistory.PROP_Cod;
                    // Протокол 
                    _Protokol = UserProtokol.MET_FactoryProtokol(PROP_MyTipProtokol.PROP_TipDocum, PROP_CodProtokol);
                }
                else                                                            // ... если карточка
                {
                    PROP_KLkbol = MyGlo.KL;
                    PROP_Date = DateTime.Parse(PROP_PoleHistory.PROP_Dp);
                    PROP_IND = PROP_PoleHistory.PROP_Cod;
                    PROP_Tip = "карта";
                }
            }    
            // Только протоколы
            if (_Protokol != null)
            {
                // Запись протокола
                PROP_CodProtokol = _Protokol.PROP_Cod;
                PROP_KLkbol = _Protokol.PROP_KL;
                // Для kdl, IND будет равен коду протокола
                PROP_IND = _Protokol.PROP_TipProtokol.PROP_TipDocum == eTipDocum.Kdl ? PROP_CodProtokol : _Protokol.PROP_CodApstac;
                PROP_Index = _Protokol.PROP_pIndex;
                PROP_NumShablon = _Protokol.PROP_NumShablon;
                PROP_Date = _Protokol.PROP_pDate;
                PROP_DateUp = _Protokol.PROP_xDateUp;
                PROP_UserUp = _Protokol.PROP_xUserUp;
                PROP_UserUpName = _Protokol.PROP_UserName;
                PROP_MyTipProtokol = _Protokol.PROP_TipProtokol;
                PROP_xDelete = _Protokol.PROP_xDelete;
                PROP_Tip = "протокол";                
            }

            PRI_KbolInfo = UserKbolInfo.MET_FactoryKbolInfo(PROP_MyTipProtokol.PROP_KbolInfo, PROP_IND, MyGlo.KL);
            if (!PRI_KbolInfo.PROP_FlagNew && PRI_KbolInfo.PROP_jTag != null)
            {               
                // Берем сразу отформатированные теги
                PROP_jTag = PRI_KbolInfo.PROP_Json.ToString();
                PROP_Oms = PRI_KbolInfo.PROP_Oms == 1;
                PART_CheckBoxOms.IsEnabled = PROP_MyTipProtokol.PROP_TipDocum == eTipDocum.Kdl;
                PART_ButtonKbolInfo.IsEnabled = false;
            }
            else
            {
                PART_StacPanelKbolInfo.IsEnabled = false;
                PROP_jTag = "Нет записи kbolInfo";
            }

            // Запрет на проверку правописания
            PART_jTag.PART_TextBox.SpellCheck.IsEnabled = false;
        }

        /// <summary>СОБЫТИЕ Сохранить строку kbolInfo</summary>
        private void PART_ButtonSaveKbolInfo_Click(object sender, RoutedEventArgs e)
        {
            if (PRI_KbolInfo.PROP_FlagChange)
            {
                if (PRI_KbolInfo.MET_UpdateSQL())
                {
                    PART_ButtonKbolInfo.IsEnabled = false;
                    PRI_KbolInfo.PROP_FlagChange = false;
                }
            }
        }

        /// <summary>СОБЫТИЕ МЕНЮ Поставить/убрать доступ на Редактирование протоколов в реестр</summary>
        private void PART_MenuItem_1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var _Key = Registry.CurrentUser.CreateSubKey("Software\\wpfBazis"))
                {
                    if ((string) _Key?.GetValue("Edit") == "true")
                    {
                        // Удаляем запись в реестре, для запрета НА редактирование
                        _Key.DeleteValue("Edit");
                        MyGlo.FlagEdit = false;
                        PART_MenuItem_1.Header = "_Поставить доступ на Редактирование (в реестре)";
                    }
                    else
                    {
                        // Добавляем запись в реестр, для разрешения НА редактриование
                        _Key?.SetValue("Edit", "true");
                        MyGlo.FlagEdit = true;
                        PART_MenuItem_1.Header = "_Убрать доступ на Редактирование (в реестре)";
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Чет нету доступа к реестру");
            }
        }

        /// <summary>СОБЫТИЕ МЕНЮ Поставить/убрать доступ на Администрирование в реестр</summary>
        private void PART_MenuItem_2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var _Key = Registry.CurrentUser.CreateSubKey("Software\\wpfBazis"))
                {
                    if ((string)_Key?.GetValue("Admin") == "true")
                    {
                        // Удаляем запись в реестре, для запрета НА редактирование
                        _Key.DeleteValue("Admin");
                        MyGlo.FlagEdit = false;
                        PART_MenuItem_2.Header = "_Поставить доступ Администратора (в реестре)";
                    }
                    else
                    {
                        // Добавляем запись в реестр, для разрешения НА редактриование
                        _Key?.SetValue("Admin", "true");
                        MyGlo.FlagEdit = true;
                        PART_MenuItem_2.Header = "_Убрать доступ Администратора (в реестре)";
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Чет нету доступа к реестру");
            }
        }

        /// <summary>СОБЫТИЕ МЕНЮ Поставить/убрать Путь к wpfBazis в реестр</summary>
        private void PART_MenuItem_3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var _Key = Registry.CurrentUser.CreateSubKey("Software\\wpfBazis"))
                {
                    // Добавляем запись Пути в реестр
                    _Key?.SetValue("Path", Assembly.GetEntryAssembly().Location);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Чет нету доступа к реестру");
            }
        }

        /// <summary>СОБЫТИЕ Нажали на галочку ОМС (только для гистологии)</summary>
        private void PART_CheckBoxOms_IsChecked(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            PROP_Oms = ((UserPole_CheckBox)sender).PROP_Checked;
        }

        /// <summary>СОБЫТИЕ Поменяли текст jTag</summary>
        private void PART_jTag_TextChanged(object sender, TextChangedEventArgs e)
        {
            PROP_jTag = ((UserPole_Text)sender).PROP_Text;
        }
    }                                
}
