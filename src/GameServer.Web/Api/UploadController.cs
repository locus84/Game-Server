using GameServer.SharedKernel;
using Microsoft.AspNetCore.Mvc;

namespace GameServer.Web.Api;

public class UploadController : BaseApiController
{
  private readonly string _uploadsFolder;
  private readonly IWebHostEnvironment _environment;

  public UploadController(IWebHostEnvironment environment)
  {
    _environment = environment;
    _uploadsFolder = Path.Combine(environment.WebRootPath, "uploads");
  }

  [HttpGet]
  public async Task<IActionResult> List()
  {
    var files = await Task.Run(() => Directory.EnumerateFileSystemEntries(_uploadsFolder).Where(x => x.Contains("."))
      .Select(x => new FileObject
      {
        FileName = Path.GetFileName(x), FilePath = x.Replace(_environment.WebRootPath, string.Empty)
      }).ToArray());

    return Ok(files);
  }

  [HttpPost]
  public async Task<IActionResult> BulkUpload(IFormFile[] files)
  {
    try
    {
      if (HttpContext.Request.Form.Files.Any())
      {
        foreach (var file in HttpContext.Request.Form.Files)
        {
          var dir = Path.Combine(_environment.WebRootPath, "uploads");
          var path = Path.Combine(dir, file.FileName);
          if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

          using var stream = new FileStream(path, FileMode.Create);
          await file.CopyToAsync(stream);
        }
      }

      return Ok();
    }
    catch (Exception ex)
    {
      return BadRequest(ex);
    }
  }

  [HttpPost]
  public async Task<IActionResult> BulkDelete([FromBody] string[] files)
  {
    try
    {
      foreach (var file in files)
      {
        var path = _environment.WebRootPath + file;
        await Task.Run(() =>
        {
          if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        });
      }
    }
    catch (Exception ex)
    {
      return BadRequest(ex);
    }

    return Ok();
  }
}
