
namespace GameServer.SharedKernel.Interfaces;

public interface IDomainEventDispatcher
{
  Task DispatchAndClearEvents(IEnumerable<IEntity> entitiesWithEvents);
}
