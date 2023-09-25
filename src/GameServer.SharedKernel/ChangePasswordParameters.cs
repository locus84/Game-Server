using System.ComponentModel.DataAnnotations;

namespace GameServer.SharedKernel;

public class ChangePasswordParameters
{
  [Required]
  public string? Username { get; set; }
  [Required]
  public string? OldPassword { get; set; }
  [Required]
  public string? NewPassword { get; set; }
}
