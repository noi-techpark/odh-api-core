using System;
using System.Collections.Generic;

namespace Helper
{
    public class JsonResult<T> where T : notnull
    {
        public uint TotalResults { get; }
        public uint TotalPages { get; }
        public uint CurrentPage { get; }
        public int? OnlineResults { get; }
        public string? ResultId { get; }
        public string? Seed { get; }
        public IEnumerable<T> Items { get; }

        public JsonResult(uint totalResults, uint totalPages, uint currentPage, int? onlineResults, string? resultId, string? seed, IEnumerable<T> items)
        {
            TotalResults = totalResults;
            TotalPages = totalPages;
            CurrentPage = currentPage;
            OnlineResults = onlineResults;
            ResultId = resultId;
            Seed = seed;
            Items = items;
        }

        public JsonResult(uint totalResults, uint totalPages, uint currentPage, string? seed, IEnumerable<T> items)
            : this(totalResults, totalPages, currentPage, null, null, seed, items)
        { }

        public override bool Equals(object? obj)
        {
            return obj is JsonResult<T> other &&
                   TotalResults == other.TotalResults &&
                   TotalPages == other.TotalPages &&
                   CurrentPage == other.CurrentPage &&
                   OnlineResults == other.OnlineResults &&
                   ResultId == other.ResultId &&
                   Seed == other.Seed &&
                   EqualityComparer<IEnumerable<T>>.Default.Equals(Items, other.Items);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TotalResults, TotalPages, CurrentPage, OnlineResults, ResultId, Seed, Items);
        }
    }
}
