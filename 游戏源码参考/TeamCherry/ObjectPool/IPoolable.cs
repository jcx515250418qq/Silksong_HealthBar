using UnityEngine;

namespace TeamCherry.ObjectPool
{
	public interface IPoolable<T> where T : Object
	{
		void SetReleaser(IPoolReleaser<T> releaser);

		void Release();

		void OnSpawn();

		void OnRelease();
	}
}
