using System;
using System.Collections;
using System.Data;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using wpfGeneral.UserNodes;
using wpfGeneral.UserPrints;
using wpfGeneral.UserControls;
using wpfGeneral.UserStruct;
using wpfGeneral.UserVariable;
using wpfStatic;

namespace wpfGeneral.UserOtchet
{
    /// <summary>КЛАСС Отчетов (абстрактный)</summary>
    public abstract class VirtualOtchet : Section
    {  
        /// <summary>Список полей</summary>
        public Hashtable PUB_HashPole;
       
        #region ---- Protected Field ----
        /// <summary>Записи шаблона</summary>
        protected DataRow PRO_RowShablon;
		/// <summary>Запись протокола</summary>
		protected DataRow PRO_RowProtokol; 
        /// <summary>Параграф</summary>
        protected Paragraph PRO_Paragraph;
        /// <summary>Отступ (есть/нету)</summary>
        protected bool PRO_OtstupFlag = false;
        /// <summary>Отчет, для просмотра печати</summary>
        protected bool PRO_Priew;
		
        /// <summary>Cтрока вопроса</summary>
        protected string xVopr = "";
        /// <summary>Cтрока ответа</summary>
        protected string xOtvet = "";
        /// <summary>Размер шрифта</summary>
        protected double xSize = 16;
        /// <summary>Выравнивание текста</summary>
        protected int xAligment = 1;
        /// <summary>Новый параграф</summary>
        protected bool xParagraph = false;
        /// <summary>Новая строка</summary>
        protected int xEnter = 0;
        /// <summary>Табуляции</summary>
        protected int xTab = 0;
        /// <summary>Формат строки</summary>
        protected string xFormat = "";
        /// <summary>Разрыв страницы</summary>
        protected bool xPage = false;
        /// <summary>Жирность</summary>
        protected FontWeight xWeight = FontWeights.Normal;
        /// <summary>Курсив</summary>
        protected FontStyle xStyle = FontStyles.Normal;
        #endregion

        #region ---- Properties ----
        /// <summary>СВОЙСТВО Отступ</summary>
        public double PROP_Otstup
        {
            get { return Blocks.FirstBlock.Padding.Top; }
            set 
            {
                if (Blocks.FirstBlock != null)
                {
                    Blocks.FirstBlock.Padding = new Thickness(Blocks.FirstBlock.Padding.Left, value, Blocks.FirstBlock.Padding.Right, Blocks.FirstBlock.Padding.Bottom);
                    PRO_OtstupFlag = Blocks.FirstBlock.Padding.Top > 0;
                }
            }
        }

        /// <summary>СВОЙСТВО Нужно ли формировать отчет (true - да, false - нет)</summary>
        public bool PROP_NewCreate { get; set; }
       
        /// <summary>СВОЙСТВО Ветка</summary>
        public VirtualNodes PROP_Nodes { get; set; }

        /// <summary>СВОЙСТВО Переменные</summary>
        public UserVariable_Standart PROP_Variable { get; set; }

        /// <summary>СВОЙСТВО Префикс таблиц</summary>
        public string PROP_Prefix { get; set; }

        /// <summary>СВОЙСТВО Сылка на родительский документ</summary>
        public UserDocument PROP_Docum { get; set; }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        protected VirtualOtchet() 
        { 
            // Ставим пометку, что нужно формировать отчет
            PROP_NewCreate = true;
        }
       
        /// <summary>МЕТОД Инициализация отчета</summary>
        /// <param name="pNodes">Ветка</param>        
        public virtual VirtualOtchet MET_Inizial(VirtualNodes pNodes)
        {
            PROP_Nodes = pNodes;                                                // ветка
            FontFamily = new FontFamily("Times New Roman");                               // Times New Roman
            FontSize = 16;
            IsHyphenationEnabled = true;                                        // включаем перенос строк
            return this;
        }

