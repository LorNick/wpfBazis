using System;
using wpfGeneral.UserModul;
using wpfStatic;

namespace wpfMKancReg
{
    /// <summary>КЛАСС Канцер регистра</summary>
    public class UserModul_KancerReg : VirtualModul
    {
        /// <summary>Код посещения в стационар</summary>
        public static decimal IND;
       
        /// <summary>МЕТОД Считываем параметры командной строки</summary>
        public override void MET_ComStr()
        {
            MyGlo.IND = 98413453655037;

            String[] args = Environment.GetCommandLineArgs();
            for (int x = 0; x < args.Length; x++)
            {
                // Нулевая строка - путь к программе
                // 1, 2, 3 заполняются в MyGlo
                if (x == 4)
                    MyGlo.IND = Convert.ToDecimal(args[4]);               
            }
        }

        /// <summary>МЕТОД Начальные данные</summary>
        public override void MET_NachDan()
        {
            // Заполняем hasAPSTAC
            MyGlo.HashAPSTAC = MySql.MET_QueryHash(MyQuery.APSTAC_Select_1(MyGlo.IND));
            // Код пациента
            MyGlo.KL = (decimal)MyGlo.HashAPSTAC["KL"];
            // Номер стац. карты
            MyGlo.NSTAC = (int)MyGlo.HashAPSTAC["NSTAC"];
            // Отделение
            MyGlo.Otd = (int)MyGlo.HashAPSTAC["otd"];
            // Заполняем hasKBOL
            MyGlo.HashKBOL = MySql.MET_QueryHash(MyQuery.kbol_Select_1(MyGlo.KL));
            // ФИО пациента
            MyGlo.FIO = Convert.ToString(MyGlo.HashKBOL["FIO"]);
            // Дата рождения пациента
            MyGlo.DR = MyMet.MET_StrDat(MyGlo.HashKBOL["DR"]);
            // Пол пациента
            MyGlo.Pol = Convert.ToInt16(MyGlo.HashKBOL["POL"]) == 1 ? "Мужской" : "Женский";
            // Дата, когда положили пациента в стационар
            MyGlo.DatePriem = MyMet.MET_StrDat(MyGlo.HashAPSTAC["DN"]);
            // Полная строка диагноза
            MyGlo.DiagStac = Convert.ToString(MyGlo.HashAPSTAC["D"]);
            // Обнуляем историю болезни
            MyGlo.HashOtchet.Clear();

            IND = MyGlo.IND;
        }

        /// <summary>МЕТОД Заголовок программы</summary>
        public override string MET_Title()
        {  
            // Наименование модуля
            string _Title = "wpfBazis -- Канцер Регистр --";
            // Номер версии
            _Title += " " + MyMet.MET_Ver();
            // Отделение 
            _Title += $" ({MyMet.MET_NameOtd()})";
            // Пациент
            _Title += "  (" + MyGlo.FIO + " " + MyGlo.DR;
            // Показываем имя пользователя
            _Title += " - " + MyGlo.UserName + ")";
            return _Title;
        }
    }
}
