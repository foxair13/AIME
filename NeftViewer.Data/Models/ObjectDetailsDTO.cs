using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Models
{
    public class ObjectDetailsDTO
    {
        public int OwnerId { get; set; }
        public int? RoadId { get; set; } 
        public int AreaId { get; set; }
        public string CodeSUID { get; set; }
    }
}
