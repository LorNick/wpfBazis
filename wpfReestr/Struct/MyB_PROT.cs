namespace wpfReestr
{
    /// <summary>КЛАСС Структура сведения об противопоказаниях и отказах -> MyONK_SL</summary>
    public class MyB_PROT
    {
        /// <summary>Код противопоказания или отказа (О,  N(1), N001)</summary>
        public int PROT { get; set; }

        /// <summary>Дата регистрации противопоказания или отказа (О, D)</summary>
        public string D_PROT { get; set; }
    }
}