        /// <summary>МЕТОД Инициализация отчета</summary>
        public virtual VirtualOtchet MET_Inizial()
        { 
            FontFamily = new FontFamily("Times New Roman");
            FontSize = 16;
            IsHyphenationEnabled = true;                                        // включаем перенос строк
            return this;
        }

        /// <summary>МЕТОД Создаем объект для печати</summary>
        /// <param name="pMyDocumentViewer">Просмоторщик печати</param> 
        /// <param name="pFlowDocument">Документ, который печатаем</param> 
        /// <param name="pPrintNow">Печатаем сразу</param> 
        public virtual void MET_CreatePrint(MyDocumentViewer pMyDocumentViewer, FlowDocument pFlowDocument, bool pPrintNow  = false)
        {
            MyGlo.BrushesOtchet = null;                                         // сбрасываем фон
            MET_Inizial(PROP_Nodes);                                            // формируем отчет (если надо)
            FlowDocument _FlowDocument = new FlowDocument { ColumnWidth = 400 };// новый документ
            _FlowDocument.Blocks.Add(this);                                     // добавляем в документ наш отчет
            MET_ImageEmblema(_FlowDocument);                                    // рисуем эмблему
            MET_PreiwPrint(pMyDocumentViewer, _FlowDocument, pPrintNow);        // формируем объект для печати
            pFlowDocument.Blocks.Add(this);                                     // возвращаем отчет на место
        }

