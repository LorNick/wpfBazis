using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using wpfGeneral.UserControls;
using wpfGeneral.UserNodes;
using wpfGeneral.UserStruct;
using wpfStatic;
using Label = System.Windows.Controls.Label;
using Orientation = System.Windows.Controls.Orientation;
using RadioButton = System.Windows.Controls.RadioButton;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace wpfGeneral.UserOtchet
{
    /// <summary>КЛАСС Отчет Истории болезни (для типа Inform)</summary>
    public class UserOtcher_History : VirtualOtchet_History
    {
        ///<summary>Запретить показывать вкладки кроме выписок и параклиники</summary>
        public bool PUB_Ban;

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
                // Формируем отчет
                MET_Otchet();
                // Добавляем последний параграф в блок
                Blocks.Add(PRO_Paragraph);
                // Помечаем, что больше его формировать не надо
                PROP_NewCreate = false;
            }
            return this;
        }

        /// <summary>МЕТОД Формируем отчет История болезни</summary>
        protected override void MET_Otchet()
        {
            PRO_Paragraph = new Paragraph();
            MyGlo.HashOtchet.Add("eleTVItem_History", this);

            // Добавляем кнопки фильтра истории
            StackPanel _Panel = new StackPanel();
            _Panel.Orientation =Orientation.Horizontal;
            PRO_Paragraph.Inlines.Add(_Panel);
            Button _ButtonSort = new Button();
            Image _Image = new Image();
            _Image.Height = 20;
            _Image.Width = 20;
            _Image.Source = (BitmapImage)FindResource("mnGreenUp");
            _ButtonSort.Content = _Image;
            _ButtonSort.ToolTip = "Меняет сортировку по дате посещения на обратную";
            _ButtonSort.Focusable = false;
            _ButtonSort.Tag = 1;
            _ButtonSort.Click += PART_ButtonSort_Click;
            _Panel.Children.Add(_ButtonSort);
            Label _LabelFiltr = new Label();
            _LabelFiltr.Content = "Фильтр по:";
            _LabelFiltr.VerticalAlignment = VerticalAlignment.Center;
            _Panel.Children.Add(_LabelFiltr);
            RadioButton _ButtonAll = new RadioButton();
            _ButtonAll.Content = "Все";
            _ButtonAll.ToolTip = "Отображает Все посещения";
            _ButtonAll.Margin = new Thickness(5);
            _ButtonAll.IsChecked = true;
            _ButtonAll.Checked += PART_RadioButton_Checked;
            _Panel.Children.Add(_ButtonAll);
            RadioButton _ButtonIssl = new RadioButton();
            _ButtonIssl.Content = "Параклиника";
            _ButtonIssl.ToolTip = "Отображает только посещения в Параклинику";
            _ButtonIssl.Margin = new Thickness(5);
            _ButtonIssl.Checked += PART_RadioButton_Checked;
            _Panel.Children.Add(_ButtonIssl);
            RadioButton _ButtonStac = new RadioButton();
            _ButtonStac.Content = "Стационар";
            _ButtonStac.ToolTip = "Отображает только посещения Стационара";
            _ButtonStac.Margin = new Thickness(5);
            _ButtonStac.Checked += PART_RadioButton_Checked;
            _Panel.Children.Add(_ButtonStac);
            RadioButton _ButtonPol = new RadioButton();
            _ButtonPol.Content = "Поликлиника";
            _ButtonPol.ToolTip = "Отображает только посещения Поликлиники";
            _ButtonPol.Margin = new Thickness(5);
            _ButtonPol.Checked += PART_RadioButton_Checked;
            _Panel.Children.Add(_ButtonPol);
            RadioButton _ButtonKdl = new RadioButton();
            _ButtonKdl.Content = "Диагностика";
            _ButtonKdl.ToolTip = "     Отображает только те посещения,\n" +
                                 "в которых есть результат Диагностических исследований\n" +
                                 "(пока только Гистологии, Цитологии)";
            _ButtonKdl.Margin = new Thickness(5);
            _ButtonKdl.Checked += PART_RadioButton_Checked;
            _Panel.Children.Add(_ButtonKdl);
            PRO_Paragraph.Inlines.Add(new LineBreak());                         // разрыв строки
            // Заполняем список шаблонов
            MySql.MET_DsAdapterFill(MyQuery.MET_History_Select_1(MyGlo.KL), "History");
            foreach (DataRow _Row in MyGlo.DataSet.Tables["History"].Rows)
            {
                PRO_RowShablon = _Row;
                UserPole_History _Pole = new UserPole_History();
                string _Icon = "";
                // Тип поля
                _Pole.PROP_Type = (eTipDocum)MET_PoleInt("Nom");
                // Код документа
                _Pole.PROP_Cod = MET_PoleDec("Cod");
                _Pole.PROP_Vrach = MET_PoleStr("Vr");
                _Pole.PROP_Dp = MET_PoleDat("Dp");
                _Pole.PROP_Diag = MET_PoleStr("D");
                _Pole.PROP_Profil = MET_PoleStr("Profil");
                _Pole.PROP_CodApstac = MET_PoleDec("CodApstac");
                _Pole.PROP_Kdl = MET_PoleStr("kdl");
                _Pole.PROP_IsDelete = MET_PoleInt("xDelete") == 1;
                _Pole.PROP_xLog = MET_PoleStr("xLog");
                // В зависимости от типа: поликлинника/стационар/параклиника
                switch (_Pole.PROP_Type)
                {
                    case eTipDocum.Pol:
                        _Icon = "mnPosPolikl";
                        _Pole.PROP_Date = MET_PoleDat("Dp");
                        _Pole.PROP_Document = MET_PoleStr("Profil");
                        _Pole.PROP_Vrach = MET_PoleStr("Vr");
                        _Pole.PROP_Diag = MET_PoleStr("D");
                        // Цвет
                        if (MET_PoleInt("pCount") > 0)
                            _Pole.PROP_Background = Brushes.Azure;
                        // Тип посещения (разовое, обращение)
                        _Pole.PROP_Metca = MET_PoleStr("Metka");
                        // Если нет протоколов, то
                        if (MET_PoleInt("pCount") == 0)
                        {
                            _Pole.PROP_Metca += " (без протоколов)";
                            _Pole.IsEnabled = false;
                        }
                        // Ставим значек консилиум
                        if (MET_PoleStr("ImageInform") == "консилиум")
                            _Pole.MET_LoadIconInform("mnKons", "Наличие консилиума");
                        _Pole.callbackOpenNew = MET_OpenPolicl;
                        break;
                    case eTipDocum.Stac:
                        _Icon = MET_PoleStr("ImageInform");
                        string _Dk = MET_PoleDat("Dk");
                        if (_Dk == "") _Dk = "(по сегодня)";
                        _Pole.PROP_Date = MET_PoleDat("Dp") + " - " + _Dk;
                        _Pole.PROP_Document = MET_PoleStr("Profil");
                        _Pole.PROP_Vrach = MET_PoleStr("Vr");
                        _Pole.PROP_Diag = MET_PoleStr("D");
                        // Цвет
                        if (MET_PoleInt("pCount") > 0)
                            _Pole.PROP_Background = Brushes.AliceBlue;
                        // Если нет протоколов, то
                        if (MET_PoleInt("pCount") == 0)
                        {
                            _Pole.PROP_Metca = "(без протоколов)";
                            _Pole.IsEnabled = false;
                        }
                        _Pole.callbackOpenNew = MET_OpenStac;
                        break;
                    case eTipDocum.Paracl:
                        _Icon = "mnParacl";
                        _Pole.PROP_Date = MET_PoleDat("Dp");
                        _Pole.PROP_Document = MET_PoleStr("Profil");
                        _Pole.PROP_Vrach = MET_PoleStr("Vr");
                        _Pole.PROP_Background = Brushes.BlanchedAlmond;
                        _Pole.PROP_NumerShablon = MET_PoleInt("pCount");
                        _Pole.PROP_IsTexted = true;
                        _Pole.PROP_DocumHistory = new UserDocument(_Pole.PROP_Type);
                        _Pole.PROP_DocumHistory.PROP_Protokol = UserProtokol.MET_FactoryProtokol(_Pole.PROP_Type, (int)_Pole.PROP_Cod);
                        _Pole.MET_Inicial();
                        _Pole.callbackOpenNew = MET_Protokol;
                        // Костыль для физиологов
                        if (_Pole.PROP_NumerShablon > 300 & _Pole.PROP_NumerShablon < 400)
                        {
                            _Icon = "mnPhysiology";
                            _Pole.PROP_Background = Brushes.LightYellow;
                        }
                        break;
                }
                // Иконка
                _Pole.MET_LoadIcon(_Icon);
                // Если есть результат КДЛ, то окрашиваем это поле в зависимости от результата
                if (_Pole.PROP_Kdl != "" && _Pole.PROP_Type != eTipDocum.Paracl)
                    MET_ColorKDL(_Pole);
                // Добавляем в очередь
                PRO_PoleHistory.Add(_Pole);
                // Добавляем поле в параграф
                PRO_Paragraph.Inlines.Add(_Pole);
                if (_Pole.PROP_Kdl != "" && _Pole.PROP_Type == eTipDocum.Paracl)
                {
                    var _KDL = MET_GreatKDL(_Pole);
                    // Добавляем в очередь
                    PRO_PoleHistory.Add(_KDL);
                    // Добавляем поле в параграф
                    PRO_Paragraph.Inlines.Add(_KDL);
                }
                 //PRO_Paragraph.Inlines.Add(new LineBreak());                     // разрыв строки
            }
        }

        /// <summary>СОБЫТИЕ Сортируем протоколы по дате в обратном порядке</summary>
        private void PART_ButtonSort_Click(object sender, RoutedEventArgs e)
        {
            // Меняем иконку на кнопке
            var _Button = (Button) sender;
            if ((int)_Button.Tag == 1)
            {
                ((Image)_Button.Content).Source = (BitmapImage)FindResource("mnGreenDown");
                _Button.Tag = 2;
            }
            else
            {
                ((Image)_Button.Content).Source = (BitmapImage)FindResource("mnGreenUp");
                _Button.Tag = 1;
            }
            // Сортировка
            var _Panel = PRO_Paragraph.Inlines.FirstInline;                     // панель управления
            var _History = PRO_Paragraph.Inlines.Reverse();                     // сортируем в обратном порядке элементы истории болезни
            PRO_Paragraph.Inlines.Add(_Panel);                                  // ставим на место сначала панель управления
            PRO_Paragraph.Inlines.AddRange(_History);                           // потом обратно историю
            PRO_PoleHistory.Reverse();
        }

        /// <summary>СОБЫТИЕ Фильтруем историю</summary>
        private void PART_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton _Radio = (RadioButton) sender;
            string _String = _Radio.Content.ToString();
            eTipDocum _Tip = eTipDocum.Null;
            switch (_String)
            {
                case "Параклиника":
                    _Tip = eTipDocum.Paracl;
                    break;
                case "Стационар":
                    _Tip = eTipDocum.Stac;
                    break;
                case "Поликлиника":
                    _Tip = eTipDocum.Pol;
                    break;
                case "Диагностика":
                    _Tip = eTipDocum.Kdl;
                    break;
            }
            // Перебираем всю историю
            foreach (UserPole_History _Pole in PRO_PoleHistory)
            {
                if (_Tip != eTipDocum.Null && _Pole.PROP_Type != _Tip)
                {
                    _Pole.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (!_Pole.PROP_IsDelete || MyGlo.ShowDeletedProtokol)
                        _Pole.Visibility = Visibility.Visible;
                }

                // Только для КДЛ
                if ((_Tip == eTipDocum.Null || _Tip == eTipDocum.Kdl) && _Pole.PROP_Kdl != "")
                    _Pole.Visibility = Visibility.Visible;
            }

        }

        /// <summary>МЕТОД Заполняем экспандер при первом открытии Поликлиники</summary>
        /// <param name="pPole">Наше поле</param>
        public void MET_OpenPolicl(UserPole_History pPole)
        {
            // Заполняем список шаблонов
            MySql.MET_DsAdapterFill(MyQuery.MET_History_Select_3(pPole.PROP_Cod), "History");
            foreach (DataRow _Row in MyGlo.DataSet.Tables["History"].Rows)
            {
                PRO_RowShablon = _Row;
                // Настраиваем поле документа
                UserPole_History _Pole = new UserPole_History();
                _Pole.PROP_Document = MET_PoleStr("NameKr");
                _Pole.Margin = new Thickness(5, 0, 0, 0);
                _Pole.PROP_Background = Brushes.LightYellow;
                // Находим иконку
                string _Icon = MET_PoleStr("Icon") != "" ? MET_PoleStr("Icon") : "mnOneOsmotr";
                _Pole.MET_LoadIcon(_Icon);
                // Дополнительные параметры
                _Pole.PROP_Type = eTipDocum.Pol;
                _Pole.PROP_Cod = MET_PoleInt("Cod");
                _Pole.PROP_ParentCod = pPole.PROP_Cod;
                _Pole.PROP_CodApstac = pPole.PROP_CodApstac;
                _Pole.PROP_NumerShablon = MET_PoleInt("NumShablon");
                _Pole.PROP_Kdl = MET_PoleStr("kdl");
                _Pole.PROP_DocumHistory = new UserDocument(_Pole.PROP_Type);
                _Pole.PROP_Dp = pPole.PROP_Dp;
                _Pole.PROP_Profil = pPole.PROP_Profil;
                _Pole.PROP_IsTexted = true;
                _Pole.PROP_DocumHistory.PROP_Protokol = UserProtokol.MET_FactoryProtokol(pPole.PROP_Type, (int)_Pole.PROP_Cod);
                _Pole.MET_Inicial();
                // Делегат при открытии документа
                _Pole.callbackOpenNew = MET_Protokol;                           // Новый протокол
                // Если запрет для просмотра
                if (PUB_Ban && MET_PoleInt("Dostup") == 0)
                {
                    _Pole.PROP_Description = "(Доступ Закрыт)   ";
                    _Pole.PROP_Background = Brushes.GhostWhite;
                    _Pole.callbackOpenNew = null;
                }
                _Pole.PROP_IsDelete = MET_PoleInt("xDelete") == 1;
                pPole.MET_AddEle(_Pole);
                if (_Pole.PROP_Kdl != "")
                {
                    var _PoleKDL = MET_GreatKDL(_Pole);
                    _PoleKDL.Margin = new Thickness(5, 0, 0, 0);
                    // Добавляем в очередь
                    pPole.MET_AddEle(_PoleKDL);
                }
            }
            // Отключаем делегат
            pPole.callbackOpenNew = null;
        }

        /// <summary>МЕТОД Заполняем экспандер при первом открытии Стационара</summary>
        /// <param name="pPole">Наше поле</param>
        private void MET_OpenStac(UserPole_History pPole)
        {
            // Заполняем список шаблонов
            MySql.MET_DsAdapterFill(MyQuery.MET_History_Select_2(pPole.PROP_Cod), "History");
            foreach (DataRow _Row in MyGlo.DataSet.Tables["History"].Rows)
            {
                PRO_RowShablon = _Row;
                // Настраиваем поле документа
                UserPole_History _Pole = new UserPole_History();
                _Pole.PROP_Date = MET_PoleDat("pDate");
                _Pole.PROP_Document = MET_PoleStr("NameKr");
                _Pole.PROP_Dp = MET_PoleDat("pDate");
                _Pole.Margin = new Thickness(5, 0, 0, 0);
                _Pole.PROP_Background = Brushes.LightYellow;
                _Pole.PROP_Type = eTipDocum.Stac;
                _Pole.PROP_IsTexted = true;
                // Находим иконку
                string _Icon = MET_PoleStr("Icon") != "" ? MET_PoleStr("Icon") : "mnOneOsmotr";
                _Pole.MET_LoadIcon(_Icon);
                _Pole.PROP_Cod = MET_PoleInt("Cod");
                _Pole.PROP_CodApstac = pPole.PROP_CodApstac;
                _Pole.PROP_NumerShablon = MET_PoleInt("NumShablon");
                _Pole.PROP_Kdl = MET_PoleStr("kdl");
                _Pole.PROP_DocumHistory = new UserDocument(_Pole.PROP_Type);
                _Pole.PROP_DocumHistory.PROP_Protokol = UserProtokol.MET_FactoryProtokol(_Pole.PROP_Type, (int)_Pole.PROP_Cod);
                _Pole.PROP_Vrach = _Pole.PROP_DocumHistory.PROP_Protokol.PROP_UserName;
                _Pole.MET_Inicial();
                // Делегат при открытии документа
                _Pole.callbackOpenNew = MET_Protokol;
                // Если запрет для просмотра
                if (PUB_Ban && MET_PoleInt("Dostup") == 0)
                {
                    _Pole.PROP_Description = "(Доступ Закрыт)   ";
                    _Pole.PROP_Background = Brushes.GhostWhite;
                    _Pole.IsEnabled = false;
                    _Pole.callbackOpenNew = null;
                }
                _Pole.PROP_IsDelete = MET_PoleInt("xDelete") == 1;
                pPole.MET_AddEle(_Pole);
                if (_Pole.PROP_Kdl != "")
                {
                    var _PoleKDL = MET_GreatKDL(_Pole);
                    _PoleKDL.Margin = new Thickness(5, 0, 0, 0);
                    // Добавляем в очередь
                    pPole.MET_AddEle(_PoleKDL);
                }
            }
            // Отключаем делегат
            pPole.callbackOpenNew = null;
        }

        /// <summary>МЕТОД Создаем поле истории результата исседования КДЛ</summary>
        /// <param name="pPole">Родительское поле</param>
        private UserPole_History MET_GreatKDL(UserPole_History pPole)
        {
            // Заполняем список шаблонов
            MySql.MET_DsAdapterFill(MyQuery.MET_History_Select_4(MET_PoleInt("kdl")), "HistoryKDL");
            PRO_RowShablon = MyGlo.DataSet.Tables["HistoryKDL"].Rows[0];
            // Настраиваем поле документа
            UserPole_History _Pole = new UserPole_History();
            _Pole.PROP_Date = MET_PoleDat("pDate");
            _Pole.PROP_Document = MET_PoleStr("NameKr");
            _Pole.PROP_Dp = MET_PoleDat("pDate");
            _Pole.PROP_Kdl = MET_PoleStr("Indicator");
            _Pole.PROP_Metca = " (" + _Pole.PROP_Kdl + ")";
            _Pole.PROP_Background = Brushes.LightYellow;
            // Окрашиваем поле в зависимости от результата
            MET_ColorKDL(_Pole);
            _Pole.PROP_Type = eTipDocum.Kdl;
            _Pole.PROP_IsTexted = true;
            // Находим иконку
            string _Icon = MET_PoleStr("Icon") != "" ? MET_PoleStr("Icon") : "mnOneOsmotr";
            _Pole.MET_LoadIcon(_Icon);
            _Pole.PROP_Cod = MET_PoleInt("Cod");
            _Pole.PROP_CodApstac = pPole.PROP_CodApstac;
            _Pole.PROP_NumerShablon = MET_PoleInt("NumShablon");
            _Pole.PROP_Kdl = "1";
            _Pole.PROP_DocumHistory = new UserDocument(_Pole.PROP_Type);
            _Pole.PROP_IsDelete = MET_PoleInt("xDelete") == 1;
            _Pole.PROP_DocumHistory.PROP_Protokol = UserProtokol.MET_FactoryProtokol(_Pole.PROP_Type, (int)_Pole.PROP_Cod);
            _Pole.PROP_Vrach = _Pole.PROP_DocumHistory.PROP_Protokol.PROP_UserName;
            _Pole.MET_Inicial();
            // Делегат при открытии документа
            _Pole.callbackOpenNew = MET_Protokol;
            return _Pole;
        }

        /// <summary>МЕТОД Окрашиваем поле в зависимости от результата КДЛ</summary>
        /// <param name="pPole">Поле которое окрашиваем</param>
        private void MET_ColorKDL(UserPole_History pPole)
        {
            Color _Color;
            switch (pPole.PROP_Kdl)
            {
                case "без патологии":
                    _Color = Colors.LightGreen;
                    break;
                case "патология":
                    _Color = Colors.Tomato;
                    break;
                default:
                    _Color = Colors.Yellow;
                    break;
            }
            LinearGradientBrush _myBrush = new LinearGradientBrush(((SolidColorBrush)pPole.PROP_Background).Color, _Color, new Point(0.85, 0.4), new Point(1.0, 0.4));
            pPole.PROP_Background = _myBrush;
        }

        /// <summary>МЕТОД Заполняем Протоколы</summary>
        /// <param name="pPole">Наше поле</param>
        public void MET_Protokol(UserPole_History pPole)
        {
            // Обязательно, если экспандер содержит текст, а не под-вкладки
            pPole.PROP_IsTexted = true;
            // Документ
            FlowDocument _FlowDocument = new FlowDocument();
            _FlowDocument.Background = Brushes.White;
            // Просмоторщик
            pPole.PUB_RichTextBox = new RichTextBox();
            RichTextBox _RichTextBox = pPole.PUB_RichTextBox;
            _RichTextBox.Document = _FlowDocument;
            // Добавляем документ в экспандер
            pPole.MET_AddEle(_RichTextBox);
            // Привязываем к полю Документ,  таблицы ListShablon и  Protokol, Отчет
            pPole.PROP_DocumHistory.PROP_ListShablon = UserListShablon.MET_FactoryListShablon(pPole.PROP_Type, pPole.PROP_NumerShablon);
            pPole.PROP_DocumHistory.PROP_Otchet = new UserOtchet_Shablon { PROP_Docum = pPole.PROP_DocumHistory };
            // Добавляем отчет к документу
            _FlowDocument.Blocks.Add(pPole.PROP_DocumHistory.PROP_Otchet.MET_Inizial());
            // Отключаем делегат
            pPole.callbackOpenNew = null;
        }
    }
}