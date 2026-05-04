using System.Collections.Immutable;
using Flink.SpamProtection.Ports.Repositories;

namespace Flink.SpamProtection.Repositories;

public sealed class BlockedHostInMemoryRepository : IBlockedHostRepository {
  private sealed record DomainNode {
    public bool IsBlocked { get; init; }

    public ImmutableDictionary<string, DomainNode> Children { get; init; }
      = ImmutableDictionary<string, DomainNode>.Empty;
  }

  private volatile DomainNode _root = new();
  
  private int _count;

  public Task<int> CountAsync()
    => Task.FromResult(Volatile.Read(ref _count));

  public Task InsertAsync(IEnumerable<string> hosts) {
    ArgumentNullException.ThrowIfNull(hosts);

    var newRoot = _root;
    var newDistinct = 0;

    foreach (var host in hosts) {
      newRoot = InsertIntoTrie(newRoot, host, out var added);

      if (added)
        newDistinct++;
    }

    _root = newRoot;

    if (newDistinct > 0)
      Interlocked.Add(ref _count, newDistinct);

    return Task.CompletedTask;
  }

  public Task<bool> Contains(string host) {
    if (string.IsNullOrWhiteSpace(host))
      return Task.FromResult(false);

    var parts = host.Split('.', StringSplitOptions.RemoveEmptyEntries);
    Array.Reverse(parts);

    var current = _root;

    foreach (var part in parts) {
      if (current.IsBlocked)
        return Task.FromResult(true);

      if (!current.Children.TryGetValue(part, out current))
        return Task.FromResult(false);
    }

    return Task.FromResult(current.IsBlocked);
  }

  private static DomainNode InsertIntoTrie(DomainNode root, string host, out bool added) {
    var parts = host.Split('.', StringSplitOptions.RemoveEmptyEntries);
    Array.Reverse(parts);

    return InsertRecursive(root, parts, 0, out added);
  }

  private static DomainNode InsertRecursive(
    DomainNode node,
    string[] parts,
    int index,
    out bool added) {
    if (index == parts.Length) {
      added = !node.IsBlocked;

      return node with { IsBlocked = true };
    }

    var part = parts[index];

    node.Children.TryGetValue(part, out var existingChild);

    var child = existingChild ?? new DomainNode();

    var updatedChild = InsertRecursive(child, parts, index + 1, out added);

    var updatedChildren = node.Children.SetItem(part, updatedChild);

    return node with {
      Children = updatedChildren
    };
  }
}
