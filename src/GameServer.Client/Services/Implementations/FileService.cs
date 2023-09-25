using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Ardalis.Result;
using GameServer.Client.Services.Contracts;
using GameServer.SharedKernel;
using Microsoft.AspNetCore.Components.Forms;

namespace GameServer.Client.Services.Implementations;

public class FileService : IFileService
{
  private const int MaxAllowedFiles = 10;
  private const long MaxFileSize = 1024 * 1024 * 40; // 40mg
  private readonly HttpClient _httpClient;
  private const string _baseUri = "api/Upload/";

  public FileService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<Result<ICollection<FileObject>>> ListAsync()
  {
    var res = await _httpClient.GetAsync(GetUri("List"));
    return res.IsSuccessStatusCode
      ? Result.Success((await res.Content.ReadFromJsonAsync<ICollection<FileObject>>())!)
      : HandleFailure(res);
  }

  public async Task<Result> BulkUploadAsync(IBrowserFile[] files)
  {
    files = files.Take(MaxAllowedFiles).ToArray();

    var upload = false;

    using var content = new MultipartFormDataContent();

    foreach (var file in files)
    {
      try
      {
        var fileContent =
          new StreamContent(file.OpenReadStream(MaxFileSize));

        fileContent.Headers.ContentType =
          new MediaTypeHeaderValue(file.ContentType);

        content.Add(
          content: fileContent,
          name: "\"files\"",
          fileName: file.Name);

        upload = true;
      }
      catch (Exception ex)
      {
        return Result.Error(ex.Message);
      }
    }

    if (upload)
    {
      var res = await _httpClient.PostAsync(GetUri("BulkUpload"), content);

      return res.IsSuccessStatusCode ? Result.Success() : HandleFailure(res);
    }

    return Result.NotFound();
  }

  public async Task<Result> BulkDeleteAsync(FileObject[] files)
  {
    var res = await _httpClient.PostAsJsonAsync(GetUri("BulkDelete"), files.Select(x=>x.FilePath));

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
