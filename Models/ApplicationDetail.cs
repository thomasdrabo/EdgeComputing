using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Edge.Models
{
    public class ApplicationDetail
    {
        public string AppID { get; set; }
        public string Owner { get; set; }
        public string State { get; set; }
        public Details Application { get; set; }
        public string ActivatedProfileName { get; set; }
        public Ressource RessourceReservation { get; set; }
    }
}
