using System.Collections;

namespace wpfStatic
{
    /// <summary>КЛАСС Форматирование xFormat</summary>
    /// <remarks>Используется в таблицах ListShablon, Shablon </remarks>
    public class MyFormat
    {
        /// <summary>Набор параметров</summary>
        public Hashtable PROP_Value { get; private set; }

        /// <summary>КОНСТРУКТОР</summary>
        public MyFormat()
        {
            MET_Initial("");
        }

        /// <summary>КОНСТРУКТОР</summary>
        /// <param name="pFormat">Строка формата</param>
        public MyFormat(string pFormat)
        {
            MET_Initial(pFormat);
        }

        /// <summary>МЕТОД Инициализация</summary>
        /// <param name="pFormat">Строка формата</param>
        private void MET_Initial(string pFormat)
        {
            PROP_Value = new Hashtable();
            if (pFormat == "") return;
            string[] _mSplit = pFormat.Split('\\');
            foreach (string _Format in _mSplit)
            {
                int i = 0;
                string _Key = "";
                string _Value = "";
                string[] _mSplit2 = _Format.Split(' ');
                foreach (string _Format2 in _mSplit2)
                {
                    i++;
                    if (i == 1 && _Format2 == "") continue;
                    if (i == 1) _Key = _Format2;
                    if (i == 2) _Value = _Format2;
                    if (i > 2) _Value += ' ' + _Format2;
                }
                if (_Key != "")
                {
                    if (PROP_Value.Contains(_Key))
                    {
                    }
                    else
                    {
                        PROP_Value.Add(_Key, _Value);
                    }
                }
            }
        }

        /// <summary>МЕТОД Есть ли такой параметр?</summary>
        /// <param name="pParamentr">Сам параметр</param>
        public bool MET_If(string pParamentr)
        {
            return PROP_Value.ContainsKey(pParamentr);
        }

        /// <summary>МЕТОД Добавляет параметр или если есть, меняет значение параметра</summary>
        /// <param name="pParamentr">Сам параметр</param>
        /// <param name="pValue">Значение параметра (по умолчанию, пусто)</param>
        public void MET_Add(string pParamentr, string pValue = "")
        {
            PROP_Value[pParamentr] = pValue;
        }
    }
}
