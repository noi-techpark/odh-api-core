using AspNetCore.CacheOutput;
using DataModel;
using Helper;
using Microsoft.AspNetCore.Authorization;
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
        /// <param name="pagenumber">Pagenumber</param>
        /// <param name="pagesize">Elements per Page, (default:10)</param>
        /// <param name="seed">Seed '1 - 10' for Random Sorting, '0' generates a Random Seed, 'null' disables Random Sorting, (default:null)</param>
        /// <param name="articletype">Type of the Article ('null' = Filter disabled, possible values: BITMASK values: 1 = basearticle, 2 = book article, 4 = contentarticle, 8 = eventarticle, 16 = pressarticle, 32 = recipe, 64 = touroperator , 128 = b2b, 256  = idmarticle, 512 = specialannouncement, 1024 = newsfeednoi), (also possible for compatibily reasons: basisartikel, buchtippartikel, contentartikel, veranstaltungsartikel, presseartikel, rezeptartikel, reiseveranstalter, b2bartikel ) (default:'255' == ALL), REFERENCE TO: GET /api/ArticleTypes</param>
        /// <param name="articlesubtype">Sub Type of the Article (depends on the Maintype of the Article 'null' = Filter disabled)</param>
        /// <param name="idlist">IDFilter (Separator ',' List of Article IDs), (default:'null')</param>
        /// <param name="startdate">Filter by ArticleDate Format (yyyy-MM-dd HH:mm)</param>
        /// <param name="enddate">Filter by ArticleDate Format (yyyy-MM-dd HH:mm)</param>
        /// <param name="sortbyarticledate">Sort By Articledate ('true' sorts Articles by Articledate)</param>
        /// <param name="odhtagfilter">ODH Taglist Filter (refers to Array SmgTags) (String, Separator ',' more Tags possible, available Tags reference to 'v1/ODHTag?validforentity=article'), (default:'null')</param>                
        /// <param name="active">Active Articles Filter (possible Values: 'true' only Active Articles, 'false' only Disabled Articles), (default:'null')</param>
        /// <param name="odhactive">ODH Active (Published) Articles Filter (Refers to field OdhActive) (possible Values: 'true' only published Article, 'false' only not published Articles), (default:'null')</param>        
        /// <param name="source">Filter by Source (Separator ','), (Sources available 'idm','noi'...),(default: 'null')</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="langfilter">Language filter (returns only data available in the selected Language, Separator ',' possible values: 'de,it,en,nl,sc,pl,fr,ru', 'null': Filter disabled)</param>
        /// <param name="publishedon">Published On Filter (Separator ',' List of publisher IDs), (default:'null')</param>       
        /// <param name="updatefrom">Returns data changed after this date Format (yyyy-MM-dd), (default: 'null')</param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of Article Objects</returns>        
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(JsonResult<ArticlesLinked>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[OdhCacheOutput(ClientTimeSpan = 0, ServerTimeSpan = 3600, CacheKeyGenerator = typeof(CustomCacheKeyGenerator), MustRevalidate = true)]
        [HttpGet, Route("Article")]
        public async Task<IActionResult> GetArticleList(
            string? language = null,
            uint pagenumber = 1,
            PageSize pagesize = null!,
            string? articletype = null,
            string? articlesubtype = null,
            string? idlist = null,
            string? langfilter = null,
            LegacyBool sortbyarticledate = null!,
            string? odhtagfilter = null,
            LegacyBool odhactive = null!,
            LegacyBool active = null!,
            string? updatefrom = null,
            string? startdate = null,
            string? enddate = null,
            string? seed = null,
            string? publishedon = null,
            string? source = null,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {

            return await GetFiltered(
                fields: fields ?? Array.Empty<string>(), language: language, pagenumber: pagenumber, pagesize: pagesize,
                type: articletype, subtypefilter: articlesubtype, searchfilter: searchfilter, idfilter: idlist, languagefilter: langfilter, highlightfilter: null,
                active: active?.Value, smgactive: odhactive?.Value, smgtags: odhtagfilter, seed: seed,  
                articledate: startdate, articledateto: enddate, source: source, lastchange: updatefrom, sortbyarticledate: sortbyarticledate?.Value, publishedon: publishedon,
                rawfilter: rawfilter, rawsort: rawsort, removenullvalues: removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Article Single 
        /// </summary>
        /// <param name="id">ID of the Article</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Article Object</returns>
        /// <response code="200">Object created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ArticlesLinked), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("Article/{id}", Name = "SingleArticle")]
        public async Task<IActionResult> GetArticleSingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);
        }

        //Special GETTER

        /// <summary>
        /// GET Article Types List
        /// </summary>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="searchfilter">String to search for, Title in all languages are searched, (default: null) <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#searchfilter" target="_blank">Wiki searchfilter</a></param>
        /// <param name="rawfilter"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawfilter" target="_blank">Wiki rawfilter</a></param>
        /// <param name="rawsort"><a href="https://github.com/noi-techpark/odh-docs/wiki/Using-rawfilter-and-rawsort-on-the-Tourism-Api#rawsort" target="_blank">Wiki rawsort</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>Collection of ArticleTypes Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(IEnumerable<ArticleTypes>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ArticleTypes")]
        public async Task<IActionResult> GetAllArticleTypesList(
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            string? searchfilter = null,
            string? rawfilter = null,
            string? rawsort = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetArticleTypesList(language, fields: fields ?? Array.Empty<string>(), searchfilter, rawfilter, rawsort, removenullvalues, cancellationToken);
        }

        /// <summary>
        /// GET Article Types Single
        /// </summary>
        /// <param name="id">ID of the Article Type</param>
        /// <param name="language">Language field selector, displays data and fields in the selected language (default:'null' all languages are displayed)</param>
        /// <param name="fields">Select fields to display, More fields are indicated by separator ',' example fields=Id,Active,Shortname (default:'null' all fields are displayed). <a href="https://github.com/noi-techpark/odh-docs/wiki/Common-parameters%2C-fields%2C-language%2C-searchfilter%2C-removenullvalues%2C-updatefrom#fields" target="_blank">Wiki fields</a></param>
        /// <param name="removenullvalues">Remove all Null values from json output. Useful for reducing json size. By default set to false. Documentation on <a href='https://github.com/noi-techpark/odh-docs/wiki/Common-parameters,-fields,-language,-searchfilter,-removenullvalues,-updatefrom#removenullvalues' target="_blank">Opendatahub Wiki</a></param>        
        /// <returns>ArticleTypes Object</returns>                
        /// <response code="200">List created</response>
        /// <response code="400">Request Error</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(typeof(ArticleTypes), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet, Route("ArticleTypes/{id}", Name = "SingleArticleTypes")]
        public async Task<IActionResult> GetAllArticlesTypeTypesSingle(
            string id,
            string? language,
            [ModelBinder(typeof(CommaSeparatedArrayBinder))]
            string[]? fields = null,
            bool removenullvalues = false,
            CancellationToken cancellationToken = default)
        {
            return await GetArticleTypeSingle(id, language, fields: fields ?? Array.Empty<string>(), removenullvalues, cancellationToken);
        }

        #endregion

        #region GETTER

        private Task<IActionResult> GetFiltered(string[] fields, string? language, uint pagenumber, int? pagesize,
            string? type, string? subtypefilter, string? searchfilter, string? idfilter, string? languagefilter, bool? highlightfilter,
            bool? active, bool? smgactive, string? smgtags, string? seed, string? articledate, string? articledateto, string? source, string? lastchange, 
            bool? sortbyarticledate, string? publishedon, string? rawfilter, string? rawsort, bool removenullvalues,
            CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                ArticleHelper myarticlehelper = ArticleHelper.Create(
                    type, subtypefilter, idfilter, languagefilter, highlightfilter,
                    active, smgactive, smgtags, articledate, articledateto, source, lastchange, publishedon);

                if(sortbyarticledate == true)
                    rawsort = "ArticleDate";

                var query =
                    QueryFactory.Query()
                        .SelectRaw("data")
                        .From("articles")
                        .ArticleWhereExpression(
                            idlist: myarticlehelper.idlist, typelist: myarticlehelper.typelist,
                            subtypelist: myarticlehelper.subtypelist, smgtaglist: myarticlehelper.smgtaglist, languagelist: myarticlehelper.languagelist,
                            highlight: myarticlehelper.highlight, activefilter: myarticlehelper.active, smgactivefilter: myarticlehelper.smgactive,
                            articledate: myarticlehelper.articledate, articledateto: myarticlehelper.articledateto, sourcelist: myarticlehelper.sourcelist,
                            publishedonlist: myarticlehelper.publishedonlist,
                            searchfilter: searchfilter, language: language, lastchange: myarticlehelper.lastchange,
                            filterClosedData: FilterClosedData, reducedData: ReducedData)
                        .ApplyRawFilter(rawfilter)
                        .ApplyOrdering_GeneratedColumns(ref seed, new PGGeoSearchResult() { geosearch = false }, rawsort);
                      

                // Get paginated data
                var data =
                    await query
                        .PaginateAsync<JsonRaw>(
                            page: (int)pagenumber,
                            perPage: pagesize ?? 25);
                
                var fieldsTohide = FieldsToHide;

                var dataTransformed =
                    data.List.Select(
                        raw => raw.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
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

        private Task<IActionResult> GetSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("articles")
                        .Select("data")
                        .Where("id", id.ToUpper())
                        //.When(FilterClosedData, q => q.FilterClosedData())
                        .Anonymous_Logged_UserRule_GeneratedColumn(FilterClosedData, ReducedData);

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                var fieldsTohide = FieldsToHide;

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }

        #endregion

        #region CATEGORIES

        private Task<IActionResult> GetArticleTypesList(string? language, string[] fields, string? searchfilter, string? rawfilter, string? rawsort, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("articletypes")
                        .SelectRaw("data")
                        .When(FilterClosedData, q => q.FilterClosedData())
                        .SearchFilter(PostgresSQLWhereBuilder.TypeDescFieldsToSearchFor(language), searchfilter)
                        .ApplyRawFilter(rawfilter)
                        .OrderOnlyByRawSortIfNotNull(rawsort);

                var data = await query.GetAsync<JsonRaw?>();
                
                var fieldsTohide = FieldsToHide;

                var dataTransformed =
                      data.Select(
                          raw => raw?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide)
                      );

                return dataTransformed;
            });
        }

        private Task<IActionResult> GetArticleTypeSingle(string id, string? language, string[] fields, bool removenullvalues, CancellationToken cancellationToken)
        {
            return DoAsyncReturn(async () =>
            {
                var query =
                    QueryFactory.Query("articletypes")
                        .Select("data")
                        //.WhereJsonb("Key", "ilike", id)
                        .Where("id", id.ToLower())
                        .When(FilterClosedData, q => q.FilterClosedData());
                
                var fieldsTohide = FieldsToHide;

                var data = await query.FirstOrDefaultAsync<JsonRaw?>();

                return data?.TransformRawData(language, fields, checkCC0: FilterCC0License, filterClosedData: FilterClosedData, filteroutNullValues: removenullvalues, urlGenerator: UrlGenerator, fieldstohide: fieldsTohide);
            });
        }


        #endregion

        #region POST PUT DELETE

        /// <summary>
        /// POST Insert new Article
        /// </summary>
        /// <param name="article">Article Object</param>
        /// <returns>Http Response</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataCreate,ArticleManager,ArticleCreate")]
        [InvalidateCacheOutput(nameof(GetArticleList))]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost, Route("Article")]
        public Task<IActionResult> Post([FromBody] ArticlesLinked article)
        {            
            return DoAsyncReturn(async () =>
            {
                //GENERATE ID
                article.Id = Helper.IdGenerator.GenerateIDFromType(article);

                article.CheckMyInsertedLanguages(new List<string> { "de", "en", "it" });

                if(article.ArticleDateTo == null)
                    article.ArticleDateTo = DateTime.MaxValue;

                return await UpsertData<ArticlesLinked>(article, "articles", true);
            });
        }

        /// <summary>
        /// PUT Modify existing Article
        /// </summary>
        /// <param name="id">Article Id</param>
        /// <param name="article">Article Object</param>
        /// <returns>Http Response</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataModify,ArticleManager,ArticleModify")]
        [InvalidateCacheOutput(nameof(GetArticleList))]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut, Route("Article/{id}")]
        public Task<IActionResult> Put(string id, [FromBody] ArticlesLinked article)
        {            
            return DoAsyncReturn(async () =>
            {
                //Check ID uppercase lowercase
                Helper.IdGenerator.CheckIdFromType(article);

                article.CheckMyInsertedLanguages(new List<string> { "de", "en", "it" });

                if (article.ArticleDateTo == null)
                    article.ArticleDateTo = DateTime.MaxValue;

                return await UpsertData<ArticlesLinked>(article, "articles", false, true);
            });
        }

        /// <summary>
        /// DELETE Article by Id
        /// </summary>
        /// <param name="id">Article Id</param>
        /// <returns>Http Response</returns>
        //[ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = "DataWriter,DataDelete,ArticleManager,ArticleDelete")]
        [InvalidateCacheOutput(nameof(GetArticleList))]
        [ProducesResponseType(typeof(PGCRUDResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete, Route("Article/{id}")]
        public Task<IActionResult> Delete(string id)
        {
            return DoAsyncReturn(async () =>
            {
                //Check ID uppercase lowercase
                id = Helper.IdGenerator.CheckIdFromType<ArticlesLinked>(id);

                return await DeleteData(id, "articles");
            });
        }


        #endregion
    }
}
