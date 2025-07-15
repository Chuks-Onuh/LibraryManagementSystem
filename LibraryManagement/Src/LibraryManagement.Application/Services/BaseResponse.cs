public class BaseResponse<T>
{
    public T Data { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class PaginatedResponse<T> : BaseResponse<T>
{
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public PaginatedResponse(T data, int totalCount, int pageNumber, int pageSize, bool success = true, string message = "")
    {
        Data = data;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        Success = success;
        Message = message;
    }
}
