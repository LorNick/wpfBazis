using System;
using System.Data;
using wpfGeneral.UserControls;
using wpfGeneral.UserFormShablon;
using wpfGeneral.UserNodes;
using wpfStatic;

namespace wpfGeneral.UserPage
{
    /// <summary>КЛАСС Страница PDF документа</summary>
    /// <remarks>В принципе лишняя страница, можно было в ручную собрать в шаблоне UseFormShablon_PDF 
    /// или даже реализовать через стандартный генератор шаблона UserFormShablon_Standart,
    /// добавив PdfViewer в UserCorntorl
    /// </remarks>
    public sealed partial class UserPage_ShablonPDF
    {        
        /// <summary>Шаблон</summary>
        private VirtualFormShablon PRI_FormShablon;
        /// <summary>Полное Имя PDF файла</summary>
        private string PRI_FullFileName;
        /// <summary>Новый протокол</summary>
        private bool PRI_NewProtokol;

        /// <summary>КОНСТРУКТОР</summary>
        public UserPage_ShablonPDF()
        {
            InitializeComponent();
        }

        /// <summary>МЕТОД Инициализация Страницы шаблона</summary>
        /// <param name="pNodes">Ветка</param>
        /// <param name="pFormShablon">Редактор шаблона</param>
        /// <param name="pNewProtokol">ture - Новый протокол, false - Старый протокол</param>       
        public void MET_Inizial(VirtualNodes pNodes, VirtualFormShablon pFormShablon, bool pNewProtokol)
        {
            PRI_FormShablon = pFormShablon;
            PRI_NewProtokol = pNewProtokol;
           
            PRI_FullFileName = pNewProtokol ? (pNodes as UserNodes_RootPdf).PROP_FullFileName :(pNodes as UserNodes_AddPdf).PROP_FullFileName;
            // Если файл новый загружаем с локального диска, иначе с сервера
            MyPdf.MET_LoadPdfFile(PRI_FullFileName, PART_PdfViewer, pNewProtokol ? eServerOrLocal.Local : eServerOrLocal.Server);

            // Формируем свиток выбора типа документа
            MySql.MET_DsAdapterFill(MyQuery.MET_s_List_Select_5("PDF-kdlList", DateTime.Today), "s_List");
            PART_PdfComboBox.ItemsSource = new DataView(MyGlo.DataSet.Tables["s_List"]);
            PART_PdfComboBox.DisplayMemberPath = "Value";
            PART_PdfComboBox.SelectedValuePath = "Value";
            if (!pNewProtokol)
            {
                PART_PdfComboBox.Text = ((VirtualPole)PRI_FormShablon.PUB_HashPole["elePoleShabl_1"]).PROP_Text;
            }

            // PDF файл
            if (pNewProtokol)
                ((VirtualPole)PRI_FormShablon.PUB_HashPole["elePoleShabl_2"]).PROP_Text = MyPdf.MET_FileNameHash(PRI_FullFileName);
        }

        /// <summary>МЕТОД Сохраняем данные</summary>
        public bool MET_Save()
        {            
            // Сохраняем Pdf файл на сервер (только у нового протокола)
            if (PRI_NewProtokol)
                return MyPdf.MET_SavePdfFileOnServer(PRI_FullFileName);
            return true;
        }

        /// <summary>СОБЫТИЕ Изменили наименование документа</summary>
        private void PART_PdfComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (PART_PdfComboBox.SelectedValue == null) return;
            ((VirtualPole)PRI_FormShablon.PUB_HashPole["elePoleShabl_1"]).PROP_Text = PART_PdfComboBox.SelectedValue.ToString();
            // Активируем кнопку "Сохранить"
            MyGlo.Event_SaveShablon?.Invoke(true);
        }
    }
}