using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Edge.Models
{
    public class Ressource
    {
        public string Memory { get; set; }
        public string Disk { get; set; }
        public string CPU { get; set; }
        public string CPUPercent { get; set; }
        public string VCPU { get; set; }

    }
}
