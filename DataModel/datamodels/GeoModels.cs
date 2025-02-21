// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class GeoShapeDB
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

        public string geometry { get; set; }

        //Table
        public string type { get; set; }

        public JsonRaw lincenseinfo { get; set; }

        public JsonRaw meta { get; set; }

        public string source { get; set; }

    }

    public class GeoShapeJson
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public int? Code_Rip { get; set; }
        public int? Code_Reg { get; set; }
        public int? Code_Prov { get; set; }
        public int? Code_Cm { get; set; }
        public int? Code_Uts { get; set; }

        public string Istatnumber { get; set; }

        public string Abbrev { get; set; }

        public string Type_Uts { get; set; }

        public string Name { get; set; }

        public string Name_Alternative { get; set; }

        public float? Shape_length { get; set; }

        public float? Shape_area { get; set; }

        public string Source { get; set; }

        public string Type { get; set; }

        public Metadata _Meta { get; set; }

        public LicenseInfo LicenseInfo { get; set; }

        public string Geometry { get; set; }
    }
}
