using Ardalis.Result;
using GameServer.SharedKernel;
using Microsoft.AspNetCore.Components.Forms;

namespace GameServer.Client.Services.Contracts;

public interface IFileService
{
  Task<Result<ICollection<FileObject>>> ListAsync();
  Task<Result> BulkUploadAsync(IBrowserFile[] files);
  Task<Result> BulkDeleteAsync(FileObject[] files);
}
