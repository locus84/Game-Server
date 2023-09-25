using GameServer.Client.Services.Contracts;
using GameServer.SharedKernel.ApiModels;

namespace GameServer.Client.Services.Implementations;

public class InboxService : ApiService<InboxDTO>, IInboxService
{
  public InboxService(HttpClient httpClient) : base(httpClient, "api/Inboxes/")
  {
  }
}
