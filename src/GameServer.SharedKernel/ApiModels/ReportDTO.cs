using GameServer.SharedKernel;
using GameServer.SharedKernel.ApiModels;
using GameServer.SharedKernel.Interfaces;

public class ReportDTO : BaseDTO
{
  public int ReporterId { get; set; }
  public int ReportedId { get; set; }
}
