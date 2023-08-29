using NeftViewer.MVC.Filters;

namespace NeftViewer.MVC.Enums
{
    public enum TableEnum
    {
        [TableText("Criterias")]
        Criterias,

        [TableText("Roads")]
        Roads,


        [TableText("JsonOwner")]
        JsonOwner,

        [TableText("AreaOwner")]
        JsonArea,

        [TableText("Objects")]
        ObjectItems,
        JsonCoordinates,

        [TableText("ObjectOnRoad")]
        ObjectOnRoad,

        [TableText("IndicatorValues")]
        IndicatorValues,

    }
}
