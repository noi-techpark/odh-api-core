using AspNetCore.CacheOutput;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        /// <summary>
        /// GET LicenseCount
        /// </summary>
        /// <returns>LicenseCountResult Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(LicenseCountResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [CacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 36000, CacheKeyGenerator = typeof(CustomCacheKeyGenerator))]
        [HttpGet, Route("LicenseCount")]
        public async Task<IActionResult> GetLicenseCount(
            CancellationToken cancellationToken = default)
        {
            return await GetAllLicenseCount(cancellationToken);
        }

        private async Task<IActionResult> GetAllLicenseCount(CancellationToken cancellationToken)
        {
            LicenseCountResult result = new LicenseCountResult();

            result.AccoTotaldataopen = await Helper.PostgresLicenseCountHelper.GetTotalCountOpendata(QueryFactory, "accommodations");
            result.GastroTotaldataopen = await Helper.PostgresLicenseCountHelper.GetTotalCountOpendata(QueryFactory, "gastronomies");
            result.EventTotaldataopen = await Helper.PostgresLicenseCountHelper.GetTotalCountOpendata(QueryFactory, "events");
            result.PoiTotaldataopen = await Helper.PostgresLicenseCountHelper.GetTotalCountOpendata(QueryFactory, "pois");
            result.ActivityTotaldataopen = await Helper.PostgresLicenseCountHelper.GetTotalCountOpendata(QueryFactory, "activities");
            result.RegionTotaldataopen = await Helper.PostgresLicenseCountHelper.GetTotalCountOpendata(QueryFactory, "regions");
            result.TVTotaldataopen = await Helper.PostgresLicenseCountHelper.GetTotalCountOpendata(QueryFactory, "tvs");
            result.SkiAreaTotaldataopen = await Helper.PostgresLicenseCountHelper.GetTotalCountOpendata(QueryFactory, "skiareas");
            result.ODHActivityPoiTotaldataopen = await Helper.PostgresLicenseCountHelper.GetTotalCountOpendata(QueryFactory, "smgpois");

            result.AccoTotaldata = await Helper.PostgresLicenseCountHelper.GetTotalCount(QueryFactory, "accommodations");
            result.GastroTotaldata = await Helper.PostgresLicenseCountHelper.GetTotalCount(QueryFactory, "gastronomies");
            result.EventTotaldata = await Helper.PostgresLicenseCountHelper.GetTotalCount(QueryFactory, "events");
            result.PoiTotaldata = await Helper.PostgresLicenseCountHelper.GetTotalCount(QueryFactory, "pois");
            result.ActivityTotaldata = await Helper.PostgresLicenseCountHelper.GetTotalCount(QueryFactory, "activities");
            result.RegionTotaldata = await Helper.PostgresLicenseCountHelper.GetTotalCount(QueryFactory, "regions");
            result.TVTotaldata = await Helper.PostgresLicenseCountHelper.GetTotalCount(QueryFactory, "tvs");
            result.SkiAreaTotaldata = await Helper.PostgresLicenseCountHelper.GetTotalCount(QueryFactory, "skiareas");
            result.ODHActivityPoiTotaldata = await Helper.PostgresLicenseCountHelper.GetTotalCount(QueryFactory, "smgpois");

            result.AccoLicenses = await Helper.PostgresLicenseCountHelper.GetAllDataWithCC0Image(QueryFactory, "accommodations");
            result.GastroLicenses = await Helper.PostgresLicenseCountHelper.GetAllDataWithCC0Image(QueryFactory, "gastronomies");
            result.EventLicenses = await Helper.PostgresLicenseCountHelper.GetAllDataWithCC0Image(QueryFactory, "events");
            result.PoiLicenses = await Helper.PostgresLicenseCountHelper.GetAllDataWithCC0Image(QueryFactory, "pois");
            result.ActivityLicenses = await Helper.PostgresLicenseCountHelper.GetAllDataWithCC0Image(QueryFactory, "activities");
            result.RegionLicenses = await Helper.PostgresLicenseCountHelper.GetAllDataWithCC0Image(QueryFactory, "regions");
            result.TVLicenses = await Helper.PostgresLicenseCountHelper.GetAllDataWithCC0Image(QueryFactory, "tvs");
            result.SkiAreaLicenses = await Helper.PostgresLicenseCountHelper.GetAllDataWithCC0Image(QueryFactory, "skiareas");
            result.ODHActivityPoiLicenses = await Helper.PostgresLicenseCountHelper.GetAllDataWithCC0Image(QueryFactory, "smgpois");

            result.AccoImagesopenCount = await Helper.PostgresLicenseCountHelper.GetAllImagesWithCC0License(QueryFactory, "accommodations");
            result.GastroImagesopenCount = await Helper.PostgresLicenseCountHelper.GetAllImagesWithCC0License(QueryFactory, "gastronomies");
            result.EventImagesopenCount = await Helper.PostgresLicenseCountHelper.GetAllImagesWithCC0License(QueryFactory, "events");
            result.PoiImagesopenCount = await Helper.PostgresLicenseCountHelper.GetAllImagesWithCC0License(QueryFactory, "pois");
            result.ActivityImagesopenCount = await Helper.PostgresLicenseCountHelper.GetAllImagesWithCC0License(QueryFactory, "activities");
            result.RegionImagesopenCount = await Helper.PostgresLicenseCountHelper.GetAllImagesWithCC0License(QueryFactory, "regions");
            result.TVImagesopenCount = await Helper.PostgresLicenseCountHelper.GetAllImagesWithCC0License(QueryFactory, "tvs");
            result.SkiAreaImagesopenCount = await Helper.PostgresLicenseCountHelper.GetAllImagesWithCC0License(QueryFactory, "skiareas");
            result.ODHActivityPoiImagesopenCount = await Helper.PostgresLicenseCountHelper.GetAllImagesWithCC0License(QueryFactory, "smgpois");

            return Ok(result);
        }
    }

    public class LicenseCountResult
    {
        public long AccoLicenses { get; set; }
        public long GastroLicenses { get; set; }
        public long EventLicenses { get; set; }
        public long PoiLicenses { get; set; }
        public long ActivityLicenses { get; set; }
        public long RegionLicenses { get; set; }
        public long TVLicenses { get; set; }
        public long SkiAreaLicenses { get; set; }
        public long ODHActivityPoiLicenses { get; set; }

        public long AccoTotaldata { get; set; }
        public long GastroTotaldata { get; set; }
        public long EventTotaldata { get; set; }
        public long PoiTotaldata { get; set; }
        public long ActivityTotaldata { get; set; }
        public long RegionTotaldata { get; set; }
        public long TVTotaldata { get; set; }
        public long SkiAreaTotaldata { get; set; }
        public long ODHActivityPoiTotaldata { get; set; }

        public long AccoTotaldataopen { get; set; }
        public long GastroTotaldataopen { get; set; }
        public long EventTotaldataopen { get; set; }
        public long PoiTotaldataopen { get; set; }
        public long ActivityTotaldataopen { get; set; }
        public long RegionTotaldataopen { get; set; }
        public long TVTotaldataopen { get; set; }
        public long SkiAreaTotaldataopen { get; set; }
        public long ODHActivityPoiTotaldataopen { get; set; }

        public long AccoImagesopenCount { get; set; }
        public long GastroImagesopenCount { get; set; }
        public long EventImagesopenCount { get; set; }
        public long PoiImagesopenCount { get; set; }
        public long ActivityImagesopenCount { get; set; }
        public long RegionImagesopenCount { get; set; }
        public long TVImagesopenCount { get; set; }
        public long SkiAreaImagesopenCount { get; set; }
        public long ODHActivityPoiImagesopenCount { get; set; }
    }
}
