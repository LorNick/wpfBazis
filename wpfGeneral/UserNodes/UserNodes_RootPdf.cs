using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using wpfGeneral.UserFormShablon;
using wpfGeneral.UserModul;
using wpfGeneral.UserOtchet;
using wpfGeneral.UserStruct;
using wpfStatic;

namespace wpfGeneral.UserNodes
{
    /// <summary>КЛАСС Ветка для Добавочных веток PDF документов из таблиц kdl</summary>
    public class UserNodes_RootPdf : VirtualNodes_RootsList
    {  
        /// <summary>СВОЙСТВО Полное Имя PDF файла (тут только для создания нового шаблона)</summary>
        public string PROP_FullFileName { get; set; }

        /// <summary>КОНСТРУКТОР</summary>
        public UserNodes_RootPdf()
        {
            PRO_TipNodeChild = eTipNodes.Kdl_AddPdf;
        }

        ///<summary>МЕТОД Инициализация ветки</summary>
        public override void MET_Inizial()
        {
            //  Тип протоколов
            PROP_shaTipProtokol = new MyTipProtokol(eTipDocum.Kdl);
            // Заполняем свойства базового класса
            base.MET_Inizial();            
        }

        ///<summary>МЕТОД Отчет подветки</summary>
        protected override VirtualOtchet MET_OtchetChild(UserDocument pDocument)
        {
            return new UserOtchet_AddDocum { PROP_Docum = pDocument };
        }

        /// <summary>МЕТОД Создание Текста подветки (описание ветки)</summary>
        /// <remarks>Берется из 1е поля протокола</remarks>
        protected override string MET_CreateTextNode(UserDocument userDocument)
        {
            return userDocument.PROP_Protokol.MET_GetPole(1);
        }

        ///<summary>МЕТОД Запрос на поиск подветок</summary>
        protected override IEnumerable<UserProtokol> MET_LoadProtokol()
        {
            var _p = ((VirtualModul) MyGlo.Modul).PUB_Protokol;
            var _l = ((VirtualModul) MyGlo.Modul).PUB_ListShablons;

            var _k =
                _p.Join(_l.Where(k => k.PROP_TipObsled == 2), a => a.PROP_NumShablon,
                    b => b.PROP_Cod, (a, b) => a).Where(p => p.PROP_TipProtokol.PROP_TipDocum == eTipDocum.Kdl
                                                             && p.PROP_KL == MyGlo.KL).ToList().OrderBy(p => p.PROP_pDate).ThenBy(p => p.PROP_Cod);
            return _k;
        }

        ///<summary>МЕТОД Создание подветки</summary>
        protected override UserNodes_Add MET_TypeNodesAdd()
        {
            return new UserNodes_AddPdf();
        }

        /// <summary>МЕТОД Создание нового шаблона</summary>
        /// <param name="pGrid">Сюда добавляем шаблон</param>
        /// <param name="pNewProtokol">ture - Новый протокол, false - Старый протокол</param>
        /// <param name="pShablon">Номер шаблона</param>
        /// <param name="pText">Наименование шаблона (по умолчанию pMyNodes.svoText)</param>
        public override bool MET_ShowShablon(Grid pGrid, bool pNewProtokol, int pShablon = 2000, string pText = "")
        {
            // Берем файл и проверяем, а не загружен он уже на сервер
            string _file = MyPdf.MET_OpenFileDialog();
            if (_file == "")
                return false;
            if (!MyPdf.MET_isNotExistPdfFileOnServer(_file))
                return false;
            PROP_FullFileName = _file;

            PROP_Docum.PROP_FormShablon = new UseFormShablon_PDF(PROP_Docum);
            PROP_Docum.PROP_FormShablon.MET_Inizial(this, true, pShablon, pText);
            PROP_Docum.PROP_FormShablon.Margin = new Thickness(0, 30, 0, 0);
            if (pGrid.Children.Count > 1)
                pGrid.Children.RemoveAt(1);
            pGrid.Children.Add(PROP_Docum.PROP_FormShablon);
            return true;
        }
    }
}
