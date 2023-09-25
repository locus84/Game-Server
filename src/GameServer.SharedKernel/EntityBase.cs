using System.ComponentModel.DataAnnotations.Schema;
using GameServer.SharedKernel.Interfaces;

namespace GameServer.SharedKernel;
// This can be modified to EntityBase<TId> to support multiple key types (e.g. Guid)
public abstract class EntityBase : IEntity
{
  public int Id { get; set; }

  private List<DomainEventBase> _domainEvents = new();
  [NotMapped]
  public IEnumerable<DomainEventBase> DomainEvents => _domainEvents.AsReadOnly();

  protected void RegisterDomainEvent(DomainEventBase domainEvent) => _domainEvents.Add(domainEvent);
  public void ClearDomainEvents() => _domainEvents.Clear();
}
