using Ardalis.Specification;

namespace GameServer.Core.PlayerAggregate.Specifications;

public class FriendRequestsByIds : Specification<FriendRequest>
{
  public FriendRequestsByIds(int? fromId = null, int? toId = null)
  {
    if (fromId.HasValue && toId.HasValue)
      Query.Where(x => x.FromPlayerId == fromId && x.ToPlayerId == toId);
    else if (fromId.HasValue)
      Query.Where(x => x.FromPlayerId == fromId);
    else
      Query.Where(x => x.ToPlayerId == toId);
  }
}
