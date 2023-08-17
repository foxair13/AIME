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
        ObjectItems,

        [TableText("IndicatorValues")]
        JsonCoordinates,
        IndicatorValues,

        [TableText("ObjectOnRoad")]
        ObjectOnRoad,

        [TableText("JsonADL")]
        JsonADL
    }
}
