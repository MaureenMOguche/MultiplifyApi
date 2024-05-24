namespace Multiplify.Application.Responses;
public static class QueryableExtension
{
    public static Paginated<T> Paginate<T>(this IEnumerable<T> query, int page = 1, int pageSize = 15, int? totalCount = null)
    {
        IEnumerable<T> data = query.Skip((page - 1) * pageSize).Take(pageSize);
        int totalRecords = totalCount != null ? (int)totalCount : query.Count();
        return new Paginated<T>(data, totalRecords, page, pageSize);
    }

    public static Paginated<T> Paginate<T>(this IQueryable<T> query, int page = 1, int pageSize = 15, int? totalCount = null)
    {
        IEnumerable<T> data = query.Skip((page - 1) * pageSize).Take(pageSize);
        int totalRecords = totalCount != null ? (int)totalCount : query.Count();
        return new Paginated<T>(data, totalRecords, page, pageSize);
    }
}


public class Paginated<T>
{
    public IEnumerable<T> Data { get; set; }

    public int Page { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }

    public bool HasPreviousPage { get; set; }

    public bool HasNextPage { get; set; }

    public Paginated(IEnumerable<T> data, int totalRecords, int page, int pageSize)
    {
        Data = data;
        TotalCount = totalRecords;
        Page = page;
        TotalPages = (int)Math.Ceiling((double)totalRecords / (double)pageSize);
        HasNextPage = Page < TotalPages;
        HasPreviousPage = Page > 1;
    }
}