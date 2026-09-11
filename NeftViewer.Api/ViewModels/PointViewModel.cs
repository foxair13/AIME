#region 
using NeftViewer.MVC.Options;
[assembly: Prot("J1Tfy2RuM_RBtYdogsBYsNvzrOUfu_PsKkN0rf9JjZg")]
//[assembly: Prot("D")]
#endregion
namespace NeftViewer.Api.ViewModels
{
    public class PointViewModel
    {
            public string id { get; set; }
            public double Lon { get; set; }
            public double Lat { get; set; }
            public string Name { get; set; }
    }
}
