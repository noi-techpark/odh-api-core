using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class GeoShapes
    {
        public int id { get; set; }
        public string country { get; set; }
        public int? code_rip { get; set; }
        public int? code_reg { get; set; }
        public int? code_prov { get; set; }
        public int? code_cm { get; set; }
        public int? code_uts { get; set; }
        public string? istatnumber { get; set; }
        public string? abbrev { get; set; }
        public string? type_uts { get; set; }
        public string? name { get; set; }
        public string? name_alternative { get; set; }

        public float shape_leng { get; set; }
        public float shape_area { get; set; }

        public string geom { get; set; }
    }
}
