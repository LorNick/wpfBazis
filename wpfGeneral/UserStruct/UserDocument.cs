using System.Collections.Generic;
using wpfGeneral.UserControls;
using wpfGeneral.UserFromShablon;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using wpfStatic;
using System;

namespace wpfGeneral.UserStruct
{
    /// <summary>КЛАСС Документы</summary>
    public class UserDocument
    {
        /// <summary>СВОЙСТВО Тип вкладки 0 - без протокола, 1 - поликлиника, 2 - стационар, 3 - параклиника, 4 - КДЛ</summary>
        public eTipDocum PROP_TipDocum { get; set; }

        /// <summary>СВОЙСТВО Информация о шаблоне из таблицы ListShablon</summary>
        public UserListShablon PROP_ListShablon { get; set; }

        private List<UserShablon> PRI_Shablons;
        /// <summary>СВОЙСТВО Вопросы из таблицы Shablon</summary>
        public List<UserShablon> PROP_Shablon
        {
            get 
            {
                return PRI_Shablons ??
                       (PRI_Shablons =
                           UserShablon.MET_FactoryListShablon(PROP_ListShablon.PROP_TipProtokol.PROP_TipDocum, PROP_ListShablon.PROP_Cod));
            }
            set { PRI_Shablons = value; }
        }
     
        /// <summary>СВОЙСТВО Данные протокола из таблицы Protokol</summary>
        public UserProtokol PROP_Protokol { get; set; }

        /// <summary>СВОЙСТВО Элементы документа</summary>
        public List<UserElemet> PROP_Elements { get; set; }

        /// <summary>СВОЙСТВО Ветка</summary>
        public VirtualNodes PROP_Nodes { get; set; }

        /// <summary>СВОЙСТВО Поле Истории</summary>
        public UserPole_History PROP_UserPole_History { get; set; }

        /// <summary>СВОЙСТВО Форма Шаблона</summary>
        public VirtualFormShablon PROP_FormShablon { get; set; }

        /// <summary>СВОЙСТВО Отчет</summary>
        public VirtualOtchet PROP_Otchet { get; set; }

        /// <summary>СВОЙСТВО Может ли текущий пользователь удалить данный протокол</summary>
        /// <remarks>Разрешает удалять админу или редактору. 
        /// Обычные пользователи могут удалять только свои протоколы с pDate за неделю.
        /// Тут есть лазейки, может пересохранить документ под собой и поменть дату.</remarks>
        public bool PROP_IsUserDeleted
        {
            get
            {
                bool _Value = false;
                if (PROP_Protokol != null)
                {
                    if (MyGlo.Admin || MyGlo.FlagEdit)
                    {
                        _Value = true;
                    }
                    else
                    {
                        if (PROP_ListShablon == null)
                            PROP_ListShablon = UserListShablon.MET_FactoryListShablon(PROP_TipDocum, PROP_Protokol.PROP_NumShablon);
                        if (string.IsNullOrEmpty(PROP_ListShablon.PROP_xInfo))
                            return _Value;
                        try
                        {

                            var _J = JObject.Parse(PROP_ListShablon.PROP_xInfo);
                            _Value = _J.ContainsKey("IsUserDeleted")
                                && PROP_Protokol.PROP_xUserUp == MyGlo.User
                                && DateTime.Today.Subtract(PROP_Protokol.PROP_pDate).Days < 8;
                        }
                        catch
                        {

                        }
                    }
                }
                return _Value;
            }
        }

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pTipDocum">Тип документа (по умолчанию NULL=0, нет документа)</param> 
        public UserDocument(eTipDocum pTipDocum = eTipDocum.Null)
        {
            PROP_TipDocum = pTipDocum;
        }

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pNodes">Ссылка на ветку</param>
        /// <param name="pTipDocum">Тип документа (по умолчанию NULL=0, нет документа)</param> 
        public UserDocument(VirtualNodes pNodes, eTipDocum pTipDocum = eTipDocum.Null)
        {
            PROP_Nodes = pNodes;
            PROP_TipDocum = pTipDocum;
        }

        ///// <summary>КОНСТРУКТОР</summary>
        ///// <param name="pNodes">Ссылка на ветку</param>
        ///// <param name="pNodes">Ссылка на ветку</param>
        ///// <param name="pTipDocum">Тип документа (по умолчанию NULL=0, нет документа)</param> 
        //public UserDocument(VirtualNodes pNodes, eTipDocum pTipDocum = eTipDocum.Null)
        //{
        //    PROP_Nodes = pNodes;
        //    PROP_TipDocum = pTipDocum;
        //}
    }
}
