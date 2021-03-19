using System.Collections.Generic;

namespace wpfStatic
{
    /// <summary>КЛАСС Смены кодировки для поиска</summary>
    public class MyTransliter
    {
        /// <summary>Словарь</summary>
        private readonly Dictionary<string, string> PRI_Words = new Dictionary<string, string>();

        /// <summary>КОНСТРУКТОР</summary>
        public MyTransliter()
        {
            // Перевод Кирилица в Латиницу
            PRI_Words.Add("Ё", "`");
            PRI_Words.Add("Й", "Й");
            PRI_Words.Add("Ц", "W");
            PRI_Words.Add("У", "E");
            PRI_Words.Add("К", "R");
            PRI_Words.Add("Е", "T");
            PRI_Words.Add("Н", "Y");
            PRI_Words.Add("Г", "U");
            PRI_Words.Add("Ш", "I");
            PRI_Words.Add("Щ", "O");
            PRI_Words.Add("З", "P");
            //   PRI_Words.Add("Х", "[");
            //   PRI_Words.Add("Ъ", "]");
            PRI_Words.Add("Ф", "A");
            PRI_Words.Add("Ы", "S");
            PRI_Words.Add("В", "D");
            PRI_Words.Add("А", "F");
            PRI_Words.Add("П", "G");
            PRI_Words.Add("Р", "H");
            PRI_Words.Add("О", "J");
            PRI_Words.Add("Л", "K");
            PRI_Words.Add("Д", "L");
            PRI_Words.Add("Ж", ";");
            PRI_Words.Add("Э", "''");
            PRI_Words.Add("Я", "Z");
            PRI_Words.Add("Ч", "X");
            PRI_Words.Add("С", "C");
            PRI_Words.Add("М", "V");
            PRI_Words.Add("И", "B");
            PRI_Words.Add("Т", "N");
            PRI_Words.Add("Ь", "M");
            PRI_Words.Add("Б", ",");
            PRI_Words.Add("Ю", ".");
            // Перевод Латиницу в Кирилица 
            PRI_Words.Add("`", "Ё");
            PRI_Words.Add("Q", "Й");
            PRI_Words.Add("W", "Ц");
            PRI_Words.Add("E", "У");
            PRI_Words.Add("R", "К");
            PRI_Words.Add("T", "Е");
            PRI_Words.Add("Y", "Н");
            PRI_Words.Add("U", "Г");
            PRI_Words.Add("I", "Ш");
            PRI_Words.Add("O", "Щ");
            PRI_Words.Add("P", "З");
            PRI_Words.Add("[", "Х");
            PRI_Words.Add("]", "Ъ");
            PRI_Words.Add("A", "Ф");
            PRI_Words.Add("S", "Ы");
            PRI_Words.Add("D", "В");
            PRI_Words.Add("F", "А");
            PRI_Words.Add("G", "П");
            PRI_Words.Add("H", "Р");
            PRI_Words.Add("J", "О");
            PRI_Words.Add("K", "Л");
            PRI_Words.Add("L", "Д");
            PRI_Words.Add(";", "Ж");
            PRI_Words.Add("'", "Э");
            PRI_Words.Add("Z", "Я");
            PRI_Words.Add("X", "Ч");
            PRI_Words.Add("C", "С");
            PRI_Words.Add("V", "М");
            PRI_Words.Add("B", "И");
            PRI_Words.Add("N", "Т");
            PRI_Words.Add("M", "Ь");
            PRI_Words.Add(",", "Б");
            PRI_Words.Add(".", "Ю");
        }

        /// <summary>МЕТОД Возвращаем текст в другой кодировке</summary>
        /// <param name="pText">Текст условия</param>    
        public string MET_Replace(string pText)
        {   
            string source = "";
            foreach (char _Char in pText)
            {
                string _Value;
                if (PRI_Words.TryGetValue(_Char.ToString(), out _Value))
                    source += _Value;
                else
                    source += _Char.ToString();
            }
            return source;
        }
    }  
}
