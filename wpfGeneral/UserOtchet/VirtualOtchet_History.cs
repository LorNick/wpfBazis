using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using wpfGeneral.UserNodes;
using wpfGeneral.UserPrints;
using wpfGeneral.UserControls;
using wpfStatic;

namespace wpfGeneral.UserOtchet
{
    /// <summary>КЛАСС Виртуальный отчет Истории болезни</summary>
    public abstract class VirtualOtchet_History : VirtualOtchet
    {
        /// <summary>Очередь элементов истории</summary>
        protected List<UserPole_History> PRO_PoleHistory;
      
        /// <summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            base.MET_Inizial(pNodes);
            return this;
        }

        /// <summary>МЕТОД Создаем объект для печати</summary>
        /// <param name="pMyDocumentViewer">Просмоторщик печати</param>
        /// <param name="pFlowDocument">Поток с нашим текстом</param>
        /// <param name="pPrintNow">Печатаем сразу</param> 
        public override void MET_CreatePrint(MyDocumentViewer pMyDocumentViewer, FlowDocument pFlowDocument, bool pPrintNow = false)
        {
            // Сбрасываем фон
            MyGlo.BrushesOtchet = null;
            // Формируем отчет (если надо)
            MET_Inizial(PROP_Nodes);
            // Новый документ
            FlowDocument _FlowDocument = new FlowDocument();
            // Ширина
            _FlowDocument.ColumnWidth = 400;
            // Перебираем все вкладки и вытаскиваем из них информацию
            foreach (UserPole_History _MyPole in PRO_PoleHistory)
                _MyPole.MET_StacPanel(_FlowDocument);                           // если есть информация, то заносим её в _FlowDocument
            // Добавляем отступы
            _FlowDocument.Blocks.FirstBlock.Padding = 
                new Thickness(Blocks.FirstBlock.Padding.Left, PROP_Otstup, Blocks.FirstBlock.Padding.Right, Blocks.FirstBlock.Padding.Bottom);
            // Рисуем эмблему
            MET_ImageEmblema(_FlowDocument);
            // Формируем объект для печати
            MET_PreiwPrint(pMyDocumentViewer, _FlowDocument, pPrintNow);
            // Возвращаем отчет на место
            pFlowDocument.Blocks.Add(this);
        }
    }
}
