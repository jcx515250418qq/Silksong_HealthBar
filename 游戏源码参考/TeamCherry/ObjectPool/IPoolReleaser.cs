namespace TeamCherry.ObjectPool
{
	public interface IPoolReleaser<T>
	{
		void Release(T element);
	}
}
