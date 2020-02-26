using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Win32;
using wpfGeneral.UserControls;
using wpfGeneral.UserModul;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfGeneral.UserWindows
{
    /// <summary>КЛАСС Виртуальной формы справочников</summary>
    public partial class UserWindow_Test
    {           
        //#region ---- СВОЙСТВО ----
        /////// <summary>СВОЙСТВО Дата создания DateUp</summary>
        ////public DateTime? PROP_DateUp
        ////{
        ////    get { return PART_DateUp.PROP_Date; }
        ////    set { PART_DateUp.PROP_Date = value; }
        ////}
        //#endregion	

		/// <summary>КОНСТРУКТОР</summary>
        public UserWindow_Test()
        {
            InitializeComponent();   
            

        }

        private void UserWindows_Loaded(object sender, RoutedEventArgs e)
        {
            PART_JsonForm.MET_Inizial();
        }


        /// <summary>МЕТОД Проверяем доступность данного окна текущему пользователю</summary>        
        public static bool MET_Access()
        {
            if (!MyGlo.Admin)
            {
                MessageBox.Show("У вас нет доступа.");
                return false;
            }
            return true;
        }
    }                                
}
