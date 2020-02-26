using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using wpfGeneral.UserNodes;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserControls;
using wpfStatic;

namespace wpfMVrStac
{
    /// <summary>КЛАСС Отчет Истории болезни для текущего стационара (для типа Inform)</summary>
    public class UserOtchet_HistoryStac : VirtualOtchet_History
    {
        ///<summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>
        public override VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            // Если нужно формировать отчет
            if (PROP_NewCreate)
            {
                base.MET_Inizial(pNodes);
                // Список элементов
                PRO_PoleHistory = new List<UserPole_History>();
                // Чистим
                Blocks.Clear();
                // Формируем отчет
                MET_Otchet();
                // Добавляем последний параграф в блок
                Blocks.Add(PRO_Paragraph);
                // Помечаем, что больше его формировать не надо
                PROP_NewCreate = false;
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
            var _ArrySort = _mHistories.OrderBy(x => Convert.ToDateTime(x.PROP_Dp));
            // Чистим очередь
            PRO_PoleHistory.Clear();
            // Выводим протокол на экра и запоминаем его в очередь
            foreach (var _Pole in _ArrySort)
            {
                PRO_PoleHistory.Add(_Pole);
                PRO_Paragraph.Inlines.Add(_Pole);
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
                   // _Pole.PROP_Type = eTipDocum.Stac;
                    _Pole.PROP_Type = _Node.PROP_Docum.PROP_TipDocum;
                    _Pole.PROP_Dp = _Node.PROP_Data.ToString();
                    _Pole.PROP_DocumHistory = _Node.PROP_Docum;
                    _Pole.PROP_NumerShablon = _Node.PROP_Docum.PROP_ListShablon?.PROP_Cod ?? 0;
                    // Тип протокола
                    string _Text = "";
                    if (_Node.PROP_TextDefault == _Node.PROP_Parent.PROP_TextDefault)
                    {
                        if (_Node.PROP_Parent.PROP_Parent.PROP_TextDefault != "Текущий стационар")
                            _Text = " (" + _Node.PROP_Parent.PROP_Parent.PROP_TextDefault + ")";
                    }
                    else
                        _Text = " (" + _Node.PROP_Parent.PROP_TextDefault + ")";
                    if (_Text == " (Текущий стационар)")
                         _Text = "";
                    // Название протокола
                    _Pole.PROP_Description = _Node.PROP_Data.ToString().Substring(0, 10) + " - " + _Node.PROP_TextDefault + _Text;
                    _Pole.Margin = new Thickness(5, 0, 0, 0);
                    _Pole.PROP_Background = Brushes.LightYellow;                     
                    // Находим иконку
                    _Pole.PROP_BitmapImage = (BitmapImage)_Node.PROP_ImageSource;
                    // Обязательно, если экспандер содержит текст, а не подвкладки
                    _Pole.PROP_IsTexted = true;

                    _Pole.PROP_Nodes = _Node;
                    _Pole.MET_Inicial();
                    // Делегат при открытии документа
                    _Pole.callbackOpenNew = MET_OpenOtchStac;
                    // Добавляем поле в очередь
                    PRO_PoleHistory.Add(_Pole);

                    _Pole.PROP_IsDelete = _Node.PROP_Delete;

                }
            }
        }

        /// <summary>МЕТОД Заполняем экспандер при первом открытии Отчетов Стационара</summary>
        /// <param name="pPole">Наше поле</param>
        public void MET_OpenOtchStac(UserPole_History pPole)
        {
            // Документ
            FlowDocument _FlowDocument = new FlowDocument();
            // Просмоторщик
            pPole.PUB_RichTextBox = new RichTextBox();
            RichTextBox _RichTextBox = pPole.PUB_RichTextBox;
            _RichTextBox.Document = _FlowDocument;
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
    }
}
