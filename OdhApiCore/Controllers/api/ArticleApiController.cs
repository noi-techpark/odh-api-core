using DataModel;
using Helper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OdhApiCore.Responses;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OdhApiCore.Controllers.api
{
    /// <summary>
    /// Articles Api (data provided by IDM) SOME DATA Available as OPENDATA 
    /// </summary>
    [EnableCors("CorsPolicy")]
    [NullStringParameterActionFilter]
    public class ArticleController : OdhController
    {
        public ArticleController(IWebHostEnvironment env, ISettings settings, ILogger<ArticleController> logger, QueryFactory queryFactory)
           : base(env, settings, logger, queryFactory)
        {
        }

        #region SWAGGER Exposed API

        //Standard GETTER

        /// <summary>
        /// GET Article List
        /// </summary>
        /// <param name="pagenumber">Pagenumber, (default:1)</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="articletype">Type of the Article ('null' = Filter disabled, possible values: BITMASK values: 1 = basearticle, 2 = book article, 4 = contentarticle, 8 = eventarticle, 16 = pressarticle, 32 = recipe, 64 = touroperator , 128 = b2b), (also possible for compatibily reasons: basisartikel, buchtippartikel, contentartikel, veranstaltungsartikel, presseartikel, rezeptartikel, reiseveranstalter, b2bartikel ) (default:'255' == ALL), REFERENCE TO: GET /api/ArticleTypes</param>
        /// <param name="articlesubtype">Sub Type of the Article (depends on the Maintype of the Article 'null' = Filter disabled)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of Article IDs), (default:'null')</param>
        /// <param name="langfilter">Language Filter (Gets only Articles Available in the passed Language)</param>
        /// <param name="sortbyarticledate">Sort By Articledate ('true' sorts Articles by Articledate)</param>
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible, available Tags reference to 'api/ODHTag?validforentity=article'), (default:'null')</param>                
        /// <param name="active">Active Articles Filter (possible Values: 'true' only Active Articles, 'false' only Disabled Articles</param>
        /// <param name="odhactive">ODH Active (Published) Activities Filter (Refers to field SmgActive) Article Filter (possible Values: 'true' only published Article, 'false' only not published Articles, (default:'null')</param>        
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null)</param>
        /// <returns>Collection of Article Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<Article>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Article")]
        public async Task<IActionResult> GetArticleList(
            string? language = null,
            uint pagenumber = 1,
            PageSize pagesize = null!,
            string? articletype = "255",
            string? articlesubtype = null,
            string? idlist = null,
            string? langfilter = null,
            LegacyBool sortbyarticledate = null!,
            string? odhtagfilter = null,
            LegacyBool odhactive = null!,
            LegacyBool active = null!,
            string? updatefrom = null,
            string? seed = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            CancellationToken cancellationToken = default)
        {

            return await GetFiltered(
                fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber, pagesize: pagesize,
                type: articletype, subtypefilter: articlesubtype, searchfilter: searchfilter, idfilter: idlist, languagefilter: langfilter, highlightfilter: null,
                active: active?.Value, smgactive: odhactive?.Value, smgtags: odhtagfilter, seed: seed, lastchange: updatefrom, sortbyarticledate: sortbyarticledate?.Value,
                rawfilter: rawfilter, rawsort: rawsort, cancellationToken);
        }

        /// <summary>
        /// GET Article Single 
        /// </summary>
        /// <param name="id">ID of the Poi</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname. Select also Dictionary fields, example Detail.de.Title, or Elements of Arrays example ImageGallery[0].ImageUrl. (default:'null' all fields are displayed)</param>
        /// <param name="language">Language field selector, displays data and fields available in the selected language (default:'null' all languages are displayed)</param>
        /// <returns>Article Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(Article), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Article/{id}", Name = "SingleArticle")]
        public async Task<IActionResult> GetArticleSingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            CancellationToken cancellationToken = default)
        {
            return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), cancellationToken);
        }

        //Special GETTER

        /// <summary>
        /// GET Article Types List
        /// </summary>
        /// <returns>Collection of ArticleTypes Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<ArticleTypes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ArticleTypes")]
        public async Task<IActionResult> GetAllArticleTypesList(CancellationToken cancellationToken)
        {
            return await GetArticleTypesList(cancellationToken);
        }

        /// <summary>
        /// GET Article Types Single
        /// </summary>
        /// <returns>ArticleTypes Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ArticleTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ArticleTypes/{id}", Name = "SingleArticleTypes")]
        public async Task<IActionResult> GetAllArticlesTypeTypesSingle(string id, CancellationToken cancellationToken)
        {
            return await GetArticleTypeSingle(id, cancellationToken);
        }


        #endregion

        #region GETTER

        private Task<IActionResult> GetFiltered(string[] fields, string? language, uint pagenumber, int? pagesize,
            string? type, string? subtypefilter, string? searchfilter, string? idfilter, string? languagefilter, bool? highlightfilter,
            bool? active, bool? smgactive, string? smgtags, string? seed, string? lastchange, bool? sortbyarticledate, string? rawfilter, string? rawsort, 
            CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                ArticleHelper myrticlehelper = ArticleHelper.Create(
                    type, subtypefilter, idfilter, languagefilter, highlightfilter,
                    active, smgactive, smgtags, lastchange);

                //TODO orderby = "to_date(data#>>'\\{ArticleDate\\}', 'YYYY-MM-DD') DESC";

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("articles")
                        .ArticleWhereExpression(
                            idlist: myrticlehelper.idlist, typelist: myrticlehelper.typelist,
                            subtypelist: myrticlehelper.subtypelist, smgtaglist: myrticlehelper.smgtaglist, languagelist: myrticlehelper.languagelist,
                            highlight: myrticlehelper.highlight,
                            activefilter: myrticlehelper.active, smgactivefilter: myrticlehelper.smgactive,
                            searchfilter: searchfilter, language: language, lastchange: myrticlehelper.lastchange,
                            filterClosedData: FilterClosedData)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering(ref seed, new PGGeoSearchResult() { geosearch = false }, rawsort);

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: (int)pagesize);

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator, userroles: UserRolesList)
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

        private Task<IActionResult> GetSingle(string id, string? language, string[] fields, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("articles")
                        .Select("data")
                        .Where("id", id)
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, urlGenerator: UrlGenerator, userroles: UserRolesList);
            });
        }

        #endregion

        #region CUSTOM METODS

        private Task<IActionResult> GetArticleTypesList(CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("articletypes")
                        .SelectRaw("data")
                        .When(FilterClosedData, q => q.FilterClosedData());

                var data = await query.GetAsync<JsonRaw?>();

                return data;
            });
        }

        private Task<IActionResult> GetArticleTypeSingle(string id, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("articletypes")
                        .Select("data")
                        //.WhereJsonb("Key", "ilike", id)
                        .Where("id", id.ToLower())
                        .When(FilterClosedData, q => q.FilterClosedData());
                //.Where("Key", "ILIKE", id);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data;
            });
        }


        #endregion
    }
}
