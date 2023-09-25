
using Ardalis.Result;
using GameServer.SharedKernel.ApiModels;
using MudBlazor;

namespace GameServer.Client.Services.Contracts;

public interface IApiService<T> where T : BaseDTO
{
  Task<Result<ICollection<T>>> ListAsync(string relativeUri);
  Task<Result<ICollection<T>>> ListAsync();
  Task<Result<ICollection<T>>> PageListAsync(int pageIndex, int pageSize, string relativeUri);
  Task<Result<ICollection<T>>> PageListAsync(int pageIndex, int pageSize);
  Task<Result<T>> GetByIdAsync(int id, string relativeUri);
  Task<Result<T>> GetByIdAsync(int id);
  Task<Result<T>> CreateAsync(T dto, string relativeUri);
  Task<Result<T>> CreateAsync(T dto);
  Task<Result> DeleteAsync(int id, string relativeUri);
  Task<Result> DeleteAsync(int id);
  Task<Result> UpdateAsync(T updatedDto, string relativeUri);
  Task<Result> UpdateAsync(T updatedDto);
}
