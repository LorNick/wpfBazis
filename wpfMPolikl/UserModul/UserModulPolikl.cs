using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wpfStatic;
using wpfGeneral;

namespace wpfMPolikl
{
    public class UserModulPolikl : VirtualModul
    {  
        /// <summary>КОНСТРУКТОР (пустой)</summary>
        public UserModulPolikl()
        {
            // Считываем параметры командной строки
            metComStr();
            // Начальные данные
            metNachDan();
        }

        /// <summary>МЕТОД Считываем параметры командной строки</summary>
        protected override void metComStr()
        {
            MyGlo.KL = 96400380579035;

            String[] args = Environment.GetCommandLineArgs();
            for (int x = 0; x < args.Length; x++)
            {
                // Нулевая строка - путь к программе
                // 1, 2, 3 заполняются в MyGlo
                if (x == 4)
                    MyGlo.KL = Convert.ToDecimal(args[4]);
            }
        }

        /// <summary>МЕТОД Начальные данные</summary>
        public override void metNachDan()
        {
            // Заполняем hasKBOL
            MyGlo.hasKBOL = MySql.metQueryHas(MyQuery.kbol_Select_1(MyGlo.KL));
            // ФИО пациента
            MyGlo.pFIO = Convert.ToString(MyGlo.hasKBOL["FIO"]);
            // Дата рождения пациента
            MyGlo.pDR = Convert.ToString(MyGlo.hasKBOL["DR"]) == "" ? "" : Convert.ToString(MyGlo.hasKBOL["DR"]).Substring(0, 10) + " г.";
            // Пол пациента
            MyGlo.pPol = Convert.ToInt16(MyGlo.hasKBOL["POL"]) == 1 ? "Мужской" : "Женский";
            // ФИО пользователя
            MyGlo.UserName = MySql.metNameSpr(Convert.ToInt16(MyGlo.User), "s_User", "Name", "Cod");
            // Обнуляем историю болезни
            MyGlo.hasOtchet.Clear();       
        }

        /// <summary>МЕТОД Заголовок программы</summary>
        public override string metTitle(string pVersion)
        {
            string locTitle;
            // Наименование модуля
            locTitle = "wpfBazis -- Врач поликлиники --";
            // Номер версии
            locTitle += " " + pVersion;
            // ФИО пациента
            locTitle += " (" + MyGlo.pFIO + " " + MyGlo.pDR + ") ";
            return locTitle;
        }
    }
}

