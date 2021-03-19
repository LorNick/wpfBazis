using System;
using System.Data;
using System.Linq;
using System.Windows;
using wpfGeneral.UserNodes;
using wpfStatic;

namespace wpfGeneral.UserOtchet
{
    /// <summary>КЛАСС Выписка из Канцер Регистра (для типа Inform)</summary>
    public class UserOtcher_InformKancerRegistr : VirtualOtchet
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
            MySql.MET_DsAdapterFill(MyQuery.MET_varRakReg_Select_2(MyGlo.KL), "KancReg");
                  
            // Номер амбулаторной карты
            xVopr = "Выписка из Канцер-Регистра";
            xAligment = 2; xParagraph = true;
            MET_Print();
            // ФИО
            xVopr = " Пациент:";
            xOtvet = MyGlo.FIO;
            xEnter = 1; 
            MET_Print();
            // Дата рождения
            xVopr = "     дата рождения:";
            xOtvet = MyGlo.DR;
            xEnter = 1; 
            MET_Print();
            // Дата смерти                                         
            xVopr = " умер:";
            xTab = 1;
            MET_RowValue("1_Паспорт", "DSmert");
            // Причина смерти
            xVopr = "        причина смерти:";
            xEnter = 1;
            MET_RowValue("1_Паспорт", "Death");
            // Дата взятия на учет в Канцер Регистр
            xVopr = "     дата взятия на учет:";
            xEnter = 1; 
            MET_RowValue("1_Паспорт", "DReg");
            // Дата снятия с учета
            xVopr = " снят с учета:";
            xTab = 1;
            MET_RowValue("1_Паспорт", "DUnReg");
            // Причина снятия с учета
            xVopr = "        причина снятия с учета:";
            xEnter = 1; 
            MET_RowValue("1_Паспорт", "Snjat", 1, 2);
            // Адрес пациента
            xVopr = " Aдрес:";
            xEnter = 1;
            MET_RowValue("1_Паспорт", "Adres");
            // ЛПУ приписки
            xVopr = " ЛПУ прикрепления:";
            xEnter = 1;
            MET_RowValue("1_Паспорт", "Lpu");
            // Клиническая группа
            xVopr = " Клиническая группа:";
            xEnter = 1;
            MET_RowValue("1_Паспорт", "KlGr", 1, 2);
            // Дата последнего наблюдения пациента
            xVopr = " Последнее наблюдение:";
            xEnter = 2; 
            MET_RowValue("1_Паспорт", "DNabl");
            // Дата последнего наблюдения пациента
            xVopr = "     состояние:";
            xEnter = 1;
            MET_RowValue("1_Паспорт", "Sost", 1, 2);
               
            // Диагноз
            int _Cod = 1;
            while (MET_IfRowCod("2_Диагноз", _Cod))
            {   
                // МКБ-10
                xVopr = " Диагноз:";
                xEnter = _Cod == 1 ? 2 : 1; 
                MET_RowValue("2_Диагноз", "MKB", _Cod);
                // Морфология
                xVopr = "     морфология:";
                xEnter = 1; 
                MET_RowValue("2_Диагноз", "Morph", _Cod);
                // Стадия
                xVopr = "     стадия:";
                xEnter = 1;
                MET_RowValue("2_Диагноз", "Stage", _Cod, 3);
                // Дата установления
                xVopr = "     дата установления:";
                xEnter = 1;
                MET_RowValue("2_Диагноз", "DDate", _Cod);

                _Cod++;
            }

            // Операции
            _Cod = 1;
            while (MET_IfRowCod("3_Операции", _Cod))
            {
                // Название операции
                xVopr = " Операция:";
                xEnter = _Cod == 1 ? 2 : 1;
                MET_RowValue("3_Операции", "Oper", _Cod);
                // Дата проведения
                xVopr = "     дата проведения:";
                xEnter = 1;
                MET_RowValue("3_Операции", "DDate", _Cod);
                // ЛПУ
                xVopr = "     место проведения:";
                xEnter = 1;
                MET_RowValue("3_Операции", "OpLpu", _Cod);
              
                _Cod++;
            }

