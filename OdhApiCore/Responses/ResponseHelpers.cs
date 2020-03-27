using System;
using System.Collections.Generic;
using System.Linq;
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
        public string? seed { get; set; }
        public int onlineResults { get; set; }
        public string? resultId { get; set; }
        public IEnumerable<T> items { get; set; } = Enumerable.Empty<T>();
        [JsonIgnore]
        public IEnumerable<T> Items => items;
    }

    public static class ResponseHelpers
    {
        public static JsonResult<T> GetResult<T>(
            uint pagenumber, uint totalpages, uint totalcount, string? seed, IEnumerable<T> data)
            where T : notnull
        {
            return new JsonResult<T>
            {
                TotalResults = totalcount,
                TotalPages = totalpages,
                CurrentPage = pagenumber,
                Seed = seed,
                Items = data
            };
        }

        public static JsonResultWithOnlineResults<T> GetResult<T>(
            uint pagenumber, uint totalpages, uint totalcount, int onlineresults, string? seed,
            IEnumerable<T> data)
            where T : notnull
        {
            return new JsonResultWithOnlineResults<T>
            {
                TotalResults = totalcount,
                TotalPages = totalpages,
                CurrentPage = pagenumber,
                OnlineResults = onlineresults,
                Seed = seed,
                Items = data
            };
        }

        public static JsonResultWithOnlineResultsAndResultId<T> GetResult<T>(
            uint pagenumber, uint totalpages, uint totalcount, int onlineresults,
            string resultid, string? seed, IEnumerable<T> data)
            where T : notnull
        {
            return new JsonResultWithOnlineResultsAndResultId<T>
            {
                TotalResults = totalcount,
                TotalPages = totalpages,
                CurrentPage = pagenumber,
                OnlineResults = onlineresults,
                ResultId = resultid,
                Seed = seed,
                Items = data
            };
        }

        public static JsonResultWithOnlineResultsAndResultIdLowercase<T> GetResultLowercase<T>(
            uint pagenumber, uint totalpages, uint totalcount, int onlineresults,
            string resultid, string seed, IEnumerable<T> data)
            where T : notnull
        {
            return new JsonResultWithOnlineResultsAndResultIdLowercase<T>
            {
                totalResults = totalcount,
                totalPages = totalpages,
                currentPage = pagenumber,
                onlineResults = onlineresults,
                resultId = resultid,
                seed = seed,
                items = data
            };
        }
    }
}
