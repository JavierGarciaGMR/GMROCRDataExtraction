using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace GMROCRDataExtraction.Utils
{
    public class Credentials
    {
        public string bigqueryAccessPath = "C:\\Users\\javiergarcia\\Documents\\GMR_Code_Apps\\BigqueryAccess\\depart-team-1-prod-svc-a19m-7bc3426b03b8.json";

        public string projectId = "depart-team-1-prod-svc-a19m";

        public string vendorKey = "3ghg55jzE,3zJe7c(+WH51jG08]gn8Munj1bx2in7ay8840o39{y2x8t00fZHr3y10Xd!8LiH9]Gw239(uR!a9dje1O!Rf,16dyv1L,c6]3s7h0o3USm4bTu540m4549";

        public string vendorName = "AppsConsultants";

        public string localHost = "https://localhost:44302/api/";

        public string qaEnvironment = "https://QA-genericapidmz.gmr.net/api/";

        public string dataSet = "dataset_qh_entity_extraction";
        public string dataTable = "gmr_hitl";
    }
}
