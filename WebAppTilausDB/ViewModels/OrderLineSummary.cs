using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace WebAppTilausDB.ViewModels
{
    public class OrderLineSummary
    {
        public int TilausriviID { get; set; }
        public Nullable<int> TuoteID { get; set; }

        [Display(Name = "Määrä")]
        public Nullable<int> Maara { get; set; }

        public Nullable<decimal> Ahinta { get; set; }

    }
}