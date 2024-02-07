// SPDX-FileCopyrightText: NOI Techpark <digital@noi.bz.it>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using DataModel;
using Helper;
using Helper.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdhApiCore.Responses;
using OdhNotifier;
using ServiceReferenceLCS;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    public class CommonController : OdhController
    {
        public CommonController(IWebHostEnvironment env, ISettings settings, ILogger<CommonController> logger, QueryFactory queryFactory, IOdhPushNotifier odhpushnotifier)
       : base(env, settings, logger, queryFactory, odhpushnotifier)
        {            
        }

        #region SWAGGER Exposed API        

        //Standard GETTER

        /// <summary>
        /// GET MetaRegion List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of data IDs), (default:'null')</param>
        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=common'), (default:'null')</param>
        /// <param name="active">Active data Filter (possible Values: 'true' only Active data, 'false' only Disabled data), (default:'null')</param>
        /// <param name="odhactive">Odhactive (Published) data Filter (possible Values: 'true' only published data, 'false' only not published data, (default:'null')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of MetaRegion Objects</returns>     
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<MetaRegionLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[Authorize(Roles = "DataReader,CommonReader")]
        [HttpGet, Route("MetaRegion")]
        public async Task<IActionResult> GetMetaRegions(
            uint? pagenumber = null,
            PageSize pagesize = null!,
            string? idlist = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? source = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null,
            string? seed = null,
            string? publishedon = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {                        
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, idfilter: idlist, languagefilter: langfilter, null, source, active?.Value, odhactive?.Value, odhtagfilter, lastchange: updatefrom, publishedon, cancellationToken);

            if (pagenumber.HasValue)
            {
                return await CommonGetPagedListHelper(pagenumber.Value, pagesize, tablename: "metaregions", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
            else
            {
                return await CommonGetListHelper(tablename: "metaregions", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }                
        }

        /// <summary>
        /// GET MetaRegion Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>MetaRegion Object</returns>        
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(MetaRegionLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("MetaRegion/{id}", Name = "SingleMetaRegion")]
        public async Task<IActionResult> GetMetaRegionSingle(
            string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {            
            return await CommonGetSingleHelper(id: id, tablename: "metaregions", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Experiencearea List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of data IDs), (default:'null')</param>
        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=common'), (default:'null')</param>
        /// <param name="active">Active data Filter (possible Values: 'true' only Active data, 'false' only Disabled data), (default:'null')</param>
        /// <param name="odhactive">Odhactive (Published) data Filter (possible Values: 'true' only published data, 'false' only not published data, (default:'null')</param>
        /// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of ExperienceArea Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<ExperienceAreaLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ExperienceArea")]
        public async Task<IActionResult> GetExperienceAreas(
            uint? pagenumber = null,
            PageSize pagesize = null!, 
            string? idlist = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? source = null,
            bool? visibleinsearch = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null,
            string? seed = null,
            string? publishedon = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {          
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, idfilter: idlist, languagefilter: langfilter, visibleinsearch, source, activefilter: active?.Value, 
                smgactivefilter: odhactive?.Value, smgtags: odhtagfilter, lastchange: updatefrom, publishedonfilter: publishedon, cancellationToken);

            if (pagenumber.HasValue)
            {
                return await CommonGetPagedListHelper(pagenumber.Value, pagesize, tablename: "experienceareas", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
            else
            {
                return await CommonGetListHelper(tablename: "experienceareas", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
        }

        /// <summary>
        /// GET ExperienceArea Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>ExperienceArea Object</returns>        
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ExperienceAreaLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ExperienceArea/{id}", Name ="SingleExperienceArea")]
        public async Task<IActionResult> GetExperienceAreaSingle(string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "experienceareas", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Region List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of data IDs), (default:'null')</param>
        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=common'), (default:'null')</param>
        /// <param name="active">Active data Filter (possible Values: 'true' only Active data, 'false' only Disabled data), (default:'null')</param>
        /// <param name="odhactive">Odhactive (Published) data Filter (possible Values: 'true' only published data, 'false' only not published data, (default:'null')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of Region Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<RegionLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Region")]
        public async Task<IActionResult> GetRegions(
            uint? pagenumber = null,
            PageSize pagesize = null!, 
            string? idlist = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? source = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null,
            string? seed = null,
            string? publishedon = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, idfilter: idlist, languagefilter: langfilter, null, source,
                activefilter: active?.Value, smgactivefilter: odhactive?.Value, smgtags: odhtagfilter, lastchange: updatefrom, publishedonfilter: publishedon, cancellationToken);

            if (pagenumber.HasValue)
            {
                return await CommonGetPagedListHelper(pagenumber.Value, pagesize, tablename: "regions", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
            else
            {
                return await CommonGetListHelper(tablename: "regions", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
        }

        /// <summary>
        /// GET Region Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Region Object</returns>        
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(RegionLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Region/{id}", Name ="SingleRegion")]
        public async Task<IActionResult> GetRegionSingle(string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "regions", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET TourismAssociation List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of data IDs), (default:'null')</param>
        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=common'), (default:'null')</param>
        /// <param name="active">Active data Filter (possible Values: 'true' only Active data, 'false' only Disabled data), (default:'null')</param>
        /// <param name="odhactive">Odhactive (Published) data Filter (possible Values: 'true' only published data, 'false' only not published data, (default:'null')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of Tourismverein Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<TourismvereinLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("TourismAssociation")]
        public async Task<IActionResult> GetTourismverein(
            uint? pagenumber = null,
            PageSize pagesize = null!, 
            string? idlist = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? source = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null, 
            string? seed = null,
            string? publishedon = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, idfilter: idlist, languagefilter: langfilter, null, source,
                activefilter: active?.Value, smgactivefilter: odhactive?.Value, smgtags: odhtagfilter, lastchange: updatefrom, publishedonfilter: publishedon, cancellationToken);

            if (pagenumber.HasValue)
            {
                return await CommonGetPagedListHelper(pagenumber.Value, pagesize, tablename: "tvs", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
            else
            {
                return await CommonGetListHelper(tablename: "tvs", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }

        }

        /// <summary>
        /// GET TourismAssociation Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Tourismverein Object</returns>        
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(TourismvereinLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("TourismAssociation/{id}", Name = "SingleTourismAssociation")]
        public async Task<IActionResult> GetTourismvereinSingle(string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "tvs", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Municipality List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of data IDs), (default:'null')</param>
        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=common'), (default:'null')</param>
        /// <param name="active">Active data Filter (possible Values: 'true' only Active data, 'false' only Disabled data), (default:'null')</param>
        /// <param name="odhactive">Odhactive (Published) data Filter (possible Values: 'true' only published data, 'false' only not published data, (default:'null')</param>
        /// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of Municipality Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<MunicipalityLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Municipality")]
        public async Task<IActionResult> GetMunicipality(
            uint? pagenumber = null,
            PageSize pagesize = null!, 
            bool? visibleinsearch = null,
            string? idlist = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? source = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null, 
            string? seed = null,
            string? publishedon = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, idfilter: idlist, languagefilter: langfilter, visibleinsearch, source,
                active?.Value, odhactive?.Value, smgtags: odhtagfilter, lastchange: updatefrom, publishedonfilter: publishedon, cancellationToken);

            if (pagenumber.HasValue)
            {
                return await CommonGetPagedListHelper(pagenumber.Value, pagesize, tablename: "municipalities", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
            else
            {
                return await CommonGetListHelper(tablename: "municipalities", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }

        }

        /// <summary>
        /// GET Municipality Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Municipality Object</returns>        
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(MunicipalityLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Municipality/{id}", Name = "SingleMunicipality")]
        public async Task<IActionResult> GetMunicipalitySingle(string id, 
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "municipalities", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET District List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of data IDs), (default:'null')</param>
        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=common'), (default:'null')</param>
        /// <param name="active">Active data Filter (possible Values: 'true' only Active data, 'false' only Disabled data), (default:'null')</param>
        /// <param name="odhactive">Odhactive (Published) data Filter (possible Values: 'true' only published data, 'false' only not published data, (default:'null')</param>
        /// <param name="visibleinsearch">Filter only Elements flagged with visibleinsearch: (possible values: 'true','false'), (default:'false')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of District Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<DistrictLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("District")]
        public async Task<IActionResult> GetDistrict(
            uint? pagenumber = null,
            PageSize pagesize = null!,
            string? idlist = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? source = null,
            bool? visibleinsearch = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null,
            string? seed = null,
            string? publishedon = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {            
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, idfilter: idlist, languagefilter: langfilter, visibleinsearch, source,
                active?.Value, odhactive?.Value, smgtags: odhtagfilter, lastchange: updatefrom, publishedonfilter: publishedon, cancellationToken);

            if (pagenumber.HasValue)
            {
                return await CommonGetPagedListHelper(pagenumber.Value, pagesize, tablename: "districts", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                    language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
            else
            {
                return await CommonGetListHelper(tablename: "districts", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                    language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);                
            }

        }

        /// <summary>
        /// GET District Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>District Object</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(DistrictLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("District/{id}", Name = "SingleDistrict")]
        public async Task<IActionResult> GetDistrictSingle(string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "districts", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }


        /// <summary>
        /// GET Area List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of data IDs), (default:'null')</param>
        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=common'), (default:'null')</param>
        /// <param name="active">Active data Filter (possible Values: 'true' only Active data, 'false' only Disabled data), (default:'null')</param>
        /// <param name="odhactive">Odhactive (Published) data Filter (possible Values: 'true' only published data, 'false' only not published data, (default:'null')</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of Area Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<AreaLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Area")]
        public async Task<IActionResult> GetAreas(
            uint? pagenumber = null,
            PageSize pagesize = null!, 
            string? idlist = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? source = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null,
            string? seed = null,
            string? publishedon = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, idfilter: idlist, languagefilter: langfilter, null, source,
                active?.Value, odhactive?.Value, smgtags: odhtagfilter, lastchange: updatefrom, publishedonfilter: publishedon, cancellationToken);

            if (pagenumber.HasValue)
            {
                return await CommonGetPagedListHelper(pagenumber.Value, pagesize, tablename: "areas", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: new PGGeoSearchResult(), rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
            else
            {
                return await CommonGetListHelper(tablename: "areas", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: new PGGeoSearchResult(), rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
        }

        /// <summary>
        /// GET Area Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Area Object</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(AreaLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Area/{id}", Name = "SingleArea")]
        public async Task<IActionResult> GetAreaSingle(string id, 
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "areas", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues : removenullvalues, cancellationToken);
        }


        /// <summary>
        /// GET SkiRegion List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of data IDs), (default:'null')</param>
        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=common'), (default:'null')</param>
        /// <param name="active">Active data Filter (possible Values: 'true' only Active data, 'false' only Disabled data), (default:'null')</param>
        /// <param name="odhactive">Odhactive (Published) data Filter (possible Values: 'true' only published data, 'false' only not published data, (default:'null')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of SkiRegion Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<SkiRegionLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("SkiRegion")]
        public async Task<IActionResult> GetSkiRegion(
            uint? pagenumber = null,
            PageSize pagesize = null!, 
            string? idlist = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? source = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null,
            string? seed = null,
            string? publishedon = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, idfilter: idlist, languagefilter: langfilter, null, source,
                active?.Value, odhactive?.Value, smgtags: odhtagfilter, lastchange: updatefrom, publishedonfilter: publishedon, cancellationToken);

            if (pagenumber.HasValue)
            {
                return await CommonGetPagedListHelper(pagenumber.Value, pagesize, tablename: "skiregions", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: new PGGeoSearchResult(), rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
            else
            {
                return await CommonGetListHelper(tablename: "skiregions", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: new PGGeoSearchResult(), rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
        }

        /// <summary>
        /// GET SkiRegion Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>SkiRegion Object</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(SkiRegionLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("SkiRegion/{id}", Name ="SingleSkiRegion")]
        public async Task<IActionResult> GetSkiRegionSingle(string id, 
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "skiregions", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET SkiArea List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of data IDs), (default:'null')</param>
        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=common'), (default:'null')</param>
        /// <param name="active">Active data Filter (possible Values: 'true' only Active data, 'false' only Disabled data), (default:'null')</param>
        /// <param name="odhactive">Odhactive (Published) data Filter (possible Values: 'true' only published data, 'false' only not published data, (default:'null')</param>
        /// <param name="latitude">GeoFilter FLOAT Latitude Format: '46.624975', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="longitude">GeoFilter FLOAT Longitude Format: '11.369909', 'null' = disabled, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="radius">Radius INTEGER to Search in Meters. Only Object withhin the given point and radius are returned and sorted by distance. Random Sorting is disabled if the GeoFilter Informations are provided, (default:'null') <a href='https://github.com/noi-techpark/odh-docs/wiki/Geosorting-and-Locationfilter-usage#geosorting-functionality' target="_blank">Wiki geosort</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of SkiArea Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<SkiAreaLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("SkiArea")]
        public async Task<IActionResult> GetSkiArea(
            uint? pagenumber = null,
            PageSize pagesize = null!, 
            string? idlist = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? source = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null, 
            string? seed = null,
            string? publishedon = null,
            string? searchfilter = null,
            string? latitude = null,
            string? longitude = null,
            string? radius = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            var geosearchresult = Helper.GeoSearchHelper.GetPGGeoSearchResult(latitude, longitude, radius);

            CommonHelper commonhelper = await CommonHelper.CreateAsync(QueryFactory, idfilter: idlist, languagefilter: langfilter, null, source,
                active?.Value, odhactive?.Value, smgtags: odhtagfilter, lastchange: updatefrom, publishedonfilter: publishedon, cancellationToken);

            if (pagenumber.HasValue)
            {
                return await CommonGetPagedListHelper(pagenumber.Value, pagesize, tablename: "skiareas", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
            else
            {
                return await CommonGetListHelper(tablename: "skiareas", seed: seed, publishedon: publishedon, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, geosearchresult: geosearchresult, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
        }

        /// <summary>
        /// GET SkiArea Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>SkiArea Object</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(SkiAreaLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("SkiArea/{id}", Name ="SingleSkiArea")]
        public async Task<IActionResult> GetSkiAreaSingle(string id,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "skiareas", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }


        ////Special GETTER

        /// <summary>
        /// GET Wine Awards List
        /// </summary>
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of data IDs), (default:'null')</param>
        /// <param name="odhtagfilter">Taglist Filter (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=common'), (default:'null')</param>
        /// <param name="active">Active data Filter (possible Values: 'true' only Active data, 'false' only Disabled data), (default:'null')</param>
        /// <param name="odhactive">Odhactive (Published) data Filter (possible Values: 'true' only published data, 'false' only not published data, (default:'null')</param>
        /// <param name="companyid">Filter by Company Id, (default:'null')</param>
        /// <param name="wineid">Filter by Wine Id, (default:'null')</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<WineLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("WineAward")]
        public async Task<IActionResult> GetWineAwardsList(
            uint? pagenumber = null,
            PageSize pagesize = null!, 
            string? idlist = null,
            string? odhtagfilter = null,
            LegacyBool active = null!,
            LegacyBool odhactive = null!,
            string? source = null,
            string? wineid = null,
            string? companyid = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            string? langfilter = null,
            string? updatefrom = null,
            string? seed = null,
            string? rawfilter = null,
            string? rawsort = null,
            string? publishedon = null,
            string? searchfilter = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default
            )
        {
            WineHelper commonhelper = await WineHelper.CreateAsync(QueryFactory, idfilter: idlist, companyid, wineid, languagefilter: langfilter, null, source,
                active?.Value, odhactive?.Value, smgtags: odhtagfilter, lastchange: updatefrom, cancellationToken);

            if (pagenumber.HasValue)
            {
                return await WineGetPagedListHelper(pagenumber.Value, pagesize, tablename: "wines", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
            else
            {
                return await WineGetListHelper(tablename: "wines", seed: seed, searchfilter: searchfilter, fields: fields ?? Array.Empty<string>(),
                language: language, commonhelper, rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
            }
        }

        /// <summary>
        /// GET Wine Award Single
        /// </summary>
        /// <param name="id">ID of the requested data</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Wine Object</returns>        
        [ProducesResponseType(typeof(WineLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("WineAward/{id}", Name ="SingleWineAward")]
        public async Task<IActionResult> GetWineAwardsSingle(string id, 
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? language = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await CommonGetSingleHelper(id: id, tablename: "wines", fields: fields ?? Array.Empty<string>(), language: language, removenullvalues: removenullvalues, cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> CommonGetListHelper(string tablename, string? seed, string? publishedon, string? searchfilter, string[] fields, string? language, CommonHelper commonhelper, PGGeoSearchResult geosearchresult, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From(tablename)
                        .CommonWhereExpression(idlist: commonhelper.idlist, languagelist: commonhelper.languagelist, visibleinsearch: commonhelper.visibleinsearch, commonhelper.smgtaglist,
                                               activefilter: commonhelper.active, odhactivefilter: commonhelper.smgactive, publishedonlist: commonhelper.publishedonlist, sourcelist: commonhelper.sourcelist, searchfilter: searchfilter, language: language, 
                                               lastchange: commonhelper.lastchange, userroles: UserRolesToFilter)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering_GeneratedColumns(ref seed, geosearchresult, rawsort); //.ApplyOrdering(ref seed, new PGGeoSearchResult() { geosearch = false }, rawsort);

                // Get paginated data
                var data =
                    await query
                        .GetAsync<JsonRaw>();
                                
                return
                    data.Select(
                        raw => raw.TransformRawData(language, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: FieldsToHide)
                    );                
            });
        }

        private Task<IActionResult> CommonGetPagedListHelper(uint pagenumber, int? pagesize, string tablename, string? seed, string? publishedon, string? searchfilter, string[] fields, string? language, CommonHelper commonhelper, PGGeoSearchResult geosearchresult, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From(tablename)
                        .CommonWhereExpression(idlist: commonhelper.idlist, languagelist: commonhelper.languagelist, visibleinsearch: commonhelper.visibleinsearch, commonhelper.smgtaglist,
                                               activefilter: commonhelper.active, odhactivefilter: commonhelper.smgactive, publishedonlist: commonhelper.publishedonlist, sourcelist: commonhelper.sourcelist, searchfilter: searchfilter, language: language,
                                               lastchange: commonhelper.lastchange, userroles: UserRolesToFilter)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering_GeneratedColumns(ref seed, geosearchresult, rawsort);

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: pagesize ?? 25);
                
                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: FieldsToHide)
                    );

                uint totalpages = (uint)data.TotalPages;
                uint totalcount = (uint)data.Count;

                return ResponseHelpers.GetResult(
                    pagenumber,
                    totalpages,
                    totalcount,
                    seed,
                    dataTransformed,
                    Url);
            });
        }

        private Task<IActionResult> WineGetListHelper(string tablename, string? seed, string? searchfilter, string[] fields, string? language, WineHelper winehelper, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From(tablename)
                        .WineWhereExpression(languagelist: new List<string>(), lastchange: winehelper.lastchange, wineid: winehelper.wineidlist, companyid: winehelper.companyidlist,
                                             activefilter: winehelper.active, odhactivefilter: winehelper.smgactive, sourcelist: winehelper.sourcelist,
                                               searchfilter: searchfilter, language: language, userroles: UserRolesToFilter)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering(ref seed, new PGGeoSearchResult() { geosearch = false }, rawsort);

                // Get paginated data
                var data =
                    await query
                        .GetAsync<JsonRaw>();
                             
                return
                    data.Select(
                        raw => raw.TransformRawData(language, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: FieldsToHide)
                    );                
            });
        }

        private Task<IActionResult> WineGetPagedListHelper(uint pagenumber, int? pagesize, string tablename, string? seed, string? searchfilter, string[] fields, string? language, WineHelper winehelper, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            //Additional Read Filters to Add Check
            AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From(tablename)
                        .WineWhereExpression(languagelist: new List<string>(), lastchange: winehelper.lastchange, wineid: winehelper.wineidlist, companyid: winehelper.companyidlist,
                                             activefilter: winehelper.active, odhactivefilter: winehelper.smgactive, sourcelist: winehelper.sourcelist,
                                               searchfilter: searchfilter, language: language, userroles: UserRolesToFilter)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering(ref seed, new PGGeoSearchResult() { geosearch = false }, rawsort);

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: pagesize ?? 25);
             
                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: FieldsToHide)
                    );

                uint totalpages = (uint)data.TotalPages;
                uint totalcount = (uint)data.Count;

                return ResponseHelpers.GetResult(
                    pagenumber,
                    totalpages,
                    totalcount,
                    seed,
                    dataTransformed,
                    Url);
            });
        }

        private Task<IActionResult> CommonGetSingleHelper(string id, string tablename, string[] fields, string? language, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Read Filters to Add Check
                AdditionalFiltersToAdd.TryGetValue("Read", out var additionalfilter);

                var query =
                    QueryFactory.Query(tablename)
                        .Select("data")
                        .Where("id", id.ToUpper());
                                
                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: FieldsToHide);
            });
        }

        #endregion

        #region POST PUT DELETE

        /// <summary>
        /// POST Insert new MetaRegion
        /// </summary>
        /// <param name="data">MetaRegion Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate,MetaRegionCreate")]
        [AuthorizeODH(PermissionAction.Create)]
        [HttpPost, Route("MetaRegion")]
        public Task<IActionResult> Post([FromBody] MetaRegionLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                data.Id = Helper.IdGenerator.GenerateIDFromType(data);
                return await UpsertData<MetaRegionLinked>(data, "metaregions", true, false, "api", additionalfilter);
            });
        }

        /// <summary>
        /// POST Insert new Region
        /// </summary>
        /// <param name="data">Region Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate,RegionCreate")]
        [AuthorizeODH(PermissionAction.Create)]
        [HttpPost, Route("Region")]
        public Task<IActionResult> Post([FromBody] RegionLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                data.Id = Helper.IdGenerator.GenerateIDFromType(data);
                return await UpsertData<RegionLinked>(data, "regions", true, false, "api", additionalfilter);
            });
        }

        /// <summary>
        /// POST Insert new ExperienceArea
        /// </summary>
        /// <param name="data">ExperienceArea Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate,ExperienceAreaCreate")]
        [AuthorizeODH(PermissionAction.Create)]
        [HttpPost, Route("ExperienceArea")]
        public Task<IActionResult> Post([FromBody] ExperienceAreaLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                data.Id = Helper.IdGenerator.GenerateIDFromType(data);
                return await UpsertData<ExperienceAreaLinked>(data, "experienceareas", true, false, "api", additionalfilter);
            });
        }

        /// <summary>
        /// POST Insert new TourismAssociation
        /// </summary>
        /// <param name="data">TourismAssociation Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate,TourismAssociationCreate")]
        [AuthorizeODH(PermissionAction.Create)]
        [HttpPost, Route("TourismAssociation")]
        public Task<IActionResult> Post([FromBody] TourismvereinLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                data.Id = Helper.IdGenerator.GenerateIDFromType(data);
                return await UpsertData<TourismvereinLinked>(data, "tvs", true, false, "api", additionalfilter);
            });
        }

        /// <summary>
        /// POST Insert new Municipality
        /// </summary>
        /// <param name="data">Municipality Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate,MunicipalityCreate")]
        [AuthorizeODH(PermissionAction.Create)]
        [HttpPost, Route("Municipality")]
        public Task<IActionResult> Post([FromBody] MunicipalityLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                data.Id = Helper.IdGenerator.GenerateIDFromType(data);
                return await UpsertData<MunicipalityLinked>(data, "municipalities", true, false, "api", additionalfilter);
            });
        }

        /// <summary>
        /// POST Insert new District
        /// </summary>
        /// <param name="data">District Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate,DistrictCreate")]
        [AuthorizeODH(PermissionAction.Create)]
        [HttpPost, Route("District")]
        public Task<IActionResult> Post([FromBody] DistrictLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                data.Id = Helper.IdGenerator.GenerateIDFromType(data);
                return await UpsertData<DistrictLinked>(data, "districts", true, false, "api", additionalfilter);
            });
        }

        /// <summary>
        /// POST Insert new Area
        /// </summary>
        /// <param name="data">Area Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate,AreaCreate")]
        [AuthorizeODH(PermissionAction.Create)]
        [HttpPost, Route("Area")]
        public Task<IActionResult> Post([FromBody] AreaLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                data.Id = Helper.IdGenerator.GenerateIDFromType(data);
                return await UpsertData<AreaLinked>(data, "areas", true, false, "api", additionalfilter);
            });
        }

        /// <summary>
        /// POST Insert new SkiRegion
        /// </summary>
        /// <param name="data">SkiRegion Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate,SkiRegionCreate")]
        [AuthorizeODH(PermissionAction.Create)]
        [HttpPost, Route("SkiRegion")]
        public Task<IActionResult> Post([FromBody] SkiRegionLinked data)
        {
            return DoAsyncReturn(async () =>
            {  
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                data.Id = Helper.IdGenerator.GenerateIDFromType(data);
                return await UpsertData<SkiRegionLinked>(data, "skiregions", true, false, "api", additionalfilter);
            });
        }

        /// <summary>
        /// POST Insert new SkiArea
        /// </summary>
        /// <param name="data">SkiArea Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate,SkiAreaCreate")]
        [AuthorizeODH(PermissionAction.Create)]
        [HttpPost, Route("SkiArea")]
        public Task<IActionResult> Post([FromBody] SkiAreaLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                data.Id = Helper.IdGenerator.GenerateIDFromType(data);
                return await UpsertData<SkiAreaLinked>(data, "skiareas", true, false, "api", additionalfilter);
            });
        }

        /// <summary>
        /// POST Insert new Wine
        /// </summary>
        /// <param name="data">Wine Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataCreate,CommonManager,CommonCreate,WineAwardCreate")]
        [AuthorizeODH(PermissionAction.Create)]
        [HttpPost, Route("WineAward")]
        public Task<IActionResult> Post([FromBody] WineLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Create
                AdditionalFiltersToAdd.TryGetValue("Create", out var additionalfilter);

                data.Id = Helper.IdGenerator.GenerateIDFromType(data);
                return await UpsertData<WineLinked>(data, "wines", true, false, "api", additionalfilter);
            });
        }

        /// <summary>
        /// PUT Modify existing MetaRegion
        /// </summary>
        /// <param name="id">MetaRegion Id</param>
        /// <param name="data">MetaRegion Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify,CommonUpdate,MetaRegionUpdate")]
        [AuthorizeODH(PermissionAction.Update)]
        [HttpPut, Route("MetaRegion/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] MetaRegionLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Update
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                data.Id = Helper.IdGenerator.CheckIdFromType<MetaRegionLinked>(id);
                return await UpsertData<MetaRegionLinked>(data, "metaregions", false, true, "api", additionalfilter);
            });
        }

        /// <summary>
        /// PUT Modify existing Region
        /// </summary>
        /// <param name="id">Region Id</param>
        /// <param name="data">Region Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify,CommonUpdate,RegionUpdate")]
        [AuthorizeODH(PermissionAction.Update)]
        [HttpPut, Route("Region/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] RegionLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Update
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                data.Id = Helper.IdGenerator.CheckIdFromType<RegionLinked>(id);
                return await UpsertData<RegionLinked>(data, "regions", false, true, "api", additionalfilter);
            });
        }

        /// <summary>
        /// PUT Modify existing ExperienceArea
        /// </summary>
        /// <param name="id">ExperienceArea Id</param>
        /// <param name="data">ExperienceArea Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify,CommonUpdate,ExperienceAreaUpdate")]
        [AuthorizeODH(PermissionAction.Update)]
        [HttpPut, Route("ExperienceArea/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] ExperienceAreaLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Update
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                data.Id = Helper.IdGenerator.CheckIdFromType<ExperienceAreaLinked>(id);
                return await UpsertData<ExperienceAreaLinked>(data, "experienceareas", false, true, "api", additionalfilter);
            });
        }

        /// <summary>
        /// PUT Modify existing TourismAssociation
        /// </summary>
        /// <param name="id">TourismAssociation Id</param>
        /// <param name="data">TourismAssociation Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify,CommonUpdate,TourismAssociationUpdate")]
        [AuthorizeODH(PermissionAction.Update)]
        [HttpPut, Route("TourismAssociation/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] TourismvereinLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Update
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                data.Id = Helper.IdGenerator.CheckIdFromType<TourismvereinLinked>(id);
                return await UpsertData<TourismvereinLinked>(data, "tvs", false, true, "api", additionalfilter);
            });
        }

        /// <summary>
        /// PUT Modify existing Municipality
        /// </summary>
        /// <param name="id">Municipality Id</param>
        /// <param name="data">Municipality Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify,CommonUpdate,MunicipalityUpdate")]
        [AuthorizeODH(PermissionAction.Update)]
        [HttpPut, Route("Municipality/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] MunicipalityLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Update
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                data.Id = Helper.IdGenerator.CheckIdFromType<MunicipalityLinked>(id);
                return await UpsertData<MunicipalityLinked>(data, "municipalities", false, true, "api", additionalfilter);
            });
        }

        /// <summary>
        /// PUT Modify existing District
        /// </summary>
        /// <param name="id">District Id</param>
        /// <param name="data">District Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify,CommonUpdate,DistrictUpdate")]
        [AuthorizeODH(PermissionAction.Update)]
        [HttpPut, Route("District/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] DistrictLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Update
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                data.Id = Helper.IdGenerator.CheckIdFromType<DistrictLinked>(id);
                return await UpsertData<DistrictLinked>(data, "districts", false, true, "api", additionalfilter);
            });
        }

        /// <summary>
        /// PUT Modify existing Area
        /// </summary>
        /// <param name="id">Area Id</param>
        /// <param name="data">Area Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify,CommonUpdate,AreaUpdate")]
        [AuthorizeODH(PermissionAction.Update)]
        [HttpPut, Route("Area/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] AreaLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Update
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                data.Id = Helper.IdGenerator.CheckIdFromType<AreaLinked>(id);
                return await UpsertData<AreaLinked>(data, "areas", false, true, "api", additionalfilter);
            });
        }

        /// <summary>
        /// PUT Modify existing SkiRegion
        /// </summary>
        /// <param name="id">SkiRegion Id</param>
        /// <param name="data">SkiRegion Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify,CommonUpdate,SkiRegionUpdate")]
        [AuthorizeODH(PermissionAction.Update)]
        [HttpPut, Route("SkiRegion/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] SkiRegionLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Update
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                data.Id = Helper.IdGenerator.CheckIdFromType<SkiRegionLinked>(id);
                return await UpsertData<SkiRegionLinked>(data, "skiregions", false, true, "api", additionalfilter);
            });
        }

        /// <summary>
        /// PUT Modify existing SkiArea
        /// </summary>
        /// <param name="id">SkiArea Id</param>
        /// <param name="data">SkiArea Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify,CommonUpdate,SkiAreaUpdate")]
        [AuthorizeODH(PermissionAction.Update)]
        [HttpPut, Route("SkiArea/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] SkiAreaLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Update
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                data.Id = Helper.IdGenerator.CheckIdFromType<SkiAreaLinked>(id);
                return await UpsertData<SkiAreaLinked>(data, "skiareas", false, true, "api", additionalfilter);
            });
        }

        /// <summary>
        /// PUT Modify existing WineAward
        /// </summary>
        /// <param name="id">WineAward Id</param>
        /// <param name="data">WineAward Object</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataModify,CommonManager,CommonModify,CommonUpdate,WineAwardUpdate")]
        [AuthorizeODH(PermissionAction.Update)]
        [HttpPut, Route("WineAward/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] WineLinked data)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Update
                AdditionalFiltersToAdd.TryGetValue("Update", out var additionalfilter);

                data.Id = Helper.IdGenerator.CheckIdFromType<WineLinked>(id);
                return await UpsertData<WineLinked>(data, "wines", false, true, "api", additionalfilter);
            });
        }

        /// <summary>
        /// DELETE MetaRegion by Id
        /// </summary>
        /// <param name="id">MetaRegion Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete,MetaRegionDelete")]
        [AuthorizeODH(PermissionAction.Delete)]
        [HttpDelete, Route("MetaRegion/{id}")]
        public Task<IActionResult> DeleteMetaRegion(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Delete
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                id = Helper.IdGenerator.CheckIdFromType<MetaRegionLinked>(id);
                return await DeleteData<MetaRegionLinked>(id, "metaregions", additionalfilter);
            });
        }

        /// <summary>
        /// DELETE Region by Id
        /// </summary>
        /// <param name="id">Region Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete,RegionDelete")]
        [AuthorizeODH(PermissionAction.Delete)]
        [HttpDelete, Route("Region/{id}")]
        public Task<IActionResult> DeleteRegion(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Delete
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                id = Helper.IdGenerator.CheckIdFromType<RegionLinked>(id);
                return await DeleteData<RegionLinked>(id, "regions", additionalfilter);
            });
        }

        /// <summary>
        /// DELETE ExperienceArea by Id
        /// </summary>
        /// <param name="id">ExperienceArea Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete,ExperienceAreaDelete")]
        [AuthorizeODH(PermissionAction.Delete)]
        [HttpDelete, Route("ExperienceArea/{id}")]
        public Task<IActionResult> DeleteExperienceArea(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Delete
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                id = Helper.IdGenerator.CheckIdFromType<ExperienceAreaLinked>(id);
                return await DeleteData<ExperienceAreaLinked>(id, "experienceareas", additionalfilter);
            });
        }

        /// <summary>
        /// DELETE TourismAssociation by Id
        /// </summary>
        /// <param name="id">TourismAssociation Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete,TourismAssociationDelete")]
        [AuthorizeODH(PermissionAction.Delete)]
        [HttpDelete, Route("TourismAssociation/{id}")]
        public Task<IActionResult> DeleteTourismAssociation(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Delete
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                id = Helper.IdGenerator.CheckIdFromType<TourismvereinLinked>(id);
                return await DeleteData<TourismvereinLinked>(id, "tvs", additionalfilter);
            });
        }

        /// <summary>
        /// DELETE Municipality by Id
        /// </summary>
        /// <param name="id">Municipality Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete,MunicipalityDelete")]
        [AuthorizeODH(PermissionAction.Delete)]
        [HttpDelete, Route("Municipality/{id}")]
        public Task<IActionResult> DeleteMunicipality(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Delete
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                id = Helper.IdGenerator.CheckIdFromType<MunicipalityLinked>(id);
                return await DeleteData<MunicipalityLinked>(id, "municipalities", additionalfilter);
            });
        }

        /// <summary>
        /// DELETE District by Id
        /// </summary>
        /// <param name="id">District Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete,DistrictDelete")]
        [AuthorizeODH(PermissionAction.Delete)]
        [HttpDelete, Route("District/{id}")]
        public Task<IActionResult> DeleteDistrict(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Delete
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                id = Helper.IdGenerator.CheckIdFromType<DistrictLinked>(id);
                return await DeleteData<DistrictLinked>(id, "districts", additionalfilter);
            });
        }

        /// <summary>
        /// DELETE Area by Id
        /// </summary>
        /// <param name="id">Area Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete,AreaDelete")]
        [AuthorizeODH(PermissionAction.Delete)]
        [HttpDelete, Route("Area/{id}")]
        public Task<IActionResult> DeleteArea(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Delete
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                id = Helper.IdGenerator.CheckIdFromType<AreaLinked>(id);
                return await DeleteData<AreaLinked>(id, "areas", additionalfilter);
            });
        }

        /// <summary>
        /// DELETE SkiRegion by Id
        /// </summary>
        /// <param name="id">SkiRegion Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete,SkiRegionDelete")]
        [AuthorizeODH(PermissionAction.Delete)]
        [HttpDelete, Route("SkiRegion/{id}")]
        public Task<IActionResult> DeleteSkiRegion(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Delete
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                id = Helper.IdGenerator.CheckIdFromType<SkiRegionLinked>(id);
                return await DeleteData<SkiRegionLinked>(id, "skiregions", additionalfilter);
            });
        }

        /// <summary>
        /// DELETE SkiArea by Id
        /// </summary>
        /// <param name="id">SkiArea Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete,SkiAreaDelete")]
        [AuthorizeODH(PermissionAction.Delete)]
        [HttpDelete, Route("SkiArea/{id}")]
        public Task<IActionResult> DeleteSkiArea(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Delete
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                id = Helper.IdGenerator.CheckIdFromType<SkiAreaLinked>(id);
                return await DeleteData<SkiAreaLinked>(id, "skiareas", additionalfilter);
            });
        }

        /// <summary>
        /// DELETE WineAward by Id
        /// </summary>
        /// <param name="id">WineAward Id</param>
        /// <returns>Http Response</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        //[Authorize(Roles = "DataWriter,DataDelete,CommonManager,CommonDelete,WineAwardDelete")]
        [AuthorizeODH(PermissionAction.Delete)]
        [HttpDelete, Route("WineAward/{id}")]
        public Task<IActionResult> DeleteWineAward(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Additional Filters on the Action Delete
                AdditionalFiltersToAdd.TryGetValue("Delete", out var additionalfilter);

                id = Helper.IdGenerator.CheckIdFromType<WineLinked>(id);
                return await DeleteData<WineLinked>(id, "wines", additionalfilter);
            });
        }

        #endregion
    }
}
