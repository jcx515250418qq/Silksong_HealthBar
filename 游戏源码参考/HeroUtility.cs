public static class HeroUtility
{
	private static UniqueList<ICancellable> cancellables = new UniqueList<ICancellable>();

	public static void Reset()
	{
		cancellables.FullClear();
	}

	public static void AddCancellable(ICancellable cancellable)
	{
		cancellables.Add(cancellable);
	}

	public static void RemoveCancellable(ICancellable cancellable)
	{
		cancellables.Remove(cancellable);
	}

	public static void CancelCancellables()
	{
		cancellables.ReserveListUsage();
		foreach (ICancellable item in cancellables.List)
		{
			item.DoCancellation();
		}
		cancellables.ReleaseListUsage();
	}
}
