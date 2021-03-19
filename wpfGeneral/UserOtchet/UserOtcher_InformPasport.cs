using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using wpfGeneral.UserNodes;
using wpfStatic;
using wpfGeneral.UserWindows;
using System.Windows;
using Newtonsoft.Json.Linq;
using wpfGeneral.UserStruct;
using System.Collections.Generic;

namespace wpfGeneral.UserOtchet
{
    /// <summary>КЛАСС Отчет Паспортных данный пациента (для типа Inform)</summary>
    public class UserOtcher_InformPasport : VirtualOtchet
    {
        ///<summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            // Если нужно формировать отчет
            if (PROP_NewCreate)
            {
                base.MET_Inizial(pNodes);
                // Формируем отчет
                MET_Otchet();
                // Добавляем последний параграф в блок
                Blocks.Add(PRO_Paragraph);
                // Помечаем, что больше его формировать не надо
                PROP_NewCreate = false;
            }
            return this;
        }

        /// <summary>МЕТОД Формируем отчет</summary>
        protected override void MET_Otchet()
        {
            // Заполняем строку данными запроса
            MySql.MET_DsAdapterFill(MyQuery.kbol_Select_2(MyGlo.KL), "Shablon");
            PRO_RowShablon = MyGlo.DataSet.Tables["Shablon"].Rows[0];

            // Номер амбулаторной карты
            xVopr = "Паспортные данные амбулаторного больного №";
            xOtvet = MET_PoleStr("NomAK");
            xAligment = 2; xParagraph = true;
            MET_Print();            
            // ФИО
            xVopr = " Пациент:";
            xOtvet = MyGlo.FIO;
            xEnter = 1; xParagraph = true;
            MET_Print();
            // Дата рождения
            xVopr = " Дата рождения:";
            string _Age = MET_PoleStr("DSmerti") == "" ? MyMet.MET_Age(DateTime.Parse(MyGlo.DR), DateTime.Now) : "";  // если не умер, то считаем сколько ему лет
            _Age = _Age == "" ? "" : "  (" + _Age + ")";
            xOtvet = MyGlo.DR + _Age;
            xEnter = 1;
            MET_Print();
            // Дата смерти
            if (MET_PoleStr("DSmerti") != "")
            {
                xVopr = " Дата смерти:";
                string _Death = MyMet.MET_Age(DateTime.Parse(MyGlo.DR), DateTime.Parse(MET_PoleDat("DSmerti")));
                _Death = _Death == "" ? "" : "  (" + _Death + ")";
                xOtvet = MET_PoleDat("DSmerti") + _Death;
                xEnter = 1;
                MET_Print();
            }
            // Адрес проживания пациента
            if (MET_PoleStr("Adres") != "")
            {
                xVopr = " Адрес:";
                xOtvet = MET_PoleStr("Adres");
                xParagraph = true;
                MET_Print();
            }
            // Домашний телефон
            if (MET_PoleStr("TelDom") != "")
            {
                xVopr = " Телефон 1:";
                xOtvet = MyMet.MET_TryPhon(MET_PoleStr("TelDom"));
                xEnter = 1;
                xParagraph = true;
                MET_Print();
            }
            // Служебный телефон
            if (MET_PoleStr("TelRab") != "")
            {
                xVopr = " Телефон 2:";
                xOtvet = MyMet.MET_TryPhon(MET_PoleStr("TelRab"));
                xEnter = 1;
                xParagraph = true;
                MET_Print();
            }            
            // Cтраховоя компания
            if (MET_PoleStr("NameSCom") != "")
            {
                xVopr = " Страховая компания:";
                xOtvet = MET_PoleStr("NameSCom");
                xParagraph = true;
                MET_Print();                
                // Регион страховой компании
                if (MET_PoleStr("NameReg") != "")
                {
                    xVopr = " регион:";
                    xOtvet = MET_PoleStr("NameReg");
                    xEnter = 1; xTab = 1;
                    MET_Print();
                }
                // Серия и номер полиса
                if (MET_PoleStr("SN") != "")
                {
                    xVopr = " серия и номер полиса:";
                    xOtvet = (MET_PoleStr("SS") == "" ? "" : MET_PoleStr("SS") + " - ") + MET_PoleStr("SN");
                    xEnter = 1; xTab = 1;
                    MET_Print();
                }
                // Срок действия полиса
                if (MET_PoleStr("UI") != "")
                {
                    xVopr = " срок действия полиса:";
                    xOtvet = MET_PoleDat("UI") + " - " + MET_PoleDat("DUI");
                    xEnter = 1; xTab = 1;
                    xParagraph = true;
                    MET_Print();
                }
            }                     
            // ЛПУ приписки
            if (MET_PoleStr("NameSCom") != "")
            {
                xVopr = " ЛПУ приписки:";
                xOtvet = MET_PoleStr("LPU");
                xParagraph = true;
                MET_Print();
            }     
            // Наименование документа удостоверяющего личность
            if (MET_PoleStr("DocName") != "")
            {
                xVopr = " Паспортные данные:";
                xOtvet = MET_PoleStr("DocName");
                xParagraph = true;
                MET_Print();
            }
            // Паспортные данные
            if (MET_PoleStr("Pasp_Ser") != "")
            {
                // Серия и номер паспорта
                xVopr = " серия и номер паспорта:";
                xOtvet = MET_PoleStr("Pasp_Ser") + " - " + MET_PoleStr("Pasp_Nom");
                xEnter = 1; xTab = 1;
                MET_Print();
                // Дата выдачи паспорта
                if (MET_PoleStr("Pasp_Kogda") != "")
                {
                    xVopr = " дата выдачи паспорта:";
                    xOtvet = MET_PoleDat("Pasp_Kogda");
                    xEnter = 1; xTab = 1;
                    MET_Print();
                }
                // Кем выдан паспорт
                if (MET_PoleStr("Pasp_Kem") != "")
                {
                    xVopr = " кем выдан:";
                    xOtvet = MET_PoleStr("Pasp_Kem");
                    xEnter = 1; xTab = 1;
                    MET_Print();
                }
            }
            // Новый параграф
            xParagraph = true;
            // Социальный статус
            if (MET_PoleStr("SocStat") != "")
            {
                xVopr = " Социальный статус:";
                xOtvet = MET_PoleStr("SocStat");
                xEnter = 1;
                MET_Print();
            }
            // Инвалидность
            if (MET_PoleInt("Inv") != 0)
            {
                xVopr = " Инвалидность:";
                string[] SocStat = { "I группа", "II группа", "III группа", "ребенок-инвалид", "", "", "снята" };
                xOtvet = SocStat[MET_PoleInt("Inv") - 1];
                xEnter = 1;
                MET_Print();
            }
            // Место работы
            if (MET_PoleStr("MRab") != "")
            {
                xVopr = " Место работы:";
                xOtvet = MET_PoleStr("MRab");
                xEnter = 1;
                MET_Print();
            }
            // Профессия
            if (MET_PoleStr("Professia") != "")
            {
                xVopr = " Профессия:";
                xOtvet = MET_PoleStr("Professia");
                xEnter = 1;
                MET_Print();
            }
            // СНИЛС
            if (MET_PoleStr("SNILS") != "")
            {
                xVopr = " СНИЛС:";
                xOtvet = MET_PoleStr("SNILS");
                xEnter = 1;
                MET_Print();
            }
            // Родители
            if (MET_PoleStr("Parents") != "")
            {
                try
                {
                    string[] _mTypeOp = { " Родитель:", " Усыновитель:", " Опекун ребенка:", " Опекун (соц. представитель):", " Попечитель:" };
                    string _Str = Convert.ToString(MET_PoleStr("Parents"));
                    int x = Convert.ToInt32(_Str[90].ToString());
                    xVopr = _mTypeOp[x - 1];
                    xOtvet = _Str.Substring(0, 30).TrimEnd(' ') +
                             _Str.Substring(29, 30).TrimEnd(' ') +
                             _Str.Substring(59, 30).TrimEnd(' ');
                    xParagraph = true;
                    MET_Print();
                }
                catch { }
            }
            // Код льготы
            if (MET_PoleStr("KatLgot") != "" & MET_PoleInt("KatLgot") != 0)
            {
                xVopr = " Код льготы:";
                xOtvet = MET_PoleStr("KatLgot");
                xParagraph = true;
                MET_Print();
                // Льготный документ
                xVopr = " документ льготы:";
                xOtvet = MET_PoleStr("NameDocLg");
                xEnter = 1; xTab = 1;
                MET_Print();
                // Серия и номер льготного документа
                xVopr = " серия и номер:";
                xOtvet = MET_PoleStr("SerNomLg");
                xEnter = 1; xTab = 1;
                MET_Print();
                // Срок действия льготы
                xVopr = " срок действия льготы:";
                xOtvet = MET_PoleDat("Ustanov") + " - " + MET_PoleDat("Pereosv");
                xEnter = 1; xTab = 1;
                MET_Print();
            }
            // Кто выдал направление в БУЗОО КОД
            if (MET_PoleInt("Napravlenie") != 0)
            {
                xVopr = " Кто выдал направление в БУЗОО КОД:";
                string[] _mSocStat = { "врач онколог", "участковый врач", "узкий специалист", "без направления" };
                xOtvet = _mSocStat[MET_PoleInt("Napravlenie") - 1];
                xParagraph = true;
                MET_Print();
            }
            // Служебная информация (только для администраторов)
            if (MyGlo.Admin)
            {
                xEnter = 1;
                MET_Print();

                // Служебная информация
                xVopr = " Служебная информация:";
                xParagraph = true;               
                MET_Print();

                // Если есть логи
                if (!string.IsNullOrEmpty(MET_PoleStr("xLog")))
                {
                    JObject _Json = JObject.Parse(MET_PoleStr("xLog"));

                    // Кто создал карту
                    xVopr = " Пользователь, создавший карту:";
                    try
                    {
                        xOtvet = MyMet.MET_UserName((int)_Json["Log"].First["User"]);
                        xOtvet += "  (" + (string)_Json["Log"].First["Date"] + ")";
                    }
                    catch (Exception ex)
                    {
                        MyGlo.PUB_Logger.Fatal(ex, "Ошибка Лога kbol");
                        return;
                    }
                    xEnter = 1; xTab = 1;
                    MET_Print();

                    // Кнопка логов
                    xEnter = 1; xTab = 1;
                    MET_Print();
                    Button _ButtonLog = new Button();

                    StackPanel _StackPanel = new StackPanel();
                    _StackPanel.Orientation = Orientation.Horizontal;

                    Image _Image = new Image();
                    _Image.Height = 20;
                    _Image.Width = 20;
                    _Image.Source = (BitmapImage)FindResource("mnLog");
                    _StackPanel.Children.Add(_Image);

                    Label _Label = new Label();
                    _Label.Content = "показать логи";
                    _StackPanel.Children.Add(_Label);

                    _ButtonLog.Content = _StackPanel;
                    _ButtonLog.ToolTip = "Показать логи изменения карточки пациента";
                    _ButtonLog.Focusable = false;
                    _ButtonLog.Tag = 1;
                    _ButtonLog.Click += delegate
                    {                       
                        // Открываем Форму Карточка Log
                        UserWindow_DocumLog _WinLog = new UserWindow_DocumLog(MET_PoleStr("xLog"));
                        _WinLog.Show();
                    };

                    PRO_Paragraph.Inlines.Add(_ButtonLog);
                }
                
                // KL
                xVopr = "  KL:";
                xOtvet = MyGlo.KL.ToString();
                xEnter = 1; xTab = 1;
                MET_Print();
            }
        }
    }
}
