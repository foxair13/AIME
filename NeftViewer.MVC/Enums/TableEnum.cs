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
        //[TableText("IndicatorValues")]
        //IndicatorValues,
        [TableText("ObjectOnRoad")]
        ObjectOnRoad,

       

        [TableText("Energies")]
        Energies,
    }
}
