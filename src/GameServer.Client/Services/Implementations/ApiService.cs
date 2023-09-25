using System.Net;
using System.Net.Http.Json;
using Ardalis.Result;
using GameServer.Client.Services.Contracts;
using GameServer.SharedKernel.ApiModels;
using MudBlazor;

namespace GameServer.Client.Services.Implementations;

public abstract class ApiService<T> : IApiService<T> where T : BaseDTO
{
  protected readonly HttpClient _httpClient;
  protected readonly string _baseUri;

  protected ApiService(HttpClient httpClient, string baseUri)
  {
    _httpClient = httpClient;
    _baseUri = baseUri;
  }

  public async Task<Result<ICollection<T>>> ListAsync() => await ListAsync("List");
  public virtual async Task<Result<ICollection<T>>> ListAsync(string relativeUri)
  {
    var res = await _httpClient.GetAsync(GetUri(relativeUri));
    return res.IsSuccessStatusCode ? Result.Success((await res.Content.ReadFromJsonAsync<ICollection<T>>())!) : HandleFailure(res);
  }

  public async Task<Result<ICollection<T>>> PageListAsync(int pageIndex, int pageSize) =>
    await PageListAsync(pageIndex, pageSize, "PagedList");
  public async Task<Result<ICollection<T>>> PageListAsync(int pageIndex, int pageSize, string relativeUri)
  {
    var res = await _httpClient.GetAsync(GetUri(relativeUri));

    if (!res.IsSuccessStatusCode) return HandleFailure(res);

    var pagedRes = await res.Content.ReadFromJsonAsync<SharedKernel.PagedResult<T>>();
    return new PagedResult<ICollection<T>>(
      new PagedInfo(pagedRes!.PageNumber, pagedRes.PageSize, pagedRes.TotalPages, pagedRes.TotalRecords),
      pagedRes.Items!);
  }

  public async Task<Result<T>> GetByIdAsync(int id) => await GetByIdAsync(id, "GetById");
  public async Task<Result<T>> GetByIdAsync(int id, string relativeUri)
  {
    var res = await _httpClient.GetAsync(GetUri($"{relativeUri}/{id}"));
    
    return res.IsSuccessStatusCode ? Result.Success((await res.Content.ReadFromJsonAsync<T>())!) : HandleFailure(res);
  }

  public async Task<Result<T>> CreateAsync(T dto) => await CreateAsync(dto,"Create");
  public async Task<Result<T>> CreateAsync(T dto, string relativeUri)
  {
    var res = await _httpClient.PostAsJsonAsync(GetUri(relativeUri), dto);
    return res.IsSuccessStatusCode ? Result.Success((await res.Content.ReadFromJsonAsync<T>())!) : HandleFailure(res);
  }

  public async Task<Result> DeleteAsync(int id) => await DeleteAsync(id, "Delete");
  public async Task<Result> DeleteAsync(int id, string relativeUri)
  {
    var res = await _httpClient.DeleteAsync(GetUri($"{relativeUri}/{id}"));
    return res.IsSuccessStatusCode ? Result.Success() : HandleFailure(res);
  }

  public async Task<Result> UpdateAsync(T updatedDto) => await UpdateAsync(updatedDto, "Update");
  public async Task<Result> UpdateAsync(T updatedDto, string relativeUri)
  {
    var res = await _httpClient.PutAsJsonAsync(GetUri(relativeUri), updatedDto);
    return res.IsSuccessStatusCode ? Result.Success() : HandleFailure(res);
  }

  private string GetUri(string relativeUri) => $"{_baseUri}{relativeUri}";

  protected virtual Result HandleFailure(HttpResponseMessage responseMessage)
  {
    return responseMessage.StatusCode switch
    {
      HttpStatusCode.NotFound => Result.NotFound(),
      HttpStatusCode.Forbidden => Result.Forbidden(),
      HttpStatusCode.Conflict => Result.Conflict(),
      _ => Result.Error(responseMessage.ReasonPhrase)
    };
  }
}
