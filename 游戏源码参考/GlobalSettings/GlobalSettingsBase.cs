using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GlobalSettings
{
	public abstract class GlobalSettingsBase<T> : ScriptableObject where T : GlobalSettingsBase<T>
	{
		private static bool _foundInstance;

		private static T _instance;

		private static AsyncOperationHandle<T>? _loadHandle;

		private static int _orderHandle;

		private static (GlobalSettings Runner, Coroutine Routine) _delayedLoader;

		protected static T Get(string fileName)
		{
			if (!_foundInstance)
			{
				if (_delayedLoader.Routine != null)
				{
					if ((bool)_delayedLoader.Runner)
					{
						_delayedLoader.Runner.StopCoroutine(_delayedLoader.Routine);
						Object.Destroy(_delayedLoader.Runner.gameObject);
					}
					_delayedLoader.Routine = null;
					_delayedLoader.Runner = null;
				}
				if (!_loadHandle.HasValue)
				{
					_loadHandle = Addressables.LoadAssetAsync<T>("GlobalSettings/" + fileName + ".asset");
					AsyncLoadOrderingManager.OnStartedLoad(_loadHandle.Value, out _orderHandle);
					AsyncOperationHandle<T> value = _loadHandle.Value;
					value.Completed += delegate(AsyncOperationHandle<T> handle)
					{
						AsyncLoadOrderingManager.OnCompletedLoad(handle, _orderHandle);
					};
				}
				AsyncLoadOrderingManager.CompleteUpTo(_loadHandle.Value, _orderHandle);
				_instance = _loadHandle.Value.WaitForCompletion();
				if (!_instance)
				{
					_instance = ScriptableObject.CreateInstance<T>();
				}
				_foundInstance = true;
			}
			return _instance;
		}

		protected static void StartPreloadAddressable(string fileName)
		{
			if (!_loadHandle.HasValue && _delayedLoader.Routine == null)
			{
				_delayedLoader = GlobalSettings.StartLoad(fileName, delegate(AsyncOperationHandle<T>? value)
				{
					_loadHandle = value;
				}, delegate(T value)
				{
					_instance = value;
					_foundInstance = true;
					_delayedLoader.Routine = null;
					_delayedLoader.Runner = null;
				});
			}
		}

		protected static void StartUnload()
		{
			if (_loadHandle.HasValue)
			{
				_loadHandle.Value.Release();
				_loadHandle = null;
				_foundInstance = false;
				_instance = null;
			}
		}

		private void OnDestroy()
		{
			if (_instance == this)
			{
				_foundInstance = false;
				_instance = null;
			}
		}
	}
}
