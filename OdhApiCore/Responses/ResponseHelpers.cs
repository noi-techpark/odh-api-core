using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OdhApiCore.Responses
{
    /// <summary>
    /// Marker interface
    /// </summary>
    public interface IResponse<T>
    {
        public IEnumerable<T> Items { get; }
    }

    public class JsonResult<T> : IResponse<T>
    {
        public uint TotalResults { get; set; }
        public uint TotalPages { get; set; }
        public uint CurrentPage { get; set; }
        public string? PreviousPage { get; set; }
        public string? NextPage { get; set; }
        public string? Seed { get; set; }
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    }

    public class JsonResultWithOnlineResults<T> : JsonResult<T>
    {
        public int OnlineResults { get; set; }
    }

    public class JsonResultWithOnlineResultsAndResultId<T> : JsonResultWithOnlineResults<T>
    {
        public string? ResultId { get; set; }
    }

    public class JsonResultWithOnlineResultsAndResultIdLowercase<T> : IResponse<T>
    {
        public uint totalResults { get; set; }
        public uint totalPages { get; set; }
        public uint currentPage { get; set; }
        public string? previousPage { get; set; }
        public string? nextPage { get; set; }
        public string? seed { get; set; }
        public int onlineResults { get; set; }
        public string? resultId { get; set; }
        public IEnumerable<T> items { get; set; } = Enumerable.Empty<T>();
        [JsonIgnore]
        public IEnumerable<T> Items => items;
    }

    public static class ResponseHelpers
    {
        private static (string? previouspage, string? nextpage) GetPreviousAndNextPage(uint pagenumber, uint totalpages, IUrlHelper? url, string? seed)
        {
            string? previouspage = null;
            string? nextpage = null;
            if (url != null)
            {
                var query = url.ActionContext.HttpContext.Request.Query;
                var queryDict = query.ToDictionary(x => x.Key, x => x.Value.ToString());
                if (seed != null)
                {
                    queryDict["seed"] = seed;
                }
                if (pagenumber > 1 && pagenumber <= totalpages)
                {
                    queryDict["pagenumber"] = (pagenumber - 1).ToString();
                    previouspage = url.Link(null, queryDict);
                }
                if (pagenumber < totalpages)
                {
                    queryDict["pagenumber"] = (pagenumber + 1).ToString();
                    nextpage = url.Link(null, queryDict);
                }
            }            
            return (previouspage, nextpage);
        }

        public static JsonResult<T> GetResult<T>(
            uint pagenumber, uint totalpages, uint totalcount, string? seed, IEnumerable<T> data, IUrlHelper? url)
        {
            var (previouspage, nextpage) = GetPreviousAndNextPage(pagenumber, totalpages, url, seed);
            return new JsonResult<T>
            {
                TotalResults = totalcount,
                TotalPages = totalpages,
                CurrentPage = pagenumber,
                PreviousPage = previouspage,
                NextPage = nextpage,
                Seed = seed,
                Items = data
            };
        }

        public static JsonResultWithOnlineResults<T> GetResult<T>(
            uint pagenumber, uint totalpages, uint totalcount, int onlineresults, string? seed,
            IEnumerable<T> data, IUrlHelper url)
            where T : notnull
        {
            var (previouspage, nextpage) = GetPreviousAndNextPage(pagenumber, totalpages, url, seed);
            return new JsonResultWithOnlineResults<T>
            {
                TotalResults = totalcount,
                TotalPages = totalpages,
                CurrentPage = pagenumber,
                PreviousPage = previouspage,
                NextPage = nextpage,
                OnlineResults = onlineresults,
                Seed = seed,
                Items = data
            };
        }

        public static JsonResultWithOnlineResultsAndResultId<T> GetResult<T>(
            uint pagenumber, uint totalpages, uint totalcount, int onlineresults,
            string resultid, string? seed, IEnumerable<T> data, IUrlHelper url)
            where T : notnull
        {
            var (previouspage, nextpage) = GetPreviousAndNextPage(pagenumber, totalpages, url, seed);
            return new JsonResultWithOnlineResultsAndResultId<T>
            {
                TotalResults = totalcount,
                TotalPages = totalpages,
                CurrentPage = pagenumber,
                PreviousPage = previouspage,
                NextPage = nextpage,
                OnlineResults = onlineresults,
                ResultId = resultid,
                Seed = seed,
                Items = data
            };
        }

        public static JsonResultWithOnlineResultsAndResultIdLowercase<T> GetResultLowercase<T>(
            uint pagenumber, uint totalpages, uint totalcount, int onlineresults,
            string resultid, string seed, IEnumerable<T> data, IUrlHelper url)
            where T : notnull
        {
            var (previouspage, nextpage) = GetPreviousAndNextPage(pagenumber, totalpages, url, seed);
            return new JsonResultWithOnlineResultsAndResultIdLowercase<T>
            {
                totalResults = totalcount,
                totalPages = totalpages,
                currentPage = pagenumber,
                previousPage = previouspage,
                nextPage = nextpage,
                onlineResults = onlineresults,
                resultId = resultid,
                seed = seed,
                items = data
            };
        }
    }
}
