using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.other
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class LicenseCountController : OdhController
    {
        public LicenseCountController(IWebHostEnvironment env, ISettings settings, ILogger<LicenseCountController> logger, QueryFactory queryFactory)
           : base(env, settings, logger, queryFactory)
        {
        }


    }

    public class LicenseCountResult
    {
        public int AccoLicenses { get; set; }
        public int GastroLicenses { get; set; }
        public int EventLicenses { get; set; }
        public int PoiLicenses { get; set; }
        public int ActivityLicenses { get; set; }
        public int RegionLicenses { get; set; }
        public int TVLicenses { get; set; }
        public int SkiAreaLicenses { get; set; }
        public int ODHActivityPoiLicenses { get; set; }

        public int AccoTotaldata { get; set; }
        public int GastroTotaldata { get; set; }
        public int EventTotaldata { get; set; }
        public int PoiTotaldata { get; set; }
        public int ActivityTotaldata { get; set; }
        public int RegionTotaldata { get; set; }
        public int TVTotaldata { get; set; }
        public int SkiAreaTotaldata { get; set; }
        public int ODHActivityPoiTotaldata { get; set; }

        public int AccoTotaldataopen { get; set; }
        public int GastroTotaldataopen { get; set; }
        public int EventTotaldataopen { get; set; }
        public int PoiTotaldataopen { get; set; }
        public int ActivityTotaldataopen { get; set; }
        public int RegionTotaldataopen { get; set; }
        public int TVTotaldataopen { get; set; }
        public int SkiAreaTotaldataopen { get; set; }
        public int ODHActivityPoiTotaldataopen { get; set; }

        public int AccoImagesopenCount { get; set; }
        public int GastroImagesopenCount { get; set; }
        public int EventImagesopenCount { get; set; }
        public int PoiImagesopenCount { get; set; }
        public int ActivityImagesopenCount { get; set; }
        public int RegionImagesopenCount { get; set; }
        public int TVImagesopenCount { get; set; }
        public int SkiAreaImagesopenCount { get; set; }
        public int ODHActivityPoiImagesopenCount { get; set; }
    }
}
