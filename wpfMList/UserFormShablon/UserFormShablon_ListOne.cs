using System;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using wpfGeneral.UserFromShablon;
using wpfStatic;

using G = wpfMList.UserModul_List;

namespace wpfMList.UserShablon
{
    /// <summary>КЛАСС для вывода списков протокола 1го типа</summary>
    public class UserFormShablon_ListOne : VirtualFormShablon
    {
        /// <summary>МЕТОД Формируем форму Шаблона</summary>
        public override void MET_CreateForm()
        {
            //PROP_EditShablon = false;                                           // вначале сохранять нечего   
            //var _Tip = G.PUB_TipProtokol;                                       // тип протокола

            //// Загружаем строку из ListShabllon
            //if (MyGlo.DataSet.Tables[_Tip.PROP_ListShablon] != null)
            //    MyGlo.DataSet.Tables[_Tip.PROP_ListShablon].Reset();
            //// Запрос
            //MySql.MET_DsAdapterFill(MyQuery.MET_ListShablon_Select_3(G.PUB_NumShablona, _Tip.PROP_Prefix), _Tip.PROP_ListShablon);
            //G.PUB_RowListShablon = MyGlo.DataSet.Tables[_Tip.PROP_ListShablon].Rows[0];

            //PUB_Text = MyMet.MET_PoleStr("NameKr", G.PUB_RowListShablon);       // название шаблона

            //// Данные для оформления вкладки
            //PUB_VirtualNodes.PROP_ImageName = MyMet.MET_PoleStr("Icon", G.PUB_RowListShablon);
            //PUB_VirtualNodes.PROP_ImageSource = PUB_VirtualNodes.PROP_ImageSource;
            //PUB_VirtualNodes.PROP_TextDown = "Шаблон " + G.PUB_NumShablona;

            //G.PUB_ListFormat = new MyFormat(MyMet.MET_PoleStr("xFormat", G.PUB_RowListShablon));
           
            //// Загружаем Шаблон
            //if (MyGlo.DataSet.Tables[_Tip.PROP_Shablon] != null)
            //    MyGlo.DataSet.Tables[_Tip.PROP_Shablon].Reset();
            //// Запрос
            //MySql.MET_DsAdapterFill(MyQuery.MET_Shablon_Select_1(G.PUB_NumShablona, _Tip.PROP_Shablon), _Tip.PROP_Shablon);
            //G.PUB_TableShablon = MyGlo.DataSet.Tables[_Tip.PROP_Shablon];                                

            //PRO_Count = G.PUB_TableShablon.Rows.Count;                          // всего вопросов в шаблоне	
            //string _StrSql = "";
            //int _VarId;
            //G.HashFormat = new Hashtable();
            //// Перебераем все вопросы
            //for (int i = 0; i < PRO_Count; i++)
            //{
            //    try
            //    {
                    // Текущая строка шаблона 
                    //PRO_RowShablon = G.PUB_TableShablon.Rows[i];
                    //MyFormat _Format = new MyFormat(MET_PoleStr("xFormat"));
                    //// Номер индификатора VarId
                    //_VarId = MET_PoleInt("VarID");
                    //// Добавим формат вопроса из шаблона в очередь
                    //G.HashFormat.Add("VarId_"+ _VarId, _Format);
                    //// Формируем подзапрос запроса протоколов  ",dbo.GetPole(2, p.Protokol) as VarId2"
                    //if (MET_PoleInt("Type") != 15)
                    //    _StrSql += String.Format(@" ,dbo.GetPole({0}, p.Protokol) as VarId_{0} ", _VarId);
                    //else
                    //{
                    //    switch (MET_PoleStr("ValueStart"))
                    //    {
                    //        case "[pDate]":
                    //            _StrSql += String.Format(@" ,pDate as VarId_{0} ", _VarId);
                    //            break;
                    //        case "[FIO]":
                    //            _StrSql += String.Format(@" ,dbo.GetFIO(k.FAM, k.I, k.O) as VarId_{0} ", _VarId);
                    //            break;
                    //    }
                    //}
      //          }
      //          catch
      //          {
      //          }
      //      } 
            
      //      // Загружаем Протоколы
      //      if (MyGlo.DataSet.Tables[_Tip.PROP_Protokol] != null)
      //          MyGlo.DataSet.Tables[_Tip.PROP_Protokol].Reset();
      //      // Запрос
    //        MySql.MET_DsAdapterFill(MyQuery.MET_Protokol_Select_4(G.PUB_NumShablona, _StrSql), _Tip.PROP_Protokol);
      //      G.PUB_TableProtokol = MyGlo.DataSet.Tables[_Tip.PROP_Protokol];

      //      Frame _Frame = new Frame();                                         // фрейм, для  списка протоколов
      //      UserPage_ListOne _Page = new UserPage_ListOne();                    // создаем лист
      //      _Frame.Navigate(_Page);                                             // помещаем лист на фрейм
      //      _Page.PUB_Node = PUB_VirtualNodes;
      ////      PUB_HashPole = _Page.PUB_HashPole;                                  // наша коллекция полей  
      //      this.Children.Add(_Frame);                                          // добавляем фрейм на вкладку
        }
    }

