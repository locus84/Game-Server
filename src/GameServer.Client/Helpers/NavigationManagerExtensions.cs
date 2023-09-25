using Microsoft.AspNetCore.Components;

namespace GameServer.Client.Helpers;

public static class NavigationManagerExtensions
{
  public static string Page(this NavigationManager navigationManager) =>
    navigationManager.Uri.Substring(navigationManager.BaseUri.Length);
}
