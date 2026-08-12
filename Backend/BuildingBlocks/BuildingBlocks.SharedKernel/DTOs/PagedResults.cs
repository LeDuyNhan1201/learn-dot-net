using System.Text.Json.Serialization;

namespace BuildingBlocks.SharedKernel.DTOs;

public static class PagedResults
{
    public sealed class Offset<T>
    {
        [JsonPropertyName("items")]
        public IReadOnlyList<T> Items { get; init; } = [];
        
        [JsonPropertyName("total")]
        public int Total { get; init; }

        [JsonPropertyName("page")]
        public int Page { get; init; }
        
        [JsonPropertyName("size")]
        public int Size { get; init; }
        
        [JsonPropertyName("totalPages")]
        public int TotalPages => Size <= 0 ? 0 : (int)Math.Ceiling((double)Total / Size);
    }
}