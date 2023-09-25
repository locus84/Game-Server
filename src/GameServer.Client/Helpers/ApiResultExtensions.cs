using Ardalis.Result;
using MudBlazor;

namespace GameServer.Client.Helpers;

public static class ApiResultExtensions
{
  public static void ShowMessageOnFailure(this IResult result, IDialogService dialogService)
  {
    if (result.Status != ResultStatus.Ok)
    {
      dialogService.ShowMessageBox(result.Status.ToString(),
        $"Error on get/post data\r\n {string.Join("\r\n", result.Errors)}");
    }
  }
}
