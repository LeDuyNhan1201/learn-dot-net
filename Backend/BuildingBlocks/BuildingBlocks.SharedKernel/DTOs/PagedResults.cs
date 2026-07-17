namespace BuildingBlocks.SharedKernel.DTOs;

public static class PagedResults
{
    public sealed class Offset<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public int Total { get; init; }
        public int Page { get; init; }
        public int Size { get; init; }
        public int TotalPages => Size <= 0 ? 0 : (int)Math.Ceiling((double)Total / Size);
    }
}