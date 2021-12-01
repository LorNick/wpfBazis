using System.Windows;
using System.Windows.Controls;
using wpfGeneral.UserNodes;
using wpfStatic;

namespace wpfMVrStac
{
    /// <summary>КЛАСС Ветка для Редактируемых веток Врача Стационара</summary>
    public class UserNodes_NaznachMed : VirtualNodes
    {
        /// <summary>СВОЙСТВО (шаблона) Разрешение редактирования шаблона</summary>
        public override bool PROP_shaButtonEdit { get { return PROP_shaPresenceProtokol; } }

        /// <summary>СВОЙСТВО (шаблона) Разрешение создания шаблона</summary>
        public override bool PROP_shaButtonNew { get { return !PROP_shaPresenceProtokol; } }

        ///<summary>МЕТОД Инициализация ветки</summary>
        public override void MET_Inizial()
        {
            // Заполняем свойства базового класса
            base.MET_Inizial();
            // Наличине данных
            PROP_shaPresenceProtokol = MET_PresenceProtokol();
            // Закрываем кнопки сохранения и очистить
            PROP_shaButtonSvaveSha = Visibility.Collapsed;
            PROP_shaButtonClearSha = Visibility.Collapsed;
            // Дата создания протокола (если есть)
            if (PROP_shaPresenceProtokol)
            {
                PROP_Data = MySql.MET_QueryDat(MyQuery.lnzVrachLS_Select_3(MyGlo.IND));
                PROP_TextDown = PROP_Data.ToString().Substring(0, 10);
            }
        }

        /// <summary>МЕТОД Отображение формы</summary>
        /// <param name="pGrid">Сюда добавляем шаблон</param>
        /// <param name="pNewProtokol">ture - Новый протокол, false - Старый протокол</param>
        /// <param name="pShablon">Номер шаблона</param>
        /// <param name="pText">Наименование шаблона (по умолчанию pMyNodes.svoText)</param>
        public override bool MET_ShowShablon(Grid pGrid, bool pNewProtokol, int pShablon = 0, string pText = "")
        {
            PROP_Docum.PROP_FormShablon = new UserFormShablon_NaznachMed(PROP_Docum);
            PROP_Docum.PROP_FormShablon.MET_Inizial(this, pNewProtokol, pShablon, pText);
            PROP_Docum.PROP_FormShablon.Margin = new Thickness(0, 30, 0, 0);
            if (pGrid.Children.Count > 1)
                pGrid.Children.RemoveAt(1);
            pGrid.Children.Add(PROP_Docum.PROP_FormShablon);
            return true;
        }

        /// <summary>МЕТОД Наличие документа</summary>
        protected bool MET_PresenceProtokol()
        {
            // Находим назначения по IND
            return MySql.MET_QueryBool(MyQuery.lnzVrachLS_Select_2(MyGlo.IND));
        }
    }
}
