using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCDIS.Models
{
    public class Keys
    {
        [Display(Name = "Ключ ID")]
        public string MashineID { get; set; }
        [Display(Name = "Серийный номер")]
        public string UserKey { get; set; }

    }
}