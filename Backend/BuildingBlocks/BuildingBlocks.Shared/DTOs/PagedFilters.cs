namespace BuildingBlocks.Shared.DTOs;

public static class PagedFilters
{
    public enum SortDirection
    {
        Asc,
        Desc
    }
    
    public enum CursorDirection
    {
        Forward,
        Backward
    }
}