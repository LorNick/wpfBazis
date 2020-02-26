using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wpfGeneral.UserStruct
{
    /// <summary>КЛАСС Дочерний Элемент документа (поле/вопрос/другой документ)</summary>
    public class UserElemet
    {
        /// <summary>СВОЙСТВО Сылка на родительский документ</summary>
        public UserDocument PROP_ParentDocument { get; set; }
    }
}
