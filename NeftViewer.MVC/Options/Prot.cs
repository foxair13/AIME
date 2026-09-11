
namespace NeftViewer.MVC.Options
{

    [AttributeUsage(AttributeTargets.Assembly)]
    public class Prot : Attribute
    {
        public string? P { get; }
        public Prot(string? p)
        {
            P = p;
        }
    }
}
