using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wpfGeneral.UserNodes;

namespace wpfMList
{
    /// <summary>КЛАСС Ветка Списка протоколов 2го типа</summary>
    public class UserNodes_ListTwo : VirtualNodes
    {  
        /// <summary>МЕТОД Наличие документа</summary>
        protected bool MET_PresenceProtokol()
        {  
            return false;
        }
    }
}
