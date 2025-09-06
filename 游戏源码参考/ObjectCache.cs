public sealed class ObjectCache<T>
{
	private T cache;

	public T Value => cache;

	public int Version { get; private set; }

	public void UpdateCache(T update, int version)
	{
		cache = update;
		Version = version;
	}

	public bool ShouldUpdate(int version)
	{
		return Version != version;
	}
}