        /// <summary>МЕТОД Формируем объект для печати</summary>
        /// <param name="pMyDocumentViewer">Просмоторщик печати</param> 
        /// <param name="pFlowDocument">Документ, который печатаем</param> 
        /// <param name="pPrintNow">Печатаем сразу</param> 
        protected void MET_PreiwPrint(MyDocumentViewer pMyDocumentViewer, FlowDocument pFlowDocument, bool pPrintNow = false)
        {            
            try
            {
                // ПЕЧАТЬ с помощью фалов в памяти
                // Подготовиться к сохранению содержимого в памяти
                MemoryStream _MemoryStream = new MemoryStream();
                // Создать пакет, используя статический метод Package.Open()
                Package _Package = Package.Open(_MemoryStream, FileMode.Create, FileAccess.ReadWrite);
                // Каждому пакету необходим URI. Использовать синтаксис pack://. 
                // Действительное имя файла неважно
                Uri _Uri = new Uri("pack://filename.xps");
                // Удаляем пакет, если он уже был с таким именем
                PackageStore.RemovePackage(_Uri);
                // Добавить пакет
                PackageStore.AddPackage(_Uri, _Package);
                // Создать документ XPS на основе этого пакета. (с уровнем сжатия)
                XpsDocument _XpsDoc = new XpsDocument(_Package, CompressionOption.SuperFast, _Uri.AbsoluteUri);
                
                // Формируем файл просмотра печати
                XpsDocumentWriter _XpsDocWri = XpsDocument.CreateXpsDocumentWriter(_XpsDoc);
                // Создаем класс просмотра печати с нашими данными
                UserPrint _MyDocPag = new UserPrint(pFlowDocument, PROP_Nodes.PROP_prnPadding);
                // Отображаем наш класс
                _XpsDocWri.Write(_MyDocPag);
                // Подсоединяем документ к просмоторщику
                pMyDocumentViewer.Document = _XpsDoc.GetFixedDocumentSequence();
                // Устанавливаем просмотр по ширине
                pMyDocumentViewer.FitToWidth();
                // Закрываем файл просмотра печати
                _XpsDoc.Close();
                
                // Если печатаем сразу, без предварительного просмотра
                if (pPrintNow)
                {
                    pMyDocumentViewer.MyPrintNow();
                    // Чистим пакет и поток (хотя судя по памяти, результат не виден)
                    _Package.Close();
                    _MemoryStream.Dispose();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка печати", "Ошибка печати");
            }
        }

        /// <summary>МЕТОД Формируем отчет (пустой)</summary>
        protected virtual void MET_Otchet() { }

        /// <summary>МЕТОД Выводим, если документ не заполнен</summary>
        protected virtual void MET_NoOtchet() 
        {
            xVopr = "Документ не заполнен";
            xOtvet = "";
            xAligment = 2; xEnter = 2;
            MET_Print();
            PRO_Paragraph = new Paragraph();
        }

        /// <summary>МЕТОД Выводим данные в Документ</summary>
        protected void MET_Print(FlowDocument pFlowDocument = null)
        {               
            // ---- Новая страница
            if (xPage)
                xParagraph = true;
            // Пропуск строк
            if (xParagraph & xEnter > 0)
                xEnter--;
            // ---- Новый параграф
            if (PRO_Paragraph == null | xParagraph)
            {
                if (xParagraph & PRO_Paragraph != null)
                {
                    if (pFlowDocument != null)
                        pFlowDocument.Blocks.Add(PRO_Paragraph);
                    else
                        Blocks.Add(PRO_Paragraph);
                    xParagraph = false;
                }
                PRO_Paragraph = new Paragraph {LineHeight = 5};
            }
            // ----- Новая страница
            if (xPage)
            {
                PRO_Paragraph.BreakPageBefore = true;
                xPage = false;
            }

            Run _Vopr = new Run(xVopr);
            Run _Otv = new Run("  " + xOtvet);
            xVopr = "";
            xOtvet = "";
            
            // ---- Новая строка
            for (; xEnter > 0; xEnter--)
            {
                if (pFlowDocument != null)
                    pFlowDocument.Blocks.Add(PRO_Paragraph);
                else
                    Blocks.Add(PRO_Paragraph);
                PRO_Paragraph = new Paragraph {LineHeight = 1};
            }
         
            // ---- Табуляция
            for (; xTab > 0; xTab--)
                PRO_Paragraph.Inlines.Add("\t");
            // ---- Устанавливаем шрифт
            _Vopr.FontSize = xSize;
            _Vopr.FontWeight = xWeight;
            _Vopr.FontStyle = xStyle;
            // ---- Устанавливаем выравнивание
            switch (xAligment)
            {
                case 0:
                case 1:
                    PRO_Paragraph.TextAlignment = TextAlignment.Justify;
                    break;
                case 2:
                    PRO_Paragraph.TextAlignment = TextAlignment.Center;
                    break;
                case 3:
                    PRO_Paragraph.TextAlignment = TextAlignment.Right;
                    break;
            }
            PRO_Paragraph.Inlines.Add(_Vopr);                                   // печатаем вопрос
            _Otv.FontWeight = FontWeights.Bold;                                 // устанавливаем стиль для ответа
            _Otv.FontSize = xSize++;                                            // размер шрифта
            PRO_Paragraph.Inlines.Add(_Otv);                                    // печатаем ответ 

			if (PRO_Paragraph.Parent == null)
			{
				if (pFlowDocument != null)
					pFlowDocument.Blocks.Add(PRO_Paragraph);
				else
					Blocks.Add(PRO_Paragraph);
			}
            
            // Сбрасываем настройки на начальные
            xSize = FontSize;                                                   // размер шрифта по умолчанию
            xStyle = FontStyles.Normal;                                         // стиль по умолчанию
            xWeight = FontWeights.Normal;                                       // жирность по умолчанию
            xAligment = 1;                                                      // выравнивание по умолчанию
            xOtvet = "";
            xVopr = "";
        }        
        
        /// <summary>МЕТОД Показываем эмблему</summary>
        protected virtual void MET_ImageEmblema(FlowDocument pFlowDocument)
        {
            // Если продолжение печати, то эмблему не печатаем
            if (PRO_OtstupFlag) return;
            // Рисуем эмблему
            ImageBrush _Image = new ImageBrush
            {
                Stretch = Stretch.None,
                ImageSource = (BitmapImage) FindResource("imgEmblema"),
                Viewbox = new Rect(0.3, -0.25, 1, 1),
                AlignmentX = AlignmentX.Right,
                AlignmentY = AlignmentY.Top,
                Opacity = 0.4
            };
            _Image.Freeze();   			
            pFlowDocument.Background = _Image;				
        }

        /// <summary>МЕТОД Создаем объект Pole</summary>
        /// <param name="pTypePole">Номер типа поля eTypePole</param>
        protected VirtualPoleOtchet MET_CreateUserPole(eVopros pTypePole)
        {
            VirtualPoleOtchet _Pole = null;
            switch (pTypePole)
            {
                case eVopros.Number:		                                    // 1. Число UserPole_Number
                    _Pole = new UserPoleOtchet_Text();
                    break;
                case eVopros.Real:		                                        // 2. Десятичное UserPole_Text
                    _Pole = new UserPoleOtchet_Text();
                    break;
                case eVopros.Date:		                                        // 3. Дата UserPole_Data
                    _Pole = new UserPoleOtchet_Text();
                    break;
                case eVopros.KrString:	                                        // 4. Текст (короткий) UserPole_Text
                    _Pole = new UserPoleOtchet_Text();
                    break;
                case eVopros.List:		                                        // 5. Список UserPole_Text
                    _Pole = new UserPoleOtchet_Text();
                    break;
                case eVopros.LoText:	                                        // 6. Текст (длинный) UserPole_Text
                    _Pole = new UserPoleOtchet_Text();
                    break;
                case eVopros.MultiList:	                                        // 7. Код операци Список (многовыборочный) UserPole_MultyList
                    _Pole = new UserPoleOtchet_Text();
                    break;
                case eVopros.Text:		                                        // 8. Свободный текст (основной) UserPole_Text
                    _Pole = new UserPoleOtchet_Text();
                    break;
                case eVopros.Razdel:	                                        // 9. Раздел UserPole_Razdel
					_Pole = new UserPoleOtchet_Razdel();
                    break;
                case eVopros.Hide:		                                        // 10. Скрытое поле UserPole_Text
                    _Pole = new UserPoleOtchet_Text();
                    break;
                case eVopros.Time:		                                        // 11. Время UserPole_Text
                    _Pole = new UserPoleOtchet_Text();
                    break;
                case eVopros.Radio:		                                        // 12. Радио кнопки UserPoleRadio_Button
                    _Pole = new UserPoleOtchet_Text();
                    break;
                case eVopros.Image:		                                        // 13. Изображения UserPole_Image
                    _Pole = new UserPoleOtchet_Image();
                    break;
                case eVopros.Grid:		                                        // 14. Таблица UserPole_Grid
                    _Pole = new UserPoleOtchet_Grid();
                    break;
                case eVopros.Label:		                                        // 15. Метка UserPole_Label
					_Pole = new UserPoleOtchet_Label();
                    break;
                case eVopros.Sprav:		                                        // 16. Справочники UserPole_Sprav
                    _Pole = new UserPoleOtchet_Text();
                    break;
                case eVopros.Calendar:		                                    // 16. Календарь UserPole_Calendar
                    _Pole = new UserPoleOtchet_Text();
                    break;
            }
            _Pole.PROP_Type = pTypePole;                                        // проставляем тип поля
            _Pole.PROP_Prefix = PROP_Prefix;                                    // префикс таблицы
            return _Pole;
        }
			  		
        /// <summary>МЕТОД Заполняем данные из протокола </summary>
        /// <param name="pFlowDocument">Если null, то текущий отчет, иначе в заданный FlowDocument</param>
        protected void MET_Protokol(FlowDocument pFlowDocument = null)
        {
            // Список полей 
            PUB_HashPole = new Hashtable();
            // Пробегаемся по вопросам шаблона
            foreach (UserShablon _Shablon in PROP_Docum.PROP_Shablon)
            {
                eVopros _Type = (eVopros)_Shablon.PROP_Type;
                VirtualPoleOtchet _Pole = MET_CreateUserPole(_Type);            // создаем поле, соответствующего типа
                _Pole.PROP_VarID = _Shablon.PROP_VarId;
                _Pole.PROP_Maska = MyMet.MET_ParseInt(_Shablon.PROP_Maska);
                _Pole.PROP_Format = new MyFormat(_Shablon.PROP_xFormat);
                _Pole.PROP_Shablon = _Shablon;
                _Pole.PROP_Protokol = PROP_Docum.PROP_Protokol;                                
                _Pole.PROP_StrProtokol = PROP_Docum.PROP_Protokol.PROP_Protokol;                                   
                _Pole.PROP_Name = "Pole_" + _Pole.PROP_VarID;                   // имя поля
                PUB_HashPole.Add(_Pole.PROP_Name, _Pole);                       // записываем элемент в очередь
                _Pole.MET_Inicial();                                            // инициализация поля
                _Pole.PROP_Parent = (VirtualPoleOtchet)PUB_HashPole["Pole_" + _Pole.PROP_Maska]; // родительское поле, если есть
				// Добавляем форму в отчет
                if (_Pole.PROP_Parent != null && _Pole.PROP_Parent.PROP_Nested && !_Pole.PROP_Parent.PROP_HideChild)
				{
					_Pole.PROP_Parent.MET_AddElement(_Pole);
				}
				else
				{
					// Если родитель запретил печатать, то и мы запретим детям печататься
					if (_Pole.PROP_Parent != null && _Pole.PROP_Parent.PROP_HideChild)
					{
						_Pole.PROP_HideChild = true;
						_Pole.PROP_Hide = true;
					}
					// Если не печатаем поле, то следующее поле
					if (_Pole.PROP_Hide)
						continue;
					TextElement _Element = _Pole.MET_Print();
					if (_Element is Block)
					{
						if (pFlowDocument != null)
							pFlowDocument.Blocks.Add((Block)_Element);
						else
							Blocks.Add((Block)_Element);						
						if (_Element is Paragraph)
							PRO_Paragraph = (Paragraph)_Element;
						else
							PRO_Paragraph = new Paragraph();
					}
					else
					{
						PRO_Paragraph.Inlines.Add((Inline)_Element);
						if (PRO_Paragraph.Parent == null)
						{
							if (pFlowDocument != null)
								pFlowDocument.Blocks.Add(PRO_Paragraph);
							else
								Blocks.Add(PRO_Paragraph);
						}
					}
				}
            }
        }
	   
        /// <summary>МЕТОД Ставим фон</summary>
        protected void MET_Background()
        {
            // Фон
            Background = MyGlo.BrushesOtchet;
            // Если не просмотр печати, то меняем фон
            if ( MyGlo.BrushesOtchet != null)
                MyGlo.BrushesOtchet = Equals(MyGlo.BrushesOtchet, Brushes.White) ? Brushes.AliceBlue : Brushes.White;
        }

        #region ---- ВЫБОР ДАННЫХ ----
        /// <summary>МЕТОД Возвращаем строку</summary>
        protected string MET_PoleStr(string pPole)
        {
            return Convert.ToString(PRO_RowShablon[pPole]);
        }

        /// <summary>МЕТОД Возвращаем строку c Датой</summary>
        protected string MET_PoleDat(string pPole)
        {
            return MyMet.MET_StrDat(PRO_RowShablon[pPole]);
        }

        /// <summary>МЕТОД Возвращаем строку c Датой</summary>
        protected DateTime? MET_PoleDate(string pPole)
        {
            return DateTime.Parse(PRO_RowShablon[pPole].ToString());
        }

        /// <summary>МЕТОД Возвращаем строку c Целым числом</summary>
        protected int MET_PoleInt(string pPole)
        {
            return MyMet.MET_ParseInt(PRO_RowShablon[pPole].ToString());
        }

        /// <summary>МЕТОД Возвращаем строку c Десятичным числом</summary>
        protected decimal MET_PoleDec(string pPole)
        {
            return MyMet.MET_ParseDec(PRO_RowShablon[pPole].ToString());
        }
        #endregion
    }
}
