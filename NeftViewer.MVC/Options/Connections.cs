namespace NeftViewer.MVC.Options
{
    public class Connections
    {
        public string BasePostgree { get; set; }
        public string FinanceMssql { get; set; }
        public string CoordsUrl { get; set; }
        public string AsuUrl { get; set; }
        public string Prot { get; }

        // Конструктор с добавлением защищенного параметра
        public Connections(string prot)
        {
            BasePostgree = ""; // Укажите значения по умолчанию, если необходимо
            FinanceMssql = "";
            CoordsUrl = "";
            AsuUrl = "";
            Prot = prot;
        }
    }
}
