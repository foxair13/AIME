using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Config.Model
{
    public class Settings
    {
        public string ApiKey { get; set; }
        public bool IsEnabled { get; set; }
        public int MaxItemCount { get; set; }
    }
}
