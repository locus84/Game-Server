using System.Collections.Concurrent;

namespace GameServer.Web.Hubs;

public sealed class GroupMapping<TKey> where TKey : class
{
  private readonly ConcurrentDictionary<TKey, HashSet<string>> _groups = new();

  public int Count => _groups.Values.Distinct().Count();

  public IEnumerable<string> GetGroups(TKey key)
  {
    HashSet<string>? groups;
    return _groups.TryGetValue(key, out groups) ? groups : Enumerable.Empty<string>();
  }

  public void Add(TKey key, string groupId)
  {
    HashSet<string>? groups;
    if (!_groups.TryGetValue(key, out groups))
    {
      groups = new HashSet<string>();
      _groups[key] = groups;
    }
    groups.Add(groupId);
  }

  public void Remove(TKey key, string groupId)
  {
    HashSet<string>? groups;
    if (_groups.TryGetValue(key, out groups))
    {
      groups.Remove(groupId);

      if (groups.Count == 0)
      {
        HashSet<string>? gToRemove;
        _groups.TryRemove(key, out gToRemove);
      }
    }
  }

  public bool HasKey(TKey key) => _groups.ContainsKey(key);
}
