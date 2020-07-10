using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfStatic;

namespace wpfMVrStac
{
    /// <summary>КЛАСС Отчет Пациента в Приемном отделении (для типа Inform)</summary>
    public class UserOtcher_InformPriem : VirtualOtchet
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
            MySql.MET_DsAdapterFill(MyQuery.APSTAC_Select_2(MyGlo.IND), "Shablon");
            PRO_RowShablon = MyGlo.DataSet.Tables["Shablon"].Rows[0];

            // Номер карты стационара
            xVopr = "Карта стационара №";
            xOtvet = MET_PoleStr("NSTAC");
            xAligment = 2; xParagraph = true;
            MET_Print();
            // ФИО
            xVopr = " Пациент:";
            xOtvet = MyGlo.FIO;
            xEnter = 1; xParagraph = true;
            MET_Print();
            // Дата рождения
            xVopr = " Дата рождения:";
            xOtvet = MyGlo.DR;
            xEnter = 1;
            MET_Print();
            // Дата смерти
            if (MET_PoleStr("DSmerti") != "")
            {
                xVopr = " Дата смерти:";
                xOtvet = MET_PoleDat("DSmerti");
                xEnter = 1;
                MET_Print();
            }
            // Вид оплаты
            xVopr = " Вид оплаты:";
            string[] _mOMS = { "ОМС", "ДМС", "Платное", "Бюджетное", "Другое" };
            xOtvet = _mOMS[MET_PoleInt("OMS") - 1];
            xParagraph = true;
            MET_Print();
            // Cтраховой полис
            if (MET_PoleInt("ScomEnd") > 4 & MET_PoleInt("OMS") == 1)
            {
                xVopr = " Страховая компания:";
                xOtvet = MySql.MET_NameSpr(MET_PoleInt("ScomEnd"), "s_StrahComp");
                xEnter = 1;
                MET_Print();
                // Серия и номер полиса
                xVopr = " серия и номер полиса:";
                xOtvet = MET_PoleStr("SSEnd") + "  " + MET_PoleStr("SNEnd");
                xEnter = 1; xTab = 1;
                MET_Print();
                // Срок действия полиса
                xVopr = " срок действия полиса: ";
                xOtvet = MET_PoleDat("DBEnd") + " - " + MET_PoleDat("DEEnd");
                xEnter = 1; xTab = 1;
                MET_Print();
            }
            // Дата поступления  - выписки
            xVopr = " Дата поступления - выписки:";
            xOtvet = MET_PoleDat("DN") + " - " + MET_PoleDat("DK");
            xParagraph = true;
            MET_Print();
            // Койко дни
            if (MET_PoleInt("UET3") > 0)
            {
                xVopr = " Койко дней:";
                xOtvet = MET_PoleStr("UET3");
                xEnter = 1;
                MET_Print();
            }
            // Отделение
            xVopr = " Отделение:";
            int _Otd = MET_PoleInt("otd");
            xOtvet = $"{_Otd}. {MyMet.MET_NameOtd(_Otd)}";
            xEnter = 1;
            MET_Print();
            // Врач
            if (MET_PoleInt("KV") > 0)
            {
                xVopr = " Врач:";
                xOtvet = MySql.MET_NameSpr(MET_PoleInt("KV"), "s_VrachStac");
                xEnter = 1;
                MET_Print();
            }
            // Диагноз
            if (MET_PoleStr("D").Length > 0)
            {
                xVopr = " Диагноз:";
                xOtvet = MET_PoleStr("D") + " " + MySql.MET_NameSpr(MET_PoleStr("D"), "s_Diag");
                xEnter = 1;
                MET_Print();
            }
            // Переведен из другого отделения
            if (MET_PoleInt("FlagIn") == 2)
            {
                xVopr = " Переведен из другого отделения:";
                int _OtdIn = MET_PoleInt("OtdIn");
                xOtvet = $"{_OtdIn}. {MyMet.MET_NameOtd(_OtdIn)}";               
                xEnter = 1;
                MET_Print();
            }
            // Переведен из другого отделения
            if (MET_PoleInt("FlagOut") == 2)
            {
                xVopr = " Переведен из другого отделения:";
                int _OtdOut = MET_PoleInt("OtdOut");
                xOtvet = $"{_OtdOut}. {MyMet.MET_NameOtd(_OtdOut)}";               
                xEnter = 1;
                MET_Print();
            }
            // Вид оплаты
            if (MET_PoleInt("ISXOD") > 0)
            {
                xVopr = " Результат госпитализации:";
                string[] lISXOD = { "выздоровление", "улучшение", "без перемен", "ухудшение", "здоров", "умер" };
                xOtvet = lISXOD[MET_PoleInt("ISXOD") - 1];
                xEnter = 1;
                MET_Print();
            }
            // Причина смерти
            if (MET_PoleStr("DS1").Length > 0)
            {
                xVopr = " Причина смерти:";
                xOtvet = MET_PoleStr("DS1") + " " + MySql.MET_NameSpr(MET_PoleStr("DS1"), "s_Diag");
                xEnter = 1;
                MET_Print();
            }
            // УКЛ
            if (MET_PoleInt("UKL") > 0)
            {
                xVopr = " Коэффициент качества лечения (УКЛ):";
                xOtvet = MET_PoleStr("UKL");
                xEnter = 1;
                MET_Print();
            }
            // Данные пациента
            xVopr = " Данные пациента:";
            xParagraph = true;
            MET_Print();
            // Рост
            if (MET_PoleInt("Height") > 0)
            {
                xVopr = " Рост (см):";
                xOtvet = MET_PoleStr("Height");
                xEnter = 1; xTab = 1;
                MET_Print();
            }
            // Вес
            if (MET_PoleDec("Weight") > 0)
            {
                xVopr = " Вес (кг):";
                xOtvet = MET_PoleStr("Weight");
                xEnter = 1; xTab = 1;
                MET_Print();
            }
            // Площадь тела
            if (MET_PoleDec("Square") > 0)
            {
                xVopr = " Площадь тела (м2):";
                xOtvet = MET_PoleStr("Square");
                xEnter = 1; xTab = 1;
                MET_Print();
            }
            // Артериальное давление
            if (MET_PoleStr("Pressure").Length > 0)
            {
                xVopr = " Артериальное давление:";
                xOtvet = MET_PoleStr("Pressure");
                xEnter = 1; xTab = 1;
                MET_Print();
            }
            // Флаг на RW
            xVopr = " Флаг на RW:";
            if (MET_PoleInt("RW") > 0)
                if (MET_PoleInt("RW1") > 0)
                    xOtvet = "сдан, результат положительный";
                else
                    xOtvet = "сдан, результат отрицательный";
            else
                xOtvet = "не сдан";
            xEnter = 1; xTab = 1;
            MET_Print();
            // Анализ на ВИЧ
            xVopr = " Анализ на ВИЧ:";
            if (MET_PoleInt("VIC") > 0)
                if (MET_PoleInt("VIC1") > 0)
                    xOtvet = "сдан, результат положительный";
                else
                    xOtvet = "сдан, результат отрицательный";
            else
                xOtvet = "не сдан";
            xEnter = 1; xTab = 1;
            MET_Print();
            // Родственники
            if (MET_PoleStr("Relative").Length > 0)
            {
                xVopr = " Родственники:";
                xOtvet = MET_PoleStr("Relative");
                xEnter = 1;
                MET_Print();
                Blocks.Add(PRO_Paragraph);
            }
        }
    }
}
