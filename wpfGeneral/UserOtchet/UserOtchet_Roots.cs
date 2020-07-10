using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using wpfGeneral.UserControls;
using wpfGeneral.UserNodes;
using wpfGeneral.UserPrints;
using wpfStatic;

namespace wpfGeneral.UserOtchet
{
    /// <summary>КЛАСС Отчет для типа Roots</summary>
    public class UserOtchet_Roots : VirtualOtchet
    {
        /// <summary>Очередь элементов истории</summary>
        protected Queue PRO_PoleHistory;

        ///<summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            // Если нужно формировать отчет
            if (PROP_NewCreate)
            {
                base.MET_Inizial(pNodes);
                // Список элементов
                PRO_PoleHistory = new Queue();
                // Чистим
                Blocks.Clear();
                // Формируем отчет
                MET_Otchet();
                // Добавляем последний параграф в блок
                Blocks.Add(PRO_Paragraph);
                // Помечаем, что больше его формировать не надо
                PROP_NewCreate = false;
                // Если всего один отчет, то сразу же его показываем
                if (PRO_PoleHistory.Count == 1)
                {
                    UserPole_History _Pole = (UserPole_History) PRO_PoleHistory.Peek();
                    // Окрываем эспандер
                    _Pole.PROP_IsExpanded = true;
                }
            }
            return this;
        }

        /// <summary>МЕТОД Формируем отчет</summary>
        protected override void MET_Otchet()
        {
            PRO_Paragraph = new Paragraph();
            // Перебераем дочерние ветки и выводим их
            MET_OtchVet(PROP_Nodes);

            // Сортировка, с помощью промежуточных массиввов
            UserPole_History[] _mHistories = new UserPole_History[PRO_PoleHistory.Count];
            PRO_PoleHistory.CopyTo(_mHistories, 0);
            var _Protokols = _mHistories.OrderBy(x => Convert.ToDateTime(x.PROP_Dp)); //.ThenBy(x => x.PROP_NumerShablon);
            // Чистим очередь
            PRO_PoleHistory.Clear();
            // Выводим протокол на экра и запоминаем его в очередь
            foreach (var _Protokol in _Protokols)
            {
                PRO_PoleHistory.Enqueue(_Protokol);
                PRO_Paragraph.Inlines.Add(_Protokol);
            }
        }

        /// <summary>МЕТОД Формируем отчет</summary>
        /// <param name="pNode">Ветка</param>
        private void MET_OtchVet(VirtualNodes pNode)
        {
            // Перебераем дочерние ветки и выводим их
            foreach (VirtualNodes _Node in pNode.Items)
            {
                // Если есть подвкладки, то перебераем их
                if (_Node.Items.Count > 0)
                    MET_OtchVet(_Node);

                // Формируем поля истории
                if (_Node.PROP_Data > DateTime.MinValue)
                {
                    // Настраиваем поле документа
                    UserPole_History _Pole = new UserPole_History();
                    _Pole.PROP_Dp = _Node.PROP_Data.ToString().Substring(0, 10);
                    _Pole.PROP_Date = _Pole.PROP_Dp;
                    _Pole.PROP_NumerShablon = _Node.PROP_shaNomerShablon;
                    _Pole.PROP_DocumHistory = _Node.PROP_Docum;
                    _Pole.PROP_Cod = (decimal?)_Node.PROP_Docum?.PROP_Protokol?.PROP_Cod ?? 0;
                    _Pole.PROP_Vrach = _Node.PROP_Docum?.PROP_Protokol?.PROP_UserName;
                    // Название протокола
                    _Pole.PROP_Document = _Node.PROP_TextDefault;
                    _Pole.Margin = new Thickness(5, 0, 0, 0);
                    _Pole.PROP_Background = Brushes.LightYellow;                    
                    _Pole.PROP_IsDelete = _Node.PROP_Docum?.PROP_Protokol?.PROP_xDelete == 1;
                    // Находим иконку
                    _Pole.PROP_BitmapImage = (BitmapImage) _Node.PROP_ImageSource;
                    // Обязательно, если экспандер содержит текст, а не подвкладки
                    _Pole.PROP_IsTexted = true;

                    _Pole.PROP_Nodes = _Node;
                    _Pole.MET_Inicial();
                    // Делегат при открытии документа
                    _Pole.callbackOpenNew = MET_OpenOtch;
                    // Добавляем поле в очередь
                    PRO_PoleHistory.Enqueue(_Pole);
                }
            }
        }

        /// <summary>МЕТОД Заполняем экспандер при первом открытии Отчетов</summary>
        /// <param name="pPole">Наше поле</param>
        public void MET_OpenOtch(UserPole_History pPole)
        {
            // Документ
            FlowDocument _FlowDocument = new FlowDocument();
            // Просмоторщик
            pPole.PUB_RichTextBox = new RichTextBox();
            RichTextBox _RichTextBox = pPole.PUB_RichTextBox;
            _RichTextBox.Document = _FlowDocument;
            // locRichTextBox.IsReadOnly = true;
            // Добавляем документ в экспандер
            pPole.MET_AddEle(_RichTextBox);
            PRO_Paragraph = new Paragraph();
            // Создаем временный поток для копирования
            using (MemoryStream _Stream = new MemoryStream())
            {
                // Выделяем область которую хотим копировать
                TextRange _TextRange = new TextRange(pPole.PROP_Nodes.PROP_Docum.PROP_Otchet.MET_Inizial(pPole.PROP_Nodes).ContentStart,
                    pPole.PROP_Nodes.PROP_Docum.PROP_Otchet.MET_Inizial(pPole.PROP_Nodes).ContentEnd);
                // Копируем текст в поток
                _TextRange.Save(_Stream, DataFormats.Xaml);
                _Stream.Position = 0;
                // Новый пораграф (что бы был отступ между документами)
                _FlowDocument.Blocks.Add(new Paragraph());
                // Выделяем концовку, куда хотим вставить текст
                _TextRange = new TextRange(_FlowDocument.ContentEnd, _FlowDocument.ContentEnd);
                // Вставляем текст в поток просмотра печати
                _TextRange.Load(_Stream, DataFormats.Xaml);
            }
            // Отключаем делегат
            pPole.callbackOpenNew = null;
        }

        ///<summary>МЕТОД Создаем объект для печати</summary>
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
                _MyPole.MET_StacPanel(_FlowDocument);  // если есть информация, то заносим её в locFlowDocument
            // Добавляем отступы
            _FlowDocument.Blocks.FirstBlock.Padding = new Thickness(Blocks.FirstBlock.Padding.Left, PROP_Otstup, Blocks.FirstBlock.Padding.Right, Blocks.FirstBlock.Padding.Bottom);
            // Рисуем эмблему
            MET_ImageEmblema(_FlowDocument);
            // Формируем объект для печати
            MET_PreiwPrint(pMyDocumentViewer, _FlowDocument, pPrintNow);
            // Возвращаем отчет на место
            pFlowDocument.Blocks.Add(this);
        }
    }
}
