namespace Flink.Shared.DataStructures;

public sealed class ConcurrentHashSet<T>(int expectedCount, IEqualityComparer<T> comparer) {
  private readonly HashSet<T> _set = new(expectedCount, comparer);
  private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);
  
  public bool Contains(T item) {
    _lock.EnterReadLock();
    
    try {
      return _set.Contains(item);
    }
    finally {
      _lock.ExitReadLock();
    }
  }

  public void Insert(IEnumerable<T> items) {
    _lock.EnterWriteLock();
    try {
      foreach (var item in items)
        _set.Add(item);
    }
    finally {
      _lock.ExitWriteLock();
    }
  }

  public int Count() {
    _lock.EnterReadLock();
    try {
      return _set.Count;
    }
    finally {
      _lock.ExitReadLock();
    }
  }
}
