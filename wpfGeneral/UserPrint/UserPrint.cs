using System;
using System.Printing;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace wpfGeneral.UserPrints
{
    /// <summary>КЛАСС для печати</summary>
    public class UserPrint : DocumentPaginator
    {
        /// <summary>Разделитель на страницы</summary>
        private readonly DocumentPaginator PRI_DocPag;
        /// <summary>Начальная страница</summary>
        private readonly int PRI_StartIndex;
        /// <summary>Конечная страница</summary>
        private readonly int PRI_EndIndex;

        #region ---- СВОЙСТВА ----
        /// <summary>СВОЙСТВО Получение значения,
        /// указывающего на то, является ли PageCount общим числом страниц,
        /// при переопределении в производном классе. (название не менять)</summary>
        public override bool IsPageCountValid => PRI_DocPag.IsPageCountValid;

        /// <summary>СВОЙСТВО Номер старницы</summary>
        public override int PageCount
        {
            get
            {
                if (PRI_StartIndex > PRI_DocPag.PageCount - 1)
                    return 0;
                if (PRI_StartIndex > PRI_EndIndex)
                    return 0;
                return PRI_EndIndex - PRI_StartIndex + 1;
            }
        }

        /// <summary>СВОЙСТВО Размеры страницы</summary>
        public override Size PageSize
        {
            get { return PRI_DocPag.PageSize; }
            set { PRI_DocPag.PageSize = value; }
        }

        /// <summary>СВОЙСТВО Источник</summary>
        public override IDocumentPaginatorSource Source => PRI_DocPag.Source;
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pFlowDoc">Сформированный документ</param>
        /// <param name="pPadding">Отступы полей (по умолчанию есть поля - 1, нету полей - 0</param>
        public UserPrint(FlowDocument pFlowDoc, byte pPadding)
        {
            // Отступы
            switch (pPadding)
            {
                case 0:
                    // Отступов нету
                    pFlowDoc.PagePadding = new Thickness(0);
                    break;
                case 1:
                    // Стандартные отступы
                    pFlowDoc.PagePadding = new Thickness(94, 56, 72, 56);
                    break;
                case 2:
                    // Минимальные отступы
                    pFlowDoc.PagePadding = new Thickness(40, 40, 40, 40);
                    break;
            }
           // pFlowDoc.ColumnWidth = 200;
            // Формируем документ
            PRI_DocPag = ((IDocumentPaginatorSource)pFlowDoc).DocumentPaginator;
        }

        /// <summary>КОНСТРУКТОР</summary>
        public UserPrint(DocumentPaginator pPaginator, PageRange pPageRange)
        {
            PRI_StartIndex = pPageRange.PageFrom - 1;
            PRI_EndIndex = pPageRange.PageTo - 1;
            PRI_DocPag = pPaginator;
            // Adjust the _endIndex
            PRI_EndIndex = Math.Min(PRI_EndIndex, PRI_DocPag.PageCount - 1);
        }

        /// <summary>МЕТОД Разбиваем документ на страницы</summary>
        /// <param name="pPageNumber">Номер страницы</param>
        public override DocumentPage GetPage(int pPageNumber)
        {
            // Размер странцы (максимальный, всё равно усекается отступами)
            PageSize = new Size(794, 1130);
            // Извлекаем запрашиваемую страницу
            DocumentPage _Page = PRI_DocPag.GetPage(pPageNumber + PRI_StartIndex);

            // Этот кусок исправляет ошибку печати с диапазоном, на некоторых принтерах
            // Create a new ContainerVisual as a new parent for page children
            var _Container = new ContainerVisual();
            if (_Page.Visual is FixedPage)
            {
                foreach (var _Сhild in ((FixedPage) _Page.Visual).Children)
                {
                    // Make a shallow clone of the child using reflection
                    var _СhildClone = (UIElement)_Сhild.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_Сhild, null);

                    // Setting the parent of the cloned child to the created ContainerVisual by using Reflection.
                    // WARNING: If we use Add and Remove methods on the FixedPage.Children, for some reason it will
                    //          throw an exception concerning event handlers after the printing job has finished.
                    var _ParentField = _СhildClone.GetType().GetField("_parent", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (_ParentField != null)
                    {
                        _ParentField.SetValue(_СhildClone, null);
                        _Container.Children.Add(_СhildClone);
                    }
                }
                return new DocumentPage(_Container, _Page.Size, _Page.BleedBox, _Page.ContentBox);
            }
            return _Page;
        }
    }

    // ========================================================================

    /// <summary>КЛАСС для отображения просмотра печати</summary>
    public class MyDocumentViewer : DocumentViewer
    {
        /// <summary>МЕТОД Печать</summary>
        protected override void OnPrintCommand()
        {
            PrintDialog _PrintDialog = new PrintDialog();
            _PrintDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            _PrintDialog.UserPageRangeEnabled = true;
            if (_PrintDialog.ShowDialog() == true)
            {
                try
                {
                    DocumentPaginator _Paginator = this.Document.DocumentPaginator;
                    if (_PrintDialog.PageRangeSelection == PageRangeSelection.UserPages)
                    {
                        _Paginator = new UserPrint(this.Document.DocumentPaginator, _PrintDialog.PageRange);
                    }
                    _PrintDialog.PrintDocument(_Paginator, "BazisWPF");
                }
                catch
                {
                    MessageBox.Show("Ошибка!");
                }
            }
        }

        /// <summary>МЕТОД Быстрая Печать</summary>
        public void MyPrintNow()
        {
            PrintDialog _PrintDialog = new PrintDialog();
            _PrintDialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
            _PrintDialog.UserPageRangeEnabled = true;
            //_PrintDialog.PrintTicket.PageOrientation = PageOrientation.Landscape;
            try
            {
                DocumentPaginator _Paginator = this.Document.DocumentPaginator;
                if (_PrintDialog.PageRangeSelection == PageRangeSelection.UserPages)
                {
                    _Paginator = new UserPrint(this.Document.DocumentPaginator, _PrintDialog.PageRange);
                }
                _PrintDialog.PrintDocument(_Paginator, "BazisWPF");
            }
            catch
            {
                MessageBox.Show("Ошибка Печати");
            }
        }
    }

}
