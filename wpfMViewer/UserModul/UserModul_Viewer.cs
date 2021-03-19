using System;
using System.Data;
using System.Linq;
using wpfGeneral.UserModul;
using wpfGeneral.UserNodes;
using wpfGeneral.UserStruct;
using wpfMViewer.UserOtchet;
using wpfStatic;

namespace wpfMViewer
{
	/// <summary>КЛАСС Модуля Работы со списком протоколов</summary>
	public class UserModul_Viewer : VirtualModul
	{   
		/// <summary>МЕТОД Считываем параметры командной строки</summary>
        public override void MET_ComStr()
		{
		    MyGlo.KL = MyGlo.DataSet.Tables["s_Users"].AsEnumerable()
                .FirstOrDefault(p => p.Field<int>("Cod") == MyGlo.User)?.Field<decimal>("KL") ?? 0;
            MyGlo.Otd = 0;
            MyGlo.IND = 1;
		   
            string[] _mArgs = Environment.GetCommandLineArgs();
            for (int x = 0; x < _mArgs.Length; x++)
            {
                // Нулевая строка - путь к программе
                // 1, 2, 3 заполняются в MyGlo
                if (x == 4)
                    MyGlo.KL = Convert.ToDecimal(_mArgs[4]);
                if (x == 5)
                    MyGlo.IND = Convert.ToDecimal(_mArgs[5]);
                if (x == 6)
                    MyGlo.Otd = Convert.ToInt32(_mArgs[6]);
                if (x == 7)
                    PUB_Menu = Convert.ToInt32(_mArgs[7]);
            }

#if DEBUG
            // Показываем меню - 1 (КОД), скрываем меню и смену пациентов = 0 (онкологи ЛПУ)    
            PUB_Menu = 1;
            MyGlo.KL = 128782448018955; // 89350339824030;
#endif
		}

		/// <summary>МЕТОД Начальные данные</summary>
		public override void MET_NachDan()
		{
            // Если нет пациента - выходим
		    if (MyGlo.KL == 0)
		        return;

            // Заполняем hasKBOL
            MyGlo.HashKBOL = MySql.MET_QueryHash(MyQuery.kbol_Select_1(MyGlo.KL));
            // Заполняем HashLastDiag
            MyGlo.HashLastDiag = MySql.MET_QueryHash(MyQuery.MET_kbolInfo_Select_3(MyGlo.KL));
            // ФИО пациента
            MyGlo.FIO = Convert.ToString(MyGlo.HashKBOL["FIO"]);
            // Дата рождения пациента
            MyGlo.DR = MyMet.MET_StrDat(MyGlo.HashKBOL["DR"]);
            // Пол пациента
            MyGlo.Pol = Convert.ToInt16(MyGlo.HashKBOL["POL"]) == 1 ? "Мужской" : "Женский"; 
            // Обнуляем историю болезни
            MyGlo.HashOtchet.Clear();
		}

		/// <summary>МЕТОД Заголовок программы</summary>
		public override string MET_Title()
		{  
            // Наименование модуля
            string _Title = "wpfBazis -- История болезни --";
            // Номер версии
            _Title += " " + MyMet.MET_Ver();

            // Пациент
            _Title += MyGlo.KL > 0 ? "  (" + MyGlo.FIO + " " + MyGlo.DR : " ( ";
            // Показываем имя пользователя
            _Title += " - " + MyGlo.UserName + ")";
            return _Title;
		}

        /// <summary>МЕТОД Формируем дерево</summary>
        public override void MET_CreateTree()
        {
            // Преварительно чистим  дерево
            MyGlo.TreeView.Items.Clear();

            if (MyGlo.KL > 0)
            {
                // Заполняем основу дерево (паспорт + история)
                base.MET_CreateTree();
            }
            else
            {
                // ВЕТКА Телефоны
                VirtualNodes _Node = new UserNodes_Inform
                {
                    Name = "elePhone",
                    PROP_Text = "Телефоны",
                    PROP_TextDefault = "Телефоны",
                    PROP_ImageName = "mnPhone",
                    PROP_ParentName = "",
                };
                _Node.PROP_Docum = new UserDocument(_Node);
                _Node.PROP_Docum.PROP_Otchet = new UserOtcher_Phone {PROP_Docum = _Node.PROP_Docum};
                _Node.MET_Inizial();
                _Node.IsSelected = true;

                // ВЕТКА Сотрудники (Только для админов)
                if (MyGlo.Admin)
                {
                    _Node = new UserNodes_Inform
                    {
                        Name = "eleStaff",
                        PROP_Text = "Сотрудники",
                        PROP_TextDefault = "Сотрудники",
                        PROP_ImageName = "mnMen",
                        PROP_ParentName = "",
                    };
                    _Node.PROP_Docum = new UserDocument(_Node);
                    _Node.PROP_Docum.PROP_Otchet = new UserOtcher_Staff { PROP_Docum = _Node.PROP_Docum };
                    _Node.MET_Inizial();
                }
            }
        }
	}
}
