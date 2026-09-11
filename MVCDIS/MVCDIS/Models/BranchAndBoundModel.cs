using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCDIS.Models
{

        public class Pbound
        {
            public double[,] M { get; set; }
            public double Fi { get; set; }
            public int RibCol { get; set; }
            public int[,] Ribs { get; set; }


        }

}