    /// <summary>КЛАСС для вывода списков протокола 1го типа</summary>
    public class UserShablon_ListException : VirtualFormShablon
    {
        /// <summary>МЕТОД Формируем форму Шаблона</summary>
        public override void MET_CreateForm()
        {
            //PROP_EditShablon = false;                                           // вначале сохранять нечего
            //PRO_NomerShablon = G.PUB_NumShablona;

            //PRO_RowProtokol = G.PUB_RowProtokol;
            //PRO_StrProtokol = PRO_RowProtokol["Protokol"].ToString();

            //PRO_StackPanel.Orientation = Orientation.Horizontal;
            //VirtualPole _Date = MET_CreateUserPole(3);
            //_Date.PROP_Description = "";
            //_Date.PROP_Text = PRO_RowProtokol["pDate"].ToString();
            //_Date.PROP_HeightText = 28;
            //PRO_StackPanel.Children.Add(_Date);                                 // добавляем фрейм на вкладку

            
            //// ---- Перебераем все вопросы
            //for (int i = 0; i < 7; i++)
            //{
            //    try
            //    {
                   
            //        // Текущая строка 
            //        PRO_RowShablon = G.PUB_TableShablon.Rows[i];
            //        // Формат
            //        MyFormatPole _Format = new MyFormatPole();//MET_PoleStr("xFormat"));
            //        // Если поле нужно проппустить, то следующее поле
            //        //if (_Format.MET_If("hide"))
            //        //    continue;
            //        // Создаем поле, соответствующего типа
            //        VirtualPole _Pole = MET_CreateUserPole(MET_PoleInt("Type"));
            //        // Ссылка на форму
            //        _Pole.PROP_FormShablon = this;
            //        // Номер шаблона, как минимум нужен для Картинок
            //        _Pole.PROP_Shablon = PRO_NomerShablon;
            //        _Pole.PROP_WidthText = 120;
            //        _Pole.PROP_HeightText = 28;
            //        // Вопрос
            //        _Pole.PROP_Description = ""; //MET_PoleStr("Name");
            //        // Номер индификатора VarId
            //        _Pole.PROP_VarId = MET_PoleInt("VarID");
            //        // Формат поля
            //        //_Pole.PROP_Format = _Format;
            //        // Обязательное поле
            //        //_Pole.PROP_Necessarily = _Format.MET_If("nec");
            //        // Запрет на редактирование 
            //        if (_Format.MET_If("disab"))
            //            _Pole.IsEnabled = false;
            //        // Значение ответа по умолчанию
            //        //_Pole.PROP_DefaultText = MET_ReplaceProp(MET_PoleStr("ValueStart"), PRO_NomerShablon, _Pole.PROP_VarId);
            //        // Ответ
            //        //if (PROP_StadNodes == eStadNodes.New || (PROP_StadNodes == eStadNodes.NewPerw && _Format.MET_If("nprev")))
            //        //    // Значения по умолчанию
            //        //    _Pole.PROP_Text = _Pole.PROP_DefaultText;
            //        //else
            //            // Значения из протокола (либо eStadNodes.Old, либо eStadNodes.NewPerw)
            //            _Pole.PROP_Text = MyMet.MET_GetPole(_Pole.PROP_VarId, PRO_StrProtokol);
            //        // Если новый шаблон, то текст серый
            //        if (PROP_Now)
            //            _Pole.PROP_ForegroundText = Brushes.Gray;
            //        // Имя поля
            //        _Pole.Name = "elePoleShabl_" + _Pole.PROP_VarId;
            //        // Записываем элемент в очередь
            //        //PUB_HashPole.Add(_Pole.Name, _Pole);
            //        // Указатель на принадлежность к разделу 
            //        //int _Maska = MET_PoleInt("Maska");
            //        // Добавляем элемент в ...
            //        //if (_Maska == 0)
            //            // Добавляем элемент на форму
            //            PRO_StackPanel.Children.Add(_Pole);
            //        //else
            //        //{
            //        //    // Добавляем элемент в родительское поле
            //        //    ((VirtualPole)PUB_HashPole["elePoleShabl_" + _Maska]).MET_AddElement(_Pole);
            //        //}
            //        // Инициализация поля (если есть)
            //        //_Pole.MET_Inicial();
                    

            //    }
            //    catch { }
            //}
            

          // this.Children.Add(PRO_StackPanel); 
        }
    }

    /// <summary>КЛАСС Колонок</summary>
    public class UserColumn
    {
       
    }
}
