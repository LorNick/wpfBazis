using wpfGeneral.UserStruct;
using wpfGeneral.UserNodes;
using wpfGeneral.UserPage;
using System.Windows.Controls;

namespace wpfGeneral.UserFormShablon
{
    /// <summary>КЛАСС работы шаблона PDF</summary>
    class UseFormShablon_PDF : VirtualFormShablon
    {
        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pDocument">Документ</param>
        public UseFormShablon_PDF(UserDocument pDocument) : base(pDocument) { }

        /// <summary>МЕТОД Инициализация Шаблона</summary>
        /// <param name="pNodes">Ветка</param>
        /// <param name="pNew">ture - Новый шаблон, false - Старый шаблон</param>
        /// <param name="pShablon">Номер шаблона, по умолчанию 0</param>
        /// <param name="pText">Наименование шаблона (по умолчанию pMyNodes.svoText)</param>
        public override VirtualFormShablon MET_Inizial(VirtualNodes pNodes, bool pNew, int pShablon = 0,
            string pText = "")
        {
            base.MET_Inizial(pNodes, pNew, pShablon, pText);
            MET_CreateForm();
            return this;
        }

        /// <summary>МЕТОД Формируем форму Шаблона</summary>
        public override void MET_CreateForm()
        {
            Frame _Frame = new Frame();                                        
            UserPage_ShablonPDF _Page = new UserPage_ShablonPDF();            
            _Frame.Navigate(_Page);                                             
            _Page.PUB_Node = PUB_VirtualNodes;
            PUB_HashPole = _Page.PUB_HashPole;                                  
            this.Children.Add(_Frame);                                         
        }
    }
}

