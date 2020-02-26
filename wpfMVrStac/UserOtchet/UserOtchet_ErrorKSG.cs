using System.Data;
using System.Windows.Documents;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfMVrStac.UserControls;
using wpfStatic;

namespace wpfMVrStac
{
    /// <summary>КЛАСС Ошибки по КСГ (не находит КСГ услуги, а только диагноза С или Д, да и то если коэффициент меньше 0.8</summary>
    public class UserOtcher_ErrorKSG : VirtualOtchet
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
            PRO_Paragraph = new Paragraph();
            // Номер карты стационара
            xVopr = "ВНИМАНИЕ! Список пациентов c \"ошибками\" текущего экономического месяца.";
            xVopr += "\n Оплата этих случаев проводиться НЕ БУДЕТ, либо будет минимальная";
            xAligment = 2;
            xSize = 18;
            MET_Print();
            xVopr += "\n (50% от минимальной оплаты).";
            xAligment = 2;
            xSize = 14;
            MET_Print();
            xVopr = "Примечание: - отображаются ВСЕ \"ошибочные\" пациенты, как врача так и текущего отделения;";
            xVopr += "\n - особо проверяйте:";
            xVopr += "\n    - ДАТУ протокола лечения/операции, она должна находиться в диапазоне стационара;";
            xVopr += "\n    - ДАТЫ выдачи препаратов/проведения лучевого лечения, они должны находиться в диапазоне стационара;";
            xVopr += "\n    - наличие препаратов (с 1го по 5й) в схеме лечения;";
            xVopr += "\n - для быстрого просмотра пациента нажмите на него правой кнопкой мыши и выберите пункт \"Редактировать\".";
            xVopr += "\n - ВНИМАНИЕ! Этот список Обновляется Каждые ПОЛЧаса.";
            xEnter = 1;
            xSize = 14;
            MET_Print();
            xParagraph = true;
            MET_Print();

            PRO_Paragraph.Inlines.Add(new LineBreak());                         // разрыв строки
            
            // Заполняем строку данными запроса
            foreach (DataRow _Row in MyGlo.DataSet.Tables["ErrorKSG"].Rows)
            {
                UserPole_ErrorKSG _Pole = new UserPole_ErrorKSG(_Row);
                PRO_Paragraph.Inlines.Add(_Pole);
            }
        }
    }
}
