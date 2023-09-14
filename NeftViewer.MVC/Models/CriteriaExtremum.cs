namespace NeftViewer.MVC.Models
{
    public class CriteriaExtremum
    {
        public int CriteriaId { get; set; }
        public double MaxVal { get; set; }
        public double MinVal { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }

    }
}