            // Лучевое лечение
            _Cod = 1;
            while (MET_IfRowCod("4_Лучевое", _Cod))
            {
                // Дата начала
                xVopr = " Лучевое лечение с:";
                xEnter = _Cod == 1 ? 2 : 1;
                MET_RowValue("4_Лучевое", "DDate", _Cod);
                // Дата окончания
                xVopr = "  по:";
                MET_RowValue("4_Лучевое", "DKRay", _Cod);
                // Вид
                xVopr = "     вид:";
                xEnter = 1;
                MET_RowValue("4_Лучевое", "VidRay", _Cod, 3);
                // ЛПУ
                xVopr = "     место проведения:";
                xEnter = 1;
                MET_RowValue("4_Лучевое", "RayLpu", _Cod);

                _Cod++;
            }

            // Химиотерапия
            _Cod = 1;
            while (MET_IfRowCod("5_Химия", _Cod))
            {
                // Дата начала
                xVopr = " Химиотерапия с:";
                xEnter = _Cod == 1 ? 2 : 1;
                MET_RowValue("5_Химия", "DDate", _Cod);
                // Дата окончания
                xVopr = "  по:";
                MET_RowValue("5_Химия", "DKChem", _Cod);
                // Вид
                xVopr = "     вид:";
                xEnter = 1;
                MET_RowValue("5_Химия", "VidChem", _Cod, 2);
                // ЛПУ
                xVopr = "     место проведения:";
                xEnter = 1;
                MET_RowValue("5_Химия", "ChemLpu", _Cod);

                _Cod++;
            }

            // Гормоноиммуннотерапия
            _Cod = 1;
            while (MET_IfRowCod("6_Гормоны", _Cod))
            {
                // Дата начала
                xVopr = " Гормоноиммуннотерапия с:";
                xEnter = _Cod == 1 ? 2 : 1;
                MET_RowValue("6_Гормоны", "DDate", _Cod);
                // Дата окончания
                xVopr = "  по:";
                MET_RowValue("6_Гормоны", "DKHorm", _Cod);
                // Вид
                xVopr = "     вид:";
                xEnter = 1;
                MET_RowValue("6_Гормоны", "VidHorm", _Cod, 2);
                // ЛПУ
                xVopr = "     место проведения:";
                xEnter = 1;
                MET_RowValue("6_Гормоны", "HormLpu", _Cod);

                _Cod++;
            }
            
        }

        ///<summary>МЕТОД Проверяем наличие следующих данных</summary>
        /// <param name="pTip">Тип вопроса (1_Паспорт, 2_Диагноз...)</param>
        /// <param name="pCod">Номер группы (диагноза, операции...) по умолчанию 1</param>
        private bool MET_IfRowCod(string pTip, int pCod)
        {
            DataRow[] _DataRows = MyGlo.DataSet.Tables["KancReg"].Select(String.Format(@"Tip = '{0}' and Cod = {1} and NameKey = 'DDate'", pTip, pCod));
            return _DataRows.Any();
        }

        ///<summary>МЕТОД Ищем и вытаскиваем данные ответа и печатаем их в отчет</summary>
        /// <param name="pTip">Тип вопроса (1_Паспорт, 2_Диагноз...)</param>
        /// <param name="pKey">Ключ вопроса (DSmert, Death, Adres...)</param>
        /// <param name="pCod">Номер группы (диагноза, операции...) по умолчанию 1</param>
        /// <param name="pRemove">Удаляет начальные кодовые символы (01-бла бла бла, если поставить 3, удалит 01-)</param>
        private void MET_RowValue(string pTip, string pKey, int pCod = 1, int pRemove = 0)
        {
            DataRow[] _DataRows = MyGlo.DataSet.Tables["KancReg"].Select($"Tip = '{pTip}' and Cod = {pCod} and NameKey = '{pKey}'");
            if (_DataRows.Any())
            {
                PRO_RowShablon = _DataRows[0];
                xOtvet = MET_PoleStr("Value").Remove(0,  pRemove);
                MET_Print();
            }
            else
            {
                // Сбрасываем настройки на начальные
                xSize = FontSize; // размер шрифта по умолчанию
                xStyle = FontStyles.Normal; // стиль по умолчанию
                xWeight = FontWeights.Normal; // жирность по умолчанию
                xAligment = 1; // выравнивание по умолчанию
                xParagraph = false;
                xOtvet = "";
                xVopr = "";
                xTab = 0;
            }
        }
    }
}
