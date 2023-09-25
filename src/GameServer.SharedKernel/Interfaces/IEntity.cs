namespace GameServer.SharedKernel.Interfaces;

public interface IEntity
{
  IEnumerable<DomainEventBase> DomainEvents { get; } 
  void ClearDomainEvents();
}
