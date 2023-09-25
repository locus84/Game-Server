namespace GameServer.SharedKernel;

public class PagedResult<T> where T : class
{
  public int TotalRecords { get; set; }
  public int PageSize { get; set; }
  public int PageNumber { get; set; }
  public int TotalPages => TotalRecords / PageSize;

  public ICollection<T>? Items { get; set; }
}
