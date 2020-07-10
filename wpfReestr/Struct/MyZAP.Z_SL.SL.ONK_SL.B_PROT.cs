namespace wpfReestr
{
    /// <summary>КЛАСС Cведения об противопоказаниях и отказах</summary>
    /// <remarks>Справочник Q018; тип файла C, T; путь ZL_LIST/ZAP/Z_SL/SL/ONK_SL/B_PROT</remarks> 
    public class MyB_PROT
    {
        /// <summary>Код противопоказания или отказа (Справочник N001; из json NOM_USL)</summary>
        public int PROT { get; set; }

        /// <summary>Дата регистрации противопоказания или отказа (из json NOM_USL)</summary>
        public string D_PROT { get; set; }
    }
}
