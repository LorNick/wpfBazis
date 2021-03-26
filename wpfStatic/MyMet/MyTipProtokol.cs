namespace wpfStatic
{
    /// <summary>КЛАСС Тип протокола</summary>
    public class MyTipProtokol
    {
        #region ---- Свойства ----
        /// <summary>СВОЙСТВО Тип документа</summary>
        public eTipDocum PROP_TipDocum { get; private set; }

        /// <summary>СВОЙСТВО Префикс протокола (ast, apaN, par, kdl)</summary>
        public string PROP_Prefix { get; }

        /// <summary>СВОЙСТВО Тип записи Tab в таблице kbolInfo</summary>
        public string PROP_KbolInfo { get; }

        /// <summary>СВОЙСТВО Таблица -  список вопросов шаблона</summary>
        public string PROP_Shablon => PROP_Prefix + "Shablon";

        /// <summary>СВОЙСТВО Таблица - протоколы</summary>
        public string PROP_Protokol => PROP_Prefix + "Protokol";

        /// <summary>СВОЙСТВО Таблица - список ответов на вопрос</summary>
        public string PROP_List => PROP_Prefix + "List";

        /// <summary>СВОЙСТВО Код индексатора в таблице NextRef</summary>
        public int PROP_NextRef { get; private set; }
        #endregion

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pTipProtokol">Тип протокола</param>
        public MyTipProtokol(eTipDocum pTipProtokol)
        {
            // Тип
            PROP_TipDocum = pTipProtokol;

            // Префикс
            switch (pTipProtokol)
            {
                case eTipDocum.Stac:
                    PROP_Prefix = "ast";
                    PROP_KbolInfo = "stac";
                    PROP_NextRef = 24;
                    break;
                case eTipDocum.Pol:
                    PROP_Prefix = "apaN";
                    PROP_KbolInfo = "pol";
                    PROP_NextRef = 21;
                    break;
                case eTipDocum.Paracl:
                    PROP_Prefix = "par";
                    PROP_KbolInfo = "par";
                    PROP_NextRef = 29;
                    break;
                case eTipDocum.Kdl:
                    PROP_Prefix = "kdl";
                    PROP_KbolInfo = "kdl";
                    PROP_NextRef = 41;
                    break;
                default:
                    PROP_Prefix = "";
                    PROP_KbolInfo = "";
                    PROP_NextRef = 0;
                    break;
            }
        }
    }
}
