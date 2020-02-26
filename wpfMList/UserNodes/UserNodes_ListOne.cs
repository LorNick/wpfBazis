using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using wpfGeneral;
using wpfGeneral.UserNodes;
using wpfGeneral.UserTab;
using wpfMList.UserShablon;
using wpfStatic;

namespace wpfMList
{
    /// <summary>КЛАСС Ветка Списка протоколов 1го типа</summary>
    public class UserNodes_ListOne : VirtualNodes
    {  
        /// <summary>МЕТОД Наличие документа</summary>
        protected bool MET_PresenceProtokol()
        {  
            return false;
        }

        /// <summary>МЕТОД Отображение формы</summary>
        /// <param name="pGrid">Сюда добавляем шабллон</param> 
        /// <param name="pNew">ture - Новый шаблон, false - Старый шаблон</param> 
        /// <param name="pShablon">Номер шаблона</param>
        /// <param name="pText">Наименование шаблона (по умолчанию pMyNodes.svoText)</param>
        public override bool MET_ShowShablon(Grid pGrid, bool pNew, int pShablon = 0, string pText = "")
        {
            PROP_Docum.PROP_FormShablon = new UserFormShablon_ListOne();
            PROP_Docum.PROP_FormShablon.PUB_VirtualNodes = this;
            PROP_Docum.PROP_FormShablon.MET_CreateForm();
            PROP_Docum.PROP_FormShablon.Margin = new Thickness(0, 30, 0, 0);
            if (pGrid.Children.Count > 1)
                pGrid.Children.RemoveAt(1);
            pGrid.Children.Add(PROP_Docum.PROP_FormShablon);
            return true;
        }

        /// <summary>МЕТОД Показываем ветку (закладку)</summary>
        /// <param name="pParent">Вкладка - родитель закладки</param> 
        /// <param name="pVkladki">Тип вкладки</param> 
        /// <param name="pClose">Кнопка закрытия вкладки</param> 
        public override UserTabVrladka MET_Header(object pParent, eVkladki pVkladki, bool pClose = false)
        {
            if (PROP_Docum.PROP_FormShablon == null) return null;
            UserTabVrladka _UserTabNods = new UserTabVrladka(PROP_ImageSource, PROP_Docum.PROP_FormShablon.PUB_Text, pParent, pVkladki, pClose, PROP_TextDown);
            return _UserTabNods;
        }
    }
}
