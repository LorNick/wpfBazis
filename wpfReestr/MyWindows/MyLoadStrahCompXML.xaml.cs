using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using wpfStatic;

namespace wpfReestr
{
    /// <summary>КЛАСС Загружаем XML файл Страховых Компаний </summary>
    /// <remarks>Устаревшие страховые компание помечаем как old, а новые добавляем</remarks>
    public partial class MyLoadStrahCompXML : Window
    {
        /// <summary>Наша база</summary>
        private StarahReestrDataContext PRI_Context;

        /// <summary>Список страховых компани</summary>
        private s_StrahComp PRI_StrahComp;

        /// <summary>Список областей</summary>
        private s_Oblast PRI_Oblast;

        /// <summary>Имя XML файла</summary>
        private string PRI_FileName;

        /// <summary>КОНСТРУКТОР</summary>
        public MyLoadStrahCompXML()
        {
            InitializeComponent();
        }

        /// <summary>СОБЫТИЕ Выбор XML файла</summary>
        private void PART_ButtonOpenXML_Click(object sender, RoutedEventArgs e)
        {  
            // Находим нужный файл
            var _DialogFileOpen = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".xml",
                Filter = "XML file|*.xml"
            };

            if (_DialogFileOpen.ShowDialog() != true) return;
            PART_LabelFileName.Text = _DialogFileOpen.FileName;
            PRI_FileName = _DialogFileOpen.FileName;
        }

        /// <summary>МЕТОД Проверим, а тот ли XML файла</summary>
        private bool MET_VerificatioXmlFile()
        {
            return false;
        }

        /// <summary>СОБЫТИЕ Нажали на кнопку Обновить Страховые компании</summary>
        private void PART_ButtonLoadFile_Click(object sender, RoutedEventArgs e)
        {   
            string _Tf_okato = "";
            int _Smocod = 0;
            string _Nam_smok = "";
            string _D_end = "";
            string _Ogrn = "";

            PRI_Context = new StarahReestrDataContext(MySql.MET_ConSql());

            ((Paragraph) PART_RichTextBox.Document.Blocks.FirstBlock).LineHeight = 2;

            XmlTextReader _TextReader = new XmlTextReader(PRI_FileName);
            while (_TextReader.Read())
            {
                if (_TextReader.NodeType == XmlNodeType.Element)
                {
                    switch (_TextReader.Name)
                    {
                        case "tf_okato":
                            _TextReader.Read();
                            _Tf_okato = _TextReader.Value;
                            break;
                        case "smocod":
                            _TextReader.Read();
                            _Smocod = Convert.ToInt32(_TextReader.Value);
                            break;
                        case "nam_smok":
                            _TextReader.Read();
                            _Nam_smok = _TextReader.Value;
                            break;
                        case "d_end":
                            _TextReader.Read();
                            _D_end = _TextReader.Value == "" ? "" : "old";   
                            break;
                        case "Ogrn":
                            _TextReader.Read();
                            _Ogrn = _TextReader.Value;
                            break;
                    }
                }

                if (_TextReader.NodeType == XmlNodeType.EndElement && _TextReader.Name == "insCompany")
                {
                   
                    //}    

                    // Связь с SQL, в принципе не очень то и нужен, только для разового сохраниения в  StrahFile  
                    try
                    {

                        bool _Er = false;

                        PRI_StrahComp = PRI_Context.s_StrahComp.Single(n => n.KOD == _Smocod);

                        if (_Nam_smok != PRI_StrahComp.TKOD)
                        {
                            _Er = true;
                            PART_RichTextBox.AppendText("Несовпадает название:\n");
                        }

                        if (_Ogrn != PRI_StrahComp.OGRN)
                        {
                            _Er = true;
                            PART_RichTextBox.AppendText("Несовпадает ОГРН:\n");
                        }

                        if (_D_end != PRI_StrahComp.NewTKod)
                        {
                            _Er = true;
                            PART_RichTextBox.AppendText("Несовпадает дата закрытия:\n");
                        }

                        if (_Er)
                        {
                            PART_RichTextBox.AppendText(_Smocod + "\n");
                            PART_RichTextBox.AppendText("ОГРН старое/новое: " + PRI_StrahComp.OGRN + " / " + _Ogrn + "\n");
                            PART_RichTextBox.AppendText("Устаревшее старое/новое: " + PRI_StrahComp.NewTKod + " / " + _D_end + "\n");
                            PART_RichTextBox.AppendText("старое:" + PRI_StrahComp.TKOD + "\n");
                            PART_RichTextBox.AppendText("новое: " + _Nam_smok + "\n\n");
                            PRI_StrahComp.OGRN = _Ogrn;
                            PRI_StrahComp.NewTKod = _D_end;
                            PRI_StrahComp.TKOD = _Nam_smok;
                        }
                        //// Запись файла  StrahFile
                        //PRI_StrahFile = new StrahFile
                        //{
                        //    Cod = PRI_Context.StrahFile.Max(e => e.Cod + 1),
                        //    DateN = PROP_DateN,
                        //    DateK = PROP_DateK,
                        //    Korekt = PROP_Korekt,
                        //    StrahComp = (byte?)PRI_Strah,
                        //    SUMMAV = 0,
                        //    Tip = 1,
                        //    YEAR = PROP_DateK.Year,
                        //    MONTH = PROP_DateK.Month,
                        //    NSCHET = PROP_Schet,
                        //    DSCHET = PROP_DateSchet,
                        //    pHide = 0,
                        //    pParent = PROP_Parent
                        //};

                    }
                    catch(InvalidOperationException)
                    {
                        PART_RichTextBox.AppendText("Новая компания:\n");
                        PART_RichTextBox.AppendText(_Smocod + "  " + _Ogrn + "   " + _D_end + "\n");
                        PART_RichTextBox.AppendText(_Nam_smok + "\n");
                        PRI_StrahComp.KOD = _Smocod;
                        PRI_StrahComp.OGRN = _Ogrn;
                        PRI_StrahComp.NewTKod = _D_end;
                        PRI_StrahComp.TKOD = _Nam_smok;

                        try
                        {
                            PRI_Oblast = PRI_Context.s_Oblast.Single(n => n.NewKod == _Tf_okato);
                            PART_RichTextBox.AppendText("ОКАТО: " + _Tf_okato + " область:" + PRI_Oblast.kod + " / " + PRI_Oblast.NAIM + "\n\n");
                        }
                        catch (InvalidOperationException)
                        {
                            PART_RichTextBox.AppendText("область НЕ найдена \n\n");
                        }  
                    }      
                    catch
                    {
                        //
                    }
                }

            }
            PART_RichTextBox.AppendText("Усё"); 
        }

        
    }
}
