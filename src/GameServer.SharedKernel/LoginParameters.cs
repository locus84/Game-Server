using System.ComponentModel.DataAnnotations;

namespace GameServer.SharedKernel;

  public class LoginParameters
  {
    public string? UserName { get; set; }
    public string? Mobile { get; set; }

      [Required]
      public string? Password { get; set; }

      public bool RememberMe { get; set; }

      public bool TokenBasedAuth { get; set; } = false;
  }
