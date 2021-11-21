using System.Windows;
using System.Windows.Controls;
using wpfGeneral.UserFormShablon;
using wpfGeneral.UserModul;
using wpfGeneral.UserPage;
using wpfGeneral.UserTab;
using wpfStatic;

namespace wpfGeneral.UserNodes
{
    /// <summary>КЛАСС Виртуальная ветка для Добавочных веток</summary>
    /// <remarks>Всё что с вязанно с PDF документами (по идее должен быть отдельный класс, но получилось, что получилось)
    /// Сохраняются в таблице kdlProtokol, Номер шаблона 2000, TipObsled = 2
    /// <see cref="VirtualModul"/> Содержит раздел, для отображения корневой ветки UserNodes_RootPdf
    /// <see cref="UserNodes_RootPdf"/> Корневая ветка содержащая ветки с PDF документами UserNodes_AddPdf
    /// <see cref="UserNodes_AddPdf"/> Ветка содержащая конткретный PDF документ, отображается в главном окне MainWindow на вкладке PART_TabPdfViewer
    /// <see cref="UserPage_ShablonPDF"/> Шаблон для создания PDF документа, показывает UserPage_ShablonPDF
    /// <see cref="UserPage_ShablonPDF"/> Страница для отображения PDF документа при создании шаблона
    /// </remarks>
    public class UserNodes_AddPdf : UserNodes_Add
    {
        /// <summary>Индекс ветки (только для eTipNodes.Add)</summary>
        private int PRI_Index;

        /// <summary>СВОЙСТВО (шаблона) Разрешение редактирования шаблона</summary>
        public override bool PROP_shaButtonEdit { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Разрешение создания шаблона</summary>
        public override bool PROP_shaButtonNew => false;

        /// <summary>СВОЙСТВО Полное Имя PDF файла</summary>
        public string PROP_FullFileName { get; set; }

        /// <summary>СВОЙСТВО (шаблона) Индекс протокола</summary>
        public override int PROP_shaIndex
        {
            get { return PRI_Index; }
            set
            {
                PRI_Index = value;
                // Запоминает максимальный индекс
                if (PRI_Index > MyGlo.MaxImdex)
                    MyGlo.MaxImdex = PRI_Index;
            }
        }

        ///<summary>МЕТОД Инициализация ветки</summary>
        public override void MET_Inizial()
        {
            // Заполняем свойства базового класса
            base.MET_Inizial();

            // По умолчанию ветку можно редактировать
            PROP_shaButtonEdit = true;
            // Наличие протокола
            PROP_shaPresenceProtokol = true;
        }

        /// <summary>МЕТОД Редактируем старый шаблон</summary>
        /// <param name="pGrid">Сюда добавляем шаблон</param>
        /// <param name="pNewProtokol">ture - Новый протокол, false - Старый протокол</param>
        /// <param name="pShablon">Номер шаблона</param>
        /// <param name="pText">Наименование шаблона (по умолчанию pMyNodes.svoText)</param>
        public override bool MET_ShowShablon(Grid pGrid, bool pNewProtokol, int pShablon = 2000, string pText = "")
        {
            PROP_FullFileName =  PROP_Docum.PROP_Protokol.MET_GetPole(2);

            if (PROP_Docum.PROP_FormShablon == null)
            {
                PROP_Docum.PROP_FormShablon = new UseFormShablon_PDF(PROP_Docum);
                PROP_Docum.PROP_FormShablon.MET_Inizial(this, false, pShablon, pText);
                PROP_Docum.PROP_FormShablon.Margin = new Thickness(0, 30, 0, 0);
            }
            if (pGrid.Children.Count > 1)
                pGrid.Children.RemoveAt(1);
            pGrid.Children.Add(PROP_Docum.PROP_FormShablon);
            return true;
        }
    }
}
