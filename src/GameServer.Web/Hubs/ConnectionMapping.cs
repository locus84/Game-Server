namespace GameServer.Web.Hubs;

public sealed class ConnectionMapping<TKey> where TKey : notnull
{
  private readonly Dictionary<TKey, HashSet<string>> _connections = new();

  public int Count => _connections.Values.Distinct().Count();

  public IEnumerable<string> GetConnections(TKey key)
  {
    HashSet<string>? connections;
    if (_connections.TryGetValue(key, out connections))
    {
      return connections;
    }

    return Enumerable.Empty<string>();
  }

  public void Add(TKey key, string connectionId)
  {
    lock (_connections)
    {
      HashSet<string>? connections;
      if (!_connections.TryGetValue(key, out connections))
      {
        connections = new HashSet<string>();
        _connections[key] = connections;
      }

      lock (connections)
      {
        connections.Add(connectionId);
      }
    }
  }

  public void Remove(TKey key, string connectionId)
  {
    lock (_connections)
    {
      HashSet<string>? connections;
      if (_connections.TryGetValue(key, out connections))
      {
        lock (connections)
        {
          connections.Remove(connectionId);

          if (connections.Count == 0)
            _connections.Remove(key);
        }
      }
    }
  }

  public bool HasKey(TKey key) => _connections.ContainsKey(key);
}
