using System;
using System.Collections;
using System.Data;
using wpfGeneral;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfMList
{
    /// <summary>КЛАСС Стартовый модуль Списочных протоколов</summary>
    public class UserModul_List : VirtualModul
    {
        #region ---- Public ----
        /// <summary>Тип протоколов</summary>
        public static MyTipProtokol PUB_TipProtokol;
        /// <summary>Номер шаблона</summary>
        public static int PUB_NumShablona;  

        /// <summary>Строка текущего Шаблона из ListShablon</summary>
        public static DataRow PUB_RowListShablon;      
        /// <summary>Таблица с Шаблоном</summary>
        public static DataTable PUB_TableShablon;
        /// <summary>Таблица с Протоколами</summary>
        public static DataTable PUB_TableProtokol;

        /// <summary>Формат таблицы из ListShablon</summary>
        public static MyFormat PUB_ListFormat; 
        /// <summary>Формат таблицы из Shablon</summary>
        public static Hashtable HashFormat; 
        #endregion

        /// <summary>МЕТОД Считываем параметры командной строки</summary>
        public override void MET_ComStr()
		{
            PUB_TipProtokol = new MyTipProtokol(eTipDocum.Kdl);
            PUB_NumShablona = 20000;


            String[] _mArgs = Environment.GetCommandLineArgs();
            for (int x = 0; x < _mArgs.Length; x++)
            {
                // Нулевая строка - путь к программе
                // 1, 2, 3 заполняются в MyGlo
                if (x == 4)
                {
                    int _Tip = MyMet.MET_ParseInt(_mArgs[4]);
                    // Проверяем, есть ли у нас такой тип
                    if (Enum.IsDefined(typeof(eTipDocum), (eTipDocum)_Tip))
                        PUB_TipProtokol = new MyTipProtokol((eTipDocum)_Tip);
                }
                if (x == 5)
                    PUB_NumShablona = MyMet.MET_ParseInt(_mArgs[5]);
            }
		}

		/// <summary>МЕТОД Начальные данные</summary>
		public override void MET_NachDan()
		{   
            // Обнуляем историю болезни
            MyGlo.HashOtchet.Clear();
		}

		/// <summary>МЕТОД Заголовок программы</summary>
		public override string MET_Title()
		{   
            // Наименование модуля
             string _Title = "wpfBazis -- Список протоколов --";
            // Номер версии
            _Title += " " + MyMet.MET_Ver();
            // Показываем имя пользователя
            _Title += " (" + MyGlo.UserName + ")";
            return _Title;
		}

        /// <summary>МЕТОД Формируем дерево</summary>
        public override void MET_CreateTree()
        {
            // ВЕТКА Списки протоколов 1го типа
            VirtualNodes _Node = new UserNodes_ListOne
            {
                PROP_TipNodes = eTipNodes.EditDocum,
                Name = "eleTVItem_ListOne",
                PROP_Text = "1 протокол",
                PROP_ImageName = "mnNaznachMed",
                PROP_ParentName = ""
            };
            _Node.PROP_Docum = new UserDocument(_Node);
            _Node.PROP_Docum.PROP_Otchet = new UserOtchet_ListOne { PROP_Docum = _Node.PROP_Docum };
            _Node.MET_Inizial();
            _Node.IsSelected = true;

            // ВЕТКА Списки протоколов 2го типа
            _Node = new UserNodes_ListTwo
            {
                PROP_TipNodes = eTipNodes.EditDocum,
                Name = "eleTVItem_ListTwo",
                PROP_Text = "2 протокол",
                PROP_ImageName = "mnNaznachMed",
                PROP_ParentName = ""
            };
            _Node.PROP_Docum = new UserDocument(_Node);
            _Node.PROP_Docum.PROP_Otchet = new UserOtchet_ListTwo { PROP_Docum = _Node.PROP_Docum };
            _Node.MET_Inizial();    
        }
    }
}
