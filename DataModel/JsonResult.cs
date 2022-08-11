using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DataModel
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

    public class JsonResultWithBookingInfo<T> : JsonResultWithOnlineResultsAndResultId<T>
    {
        public int AvailableOnline { get; set; }
        public int AvailableOnRequest { get; set; }
        public int AccommodationsRequested { get; set; }
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

    public class SearchResult<T> : IResponse<T>
    {
        public uint totalResults { get; set; }
        public string? searchTerm { get; set; }
        public Dictionary<string, uint> detailedResults { get; set; } = new Dictionary<string, uint>();
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    }
}
