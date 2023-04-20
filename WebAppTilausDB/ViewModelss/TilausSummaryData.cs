using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;

namespace WebAppTilausDB.ViewModels
{
    public class TilausSummaryData
    {
        public int TilausID{ get; set; }
        public Nullable<System.DateTime> TilausPvm { get; set; }
        public Nullable<System.DateTime> ToimitusPvm { get; set; }

        public Nullable<int> AsiakasID { get; set; }

        public string Toimitusosoite { get; set; }
    }
}