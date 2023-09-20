namespace NeftViewer.MVC.Models
{
    public class Point
    {
        public string id { get; set; }
        public double Lon { get; set; } 
        public double Lat { get; set; }
        public string Name { get; set; }
        public bool HasValue { get; set; }
        public decimal Value { get; set; }
        public string scaleUnit { get; set;}
    }
}
