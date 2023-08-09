using NeftViewer.MVC.Filters;

namespace NeftViewer.MVC.Enums
{
    public enum TableEnum
    {
        [TableText("Criterias")]
        Criterias,

        [TableText("Roads")]
        Roads,

        [TableText("Customers")]
        Customers,

        [TableText("Objects")]
        Objects,

        [TableText("ObjectOnRoad")]
        ObjectOnRoad,

        [TableText("IndicatorValues")]
        IndicatorValues
    }
}
