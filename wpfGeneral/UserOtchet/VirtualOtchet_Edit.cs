using wpfGeneral.UserNodes;

namespace wpfGeneral.UserOtchet
{
    /// <summary>КЛАСС Виртуальный отчет для типа Inform</summary>
    public abstract class VirtualOtchet_Edit : VirtualOtchet
    {
        ///<summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            base.MET_Inizial(pNodes);
            return this;
        }
    }
}
