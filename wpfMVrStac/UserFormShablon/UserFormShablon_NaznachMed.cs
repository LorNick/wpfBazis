using System.Windows.Controls;
using wpfGeneral.UserFromShablon;
using wpfGeneral.UserNodes;
using wpfGeneral.UserStruct;

namespace wpfMVrStac
{
    /// <summary>КЛАСС для вывода Шаблона Листа Назначения Медикоментов</summary>
    public class UserFormShablon_NaznachMed : VirtualFormShablon
    {
        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pDocument">Документ</param>
        public UserFormShablon_NaznachMed(UserDocument pDocument) : base(pDocument) { }

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
            Frame _Frame = new Frame();                                         // фрейм, для  мед. назначений
            UserPage_NaznachMed _Page = new UserPage_NaznachMed();              // создаем лист мед. назначений
            _Frame.Navigate(_Page);                                             // помещаем лист мед. назначений на фрейм
            _Page.PUB_Node = PUB_VirtualNodes;        
            PUB_HashPole = _Page.PUB_HashPole;                                  // наша коллекция полей  
            this.Children.Add(_Frame);                                          // добавляем фрейм на вкладку
        } 
    }
}
